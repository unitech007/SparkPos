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
    public partial class FrmEntryReturnProductPurchase : FrmEntryStandard, IListener
    {        
        private IReturnPurchaseProductBll _bll = null;
        private ReturnPurchaseProduct _return = null;
        private Supplier _supplier = null;
        private PurchaseProduct _beli = null;
        private IList<ItemReturnPurchaseProduct> _listOfItemReturn = new List<ItemReturnPurchaseProduct>();
        private IList<ItemReturnPurchaseProduct> _listOfItemReturnOld = new List<ItemReturnPurchaseProduct>();
        private IList<ItemReturnPurchaseProduct> _listOfItemReturnDeleted = new List<ItemReturnPurchaseProduct>();

        private int _rowIndex = 0;
        private int _colIndex = 0;        

        private bool _isNewData = false;
        private bool _isValidJumlahReturn = false;
        private bool _isValidCodeProduct = false;

        private ILog _log;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntryReturnProductPurchase(string header, IReturnPurchaseProductBll bll) 
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

            _listOfItemReturn.Add(new ItemReturnPurchaseProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryReturnProductPurchase(string header, ReturnPurchaseProduct retur, IReturnPurchaseProductBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._return = retur;
            this._supplier = retur.Supplier;
            this._beli = retur.PurchaseProduct;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = this._return.invoice;
            txtInvoice.Enabled = false;

            dtpDate.Value = (DateTime)this._return.date;
            txtSupplier.Text = this._supplier.name_supplier;            
            txtKeterangan.Text = this._return.description;

            if (this._beli != null)
            {
                txtInvoicePurchase.Text = this._beli.invoice;
                txtInvoicePurchase.Enabled = false;

                LoadItemPurchase(this._beli);
            }                

            // save data lama
            _listOfItemReturnOld.Clear();
            foreach (var item in this._return.item_return)
            {
                _listOfItemReturnOld.Add(new ItemReturnPurchaseProduct
                {
                    return_purchase_item_id = item.return_purchase_item_id,
                    return_quantity = item.return_quantity,
                    price = item.price
                });
            }
            
            _listOfItemReturn = this._return.item_return;
            _listOfItemReturn.Add(new ItemReturnPurchaseProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
            RefreshTotal();
        }

        private void LoadItemPurchase(PurchaseProduct beliProduct)
        {
            IPurchaseProductBll bll = new PurchaseProductBll(_log);
            _beli.item_beli = bll.GetItemPurchase(_beli.purchase_id).ToList();
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

            GridListControlHelper.InitializeGridListControl<ItemReturnPurchaseProduct>(grid, _listOfItemReturn, gridListProperties, 30);

            grid.PushButtonClick += delegate(object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.ColIndex == 7)
                {
                    if (grid.RowCount == 1)
                    {
                        MsgHelper.MsgWarning("At least 1 product must be input!");
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
                            e.Style.CellValue = NumberHelper.NumberToString(itemReturn.price);

                            break;

                        case 6: // subtotal
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.Enabled = false;

                            e.Style.CellValue = NumberHelper.NumberToString(itemReturn.return_quantity * itemReturn.price);
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

        private double SumGrid(IList<ItemReturnPurchaseProduct> listOfItemReturn)
        {
            double total = 0;
            foreach (var item in _listOfItemReturn.Where(f => f.Product != null))
            {
                total += item.price * item.return_quantity;
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemReturn));
        }

        protected override void Save()
        {
            if (this._supplier == null || txtSupplier.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Supplier' should not empty !");
                txtSupplier.Focus();

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
                _return = new ReturnPurchaseProduct();

            _return.purchase_id = this._beli.purchase_id;
            _return.PurchaseProduct = this._beli;
            _return.user_id = this._user.user_id;
            _return.User = this._user;
            _return.supplier_id = this._supplier.supplier_id;
            _return.Supplier = this._supplier;
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

                    _supplier = null;
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
                    var itemReturn = _listOfItemReturnOld.Where(f => f.return_purchase_item_id == item.return_purchase_item_id)
                                                       .SingleOrDefault();

                    if (itemReturn != null)
                    {
                        item.return_quantity = itemReturn.return_quantity;
                        item.price = itemReturn.price;
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
            if (data is ItemPurchaseProduct) // pencarian product baku
            {
                var itemPurchase = (ItemPurchaseProduct)data;
                var product = itemPurchase.Product;

                if (!IsExist(product.product_id))
                {
                    SetItemProduct(this.gridControl, _rowIndex, itemPurchase, itemPurchase.quantity - itemPurchase.return_quantity, itemPurchase.price);
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
            else if (data is Supplier) // pencarian Customer
            {
                this._supplier = (Supplier)data;
                txtSupplier.Text = this._supplier.name_supplier;
                KeyPressHelper.NextFocus();
            }
            else if (data is PurchaseProduct) // pencarian data sale
            {
                IPurchaseProductBll bll = new PurchaseProductBll(_log);

                this._beli = (PurchaseProduct)data;
                this._beli.item_beli = bll.GetItemPurchase(this._beli.purchase_id).ToList();
                txtInvoicePurchase.Text = this._beli.invoice;

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

        private void txtSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var name = ((TextBox)sender).Text;

                ISupplierBll bll = new SupplierBll(_log);
                var listOfSupplier = bll.GetByName(name);

                if (listOfSupplier.Count == 0)
                {
                    MsgHelper.MsgWarning("Data Supplier not found");
                    txtSupplier.Focus();
                    txtSupplier.SelectAll();

                }
                else if (listOfSupplier.Count == 1)
                {
                    _supplier = listOfSupplier[0];
                    txtSupplier.Text = _supplier.name_supplier;
                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Data Supplier", listOfSupplier);
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

                if (this._beli == null || txtInvoicePurchase.Text.Length == 0)
                {
                    MsgHelper.MsgWarning("Sorry, the data entry is incomplete!");
                    txtInvoicePurchase.Focus();

                    return;
                }

                var grid = (GridControl)sender;

                var rowIndex = grid.CurrentCell.RowIndex;
                var colIndex = grid.CurrentCell.ColIndex;

                GridCurrentCell cc;

                ItemPurchaseProduct itemPurchase;

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
                            itemPurchase = this._beli.item_beli.Where(f => f.Product.product_code.ToLower() == codeProduct.ToLower() && f.quantity > f.return_quantity)
                                                           .SingleOrDefault();

                            //this._beli.item_beli.Where(f => f.Product.product_code.ToLower() == codeProduct.ToLower() && f.quantity > f.return_quantity).SingleOrDefault();

                            if (itemPurchase == null)
                            {
                                MsgHelper.MsgWarning("Product data not found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                return;
                            }

                            

                            _isValidCodeProduct = true;

                            if (!IsExist(itemPurchase.product_id))
                            {
                                SetItemProduct(grid, rowIndex, itemPurchase, itemPurchase.quantity - itemPurchase.return_quantity, itemPurchase.price);
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
                            //var listOfItemPurchase = this._beli.item_beli.Where(f => f.Product.
                            //product_name.ToLower().Contains(nameProduct.ToLower()) && f.quantity > f.return_quantity)
                            //                                         .ToList();
                            var listOfItemPurchase = this._beli?.item_beli
     .Where(f => f.Product?.product_name?.ToLower().Contains(nameProduct?.ToLower()) == true
                 && f.quantity > f.return_quantity)
     .ToList();
                            if (this._beli != null && nameProduct != null)
                            {
                                 listOfItemPurchase = this._beli.item_beli
                                    .Where(f => f.Product != null
                                        && f.Product.product_name != null
                                        && f.Product.product_name.ToLower().Contains(nameProduct.ToLower())
                                        && f.quantity > f.return_quantity)
                                    .ToList();
                                // ...
                            }

                            if (listOfItemPurchase.Count == 0)
                            {
                                MsgHelper.MsgWarning("Product data not found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                            }
                            else if (listOfItemPurchase.Count == 1)
                            {
                                itemPurchase = listOfItemPurchase[0];

                                if (!IsExist(itemPurchase.product_id))
                                {
                                    SetItemProduct(grid, rowIndex, itemPurchase, itemPurchase.quantity - itemPurchase.return_quantity, itemPurchase.price);
                                    grid.Refresh();

                                    GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                                }
                                else
                                {
                                    MsgHelper.MsgWarning("Data product already entered");
                                    GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                }
                            }
                            //                       var listOfItemPurchase = this._beli?.item_beli
                            //.Where(f => f.Product?.product_name?.ToLower().Contains(nameProduct?.ToLower()) == true
                            //            && f.quantity > f.return_quantity)
                            //.ToList();


                            else // data lebih dari one
                            {
                                _rowIndex = rowIndex;
                                _colIndex = colIndex;

                                var frmLookup = new FrmLookupItemInvoice("Item Purchase", listOfItemPurchase);
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
                                MsgHelper.MsgWarning("Sorry, the quantity returned cannot exceed the quantity sold");
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
                            _listOfItemReturn.Add(new ItemReturnPurchaseProduct());
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

        private void SetItemProduct(GridControl grid, int rowIndex, ItemPurchaseProduct itemPurchase, double jumlahReturn = 0, double price = 0)
        {
            ItemReturnPurchaseProduct itemReturn;

            if (_isNewData)
            {
                itemReturn = new ItemReturnPurchaseProduct();
            }
            else
            {
                itemReturn = _listOfItemReturn[rowIndex - 1];

                if (itemReturn.entity_state == EntityState.Unchanged)
                    itemReturn.entity_state = EntityState.Modified;
            }

            var product = itemPurchase.Product;

            itemReturn.purchase_item_id = itemPurchase.purchase_item_id;
            itemReturn.product_id = product.product_id;
            itemReturn.Product = product;

            itemReturn.quantity = itemPurchase.quantity;
            itemReturn.return_quantity = jumlahReturn;
            itemReturn.price = price;

            _listOfItemReturn[rowIndex - 1] = itemReturn;

            itemPurchase.return_quantity = jumlahReturn;
        }

        private void gridControl_CurrentCellValidated(object sender, EventArgs e)
        {
            var grid = (GridControl)sender;

            GridCurrentCell cc = grid.CurrentCell;

            if (this._beli != null)
            {
                var obj = _listOfItemReturn[cc.RowIndex - 1];
                var itemSelling = this._beli.item_beli.Where(f => f.product_id == obj.product_id)
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
                            obj.price = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                            break;

                        default:
                            break;
                    }

                    SetItemProduct(grid, cc.RowIndex, itemSelling, obj.return_quantity, obj.price);
                    grid.Refresh();

                    RefreshTotal();
                }
            }           
        }

        private void FrmEntryReturnProductPurchase_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtInvoicePurchase_KeyPress(object sender, KeyPressEventArgs e)
        
        {
            if (KeyPressHelper.IsEnter(e))
            {
                if (this._supplier == null || txtSupplier.Text.Length == 0)
                {
                    MsgHelper.MsgWarning("Sorry, the data entry is incomplete!");
                    txtSupplier.Focus();

                    return;
                }

                var invoice = ((TextBox)sender).Text;

                IPurchaseProductBll bll = new PurchaseProductBll(_log);
                var listOfPurchase = bll.GetInvoiceSupplier(this._supplier.supplier_id, invoice);

                if (listOfPurchase.Count == 0)
                {
                    MsgHelper.MsgWarning("Data purchase invoice not found");
                    txtInvoicePurchase.Focus();
                    txtInvoicePurchase.SelectAll();
                }
                else if (listOfPurchase.Count == 1)
                {
                    _beli = listOfPurchase[0];
                    _beli.item_beli = bll.GetItemPurchase(_beli.purchase_id).ToList();

                    txtInvoicePurchase.Text = _beli.invoice;
                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupInvoice("Data Invoice Purchase", listOfPurchase);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }
    }
}
