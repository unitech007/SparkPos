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
using SparkPOS.Model.RajaOngkir;
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper.UI.Template;
using MultilingualApp;

namespace SparkPOS.App.Lookup
{
    public partial class FrmLookupReference : FrmLookupEmptyBody
    {
        private IList<Supplier> _listOfSupplier = null;
        private IList<Customer> _listOfCustomer = null;
        private IList<Dropshipper> _listOfDropshipper = null;
        private IList<Product> _listOfProduct = null;
        private IList<TypeExpense> _listOfTypeExpense = null;
        private IList<RegencyAsalRajaOngkir> _listOfRegencyAsal = null;
        private IList<RegencyTujuanRajaOngkir> _listOfRegencyTujuan = null;

        private ReferencesType _ReferenceType = ReferencesType.Supplier;
        private bool _isShowPricePurchase = false;

        public IListener Listener { private get; set; }

        public FrmLookupReference(string header, IList<TypeExpense> listOfTypeExpense)
            : base()
        {
             InitializeComponent();  

            base.SetHeader(header);
            this._listOfTypeExpense = listOfTypeExpense;
            this._ReferenceType = ReferencesType.TypeExpense;

            InitGridList();
            base.SetActiveBtnSelect(listOfTypeExpense.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<Supplier> listOfSupplier)
            : base()
        {
             InitializeComponent(); 

            base.SetHeader(header);
            this._listOfSupplier = listOfSupplier;
            this._ReferenceType = ReferencesType.Supplier;

            InitGridList();
            base.SetActiveBtnSelect(listOfSupplier.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<Customer> listOfCustomer)
            : base()
        {
             InitializeComponent(); 

            base.SetHeader(header);
            this._listOfCustomer = listOfCustomer;
            this._ReferenceType = ReferencesType.Customer;

            InitGridList();
            base.SetActiveBtnSelect(listOfCustomer.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<Dropshipper> listOfDropshipper)
            : base()
        {
             InitializeComponent(); 

            base.SetHeader(header);
            this._listOfDropshipper = listOfDropshipper;
            this._ReferenceType = ReferencesType.Dropshipper;

            InitGridList();
            base.SetActiveBtnSelect(listOfDropshipper.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<Product> listOfProduct, bool isShowPricePurchase = false)
            : base()
        {
             InitializeComponent(); 

            base.SetHeader(header);
            this._listOfProduct = listOfProduct;
            this._ReferenceType = ReferencesType.Product;
            this._isShowPricePurchase = isShowPricePurchase;

            InitGridList();
            base.SetActiveBtnSelect(listOfProduct.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<RegencyAsalRajaOngkir> listOfRegencyAsal)
            : base()
        {
             InitializeComponent();

            base.SetHeader(header);
            this._listOfRegencyAsal = listOfRegencyAsal;
            this._ReferenceType = ReferencesType.RegencyAsal;

            InitGridList();
            base.SetActiveBtnSelect(listOfRegencyAsal.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmLookupReference(string header, IList<RegencyTujuanRajaOngkir> listOfRegencyTujuan)
            : base()
        {
             InitializeComponent();

            base.SetHeader(header);
            this._listOfRegencyTujuan = listOfRegencyTujuan;
            this._ReferenceType = ReferencesType.RegencyTujuan;

            InitGridList();
            base.SetActiveBtnSelect(listOfRegencyTujuan.Count > 0);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });

            var listCount = 0;

            switch (this._ReferenceType)
            {
                case ReferencesType.TypeExpense:
                    gridListProperties.Add(new GridListControlProperties { Header = "Type Cost" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<TypeExpense>(this.gridList, _listOfTypeExpense, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridTypeExpense_QueryCellInfo;

                    listCount = _listOfTypeExpense.Count;

                    break;

                case ReferencesType.Customer:
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Customer", Width = 200 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Address" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<Customer>(this.gridList, _listOfCustomer, gridListProperties);

                    this.gridList.Grid.QueryCellInfo += GridCustomer_QueryCellInfo;

                    listCount = _listOfCustomer.Count;

                    break;

                case ReferencesType.Supplier:
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Supplier", Width = 200 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Address" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<Supplier>(this.gridList, _listOfSupplier, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridSupplier_QueryCellInfo;

                    listCount = _listOfSupplier.Count;
                    break;

                case ReferencesType.Dropshipper:
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Dropshipper", Width = 200 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Address" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<Dropshipper>(this.gridList, _listOfDropshipper, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridDropshipper_QueryCellInfo;

                    listCount = _listOfDropshipper.Count;
                    break;

                case ReferencesType.Product:
                    gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 100 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 260 });
                    gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 70 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Stock", Width = 50 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Category" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<Product>(this.gridList, _listOfProduct, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridProduct_QueryCellInfo;

                    listCount = _listOfProduct.Count;
                    break;

                case ReferencesType.RegencyAsal:
                    gridListProperties.Add(new GridListControlProperties { Header = "Provinsi", Width = 250 });
                    gridListProperties.Add(new GridListControlProperties { Header = "City/Regency", Width = 250 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Code Pos" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<RegencyAsalRajaOngkir>(this.gridList, _listOfRegencyAsal, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridRegencyAsal_QueryCellInfo;

                    listCount = _listOfRegencyAsal.Count;
                    break;

                case ReferencesType.RegencyTujuan:
                    gridListProperties.Add(new GridListControlProperties { Header = "Provinsi", Width = 250 });
                    gridListProperties.Add(new GridListControlProperties { Header = "City/Regency", Width = 250 });
                    gridListProperties.Add(new GridListControlProperties { Header = "Code Pos" });
                    LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
                    GridListControlHelper.InitializeGridListControl<RegencyTujuanRajaOngkir>(this.gridList, _listOfRegencyTujuan, gridListProperties);
                    this.gridList.Grid.QueryCellInfo += GridRegencyTujuan_QueryCellInfo;

                    listCount = _listOfRegencyTujuan.Count;
                    break;

                default:
                    break;
            }

            if (listCount > 0)
                this.gridList.SetSelected(0, true);
        }

        private void GridDropshipper_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfDropshipper.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfDropshipper.Count)
                    {
                        var dropshipper = _listOfDropshipper[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = dropshipper.name_dropshipper;
                                break;

                            case 3:
                                e.Style.CellValue = dropshipper.address;
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

        private void GridRegencyTujuan_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfRegencyTujuan.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfRegencyTujuan.Count)
                    {
                        var regency = _listOfRegencyTujuan[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = regency.Provinsi.name_province;
                                break;

                            case 3:
                                e.Style.CellValue = regency.name_regency;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = regency.postal_code;
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

        private void GridRegencyAsal_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfRegencyAsal.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfRegencyAsal.Count)
                    {
                        var regency = _listOfRegencyAsal[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = regency.Provinsi.name_province;
                                break;

                            case 3:
                                e.Style.CellValue = regency.name_regency;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = regency.postal_code;
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

        private void GridTypeExpense_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {            
            if (_listOfTypeExpense.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfTypeExpense.Count)
                    {
                        var typeExpense = _listOfTypeExpense[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = typeExpense.name_expense_type;
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

        private void GridCustomer_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (_listOfCustomer.Count > 0)
            {
                if (e.RowIndex > 0)
                {
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
                                e.Style.CellTipText = e.Style.Text;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = _isShowPricePurchase ? NumberHelper.NumberToString(product.purchase_price) : NumberHelper.NumberToString(product.selling_price);
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

        private void GridSupplier_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (_listOfSupplier.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfSupplier.Count)
                    {
                        var supplier = _listOfSupplier[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.CellValue = supplier.name_supplier;
                                break;

                            case 3:
                                e.Style.CellValue = supplier.address;
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
                case ReferencesType.TypeExpense:
                    var typeExpense = _listOfTypeExpense[rowIndex];
                    this.Listener.Ok(this, typeExpense);
                    break;

                case ReferencesType.Supplier:
                    var supplier = _listOfSupplier[rowIndex];
                    this.Listener.Ok(this, supplier);
                    break;

                case ReferencesType.Customer:
                    var customer = _listOfCustomer[rowIndex];
                    this.Listener.Ok(this, customer);
                    break;

                case ReferencesType.Dropshipper:
                    var dropshipper = _listOfDropshipper[rowIndex];
                    this.Listener.Ok(this, dropshipper);
                    break;

                case ReferencesType.Product:
                    var product = _listOfProduct[rowIndex];
                    this.Listener.Ok(this, product);
                    break;

                case ReferencesType.RegencyAsal:
                    var regencyAsal = _listOfRegencyAsal[rowIndex];
                    this.Listener.Ok(this, regencyAsal);
                    break;

                case ReferencesType.RegencyTujuan:
                    var regencyTujuan = _listOfRegencyTujuan[rowIndex];
                    this.Listener.Ok(this, regencyTujuan);
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
