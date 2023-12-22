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
using SparkPOS.Helper;
using SparkPOS.Helper.UI.Template;

namespace SparkPOS.App.Settings
{
    public partial class FrmEntryOperator : FrmEntryStandard
    {        
        private IUserBll _bll = null; // deklarasi objek business logic layer 
        private User _operator = null;
        private IList<Role> listOfRole;
        private bool _isNewData = false;

        public IListener Listener { private get; set; }

        public FrmEntryOperator(string header, IList<Role> listOfRole, IUserBll bll)
            : base()
        {
            InitializeComponent();
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this.listOfRole = listOfRole;

            this._isNewData = true;
            LoadRole();
        }

        public FrmEntryOperator(string header, User userOperator, IList<Role> listOfRole, IUserBll bll)
            : base()
        {
            InitializeComponent();
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._operator = userOperator;
            this.listOfRole = listOfRole;

            LoadRole();

            txtName.Text = this._operator.name_user;

            try
            {
                if (this._operator.Role != null)
                    cmbRole.SelectedItem = this._operator.Role.name_role;
            }
            catch
            {

                if (listOfRole.Count > 0)
                    cmbRole.SelectedIndex = 0;
            }

            if (this._operator.is_active)
                rdoActive.Checked = true;
            else
                rdoNonActive.Checked = true;
        }

        private void LoadRole()
        {
            cmbRole.Items.Clear();
            foreach (var role in listOfRole.Where(f => f.is_active == true))
            {
                cmbRole.Items.Add(role.name_role);
            }

            if (listOfRole.Count > 0)
                cmbRole.SelectedIndex = 0;
        }

        protected override void Save()
        {
            if (txtPassword.Text.Length > 0 && txtKonfirmasiPassword.Text.Length > 0)
            {
                if (txtPassword.Text != txtKonfirmasiPassword.Text)
                {
                    MsgHelper.MsgWarning("Password and confirmation password must be the same");
                    txtPassword.Focus();
                    txtPassword.SelectAll();
                    return;
                }
            }            

            if (_isNewData)
                _operator = new User();

            _operator.name_user = txtName.Text;
            _operator.user_password = txtPassword.Text;

            if (txtKonfirmasiPassword.Text.Length > 0)
                _operator.konf_user_password = CryptoHelper.GetMD5Hash(txtKonfirmasiPassword.Text, MainProgram.securityCode);

            var role = listOfRole[cmbRole.SelectedIndex];
            _operator.role_id = role.role_id;
            _operator.Role = role;
            _operator.is_active = rdoActive.Checked;

            var result = 0;
            var validationError = new ValidationError();

            if (_isNewData)
                result = _bll.Save(_operator, ref validationError);
            else
                result = _bll.Update(_operator);

            if (result > 0) 
            {
                Listener.Ok(this, _isNewData, _operator);

                if (_isNewData)
                {
                    base.ResetForm(this);
                    txtName.Focus();

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
}
