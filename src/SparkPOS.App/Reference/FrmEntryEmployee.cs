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
using System.Windows.Forms;

using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Reference
{
    public partial class FrmEntryEmployee : FrmEntryStandard
    {
        private IEmployeeBll _bll = null; // deklarasi objek business logic layer 
        private Employee _employee = null;
        private IList<job_titles> _listOfTitles;
        private bool _isNewData = false;
        
        public IListener Listener { private get; set; }

        public FrmEntryEmployee(string header, IList<job_titles> listOfTitles, IEmployeeBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._listOfTitles = listOfTitles;
            this._bll = bll;

            this._isNewData = true;

            LoadTitles();
            cmbTypeSalary.SelectedIndex = 0;
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryEmployee(string header, Employee employee, IList<job_titles> listOfTitles, IEmployeeBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._listOfTitles = listOfTitles;
            this._bll = bll;
            this._employee = employee;
            LoadTitles();

            txtName.Text = this._employee.employee_name;
            txtAddress.Text = this._employee.address;
            txtphone.Text = this._employee.phone;

            if (this._employee.is_active)
                rdoActive.Checked = true;
            else
                rdoNonActive.Checked = true;

            if (this._employee.job_titles != null)
                cmbTitles.SelectedItem = this._employee.job_titles.name_job_titles;

            // settings gaji            
            cmbTypeSalary.SelectedIndex = this._employee.payment_type == TypePayment.Weekly ? 0 : 1;
            txtSalaryPokok.Text = this._employee.basic_salary.ToString();
            txtLembur.Text = this._employee.overtime_salary.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadTitles()
        {
            cmbTitles.Items.Clear();
            foreach (var job_titles in _listOfTitles)
            {
                cmbTitles.Items.Add(job_titles.name_job_titles);
            }

            if (_listOfTitles.Count > 0)
                cmbTitles.SelectedIndex = 0;
        }

        protected override void Save()
        {
            if (_isNewData)
                _employee = new Employee();

            _employee.employee_name = txtName.Text;
            _employee.address = txtAddress.Text;
            _employee.phone = txtphone.Text;
            _employee.is_active = rdoActive.Checked ? true : false;

            var job_titles = _listOfTitles[cmbTitles.SelectedIndex];
            _employee.job_titles_id = job_titles.job_titles_id;
            _employee.job_titles = job_titles;

            _employee.payment_type = (TypePayment)cmbTypeSalary.SelectedIndex;
            _employee.basic_salary = NumberHelper.StringToDouble(txtSalaryPokok.Text);
            _employee.overtime_salary = NumberHelper.StringToDouble(txtLembur.Text);

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_employee, ref validationError);
                else
                    result = _bll.Update(_employee, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _employee);

                    if (_isNewData)
                    {
                        base.ResetForm(this);
                        txtName.Focus();

                    }
                    else
                        this.Close();

                }
                else
                {
                    if (validationError.Message.NullToString().Length > 0)
                    {
                        MsgHelper.MsgWarning(validationError.Message);
                        base.SetFocusObject(validationError.PropertyName, this);
                    }
                    else
                        MsgHelper.MsgUpdateError();
                }
            }                            
        }

        private void txtCategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void txtLembur_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void cmbTypeSalary_SelectedIndexChanged(object sender, EventArgs e)
        {
            label8.Text = ((ComboBox)sender).SelectedIndex == 0 ? "Salary Per Day" : "Salary Monthly";
        }
    }
}
