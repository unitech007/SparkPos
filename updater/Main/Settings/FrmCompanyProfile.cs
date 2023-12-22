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
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;

namespace SparkPOS.App.Settings
{
    public partial class FrmCompanyProfile : FrmEntryStandard
    {
        private Profil _profil = null;
        
        public IListener Listener { private get; set; }

        public FrmCompanyProfile(string header, Profil profil)
            : base()
        {
            InitializeComponent();
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._profil = profil;

            if (this._profil != null)
            {
                txtNameCompany.Text = this._profil.name_profile;
                txtAddress.Text = this._profil.address;
                txtCity.Text = this._profil.city;
                txtphone.Text = this._profil.phone;
                txtEmail.Text = this._profil.email;
                txtWebsite.Text = this._profil.website;
            }            
        }

        protected override void Save()
        {
            if (this._profil == null)
                this._profil = new Profil();

            _profil.name_profile = txtNameCompany.Text;
            _profil.address = txtAddress.Text;
            _profil.city = txtCity.Text;
            _profil.phone = txtphone.Text;
            _profil.email = txtEmail.Text;
            _profil.website = txtWebsite.Text;

            var result = 0;
            var validationError = new ValidationError();

            IProfilBll bll = new ProfilBll(MainProgram.log);
            result = bll.Save(_profil, ref validationError);

            if (result > 0) 
            {
                Listener.Ok(this, _profil);
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

        private void txtWebsite_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }        
    }
}
