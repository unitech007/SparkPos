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
    public partial class FrmEntryTax : FrmEntryStandard
    {
        private ITaxBll _bll = null; // deklarasi objek business logic layer 
        private Tax _golongan = null;
        private bool _isNewData = false;

        public IListener Listener { private get; set; }

        public FrmEntryTax(string header, ITaxBll bll)
            : base()
        {
             InitializeComponent();  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;

            this._isNewData = true;
        }

        public FrmEntryTax(string header, Tax category, ITaxBll bll)
            : base()
        {
             InitializeComponent();  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._golongan = category;

            txttaxName.Text = this._golongan.name_tax;
            txttax.Text = this._golongan.tax_percentage.ToString();
            txtDiscription.Text = this._golongan.description.ToString();

            chkBoxDefaultTax.Checked = this._golongan.is_default_tax;
        }

        protected override void Save()
        {
            if (_isNewData)

                _golongan = new Tax();

            _golongan.name_tax = txttaxName.Text;
            _golongan.tax_percentage = (int)NumberHelper.StringToDouble(txttax.Text, true);
            _golongan.description =txtDiscription.Text;
            _golongan.is_default_tax = chkBoxDefaultTax.Checked;

            //Guid currentTaxId;
            //if (Guid.TryParse(_golongan.tax_id, out currentTaxId))
            //{
            //    if (chkBoxDefaultTax.Checked)
            //    {
            //        _bll.ClearDefaultTax(currentTaxId); // Call the method to clear the default tax flag from other taxes in the database
            //    }
            //}
            //else
            //{
            //    // Handle the case where the tax_id is not a valid Guid
            //    MsgHelper.MsgWarning("Invalid tax_id");
            //}

            //if (chkBoxDefaultTax.Checked)
            //{
            //    string currentTaxId = _golongan.tax_id.ToString();
            //    _bll.ClearDefaultTax(currentTaxId); // Call the method to clear the default tax flag from other taxes in the database
            //}

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_golongan, ref validationError);
                else
                    result = _bll.Update(_golongan, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _golongan);

                    if (_isNewData)
                    {
                        base.ResetForm(this);
                        txttaxName.Focus();

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

        private void txtDiskon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void FrmEntryTax_Load(object sender, EventArgs e)
        {


        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtDiscription_TextChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void txttax_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDiscription_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txttaxName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
