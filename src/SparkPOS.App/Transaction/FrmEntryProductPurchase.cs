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
using Microsoft.Reporting.WinForms;
using System.Reflection;
using MultilingualApp;

namespace SparkPOS.App.Transactions
{
    public partial class FrmEntryProductPurchase : FrmEntryStandard, IListener
    {
        private string _reportNameSpace = @"SparkPOS.Report.{0}.rdlc";
        private Assembly _assemblyReport;
        private IPurchaseProductBll _bll = null;
        private PurchaseProduct _beli = null;
        private Supplier _supplier = null;
        private IList<ItemPurchaseProduct> _listOfItemPurchase = new List<ItemPurchaseProduct>();
        private IList<ItemPurchaseProduct> _listOfItemPurchaseOld = new List<ItemPurchaseProduct>();
        private IList<ItemPurchaseProduct> _listOfItemPurchaseDeleted = new List<ItemPurchaseProduct>();

        private int _rowIndex = 0;
        private int _colIndex = 0;
        private bool _isValidCodeProduct = false;

        private bool _isNewData = false;
        private ILog _log;
        private User _user;
        private Profil _profil;
        private GeneralSupplier _GeneralSupplier;

        public IListener Listener { private get; set; }

        public FrmEntryProductPurchase(string header, IPurchaseProductBll bll) 
            : base()
        {            
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this._isNewData = true;
            this._log = MainProgram.log;
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;
            _assemblyReport = Assembly.LoadFrom("SparkPOS.Report.dll");
            txtInvoice.Text = bll.GetLastInvoice();
            dtpDate.Value = DateTime.Today;
            dtpDateCreditTerm.Value = dtpDate.Value;
            chkPrintInvoicePurchase.Checked = this._GeneralSupplier.is_auto_print;


            List<Tax> taxNames = bll.GetTaxNames();
            cmbTaxName.ValueMember = "tax_id";
            cmbTaxName.DisplayMember = "CombinedDisplay";
            cmbTaxName.DataSource = taxNames;
            TaxCalculation();
            _listOfItemPurchase.Add(new ItemPurchaseProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
            SetLabelTextBasedOnLanguage();
        }

        public FrmEntryProductPurchase(string header, PurchaseProduct beli, IPurchaseProductBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._beli = beli;
            this._supplier = beli.Supplier;
            this._log = MainProgram.log;
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;

            txtInvoice.Text = this._beli.invoice;
            dtpDate.Value = (DateTime)this._beli.date;
            dtpDateCreditTerm.Value = dtpDate.Value;
            chkPrintInvoicePurchase.Checked = this._GeneralSupplier.is_auto_print;

            if (!this._beli.due_date.IsNull())
            {
                rdoKredit.Checked = true;
                dtpDateCreditTerm.Value = (DateTime)this._beli.due_date;
            }

            txtSupplier.Text = this._supplier.name_supplier;
            txtKeterangan.Text = this._beli.description;

            txtDiskon.Text = this._beli.discount.ToString();
            txtPPN.Text = this._beli.tax.ToString();

            // save data lama
            _listOfItemPurchaseOld.Clear();
            foreach (var item in this._beli.item_beli)
            {
                _listOfItemPurchaseOld.Add(new ItemPurchaseProduct
                {
                    purchase_item_id = item.purchase_item_id,
                    quantity = item.quantity,
                    price = item.price
                });
            }

            _listOfItemPurchase = this._beli.item_beli;
            _listOfItemPurchase.Add(new ItemPurchaseProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
            RefreshTotal();
        }

        private void InitGridControl(GridControl grid)
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 320 });
            gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "discount", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Sub Total", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Action" });

            GridListControlHelper.InitializeGridListControl<ItemPurchaseProduct>(grid, _listOfItemPurchase, gridListProperties);

            grid.PushButtonClick += delegate(object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.ColIndex == 8)
                {
                    if (grid.RowCount == 1)
                    {
                        MsgHelper.MsgWarning("Minimum 1 product item must be inputted !");
                        return;
                    }

                    if (MsgHelper.MsgDelete())
                    {
                        var itemPurchase = _listOfItemPurchase[e.RowIndex - 1];
                        itemPurchase.entity_state = EntityState.Deleted;

                        _listOfItemPurchaseDeleted.Add(itemPurchase);
                        _listOfItemPurchase.Remove(itemPurchase);

                        grid.RowCount = _listOfItemPurchase.Count();
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
                    if (!(_listOfItemPurchase.Count > 0))
                        return;

                    var itemPurchase = _listOfItemPurchase[e.RowIndex - 1];
                    var product = itemPurchase.Product;

                    if (e.RowIndex % 2 == 0)
                        e.Style.BackColor = ColorCollection.BACK_COLOR_ALTERNATE;

                    double price = 0;
                    double quantity = 0;

                    var isReturn = itemPurchase.return_quantity > 0;
                    if (isReturn)
                    {
                        e.Style.BackColor = Color.Red;
                        e.Style.Enabled = false;
                    }

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

                        case 4: // quantity
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemPurchase.quantity - itemPurchase.return_quantity;

                            break;

                        case 5: // discount
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemPurchase.discount;

                            break;

                        case 6: // price
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;

                            price = itemPurchase.price;

                            if (!(price > 0))
                            {
                                if (product != null)
                                    price = product.purchase_price;
                            }

                            e.Style.CellValue = NumberHelper.NumberToString(price);

                            break;

                        case 7: // subtotal
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.Enabled = false;

                            quantity = itemPurchase.quantity - itemPurchase.return_quantity;
                            price = itemPurchase.harga_setelah_diskon;

                            if (!(price > 0))
                            {
                                if (product != null)
                                    price = product.purchase_price;
                            }

                            e.Style.CellValue = NumberHelper.NumberToString(quantity * price);
                            TaxCalculation();
                            break;

                        case 8: // button hapus
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellType = GridCellTypeName.PushButton;
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

        private double SumGrid(IList<ItemPurchaseProduct> listOfItemPurchase)
        {
            double total = 0;
            foreach (var item in listOfItemPurchase.Where(f => f.Product != null))
            {
                double price = 0;

                if (item.price > 0)
                    price = item.harga_setelah_diskon;
                else
                {
                    if (item.Product != null)
                        price = item.Product.purchase_price;
                }

                total += price * (item.quantity - item.return_quantity);
            }

            if (total > 0)
            {
                total -= NumberHelper.StringToDouble(txtDiskon.Text);
                total += NumberHelper.StringToDouble(txtPPN.Text);
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemPurchase));
        }

        protected override void Save()
        {
            if (this._supplier == null || txtSupplier.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Supplier' Cannot be empty !");
                txtSupplier.Focus();

                return;
            }

            var total = SumGrid(this._listOfItemPurchase);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You have not completed the input of product data !");
                return;
            }

            if (rdoKredit.Checked)
            {
                if (!DateTimeHelper.IsValidRangeDate(dtpDate.Value, dtpDateCreditTerm.Value))
                {
                    MsgHelper.MsgNotValidRangeDate();
                    return;
                }
            }

            if (!MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
                return;

            if (_isNewData)
                _beli = new PurchaseProduct();

            _beli.user_id = this._user.user_id;
            _beli.User = this._user;
            _beli.supplier_id = this._supplier.supplier_id;
            _beli.Supplier = this._supplier;
            _beli.invoice = txtInvoice.Text;
            _beli.date = dtpDate.Value;
            _beli.due_date = DateTimeHelper.GetNullDateTime();
            _beli.is_cash = rdoCash.Checked;

            if (rdoKredit.Checked) // Purchase Credit
            {
                _beli.due_date = dtpDateCreditTerm.Value;
            }

            _beli.tax = NumberHelper.StringToDouble(txtPPN.Text);
            _beli.discount = NumberHelper.StringToDouble(txtDiskon.Text);
            _beli.description = txtKeterangan.Text;

            _beli.item_beli = this._listOfItemPurchase.Where(f => f.Product != null).ToList();
            foreach (var item in _beli.item_beli)
            {
                if (!(item.price > 0))
                    item.price = item.Product.purchase_price;

                if (cmbTaxName.SelectedValue != null)
                {
                    item.tax_id = cmbTaxName.SelectedValue.ToString();
                }
                else
                {
                    // Handle the case where the selected tax ID is invalid or not available
                    MsgHelper.MsgWarning("Invalid Tax ID.");
                    return;
                }
            }

            if (!_isNewData) // update
                _beli.item_beli_deleted = _listOfItemPurchaseDeleted.ToList();

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                {
                    result = _bll.Save(_beli, ref validationError);
                }
                else
                {
                    result = _bll.Update(_beli, ref validationError);
                }

                if (result > 0)
                {
                    try
                    {
                        if (chkPrintInvoicePurchase.Checked)
                            PrintInvoice(_beli.purchase_id);
                    }
                    catch (Exception ex)
                    {
                        MainProgram.LogException(ex);
                        // Error handling and logging
                        var msg = MainProgram.GlobalWarningMessage();
                        MsgHelper.MsgWarning(msg);
                        //WarningMessageHandler.ShowTranslatedWarning(msg, MainProgram.currentLanguage);
                    }

                    Listener.Ok(this, _isNewData, _beli);

                    _supplier = null;
                    _listOfItemPurchase.Clear();
                    _listOfItemPurchaseDeleted.Clear();                                        

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

        private void PrintInvoice(string beliProductId)
        {
            IPrintInvoiceBll printBll = new PrintInvoiceBll(_log);
            var listOfItemInvoice = printBll.GetInvoicePurchase(beliProductId);

            if (listOfItemInvoice.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "InvoicePurchase",
                    Value = listOfItemInvoice
                };

                //  set header invoice
                var parameters = new List<ReportParameter>();
                var index = 1;

                foreach (var item in _GeneralSupplier.list_of_header_nota)
                {
                    var paramName = string.Format("header{0}", index);
                    parameters.Add(new ReportParameter(paramName, item.description));

                    index++;
                }

                //  set footer invoice
                var dt = DateTime.Now;
                var cityAndDate = string.Format("{0}, {1}", _profil.city, dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);

                parameters.Add(new ReportParameter("city", cityAndDate));
                parameters.Add(new ReportParameter("footer", _user.name_user));
                var reportName = "RvInvoiceProductPurchase"; // Specify the path to your RDLC report file

                var frmPreviewReport = new FrmPreviewReport("Preview Invoice Sales", reportName, reportDataSource, parameters, true);
                frmPreviewReport.ShowDialog();
                var printReport = new ReportViewerPrintHelper("RvInvoiceProductPurchase", reportDataSource, parameters, _GeneralSupplier.name_printer);

                printReport.Print();
            }
        }
      

        protected override void Cancel()
        {
            // restore data lama
            if (!_isNewData)
            {
                // restore item yang di edit
                var itemsModified = _beli.item_beli.Where(f => f.Product != null && f.entity_state == EntityState.Modified)
                                                   .ToArray();

                foreach (var item in itemsModified)
                {
                    var itemPurchase = _listOfItemPurchaseOld.Where(f => f.purchase_item_id == item.purchase_item_id)
                                                     .SingleOrDefault();

                    if (itemPurchase != null)
                    {
                        item.quantity = itemPurchase.quantity;
                        item.price = itemPurchase.price;
                    }
                }

                // restore item yang di delete
                var itemsDeleted = _listOfItemPurchaseDeleted.Where(f => f.Product != null && f.entity_state == EntityState.Deleted)
                                                         .ToArray();
                foreach (var item in itemsDeleted)
                {
                    item.entity_state = EntityState.Unchanged;
                    this._beli.item_beli.Add(item);
                }

                _listOfItemPurchaseDeleted.Clear();
            }

            base.Cancel();
        }

        public void Ok(object sender, object data)
        {
            if (data is Product) // pencarian product baku
            {
                var product = (Product)data;

                SetItemProduct(this.gridControl, _rowIndex, product);
                this.gridControl.Refresh();
                RefreshTotal();

                GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, _colIndex + 1);
            }
            else if (data is Supplier) // pencarian supplier
            {
                this._supplier = (Supplier)data;
                txtSupplier.Text = this._supplier.name_supplier;
                KeyPressHelper.NextFocus();
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            // do nothing
        }

        private void rdoCash_CheckedChanged(object sender, EventArgs e)
        {
            dtpDateCreditTerm.Enabled = false;
        }

        private void rdoKredit_CheckedChanged(object sender, EventArgs e)
        {
            dtpDateCreditTerm.Enabled = true;
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
                var supplierName = ((AdvancedTextbox)sender).Text;

                ISupplierBll bll = new SupplierBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                var listOfSupplier = bll.GetByName(supplierName);

                if (listOfSupplier.Count == 0)
                {
                    MsgHelper.MsgWarning("Data supplier not found");
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

        private void SetItemProduct(GridControl grid, int rowIndex, Product product, double quantity = 1, double price = 0, double discount = 0)
        {
            ItemPurchaseProduct itemPurchase;

            if (_isNewData)
            {
                itemPurchase = new ItemPurchaseProduct();
            }
            else
            {
                itemPurchase = _listOfItemPurchase[rowIndex - 1];

                if (itemPurchase.entity_state == EntityState.Unchanged)
                    itemPurchase.entity_state = EntityState.Modified;
            }

            itemPurchase.product_id = product.product_id;
            itemPurchase.Product = product;
            itemPurchase.quantity = quantity;
            itemPurchase.price = product.purchase_price;

            if (price > 0)
                itemPurchase.price = price;

            itemPurchase.discount = discount;

            _listOfItemPurchase[rowIndex - 1] = itemPurchase;
        }

        private void gridControl_CurrentCellKeyDown(object sender, KeyEventArgs e)
        
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var grid = (GridControl)sender;

                var rowIndex = grid.CurrentCell.RowIndex;
                var colIndex = grid.CurrentCell.ColIndex;

                IProductBll bll = new ProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                Product product = null;
                GridCurrentCell cc;

                switch (colIndex)
                {
                    case 2: // code product
                        _isValidCodeProduct = false;

                        cc = grid.CurrentCell;
                        var codeProduct = cc.Renderer.ControlValue.ToString();

                        if (codeProduct.Length == 0) // code product empty
                        {
                            // fokus ke column name product
                            GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                        }
                        else
                        {
                            // pencarian based code product
                            product = bll.GetByCode(codeProduct);

                            if (product == null)
                            {
                                MsgHelper.MsgWarning("Data product not found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                return;
                            }

                            _isValidCodeProduct = true;

                            SetItemProduct(grid, rowIndex, product);
                            grid.Refresh();
                            RefreshTotal();

                            GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 2);
                        }

                        break;

                    case 3: // pencarian based name product

                        cc = grid.CurrentCell;
                        var nameProduct = cc.Renderer.ControlValue.ToString();

                        if (nameProduct.Length == 0)
                        {
                            MsgHelper.MsgWarning("Name product not allowed empty");
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);

                            return;
                        }

                        if (!_isValidCodeProduct)
                        {
                            var listOfProduct = bll.GetByName(nameProduct, false);

                            if (listOfProduct.Count == 0)
                            {
                                MsgHelper.MsgWarning("Data product Not Found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                            }
                            else if (listOfProduct.Count == 1)
                            {
                                product = listOfProduct[0];

                                SetItemProduct(grid, rowIndex, product);
                                grid.Refresh();
                                RefreshTotal();

                                GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                            }
                            else // data lebih dari one
                            {
                                _rowIndex = rowIndex;
                                _colIndex = colIndex;

                                var frmLookup = new FrmLookupReference("Data Product", listOfProduct, true);
                                frmLookup.Listener = this;
                                frmLookup.ShowDialog();
                            }
                        }
                        else
                        {
                            GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                        }

                        break;

                    case 4: // quantity
                    case 5: // discount
                        GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                        break;

                    case 6:
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemPurchase.Add(new ItemPurchaseProduct());
                            grid.RowCount = _listOfItemPurchase.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, rowIndex + 1, 2); // fokus ke column code product
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

            // validasi input angka untuk column quantity dan price
            switch (cc.ColIndex)
            {
                case 4: // quantity
                case 5: // discount
                case 6: // price
                    e.Handled = KeyPressHelper.NumericOnly(e);
                    break;

                default:
                    break;
            }
        }

        private void gridControl_CurrentCellValidated(object sender, EventArgs e)
        {
            var grid = (GridControl)sender;

            GridCurrentCell cc = grid.CurrentCell;

            var itemPurchase = _listOfItemPurchase[cc.RowIndex - 1];
            var product = itemPurchase.Product;

            if (product != null)
            {
                switch (cc.ColIndex)
                {
                    case 4: // column quantity
                        itemPurchase.quantity = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    case 5: // dikson
                        itemPurchase.discount = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    case 6: // column price
                        itemPurchase.price = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    default:
                        break;
                }

                SetItemProduct(grid, cc.RowIndex, product, itemPurchase.quantity, itemPurchase.price, itemPurchase.discount);
                grid.Refresh();

                RefreshTotal();
            }           
        }

        private void gridControl_KeyDown(object sender, KeyEventArgs e)
        {
            // kasus khusus untuk shortcut F2, tidak jalan jika dipanggil melalui event Form KeyDown
            if (KeyPressHelper.IsShortcutKey(Keys.F2, e)) // Optional data supplier
            {
                ShowEntrySupplier();
            }
        }

        private void ShowEntryProduct()
        {
            var isGrant = RolePrivilegeHelper.IsHaveRightAccess("mnuProduct", _user);
            if (!isGrant)
            {
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
                return;
            }
            
            ICategoryBll golonganBll = new CategoryBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var listOfCategory = golonganBll.GetAll();

            Category category = null;
            if (listOfCategory.Count > 0)
                category = listOfCategory[0];

            IProductBll produkBll = new ProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var frmEntryProduct = new FrmEntryProduct("Add Data Product", category, listOfCategory, produkBll);
            frmEntryProduct.Listener = this;
            frmEntryProduct.ShowDialog();
        }

        private void ShowEntrySupplier()
        {
            var isGrant = RolePrivilegeHelper.IsHaveRightAccess("mnuSupplier", _user);
            if (!isGrant)
            {
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
                return;
            }

            ISupplierBll supplierBll = new SupplierBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var frmEntrySupplier = new FrmEntrySupplier("Add Data Supplier", supplierBll);
            frmEntrySupplier.Listener = this;
            frmEntrySupplier.ShowDialog();
        }

        private void FrmEntryProductPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsShortcutKey(Keys.F1, e)) // tambah data product
            {
                ShowEntryProduct();
            }
            else if (KeyPressHelper.IsShortcutKey(Keys.F2, e)) // Optional data supplier
            {
                ShowEntrySupplier();
            }
        }

        private void txtDiskon_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void txtPPN_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void FrmEntryProductPurchase_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hapus objek dumm
            if (!_isNewData)
            {
                var itemsToRemove = _beli.item_beli.Where(f => f.Product == null && f.entity_state == EntityState.Added)
                                                   .ToArray();

                foreach (var item in itemsToRemove)
                {
                    _beli.item_beli.Remove(item);
                }
            }
        }

        private void txtPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void SetLabelTextBasedOnLanguage()
        {
            if (MainProgram.currentLanguage == "en-US")
            {
                //this.label10.Text = "F1: Add data product | F2: Add data customer | F3: Add data dropshipper | F5: Edit quantity | F6: Edit discount | F7: Edit price | F8: Pay";
                this.label10.Text = "F1: Add data product | F2: Add data supplier";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                //this.label10.Text = "F1: إضافة منتج البيانات | F2: إضافة مورد البيانات | F3: إضافة بيانات دروبشيبر | F5: تعديل الكمية | F6: تعديل الخصم | F7: تعديل السعر | F8: الدفع";
                this.label10.Text = "F1: إضافة منتج البيانات | F2: إضافة مورد البيانات";
            }
        }
        private void gridControl_CellClick(object sender, GridCellClickEventArgs e)
        {

        }
        private double originalSubtotal = 0.0; // Keep track of the original subtotal

        private void cmbTaxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaxCalculation();
        }

        private void TaxCalculation()
        {
            txtPPN.Clear();
            double subtotal = SumGrid(_listOfItemPurchase);


            double taxRate = 0.0;
            Tax tax = (Tax)cmbTaxName.SelectedItem;

            if (tax != null && double.TryParse(tax.tax_percentage.ToString(), out taxRate))
            {

                double taxTotal = subtotal * taxRate / 100.0;  // Convert the tax rate to a decimal

                txtPPN.Text = taxTotal.ToString();
            }
            else
            {
                // Handle the case where the selected tax rate is invalid or not available

               
                if (MainProgram.currentLanguage == "en-US")
                {
                    txtPPN.Text = "0.0";
                }
                else if (MainProgram.currentLanguage == "ar-SA")
                {
                    txtPPN.Text = "معدل الضريبة غير صالح";
                }
            }
        }

    }
}
