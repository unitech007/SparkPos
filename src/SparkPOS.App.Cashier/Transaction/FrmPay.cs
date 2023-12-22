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
    public partial class FrmPay : FrmEntryStandard
    {
        private ISellingProductBll _bll = null; // deklarasi objek business logic layer 
        private SellingProduct _jual = null;
        private IList<Card> _listOfCard;

        public IListener Listener { private get; set; }

        public FrmPay(string header, SellingProduct sale, ISellingProductBll bll)
            : base()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._jual = sale;
            this._listOfCard = MainProgram.listOfCard;

            AddHandler();
            LoadCard();

            txtPPN.Text = MainProgram.GeneralSupplier.default_ppn.ToString();
            txtTotal.Text = this._jual.total_invoice.ToString();
            txtGrandTotal.Text = this._jual.grand_total.ToString();

            this.ActiveControl = txtPayCash;
            MainProgram.GlobalLanguageChange(this);
        }

        private void AddHandler()
        {
            txtDiskon.TextChanged += HitungGrandTotal;
            txtDiskon.TextChanged += HitungRefund;

            txtPPN.TextChanged += HitungGrandTotal;
            txtPPN.TextChanged += HitungRefund;

            txtPayCash.TextChanged += HitungRefund;
            txtPayCard.TextChanged += HitungRefund;
        }

        private void LoadCard()
        {
            cmbCard.Items.Clear();
            foreach (var card in _listOfCard)
            {
                cmbCard.Items.Add(card.card_name);
            }

            if (_listOfCard.Count > 0)
                cmbCard.SelectedIndex = 0;
        }

        private void HitungGrandTotal(object sender, EventArgs e)
        {
            _jual.discount = NumberHelper.StringToNumber(txtDiskon.Text);
            _jual.tax = NumberHelper.StringToFloat(txtPPN.Text);

            txtGrandTotal.Text = NumberHelper.NumberToString(_jual.grand_total);
        }

        private void HitungRefund(object sender, EventArgs e)
        {            
            var payCash = NumberHelper.StringToNumber(txtPayCash.Text);
            var payCard = NumberHelper.StringToNumber(txtPayCard.Text);

            var refund = (payCash + payCard) - _jual.grand_total;

            txtRefund.Text = "0";            
            if (refund > 0)
            {
                txtRefund.Text = refund.ToString();
            }                
        }

        private void UpdateDefaultPPN(double tax)
        {
            var appConfigFile = string.Format("{0}\\SparkPOSCashier.exe.config", Utils.GetAppPath());
            MainProgram.GeneralSupplier.default_ppn = tax;

            AppConfigHelper.SaveValue("defaultPPN", tax.ToString(), appConfigFile);
        }

        protected override void Save()
        {
            var msg = "'{0}' should not empty !";
            var payCash = 0;
            var payCard = 0;

            if (chkPayViaCard.Checked) // payment via card
            {
                payCard = (int)NumberHelper.StringToNumber(txtPayCard.Text);
                if (!(payCard > 0))
                {
                    MsgHelper.MsgWarning(msg, "Pay via Card");
                    txtPayCard.Focus();
                    return;
                }
            }

            // payment cash
            payCash = (int)NumberHelper.StringToNumber(txtPayCash.Text);

            if (payCash == 0 && payCard == 0)
            {
                MsgHelper.MsgWarning(msg, "Pay Cash");
                txtPayCash.Focus();
                return;
            }

            _jual.payment_cash = payCash;
            _jual.payment_card = payCard;            

            if ((_jual.jumlah_pay - _jual.grand_total) < 0)
            {
                MsgHelper.MsgWarning("Sorry quantity pay less");

                if (payCash > 0)
                {
                    txtPayCash.Focus();
                    txtPayCash.SelectAll();
                    return;
                }

                if (payCard > 0)
                {
                    txtPayCard.Focus();
                    txtPayCard.SelectAll();
                    return;
                }
            }

            if (!MsgHelper.MsgConfirmation("Apakah proses penyimpanan ingin di lanjutkan ?"))
                return;

            _jual.machine_id= MainProgram.mesinId;
            _jual.tax = NumberHelper.StringToDouble(txtPPN.Text);
            _jual.discount = NumberHelper.StringToDouble(txtDiskon.Text);

            if (_jual.payment_card > 0)
            {
                var card = _listOfCard[cmbCard.SelectedIndex];

                _jual.card_id = card.card_id;
                _jual.Card = card;
                _jual.card_number = txtNoCard.Text;
            }

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                result = _bll.Save(_jual, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _jual);

                    UpdateDefaultPPN(_jual.tax);
                    this.Close();
                }
                else
                {
                    if (validationError.Message != null && validationError.Message.Length > 0)
                    {
                        MsgHelper.MsgWarning(validationError.Message);
                        base.SetFocusObject(validationError.PropertyName, this);
                    }
                    else
                        MsgHelper.MsgUpdateError();
                }
            }
        }

        private void txtPayCash_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                KeyPressHelper.NextFocus();
                KeyPressHelper.NextFocus();
            }                
        }

        private void chkPayViaCard_CheckedChanged(object sender, EventArgs e)
        {
            var isChecked = ((CheckBox)sender).Checked;

            cmbCard.Enabled = isChecked;
            txtPayCard.Enabled = isChecked;
            txtNoCard.Enabled = isChecked;

            if (!isChecked)
                txtPayCard.Text = "0";
        }        
    }
}
