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
    public partial class FrmEntryCustomer : FrmEntryStandard
    {
        private ICustomerBll _bll = null; // deklarasi objek business logic layer 
        private Customer _customer = null;
        private bool _isNewData = false;

        private IList<Provinsi> _listOfProvinsi;
        private IList<Regency> _listOfRegency;
        private IList<subdistrict> _listOfsubdistrict;
        private IList<Area> _listOfArea;

        public IListener Listener { private get; set; }

        public FrmEntryCustomer(string header, ICustomerBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this._listOfArea = MainProgram.ListOfArea;

            this._isNewData = true;
            LoadProvinsi();
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryCustomer(string header, Customer customer, ICustomerBll bll)
            : base()
        {
             InitializeComponent();
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._listOfArea = MainProgram.ListOfArea;
            this._customer = customer;

            LoadProvinsi();

            txtCustomer.Text = this._customer.name_customer;

            if (this._customer.Provinsi != null)
                cmbProvinsi.SelectedItem = this._customer.Provinsi.name_province;

            if (this._customer.Regency != null)
                cmbRegency.SelectedItem = this._customer.Regency.name_regency;

            if (this._customer.subdistrict != null)
                cmbsubdistrict.SelectedItem = this._customer.subdistrict.name_subdistrict;

            txtAddress.Text = this._customer.address;
            txtCodePos.Text = this._customer.postal_code;
            txtContact.Text = this._customer.contact;
            txtCRNo.Text = this._customer.cr_no;
            txtvatno.Text = this._customer.vat_no;
            txtphone.Text = this._customer.phone;
            txtDiskon.Text = this._customer.discount.ToString();
            txtLimitCredit.Text = this._customer.credit_limit.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadProvinsi()
        {
            _listOfProvinsi = _listOfArea.GroupBy(g => new { g.province_id, g.name_province })
                                            .Select(f => new Provinsi { province_id = f.FirstOrDefault().province_id, name_province = f.FirstOrDefault().name_province })
                                            .OrderBy(f => f.name_province)
                                            .ToList();

            cmbProvinsi.Items.Clear();
            cmbProvinsi.Items.Add("Select");

            foreach (var provinsi in _listOfProvinsi)
            {
                cmbProvinsi.Items.Add(provinsi.name_province);
            }

            cmbProvinsi.SelectedIndex = 0;
        }

        private void LoadRegency(string provinsiId)
        {
            _listOfRegency = _listOfArea.Where(f => f.province_id == provinsiId)
                                             .GroupBy(g => new { g.regency_id, g.name_regency })
                                             .Select(f => new Regency { regency_id = f.FirstOrDefault().regency_id, name_regency = f.FirstOrDefault().name_regency })
                                             .OrderBy(f => f.name_regency)
                                             .ToList();

            cmbRegency.Items.Clear();
            cmbRegency.Items.Add("Select");

            foreach (var regency in _listOfRegency)
            {
                cmbRegency.Items.Add(regency.name_regency);
            }

            cmbRegency.SelectedIndex = 0;
        }

        private void Loadsubdistrict(string regencyId)
        {            
            _listOfsubdistrict = _listOfArea.Where(f => f.regency_id == regencyId)
                                             .GroupBy(g => new { g.subdistrict_id, g.name_subdistrict })
                                             .Select(f => new subdistrict { subdistrict_id = f.FirstOrDefault().subdistrict_id, name_subdistrict = f.FirstOrDefault().name_subdistrict })
                                             .OrderBy(f => f.name_subdistrict)
                                             .ToList();            
            
            cmbsubdistrict.Items.Clear();
            cmbsubdistrict.Items.Add("Select");

            foreach (var regency in _listOfsubdistrict)
            {
                cmbsubdistrict.Items.Add(regency.name_subdistrict);
            }

            cmbsubdistrict.SelectedIndex = 0;
        }

        protected override void Save()
        {
            if (_isNewData)
                _customer = new Customer();

            _customer.name_customer = txtCustomer.Text;

            _customer.province_id = null;
            _customer.Provinsi = null;

            if (cmbProvinsi.SelectedIndex > 0)
            {
                var provinsi = _listOfProvinsi[cmbProvinsi.SelectedIndex - 1];

                _customer.province_id = provinsi.province_id;
                _customer.Provinsi = provinsi;
            }

            _customer.regency_id = null;
            _customer.Regency = null;

            if (cmbRegency.SelectedIndex > 0)
            {
                var regency = _listOfRegency[cmbRegency.SelectedIndex - 1];

                _customer.regency_id = regency.regency_id;
                _customer.Regency = regency;
            }

            _customer.subdistrict_id = null;
            _customer.subdistrict = null;

            if (cmbsubdistrict.SelectedIndex > 0)
            {
                var subdistrict = _listOfsubdistrict[cmbsubdistrict.SelectedIndex - 1];

                _customer.subdistrict_id = subdistrict.subdistrict_id;
                _customer.subdistrict = subdistrict;
            }

            _customer.address = txtAddress.Text;
            _customer.country = string.Empty;
            _customer.village = string.Empty;
            _customer.city = string.Empty;
            _customer.cr_no = txtCRNo.Text;
            _customer.vat_no = txtvatno.Text;
            _customer.postal_code = txtCodePos.Text;
            _customer.contact = txtContact.Text;
            _customer.phone = txtphone.Text;
            _customer.discount = NumberHelper.StringToDouble(txtDiskon.Text, true);
            _customer.credit_limit = NumberHelper.StringToDouble(txtLimitCredit.Text);

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_customer, ref validationError);
                else
                    result = _bll.Update(_customer, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _customer);

                    if (_isNewData)
                    {
                        base.ResetForm(this);
                        txtCustomer.Focus();
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

        private void txtLimitCredit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void cmbProvinsi_SelectedIndexChanged(object sender, EventArgs e)
        {
            var provinsiId = ((ComboBox)sender).SelectedIndex == 0 ? "0" : _listOfProvinsi[((ComboBox)sender).SelectedIndex - 1].province_id;
            LoadRegency(provinsiId);
        }

        private void cmbRegency_SelectedIndexChanged(object sender, EventArgs e)
        {
            var regencyId = ((ComboBox)sender).SelectedIndex == 0 ? "0" : _listOfRegency[((ComboBox)sender).SelectedIndex - 1].regency_id;
            Loadsubdistrict(regencyId);
        }

        private void cmbsubdistrict_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
