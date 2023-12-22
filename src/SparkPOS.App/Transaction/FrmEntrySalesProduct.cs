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
using SparkPOS.Model.Invoice;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Reference;
using ConceptCave.WaitCursor;
using log4net;
using Microsoft.Reporting.WinForms;
using SparkPOS.Model.RajaOngkir;
using SparkPOS.Helper.RAWPrinting;
using SparkPOS.Logger;
using System.Data.SqlClient;
using SparkPOS.Model.Transaction;
using System.Globalization;
using MultilingualApp;

namespace SparkPOS.App.Transactions
{

    public partial class FrmEntrySalesProduct : FrmEntryStandard, IListener
    {
        private ISellingProductBll _bll = null;
        private SellingProduct _jual = null;
        private Customer _customer = null;
        private Dropshipper _dropshipper = null;
        private IList<ItemSellingProduct> _listOfItemSelling = new List<ItemSellingProduct>();
        private IList<ItemSellingProduct> _listOfItemSellingOld = new List<ItemSellingProduct>();
        private IList<ItemSellingProduct> _listOfItemSellingDeleted = new List<ItemSellingProduct>();

        private int _rowIndex = 0;
        private int _colIndex = 0;

        private ILogger _ILogger = Logger.Logger.GetInstance;
        //_ILogger.LogError(filterContext.Exception.ToString());


        private bool _isNewData = false;
        private ILog _log;
        private User _user;
        private Profil _profil;
        private GeneralSupplier _GeneralSupplier;
        private SettingPort _settingPort;
        private SettingCustomerDisplay _settingCustomerDisplay;

        public IListener Listener { private get; set; }

        public FrmEntrySalesProduct(string header, ISellingProductBll bll)
            : base()
        {
             InitializeComponent();  
           // MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this._isNewData = true;
            this._log = MainProgram.log;
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;
            this._settingPort = MainProgram.settingPort;
            this._settingCustomerDisplay = MainProgram.settingCustomerDisplay;

            txtInvoice.Text = bll.GetLastInvoice();
            dtpDate.Value = DateTime.Today;
            dtpDateCreditTerm.Value = dtpDate.Value;
            btnPreviewInvoice.Visible = _GeneralSupplier.type_printer == TypePrinter.InkJet;
            txtPPN.Text = _GeneralSupplier.default_ppn.ToString();

            SetSettingsPrinter();

            List<Tax> taxNames = bll.GetTaxNames();
            
            cmbTaxName.ValueMember = "tax_id";
            cmbTaxName.DisplayMember = "CombinedDisplay";
            cmbTaxName.DataSource = taxNames;
            TaxCalculation();

            _listOfItemSelling.Add(new ItemSellingProduct()); // add dummy objek

            InitGridControl(gridControl);

            DisplayOpeningSentence();
            tmrDisplayKalimatPenutup.Interval = _settingCustomerDisplay.delay_display_closing_sentence * 1000;
            MainProgram.GlobalLanguageChange(this);
            LanguageHelper.TranslateToolTripTitle(this);
        }

        public FrmEntrySalesProduct(string header, SellingProduct sale, ISellingProductBll bll)
            : base()
        {
             InitializeComponent();  
           // MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._jual = sale;
            this._customer = sale.Customer;
            this._dropshipper = sale.Dropshipper;
            this._log = MainProgram.log;
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;
            this._settingPort = MainProgram.settingPort;
            this._settingCustomerDisplay = MainProgram.settingCustomerDisplay;

            txtInvoice.Text = this._jual.invoice;
            dtpDate.Value = (DateTime)this._jual.date;
            dtpDateCreditTerm.Value = dtpDate.Value;
            btnPreviewInvoice.Visible = _GeneralSupplier.type_printer == TypePrinter.InkJet;

            chkDropship.Checked = this._jual.is_dropship;
            SetSettingsPrinter();

            if (!this._jual.due_date.IsNull())
            {
                rdoKredit.Checked = true;
                dtpDateCreditTerm.Value = (DateTime)this._jual.due_date;
            }

            if (this._customer != null)
                txtCustomer.Text = this._customer.name_customer;

            if (this._dropshipper != null)
                txtDropshipper.Text = this._dropshipper.name_dropshipper;

            txtKeterangan.Text = this._jual.description;

            if (!string.IsNullOrEmpty(this._jual.courier))
                cmbKurir.Text = this._jual.courier;
            if(_jual.SellingQuotation!=null)
            { 
            if (!string.IsNullOrEmpty(this._jual.SellingQuotation.quotation))
                cmbQuotation.Text = this._jual.SellingQuotation.quotation;
            }

            txtCostShipping.Text = this._jual.shipping_cost.ToString();
            txtDiskon.Text = this._jual.discount.ToString();
            txtPPN.Text = this._jual.tax.ToString();

            // save data lama
            _listOfItemSellingOld.Clear();
            foreach (var item in this._jual.item_jual)
            {
                _listOfItemSellingOld.Add(new ItemSellingProduct
                {
                    sale_item_id = item.sale_item_id,
                    quantity = item.quantity,
                    selling_price = item.selling_price
                });
            }

            _listOfItemSelling = this._jual.item_jual;
            _listOfItemSelling.Add(new ItemSellingProduct()); // add dummy objek

            InitGridControl(gridControl);

            RefreshTotal();

            DisplayOpeningSentence();
            tmrDisplayKalimatPenutup.Interval = _settingCustomerDisplay.delay_display_closing_sentence * 1000;
            MainProgram.GlobalLanguageChange(this);

            //  var salesProductBLL = new SalesProductBLL(); // Replace with your actual BLL instance

        }

        // private ISellingProductBll _sellingProductBll;
        //_sellingProductBll = new SellingProductBll();

        private void LoadQuotations(string customerId)
        {
            // _sellingProductBll = new SellingProductBll(log);
            //List<string> quotations = _sellingProductBll.GetQuotationsByCustomerId(customerId);
            List<string> quotations = this._bll.GetQuotationsByCustomerId(customerId);

            cmbQuotation.Items.Clear();
            cmbQuotation.Items.Add("Select");
            cmbQuotation.Items.AddRange(quotations.ToArray());
            cmbQuotation.SelectedIndex = 0;
        }


        //private void QuotationDropdown_SelectionChanged(object sender, EventArgs e)
        //{
        //    //string selectedQuotation = cmbQuotation.SelectedItem.ToString();

        //    //// Call the GetProductDetailsByQUotation method and receive the list of product details
        //    //List<string> productDetails = this._bll.GetProductDetailsByQUotation(selectedQuotation);

        //    //// Clear the existing data in the grid control
        //    //grid.Clear();

        //    //// Populate the grid control with the product details
        //    //int rowIndex = 0;
        //    //foreach (string productDetail in productDetails)
        //    //{
        //    //    string[] detailArray = productDetail.Split(',');

        //    //    // Create a new row and populate the cells
        //    //    grid.SetCellValue(rowIndex, 0, rowIndex.ToString()); // No
        //    //    grid.SetCellValue(rowIndex, 1, detailArray[0]); // Code Product
        //    //    grid.SetCellValue(rowIndex, 2, detailArray[1]); // Name Product
        //    //    grid.SetCellValue(rowIndex, 3, detailArray[2]); // Quantity
        //    //    grid.SetCellValue(rowIndex, 4, detailArray[3]); // Discount
        //    //    grid.SetCellValue(rowIndex, 5, detailArray[4]); // Price
        //    //    grid.SetCellValue(rowIndex, 6, detailArray[5]); // Sub Total

