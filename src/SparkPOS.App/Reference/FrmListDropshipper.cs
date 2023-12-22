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
using SparkPOS.Helper.UserControl;
using MultilingualApp;

namespace SparkPOS.App.Reference
{
    public partial class FrmListDropshipper : FrmListEmptyBody, IListener
    {                
        private IDropshipperBll _bll; // deklarasi objek business logic layer 
        private IList<Dropshipper> _listOfDropshipper = new List<Dropshipper>();
        private ILog _log;
        private User _user;
        private string _menuId = string.Empty;

        public FrmListDropshipper(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            this.btnImport.Visible = true;
          
          //  WarningMessageHandler.SetToolTip(this.btnImport, "Import/Export Data Dropshipper", MainProgram.currentLanguage);

            
            if (MainProgram.currentLanguage == "en-US")
            {
                this.toolTip1.SetToolTip(this.btnImport, "Import/Export Data Dropshipper");
                this.mnuBukaFileMaster.Text = "Open the Dropshipper Master File";
                this.mnuImportFileMaster.Text = "Import File Master Dropshipper";
                this.mnuExportData.Text = "Export Data Dropshipper";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.toolTip1.SetToolTip(this.btnImport, "استيراد / تصدير البيانات دروبشيبر");
                this.mnuBukaFileMaster.Text = "افتح ملف دروبشيبر الرئيسي";
                this.mnuImportFileMaster.Text = "استيراد ملف رئيسي دروبشيبر";
                this.mnuExportData.Text = "تصدير البيانات دروبشيبر";
            }
            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new DropshipperBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                {
                    LoadData();

                    btnImport.Enabled = _user.is_administrator;
                }                    

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfDropshipper.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name", Width = 400 });
            gridListProperties.Add(new GridListControlProperties { Header = "Address", Width = 700 });
            gridListProperties.Add(new GridListControlProperties { Header = "phone" });

            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<Dropshipper>(this.gridList, _listOfDropshipper, gridListProperties);

            if (_listOfDropshipper.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
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

                                case 4:
                                    e.Style.CellValue = dropshipper.phone;
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
                _listOfDropshipper = _bll.GetAll();

                GridListControlHelper.Refresh<Dropshipper>(this.gridList, _listOfDropshipper);
            }

            ResetButton();
        }

        private void LoadData(string dropshipperName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfDropshipper = _bll.GetByName(dropshipperName);

                GridListControlHelper.Refresh<Dropshipper>(this.gridList, _listOfDropshipper);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfDropshipper.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryDropshipper(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var dropshipper = _listOfDropshipper[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryDropshipper(formTitle + this.TabText, dropshipper, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            if (MsgHelper.MsgDelete())
            {
                var dropshipper = _listOfDropshipper[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(dropshipper);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Dropshipper>(this.gridList, _listOfDropshipper, dropshipper);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        protected override void OpenFileMaster()
        {
            var msg = "Key-FailedMaster";
            if (MsgHelper.MsgConfirmation(msg))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_dropshipper.xlsx";

                if (!File.Exists(fileMaster))
                {
                    MsgHelper.MsgWarning("Sorry file master Dropshipper not found.");
                    return;
                }

                try
                {
                    Process.Start(fileMaster);
                }
                catch
                {
                    msg = "Key-DropshipperFile";

                    MsgHelper.MsgError(msg);
                }
            }
        }

        protected override void ImportData()
        {
            var frm = new FrmImportDataDropshipper("Import Data Dropshipper from File Excel");
            frm.Listener = this;
            frm.ShowDialog();        
        }

        protected override void ExportData()
        {
            using (var dlgSave = new SaveFileDialog())
            {
                dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                dlgSave.Title = "Export Data Dropshipper";

                var result = dlgSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        IImportExportDataBll<Dropshipper> _importDataBll = new ImportExportDataDropshipperBll(dlgSave.FileName, _log);
                        _importDataBll.Export(_listOfDropshipper);
                    }
                }
            }
        }


        public void Ok(object sender, object data)
        {
            if (sender is FrmImportDataDropshipper)
            {
                LoadData(); // refresh data setelah import dari file excel
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var dropshipper = (Dropshipper)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Dropshipper>(this.gridList, _listOfDropshipper, dropshipper);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Dropshipper>(this.gridList, _listOfDropshipper, dropshipper);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    
                    if (MainProgram.currentLanguage == "en-US")
                    {
                        if (txtNameDropshipper.Text == "Find name dropshipper ...")
                            LoadData();
                        else
                            LoadData(txtNameDropshipper.Text);
                    }
                    else if (MainProgram.currentLanguage == "ar-SA")
                    {
                        if (txtNameDropshipper.Text == "البحث عن اسم دروبشيبر ...")
                            LoadData();
                        else
                            LoadData(txtNameDropshipper.Text);
                    }
                }
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfDropshipper.Count);
        }

        private void txtNameDropshipper_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }

        private void txtNameDropshipper_Leave(object sender, EventArgs e)
        {
            var txtFind = (AdvancedTextbox)sender;
            if (MainProgram.currentLanguage == "en-US")
            {
                if (txtFind.Text.Length == 0)
                    txtFind.Text = "Find name dropshipper ...";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                if (txtFind.Text.Length == 0)
                    txtFind.Text = "البحث عن اسم العميل ...";
            }
           
        }

        private void txtNameDropshipper_Enter(object sender, EventArgs e)
        {
            ((AdvancedTextbox)sender).Clear();
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }
    }
}
