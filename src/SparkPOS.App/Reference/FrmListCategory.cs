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
using System.IO;
using System.Diagnostics;
using MultilingualApp;

namespace SparkPOS.App.Reference
{
    public partial class FrmListCategory : FrmListStandard, IListener
    {
        private ICategoryBll _bll; // deklarasi objek business logic layer 
        private IList<Category> _listOfCategory = new List<Category>();
        private ILog _log;

        public FrmListCategory(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent();  
            this.btnImport.Visible = true;
         //   this.toolTip1.SetToolTip(this.btnImport, "Import/Export Data Category");
            if (MainProgram.currentLanguage == "en-US")
            {
                this.toolTip1.SetToolTip(this.btnImport, "Import/Export Data Category");
                this.mnuBukaFileMaster.Text = "Open File Master Category";
                this.mnuImportFileMaster.Text = "Import File Master Category";
                this.mnuExportData.Text = "Export Data Category";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.toolTip1.SetToolTip(this.btnImport, "فئة بيانات الاستيراد / التصدير");
                this.mnuBukaFileMaster.Text = "فتح فئة الملف الرئيسي";
                this.mnuImportFileMaster.Text = "فئة استيراد الملف الرئيسي";
                this.mnuExportData.Text = "فئة بيانات التصدير";
            }
           

            _log = MainProgram.log;
            _bll = new CategoryBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                {
                    LoadData();

                    btnImport.Enabled = user.is_administrator;
                }                    

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfCategory.Count);
            MainProgram.GlobalLanguageChange(this);

        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Category", Width = 700 });
            gridListProperties.Add(new GridListControlProperties { Header = "Profit (%)", Width = 400 });
            gridListProperties.Add(new GridListControlProperties { Header = "discount" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<Category>(this.gridList, _listOfCategory, gridListProperties);

            if (_listOfCategory.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfCategory.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfCategory.Count)
                        {
                            var category = _listOfCategory[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = category.name_category;
                                    break;

                                case 3:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = category.profit_percentage;
                                    break;

                                case 4:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = category.discount;
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
                _listOfCategory = _bll.GetAll();

                GridListControlHelper.Refresh<Category>(this.gridList, _listOfCategory);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfCategory.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryCategory(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var category = _listOfCategory[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryCategory(formTitle + this.TabText, category, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            if (MsgHelper.MsgDelete())
            {
                var category = _listOfCategory[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(category);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Category>(this.gridList, _listOfCategory, category);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        protected override void OpenFileMaster()
        {
            var msg = "keyMasterExcel";

            if (MsgHelper.MsgConfirmation(msg))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_golongan.xlsx";

                if (!File.Exists(fileMaster))
                {
                    MsgHelper.MsgWarning("Sorry file master Category not found.");
                    return;
                }

                try
                {
                    Process.Start(fileMaster);
                }
                catch
                {
                    msg = "Key-FailedMaster";

                    MsgHelper.MsgError(msg);
                }
            }
        }

        protected override void ImportData()
        {
            var frm = new FrmImportDataCategory("Import Data Category dari File Excel");
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void ExportData()
        {
            using (var dlgSave = new SaveFileDialog())
            {
                dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                dlgSave.Title = "Export Data Category";

                var result = dlgSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        IImportExportDataBll<Category> _importDataBll = new ImportExportDataCategoryBll(dlgSave.FileName, _log);
                        _importDataBll.Export(_listOfCategory);
                    }
                }
            }
        }

        public void Ok(object sender, object data)
        {
            if (sender is FrmImportDataCategory)
            {
                LoadData(); // refresh data setelah import dari file excel
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var category = (Category)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Category>(this.gridList, _listOfCategory, category);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Category>(this.gridList, _listOfCategory, category);
        }
    }
}
