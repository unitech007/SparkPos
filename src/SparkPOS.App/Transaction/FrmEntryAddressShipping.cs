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

using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Helper;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Model;
using SparkPOS.Model.Transaction;

namespace SparkPOS.App.Transactions
{
    public partial class FrmEntryAddressShipping : FrmEntryStandard
    {
        private AddressShipping _addressShipping = null;
        private Customer _customer = null;
        private SellingProduct _jual = null;
        private SellingQuotation _jual1 = null;

        public IListener Listener { private get; set; }

        public FrmEntryAddressShipping(string header, Customer customer, SellingProduct sale)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);
            base.SetHeader(header);

            this._addressShipping = new AddressShipping();
            this._customer = customer;
            this._jual = sale;

            if (this._jual == null)
                chkIsSdac.Checked = true;
            else
                chkIsSdac.Checked = this._jual.is_sdac;

            chkIsSdac_CheckedChanged(chkIsSdac, new EventArgs());
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryAddressShipping(string header, Customer customer, SellingQuotation sale)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);
            base.SetHeader(header);

            this._addressShipping = new AddressShipping();
            this._customer = customer;
            this._jual1 = sale;

            if (this._jual1 == null)
                chkIsSdac.Checked = true;
            else
                chkIsSdac.Checked = this._jual1.is_sdac;

            chkIsSdac_CheckedChanged(chkIsSdac, new EventArgs());
            MainProgram.GlobalLanguageChange(this);
        }
        protected override void Save()
        {
            this._addressShipping.is_sdac = chkIsSdac.Checked;
            this._addressShipping.to = txtto1.Text;
            this._addressShipping.address = txtto2.Text;
            this._addressShipping.subdistrict = txtto3.Text;
            this._addressShipping.village = txtto4.Text;

            Listener.Ok(this, this._addressShipping);
            this.Close();
        }

        private void chkIsSdac_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            pnlAddressShipping.Enabled = !chk.Checked;

            var to1 = _customer.name_customer;
            var to2 = _customer.address;
            var to3 = _customer.get_region_lengkap;
            var to4 = string.Format("HP: {0}", _customer.phone.NullToString());

            if (!chk.Checked) // address shipping tidak sama dengan address customer
            {
                if (_jual != null)
                {
                    to1 = string.IsNullOrEmpty(_jual.shipping_to) ? to1 : _jual.shipping_to;
                    to2 = string.IsNullOrEmpty(_jual.shipping_address) ? to2 : _jual.shipping_address;
                    to3 = string.IsNullOrEmpty(_jual.shipping_subdistrict) ? to3 : _jual.shipping_subdistrict;
                    to4 = string.IsNullOrEmpty(_jual.shipping_village) ? to4 : _jual.shipping_village;
                }
            }

            txtto1.Text = to1;
            txtto2.Text = to2;
            txtto3.Text = to3;
            txtto4.Text = to4;
        }
    }
}
