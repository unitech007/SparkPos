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
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper.UI.Template;

namespace SparkPOS.App.Cashier.Lookup
{
    public partial class FrmLookupReference : FrmLookupEmptyBody
    {
        private IList<Customer> _listOfCustomer = null;
        private IList<Product> _listOfProduct = null;

        private ReferencesType _ReferenceType = ReferencesType.Customer;
        public IListener Listener { private get; set; }

        public FrmLookupReference(string header, IList<Customer> listOfCustomer)
            : base()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);

            base.SetHeader(header);
            this._listOfCustomer = listOfCustomer;
            this._ReferenceType = ReferencesType.Customer;

            InitGridList();
            base.SetActiveBtnSelect(listOfCustomer.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<Product> listOfProduct)
            : base()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);

            base.SetHeader(header);
            this._listOfProduct = listOfProduct;
            this._ReferenceType = ReferencesType.Product;

            InitGridList();
            base.SetActiveBtnSelect(listOfProduct.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 50 });

            var listCount = 0;

            this.gridList.Grid.QueryRowHeight += delegate(object sender, GridRowColSizeEventArgs e)
            {
                e.Size = 27;
                e.Handled = true;
            };

            switch (this._ReferenceType)
            {
                case ReferencesType.Customer:
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Customer", Width = 400 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Address" });

                    GridListControlHelper.InitializeGridListControl<Customer>(this.gridList, _listOfCustomer, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridCustomer_QueryCellInfo;

                    listCount = _listOfCustomer.Count;

                    break;

                case ReferencesType.Product:
                    gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 150 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 400 });
                    gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 120 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Stock", Width = 70 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Category" });
                    GridListControlHelper.InitializeGridListControl<Product>(this.gridList, _listOfProduct, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridProduct_QueryCellInfo;

                    listCount = _listOfProduct.Count;
                    break;

                default:
                    break;
            }

            if (listCount > 0)
                this.gridList.SetSelected(0, true);
        }

        private void GridCustomer_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (_listOfCustomer.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    e.Style.Font = new GridFontInfo(new Font("Arial", 14f));

                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfCustomer.Count)
                    {
                        var customer = _listOfCustomer[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = customer.name_customer;
                                break;

                            case 3:
                                e.Style.CellValue = customer.address;
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

        private void GridProduct_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (_listOfProduct.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    e.Style.Font = new GridFontInfo(new Font("Arial", 14f));

                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfProduct.Count)
                    {
                        var product = _listOfProduct[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = product.product_code;
                                break;

                            case 3:
                                e.Style.CellValue = product.product_name;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString(product.selling_price);
                                break;

                            case 5:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = (product.stock + product.warehouse_stock);
                                break;

                            case 6:
                                var category = product.Category;

                                if (category != null)
                                    e.Style.CellValue = category.name_category;

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

        protected override void Select()
        {
            var rowIndex = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(rowIndex, this.Text))
                return;

            switch (this._ReferenceType)
            {
                case ReferencesType.Customer:
                    var customer = _listOfCustomer[rowIndex];
                    this.Listener.Ok(this, customer);
                    break;
    
                case ReferencesType.Product:
                    var product = _listOfProduct[rowIndex];
                    this.Listener.Ok(this, product);
                    break;

                default:
                    break;
            }

           // this.Close();
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
