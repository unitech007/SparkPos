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

using log4net;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Expense
{
    public partial class FrmEntrySalaryEmployee : FrmEntryStandard
    {                    
        private ISalaryEmployeeBll _bll = null; // deklarasi objek business logic layer 
        private SalaryEmployee _gaji = null;
        private Employee _employee;
        private IList<Employee> _listOfEmployee;
        
        private bool _isNewData = false;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntrySalaryEmployee(string header, string month, string year, IList<Employee> listOfEmployee, ISalaryEmployeeBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._listOfEmployee = listOfEmployee;
            this._bll = bll;
            this._user = MainProgram.user;

            this._isNewData = true;
            txtInvoice.Text = this._bll.GetLastInvoice();

            AddHandlerTotal();
            SetMonthYear(month, year);
            LoadDataEmployee();
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntrySalaryEmployee(string header, string month, string year, SalaryEmployee gaji, IList<Employee> listOfEmployee, ISalaryEmployeeBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._listOfEmployee = listOfEmployee;
            this._bll = bll;
            this._user = MainProgram.user;
            this._gaji = gaji;

            AddHandlerTotal();
            SetMonthYear(month, year);
            LoadDataEmployee();

            cmbEmployee.SelectedItem = this._gaji.Employee.employee_name;
            cmbEmployee.Enabled = false;

            txtInvoice.Text = this._gaji.invoice;
            dtpDate.Value = (DateTime)this._gaji.date;            

            AddHandlerTotal();

            var typePayment = this._gaji.Employee.payment_type;

            txtKehadiran.Text = this._gaji.attendance.ToString();
            txtAbsen.Text = this._gaji.absence.ToString();
            txtJumlahDay.Text = this._gaji.days_worked.ToString();
            txtSalary.Text = this._gaji.basic_salary.ToString();
            txtTunjangan.Text = this._gaji.allowance.ToString();
            txtJam.Text = this._gaji.time.ToString();
            txtLembur.Text = this._gaji.overtime.ToString();
            txtBonus.Text = this._gaji.bonus.ToString();
            txtPotongan.Text = this._gaji.deductions.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        private void AddHandlerTotal()
        {
            txtJumlahDay.TextChanged += RefreshTotal;
            txtSalary.TextChanged += RefreshTotal;
            txtTunjangan.TextChanged += RefreshTotal;
            txtJam.TextChanged += RefreshTotal;
            txtLembur.TextChanged += RefreshTotal;
            txtBonus.TextChanged += RefreshTotal;
            txtPotongan.TextChanged += RefreshTotal;
        }

        private void RefreshTotal(object sender, EventArgs e)
        {
            if (_employee == null)
                return;

            TypePayment typeSalary = _employee.payment_type;

            double allowance = NumberHelper.StringToDouble(txtTunjangan.Text);
            double bonus = NumberHelper.StringToDouble(txtBonus.Text);
            double deductions = NumberHelper.StringToDouble(txtPotongan.Text);

            double gaji = typeSalary == TypePayment.Monthly ? NumberHelper.StringToDouble(txtSalary.Text) : NumberHelper.StringToDouble(txtJumlahDay.Text) * NumberHelper.StringToDouble(txtSalary.Text);
            double overtime = typeSalary == TypePayment.Monthly ? NumberHelper.StringToDouble(txtLembur.Text) : NumberHelper.StringToDouble(txtJam.Text) * NumberHelper.StringToDouble(txtLembur.Text);

            txtTotal.Text = NumberHelper.NumberToString(gaji + overtime + allowance + bonus - deductions);
        }

        private void SetMonthYear(string month, string year)
        {
            cmbMonth.Items.Add(month);
            cmbMonth.SelectedItem = month;

            cmbYear.Items.Add(year);
            cmbYear.SelectedItem = year;
        }

        private void LoadDataEmployee()
        {
            cmbEmployee.Items.Add("--- Select employee ---");

            FillDataHelper.FillEmployee(cmbEmployee, _listOfEmployee, false);
            cmbEmployee.SelectedIndex = 0;
        }

        protected override void Save()
        {
            if (_isNewData)
            {
                _gaji = new SalaryEmployee();
            
                if (this._employee == null)
                {
                    MsgHelper.MsgWarning("Employee Not yet selected");
                    return;
                }

                _gaji.employee_id = _employee.employee_id;
                _gaji.Employee = _employee;

                _gaji.month = DayMonthHelper.GetMonthAngka(cmbMonth.Text);
                _gaji.year = int.Parse(cmbYear.Text);
            }

            _gaji.user_id = this._user.user_id;
            _gaji.User = this._user;
            _gaji.invoice = txtInvoice.Text;            

            _gaji.date = dtpDate.Value;
            _gaji.attendance = int.Parse(txtKehadiran.Text);
            _gaji.absence = int.Parse(txtAbsen.Text);

            _gaji.days_worked = int.Parse(txtJumlahDay.Text);
            _gaji.basic_salary = NumberHelper.StringToDouble(txtSalary.Text);
            _gaji.allowance = NumberHelper.StringToDouble(txtTunjangan.Text);
            _gaji.bonus = NumberHelper.StringToDouble(txtBonus.Text);
            _gaji.time = int.Parse(txtJam.Text);
            _gaji.overtime = NumberHelper.StringToDouble(txtLembur.Text);
            _gaji.deductions = NumberHelper.StringToDouble(txtPotongan.Text);

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_gaji, ref validationError);
                else
                    result = _bll.Update(_gaji, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _gaji);

                    if (_isNewData)
                    {
                        cmbEmployee.SelectedIndex = 0;
                        cmbEmployee.Focus();

                        txtInvoice.Text = _bll.GetLastInvoice();
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
                    {
                        var pesan = string.Format("Key-CheckSalary", _gaji.Employee.employee_name);
                        MsgHelper.MsgWarning(pesan);
                    }
                }
            }                            
        }

        private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cmbEmployee.SelectedIndex;

            if (index == 0)
            {
                base.ResetForm(this);
                this._employee = null;
                return;
            }                

            txtTitles.Clear();

            this._employee = _listOfEmployee[index - 1];
            var job_titles = this._employee.job_titles;

            if (job_titles != null)
                txtTitles.Text = job_titles.name_job_titles;

            if (this._employee != null)
            {
                switch (_employee.payment_type)
                {
                    case TypePayment.Weekly:
                        txtJumlahDay.Visible = true;
                        label9.Visible = true;
                        txtJam.Visible = true;
                        label10.Visible = true;
                        break;

                    case TypePayment.Monthly:
                        txtJumlahDay.Visible = false;
                        label9.Visible = false;
                        txtJam.Visible = false;
                        label10.Visible = false;
                        break;

                    default:
                        break;
                }

                txtSalary.Text = this._employee.basic_salary.ToString();
                txtLembur.Text = this._employee.overtime_salary.ToString();
            }
        }

        private void txtPotongan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }
    }
}
