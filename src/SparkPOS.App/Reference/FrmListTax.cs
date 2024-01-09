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
    public partial class FrmListTax : FrmListStandard, IListener
    {
        private ITaxBll _bll; // deklarasi objek business logic layer 
        private IList<Tax> _listOfTax = new List<Tax>();
        private ILog _log;

        public FrmListTax(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent();  
            this.btnImport.Visible = true;
           
            if (MainProgram.currentLanguage == "en-US")
            {
                this.toolTip1.SetToolTip(this.btnImport, "Import/Export Data Tax");
                this.mnuBukaFileMaster.Text = "Open the Master Tax File";
                this.mnuImportFileMaster.Text = "Import File Master Tax";
                this.mnuExportData.Text = "Export Data Tax";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.toolTip1.SetToolTip(this.btnImport, "ضريبة بيانات الاستيراد / التصدير");
                this.mnuBukaFileMaster.Text = "افتح الملف الضريبي الرئيسي";
                this.mnuImportFileMaster.Text = "ضريبة ملف الاستيراد الرئيسية";
                this.mnuExportData.Text = "ضريبة بيانات التصدير";
            }
            

            _log = MainProgram.log;
            _bll = new TaxBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

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
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfTax.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Tax Name", Width = 400 });
            gridListProperties.Add(new GridListControlProperties { Header = "Tax (%)", Width = 400 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<Tax>(this.gridList, _listOfTax, gridListProperties);

            if (_listOfTax.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfTax.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfTax.Count)
                        {
                            var category = _listOfTax[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = category.name_tax;
                                    break;

                                case 3:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = category.tax_percentage;
                                    break;

                                case 4:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = category.description;
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
                _listOfTax = _bll.GetAll();

                GridListControlHelper.Refresh<Tax>(this.gridList, _listOfTax);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfTax.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryTax(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var category = _listOfTax[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryTax(formTitle + this.TabText, category, _bll);
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
                var category = _listOfTax[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(category);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Tax>(this.gridList, _listOfTax, category);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        protected override void OpenFileMaster()
        {
            var msg = "Key-TaxFile";
            if (MsgHelper.MsgConfirmation(msg))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_golongan.xlsx";

                if (!File.Exists(fileMaster))
                {
                    MsgHelper.MsgWarning("Sorry file master Tax not found.");
                    return;
                }

                try
                {
                    Process.Start(fileMaster);
                }
                //catch
                //{
                //    msg = "Key-FailedTax";
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

        //protected override void ImportData()
        //{
        //    var frm = new FrmImportDataTax("Import Data Tax dari File Excel");
        //    frm.Listener = this;
        //    frm.ShowDialog();
        //}

        //protected override void ExportData()
        //{
        //    using (var dlgSave = new SaveFileDialog())
        //    {
        //        dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
        //        dlgSave.Title = "Export Data Tax";

        //        var result = dlgSave.ShowDialog();
        //        if (result == DialogResult.OK)
        //        {
        //            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
        //            {
        //                IImportExportDataBll<Tax> _importDataBll = new ImportExportDataTaxBll(dlgSave.FileName, _log);
        //                _importDataBll.Export(_listOfTax);
        //            }
        //        }
        //    }
        //}

        //public void Ok(object sender, object data)
        //{
        //    if (sender is FrmImportDataTax)
        //    {
        //        LoadData(); // refresh data setelah import dari file excel
        //    }
        //}

        public void Ok(object sender, bool isNewData, object data)
        {
            var category = (Tax)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Tax>(this.gridList, _listOfTax, category);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Tax>(this.gridList, _listOfTax, category);
        }

        public void Ok(object sender, object data)
        {
            throw new NotImplementedException();
        }
    }
}
