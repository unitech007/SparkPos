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
    public partial class FrmLookupMinimalStock : Form
    {
        private IList<Product> _listOfProduct = null;
        private ILog _log;

        public FrmLookupMinimalStock(string header, IList<Product> listOfProduct)
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);
            LanguageHelper.TranslateToolTripTitle(this);
            this.Text = header;
            this.lblHeader.Text = header;
            this._listOfProduct = listOfProduct;
            this._log = MainProgram.log;

            InitGridList();
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Category", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "Stock Shelf", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "Stock Warehouse", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "Min. Stock Warehouse" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);

            GridListControlHelper.InitializeGridListControl<Product>(this.gridList, _listOfProduct, gridListProperties, rowHeight:40);

            if (_listOfProduct.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
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
                                    e.Style.CellValue = product.Category.name_category;
                                    break;

                                case 3:
                                    e.Style.CellValue = product.product_code;
                                    break;

                                case 4:
                                    e.Style.CellValue = product.product_name;
                                    break;

                                case 5:
                                    e.Style.CellValue = product.stock;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 6:
                                    e.Style.CellValue = product.warehouse_stock;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 7:
                                    e.Style.CellValue = product.minimal_stock_warehouse;
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLookupMinimalStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                this.Close();
        }
       
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dlgSave = new SaveFileDialog())
            {
               
                if (MainProgram.currentLanguage == "en-US")
                {
                    dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                    dlgSave.Title = "Export Data Product";
                }
                else if (MainProgram.currentLanguage == "ar-SA")
                {
                    dlgSave.Filter = "ملفات Microsoft Excel (* .xlsx) | * .xlsx";
                    dlgSave.Title = "اسبيرد تاتا موتورز";
                }
                var result = dlgSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        IImportExportDataBll<Product> _importDataBll = new ImportExportDataProductBll(dlgSave.FileName, _log);
                        _importDataBll.Export(_listOfProduct);
                    }
                }
            }
        }
    }
}
