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
using MultilingualApp;

namespace SparkPOS.App.Transactions
{
    public partial class FrmListPaymentCreditSalesProduct : FrmListEmptyBody, IListener
    {
        private IPaymentCreditProductBll _bll; // deklarasi objek business logic layer 
        private IList<PaymentCreditProduct> _listOfPaymentCredit = new List<PaymentCreditProduct>();
        private ILog _log;
        private User _user;
        private string _menuId;

        public FrmListPaymentCreditSalesProduct(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  
            
          //  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new PaymentCreditProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);

                txtNameCustomer.Enabled = role.is_grant;
                btnFind.Enabled = role.is_grant;

                filterRangeDate.Enabled = role.is_grant;
            }            

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfPaymentCredit.Count);
            MainProgram.GlobalLanguageChange(this);

        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice Payment", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Customer", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Payment", Width = 150 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit, gridListProperties);

            if (_listOfPaymentCredit.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfPaymentCredit.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfPaymentCredit.Count)
                        {
                            var payment = _listOfPaymentCredit[rowIndex];
                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(payment.date);
                                    break;

                                case 3:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = payment.invoice;
                                    break;

                                case 4:
                                    var customer = payment.Customer;

                                    if (customer != null)
                                        e.Style.CellValue = customer.name_customer;

                                    break;

                                case 5:
                                    var total = payment.item_payment_credit.Sum(f => f.amount);

                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(total);

                                    break;

                                case 6:
                                    e.Style.CellValue = payment.description;
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
                _listOfPaymentCredit = _bll.GetAll();
                GridListControlHelper.Refresh<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit);
            }

            ResetButton();
        }

        private void LoadData(string customerName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfPaymentCredit = _bll.GetByName(customerName);
                GridListControlHelper.Refresh<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit);
            }

            ResetButton();
        }

        private void LoadData(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfPaymentCredit = _bll.GetByDate(tanggalMulai, tanggalSelesai);
                GridListControlHelper.Refresh<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfPaymentCredit.Count > 0);

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfPaymentCredit.Count);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryPaymentCreditSalesProduct(formTitle + this.Text, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var payment = _listOfPaymentCredit[index];

            LogicalThreadContext.Properties["OldValue"] = payment.ToJson();
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryPaymentCreditSalesProduct(formTitle + this.Text, payment, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var payment = _listOfPaymentCredit[index];
            if (payment.is_cash)
            {
                MsgHelper.MsgWarning("Sorry payment credit sales cash can not be deleted");
                return;
            }

            if (MsgHelper.MsgDelete())
            {                
                var result = _bll.Delete(payment);
                if (result > 0)
                {
                    GridListControlHelper.RemoveObject<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit, payment);
                    ResetButton();
                }
                else
                    MsgHelper.MsgDeleteError();
            }
        }

        public void Ok(object sender, object data)
        {
            throw new NotImplementedException();
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var payment = (PaymentCreditProduct)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit, payment);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<PaymentCreditProduct>(this.gridList, _listOfPaymentCredit, payment);
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }

        private void filterRangeDate_BtnShowClicked(object sender, EventArgs e)
        {
            var tanggalMulai = filterRangeDate.DateMulai;
            var tanggalSelesai = filterRangeDate.DateSelesai;

            if (!DateTimeHelper.IsValidRangeDate(tanggalMulai, tanggalSelesai))
            {
                MsgHelper.MsgNotValidRangeDate();
                return;
            }

            txtNameCustomer.Clear();
            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeDate_ChkShowAllDataClicked(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            txtNameCustomer.Clear();

            if (chk.Checked)
            {
                LoadData();
                txtNameCustomer.Enabled = false;
                btnFind.Enabled = false;
            }                
            else
            {
                LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                txtNameCustomer.Enabled = true;
                btnFind.Enabled = true;
            }                
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (txtNameCustomer.Text.Length > 0)
                LoadData(txtNameCustomer.Text);
        }

        private void txtNameCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }
    }
}
