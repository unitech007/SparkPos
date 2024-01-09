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
    public partial class FrmListSupplier : FrmListEmptyBody, IListener
    {        
        private ISupplierBll _bll; // deklarasi objek business logic layer 
        private IList<Supplier> _listOfSupplier = new List<Supplier>();
        private ILog _log;
        private User _user;
        private string _menuId = string.Empty;

        public FrmListSupplier(string header, User user, string menuId)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            this.btnImport.Visible = true;
        
            if (MainProgram.currentLanguage == "en-US")
            {
                this.toolTip1.SetToolTip(this.btnImport, "Import/Export Data Supplier");

                this.mnuBukaFileMaster.Text = "Open the Supplier Master File";
                this.mnuImportFileMaster.Text = "Import File Master Supplier";
                this.mnuExportData.Text = "Export Data Supplier";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.toolTip1.SetToolTip(this.btnImport, "مورد بيانات الاستيراد / التصدير");

                this.mnuBukaFileMaster.Text = "افتح الملف الرئيسي للمورد";
                this.mnuImportFileMaster.Text = "استيراد ملف المورد الرئيسي";
                this.mnuExportData.Text = "مورد بيانات التصدير";
            }
            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new SupplierBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                {
                    LoadData();

                    btnImport.Enabled = user.is_administrator;
                }                    

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSupplier.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "Address", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Contact", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "phone", Width = 130 });
            gridListProperties.Add(new GridListControlProperties { Header = "Remaining Debt", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "CR NO", Width = 60 });
            gridListProperties.Add(new GridListControlProperties { Header = "VAT NO", Width = 60 });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<Supplier>(this.gridList, _listOfSupplier, gridListProperties);

            if (_listOfSupplier.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
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

                                case 4:
                                    e.Style.CellValue = supplier.contact;
                                    break;

                                case 5:
                                    e.Style.CellValue = supplier.phone;
                                    break;

                                case 6:
                                    e.Style.CellValue = NumberHelper.NumberToString(supplier.total_debt - supplier.total_debt_payment);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;
                                case 7:
                                    e.Style.CellValue = supplier.cr_no;
                                    break;

                                case 8:
                                    e.Style.CellValue = supplier.vat_no;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
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
                _listOfSupplier = _bll.GetAll();

                GridListControlHelper.Refresh<Supplier>(this.gridList, _listOfSupplier);
            }

            ResetButton();
        }

        private void LoadData(string supplierName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfSupplier = _bll.GetByName(supplierName);

                GridListControlHelper.Refresh<Supplier>(this.gridList, _listOfSupplier);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfSupplier.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntrySupplier(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var supplier = _listOfSupplier[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntrySupplier(formTitle + this.TabText, supplier, _bll);
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
                var supplier = _listOfSupplier[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(supplier);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Supplier>(this.gridList, _listOfSupplier, supplier);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        protected override void OpenFileMaster()
        {
            var msg = "Key-SupplierMaster";

            if (MsgHelper.MsgConfirmation(msg))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_supplier.xlsx";

                if (!File.Exists(fileMaster))
                {
                    MsgHelper.MsgWarning("Sorry file master Supplier not found.");
                    return;
                }

                try
                {
                    Process.Start(fileMaster);
                }
                //catch
                //{
                //    msg = "Key-FailedSupplier";
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
            var frm = new FrmImportDataSupplier("Import Data Supplier from File Excel");
            frm.Listener = this;
            frm.ShowDialog();        
        }

        protected override void ExportData()
        {
            using (var dlgSave = new SaveFileDialog())
            {
                dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                dlgSave.Title = "Export Data Supplier";

                var result = dlgSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        IImportExportDataBll<Supplier> _importDataBll = new ImportExportDataSupplierBll(dlgSave.FileName, _log);
                        _importDataBll.Export(_listOfSupplier);
                    }
                }
            }
        }


        public void Ok(object sender, object data)
        {
            if (sender is FrmImportDataSupplier)
            {
                LoadData(); // refresh data setelah import dari file excel
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var supplier = (Supplier)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Supplier>(this.gridList, _listOfSupplier, supplier);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Supplier>(this.gridList, _listOfSupplier, supplier);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    if (txtNameSupplier.Text == "Find name supplier ...")
                        LoadData();
                    else
                        LoadData(txtNameSupplier.Text);
                }
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSupplier.Count);
        }

        private void txtNameSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }

        private void txtNameSupplier_Leave(object sender, EventArgs e)
        {
            var txtFind = (AdvancedTextbox)sender;
            if (MainProgram.currentLanguage == "en-US")
            {
                if (txtFind.Text.Length == 0)
                    txtFind.Text = "Find name supplier ...";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                if (txtFind.Text.Length == 0)
                    txtFind.Text = "البحث عن اسم المورد ...";
            }
           
        }

        private void txtNameSupplier_Enter(object sender, EventArgs e)
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
