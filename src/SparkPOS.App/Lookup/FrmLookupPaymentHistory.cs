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

using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using ConceptCave.WaitCursor;
using log4net;
using MultilingualApp;

namespace SparkPOS.App.Lookup
{
    public partial class FrmLookupPaymentHistory : Form
    {
        private IList<ItemPaymentCreditProduct> _listOfPaymentHistoryCredit = null;
        private IList<ItemDebtPaymentProduct> _listOfHistoryDebtPayment = null;
        private PaymentHistoryType _paymentType = PaymentHistoryType.DebtPayment;
        private ILog _log;

        public FrmLookupPaymentHistory(string header)
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);
            this._log = MainProgram.log;

            this.Text = header;
            this.lblHeader.Text = header;
            LanguageHelper.TranslateToolTripTitle(this);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupPaymentHistory(string header, PurchaseProduct beli, IList<ItemDebtPaymentProduct> listOfPaymentHistory)
            : this(header)
        {
            this.groupBox1.Text = " [ Information Purchase ] ";
            this.label3.Text = "Supplier";
            this._listOfHistoryDebtPayment = listOfPaymentHistory;
            this._paymentType = PaymentHistoryType.DebtPayment;

            txtDate.Text = DateTimeHelper.DateToString(beli.date);
            txtInvoice.Text = beli.invoice;
            txtCustomerOrSupplier.Text = beli.Supplier.name_supplier;
            txtTotal.Text = NumberHelper.NumberToString(SumGrid(listOfPaymentHistory));

            InitGridList();
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupPaymentHistory(string header, SellingProduct sale, IList<ItemPaymentCreditProduct> listOfPaymentHistory)
            : this(header)
        {
            this.groupBox1.Text = " [ Information Sales ] ";
            this.label3.Text = "Customer";
            this._listOfPaymentHistoryCredit = listOfPaymentHistory;
            this._paymentType = PaymentHistoryType.PaymentCredit;

            txtDate.Text = DateTimeHelper.DateToString(sale.date);
            txtInvoice.Text = sale.invoice;
            txtCustomerOrSupplier.Text = sale.Customer.name_customer;
            txtTotal.Text = NumberHelper.NumberToString(SumGrid(listOfPaymentHistory));
            InitGridList();
            MainProgram.GlobalLanguageChange(this);
        }

        private double SumGrid(IList<ItemDebtPaymentProduct> listOfPaymentHistory)
        {
            return listOfPaymentHistory.Sum(f => f.amount);
        }

        private double SumGrid(IList<ItemPaymentCreditProduct> listOfPaymentHistory)
        {
            return listOfPaymentHistory.Sum(f => f.amount);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice", Width = 90 });            
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "Operator", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "amount" });

            var listCount = 0;
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            switch (this._paymentType)
            {
                case PaymentHistoryType.DebtPayment:
                    GridListControlHelper.InitializeGridListControl<ItemDebtPaymentProduct>(this.gridList, _listOfHistoryDebtPayment, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridDebtPayment_QueryCellInfo;
                    listCount = _listOfHistoryDebtPayment.Count;

                    break;

                case PaymentHistoryType.PaymentCredit:
                    GridListControlHelper.InitializeGridListControl<ItemPaymentCreditProduct>(this.gridList, _listOfPaymentHistoryCredit, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridPaymentCredit_QueryCellInfo;
                    listCount = _listOfPaymentHistoryCredit.Count;

                    break;

                default:
                    break;
            }

            if (listCount > 0)
                this.gridList.SetSelected(0, true);
        }

        private void GridDebtPayment_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (_listOfHistoryDebtPayment.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfHistoryDebtPayment.Count)
                    {
                        var PaymentHistory = _listOfHistoryDebtPayment[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = DateTimeHelper.DateToString(PaymentHistory.DebtPaymentProduct.date);
                                break;

                            case 3:
                                e.Style.CellValue = PaymentHistory.DebtPaymentProduct.invoice;
                                break;                            

                            case 4:
                                e.Style.CellValue = PaymentHistory.description;
                                break;

                            case 5:
                                e.Style.CellValue = PaymentHistory.DebtPaymentProduct.User.name_user;
                                break;

                            case 6:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString(PaymentHistory.amount);
                                break;

                            default:
                                break;
                        }

                        // we handled it, let the grid know
                        e.Handled = true;
                    }
                }
            }
        }

        void GridPaymentCredit_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfPaymentHistoryCredit.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfPaymentHistoryCredit.Count)
                    {
                        var PaymentHistory = _listOfPaymentHistoryCredit[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = DateTimeHelper.DateToString(PaymentHistory.PaymentCreditProduct.date);
                                break;

                            case 3:
                                e.Style.CellValue = PaymentHistory.PaymentCreditProduct.invoice;
                                break;                            

                            case 4:
                                e.Style.CellValue = PaymentHistory.description;
                                break;

                            case 5:
                                e.Style.CellValue = PaymentHistory.PaymentCreditProduct.User.name_user;
                                break;

                            case 6:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString(PaymentHistory.amount);
                                break;

                            default:
                                break;
                        }

                        // we handled it, let the grid know
                        e.Handled = true;
                    }
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLookupPaymentHistory_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                this.Close();
        }
    }
}
