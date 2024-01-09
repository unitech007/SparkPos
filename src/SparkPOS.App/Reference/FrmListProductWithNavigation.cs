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
using System.Windows.Forms;

using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using ConceptCave.WaitCursor;
using log4net;
using System.IO;
using System.Diagnostics;
using Syncfusion.Styles;
using MultilingualApp;

namespace SparkPOS.App.Reference
{
    public partial class FrmListProductWithNavigation : FrmListEmptyBodyWithNavigation, IListener
    {
        private IProductBll _bll; // deklarasi objek business logic layer 
        private IList<Product> _listOfProduct = new List<Product>();
        private IList<Category> _listOfCategory = new List<Category>();
        private ILog _log;
        private User _user;

        private int _pageNumber = 1;
        private int _pagesCount = 0;
        private int _pageSize = 0;
        private string _menuId = string.Empty;

        public FrmListProductWithNavigation(string header, User user, string menuId)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            this.btnImport.Visible = true;

            // cmbSortBy.Items.Clear();

            TitleExcelTranslate(this);

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _pageSize = MainProgram.pageSize;
            _log = MainProgram.log;
            _bll = new ProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    cmbSortBy.SelectedIndex = 1;
                    this.updLimit.Value = _pageSize;

                    LoadDataCategory();                    
                }

                cmbSortBy.Enabled = role.is_grant;
                txtNameProduct.Enabled = role.is_grant;
                btnFind.Enabled = role.is_grant;

