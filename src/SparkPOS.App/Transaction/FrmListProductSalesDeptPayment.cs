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
    public partial class FrmListProductPurchaseDebitPayment : FrmListEmptyBody, IListener
    {
        private IDebtPaymentProductBll _bll; // deklarasi objek business logic layer 
        private IList<DebtPaymentProduct> _listOfDebtPayment = new List<DebtPaymentProduct>();
        private ILog _log;
        private User _user;
        private string _menuId;

        public FrmListProductPurchaseDebitPayment(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new DebtPaymentProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);

                txtNameSupplier.Enabled = role.is_grant;
                btnFind.Enabled = role.is_grant;

                filterRangeDate.Enabled = role.is_grant;
            }

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfDebtPayment.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice Payment", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Supplier", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Payment", Width = 150 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<DebtPaymentProduct>(this.gridList, _listOfDebtPayment, gridListProperties);

            if (_listOfDebtPayment.Count > 0)
                this.gridList.SetSelected(0, true);

      //      MainProgram.GlobalLanguageChange(this);
            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfDebtPayment.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfDebtPayment.Count)
                        {
                            var payment = _listOfDebtPayment[rowIndex];
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
                                    var supplier = payment.Supplier;

                                    if (supplier != null)
                                        e.Style.CellValue = supplier.name_supplier;

                                    break;

                                case 5:
                                    var total = payment.item_payment_debt.Sum(f => f.amount);

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
                _listOfDebtPayment = _bll.GetAll();
                GridListControlHelper.Refresh<DebtPaymentProduct>(this.gridList, _listOfDebtPayment);
            }

            ResetButton();
        }

        private void LoadData(string supplierName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfDebtPayment = _bll.GetByName(supplierName);
                GridListControlHelper.Refresh<DebtPaymentProduct>(this.gridList, _listOfDebtPayment);
            }

            ResetButton();
        }

        private void LoadData(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfDebtPayment = _bll.GetByDate(tanggalMulai, tanggalSelesai);
                GridListControlHelper.Refresh<DebtPaymentProduct>(this.gridList, _listOfDebtPayment);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfDebtPayment.Count > 0);

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfDebtPayment.Count);
        }

        protected override void Add()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
                var frm = new FrmEntryDebtPaymentProductPurchase(formTitle + this.Text, _bll);
                frm.Listener = this;
                frm.ShowDialog();
            }            
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                var payment = _listOfDebtPayment[index];

                LogicalThreadContext.Properties["OldValue"] = payment.ToJson();
                string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
                var frm = new FrmEntryDebtPaymentProductPurchase(formTitle + this.Text, payment, _bll);
                frm.Listener = this;
                frm.ShowDialog();
            }            
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                var payment = _listOfDebtPayment[index];
                if (payment.is_cash)
                {
                    MsgHelper.MsgWarning("Sorry Dept Payment Purchase cash can not be deleted");
                    return;
                }

                if (MsgHelper.MsgDelete())
                {
                    var result = _bll.Delete(payment);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<DebtPaymentProduct>(this.gridList, _listOfDebtPayment, payment);
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
            var payment = (DebtPaymentProduct)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<DebtPaymentProduct>(this.gridList, _listOfDebtPayment, payment);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<DebtPaymentProduct>(this.gridList, _listOfDebtPayment, payment);
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

            txtNameSupplier.Clear();
            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeDate_ChkShowAllDataClicked(object sender, EventArgs e)
        {
            txtNameSupplier.Clear();

            var chk = (CheckBox)sender;

            if (chk.Checked)
            {
                LoadData();
                txtNameSupplier.Enabled = false;
                btnFind.Enabled = false;
            }                
            else
            {
                LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                txtNameSupplier.Enabled = true;
                btnFind.Enabled = true;
            }                
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (txtNameSupplier.Text.Length > 0)
                LoadData(txtNameSupplier.Text);
        }

        private void txtNameSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }
    }
}
