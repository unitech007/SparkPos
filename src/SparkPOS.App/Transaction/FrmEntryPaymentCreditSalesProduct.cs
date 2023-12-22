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
    public partial class FrmEntryPaymentCreditSalesProduct : FrmEntryStandard, IListener
    {        
        private IPaymentCreditProductBll _bll = null;
        private PaymentCreditProduct _paymentCredit = null;
        private Customer _customer = null;
        private IList<ItemPaymentCreditProduct> _listOfItemPaymentCredit = new List<ItemPaymentCreditProduct>();
        private IList<ItemPaymentCreditProduct> _listOfItemPaymentCreditOld = new List<ItemPaymentCreditProduct>();
        private IList<ItemPaymentCreditProduct> _listOfItemPaymentCreditDeleted = new List<ItemPaymentCreditProduct>();
        
        private int _rowIndex = 0;
        private int _colIndex = 0;

        private bool _isNewData = false;
        private ILog _log;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntryPaymentCreditSalesProduct(string header, IPaymentCreditProductBll bll) 
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

            _listOfItemPaymentCredit.Add(new ItemPaymentCreditProduct()); // add dummy objek
            
            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryPaymentCreditSalesProduct(string header, PaymentCreditProduct paymentCredit, IPaymentCreditProductBll bll)
            : base()
        {
             InitializeComponent();  
          
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._paymentCredit = paymentCredit;
            this._customer = paymentCredit.Customer;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = this._paymentCredit.invoice;
            dtpDate.Value = (DateTime)this._paymentCredit.date;

            if (this._customer != null)
                txtCustomer.Text = this._customer.name_customer;

            txtKeterangan.Text = this._paymentCredit.description;
            
            // save data lama
            _listOfItemPaymentCreditOld.Clear();
            foreach (var item in this._paymentCredit.item_payment_credit)
            {
                _listOfItemPaymentCreditOld.Add(new ItemPaymentCreditProduct
                {
                    pay_sale_item_id = item.pay_sale_item_id,
                    amount = item.amount,
                    description = item.description
                });
            }
            
            _listOfItemPaymentCredit = this._paymentCredit.item_payment_credit;
            _listOfItemPaymentCredit.Add(new ItemPaymentCreditProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
            RefreshTotal();
        }

        private void InitGridControl(GridControl grid)
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice Selling", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Total", Width = 100, IsEditable = false, HorizontalAlignment = GridHorizontalAlignment.Right });
            gridListProperties.Add(new GridListControlProperties { Header = "Shortage", Width = 100, IsEditable = false, HorizontalAlignment = GridHorizontalAlignment.Right });
            gridListProperties.Add(new GridListControlProperties { Header = "Payment", Width = 100, HorizontalAlignment = GridHorizontalAlignment.Right });
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 200 });
            gridListProperties.Add(new GridListControlProperties { Header = "Action" });

            GridListControlHelper.InitializeGridListControl<ItemPaymentCreditProduct>(grid, _listOfItemPaymentCredit, gridListProperties);

            grid.PushButtonClick += delegate(object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.ColIndex == 7)
                {
                    if (grid.RowCount == 1)
                    {
                        MsgHelper.MsgWarning("Minimum 1 invoice must entered !");
                        return;
                    }

                    if (MsgHelper.MsgDelete())
                    {
                        var paymentCredit = _listOfItemPaymentCredit[e.RowIndex - 1];
                        paymentCredit.entity_state = EntityState.Deleted;

                        _listOfItemPaymentCreditDeleted.Add(paymentCredit);
                        _listOfItemPaymentCredit.Remove(paymentCredit);

                        grid.RowCount = _listOfItemPaymentCredit.Count();
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
                    if (!(_listOfItemPaymentCredit.Count > 0))
                        return;

                    double grand_total = 0;
                    double RemainingInvoice = 0;

                    var itemPayment = _listOfItemPaymentCredit[e.RowIndex - 1];
                    var sale = itemPayment.SellingProduct;
                    if (sale != null)
                    {
                        grand_total = sale.grand_total;
                        RemainingInvoice = sale.remaining_nota;
                    }

                    switch (e.ColIndex)
                    {
                        case 1: // no urut
                            e.Style.CellValue = e.RowIndex.ToString();
                            break;

                        case 2: // invoice sale
                            if (sale != null)
                                e.Style.CellValue = sale.invoice;

                            if (sale != null)
                            {
                                if (sale.due_date.IsNull()) // invoice cash amountnya cannot be edited
                                {
                                    e.Style.Enabled = false;
                                    e.Style.BackColor = ColorCollection.DEFAULT_FORM_COLOR;
                                    base.SetButtonSaveToFalse(true);
                                }
                            }

                            break;

                        case 3: // total
                            e.Style.CellValue = NumberHelper.NumberToString(grand_total);

                            break;

                        case 4: // Shortage
                            e.Style.CellValue = NumberHelper.NumberToString(RemainingInvoice);

                            break;

                        case 5: // payment
                            if (sale != null)
                            {
                                if (sale.due_date.IsNull()) // invoice cash amountnya cannot be edited
                                {
                                    e.Style.Enabled = false;
                                    e.Style.BackColor = ColorCollection.DEFAULT_FORM_COLOR;
                                }
                            }

                            e.Style.CellValue = NumberHelper.NumberToString(itemPayment.amount);

                            break;

                        case 6: // description
                            e.Style.CellValue = itemPayment.description;

                            break;

                        case 7: // button hapus
                            e.Style.CellType = GridCellTypeName.PushButton;
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            if (MainProgram.currentLanguage == "en-US")
                            {
                                e.Style.Description = "Delete";
                            }
                            else if (MainProgram.currentLanguage == "ar-SA")
                            {
                                e.Style.Description = "يمسح";
                            }

                            if (sale != null)
                            {
                                e.Style.Enabled = !sale.due_date.IsNull();
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

        private double SumGrid(IList<ItemPaymentCreditProduct> listOfItemPaymentCredit)
        {
            double total = 0;
            foreach (var item in listOfItemPaymentCredit.Where(f => f.SellingProduct != null))
            {
                total += item.amount;
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemPaymentCredit));
        }

        protected override void Save()
        {
            if (this._customer == null || txtCustomer.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Customer' should not empty !");
                txtCustomer.Focus();

                return;
            }

            var total = SumGrid(this._listOfItemPaymentCredit);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You have not completed the payment data input !");
                return;
            }

            if (!MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
                return;

            if (_isNewData)
                _paymentCredit = new PaymentCreditProduct();

            _paymentCredit.user_id = this._user.user_id;
            _paymentCredit.User = this._user;
            _paymentCredit.customer_id = this._customer.customer_id;
            _paymentCredit.Customer = this._customer;
            _paymentCredit.invoice = txtInvoice.Text;
            _paymentCredit.date = dtpDate.Value;
            _paymentCredit.description = txtKeterangan.Text;

            _paymentCredit.item_payment_credit = this._listOfItemPaymentCredit.Where(f => f.SellingProduct != null).ToList();

            if (!_isNewData) // update
                _paymentCredit.item_payment_credit_deleted = _listOfItemPaymentCreditDeleted.ToList();

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                {
                    result = _bll.Save(_paymentCredit, false, ref validationError);
                }
                else
                {
                    result = _bll.Update(_paymentCredit, false, ref validationError);
                }

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _paymentCredit);

                    _customer = null;
                    _listOfItemPaymentCredit.Clear();
                    _listOfItemPaymentCreditDeleted.Clear();                                        

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
                var itemsModified = _paymentCredit.item_payment_credit.Where(f => f.SellingProduct != null && f.entity_state == EntityState.Modified)
                                                     .ToArray();

                foreach (var item in itemsModified)
                {
                    var itemPayment = _listOfItemPaymentCreditOld.Where(f => f.pay_sale_item_id == item.pay_sale_item_id)
                                                                       .SingleOrDefault();                    
                    if (itemPayment != null)
                    {
                        item.amount = itemPayment.amount;
                        item.description = itemPayment.description;
                    }
                }

                // restore item yang di delete
                var itemsDeleted = _listOfItemPaymentCreditDeleted.Where(f => f.SellingProduct != null && f.entity_state == EntityState.Deleted)
                                                                     .ToArray();
                foreach (var item in itemsDeleted)
                {
                    item.entity_state = EntityState.Unchanged;
                    this._paymentCredit.item_payment_credit.Add(item);
                }

                _listOfItemPaymentCreditDeleted.Clear();
            }

            base.Cancel();
        }

        public void Ok(object sender, object data)
        {
            if (data is SellingProduct) // pencarian invoice
            {
                var sale = (SellingProduct)data;

                if (!IsExist(sale.invoice))
                {
                    SetItemPay(this.gridControl, _rowIndex, _colIndex + 1, sale);
                    this.gridControl.Refresh();

                    GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 5); // column payment
                }
                else
                {
                    MsgHelper.MsgWarning("Data invoice already entered");
                    GridListControlHelper.SelectCellText(this.gridControl, _rowIndex, _colIndex);
                }
            }
            else if (data is Customer) // pencarian customer
            {
                this._customer = (Customer)data;
                txtCustomer.Text = this._customer.name_customer;
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

                ICustomerBll bll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                var listOfCustomer = bll.GetByName(name);

                if (listOfCustomer.Count == 0)
                {
                    MsgHelper.MsgWarning("Data customer not found");
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

        private bool IsExist(string invoice)
        {
            var count = _listOfItemPaymentCredit.Where(f => f.SellingProduct != null && f.SellingProduct.invoice.ToLower() == invoice.ToLower())
                                                   .Count();

            return (count > 0);
        }

        private void gridControl_CurrentCellKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var grid = (GridControl)sender;

                var rowIndex = grid.CurrentCell.RowIndex;
                var colIndex = grid.CurrentCell.ColIndex;                

                switch (colIndex)
                {
                    case 2: // column invoice
                        if (this._customer == null || txtCustomer.Text.Length == 0)
                        {
                            MsgHelper.MsgWarning("Name Customer Not yet entered");
                            txtCustomer.Focus();
                            return;
                        }

                        var cc = grid.CurrentCell;
                        var invoice = cc.Renderer.ControlValue.ToString();

                        IList<SellingProduct> listOfSelling = null;
                        ISellingProductBll bll = new SellingProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

                        if (invoice.Length > 0) // menampilkan invoice Credit based invoice
                        {
                            listOfSelling = bll.GetInvoiceKreditByInvoice(this._customer.customer_id, invoice);
                        }
                        else
                        {
                            listOfSelling = bll.GetInvoiceKreditByCustomer(this._customer.customer_id, false);
                        }

                        if (listOfSelling.Count == 0)
                        {
                            MsgHelper.MsgWarning("Data sales Credit not found");
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                        }
                        else if (listOfSelling.Count == 1)
                        {
                            var sale = listOfSelling[0];

                            if (!IsExist(sale.invoice))
                            {
                                SetItemPay(grid, rowIndex, colIndex, sale);
                                grid.Refresh();

                                GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // column payment
                            }
                            else
                            {
                                MsgHelper.MsgWarning("Data payment already entered");
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                            }
                        }
                        else // data invoice lebih dari one, tampilkan di form lookup
                        {
                            _rowIndex = rowIndex;
                            _colIndex = colIndex;

                            var frmLookup = new FrmLookupInvoice("Data Invoice Sales", listOfSelling);
                            frmLookup.Listener = this;
                            frmLookup.ShowDialog();
                        }

                        break;

                    case 6: // description
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemPaymentCredit.Add(new ItemPaymentCreditProduct());
                            grid.RowCount = _listOfItemPaymentCredit.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, rowIndex + 1, 2); // fokus ke column invoice sale
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

            switch (cc.ColIndex)
            {
                case 5: // payment
                    e.Handled = KeyPressHelper.NumericOnly(e);
                    break;

                default:
                    break;
            }
        }

        private void SetItemPay(GridControl grid, int rowIndex, int colIndex, SellingProduct sale, double amount = 0, string description = "")
        {
            ItemPaymentCreditProduct itemPay;

            if (_isNewData)
            {
                itemPay = new ItemPaymentCreditProduct();
            }
            else
            {
                itemPay = _listOfItemPaymentCredit[rowIndex - 1];

                if (itemPay.entity_state == EntityState.Unchanged)
                    itemPay.entity_state = EntityState.Modified;
            }

            itemPay.sale_id = sale.sale_id;
            itemPay.SellingProduct = sale;
            itemPay.amount = amount;
            itemPay.description = description;

            _listOfItemPaymentCredit[rowIndex - 1] = itemPay;
        }

        private void gridControl_CurrentCellValidated(object sender, EventArgs e)
        {
            var grid = (GridControl)sender;

            GridCurrentCell cc = grid.CurrentCell;

            var itemPayment = _listOfItemPaymentCredit[cc.RowIndex - 1];
            var sale = itemPayment.SellingProduct;
            var isValidRemainingInvoice = true;

            if (sale != null)
            {
                switch (cc.ColIndex)
                {
                    case 5: // column payment
                        itemPayment.amount = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString());
                        sale.total_payment = itemPayment.amount + sale.total_repayment_old;
                        isValidRemainingInvoice = sale.remaining_nota >= 0;

                        if (!isValidRemainingInvoice)
                        {
                            sale.total_payment -= itemPayment.amount;

                            MsgHelper.MsgWarning("Sorry, quantity payment exceeds  remaining credit");

                            GridListControlHelper.SetCurrentCell(grid, cc.RowIndex, cc.ColIndex);
                            GridListControlHelper.SelectCellText(grid, cc.RowIndex, cc.ColIndex);

                            break;
                        }
                        else
                            GridListControlHelper.SetCurrentCell(grid, cc.RowIndex, cc.ColIndex + 1);

                        break;

                    case 6: // column description
                        itemPayment.description = cc.Renderer.ControlValue.ToString();
                        break;

                    default:
                        break;
                }

                SetItemPay(grid, cc.RowIndex, cc.ColIndex, sale, itemPayment.amount, itemPayment.description);
                grid.Refresh();
                RefreshTotal();
            }             
        }

        private void FrmEntryPaymentCreditSalesProduct_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hapus objek dumm
            if (!_isNewData)
            {
                var itemsToRemove = _paymentCredit.item_payment_credit.Where(f => f.SellingProduct == null && f.entity_state == EntityState.Added)
                                                                            .ToArray();   
                foreach (var item in itemsToRemove)
                {
                    _paymentCredit.item_payment_credit.Remove(item);
                }
            }
        }        
    }
}
