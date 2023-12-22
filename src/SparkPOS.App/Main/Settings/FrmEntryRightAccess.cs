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

namespace SparkPOS.App.Settings
{
    public partial class FrmEntryRightAccess : FrmEntryStandard
    {        
        private IRoleBll _bll = null; // deklarasi objek business logic layer 
        private Role _role = null;

        private bool _isNewData = false;
        
        public IListener Listener { private get; set; }

        public FrmEntryRightAccess(string header, IRoleBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            
            this._bll = bll;

            this._isNewData = true;
            MainProgram.GlobalLanguageChange(this);
            base.SetHeader(header);
        }

        public FrmEntryRightAccess(string header, Role role, IRoleBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._role = role;

            txtNameRole.Text = this._role.name_role;
            if (this._role.is_active)
                rdoActive.Checked = true;
            else
                rdoNonActive.Checked = true;
            MainProgram.GlobalLanguageChange(this);
        }

        protected override void Save()
        {
            if (_isNewData)
                _role = new Role();

            _role.name_role = txtNameRole.Text;
            _role.is_active = rdoActive.Checked;

            var result = 0;
            var validationError = new ValidationError();

            if (_isNewData)
                result = _bll.Save(_role, ref validationError);
            else
                result = _bll.Update(_role, ref validationError);

            if (result > 0) 
            {
                Listener.Ok(this, _isNewData, _role);

                if (_isNewData)
                {
                    base.ResetForm(this);
                    txtNameRole.Focus();
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
                {
                    MsgHelper.MsgUpdateError();
                }                    
            }                
        }
    }
}
