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

using SparkPOS.Helper;
using SparkPOS.Helper.UserControl;
using System.Configuration;
using System.Collections.Specialized;

namespace SparkPOS.Helper.UI.Template
{
    /// <summary>
    /// Base class form untuk entry data
    /// </summary>
    public partial class FrmEntryStandard : Form
    {
        public FrmEntryStandard()
        {
             InitializeComponent();  ////MainProgram.GlobalLanguageChange(this);            
        }        

        #region protected dan override method

        /// <summary>
        /// Method protected untuk mengeset header form entri
        /// </summary>
        /// <param name="header"></param>
        protected void SetHeader(string header)
        {
            string localizedHeaderText = MultilingualHelper.LoadAndCallHeaderHelper(header, MainProgram.currentLanguage);
            this.Text = localizedHeaderText;
            this.lblHeader.Text = localizedHeaderText;
        }

        /// <summary>
        /// Method protected untuk mengeset tulisan tombol selesai
        /// </summary>
        /// <param name="title"></param>
        protected void SetButtonSelesaiToClose()
        {
            btnSelesai.Text = "Esc Close";
        }

        /// <summary>
        /// Method protected untuk mengeset tulisan tombol save
        /// </summary>
        protected void SetButtonSaveToProses()
        {
            btnSave.Text = "Proses";
        }

        /// <summary>
        /// Method protected untuk deactivate tombol save
        /// </summary>
        protected void SetButtonSaveToFalse(bool isEnabled = true)
        {
            btnSave.Enabled = !isEnabled;
        }

        /// <summary>
        /// Method override untuk menghandle proses save
        /// </summary>
        protected virtual void Save()
        {
        }

        /// <summary>
        /// Method override untuk menghandle proses selesai
        /// </summary>
        protected virtual void Cancel()
        {
            this.Close();
        }

        /// <summary>
        /// Method untuk mengeset fokus objek yang nilainya null atau empty
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="parent"></param>
        protected void SetFocusObject(string propertyName, Control parent)
        {
            if (propertyName == null || propertyName.Length == 0)
                return;

            foreach (Control ctl in parent.Controls)
            {
                if (ctl.Tag != null && ctl.Tag.ToString().ToLower() == propertyName.ToLower())
                {
                    var ctlType = ctl.GetType().Name;

                    switch (ctlType)
                    {
                        case "AdvancedTextbox":
                            ((AdvancedTextbox)ctl).Focus();
                            ((AdvancedTextbox)ctl).SelectAll();
                            break;
                        
                        case "ComboBox":
                            ((ComboBox)ctl).Focus();
                            break;

                        case "DateTimePicker":
                            ((DateTimePicker)ctl).Focus();
                            break;

                        case "MaskedTextBox":
                            ((MaskedTextBox)ctl).Focus();
                            ((MaskedTextBox)ctl).SelectAll();
                            break;

                        case "TextBox":
                            ((TextBox)ctl).Focus();
                            ((TextBox)ctl).SelectAll();
                            break;

                        default:
                            break;
                    }
                }

                SetFocusObject(propertyName, ctl);
            }
        }

        /// <summary>
        /// Method untuk mengosongkan inputan form
        /// </summary>
        /// <param name="parent"></param>
        protected void ResetForm(Control parent)
        {
            foreach (Control ctl in parent.Controls)
            {

                var ctlType = ctl.GetType().Name;

                switch (ctlType)
                {
                    case "PictureBox":
                        ((PictureBox)ctl).Image = null;
                        break;

                    case "AdvancedTextbox":
                        var objAdvancedTextbox = (AdvancedTextbox)ctl;

                        if (objAdvancedTextbox.NumericOnly)
                            objAdvancedTextbox.Text = "0";
                        else
                        {
                            if (objAdvancedTextbox.Tag == null || objAdvancedTextbox.Tag.ToString().Equals("") || objAdvancedTextbox.Tag.ToString() != "ignore")
                                objAdvancedTextbox.Clear();
                        }

                        break;

                    case "ListBox":
                        var objListBox = (ListBox)ctl;

                        if (objListBox.Tag == null || objListBox.Tag.ToString().Equals("") || objListBox.Tag.ToString() != "ignore")
                            objListBox.Items.Clear();

                        break;

                    case "ComboBox":
                        var objComboBox = (ComboBox)ctl;

                        if (objComboBox.Tag == null || objComboBox.Tag.ToString().Equals("") || objComboBox.Tag.ToString() != "ignore")
                            objComboBox.Items.Clear();

                        break;

                    case "DateTimePicker":
                        ((DateTimePicker)ctl).Value = DateTime.Today;
                        break;

                    case "MaskedTextBox":
                        ((MaskedTextBox)ctl).Clear();
                        break;

                    default:
                        break;
                }

                ResetForm(ctl);
            }
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void FrmEntryStandard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                Cancel();
        }

        private void FrmEntryStandard_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F11:
                    if (btnSave.Enabled)
                        Save();

                    break;

                default:
                    break;
            }
        }
    }
}
