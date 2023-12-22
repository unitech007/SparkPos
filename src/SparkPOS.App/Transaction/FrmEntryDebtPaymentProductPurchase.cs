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
    public partial class FrmEntryDebtPaymentProductPurchase : FrmEntryStandard, IListener
    {
        private IDebtPaymentProductBll _bll = null;
        private DebtPaymentProduct _DebtPayment = null;
        private Supplier _supplier = null;
        private IList<ItemDebtPaymentProduct> _listOfItemDebtPayment = new List<ItemDebtPaymentProduct>();
        private IList<ItemDebtPaymentProduct> _listOfItemDebtPaymentOld = new List<ItemDebtPaymentProduct>();
        private IList<ItemDebtPaymentProduct> _listOfItemDebtPaymentDeleted = new List<ItemDebtPaymentProduct>();

        private int _rowIndex = 0;
        private int _colIndex = 0;

        private bool _isNewData = false;
        private ILog _log;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntryDebtPaymentProductPurchase(string header, IDebtPaymentProductBll bll) 
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

            _listOfItemDebtPayment.Add(new ItemDebtPaymentProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryDebtPaymentProductPurchase(string header, DebtPaymentProduct DebtPayment, IDebtPaymentProductBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._DebtPayment = DebtPayment;
            this._supplier = DebtPayment.Supplier;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = this._DebtPayment.invoice;
            dtpDate.Value = (DateTime)this._DebtPayment.date;

            txtSupplier.Text = this._supplier.name_supplier;
            txtKeterangan.Text = this._DebtPayment.description;

            // save data lama
            _listOfItemDebtPaymentOld.Clear();
            foreach (var item in this._DebtPayment.item_payment_debt)
            {
                _listOfItemDebtPaymentOld.Add(new ItemDebtPaymentProduct
                {
                    pay_purchase_item_id = item.pay_purchase_item_id,
                    amount = item.amount,
                    description = item.description
                });
            }
            
            _listOfItemDebtPayment = this._DebtPayment.item_payment_debt;
            _listOfItemDebtPayment.Add(new ItemDebtPaymentProduct()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
            RefreshTotal();
        }

        private void InitGridControl(GridControl grid)
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "purchase invoice", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Total", Width = 100, IsEditable = false, HorizontalAlignment = GridHorizontalAlignment.Right });
            gridListProperties.Add(new GridListControlProperties { Header = "Shortage", Width = 100, IsEditable = false, HorizontalAlignment = GridHorizontalAlignment.Right });
            gridListProperties.Add(new GridListControlProperties { Header = "Payment", Width = 100, HorizontalAlignment = GridHorizontalAlignment.Right });
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 200 });
            gridListProperties.Add(new GridListControlProperties { Header = "Action" });

            GridListControlHelper.InitializeGridListControl<ItemDebtPaymentProduct>(grid, _listOfItemDebtPayment, gridListProperties);

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
                        var DebtPayment = _listOfItemDebtPayment[e.RowIndex - 1];
                        DebtPayment.entity_state = EntityState.Deleted;

                        _listOfItemDebtPaymentDeleted.Add(DebtPayment);
                        _listOfItemDebtPayment.Remove(DebtPayment);

                        grid.RowCount = _listOfItemDebtPayment.Count();
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
                    if (!(_listOfItemDebtPayment.Count > 0))
                        return;

                    double grand_total = 0;
                    double RemainingInvoice = 0;

                    var itemPayment = _listOfItemDebtPayment[e.RowIndex - 1];
                    var beli = itemPayment.PurchaseProduct;
                    if (beli != null)
                    {
                        grand_total = beli.grand_total;
                        RemainingInvoice = beli.remaining_nota;
                    }

                    switch (e.ColIndex)
                    {
                        case 1: // no urut
                            e.Style.CellValue = e.RowIndex.ToString();
                            break;

                        case 2: // purchase invoice                            
                            if (beli != null)
                                e.Style.CellValue = beli.invoice;

                            if (beli != null)
                            {
                                if (beli.due_date.IsNull()) // invoice cash amountnya cannot be edited
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
                            if (beli != null)
                            {
                                if (beli.due_date.IsNull()) // invoice cash amountnya cannot be edited
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

                            if (beli != null)
                            {
                                e.Style.Enabled = !beli.due_date.IsNull();
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

        private double SumGrid(IList<ItemDebtPaymentProduct> listOfItemDebtPayment)
        {
            double total = 0;
            foreach (var item in listOfItemDebtPayment.Where(f => f.PurchaseProduct != null))
            {
                total += item.amount;
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemDebtPayment));
        }

        protected override void Save()
        {
            if (this._supplier == null || txtSupplier.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Supplier' should not empty !");
                txtSupplier.Focus();

                return;
            }

            var total = SumGrid(this._listOfItemDebtPayment);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You have not completed the payment data input !");
                return;
            }

            if (!MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
                return;

            if (_isNewData)
                _DebtPayment = new DebtPaymentProduct();

            _DebtPayment.user_id = this._user.user_id;
            _DebtPayment.User = this._user;
            _DebtPayment.supplier_id = this._supplier.supplier_id;
            _DebtPayment.Supplier = this._supplier;
            _DebtPayment.invoice = txtInvoice.Text;
            _DebtPayment.date = dtpDate.Value;
            _DebtPayment.description = txtKeterangan.Text;

            _DebtPayment.item_payment_debt = this._listOfItemDebtPayment.Where(f => f.PurchaseProduct != null).ToList();

            if (!_isNewData) // update
                _DebtPayment.item_payment_debt_deleted = _listOfItemDebtPaymentDeleted.ToList();

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                {
                    result = _bll.Save(_DebtPayment, false, ref validationError);
                }
                else
                {
                    result = _bll.Update(_DebtPayment, false, ref validationError);
                }

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _DebtPayment);

                    _supplier = null;
                    _listOfItemDebtPayment.Clear();
                    _listOfItemDebtPaymentDeleted.Clear();                                        

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
                var itemsModified = _DebtPayment.item_payment_debt.Where(f => f.PurchaseProduct != null && f.entity_state == EntityState.Modified)
                                                     .ToArray();

                foreach (var item in itemsModified)
                {
                    var itemPayment = _listOfItemDebtPaymentOld.Where(f => f.pay_purchase_item_id == item.pay_purchase_item_id)
                                                                       .SingleOrDefault();

                    if (itemPayment != null)
                    {
                        item.amount = itemPayment.amount;
                        item.description = itemPayment.description;
                    }
                }

                // restore item yang di delete
                var itemsDeleted = _listOfItemDebtPaymentDeleted.Where(f => f.PurchaseProduct != null && f.entity_state == EntityState.Deleted)
                                                                     .ToArray();
                foreach (var item in itemsDeleted)
                {
                    item.entity_state = EntityState.Unchanged;
                    this._DebtPayment.item_payment_debt.Add(item);
                }

                _listOfItemDebtPaymentDeleted.Clear();
            }

            base.Cancel();
        }

        public void Ok(object sender, object data)
        {
            if (data is PurchaseProduct) // pencarian invoice
            {
                var beli = (PurchaseProduct)data;

                if (!IsExist(beli.invoice))
                {
                    SetItemPay(this.gridControl, _rowIndex, _colIndex + 1, beli);
                    this.gridControl.Refresh();

                    GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 5); // column payment
                }
                else
                {
                    MsgHelper.MsgWarning("Data invoice already entered");
                    GridListControlHelper.SelectCellText(this.gridControl, _rowIndex, _colIndex);
                }
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

                ISupplierBll bll = new SupplierBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                var listOfSupplier = bll.GetByName(name);

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

        private bool IsExist(string invoice)
        {
            var count = _listOfItemDebtPayment.Where(f => f.PurchaseProduct != null && f.PurchaseProduct.invoice.ToLower() == invoice.ToLower())
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
                        if (this._supplier == null || txtSupplier.Text.Length == 0)
                        {
                            MsgHelper.MsgWarning("Name supplier Not yet entered");
                            txtSupplier.Focus();
                            return;
                        }

                        var cc = grid.CurrentCell;
                        var invoice = cc.Renderer.ControlValue.ToString();

                        IList<PurchaseProduct> listOfPurchase = null;
                        IPurchaseProductBll bll = new PurchaseProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

                        if (invoice.Length > 0) // menampilkan invoice Credit based invoice
                        {
                            listOfPurchase = bll.GetInvoiceKreditByInvoice(this._supplier.supplier_id, invoice);
                        }
                        else
                        {
                            listOfPurchase = bll.GetInvoiceKreditBySupplier(this._supplier.supplier_id, false);
                        }

                        if (listOfPurchase.Count == 0)
                        {
                            MsgHelper.MsgWarning("Purchase Credit data not found");
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                        }
                        else if (listOfPurchase.Count == 1)
                        {
                            var beli = listOfPurchase[0];

                            if (!IsExist(beli.invoice))
                            {
                                SetItemPay(grid, rowIndex, colIndex, beli);
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

                            var frmLookup = new FrmLookupInvoice("Data Invoice Purchase", listOfPurchase);
                            frmLookup.Listener = this;
                            frmLookup.ShowDialog();
                        }

                        break;

                    case 6: // description
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemDebtPayment.Add(new ItemDebtPaymentProduct());
                            grid.RowCount = _listOfItemDebtPayment.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, rowIndex + 1, 2); // fokus ke column purchase invoice
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

        private void SetItemPay(GridControl grid, int rowIndex, int colIndex, PurchaseProduct beli, double amount = 0, string description = "")
        {
            ItemDebtPaymentProduct itemPay;

            if (_isNewData)
            {
                itemPay = new ItemDebtPaymentProduct();
            }
            else
            {
                itemPay = _listOfItemDebtPayment[rowIndex - 1];

                if (itemPay.entity_state == EntityState.Unchanged)
                    itemPay.entity_state = EntityState.Modified;
            }

            itemPay.purchase_id = beli.purchase_id;
            itemPay.PurchaseProduct = beli;
            itemPay.amount = amount;
            itemPay.description = description;

            _listOfItemDebtPayment[rowIndex - 1] = itemPay;
        }

        private void gridControl_CurrentCellValidated(object sender, EventArgs e)
        {
            var grid = (GridControl)sender;

            GridCurrentCell cc = grid.CurrentCell;

            var itemPayment = _listOfItemDebtPayment[cc.RowIndex - 1];
            var beli = itemPayment.PurchaseProduct;
            var isValidRemainingInvoice = true;

            if (beli != null)
            {
                switch (cc.ColIndex)
                {
                    case 5: // column payment
                        itemPayment.amount = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString());
                        beli.total_payment = itemPayment.amount + beli.total_repayment_old;
                        isValidRemainingInvoice = beli.remaining_nota >= 0;

                        if (!isValidRemainingInvoice)
                        {
                            beli.total_payment -= itemPayment.amount;

                            MsgHelper.MsgWarning("Sorry, quantity payment exceeds  remaining debt");

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

                SetItemPay(grid, cc.RowIndex, cc.ColIndex, beli, itemPayment.amount, itemPayment.description);
                grid.Refresh();
                RefreshTotal();
            }             
        }

        private void FrmEntryDebtPaymentProductPurchase_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hapus objek dumm
            if (!_isNewData)
            {
                var itemsToRemove = _DebtPayment.item_payment_debt.Where(f => f.PurchaseProduct == null && f.entity_state == EntityState.Added)
                                                                            .ToArray();   
                foreach (var item in itemsToRemove)
                {
                    _DebtPayment.item_payment_debt.Remove(item);
                }
            }
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
