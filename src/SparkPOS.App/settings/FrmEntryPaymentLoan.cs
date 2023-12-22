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
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Expense
{
    public partial class FrmEntryPaymentLoan : FrmEntryStandard
    {                    
        private IPaymentLoanBll _bll = null; // deklarasi objek business logic layer 
        private loan _kasbon = null;
        private PaymentLoan _paymentLoan = null;
        
        private bool _isNewData = false;
        private User _user;
        private ILog _log;

        public IListener Listener { private get; set; }

        public FrmEntryPaymentLoan(string header, loan loan)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);

            this._log = MainProgram.log;
            this._bll = new PaymentLoanBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            this._kasbon = loan;
            this._user = MainProgram.user;

            this._isNewData = true;
            txtInvoice.Text = this._bll.GetLastInvoice();
            txtNameEmployee.Text = this._kasbon.Employee.employee_name;
            txtRemainingLoan.Text = this._kasbon.remaining.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryPaymentLoan(string header, loan loan, PaymentLoan paymentLoan)
            : base()
        {
             InitializeComponent();
            ColorManagerHelper.SetTheme(this, this);
            
            base.SetHeader(header);
            base.SetButtonSelesaiToClose();

            this._log = MainProgram.log;
            this._bll = new PaymentLoanBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            this._kasbon = loan;
            this._paymentLoan = paymentLoan;
            this._user = MainProgram.user;

            txtInvoice.Text = this._paymentLoan.invoice;
            dtpDate.Value = (DateTime)this._paymentLoan.date;
            txtNameEmployee.Text = this._kasbon.Employee.employee_name;
            txtRemainingLoan.Text = this._kasbon.remaining.ToString();
            txtKeterangan.Text = this._paymentLoan.description;
            txtJumlah.Text = this._paymentLoan.amount.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        protected override void Save()
        {
            if (_isNewData)
                _paymentLoan = new PaymentLoan();

            _paymentLoan.loan_id = this._kasbon.loan_id;
            _paymentLoan.loan = this._kasbon;

            _paymentLoan.user_id = this._user.user_id;
            _paymentLoan.User = this._user;

            _paymentLoan.invoice = txtInvoice.Text;
            _paymentLoan.date = dtpDate.Value;
            _paymentLoan.amount = NumberHelper.StringToDouble(txtJumlah.Text);
            _paymentLoan.remaining_kasbon = NumberHelper.StringToDouble(txtRemainingLoan.Text);
            _paymentLoan.description = txtKeterangan.Text;            

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_paymentLoan, ref validationError);
                else
                    result = _bll.Update(_paymentLoan, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _paymentLoan);
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