        //    //    rowIndex++;
        //    }
        //}




        private void SetSettingsPrinter()
        {
            if (!(this._GeneralSupplier.type_printer == TypePrinter.InkJet))
            {
                chkDropship.Visible = false;
                chkDropship.Checked = false;
            }

            chkPrintInvoiceSelling.Checked = this._GeneralSupplier.is_auto_print;
        }
        
        private void InitGridControl(GridControl grid)
        {

            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 120 });

            gridListProperties.Add(new GridListControlProperties
            {
                Header = "Name Product",
                Width = _GeneralSupplier.is_show_additional_sales_item_information ? 390 : 500
            }
            );

            gridListProperties.Add(new GridListControlProperties
            {
                Header = _GeneralSupplier.additional_sales_item_information,
                Width = _GeneralSupplier.is_show_additional_sales_item_information ? 110 : 0
            }
            );

            gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "discount", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Sub Total", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Action" });

            GridListControlHelper.InitializeGridListControl<ItemSellingProduct>(grid, _listOfItemSelling, gridListProperties);
        //    ProcessGridControl(grid, rm, languageToLoad);
            grid.PushButtonClick += delegate (object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.ColIndex == 9)
                {
                    if (grid.RowCount == 1)
                    {
                        MsgHelper.MsgWarning("Minimum 1 item product must entered !");
                        return;
                    }

                    if (MsgHelper.MsgDelete())
                    {
                        var itemSelling = _listOfItemSelling[e.RowIndex - 1];
                        itemSelling.entity_state = EntityState.Deleted;

                        _listOfItemSellingDeleted.Add(itemSelling);
                        _listOfItemSelling.Remove(itemSelling);

                        grid.RowCount = _listOfItemSelling.Count();
                        grid.Refresh();

                        RefreshTotal();
                    }
                }
            };

            grid.QueryCellInfo += delegate (object sender, GridQueryCellInfoEventArgs e)
            {
                // Make sure the cell falls inside the grid
                if (e.RowIndex > 0)
                {
                    if (!(_listOfItemSelling.Count > 0))
                        return;

                    var itemSelling = _listOfItemSelling[e.RowIndex - 1];
                    var product = itemSelling.Product;

                    if (e.RowIndex % 2 == 0)
                        e.Style.BackColor = ColorCollection.BACK_COLOR_ALTERNATE;

                    double hargaPurchase = 0;
                    double hargaSelling = 0;
                    double quantity = 0;

                    var isReturn = itemSelling.return_quantity > 0;
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

                        case 4: // description
                            e.Style.CellValue = itemSelling.description;

                            break;

                        case 5: // quantity
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemSelling.quantity - itemSelling.return_quantity;

                            break;

                        case 6: // discount
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemSelling.discount;

                            break;

                        case 7: // price
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;

                            hargaPurchase = itemSelling.purchase_price;
                            hargaSelling = itemSelling.selling_price;

                            if (product != null)
                            {
                                if (!(hargaPurchase > 0))
                                    hargaPurchase = product.purchase_price;

                                if (!(hargaSelling > 0))
                                {
                                    quantity = itemSelling.quantity - itemSelling.return_quantity;
                                    hargaSelling = GetPriceSellingFix(product, quantity, product.selling_price);
                                }
                            }

                            e.Style.CellValue = NumberHelper.NumberToString(hargaSelling);

                            break;

                        case 8: // subtotal
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.Enabled = false;

                            quantity = itemSelling.quantity - itemSelling.return_quantity;

                            hargaPurchase = itemSelling.purchase_price;
                            hargaSelling = itemSelling.harga_setelah_diskon;

                            if (product != null)
                            {
                                if (!(hargaPurchase > 0))
                                    hargaPurchase = product.purchase_price;

                                if (!(hargaSelling > 0))
                                {
                                    double discount = itemSelling.discount;
                                    double diskonRupiah = 0;

                                    if (!(discount > 0))
                                    {
                                        if (_customer != null)
                                        {
                                            discount = _customer.discount;
                                        }

                                        if (!(discount > 0))
                                        {
                                            var diskonProduct = GetDiskonSellingFix(product, quantity, product.discount);
                                            discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                                        }
                                    }

                                    hargaSelling = GetPriceSellingFix(product, quantity, product.selling_price);

                                    diskonRupiah = discount / 100 * hargaSelling;
                                    hargaSelling -= diskonRupiah;
                                }
                            }

                            e.Style.CellValue = NumberHelper.NumberToString(quantity * hargaSelling);
                            TaxCalculation();
                            break;

                        case 9: // button hapus
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
                           // e.Style.Description = "Delete";
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

        private double SumGrid(IList<ItemSellingProduct> listOfItemSelling)
        {
            double total = 0;
            foreach (var item in _listOfItemSelling.Where(f => f.Product != null))
            {
                double price = 0;
                var quantity = item.quantity - item.return_quantity;

                if (item.selling_price > 0)
                {
                    price = item.harga_setelah_diskon;
                }
                else
                {
                    if (item.Product != null)
                    {
                        var hargaSelling = GetPriceSellingFix(item.Product, quantity, item.Product.selling_price);
                        var discount = GetDiskonSellingFix(item.Product, quantity, item.discount);

                        double diskonRupiah = discount / 100 * hargaSelling;

                        price = hargaSelling - diskonRupiah;
                    }
                }

                total += price * quantity;
            }

            if (total > 0)
            {
                total -= NumberHelper.StringToDouble(txtDiskon.Text);
                total += NumberHelper.StringToDouble(txtCostShipping.Text);
                total += NumberHelper.StringToDouble(txtPPN.Text, true);
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemSelling));
        }

        //private void cmbTaxName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    double subtotal = SumGrid(_listOfItemSelling);

        //    // Get the selected tax rate
        //    double taxRate = cmbTaxName.SelectedValue;
        //   // double taxRate = _bll.GetTaxRate(selectedTaxName);

        //    // Calculate the total including tax
        //    double taxTotal = subtotal *  taxRate;

        //    // Update the UI with the calculated total
        //    //   lblTotal.Text = total.ToString();
        // //   double taxTotal = subtotal -  total;
        //    // Update the grid level subtotal with tax value
        //    txtPPN.Text = taxTotal.ToString();
        //   // NumberHelper.dou
        //    // Refresh the total value
        //    // RefreshTotal();

        //    // Calculate the total including tax
        //    //double taxTotal = subtotal * taxRate;
        //    //int roundedTaxTotal = (int)Math.Round(taxTotal);

        //    //// Update the UI with the rounded tax total
        //    //txtPPN.Text = roundedTaxTotal.ToString();

        //}

        //private double originalSubtotal = 0.0; // Keep track of the original subtotal

        //private void cmbTaxName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    double subtotal = SumGrid(_listOfItemSelling);

        //    // Check if the original subtotal has been set
        //    if (originalSubtotal == 0.0)
        //    {
        //        originalSubtotal = subtotal; // Set the original subtotal
        //    }

        //    // Get the selected tax rate
        //    double taxRate = 0.0;
        //    Tax tax = (Tax)cmbTaxName.SelectedItem;
        //    //string strTaxRate = string.Empty;
        //    //var taxArray = cmbTaxName.SelectedText.Split('-');
        //    //if(taxArray.Length > 0)
        //    //{
        //    //    var taxValues = taxArray[1].Split(':');
        //    //    if(taxValues.Length > 0)
        //    //    {
        //    //        strTaxRate =  taxValues[1].Replace("%","").Trim()
        //    //    }
        //    //}
        //    if (tax != null && double.TryParse(tax.tax_percentage.ToString(), out taxRate))
        //    {
        //        // Calculate the total including tax based on the original subtotal
        //        double taxTotal = originalSubtotal * taxRate / 100.0;  // Convert the tax rate to a decimal

        //        // Update the UI with the calculated total
        //        txtPPN.Text = taxTotal.ToString();
        //    }
        //    else
        //    {
        //        // Handle the case where the selected tax rate is invalid or not available
        //        txtPPN.Text = "Invalid Tax Rate";
        //    }
        //}

        private void cmbTaxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaxCalculation();
        }

        private void TaxCalculation()
        {
            txtPPN.Clear();
            double subtotal = SumGrid(_listOfItemSelling);


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
                    txtPPN.Text = "0.0";
                }
            
            }
        }

        //private void cmbTax_name_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    // Calculate subtotal after tax value when the tax name is selected
        //    double subtotal = _bll.SumGrid(listOfItemSelling);

        //    // Get the selected tax rate
        //    string selectedTaxName = cmbTax_name.SelectedItem.ToString();
        //    double taxRate = _salesRepository.GetTaxRate(selectedTaxName);

        //    // Calculate the total including tax
        //    double total = _bll.CalculateTotal(subtotal, taxRate);

        //    // Update the UI with the calculated total
        //    lblTotal.Text = total.ToString();
        //}
        private PriceWholesale GetPriceWholesale(Product product, double quantity)
        {
            PriceWholesale hargaWholesale = null;

            if (product.list_of_harga_grosir.Count(f => f.minimum_quantity > 0) > 0)
            {
                hargaWholesale = product.list_of_harga_grosir
                                    .Where(f => f.product_id == product.product_id && (f.minimum_quantity > 0 && f.minimum_quantity <= quantity))
                                    .LastOrDefault();

                // price grosir tidak there yang cocok, set price retil
                if (hargaWholesale == null)
                    hargaWholesale = new PriceWholesale { retail_price = 1, wholesale_price = product.selling_price, discount = product.discount };
            }

            return hargaWholesale;
        }

        private double GetPriceSellingFix(Product product, double quantity, double hargaSellingRetail)
        {
            var result = hargaSellingRetail;

            if (quantity >= 1)
            {
                var grosir = GetPriceWholesale(product, quantity);
                if (grosir != null)
                {
                    if (grosir.wholesale_price > 0)
                        result = grosir.wholesale_price;
                }
            }

            return result;
        }

        private double GetDiskonSellingFix(Product product, double quantity, double diskonSellingRetail)
        {
            var result = diskonSellingRetail;

            if (quantity >= 1)
            {
                var grosir = GetPriceWholesale(product, quantity);
                if (grosir != null)
                {
                    if (grosir.discount > 0)
                        result = grosir.discount;
                }
            }

            return result;
        }

        private void UpdateDefaultPPN(double tax)
        {
            var appConfigFile = string.Format("{0}\\SparkPOS.exe.config", Utils.GetAppPath());
            _GeneralSupplier.default_ppn = tax;

            AppConfigHelper.SaveValue("defaultPPN", tax.ToString(), appConfigFile);
        }

        protected override void Save()
        {
            if (_GeneralSupplier.is_customer_required)
            {
                if (this._customer == null || txtCustomer.Text.Length == 0)
                {
                    MsgHelper.MsgWarning("'Customer' should not empty !");
                    txtCustomer.Focus();

                    return;
                }
            }

            var total = SumGrid(this._listOfItemSelling);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You haven't completed the product data input yet!");
                return;
            }

            var jumlahPay = NumberHelper.StringToNumber(txtJumlahPay.Text);
            if (jumlahPay > 0 && jumlahPay < total)
            {
                MsgHelper.MsgWarning("quantity pay less !");
                txtJumlahPay.Focus();
                txtJumlahPay.SelectAll();
                return;
            }

            if (rdoKredit.Checked)
            {
                if (!DateTimeHelper.IsValidRangeDate(dtpDate.Value, dtpDateCreditTerm.Value))
                {
                    MsgHelper.MsgNotValidRangeDate();
                    return;
                }

                total = NumberHelper.StringToDouble(lblTotal.Text);

                if (this._customer != null)
                {
                    if (this._customer.credit_limit > 0)
                    {
                        if (!(this._customer.credit_limit >= (total + this._customer.remaining_credit)))
                        {
                            var msg = string.Empty;

                            if (this._customer.remaining_credit > 0)
                            {
                                msg = "Sorry, the maximum credit limit for customer '{0}' is: {1}" +
      "\nCurrently, customer '{0}' still has a credit of: {2}";

                                msg = string.Format(msg, this._customer.name_customer, NumberHelper.NumberToString(this._customer.credit_limit), NumberHelper.NumberToString(this._customer.remaining_credit));
                            }
                            else
                            {
                                msg = "Sorry, the maximum credit limit for customer '{0}' is: {1}";


                                msg = string.Format(msg, this._customer.name_customer, NumberHelper.NumberToString(this._customer.credit_limit));
                            }

                            MsgHelper.MsgWarning(msg);
                            return;
                        }
                    }
                }
            }

            if (!MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
                return;

            if (_isNewData)
            {
                if (this._jual == null)
                    _jual = new SellingProduct();
            }

            _jual.user_id = this._user.user_id;
            _jual.User = this._user;

            if (this._customer != null)
            {
                _jual.customer_id = this._customer.customer_id;
                _jual.Customer = this._customer;
            }

            _jual.invoice = txtInvoice.Text;
            _jual.date = dtpDate.Value;
            _jual.due_date = DateTimeHelper.GetNullDateTime();
            _jual.is_cash = rdoCash.Checked;
            _jual.payment_cash = jumlahPay;

            if (rdoKredit.Checked) // sales Credit
            {
                _jual.due_date = dtpDateCreditTerm.Value;
            }

            _jual.dropshipper_id = null;
            _jual.Dropshipper = null;
            _jual.is_dropship = chkDropship.Checked;
            if (_jual.is_dropship)
            {
                if (this._dropshipper != null)
                {
                    _jual.dropshipper_id = this._dropshipper.dropshipper_id;
                    _jual.Dropshipper = this._dropshipper;
                }
            }

            _jual.courier = cmbKurir.Text;
            _jual.shipping_cost = NumberHelper.StringToDouble(txtCostShipping.Text);
            //_jual.tax = NumberHelper.StringToDouble(txtPPN.Text);
            _jual.tax = double.Parse(txtPPN.Text, CultureInfo.CurrentCulture);


            _jual.discount = NumberHelper.StringToDouble(txtDiskon.Text);
            _jual.description = txtKeterangan.Text;

            _jual.item_jual = this._listOfItemSelling.Where(f => f.Product != null).ToList();
            foreach (var item in _jual.item_jual)
            {
                if (!(item.purchase_price > 0))
                    item.purchase_price = item.Product.purchase_price;
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


                if (!(item.selling_price > 0))
                    item.selling_price = GetPriceSellingFix(item.Product, item.quantity - item.return_quantity, item.Product.selling_price);
            }

            if (!_isNewData) // update
                _jual.item_jual_deleted = _listOfItemSellingDeleted.ToList();

            // Retrieve the selected tax ID from the combo box

            // Retrieve the selected tax ID from the combo box




            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                {
                    result = _bll.Save(_jual, ref validationError);
                }
                else
                {
                    result = _bll.Update(_jual, ref validationError);
                }

                if (result > 0)
                {
                    try
                    {
                        if (chkPrintInvoiceSelling.Checked)
                        {
                            switch (this._GeneralSupplier.type_printer)
                            {
                                case TypePrinter.DotMatrix:
                                    if (MsgHelper.MsgConfirmation("Do you want to continue the printing process?"))
                                        PrintInvoiceDotMatrix(_jual);

                                    break;

                                case TypePrinter.MiniPOS:
                                    if (MsgHelper.MsgConfirmation("Do you want to continue the printing process?"))
                                        PrintInvoiceMiniPOS(_jual);

                                    break;
                                default:
                                    PrintInvoice(_jual.sale_id);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error:", ex);
                    }

                    Listener.Ok(this, _isNewData, _jual);

                    _customer = null;
                    _dropshipper = null;

                    _listOfItemSelling.Clear();
                    _listOfItemSellingDeleted.Clear();

                    UpdateDefaultPPN(_jual.tax);
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

        private void PrintInvoice(string jualProductId)
        {
            IPrintInvoiceBll printBll = new PrintInvoiceBll(_log);
            var listOfItemInvoice = printBll.GetInvoiceSales(jualProductId);

            if (listOfItemInvoice.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "InvoiceSales",
                    Value = listOfItemInvoice
                };

                // set header invoice
                var parameters = new List<ReportParameter>();
                var index = 1;

                foreach (var item in _GeneralSupplier.list_of_header_nota)
                {
                    var paramName = string.Format("header{0}", index);
                    parameters.Add(new ReportParameter(paramName, item.description));

                    index++;
                }

                foreach (var item in listOfItemInvoice)
                {
                    if (!_GeneralSupplier.is_print_keterangan_nota)
                        item.description = string.Empty;

                    if (item.from_label1.Length == 0)
                        item.from_label1 = this._GeneralSupplier.list_of_label_nota[0].description;

                    if (item.from_label2.Length == 0)
                        item.from_label2 = this._GeneralSupplier.list_of_label_nota[1].description;


                }

                // set footer invoice
                var dt = DateTime.Now;
                var cityAndDate = string.Format("{0}, {1}", _profil.city, dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);

                parameters.Add(new ReportParameter("city", cityAndDate));
                parameters.Add(new ReportParameter("footer", _user.name_user));

                var reportName = chkDropship.Checked ? "RvInvoiceSalesProductTanpaLabelDropship" : "RvInvoiceSalesProductTanpaLabel";

                var printReport = new ReportViewerPrintHelper(reportName, reportDataSource, parameters, _GeneralSupplier.name_printer);
                printReport.Print();
            }
        }

        private void PrintInvoiceMiniPOS(SellingProduct sale)
        {
            var autocutCode = _GeneralSupplier.is_autocut ? _GeneralSupplier.autocut_code : string.Empty;
            var openCashDrawerCode = _GeneralSupplier.is_open_cash_drawer ? _GeneralSupplier.open_cash_drawer_code : string.Empty;

            IRAWPrinting printerMiniPos = new PrinterMiniPOS(_GeneralSupplier.name_printer);

            printerMiniPos.Print(sale, _GeneralSupplier.list_of_header_nota_mini_pos, _GeneralSupplier.list_of_footer_nota_mini_pos,
                _GeneralSupplier.jumlah_karakter, _GeneralSupplier.jumlah_gulung, _GeneralSupplier.is_print_customer, FontSize: _GeneralSupplier.size_font,
                autocutCode: autocutCode, openCashDrawerCode: openCashDrawerCode);
        }

        private void PrintInvoiceDotMatrix(SellingProduct sale)
        {
            IRAWPrinting printerMiniPos = new PrinterDotMatrix(_GeneralSupplier.name_printer);
            printerMiniPos.Print(sale, _GeneralSupplier.list_of_header_nota, isPrintKeteranganInvoice: _GeneralSupplier.is_print_keterangan_nota);
        }

        protected override void Cancel()
        {
            // restore data lama
            if (!_isNewData)
            {
                // restore item yang di edit
                var itemsModified = _jual.item_jual.Where(f => f.Product != null && f.entity_state == EntityState.Modified)
                                                   .ToArray();

                foreach (var item in itemsModified)
                {
                    var itemSelling = _listOfItemSellingOld.Where(f => f.sale_item_id == item.sale_item_id)
                                                     .SingleOrDefault();

                    if (itemSelling != null)
                    {
                        item.quantity = itemSelling.quantity;
                        item.selling_price = itemSelling.selling_price;
                        item.description = itemSelling.description;
                    }
                }

                // restore item yang di delete
                var itemsDeleted = _listOfItemSellingDeleted.Where(f => f.Product != null && f.entity_state == EntityState.Deleted)
                                                         .ToArray();
                foreach (var item in itemsDeleted)
                {
                    item.entity_state = EntityState.Unchanged;
                    this._jual.item_jual.Add(item);
                }

                _listOfItemSellingDeleted.Clear();
            }

            base.Cancel();
        }

        public void Ok(object sender, object data)
        {
            if (data is Product) // pencarian product
            {
                var product = (Product)data;

                IPriceWholesaleBll hargaWholesaleBll = new PriceWholesaleBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                product.list_of_harga_grosir = hargaWholesaleBll.GetListPriceWholesale(product.product_id).ToList();

                if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                {
                    if (product.is_stock_minus)
                    {
                        //var msg = "Sorry stock product less.\n\n" +
                        //          "Stock saat ini: {0}";

                        var msg = " Key-CurrentStocks";
                        MsgHelper.MsgWarning(msg, product.remaining_stock.ToString());

                        GridListControlHelper.SelectCellText(this.gridControl, _rowIndex, 3);
                        return;
                    }
                }

                double discount = 0;
                if (_customer != null)
                {
                    discount = _customer.discount;
                }

                if (!(discount > 0))
                {
                    var diskonProduct = GetDiskonSellingFix(product, 1, product.discount);
                    discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                }

                ItemSellingProduct itemSelling = null;

                // cek item product already entered atau Not yet ?
                var itemProduct = GetExistItemProduct(product.product_id);

                if (itemProduct != null) // already there, tinggal update quantity
                {
                    var index = _listOfItemSelling.IndexOf(itemProduct);

                    UpdateItemProduct(this.gridControl, index);
                    this.gridControl.GetCellRenderer(_rowIndex, _colIndex).ControlText = string.Empty;

                    itemSelling = _listOfItemSelling[index];
                }
                else
                {
                    SetItemProduct(this.gridControl, _rowIndex, product, discount: discount);
                    itemSelling = _listOfItemSelling[_rowIndex - 1];

                    if (this.gridControl.RowCount == _rowIndex)
                    {
                        _listOfItemSelling.Add(new ItemSellingProduct());
                        this.gridControl.RowCount = _listOfItemSelling.Count;
                    }
                }

                this.gridControl.Refresh();
                RefreshTotal();
                DisplayItemProduct(itemSelling);

                if (_GeneralSupplier.is_show_additional_sales_item_information)
                {
                    // fokus ke column description
                    GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 4);
                }
                else
                {
                    if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                    {
                        GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 5); // fokus ke column quantity
                    }
                    else
                    {
                        if (itemProduct != null)
                            GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 2); // fokus ke column code
                        else
                            GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex + 1, 2); // fokus kerow berikutnya
                    }
                }
            }
            else if (data is Customer) // pencarian customer
            {
                this._customer = (Customer)data;
                txtCustomer.Text = this._customer.name_customer;
                KeyPressHelper.NextFocus();
            }
            else if (data is Dropshipper) // pencarian dropshipper
            {
                this._dropshipper = (Dropshipper)data;
                txtDropshipper.Text = this._dropshipper.name_dropshipper;
            }
            else if (data is AddressShipping)
            {
                var addressShipping = (AddressShipping)data;

                if (this._jual == null)
                    this._jual = new SellingProduct();

                this._jual.is_sdac = addressShipping.is_sdac;
                this._jual.shipping_to = addressShipping.to;
                this._jual.shipping_address = addressShipping.address;
                this._jual.shipping_subdistrict = addressShipping.subdistrict;
                this._jual.shipping_village = addressShipping.village;
            }
            else if (data is costs)
            {
                var ongkir = (costs)data;

                try
                {
                    cmbKurir.Text = string.Format("{0} {1}", ongkir.kurir_code, ongkir.service);
                    txtCostShipping.Text = ongkir.cost[0].value.ToString();
                }
                catch
                {
                }
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

        //private void txtCustomer_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (KeyPressHelper.IsEnter(e))
        //    {
        //        var customerName = ((AdvancedTextbox)sender).Text;

        //        ICustomerBll bll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
        //        var listOfCustomer = bll.GetByName(customerName);

        //        if (listOfCustomer.Count == 0)
        //        {
        //            MsgHelper.MsgWarning("Data customer not found");
        //            txtCustomer.Focus();
        //            txtCustomer.SelectAll();

        //        }
        //        else if (listOfCustomer.Count == 1)
        //        {
        //            _customer = listOfCustomer[0];
        //            txtCustomer.Text = _customer.name_customer;
        //            //int customerId;
        //            //if (int.TryParse(_customer.customer_id, out customerId))
        //            //{
        //            //    LoadQuotations(customerId);
        //            //}
        //            //else
        //            //{
        //            //    // Handle the case when _customer.customer_id is not a valid integer
        //            //    // Display an error message or take appropriate action
        //            //    MsgHelper.MsgWarning("customer value not loaded with quotation data entered");
        //            //}

        //            LoadQuotations(_customer.customer_id);
        //            KeyPressHelper.NextFocus();
        //        }
        //        else // data lebih dari one
        //        {
        //            var frmLookup = new FrmLookupReference("Data Customer", listOfCustomer);
        //            frmLookup.Listener = this;
        //            frmLookup.ShowDialog();
        //        }
        //    }
        //}

        private void txtCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var customerName = ((AdvancedTextbox)sender).Text;

                ICustomerBll bll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                var listOfCustomer = bll.GetByName(customerName);

                if (listOfCustomer.Count == 0)
                {
                    MsgHelper.MsgWarning("Customer Data not found");
                    txtCustomer.Focus();
                    txtCustomer.SelectAll();
                }
                else if (listOfCustomer.Count == 1)
                {
                    _customer = listOfCustomer[0];
                    txtCustomer.Text = _customer.name_customer;

                    // Load quotations based on customer ID
                    LoadQuotations(_customer.customer_id);

                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Data Customer", listOfCustomer);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();

                    if (_customer != null && _customer.customer_id != null)
                    {
                        // Load quotations based on customer ID
                        LoadQuotations(_customer.customer_id);
                    }
                    else
                    {
                        MsgHelper.MsgWarning("Please select a customer");
                    }

                }
            }
        }


        private void txtCustomer_Validated(object sender, EventArgs e)
        {
            var customerName = txtCustomer.Text;

            ICustomerBll bll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var listOfCustomer = bll.GetByName(customerName);

            if (listOfCustomer.Count == 1)
            {
                _customer = listOfCustomer[0];
                LoadQuotations(_customer.customer_id);
            }
        }

        //private void txtCustomer_LostFocus(object sender, EventArgs e)
        //{
        //    var customerName = txtCustomer.Text;

        //    ICustomerBll bll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
        //    var listOfCustomer = bll.GetByName(customerName);

        //    if (listOfCustomer.Count == 1)
        //    {
        //        _customer = listOfCustomer[0];
        //        LoadQuotations(_customer.customer_id);
        //    }
        //}

        //private void txtCustomer_TextChanged(object sender, EventArgs e)
        //{
        //    LoadQuotations(_customer.customer_id);
        //}


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbQuotation.SelectedIndex > 0)
            {
                PopulateGridWithProductDetails();
            }
        }

        //  comboBox1_SelectedIndexChanged
        private void PopulateGridWithProductDetails()
        {
            if (cmbQuotation.SelectedItem != null)
            {
                string selectedQuotation = cmbQuotation.SelectedItem.ToString();

                // Call the GetProductDetailsByQUotation method and receive the list of product details
                List<ItemSellingQuotation> productDetails = _bll.GetProductDetailsByQUotation(selectedQuotation);

                // Clear the existing data in the grid control
                _listOfItemSelling.Clear();

                // Add the new product details to the list
                foreach (var productDetail in productDetails)
                {
                    _listOfItemSelling.Add(new ItemSellingProduct
                    {
                        product_id = productDetail.product_id,
                        Product = productDetail.Product,
                        quantity = productDetail.quantity,
                        selling_price = productDetail.selling_price,
                        discount = productDetail.Product.discount,

                        // Assign other properties as needed based on the available data in the productDetail object
                    });
                }

                // Refresh the grid control
                gridControl.RowCount = _listOfItemSelling.Count();
                gridControl.Refresh();

                RefreshTotal();
            }
        }

        private void SetItemProduct(GridControl grid, int rowIndex, Product product,
            double quantity = 1, double price = 0, double discount = 0, string description = "")
        {
            ItemSellingProduct itemSelling;

            if (_isNewData)
            {
                itemSelling = new ItemSellingProduct();
            }
            else
            {
                itemSelling = _listOfItemSelling[rowIndex - 1];

                if (itemSelling.entity_state == EntityState.Unchanged)
                    itemSelling.entity_state = EntityState.Modified;
            }

            itemSelling.product_id = product.product_id;
            itemSelling.Product = product;
            itemSelling.description = description;
            itemSelling.quantity = quantity;
            itemSelling.purchase_price = product.purchase_price;
            itemSelling.selling_price = price > 0 ? price : product.selling_price;
            itemSelling.discount = discount;

            _listOfItemSelling[rowIndex - 1] = itemSelling;
        }

        private void UpdateItemProduct(GridControl grid, int rowIndex)
        {
            var itemSelling = _listOfItemSelling[rowIndex];

            if (itemSelling.entity_state == EntityState.Unchanged)
                itemSelling.entity_state = EntityState.Modified;

            itemSelling.quantity += 1;
            itemSelling.discount = GetDiskonSellingFix(itemSelling.Product, itemSelling.quantity, itemSelling.discount);
            itemSelling.selling_price = GetPriceSellingFix(itemSelling.Product, itemSelling.quantity, itemSelling.selling_price);

            _listOfItemSelling[rowIndex] = itemSelling;
        }

        private ItemSellingProduct GetExistItemProduct(string produkId)
        {
            var obj = _listOfItemSelling.Where(f => f.product_id == produkId)
                                     .FirstOrDefault();
            return obj;
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
                                MsgHelper.MsgWarning("Product data not found");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                return;
                            }

                            if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                            {
                                if (product.is_stock_minus)
                                {
                                    var msg = " Key-CurrentStocks";

                                    MsgHelper.MsgWarning(msg, product.remaining_stock.ToString());

                                    GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                    return;
                                }
                            }

                            double discount = 0;

                            if (_customer != null)
                            {
                                discount = _customer.discount;
                            }

                            if (!(discount > 0))
                            {
                                var diskonProduct = GetDiskonSellingFix(product, 1, product.discount);
                                discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                            }

                            ItemSellingProduct itemSelling = null;

                            // cek item product already entered atau Not yet ?
                            var itemProduct = GetExistItemProduct(product.product_id);

                            if (itemProduct != null) // already there, tinggal update quantity
                            {
                                var index = _listOfItemSelling.IndexOf(itemProduct);

                                UpdateItemProduct(grid, index);
                                cc.Renderer.ControlText = string.Empty;

                                itemSelling = _listOfItemSelling[index];
                            }
                            else
                            {
                                SetItemProduct(grid, rowIndex, product, discount: discount);
                                itemSelling = _listOfItemSelling[rowIndex - 1];

                                if (grid.RowCount == rowIndex)
                                {
                                    _listOfItemSelling.Add(new ItemSellingProduct());
                                    grid.RowCount = _listOfItemSelling.Count;
                                }
                            }

                            grid.Refresh();
                            RefreshTotal();
                            DisplayItemProduct(itemSelling);

                            if (_GeneralSupplier.is_show_additional_sales_item_information)
                            {
                                // fokus ke column description
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, 4);
                            }
                            else
                            {
                                if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                                {
                                    GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // fokus ke column quantity
                                }
                                else
                                {
                                    GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                                }
                            }
                        }

                        break;

                    case 3: // pencarian based name product

                        cc = grid.CurrentCell;
                        var nameProduct = cc.Renderer.ControlValue.ToString();
                        if (nameProduct.Length == 0)
                        {
                            MsgHelper.MsgWarning("Product name cannot be empty");
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);

                            return;
                        }

                        var listOfProduct = bll.GetByName(nameProduct, false);

                        if (listOfProduct.Count == 0)
                        {
                            MsgHelper.MsgWarning("Product data not found");
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                        }
                        else if (listOfProduct.Count == 1)
                        {
                            product = listOfProduct[0];

                            IPriceWholesaleBll hargaWholesaleBll = new PriceWholesaleBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                            product.list_of_harga_grosir = hargaWholesaleBll.GetListPriceWholesale(product.product_id).ToList();

                            if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                            {
                                if (product.is_stock_minus)
                                {
                                    var msg = " Key-CurrentStocks";

                                    MsgHelper.MsgWarning(msg, product.remaining_stock.ToString());
                                    GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                    return;
                                }
                            }

                            double discount = 0;

                            if (_customer != null)
                            {
                                discount = _customer.discount;
                            }

                            if (!(discount > 0))
                            {
                                var diskonProduct = GetDiskonSellingFix(product, 1, product.discount);
                                discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                            }

                            ItemSellingProduct itemSelling = null;

                            // cek item product already entered atau Not yet ?
                            var itemProduct = GetExistItemProduct(product.product_id);

                            if (itemProduct != null) // already there, tinggal update quantity
                            {
                                var index = _listOfItemSelling.IndexOf(itemProduct);

                                UpdateItemProduct(grid, index);
                                cc.Renderer.ControlText = string.Empty;

                                itemSelling = _listOfItemSelling[index];
                            }
                            else
                            {
                                SetItemProduct(grid, rowIndex, product, discount: discount);

                                itemSelling = _listOfItemSelling[rowIndex - 1];

                                if (grid.RowCount == rowIndex)
                                {
                                    _listOfItemSelling.Add(new ItemSellingProduct());
                                    grid.RowCount = _listOfItemSelling.Count;
                                }
                            }

                            grid.Refresh();
                            RefreshTotal();
                            DisplayItemProduct(itemSelling);

                            if (_GeneralSupplier.is_show_additional_sales_item_information)
                            {
                                // fokus ke column description
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, 4);
                            }
                            else
                            {
                                if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                                {
                                    GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // fokus ke column quantity
                                }
                                else
                                {
                                    GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                                }
                            }
                        }
                        else // data lebih dari one
                        {
                            _rowIndex = rowIndex;
                            _colIndex = colIndex;

                            var frmLookup = new FrmLookupReference("Data Product", listOfProduct);
                            frmLookup.Listener = this;
                            frmLookup.ShowDialog();
                        }

                        break;

                    case 4: // description
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                        {
                            GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // fokus ke column quantity
                        }
                        else
                        {
                            GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                        }

                        break;

                    case 5: // quantity
                        if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                        {
                            gridControl_CurrentCellValidated(sender, new EventArgs());

                            var itemSelling = _listOfItemSelling[rowIndex - 1];
                            product = itemSelling.Product;

                            var isValidStock = (product.remaining_stock + itemSelling.old_jumlah - itemSelling.quantity) >= 0;

                            if (!isValidStock)
                            {
                                var msg = "Key-RemainingStock";
                                object[] params1 = new object[] { product.remaining_stock, itemSelling.quantity, product.remaining_stock + itemSelling.old_jumlah - itemSelling.quantity };
                                MsgHelper.MsgWarnings(msg, params1);
                               
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);

                                return;
                            }
                        }

                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                        break;

                    case 6: // discount
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                        break;

                    case 7:
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // fokus ke column code product
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
                case 5: // quantity
                case 6: // discount
                case 7: // price
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

            var itemSelling = _listOfItemSelling[cc.RowIndex - 1];
            var product = itemSelling.Product;

            if (product != null)
            {
                switch (cc.ColIndex)
                {
                    case 4: // column description
                        itemSelling.description = cc.Renderer.ControlValue.ToString();
                        break;

                    case 5: // column quantity
                        itemSelling.quantity = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);

                        itemSelling.discount = GetDiskonSellingFix(product, itemSelling.quantity, itemSelling.discount);
                        itemSelling.selling_price = GetPriceSellingFix(product, itemSelling.quantity, itemSelling.selling_price);
                        break;

                    case 6: // column discount
                        itemSelling.discount = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    case 7: // column price
                        itemSelling.selling_price = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    default:
                        break;
                }

                SetItemProduct(grid, cc.RowIndex, product, itemSelling.quantity, itemSelling.selling_price, itemSelling.discount, itemSelling.description);
                grid.Refresh();

                RefreshTotal();

                if (cc.ColIndex == 5 || cc.ColIndex == 6 || cc.ColIndex == 7)
                {
                    itemSelling = _listOfItemSelling[cc.RowIndex - 1];
                    DisplayItemProduct(itemSelling);
                }
            }
        }

        private void gridControl_KeyDown(object sender, KeyEventArgs e)
        {
            Shortcut(sender, e);
        }

        private void FrmEntrySalesProduct_KeyDown(object sender, KeyEventArgs e)
        {
            Shortcut(sender, e);
        }

        private void Shortcut(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsShortcutKey(Keys.F1, e)) // tambah data product
            {
                ShowEntryProduct();
            }
            else if (KeyPressHelper.IsShortcutKey(Keys.F2, e)) // Optional data customer
            {
                // kasus khusus untuk shortcut F2, tidak jalan jika dipanggil melalui event Form KeyDown, 
                // must di panggil di event gridControl_KeyDown
                ShowEntryCustomer();
            }
            else if (KeyPressHelper.IsShortcutKey(Keys.F3, e)) // Optional data dropshipper
            {
                ShowEntryDropshipper();
            }
            else if (KeyPressHelper.IsShortcutKey(Keys.F5, e) || KeyPressHelper.IsShortcutKey(Keys.F6, e) || KeyPressHelper.IsShortcutKey(Keys.F7, e))
            {
                var colIndex = 5;
                var rowIndex = this.gridControl.CurrentCell.RowIndex;

                switch (e.KeyCode)
                {
                    case Keys.F5: // edit quantity
                        colIndex = 5;
                        break;

                    case Keys.F6: // edit discount
                        colIndex = 6;
                        break;

                    case Keys.F7: // edit price
                        colIndex = 7;
                        break;

                    default:
                        break;
                }

                if (gridControl.RowCount > 1 && gridControl.RowCount == rowIndex)
                {
                    gridControl.Focus();
                    GridListControlHelper.SetCurrentCell(gridControl, _listOfItemSelling.Count - 1, colIndex);
                }
            }
            else if (KeyPressHelper.IsShortcutKey(Keys.F8, e)) // pay
            {
                txtJumlahPay.Text = "0";
                txtReturn.Text = "0";
                txtJumlahPay.Focus();
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

        private void ShowEntryCustomer()
        {
            var isGrant = RolePrivilegeHelper.IsHaveRightAccess("mnuCustomer", _user);
            if (!isGrant)
            {
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
                return;
            }

            ICustomerBll customerBll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var frmEntryCustomer = new FrmEntryCustomer("Add Data Customer", customerBll);
            frmEntryCustomer.Listener = this;
            frmEntryCustomer.ShowDialog();
        }

        private void ShowEntryDropshipper()
        {
            var isGrant = RolePrivilegeHelper.IsHaveRightAccess("mnuDropshipper", _user);
            if (!isGrant)
            {
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
                return;
            }

            IDropshipperBll dropshipperBll = new DropshipperBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var frmEntryDropshipper = new FrmEntryDropshipper("Add Data Dropshipper", dropshipperBll);
            frmEntryDropshipper.Listener = this;
            frmEntryDropshipper.ShowDialog();
        }

        private void txtCostshipping_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void txtDiskon_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void txtPPN_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void FrmEntrySalesProduct_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hapus objek dumm
            if (!_isNewData)
            {
                var itemsToRemove = _jual.item_jual.Where(f => f.Product == null && f.entity_state == EntityState.Added)
                                                   .ToArray();

                foreach (var item in itemsToRemove)
                {
                    _jual.item_jual.Remove(item);
                }
            }
        }

        private void btnSetAddressshipping_Click(object sender, EventArgs e)
        {
            if (this._customer == null || txtCustomer.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Customer' should not  empty !");
                txtCustomer.Focus();

                return;
            }

            var frmEntryAddressShipping = new FrmEntryAddressShipping("Address Shipping", this._customer, this._jual);
            frmEntryAddressShipping.Listener = this;
            frmEntryAddressShipping.ShowDialog();
        }

        private void btnPreviewInvoice_Click(object sender, EventArgs e)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                PreviewInvoice();
            }
        }

        private void PreviewInvoice()
        {
            if (this._customer == null || txtCustomer.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Customer' should not empty !");
                txtCustomer.Focus();

                return;
            }

            var total = SumGrid(this._listOfItemSelling);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You haven't completed the product data input yet!");
                return;
            }

            if (total > 0)
            {
                total += NumberHelper.StringToDouble(txtDiskon.Text);
                total -= NumberHelper.StringToDouble(txtCostShipping.Text);
                total -= NumberHelper.StringToDouble(txtPPN.Text);
            }

            if (this._jual == null)
            {
                this._jual = new SellingProduct();
            }

            var listOfItemInvoice = new List<InvoiceSales>();

            foreach (var item in this._listOfItemSelling.Where(f => f.Product != null))
            {
                var itemInvoice = new InvoiceSales
                {
                    name_customer = this._customer.name_customer,
                    address = string.IsNullOrEmpty(this._customer.address) ? "" : this._customer.address,
                    cr_no = this._customer.cr_no,
                    vat_no = this._customer.vat_no,
                    provinsi = this._customer.Provinsi != null ? this._customer.Provinsi.name_province : string.Empty,
                    regency = this._customer.Regency != null ? this._customer.Regency.name_regency : string.Empty,
                    subdistrict = this._customer.subdistrict != null ? this._customer.subdistrict.name_subdistrict : string.Empty,

                    postal_code = (string.IsNullOrEmpty(this._customer.postal_code) || this._customer.postal_code == "0") ? "-" : this._customer.postal_code,
                    contact = string.IsNullOrEmpty(this._customer.contact) ? "" : this._customer.contact,
                    phone = string.IsNullOrEmpty(this._customer.phone) ? "-" : this._customer.phone,
                    invoice = txtInvoice.Text,
                    date = dtpDate.Value,
                    tax = NumberHelper.StringToDouble(txtPPN.Text),
                    diskon_nota = NumberHelper.StringToDouble(txtDiskon.Text),
                    courier = cmbKurir.Text,
                    shipping_cost = NumberHelper.StringToDouble(txtCostShipping.Text),
                    total_invoice = total,
                    sub_total_with_tax = total + NumberHelper.StringToDouble(txtPPN.Text),
                    is_sdac = this._jual.is_sdac,
                    shipping_to = string.IsNullOrEmpty(this._jual.shipping_to) ? "" : this._jual.shipping_to,
                    shipping_address = string.IsNullOrEmpty(this._jual.shipping_address) ? "" : this._jual.shipping_address,
                    shipping_village = this._jual.shipping_village.NullToString(),
                    shipping_subdistrict = this._jual.shipping_subdistrict.NullToString(),
                    shipping_regency = this._jual.shipping_regency.NullToString(),
                    shipping_city = this._jual.shipping_city.NullToString(),
                    shipping_country = string.IsNullOrEmpty(this._jual.shipping_country) ? "-" : this._jual.shipping_country,
                    shipping_postal_code = string.IsNullOrEmpty(this._jual.shipping_postal_code) ? "-" : this._jual.shipping_postal_code,
                    shipping_phone = string.IsNullOrEmpty(this._jual.shipping_phone) ? "-" : this._jual.shipping_phone,
                    from_label1 = string.IsNullOrEmpty(this._jual.from_label1) ? "" : this._jual.from_label1,
                    from_label2 = string.IsNullOrEmpty(this._jual.from_label2) ? "" : this._jual.from_label1,
                    to_label1 = string.IsNullOrEmpty(this._jual.to_label1) ? "" : this._jual.to_label1,
                    to_label2 = string.IsNullOrEmpty(this._jual.to_label2) ? "" : this._jual.to_label2,
                    to_label3 = string.IsNullOrEmpty(this._jual.to_label3) ? "" : this._jual.to_label3,
                    to_label4 = string.IsNullOrEmpty(this._jual.to_label4) ? "" : this._jual.to_label4,
                    product_code = item.Product.product_code,
                    product_name = item.Product.product_name,
                    unit = item.Product.unit,
                    price = item.selling_price,
                    quantity = item.quantity,
                    return_quantity = item.return_quantity,
                    discount = item.discount,
                    

                };

                itemInvoice.due_date = DateTimeHelper.GetNullDateTime();
                if (rdoKredit.Checked)
                    itemInvoice.due_date = dtpDateCreditTerm.Value;

                if (string.IsNullOrEmpty(itemInvoice.to_label1))
                    itemInvoice.to_label1 = this._customer.name_customer;

                if (string.IsNullOrEmpty(itemInvoice.to_label2))
                    itemInvoice.to_label2 = this._customer.address;

                if (string.IsNullOrEmpty(itemInvoice.to_label3))
                    itemInvoice.to_label3 = "HP: " + this._customer.phone;

                listOfItemInvoice.Add(itemInvoice);
            }

            var reportDataSource = new ReportDataSource
            {
                Name = "InvoiceSales",
                Value = listOfItemInvoice
            };

            // set header invoice
            var parameters = new List<ReportParameter>();
            var index = 1;

            foreach (var item in _GeneralSupplier.list_of_header_nota)
            {
                var paramName = string.Format("header{0}", index);
                parameters.Add(new ReportParameter(paramName, item.description));

                index++;
            }

            foreach (var item in listOfItemInvoice)
            {
                if (item.from_label1.Length == 0)
                    item.from_label1 = this._GeneralSupplier.list_of_label_nota[0].description;

                if (item.from_label2.Length == 0)
                    item.from_label2 = this._GeneralSupplier.list_of_label_nota[1].description;
            }

            // set footer invoice
            var dt = DateTime.Now;
            var cityAndDate = string.Format("{0}, {1}", _profil.city, dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);

            parameters.Add(new ReportParameter("city", cityAndDate));
            parameters.Add(new ReportParameter("footer", _user.name_user));

            var reportName = chkDropship.Checked ? "RvInvoiceSalesProductTanpaLabelDropship" : "RvInvoiceSalesProductTanpaLabel";

            var frmPreviewReport = new FrmPreviewReport("Preview Invoice Sales", reportName, reportDataSource, parameters, true);
            frmPreviewReport.ShowDialog();
        }

        private void btnCekOngkir_Click(object sender, EventArgs e)
        {
            var frmCekOngkir = new FrmLookupShippingCost("Check Cost Shipping");
            frmCekOngkir.Listener = this;
            frmCekOngkir.ShowDialog();
        }

        private void cmbKurir_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                KeyPressHelper.NextFocus();
        }

        private void txtJumlahPay_Enter(object sender, EventArgs e)
        {
            if (lblTotal.Text != "0")
                DisplayTotal(lblTotal.Text);
        }

        private void txtJumlahPay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                txtReturn.Text = "0";

                var total = NumberHelper.StringToNumber(lblTotal.Text);
                if (total > 0)
                {
                    var jumlahPay = NumberHelper.StringToNumber(((AdvancedTextbox)sender).Text);
                    var return1 = jumlahPay - total;

                    if (return1 >= 0)
                    {
                        txtReturn.Text = return1.ToString();

                        DisplayRefund(txtReturn.Text);
                        tmrDisplayKalimatPenutup.Enabled = true;
                    }
                    else
                    {
                        MsgHelper.MsgWarning("quantity pay less !");
                        txtJumlahPay.Focus();
                        txtJumlahPay.SelectAll();
                    }
                }
            }
        }

        private void chkDropship_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            lblDropshipper.Visible = chk.Checked;
            txtDropshipper.Visible = chk.Checked;

            if (chk.Checked)
                KeyPressHelper.NextFocus();
        }

        private void txtDropshipper_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var dropshipperName = ((AdvancedTextbox)sender).Text;

                IDropshipperBll bll = new DropshipperBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                var listOfDropshipper = bll.GetByName(dropshipperName);

                if (listOfDropshipper.Count == 0)
                {
                    MsgHelper.MsgWarning("Data dropshipper not found");
                    txtDropshipper.Focus();
                    txtDropshipper.SelectAll();

                }
                else if (listOfDropshipper.Count == 1)
                {
                    _dropshipper = listOfDropshipper[0];
                    txtDropshipper.Text = _dropshipper.name_dropshipper;
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Data Dropshipper", listOfDropshipper);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }

        private void tmrDisplayKalimatPenutup_Tick(object sender, EventArgs e)
        {
            DisplayKalimatPenutup();
            ((Timer)sender).Enabled = false;
        }

        private void gridControl_CellClick(object sender, GridCellClickEventArgs e)
        {

        }

        private void FrmEntrySalesProduct_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {

        }







        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //                        {
        //    PopulateGridWithProductDetails();
        //}

        ////private void PopulateGridWithProductDetails()
        ////{
        ////    string selectedQuotation = cmbQuotation.SelectedItem.ToString();

        ////    // Call the GetProductDetailsByQUotation method and receive the list of product details
        ////    List<ItemSellingQuotation> productDetails = _bll.GetProductDetailsByQUotation(selectedQuotation);

        ////    // Clear the existing data in the grid control
        ////    gridControl.RowCount = 0;

        ////    // Populate the grid control with the product details
        ////    int rowIndex = 1; // Start from 1 to skip the header row
        ////    foreach (var productDetail in productDetails)
        ////    {
        ////        // Check if productDetail and its Product property are not null
        ////        if (productDetail != null && productDetail.Product != null)
        ////        {
        ////            // Add a new row to the grid control
        ////            gridControl.RowCount++;

        ////            // Populate the cells in the new row
        ////            gridControl[rowIndex, 0].CellValue = rowIndex.ToString(); // No
        ////            gridControl[rowIndex, 1].CellValue = productDetail.Product.product_code; // Code Product
        ////            gridControl[rowIndex, 2].CellValue = productDetail.Product.product_name; // Name Product
        ////            gridControl[rowIndex, 3].CellValue = productDetail.quantity.ToString(); // Quantity
        ////            gridControl[rowIndex, 4].CellValue = productDetail.Product.discount.ToString(); // Discount
        ////            gridControl[rowIndex, 5].CellValue = productDetail.selling_price.ToString(); // Price
        ////            gridControl[rowIndex, 6].CellValue = (productDetail.Product.selling_price * productDetail.quantity).ToString(); // Sub Total

        ////            rowIndex++;
        ////        }
        ////        else
        ////        {
        ////            // Handle the case when productDetail or its Product property is null or missing required properties
        ////            // You can log an error or display a message to the user
        ////            MsgHelper.MsgWarning("Invalid product details: " + productDetail?.ToString());
        ////        }
        ////    }
        ////}

        //private void PopulateGridWithProductDetails()
        //{
        //    string selectedQuotation = cmbQuotation.SelectedItem.ToString();

        //    // Call the GetProductDetailsByQUotation method and receive the list of product details
        //    List<ItemSellingQuotation> productDetails = _bll.GetProductDetailsByQUotation(selectedQuotation);

        //    // Clear the existing data in the grid control
        //    _listOfItemSelling.Clear();

        //    // Add the new product details to the list
        //    foreach (var productDetail in productDetails)
        //    {
        //        _listOfItemSelling.Add(new ItemSellingProduct
        //        {
        //            product_id = productDetail.product_id,
        //            Product = productDetail.Product,
        //            quantity = productDetail.quantity,
        //            selling_price = productDetail.selling_price,
        //            discount = productDetail.Product.discount,

        //            // Assign other properties as needed based on the available data in the productDetail object
        //        });
        //    }

        //    // Refresh the grid control
        //    gridControl.RowCount = _listOfItemSelling.Count();
        //    gridControl.Refresh();

        //    RefreshTotal();
        //}



        // actually my control is  private Syncfusion.Windows.Forms.Grid.GridControl gridControl; i want base my process base gridcontrol

        //private void comboDropDown1_Click(object sender, EventArgs e)
        //{

        //}

        //private void comboDropDown1_Click(object sender, EventArgs e)
        //{
        //    // Get the customer ID from wherever it is stored
        //    int customerId = GetCustomerId();

        //    // Retrieve quotation data from the database based on the customer ID
        //    List<string> quotations = GetQuotationsByCustomerId(customerId);

        //    // Clear existing items in the dropdown
        //    cmbQuotation.Items.Clear();

        //    // Add each quotation to the dropdown
        //    foreach (string quotation in quotations)
        //    {
        //        cmbQuotation.Items.Add(quotation);
        //    }
        //}




    }
}
