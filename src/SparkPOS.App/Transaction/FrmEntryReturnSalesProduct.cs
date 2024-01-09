/**
 * Copyright (C) 2017  (http://coding4ever.net/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * The latest version of this file can be found at https://github.com/rudi-krsoftware/spark-pos
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SparkPOS.Helper;
using SparkPOS.Helper.UI.Template;
using SparkPOS.App.Lookup;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Reference;
using ConceptCave.WaitCursor;
using log4net;

namespace SparkPOS.App.Transactions
{
    public partial class FrmEntryReturnSalesProduct : FrmEntryStandard, IListener
    {                
        private IReturnSellingProductBll _bll = null;
        private ReturnSellingProduct _return = null;
        private Customer _customer = null;
        private SellingProduct _jual = null;
        private IList<ItemReturnSellingProduct> _listOfItemReturn = new List<ItemReturnSellingProduct>();
        private IList<ItemReturnSellingProduct> _listOfItemReturnOld = new List<ItemReturnSellingProduct>();
        private IList<ItemReturnSellingProduct> _listOfItemReturnDeleted = new List<ItemReturnSellingProduct>();
        
        private int _rowIndex = 0;
        private int _colIndex = 0;        

        private bool _isNewData = false;
        private bool _isValidJumlahReturn = false;
        private bool _isValidCodeProduct = false;

        private ILog _log;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntryReturnSalesProduct(string header, IReturnSellingProductBll bll) 
            : base()
        {            
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this._isNewData = true;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = bll.GetLastInvoice();
            dtpDate.Value = DateTime.Today;

            _listOfItemReturn.Add(new ItemReturnSellingProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryReturnSalesProduct(string header, ReturnSellingProduct retur, IReturnSellingProductBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._return = retur;
            this._customer = retur.Customer;
            this._jual = retur.SellingProduct;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = this._return.invoice;
            txtInvoice.Enabled = false;

            dtpDate.Value = (DateTime)this._return.date;
            txtCustomer.Text = this._customer.name_customer;            
            txtKeterangan.Text = this._return.description;

            if (this._jual != null)
            {
                txtInvoiceSelling.Text = this._jual.invoice;
                txtInvoiceSelling.Enabled = false;

                LoadItemSelling(this._jual);
            }                

            // save data lama
            _listOfItemReturnOld.Clear();
            foreach (var item in this._return.item_return)
            {                
                _listOfItemReturnOld.Add(new ItemReturnSellingProduct
                {
                    return_sale_item_id = item.return_sale_item_id,
                    return_quantity = item.return_quantity,
                    selling_price = item.selling_price
                });
            }
            
            _listOfItemReturn = this._return.item_return;
            _listOfItemReturn.Add(new ItemReturnSellingProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
            RefreshTotal();
        }

        private void LoadItemSelling(SellingProduct jualProduct)
        {
            ISellingProductBll bll = new SellingProductBll(_log);
            _jual.item_jual = bll.GetItemSelling(_jual.sale_id).ToList();
        }

        private void InitGridControl(GridControl grid)
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 240 });
            gridListProperties.Add(new GridListControlProperties { Header = "quantity Return", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Sub Total", Width = 110 });
            gridListProperties.Add(new GridListControlProperties { Header = "Action" });

            GridListControlHelper.InitializeGridListControl<ItemReturnSellingProduct>(grid, _listOfItemReturn, gridListProperties, 30);

            grid.PushButtonClick += delegate(object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.ColIndex == 7)
                {
                    if (grid.RowCount == 1)
                    {
                        MsgHelper.MsgWarning("Minimum 1 product must entered !");
                        return;
                    }

                    if (MsgHelper.MsgDelete())
                    {
                        var itemReturn = _listOfItemReturn[e.RowIndex - 1];
                        itemReturn.entity_state = EntityState.Deleted;

                        _listOfItemReturnDeleted.Add(itemReturn);
                        _listOfItemReturn.Remove(itemReturn);

                        grid.RowCount = _listOfItemReturn.Count();
                        grid.Refresh();

                        RefreshTotal();
                    }                    
                }
            };

            grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                // Make sure the cell falls inside the grid
                if (e.RowIndex > 0)
                {
                    if (!(_listOfItemReturn.Count > 0))
                        return;

                    var itemReturn = _listOfItemReturn[e.RowIndex - 1];
                    var product = itemReturn.Product;

                    if (e.RowIndex % 2 == 0)
                        e.Style.BackColor = ColorCollection.BACK_COLOR_ALTERNATE;

                    switch (e.ColIndex)
                    {
                        case 1: // no urut
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.Enabled = false;
                            e.Style.CellValue = e.RowIndex.ToString();
                            break;

                        case 2:
                            if (product != null)
                                e.Style.CellValue = product.product_code;

                            break;

                        case 3: // name product
                            if (product != null)
                                e.Style.CellValue = product.product_name;

                            break;

                        case 4: // quantity return
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemReturn.return_quantity;

                            break;

                        case 5: // price
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.CellValue = NumberHelper.NumberToString(itemReturn.selling_price);

                            break;

                        case 6: // subtotal
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.Enabled = false;

                            e.Style.CellValue = NumberHelper.NumberToString(itemReturn.return_quantity * itemReturn.selling_price);
                            break;

                        case 7: // button hapus
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellType = GridCellTypeName.PushButton;
                            e.Style.Enabled = true;
                            if (MainProgram.currentLanguage == "en-US")
                            {
                                e.Style.Description = "Delete";
                            }
                            else if (MainProgram.currentLanguage == "ar-SA")
                            {
                                e.Style.Description = "يمسح";
                            }
                            break;

                        default:
                            break;
                    }

                    e.Handled = true; // we handled it, let the grid know
                }
            };

            var colIndex = 2; // column name product
            grid.CurrentCell.MoveTo(1, colIndex, GridSetCurrentCellOptions.BeginEndUpdate);
        }

        private double SumGrid(IList<ItemReturnSellingProduct> listOfItemReturn)
        {
            double total = 0;
            foreach (var item in _listOfItemReturn.Where(f => f.Product != null))
            {
                total += item.selling_price * item.return_quantity;
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemReturn));
        }

        protected override void Save()
        {
            if (this._customer == null || txtCustomer.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Customer' should not empty !");
                txtCustomer.Focus();

                return;
            }

            var total = SumGrid(this._listOfItemReturn);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You haven't completed the product data input yet!");
                return;
            }

            if (!MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
                return;

            if (_isNewData)
                _return = new ReturnSellingProduct();
            
            _return.sale_id = this._jual.sale_id;
            _return.SellingProduct = this._jual;
            _return.user_id = this._user.user_id;
            _return.User = this._user;
            _return.customer_id = this._customer.customer_id;
            _return.Customer = this._customer;
            _return.invoice = txtInvoice.Text;
            _return.date = dtpDate.Value;
            _return.description = txtKeterangan.Text;

            _return.item_return = this._listOfItemReturn.Where(f => f.Product != null).ToList();

            if (!_isNewData) // update
                _return.item_return_deleted = _listOfItemReturnDeleted;

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                {
                    result = _bll.Save(_return, ref validationError);
                }
                else
                {
                    result = _bll.Update(_return, ref validationError);
                }

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _return);

                    _customer = null;
                    _listOfItemReturn.Clear();
                    _listOfItemReturnDeleted.Clear();                                        

                    this.Close();
                }
                else
                {
                    if (validationError.Message.NullToString().Length > 0)
                    {
                        MsgHelper.MsgWarning(validationError.Message);
                        base.SetFocusObject(validationError.PropertyName, this);
                    }
                    else
                        MsgHelper.MsgUpdateError();
                }
            }            
        }

        protected override void Cancel()
        {
            // restore data lama
            if (!_isNewData)
            {
                // restore item yang di edit
                var itemsModified = _return.item_return.Where(f => f.Product != null && f.entity_state == EntityState.Modified)
                                                     .ToArray();

                foreach (var item in itemsModified)
                {
                    var itemReturn = _listOfItemReturnOld.Where(f => f.return_sale_item_id == item.return_sale_item_id)
                                                       .SingleOrDefault();

                    if (itemReturn != null)
                    {
                        item.return_quantity = itemReturn.return_quantity;
                        item.selling_price = itemReturn.selling_price;
                    }
                }

                // restore item yang di delete
                var itemsDeleted = _listOfItemReturnDeleted.Where(f => f.Product != null && f.entity_state == EntityState.Deleted)
                                                          .ToArray();
                foreach (var item in itemsDeleted)
                {
                    item.entity_state = EntityState.Unchanged;
                    this._return.item_return.Add(item);
                }

                _listOfItemReturnDeleted.Clear();
            }

            base.Cancel();
        }

        public void Ok(object sender, object data)
        {
            if (data is ItemSellingProduct) // pencarian product baku
            {
                var itemSelling = (ItemSellingProduct)data;
                var product = itemSelling.Product;

                if (!IsExist(product.product_id))
                {
                    SetItemProduct(this.gridControl, _rowIndex, itemSelling, itemSelling.quantity - itemSelling.return_quantity, itemSelling.selling_price);
                    this.gridControl.Refresh();
                    RefreshTotal();

                    GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, _colIndex + 1);

                    RefreshTotal();
                }
                else
                {
                    MsgHelper.MsgWarning("Data product already entered");
                    GridListControlHelper.SelectCellText(this.gridControl, _rowIndex, _colIndex);
                }
            }
            else if (data is Customer) // pencarian customer
            {
                this._customer = (Customer)data;
                txtCustomer.Text = this._customer.name_customer;
                KeyPressHelper.NextFocus();
            }
            else if (data is SellingProduct) // pencarian data sale
            {
                ISellingProductBll bll = new SellingProductBll(_log);

                this._jual = (SellingProduct)data;
                this._jual.item_jual = bll.GetItemSelling(this._jual.sale_id).ToList();
                txtInvoiceSelling.Text = this._jual.invoice;

                KeyPressHelper.NextFocus();
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            // do nothing
        }

        private void txtKeterangan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                gridControl.Focus();
                GridListControlHelper.SetCurrentCell(gridControl, 1, 2); // fokus ke column name product
            }
        }

        private void txtCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var name = ((TextBox)sender).Text;

                ICustomerBll bll = new CustomerBll(_log);
                var listOfCustomer = bll.GetByName(name);

                if (listOfCustomer.Count == 0)
                {
                    MsgHelper.MsgWarning("Data Customer not found");
                    txtCustomer.Focus();
                    txtCustomer.SelectAll();

                }
                else if (listOfCustomer.Count == 1)
                {
                    _customer = listOfCustomer[0];
                    txtCustomer.Text = _customer.name_customer;
                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Data Customer", listOfCustomer);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }

        private bool IsExist(string produkId)
        {
            var count = _listOfItemReturn.Where(f => f.product_id != null && f.product_id.ToLower() == produkId.ToLower())
                                        .Count();

            return (count > 0);
        }

        private void gridControl_CurrentCellKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (this._jual == null || txtInvoiceSelling.Text.Length == 0)
                {
                    MsgHelper.MsgWarning("Sorry, the data entry is incomplete!");
                    txtInvoiceSelling.Focus();

                    return;
                }

                var grid = (GridControl)sender;

                var rowIndex = grid.CurrentCell.RowIndex;
                var colIndex = grid.CurrentCell.ColIndex;

                GridCurrentCell cc;

                ItemSellingProduct itemSelling;

                switch (colIndex)
                {
                    case 2: // code product
                        _isValidCodeProduct = false;

                        cc = grid.CurrentCell;
                        var codeProduct = cc.Renderer.ControlValue.ToString();

                        if (codeProduct.Length == 0)
                        {
                            GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                        }
                        else
                        {
                            itemSelling = this._jual.item_jual.Where(f => f.Product.product_code.ToLower() == codeProduct.ToLower() && f.quantity > f.return_quantity)
                                                           .SingleOrDefault();

                            if (itemSelling == null)
                            {
                                MsgHelper.MsgWarning("Product data not found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                return;
                            }

                            _isValidCodeProduct = true;

                            if (!IsExist(itemSelling.product_id))
                            {
                                SetItemProduct(grid, rowIndex, itemSelling, itemSelling.quantity - itemSelling.return_quantity, itemSelling.selling_price);
                                grid.Refresh();

                                RefreshTotal();

                                GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 2);
                            }
                            else
                            {
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 2);
                            }
                        }

                        break;

                    case 3: // name product

                        cc = grid.CurrentCell;
                        var nameProduct = cc.Renderer.ControlValue.ToString();

                        if (!_isValidCodeProduct)
                        {
                            var listOfItemSelling = this._jual.item_jual.Where(f => f.Product.product_name.ToLower().Contains(nameProduct.ToLower()) && f.quantity > f.return_quantity)
                                                                     .ToList();

                            if (listOfItemSelling.Count == 0)
                            {
                                MsgHelper.MsgWarning("Product data not found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                            }
                            else if (listOfItemSelling.Count == 1)
                            {
                                itemSelling = listOfItemSelling[0];

                                if (!IsExist(itemSelling.product_id))
                                {
                                    SetItemProduct(grid, rowIndex, itemSelling, itemSelling.quantity - itemSelling.return_quantity, itemSelling.selling_price);
                                    grid.Refresh();

                                    GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                                }
                                else
                                {
                                    MsgHelper.MsgWarning("Data product already entered");
                                    GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                }
                            }
                            else // data lebih dari one
                            {
                                _rowIndex = rowIndex;
                                _colIndex = colIndex;

                                var frmLookup = new FrmLookupItemInvoice("Item Sales", listOfItemSelling);
                                frmLookup.Listener = this;
                                frmLookup.ShowDialog();
                            }
                        }                        

                        break;

                    case 4:
                        _isValidJumlahReturn = false;

                        try
                        {
                            cc = grid.CurrentCell;
                            double jumlahReturn = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);

                            var jumlahSelling = _listOfItemReturn[rowIndex - 1].quantity;
                            if (jumlahReturn <= jumlahSelling)
                            {
                                _isValidJumlahReturn = true;
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                            }
                            else
                                MsgHelper.MsgWarning("Sorry, quantity return should not exceeds  quantity sale");
                        }
                        catch (Exception ex)
                        {
                            MainProgram.LogException(ex);
                            // Error handling and logging
                            var msg = MainProgram.GlobalWarningMessage();
                            MsgHelper.MsgWarning(msg);
                            //WarningMessageHandler.ShowTranslatedWarning(msg, MainProgram.currentLanguage);
                        }

                        break;

                    case 5:
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemReturn.Add(new ItemReturnSellingProduct());
                            grid.RowCount = _listOfItemReturn.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, rowIndex + 1, 2); // fokus ke column name product
                        break;

                    default:
                        break;
                }
            }
        }

        private void gridControl_CurrentCellKeyPress(object sender, KeyPressEventArgs e)
        {
            var grid = (GridControl)sender;
            GridCurrentCell cc = grid.CurrentCell;

            // validasi input angka untuk column quantity return dan price
            switch (cc.ColIndex)
            {
                case 4: // quantity return
                case 5: // price
                    e.Handled = KeyPressHelper.NumericOnly(e);
                    break;

                default:
                    break;
            }
        }

        private void SetItemProduct(GridControl grid, int rowIndex, ItemSellingProduct itemSelling, double jumlahReturn = 0, double price = 0)
        {
            ItemReturnSellingProduct itemReturn;

            if (_isNewData)
            {
                itemReturn = new ItemReturnSellingProduct();
            }
            else
            {
                itemReturn = _listOfItemReturn[rowIndex - 1];

                if (itemReturn.entity_state == EntityState.Unchanged)
                    itemReturn.entity_state = EntityState.Modified;
            }

            var product = itemSelling.Product;

            itemReturn.sale_item_id = itemSelling.sale_item_id;
            itemReturn.product_id = product.product_id;
            itemReturn.Product = product;

            itemReturn.quantity = itemSelling.quantity;
            itemReturn.return_quantity = jumlahReturn;
            itemReturn.selling_price = price;

            _listOfItemReturn[rowIndex - 1] = itemReturn;

            itemSelling.return_quantity = jumlahReturn;
        }

        private void gridControl_CurrentCellValidated(object sender, EventArgs e)
        {
            var grid = (GridControl)sender;

            GridCurrentCell cc = grid.CurrentCell;

            if (this._jual != null)
            {
                var obj = _listOfItemReturn[cc.RowIndex - 1];
                var itemSelling = this._jual.item_jual.Where(f => f.product_id == obj.product_id)
                                                  .SingleOrDefault();

                if (itemSelling != null)
                {
                    switch (cc.ColIndex)
                    {
                        case 4:
                            if (_isValidJumlahReturn)
                                obj.return_quantity = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);

                            break;

                        case 5:
                            obj.selling_price = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                            break;

                        default:
                            break;
                    }

                    SetItemProduct(grid, cc.RowIndex, itemSelling, obj.return_quantity, obj.selling_price);
                    grid.Refresh();

                    RefreshTotal();
                }
            }           
        }

        private void FrmEntryReturnSalesProduct_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hapus objek dumm
            if (!_isNewData)
            {
                var itemsToRemove = _return.item_return.Where(f => f.Product == null && f.entity_state == EntityState.Added)
                                                     .ToArray();

                foreach (var item in itemsToRemove)
                {
                    _return.item_return.Remove(item);
                }
            }
        }

        private void txtInvoiceSelling_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                if (this._customer == null || txtCustomer.Text.Length == 0)
                {
                    MsgHelper.MsgWarning("Sorry, the data entry is incomplete!");
                    txtCustomer.Focus();

                    return;
                }

                var invoice = ((TextBox)sender).Text;

                ISellingProductBll bll = new SellingProductBll(_log);
                var listOfSelling = bll.GetInvoiceCustomer(this._customer.customer_id, invoice);

                if (listOfSelling.Count == 0)
                {
                    MsgHelper.MsgWarning("Data invoice sale not found");
                    txtInvoiceSelling.Focus();
                    txtInvoiceSelling.SelectAll();
                }
                else if (listOfSelling.Count == 1)
                {
                    _jual = listOfSelling[0];
                    _jual.item_jual = bll.GetItemSelling(_jual.sale_id).ToList();

                    txtInvoiceSelling.Text = _jual.invoice;
                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupInvoice("Data Invoice Sales", listOfSelling);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }

        private void FrmEntryReturnSalesProduct_Load(object sender, EventArgs e)
        {

        }
    }
}
