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

using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using SparkPOS.Model;
using Syncfusion.Windows.Forms.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultilingualApp;

namespace SparkPOS.App.Lookup
{
    public partial class FrmLookupInvoice : FrmLookupEmptyBody
    {
        private IList<PurchaseProduct> _listOfInvoicePurchaseProduct = null;
        private IList<SellingProduct> _listOfInvoiceSellingProduct = null;

        private ReferencesType _ReferenceType = ReferencesType.InvoicePurchaseProduct;

        public IListener Listener { private get; set; }

        public FrmLookupInvoice(string header, IList<PurchaseProduct> listOfInvoicePurchaseProduct)
            : base()
        {
             InitializeComponent();  

            base.SetHeader(header);
            this._listOfInvoicePurchaseProduct = listOfInvoicePurchaseProduct;
            this._ReferenceType = ReferencesType.InvoicePurchaseProduct;

            InitGridList();
            base.SetActiveBtnSelect(listOfInvoicePurchaseProduct.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupInvoice(string header, IList<SellingProduct> listOfInvoiceSellingProduct)
            : base()
        {
             InitializeComponent();  

            base.SetHeader(header);
            this._listOfInvoiceSellingProduct = listOfInvoiceSellingProduct;
            this._ReferenceType = ReferencesType.InvoiceSellingProduct;

            InitGridList();
            base.SetActiveBtnSelect(listOfInvoiceSellingProduct.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 80, HorizontalAlignment = GridHorizontalAlignment.Center });
            gridListProperties.Add(new GridListControlProperties { Header = "CreditTerm", Width = 80, HorizontalAlignment = GridHorizontalAlignment.Center });

            if (this._ReferenceType == ReferencesType.InvoiceSellingProduct)
            {
                gridListProperties.Add(new GridListControlProperties { Header = "Invoice Selling", Width = 90, HorizontalAlignment = GridHorizontalAlignment.Center });
                gridListProperties.Add(new GridListControlProperties { Header = "Customer", Width = 170 });
                gridListProperties.Add(new GridListControlProperties { Header = "Remaining Credit", HorizontalAlignment = GridHorizontalAlignment.Right });
            }
            else
            {
                gridListProperties.Add(new GridListControlProperties { Header = "purchase invoice", Width = 90, HorizontalAlignment = GridHorizontalAlignment.Center });
                gridListProperties.Add(new GridListControlProperties { Header = "Supplier", Width = 170 });
                gridListProperties.Add(new GridListControlProperties { Header = "Remaining Debt", HorizontalAlignment = GridHorizontalAlignment.Right });
            }
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);

            var listCount = 0;

            switch (this._ReferenceType)
            {
                case ReferencesType.InvoicePurchaseProduct:
                    GridListControlHelper.InitializeGridListControl<PurchaseProduct>(this.gridList, _listOfInvoicePurchaseProduct, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridInvoicePurchaseProduct_QueryCellInfo;

                    listCount = _listOfInvoicePurchaseProduct.Count;

                    break;

                case ReferencesType.InvoiceSellingProduct:
                    GridListControlHelper.InitializeGridListControl<SellingProduct>(this.gridList, _listOfInvoiceSellingProduct, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridInvoiceSellingProduct_QueryCellInfo;

                    listCount = _listOfInvoiceSellingProduct.Count;

                    break;

                default:
                    break;
            }

            if (listCount > 0)
                this.gridList.SetSelected(0, true);
        }

        private void GridInvoiceSellingProduct_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfInvoiceSellingProduct.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfInvoiceSellingProduct.Count)
                    {
                        var InvoiceSelling = _listOfInvoiceSellingProduct[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2: // date
                                e.Style.CellValue = DateTimeHelper.DateToString(InvoiceSelling.date);
                                break;

                            case 3: // creditTerm
                                e.Style.CellValue = DateTimeHelper.DateToString(InvoiceSelling.due_date);
                                break;

                            case 4: // invoice
                                e.Style.CellValue = InvoiceSelling.invoice;
                                break;

                            case 5: // customer
                                var customer = InvoiceSelling.Customer;

                                if (customer != null)
                                    e.Style.CellValue = customer.name_customer;

                                break;

                            case 6: // remaining
                                e.Style.CellValue = NumberHelper.NumberToString(InvoiceSelling.remaining_nota);
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

        private void GridInvoicePurchaseProduct_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfInvoicePurchaseProduct.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfInvoicePurchaseProduct.Count)
                    {
                        var notaPurchase = _listOfInvoicePurchaseProduct[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2: // date
                                e.Style.CellValue = DateTimeHelper.DateToString(notaPurchase.date);
                                break;

                            case 3: // creditTerm
                                e.Style.CellValue = DateTimeHelper.DateToString(notaPurchase.due_date);
                                break;

                            case 4: // invoice
                                e.Style.CellValue = notaPurchase.invoice;
                                break;

                            case 5: // supplier
                                var supplier = notaPurchase.Supplier;

                                if (supplier != null)
                                    e.Style.CellValue = supplier.name_supplier;

                                break;

                            case 6: // remaining
                                e.Style.CellValue = NumberHelper.NumberToString(notaPurchase.remaining_nota);
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

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (base.IsButtonSelectEnabled)
                Select();
        }

        private void gridList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                if (base.IsButtonSelectEnabled)
                    Select();
            }
        }

        protected override void Select()
        {
            var rowIndex = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(rowIndex, this.Text))
                return;

            switch (this._ReferenceType)
            {
                case ReferencesType.InvoicePurchaseProduct:
                    var notaPurchaseProduct = _listOfInvoicePurchaseProduct[rowIndex];
                            this.Listener.Ok(this, notaPurchaseProduct);

                    break;

                case ReferencesType.InvoiceSellingProduct:
                    var InvoiceSellingProduct = _listOfInvoiceSellingProduct[rowIndex];
                    this.Listener.Ok(this, InvoiceSellingProduct);
                    break;

                default:
                    break;
            }

            //this.Close();
        }        
    }
}
