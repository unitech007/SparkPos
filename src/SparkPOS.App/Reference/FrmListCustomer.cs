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
    public partial class FrmListCustomer : FrmListEmptyBody, IListener
    {
        private ICustomerBll _bll; // deklarasi objek business logic layer 
        private IList<Customer> _listOfCustomer = new List<Customer>();        
        private ILog _log;
        private User _user;
        private string _menuId = string.Empty;

        public FrmListCustomer(string header, User user, string menuId)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            this.btnImport.Visible = true;
            

            //cmbTypeCustomer.Items.Clear();
            if (MainProgram.currentLanguage == "en-US")
            {
                this.toolTip1.SetToolTip(this.btnImport, "Import/Export Data Customer");
                this.mnuBukaFileMaster.Text = "Open Customer Master File";
                this.mnuImportFileMaster.Text = "Import File Master Customer";
                this.mnuExportData.Text = "Export Data Customer";
              
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.toolTip1.SetToolTip(this.btnImport, "استيراد / تصدير بيانات الزبون");
                this.mnuBukaFileMaster.Text = "افتح ملف العميل الرئيسي";
                this.mnuImportFileMaster.Text = "استيراد ملف رئيسي للعميل";
                this.mnuExportData.Text = "عميل بيانات التصدير";
            }
            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;
            
            _log = MainProgram.log;
            _bll = new CustomerBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            cmbTypeCustomer.Enabled = false;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    cmbTypeCustomer.SelectedIndex = 0;

                cmbTypeCustomer.Enabled = role.is_grant;
                btnImport.Enabled = _user.is_administrator;
            }                


            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCustomer.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name", Width = 150 });

            gridListProperties.Add(new GridListControlProperties { Header = "Provinsi", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Regency", Width = 140 });
            gridListProperties.Add(new GridListControlProperties { Header = "subdistrict", Width = 140 });            
            gridListProperties.Add(new GridListControlProperties { Header = "Address", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Pos", Width = 70 });

            gridListProperties.Add(new GridListControlProperties { Header = "Contact", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "phone", Width = 100 });

            gridListProperties.Add(new GridListControlProperties { Header = "discount", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "Limit Credit", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Remaining Credit" , Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "CR NO", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "VAT NO", Width = 50 });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<Customer>(this.gridList, _listOfCustomer, gridListProperties);

            if (_listOfCustomer.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
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
                                    e.Style.CellValue = customer.Provinsi != null ? customer.Provinsi.name_province : string.Empty;
                                    break;

                                case 4:
                                    e.Style.CellValue = customer.Regency != null ? customer.Regency.name_regency : customer.regency_old.NullToString();
                                    break;

                                case 5:
                                    e.Style.CellValue = customer.subdistrict != null ? customer.subdistrict.name_subdistrict : customer.subdistrict_old.NullToString();
                                    break;

                                case 6:
                                    e.Style.CellValue = customer.address;
                                    break;

                                case 7:
                                    e.Style.CellValue = customer.postal_code;
                                    break;

                                case 8:
                                    e.Style.CellValue = customer.contact;
                                    break;

                                case 9:
                                    e.Style.CellValue = customer.phone;
                                    break;

                                case 10:
                                    e.Style.CellValue = customer.discount;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 11:
                                    e.Style.CellValue = NumberHelper.NumberToString(customer.credit_limit);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;
                                

                                case 12:
                                    e.Style.CellValue = NumberHelper.NumberToString(customer.total_credit - customer.total_receivable_payment);
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;

                                case 13:
                                    e.Style.CellValue = customer.cr_no;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    break;
                                case 14:
                                    e.Style.CellValue = customer.vat_no;
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
                _listOfCustomer = _bll.GetAll();

                GridListControlHelper.Refresh<Customer>(this.gridList, _listOfCustomer);
            }

            ResetButton();
        }

        private void LoadData(bool isReseller)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfCustomer = _bll.GetAll(isReseller);

                GridListControlHelper.Refresh<Customer>(this.gridList, _listOfCustomer);
            }

            ResetButton();
        }

        private void LoadData(string customerName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfCustomer = _bll.GetByName(customerName);

                GridListControlHelper.Refresh<Customer>(this.gridList, _listOfCustomer);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfCustomer.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryCustomer(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var customer = _listOfCustomer[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryCustomer(formTitle + this.TabText, customer, _bll);
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
                var customer = _listOfCustomer[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(customer);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Customer>(this.gridList, _listOfCustomer, customer);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        protected override void OpenFileMaster()
        {
            var msg = "Key-OpeningFile";


            if (MsgHelper.MsgConfirmation(msg))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_customer.xlsx";

                if (!File.Exists(fileMaster))
                {
                    MsgHelper.MsgWarning("Sorry, the Customer master file was not found.");
                    return;

                }

                try
                {
                    Process.Start(fileMaster);
                }
                catch
                {
                    msg = "Key-FailedFile";


                    MsgHelper.MsgError(msg);
                }
            }
        }

        protected override void ImportData()
        {
            var frm = new FrmImportDataCustomer("Import Data Customer to File Excel");
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void ExportData()
        {
            using (var dlgSave = new SaveFileDialog())
            {
                dlgSave.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                dlgSave.Title = "Export Data Customer";

                var result = dlgSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        IImportExportDataBll<Customer> _importDataBll = new ImportExportDataCustomerBll(dlgSave.FileName, _log);
                        _importDataBll.Export(_listOfCustomer);
                    }
                }
            }
        }

        public void Ok(object sender, object data)
        {
            if (sender is FrmImportDataCustomer)
            {
                LoadData(); // refresh data setelah import dari file excel
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var customer = (Customer)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Customer>(this.gridList, _listOfCustomer, customer);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Customer>(this.gridList, _listOfCustomer, customer);
        }

        private void cmbTypeCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = ((ComboBox)sender).SelectedIndex;

            switch (index)
            {
                case 0: // semua
                    LoadData();
                    break;

                case 1: // General
                    LoadData(false);
                    break;

                case 2: // reseller
                    LoadData(true);
                    break;

                default:
                    break;
            }
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    if(MainProgram.currentLanguage == "en-US")
                    {
                        if (txtNameCustomer.Text == "Find name customer ...")
                            LoadData();
                        else
                            LoadData(txtNameCustomer.Text);
                    }
                    else if(MainProgram.currentLanguage == "ar-SA")
                    {
                        if (txtNameCustomer.Text == "البحث عن اسم العميل ...")
                            LoadData();
                        else
                            LoadData(txtNameCustomer.Text);
                    }
                   
                   
                }
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfCustomer.Count);
        }

        private void txtNameCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }

        private void txtNameCustomer_Enter(object sender, EventArgs e)
        {
            ((AdvancedTextbox)sender).Clear();
        }

        private void txtNameCustomer_Leave(object sender, EventArgs e)
        {
            var txtFind = (AdvancedTextbox)sender;
            if (MainProgram.currentLanguage == "en-US")
            {
                if (txtFind.Text.Length == 0)
                    txtFind.Text = "Find name customer ...";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                if (txtFind.Text.Length == 0)
                    txtFind.Text = "البحث عن اسم العميل ...";
            }
           
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {

        }
    }
}
