﻿/**
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
    public partial class FrmEntryCategory : FrmEntryStandard
    {
        private ICategoryBll _bll = null; // deklarasi objek business logic layer 
        private Category _golongan = null;
        private bool _isNewData = false;

        public IListener Listener { private get; set; }

        public FrmEntryCategory(string header, ICategoryBll bll)
            : base()
        {
             InitializeComponent();  
          
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;

            this._isNewData = true;
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryCategory(string header, Category category, ICategoryBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._golongan = category;

            txtCategory.Text = this._golongan.name_category;
            txtProfit.Text = this._golongan.profit_percentage.ToString();
            txtDiskon.Text = this._golongan.discount.ToString();
            MainProgram.GlobalLanguageChange(this);

        }

        protected override void Save()
        {
            if (_isNewData)
                _golongan = new Category();

            _golongan.name_category = txtCategory.Text;
            _golongan.profit_percentage = NumberHelper.StringToDouble(txtProfit.Text, true);
            _golongan.discount = NumberHelper.StringToDouble(txtDiskon.Text, true);

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
                        txtCategory.Focus();

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

        private void FrmEntryCategory_Load(object sender, EventArgs e)
        {


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
