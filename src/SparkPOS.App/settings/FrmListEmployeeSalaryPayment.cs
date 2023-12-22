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
    public partial class FrmListEmployeeSalaryPayment : FrmListEmptyBody, IListener
    {                
        private ISalaryEmployeeBll _bll; // deklarasi objek business logic layer 
        private IList<SalaryEmployee> _listOfSalary = new List<SalaryEmployee>();
        private IList<Employee> _listOfEmployee = new List<Employee>();
        private ILog _log;
        private User _user;
        private string _menuId;
        
        public FrmListEmployeeSalaryPayment(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new SalaryEmployeeBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _menuId = menuId;

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    LoadMonthDanYear();

                    var month = cmbMonth.SelectedIndex + 1;
                    var year = int.Parse(cmbYear.Text);

                    LoadData(month, year);
                    LoadDataEmployee();
                }
            }            

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSalary.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadDataEmployee()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                IEmployeeBll bll = new EmployeeBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                _listOfEmployee = bll.GetAll();
            }
        }

        private void LoadMonthDanYear()
        {
            FillDataHelper.FillMonth(cmbMonth, true);
            FillDataHelper.FillYear(cmbYear, true);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 80 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name", Width = 200 });
            gridListProperties.Add(new GridListControlProperties { Header = "job_titles", Width = 200 });
            gridListProperties.Add(new GridListControlProperties { Header = "attendance", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "absence", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "Salary", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "allowance", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Bonus", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "overtime", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "deductions", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Total" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<SalaryEmployee>(this.gridList, _listOfSalary, gridListProperties);

            if (_listOfSalary.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfSalary.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfSalary.Count)
                        {
                            var gaji = _listOfSalary[rowIndex];                            

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(gaji.date);
                                    break;

                                case 3:
                                    var employee = gaji.Employee;
                                    if (employee != null)
                                        e.Style.CellValue = employee.employee_name;

                                    break;

                                case 4:
                                    var job_titles = gaji.Employee.job_titles;

                                    if (job_titles != null)
                                        e.Style.CellValue = job_titles.name_job_titles;

                                    break;

                                case 5:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = gaji.attendance;
                                    break;

                                case 6:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = gaji.absence;
                                    break;

                                case 7:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(gaji.gaji_akhir);

                                    break;

                                case 8:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(gaji.allowance);

                                    break;

                                case 9:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(gaji.bonus);

                                    break;

                                case 10:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(gaji.lembur_akhir);

                                    break;

                                case 11:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(gaji.deductions);

                                    break;

                                case 12:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(gaji.total_gaji);

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

        private void LoadData(int month, int year)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfSalary = _bll.GetByMonthAndYear(month, year);
                GridListControlHelper.Refresh<SalaryEmployee>(this.gridList, _listOfSalary);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfSalary.Count > 0);

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSalary.Count);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntrySalaryEmployee(formTitle + this.Text, cmbMonth.Text, cmbYear.Text, _listOfEmployee, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var gaji = _listOfSalary[index];

            LogicalThreadContext.Properties["OldValue"] = gaji.ToJson();
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntrySalaryEmployee(formTitle + this.Text, cmbMonth.Text, cmbYear.Text, gaji, _listOfEmployee, _bll);
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
                var expense = _listOfSalary[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(expense);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<SalaryEmployee>(this.gridList, _listOfSalary, expense);
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
            var gaji = (SalaryEmployee)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<SalaryEmployee>(this.gridList, _listOfSalary, gaji);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<SalaryEmployee>(this.gridList, _listOfSalary, gaji);
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            var month = cmbMonth.SelectedIndex + 1;
            var year = int.Parse(cmbYear.Text);

            LoadData(month, year);
        }
    }
}
