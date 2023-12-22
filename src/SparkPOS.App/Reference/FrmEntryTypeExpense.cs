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
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Reference
{
    public partial class FrmEntryTypeExpense : FrmEntryStandard
    {
        private ITypeExpenseBll _bll = null; // deklarasi objek business logic layer 
        private TypeExpense _typeExpense = null;
        private bool _isNewData = false;

        public IListener Listener { private get; set; }

        public FrmEntryTypeExpense(string header, ITypeExpenseBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;

            this._isNewData = true;
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryTypeExpense(string header, TypeExpense typeExpense, ITypeExpenseBll bll)
            : base()
        {
             InitializeComponent();  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._typeExpense = typeExpense;

            txtTypeExpense.Text = this._typeExpense.name_expense_type;
        }

        protected override void Save()
        {
            if (_isNewData)
                _typeExpense = new TypeExpense();

            _typeExpense.name_expense_type = txtTypeExpense.Text;

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_typeExpense, ref validationError);
                else
                    result = _bll.Update(_typeExpense, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _typeExpense);

                    if (_isNewData)
                    {
                        base.ResetForm(this);
                        txtTypeExpense.Focus();

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

        private void txtTypeExpense_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }
    }
}
