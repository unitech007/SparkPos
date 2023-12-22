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

namespace SparkPOS.App.Cashier.Settings
{
    public partial class FrmEntryCustomeCode : FrmEntryStandard
    {
        private GeneralSupplier _GeneralSupplier = null;
        private bool _isAutocutCode;

        public FrmEntryCustomeCode(string header, GeneralSupplier GeneralSupplier, bool isAutocutCode)
            : base()
        {
             InitializeComponent();  //
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._GeneralSupplier = GeneralSupplier;
            this._isAutocutCode = isAutocutCode;

            txtCustomeCode.Text = isAutocutCode ? this._GeneralSupplier.autocut_code : this._GeneralSupplier.open_cash_drawer_code;
            MainProgram.GlobalLanguageChange(this);
        }

        protected override void Save()
        {
            if (_isAutocutCode)
                _GeneralSupplier.autocut_code = txtCustomeCode.Text;
            else
                _GeneralSupplier.open_cash_drawer_code = txtCustomeCode.Text;

            this.Close();
        }
    }
}