                btnImport.Enabled = user.is_administrator;
            }
           
            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCategory.Count);
            MainProgram.GlobalLanguageChange(this);

        }
       
        private void LoadDataCategory()
        {
            ICategoryBll golonganBll = new CategoryBll(_log);

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfCategory = golonganBll.GetAll();

                cmbCategory.Items.Clear();
                if (MainProgram.currentLanguage == "en-US")
                {
                    cmbCategory.Items.Add("-- All --");

                }
                else if (MainProgram.currentLanguage == "ar-SA")
                {
                    cmbCategory.Items.Add("-- الجميع --");
                }
               
                foreach (var category in _listOfCategory)
                {
                    cmbCategory.Items.Add(category.name_category);
                }

                cmbCategory.SelectedIndex = 0;
            }
        }

        private void LoadDataProduct(string golonganId = "", int sortIndex = 1)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (golonganId.Length > 0)
                    _listOfProduct = _bll.GetByCategory(golonganId, sortIndex, _pageNumber, _pageSize, ref _pagesCount);
                else
                    _listOfProduct = _bll.GetAll(sortIndex, _pageNumber, _pageSize, ref _pagesCount);

                GridListControlHelper.Refresh<Product>(this.gridList, _listOfProduct, additionalRowCount: 1);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfProduct.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void LoadDataProductByName(string name, int sortIndex = 1)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfProduct = _bll.GetByName(name, sortIndex, _pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<Product>(this.gridList, _listOfProduct, additionalRowCount: 1);
                
                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfProduct.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfProduct.Count > 0);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "Category", Width = 130 });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 110 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 350 });
            gridListProperties.Add(new GridListControlProperties { Header = "unit", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "Buying Price", Width = 70 });
            
            gridListProperties.Add(new GridListControlProperties { Header = "price Selling", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "price Selling", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "price Selling", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "price Selling", Width = 70 });

            gridListProperties.Add(new GridListControlProperties { Header = "discount", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "Stock Shelf", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "Stock Warehouse", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "Min. Stock Warehouse", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "Status" });
           
            GridListControlHelper.InitializeGridListControl<Product>(this.gridList, _listOfProduct, gridListProperties, false, additionalRowCount: 1);
            this.gridList.Grid.Model.RowHeights[1] = 25;
            this.gridList.Grid.Model.Rows.FrozenCount = 1;

            this.gridList.Grid.PrepareViewStyleInfo += delegate(object sender, GridPrepareViewStyleInfoEventArgs e)
            {
                var subHeaderPriceSelling = new string[] { "Retail", "Wholesale 1", "Wholesale 2", "Wholesale 3" };
                if (MainProgram.currentLanguage == "en-US")
                {
                     subHeaderPriceSelling = new string[] { "Retail", "Wholesale 1", "Wholesale 2", "Wholesale 3" };

                }
                else if (MainProgram.currentLanguage == "ar-SA")
                {
                     subHeaderPriceSelling = new string[] { "البيع بالتجزئة" , "الجملة 1" , "الجملة 2" , "الجملة 3" };

                }

                if (e.ColIndex > 6 && e.RowIndex == 1)
                {
                    var colIndex = 7;

                    foreach (var header in subHeaderPriceSelling)
                    {
                        if (colIndex == e.ColIndex)
                            e.Style.Text = header;

                        colIndex++;
                    }
                }
            };

            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            if (_listOfProduct.Count > 0)
                this.gridList.SetSelected(1, true);

            // merge cell
            var column = 1; // column no
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 2; // column category
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 3; // column code
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 4; // column name product
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 5; // column unit
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 6; // column Buying Price
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 7; // column price sale
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 0, column + 3));

            column = 11; // column discount
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 12; // column stock etalase
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 13; // column stock gudang
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 14; // column Minimum stock
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 15; // column status
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            var headerStyle = this.gridList.Grid.BaseStylesMap["Column Header"].StyleInfo;
            headerStyle.CellType = GridCellTypeName.Header;

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (e.RowIndex == 1)
                {
                    if (e.ColIndex > 6)
                    {
                        e.Style.ModifyStyle(headerStyle, StyleModifyType.ApplyNew);
                    }

                    // we handled it, let the grid know
                    e.Handled = true;
                }

                if (_listOfProduct.Count > 0)
                {                    
                    if (e.RowIndex > 1)
                    {

                        var rowIndex = e.RowIndex - 2;

                        if (rowIndex < _listOfProduct.Count)
                        {
                            var product = _listOfProduct[rowIndex];
                            var listOfPriceWholesale = product.list_of_harga_grosir;
                            var hargaWholesale = 0d;

                            switch (e.ColIndex)
                            {
                                case 1:
                                    var noUrut = (_pageNumber - 1) * _pageSize + e.RowIndex - 1;
                                    e.Style.CellValue = noUrut;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 2:
                                    if (product.Category != null)
                                        e.Style.CellValue = product.Category.name_category;

                                    break;

                                case 3:
                                    e.Style.CellValue = product.product_code;
                                    break;

                                case 4:
                                    e.Style.CellValue = product.product_name;
                                    break;

                                case 5:
                                    var unit = string.Empty;

                                    if (product.unit.Length > 0)
                                        unit = product.unit;

                                    e.Style.CellValue = unit;
                                    break;

                                case 6:
                                    e.Style.CellValue = NumberHelper.NumberToString(product.purchase_price);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;

                                case 7: // price sale ritel
                                    e.Style.CellValue = NumberHelper.NumberToString(product.selling_price);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;

                                case 8: // price grosir 1
                                    hargaWholesale = listOfPriceWholesale.Count > 0 ? listOfPriceWholesale[0].wholesale_price : 0;

                                    e.Style.CellValue = NumberHelper.NumberToString(hargaWholesale);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;

                                case 9: // price grosir 2
                                    hargaWholesale = listOfPriceWholesale.Count > 1 ? listOfPriceWholesale[1].wholesale_price : 0;

                                    e.Style.CellValue = NumberHelper.NumberToString(hargaWholesale);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;

                                case 10: // price grosir 3
                                    hargaWholesale = listOfPriceWholesale.Count > 2 ? listOfPriceWholesale[2].wholesale_price : 0;

                                    e.Style.CellValue = NumberHelper.NumberToString(hargaWholesale);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;

                                case 11:
                                    e.Style.CellValue = product.discount;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 12:
                                    e.Style.CellValue = product.stock;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 13:
                                    e.Style.CellValue = product.warehouse_stock;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 14:
                                    e.Style.CellValue = product.minimal_stock_warehouse;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 15:
                                    e.Style.CellValue = product.is_active ? "Active" : "Non Active";
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                default:
                                    break;
                            }

                            // we handled it, let the grid know
                            e.Handled = true;
                        }
                    }
                }
            };
        }

        public void Ok(object sender, object data)
        {
            if (sender is FrmImportDataProduct)
            {
                // refresh data setelah import dari file excel
                if (cmbCategory.SelectedIndex == 0)
                    LoadDataProduct();
                else
                    cmbCategory.SelectedIndex = 0;
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var product = (Product)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Product>(this.gridList, _listOfProduct, product, additionalRowCount: 1);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Product>(this.gridList, _listOfProduct, product, additionalRowCount: 1);
        }

        private void txtNameProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            _pageNumber = 1;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    LoadDataProductByName(txtNameProduct.Text, cmbSortBy.SelectedIndex);
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCategory.Count);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var golonganId = string.Empty;

            var index = ((ComboBox)sender).SelectedIndex;

            if (index > 0)
            {
                var category = _listOfCategory[index - 1];
                golonganId = category.category_id;
            }

            _pageNumber = 1;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    LoadDataProduct(golonganId, cmbSortBy.SelectedIndex);
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCategory.Count);
        }

        protected override void Add()
        {
            if (cmbCategory.SelectedIndex == 0)
            {
                var msg = "Sorry data 'Category' Not yet selected.";
                //  var msg = "CategoryNotSelected";
                MsgHelper.MsgWarning(msg);
                //var msg = "KeyValuesPaid";

                //MsgHelper.MsgWarning(msg,"val");

                return;
            }

            var category = _listOfCategory[cmbCategory.SelectedIndex - 1];
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryProduct(formTitle + this.Text, category, _listOfCategory, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

       

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex - 1;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var product = _listOfProduct[index];
            product.code_produk_old = product.product_code;
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryProduct(formTitle + this.Text, product, _listOfCategory, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex - 1;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            if (MsgHelper.MsgDelete())
            {
                var product = _listOfProduct[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(product);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Product>(this.gridList, _listOfProduct, product, additionalRowCount: 1);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        protected override void OpenFileMaster()
        {
            var msg = "Key-FileRequires";

            if (MsgHelper.MsgConfirmation(msg))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_produk.xlsx";

                if (!File.Exists(fileMaster))
                {
                    MsgHelper.MsgWarning("Sorry file master Product not found.");
                    return;
                }

                try
                {
                    Process.Start(fileMaster);
                }
                //catch
                //{
                //    msg = "Key-ProductFile";
                //    MsgHelper.MsgError(msg);
                //}
                catch (Exception ex)
                {
                    MainProgram.LogException(ex);
                    // Error handling and logging
                    var msg1 = MainProgram.GlobalWarningMessage();
                    MsgHelper.MsgWarning(msg1);
                    //WarningMessageHandler.ShowTranslatedWarning(msg, MainProgram.currentLanguage);
                }
            }
        }

        protected override void ImportData()
        {
            var frm = new FrmImportDataProduct("Import Data Product from File Excel");
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void ExportData()
        {            
            using (var dlgSave = new SaveFileDialog())
            {
                dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                dlgSave.Title = "Export Data Product";

                var result = dlgSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        var listOfProduct = _bll.GetAll(cmbSortBy.SelectedIndex);
                        
                        IImportExportDataBll<Product> _importDataBll = new ImportExportDataProductBll(dlgSave.FileName, _log);
                        _importDataBll.Export(listOfProduct);
                    }                    
                }
            }                   
        }

        private void RefreshData()
        {
            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    if (txtNameProduct.Text.Length > 0)
                        LoadDataProductByName(txtNameProduct.Text);
                    else
                    {
                        var golonganId = string.Empty;

                        var index = cmbCategory.SelectedIndex;

                        if (index > 0)
                        {
                            var category = _listOfCategory[index - 1];
                            golonganId = category.category_id;
                        }

                        LoadDataProduct(golonganId);
                    }
                }
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCategory.Count);
        }
    
        protected override void MoveFirst()
        {
            _pageNumber = 1;

            RefreshData();
        }        

        protected override void MovePrevious()
        {
            _pageNumber--;

            RefreshData();
        }

        protected override void MoveNext()
        {
            _pageNumber++;

            RefreshData();
        }

        protected override void MoveLast()
        {
            _pageNumber = _pagesCount;

            RefreshData();
        }

        protected override void LimitRowChanged()
        {
            MainProgram.pageSize = (int)this.updLimit.Value;
            _pageSize = MainProgram.pageSize;

            cmbCategory_SelectedIndexChanged(cmbCategory, new EventArgs());
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }
        private static void TitleExcelTranslate(FrmListProductWithNavigation frm)
        {
            if (MainProgram.currentLanguage == "en-US")
            {
                frm.toolTip1.SetToolTip(frm.btnImport, "Import/Export Data Product");
                frm.mnuBukaFileMaster.Text = "Open the Product Master File";
                frm.mnuImportFileMaster.Text = "Import File Master Product";
                frm.mnuExportData.Text = "Export Data Product";

            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                frm.toolTip1.SetToolTip(frm.btnImport, "استيراد / تصدير منتج البيانات");
                frm.mnuBukaFileMaster.Text = "افتح ملف المنتج الرئيسي";
                frm.mnuImportFileMaster.Text = "استيراد منتج الملف الرئيسي";
                frm.mnuExportData.Text = "تصدير منتج البيانات";

            }

        }
        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            var golonganId = string.Empty;

            var index = cmbCategory.SelectedIndex;

            if (index > 0)
            {
                var category = _listOfCategory[index - 1];
                golonganId = category.category_id;
            }

            _pageNumber = 1;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    LoadDataProduct(golonganId, cmbSortBy.SelectedIndex);
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCategory.Count);
        }
      
        private void btnTambah_Click(object sender, EventArgs e)
        {

        }
    }
}
