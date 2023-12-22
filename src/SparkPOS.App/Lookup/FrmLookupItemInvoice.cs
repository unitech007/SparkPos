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
    public partial class FrmLookupItemInvoice : FrmLookupEmptyBody
    {
        private IList<ItemSellingProduct> _listOfItemSelling = null;
        private IList<ItemPurchaseProduct> _listOfItemPurchase = null;

        private ReferencesType _ReferenceType = ReferencesType.Product;

        public IListener Listener { private get; set; }

        public FrmLookupItemInvoice(string header, IList<ItemSellingProduct> listOfItemSelling)
            : base()
        {
             InitializeComponent();  

            base.SetHeader(header);
            this._listOfItemSelling = listOfItemSelling;
            this._ReferenceType = ReferencesType.InvoiceSellingProduct;

            InitGridList();
            base.SetActiveBtnSelect(listOfItemSelling.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupItemInvoice(string header, IList<ItemPurchaseProduct> listOfItemPurchase)
            : base()
        {
             InitializeComponent(); 
            base.SetHeader(header);
            this._listOfItemPurchase = listOfItemPurchase;
            this._ReferenceType = ReferencesType.InvoicePurchaseProduct;

            InitGridList();
            base.SetActiveBtnSelect(listOfItemPurchase.Count > 0);
            MainProgram.GlobalLanguageChange(this);

        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            switch (_ReferenceType)
            {
                case ReferencesType.InvoiceSellingProduct:
                case ReferencesType.InvoicePurchaseProduct:
                    gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 300 });
                    gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = 50 });
                    gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 90 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Sub Total", Width = 90 });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);

                    break;

                default:
                    break;
            }

            var listCount = 0;

            switch (_ReferenceType)
            {
                case ReferencesType.InvoiceSellingProduct:
                    GridListControlHelper.InitializeGridListControl<ItemSellingProduct>(this.gridList, _listOfItemSelling, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridItemSellingProduct_QueryCellInfo;

                    listCount = _listOfItemSelling.Count;

                    break;

                case ReferencesType.InvoicePurchaseProduct:
                    GridListControlHelper.InitializeGridListControl<ItemPurchaseProduct>(this.gridList, _listOfItemPurchase, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridItemPurchaseProduct_QueryCellInfo;

                    listCount = _listOfItemPurchase.Count;

                    break;

                default:
                    break;
            }

            if (listCount > 0)
                this.gridList.SetSelected(0, true);
        }

        private void GridItemPurchaseProduct_QueryCellInfo(object sender, Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventArgs e)
        {
            if (_listOfItemPurchase.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfItemPurchase.Count)
                    {
                        var obj = _listOfItemPurchase[rowIndex];
                        var product = obj.Product;

                        switch (e.ColIndex)
                        {
                            case 2:
                                if (product != null)
                                    e.Style.CellValue = product.product_name;
                                break;

                            case 3:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = obj.quantity - obj.return_quantity;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString(obj.price);
                                break;

                            case 5:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString((obj.quantity - obj.return_quantity) * obj.price);
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

        private void GridItemSellingProduct_QueryCellInfo(object sender, Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventArgs e)
        {
            if (_listOfItemSelling.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfItemSelling.Count)
                    {
                        var obj = _listOfItemSelling[rowIndex];
                        var product = obj.Product;

                        switch (e.ColIndex)
                        {
                            case 2:
                                if (product != null)
                                    e.Style.CellValue = product.product_name;
                                break;

                            case 3:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = obj.quantity - obj.return_quantity;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString(obj.selling_price);
                                break;

                            case 5:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString((obj.quantity - obj.return_quantity) * obj.selling_price);
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

        protected override void Select()
        {
            var rowIndex = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(rowIndex, this.Text))
                return;

            switch (_ReferenceType)
            {
                case ReferencesType.InvoiceSellingProduct:
                    var itemSelling = _listOfItemSelling[rowIndex];
                    this.Listener.Ok(this, itemSelling);
                    break;

                case ReferencesType.InvoicePurchaseProduct:
                    var itemPurchase = _listOfItemPurchase[rowIndex];
                    this.Listener.Ok(this, itemPurchase);
                    break;

                default:
                    break;
            }

            //this.Close();
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
    }
}
