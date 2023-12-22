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

namespace SparkPOS.App.Reference
{
    public partial class FrmListEmployee : FrmListStandard, IListener
    {
        private IEmployeeBll _bll; // deklarasi objek business logic layer 
        private IList<Employee> _listOfEmployee = new List<Employee>();
        private IList<job_titles> _listOfTitles = new List<job_titles>();
        private ILog _log;

        public FrmListEmployee(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent();  

            _log = MainProgram.log;
            _bll = new EmployeeBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                    LoadData();

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfEmployee.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Address", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "phone", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Type Payment", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Status", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "job_titles", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Remaining loan" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<Employee>(this.gridList, _listOfEmployee, gridListProperties);

            if (_listOfEmployee.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfEmployee.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfEmployee.Count)
                        {
                            var employee = _listOfEmployee[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = employee.employee_name;
                                    break;

                                case 3:
                                    e.Style.CellValue = employee.address;
                                    break;

                                case 4:
                                    e.Style.CellValue = employee.phone;
                                    break;

                                case 5:
                                    e.Style.CellValue = employee.payment_type == TypePayment.Weekly ? "Weekly" : "Monthly";
                                    break;

                                case 6:
                                    e.Style.CellValue = employee.is_active ? "Active" : "Non Active";
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 7:
                                    var job_titles = employee.job_titles;

                                    if (job_titles != null)
                                        e.Style.CellValue = job_titles.name_job_titles;

                                    break;

                                case 8:
                                    e.Style.CellValue = NumberHelper.NumberToString(employee.remaining_kasbon);
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
                _listOfEmployee = _bll.GetAll();

                ITitlesBll titlesBll = new TitlesBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                _listOfTitles = titlesBll.GetAll();
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfEmployee.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryEmployee(formTitle + this.TabText, _listOfTitles, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var employee = _listOfEmployee[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryEmployee(formTitle + this.TabText, employee, _listOfTitles, _bll);
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
                var employee = _listOfEmployee[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(employee);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Employee>(this.gridList, _listOfEmployee, employee);
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
            var employee = (Employee)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Employee>(this.gridList, _listOfEmployee, employee);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Employee>(this.gridList, _listOfEmployee, employee);
        }
    }
}
