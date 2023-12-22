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
    public partial class FrmEntryLoan : FrmEntryStandard
    {            
        private ILoanBll _bll = null; // deklarasi objek business logic layer 
        private loan _kasbon = null;
        private IList<Employee> _listOfEmployee;
        
        private bool _isNewData = false;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntryLoan(string header, IList<Employee> listOfEmployee, ILoanBll bll)
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

            LoadDataEmployee();
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryLoan(string header, loan loan, IList<Employee> listOfEmployee, ILoanBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._listOfEmployee = listOfEmployee;
            this._bll = bll;
            this._user = MainProgram.user;
            this._kasbon = loan;

            txtInvoice.Text = this._kasbon.invoice;
            dtpDate.Value = (DateTime)this._kasbon.date;
            txtKeterangan.Text = this._kasbon.description;
            txtJumlah.Text = this._kasbon.amount.ToString();

            LoadDataEmployee();
            if (this._kasbon.Employee != null)
                cmbEmployee.SelectedItem = this._kasbon.Employee.employee_name;
            MainProgram.GlobalLanguageChange(this);
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
                _kasbon = new loan();                

            if (cmbEmployee.SelectedIndex == 0)
            {
                MsgHelper.MsgWarning("Employee Not yet selected");
                return;
            }

            _kasbon.invoice = txtInvoice.Text;
            _kasbon.date = dtpDate.Value;
            _kasbon.amount = NumberHelper.StringToDouble(txtJumlah.Text);
            _kasbon.description = txtKeterangan.Text;

            var employee = _listOfEmployee[cmbEmployee.SelectedIndex - 1];
            _kasbon.employee_id = employee.employee_id;
            _kasbon.Employee = employee;

            _kasbon.user_id = this._user.user_id;
            _kasbon.User = this._user;

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_kasbon, ref validationError);
                else
                    result = _bll.Update(_kasbon, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _kasbon);
                    this.Close();
                }
                else
                {
                    if (validationError.Message.NullToString().Length > 0)
                    {
                        MsgHelper.MsgWarning(validationError.Message);
                        base.SetFocusObject(validationError.PropertyName, this);
                    }
                }
            }                            
        }

        private void txtKeterangan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }
    }
}
