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
using SparkPOS.Helper.RAWPrinting;
using log4net;

namespace SparkPOS.App.Cashier.Transactions
{
    public partial class FrmDeleteItemTransactions : FrmEntryStandard
    {
        private SellingProduct _jual = null;

        public IListener Listener { private get; set; }

        public FrmDeleteItemTransactions(string header, SellingProduct sale)
            : base()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._jual = sale;
            MainProgram.GlobalLanguageChange(this);
        }

        protected override void Save()
        {
            var noTransactions = NumberHelper.StringToNumber(txtNomorTransactions.Text);

            if (!(noTransactions > 0))
            {
                MsgHelper.MsgWarning("'Number Transactions' should not be empty !");
                txtNomorTransactions.Focus();
                txtNomorTransactions.SelectAll();
                return;
            }                

            if (noTransactions > _jual.item_jual.Count)
            {
                MsgHelper.MsgWarning("Key-NumberTransaction", _jual.item_jual.Count.ToString());
                txtNomorTransactions.Focus();
                txtNomorTransactions.SelectAll();
                return;
            }

            if (!MsgHelper.MsgConfirmation("Apakah proses penyimpanan ingin di lanjutkan ?"))
                return;

            Listener.Ok(this, new { noTransactions });
            this.Close();
        }

        private void txtNomorTransactions_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }
    }
}
