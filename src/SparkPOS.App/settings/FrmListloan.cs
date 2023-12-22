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

namespace SparkPOS.App.Expense
{
    public partial class FrmListloan : FrmListEmptyBody, IListener
    {                
        private ILoanBll _bll; // deklarasi objek business logic layer 
        private IList<loan> _listOfLoan = new List<loan>();
        private IList<PaymentLoan> _listOfPaymentHistoryLoan = new List<PaymentLoan>();
        private IList<Employee> _listOfEmployee = new List<Employee>();
        private ILog _log;
        private User _user;
        private string _menuId;
        
        public FrmListloan(string header, User user, string menuId)
            : base()
        {
             InitializeComponent(); 

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;
            ColorManagerHelper.SetTheme(this, this);            

            _log = MainProgram.log;
            _bll = new LoanBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    LoadDataEmployee();
                    LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);                    
                }                    

                filterRangeDate.Enabled = role.is_grant;
            }            

            InitGridList();
            InitGridListPaymentHistory();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfLoan.Count);
            MainProgram.GlobalLanguageChange(this);
            SetLabelTextBasedOnLanguage();
        }        

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Employee", Width = 230 });
            gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Remaining", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<loan>(this.gridList, _listOfLoan, gridListProperties);

            if (_listOfLoan.Count > 0)
            {
                this.gridList.SetSelected(0, true);
                GridListHandleSelectionChanged(this.gridList);
            }                

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfLoan.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfLoan.Count)
                        {
                            var loan = _listOfLoan[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(loan.date);
                                    break;

                                case 3:
                                    e.Style.CellValue = loan.invoice;
                                    break;

                                case 4:
                                    var employee = loan.Employee;
                                    if (employee != null)
                                        e.Style.CellValue = loan.Employee.employee_name;

                                    break;

                                case 5:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(loan.amount);
                                    break;

                                case 6:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(loan.remaining);
                                    break;

                                case 7:
                                    e.Style.CellValue = loan.description;
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

            this.gridList.SelectedValueChanged += delegate(object sender, EventArgs e)
            {
                GridListHandleSelectionChanged((GridListControl)sender);
            };
        }

        private void GridListHandleSelectionChanged(GridListControl gridList)
        {
            if (gridList.SelectedIndex < 0)
                return;

            if (_listOfLoan.Count > 0 && !(gridList.SelectedIndex > _listOfLoan.Count))
            {
                var loan = _listOfLoan[gridList.SelectedIndex];
                if (loan != null)
                {
                    _listOfPaymentHistoryLoan = loan.item_payment_loan;
                    GridListControlHelper.Refresh<PaymentLoan>(this.gridListPaymentHistory, _listOfPaymentHistoryLoan);

                    btnTambahPayment.Enabled = loan.remaining > 0;
                    GridListPaymentHistoryHandleSelectionChanged(this.gridListPaymentHistory);
                }
            }
            else
            {
                _listOfPaymentHistoryLoan.Clear();
                GridListControlHelper.Refresh<PaymentLoan>(this.gridListPaymentHistory, _listOfPaymentHistoryLoan);

                ResetButtonPaymentHistory(false);
            }
        }

        private void InitGridListPaymentHistory()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<PaymentLoan>(this.gridListPaymentHistory, _listOfPaymentHistoryLoan, gridListProperties);

            if (_listOfPaymentHistoryLoan.Count > 0)
                this.gridListPaymentHistory.SetSelected(0, true);

            this.gridListPaymentHistory.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfPaymentHistoryLoan.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfPaymentHistoryLoan.Count)
                        {
                            var paymentLoan = _listOfPaymentHistoryLoan[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(paymentLoan.date);
                                    break;

                                case 3:
                                    e.Style.CellValue = paymentLoan.invoice;
                                    break;

                                case 4:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(paymentLoan.amount);
                                    break;

                                case 5:
                                    e.Style.CellValue = paymentLoan.description;
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

            this.gridListPaymentHistory.SelectedValueChanged += delegate(object sender, EventArgs e)
            {
                GridListPaymentHistoryHandleSelectionChanged((GridListControl)sender);
            };

            this.gridListPaymentHistory.DoubleClick += delegate(object sender, EventArgs e)
            {
                if (btnPerbaikiPayment.Enabled)
                    btnPerbaikiPayment_Click(sender, e);
            };
        }

        private void GridListPaymentHistoryHandleSelectionChanged(GridListControl gridList)
        {
            if (gridList.SelectedIndex < 0)
                return;

            ResetButtonPaymentHistory(false);

            if (_listOfPaymentHistoryLoan.Count > 0)
            {
                ResetButtonPaymentHistory(true);

                var paymentLoan = _listOfPaymentHistoryLoan[gridList.SelectedIndex];
                if (paymentLoan != null)
                {
                    // nonActivate tombol edit dan hapus jika payment loan dari gaji
                    ResetButtonPaymentHistory(paymentLoan.gaji_employee_id == null);
                }
            }            
        }

        private void LoadDataEmployee()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                IEmployeeBll bll = new EmployeeBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                _listOfEmployee = bll.GetAll();
            }
        }

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {                
                _listOfLoan = _bll.GetAll();
                GridListControlHelper.Refresh<loan>(this.gridList, _listOfLoan);                
            }

            ResetButton();

            btnTambahPayment.Enabled = _listOfLoan.Count > 0;
            GridListHandleSelectionChanged(this.gridListPaymentHistory);
        }

        private void LoadData(bool isLunas)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfLoan = _bll.GetByStatus(isLunas);
                GridListControlHelper.Refresh<loan>(this.gridList, _listOfLoan);
            }

            ResetButton();

            btnTambahPayment.Enabled = _listOfLoan.Count > 0;
            GridListHandleSelectionChanged(this.gridListPaymentHistory);
        }

        private void LoadData(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfLoan = _bll.GetByDate(tanggalMulai, tanggalSelesai);
                GridListControlHelper.Refresh<loan>(this.gridList, _listOfLoan);                
            }

            ResetButton();

            btnTambahPayment.Enabled = _listOfLoan.Count > 0;
            GridListHandleSelectionChanged(this.gridListPaymentHistory);
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfLoan.Count > 0);
            
            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfLoan.Count);
        }

        private void ResetButtonPaymentHistory(bool status)
        {
            btnPerbaikiPayment.Enabled = status;
            btnHapusPayment.Enabled = status;
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryLoan(formTitle + this.Text, _listOfEmployee, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var loan = _listOfLoan[index];

            LogicalThreadContext.Properties["OldValue"] = loan.ToJson();
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryLoan(formTitle + this.Text, loan, _listOfEmployee, _bll);
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
                var loan = _listOfLoan[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(loan);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<loan>(this.gridList, _listOfLoan, loan);
                        GridListHandleSelectionChanged(this.gridList);

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
            if (data is loan)
            {
                var loan = (loan)data;

                if (isNewData)
                {
                    GridListControlHelper.AddObject<loan>(this.gridList, _listOfLoan, loan);
                    ResetButton();
                    btnTambahPayment.Enabled = _listOfLoan.Count > 0;
                }
                else
                    GridListControlHelper.UpdateObject<loan>(this.gridList, _listOfLoan, loan);
            }            
            else if (data is PaymentLoan)
            {
                var index = this.gridList.SelectedIndex;
                var loan = _listOfLoan[index];
                var paymentLoan = (PaymentLoan)data;

                if (isNewData)
                {
                    loan.total_payment += paymentLoan.amount;
                    GridListControlHelper.UpdateObject<loan>(this.gridList, _listOfLoan, loan);

                    GridListControlHelper.AddObject<PaymentLoan>(this.gridListPaymentHistory, loan.item_payment_loan, paymentLoan);

                    btnTambahPayment.Enabled = loan.remaining > 0;
                    ResetButtonPaymentHistory(_listOfPaymentHistoryLoan.Count > 0);
                }
                else
                {
                    loan.total_payment -= paymentLoan.old_amount;
                    loan.total_payment += paymentLoan.amount;

                    btnTambahPayment.Enabled = loan.remaining > 0;
                    GridListControlHelper.UpdateObject<loan>(this.gridList, _listOfLoan, loan);
                    GridListControlHelper.UpdateObject<PaymentLoan>(this.gridListPaymentHistory, loan.item_payment_loan, paymentLoan);
                }                                    
            }
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

            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeDate_ChkShowAllDataClicked(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            if (chk.Checked)
                LoadData();
            else
                LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
        }

        private void btnTambahPayment_Click(object sender, EventArgs e)
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var loan = _listOfLoan[index];

            var frm = new FrmEntryPaymentLoan("Add Data Payment loan", loan);
            frm.Listener = this;
            frm.ShowDialog();
        }

        private void btnPerbaikiPayment_Click(object sender, EventArgs e)
        {
            var index = this.gridListPaymentHistory.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var loan = _listOfLoan[this.gridList.SelectedIndex];           

            if (_listOfPaymentHistoryLoan.Count > 0)
            {
                var paymentLoan = _listOfPaymentHistoryLoan[index];
                paymentLoan.old_amount = paymentLoan.amount;

                LogicalThreadContext.Properties["OldValue"] = paymentLoan.ToJson();

                var frm = new FrmEntryPaymentLoan("Edit Data Payment loan", loan, paymentLoan);
                frm.Listener = this;
                frm.ShowDialog();
            }            
        }

        private void btnHapusPayment_Click(object sender, EventArgs e)
        {
            var index = this.gridListPaymentHistory.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            if (MsgHelper.MsgDelete())
            {
                var paymentLoan = _listOfPaymentHistoryLoan[index];

                IPaymentLoanBll bll = new PaymentLoanBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = bll.Delete(paymentLoan);
                    if (result > 0)
                    {
                        var loan = _listOfLoan[this.gridList.SelectedIndex];

                        loan.total_payment -= paymentLoan.amount;
                        loan.item_payment_loan.Remove(paymentLoan);

                        GridListControlHelper.UpdateObject<loan>(this.gridList, _listOfLoan, loan);
                        GridListControlHelper.RemoveObject<PaymentLoan>(this.gridListPaymentHistory, _listOfPaymentHistoryLoan, paymentLoan);

                        btnTambahPayment.Enabled = loan.remaining > 0;
                        ResetButtonPaymentHistory(_listOfPaymentHistoryLoan.Count > 0);
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        private void chkShowYangBelumLunas_CheckedChanged(object sender, EventArgs e)
        {
            filterRangeDate.Enabled = !((CheckBox)sender).Checked;

            if (!filterRangeDate.Enabled)
            {
                LoadData(false);
            }
            else
            {
                filterRangeDate_BtnShowClicked(sender, e);
            }
        }
        private void SetLabelTextBasedOnLanguage()
        {
            if (MainProgram.currentLanguage == "en-US")
            {
                this.chkShowYangBelumLunas.Text = "shows that have not been paid in full";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.chkShowYangBelumLunas.Text = "تبين أنه لم يتم دفعها بالكامل";
            }
        }
    }
}
