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
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using ConceptCave.WaitCursor;
using log4net;
using Syncfusion.Styles;
using MultilingualApp;

namespace SparkPOS.App.Reference
{
    public partial class FrmSalesperCategory : FrmListStandard, IListener
    {
        private IAdjustmentStockBll _bll; // deklarasi objek business logic layer 
        private IList<AdjustmentStock> _listOfAdjustmentStock = new List<AdjustmentStock>();
        private ILog _log;

        public FrmSalesperCategory(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent(); 

            _log = MainProgram.log;
            _bll = new AdjustmentStockBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                    LoadData();

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfAdjustmentStock.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 80 });
            gridListProperties.Add(new GridListControlProperties { Header = "Product", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "Addition", Width = 80 });
            gridListProperties.Add(new GridListControlProperties { Header = "Addition", Width = 80 });
            gridListProperties.Add(new GridListControlProperties { Header = "Reduction", Width = 80 });
            gridListProperties.Add(new GridListControlProperties { Header = "Reduction", Width = 80 });
            gridListProperties.Add(new GridListControlProperties { Header = "reason Adjustment", Width = 350 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<AdjustmentStock>(this.gridList, _listOfAdjustmentStock, gridListProperties, false, additionalRowCount: 1);
            this.gridList.Grid.Model.RowHeights[1] = 25;
            this.gridList.Grid.Model.Rows.FrozenCount = 1;

            this.gridList.Grid.PrepareViewStyleInfo += delegate(object sender, GridPrepareViewStyleInfoEventArgs e)
            {
                var subHeaderPriceSelling = new string[] { "Stock Shelf", "Stock Warehouse", "Stock Shelf", "Stock Warehouse" };
                if (e.ColIndex > 3 && e.RowIndex == 1)
                {
                    var colIndex = 4;

                    foreach (var header in subHeaderPriceSelling)
                    {
                        if (colIndex == e.ColIndex)
                            e.Style.Text = header;

                        colIndex++;
                    }
                }
            };

            if (_listOfAdjustmentStock.Count > 0)
                this.gridList.SetSelected(1, true);

            // merge cell
            var column = 1; // column no
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 2; // date
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 3; // product
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 4; // column Addition stock Shelfdan gudang
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 0, column + 1));

            column = 6; // column pengurangan stock Shelfdan gudang
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 0, column + 1));

            column = 8; // reason Adjustment
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            column = 9; // description
            this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, column, 1, column));

            var headerStyle = this.gridList.Grid.BaseStylesMap["Column Header"].StyleInfo;
            headerStyle.CellType = GridCellTypeName.Header;

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (e.RowIndex == 1)
                {
                    if (e.ColIndex > 3)
                    {
                        e.Style.ModifyStyle(headerStyle, StyleModifyType.ApplyNew);
                    }

                    // we handled it, let the grid know
                    e.Handled = true;
                }

                if (_listOfAdjustmentStock.Count > 0)
                {
                    if (e.RowIndex > 1)
                    {
                        var rowIndex = e.RowIndex - 2;

                        if (rowIndex < _listOfAdjustmentStock.Count)
                        {
                            var penyesuaianStock = _listOfAdjustmentStock[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 1:
                                    e.Style.CellValue = e.RowIndex - 1;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(penyesuaianStock.date);
                                    break;

                                case 3:
                                    e.Style.CellValue = penyesuaianStock.Product.product_name;
                                    break;

                                case 4:
                                    e.Style.CellValue = penyesuaianStock.stock_addition;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 5:
                                    e.Style.CellValue = penyesuaianStock.warehouse_stock_addition;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 6:
                                    e.Style.CellValue = penyesuaianStock.stock_reduction;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 7:
                                    e.Style.CellValue = penyesuaianStock.warehouse_stock_reduction;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 8:
                                    e.Style.CellValue = penyesuaianStock.ReasonAdjustmentStock.reason;
                                    break;

                                case 9:
                                    e.Style.CellValue = penyesuaianStock.description;
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

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfAdjustmentStock = _bll.GetAll();
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfAdjustmentStock.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryAdjustmentStock(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex - 1;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var penyesuaianStock = _listOfAdjustmentStock[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryAdjustmentStock(formTitle + this.TabText, penyesuaianStock, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex - 1;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            if (MsgHelper.MsgDelete())
            {
                var penyesuaianStock = _listOfAdjustmentStock[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(penyesuaianStock);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<AdjustmentStock>(this.gridList, _listOfAdjustmentStock, penyesuaianStock, additionalRowCount: 1);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        public void Ok(object sender, object data)
        {
            throw new NotImplementedException();
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var penyesuaianStock = (AdjustmentStock)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<AdjustmentStock>(this.gridList, _listOfAdjustmentStock, penyesuaianStock, additionalRowCount: 1);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<AdjustmentStock>(this.gridList, _listOfAdjustmentStock, penyesuaianStock, additionalRowCount: 1);
        }
    }
}
