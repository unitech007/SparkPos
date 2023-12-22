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
using Microsoft.Reporting.WinForms;
using SparkPOS.Model;
using SparkPOS.Helper.UserControl;
using System.Resources;
using System.Reflection;

namespace SparkPOS.Helper.UI.Template
{
    public partial class FrmSettingReportStandard : Form
    {
        private string _toolTip;

        public FrmSettingReportStandard()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            dtpDateMulai.Value = DateTime.Today;
            dtpDateSelesai.Value = DateTime.Today;  
        }

        private static ResourceManager GetResourceManager(string lanaguageToLoad)
        {
            ResourceManager rm = new ResourceManager("MultilingualApp." + lanaguageToLoad,
                    Assembly.GetExecutingAssembly());
            return rm;
        }

        protected void SetHeader(string header)
        {
           // ResourceManager rm = GetResourceManager(MainProgram.currentLanguage);
            string localizedHeaderText = MultilingualHelper.LoadAndCallHeaderHelper(header,MainProgram.currentLanguage);

            this.Text = localizedHeaderText;
            this.lblHeader.Text = localizedHeaderText;
        }


        protected void SetCheckBoxTitle(string title)
        {
            string localizedHeaderText = MultilingualHelper.LoadAndCallHeaderHelper(title, MainProgram.currentLanguage);
            this.chkBoxTitle.Text = localizedHeaderText;
        }

        protected void SetToolTip(string toolTip)
        {
            string localizedHeaderText = MultilingualHelper.LoadAndCallHeaderHelper(toolTip, MainProgram.currentLanguage);

            this._toolTip = localizedHeaderText;
            txtKeyword.Text = this._toolTip;
        }

        /// <summary>
        /// Method protected untuk mengeset ulang size form
        /// </summary>
        /// <param name="newSize">size form standar dilessi dengan nilai newSize</param>
        protected void ReSize(int newSize)
        {
            this.Size = new Size(this.Width - newSize, this.Height);
        }

        protected void ShowReport(string header, string reportName, ReportDataSource reportDataSource, IEnumerable<ReportParameter> parameters = null)
        {
            var frmPreview = new FrmPreviewReport(header, reportName, reportDataSource, parameters);
            frmPreview.ShowDialog();
        }

        protected void ShowReport(string header, string reportName, IList<ReportDataSource> reportDataSources, IEnumerable<ReportParameter> parameters = null)
        {
            var frmPreview = new FrmPreviewReport(header, reportName, reportDataSources, parameters);
            frmPreview.ShowDialog();
        }

        protected IList<string> GetSupplierId(IList<Supplier> listOfSupplier)
        {
            var listOfSupplierId = new List<string>();

            for (var i = 0; i < chkListBox.Items.Count; i++)
            {
                if (chkListBox.GetItemChecked(i))
                {
                    var supplier = listOfSupplier[i];
                    listOfSupplierId.Add(supplier.supplier_id);
                }
            }

            return listOfSupplierId;
        }

        protected IList<string> GetCustomerId(IList<Customer> listOfCustomer)
        {
            var listOfCustomerId = new List<string>();

            for (var i = 0; i < chkListBox.Items.Count; i++)
            {
                if (chkListBox.GetItemChecked(i))
                {
                    var customer = listOfCustomer[i];
                    listOfCustomerId.Add(customer.customer_id);
                }
            }

            return listOfCustomerId;
        }

        protected IList<string> GetCategoryId(IList<Category> listOfCategory)
        {
            var listOfCategoryId = new List<string>();

            for (var i = 0; i < chkListBox.Items.Count; i++)
            {
                if (chkListBox.GetItemChecked(i))
                {
                    var category = listOfCategory[i];
                    listOfCategoryId.Add(category.category_id);
                }
            }

            return listOfCategoryId;
        }

        /// <summary>
        /// Method override untuk menghandle proses preview
        /// </summary>
        protected virtual void Preview()
        {
        }

        protected virtual void SelectCheckBoxShowInvoice()
        {
        }

        protected virtual void Find()
        {
        }

        protected virtual void SelectAll()
        {
            for (int i = 0; i < chkListBox.Items.Count; i++)
            {
                chkListBox.SetItemChecked(i, chkSelectAll.Checked);
            }
        }

        protected virtual void SelectCheckBoxTitle()
        {
            txtKeyword.Enabled = chkBoxTitle.Checked;
            chkListBox.Enabled = chkBoxTitle.Checked;
            chkSelectAll.Enabled = chkBoxTitle.Checked;

            if (!chkBoxTitle.Checked)
            {
                for (int i = 0; i < chkListBox.Items.Count; i++)
                {
                    chkListBox.SetItemChecked(i, false);
                }

                chkSelectAll.Checked = false;
                txtKeyword.Text = _toolTip;
            }
        }

        /// <summary>
        /// Method override untuk menghandle proses selesai
        /// </summary>
        protected virtual void Cancel()
        {
            this.Close();
        }                

        private void chkBoxTitle_CheckedChanged(object sender, EventArgs e)
        {
            SelectCheckBoxTitle();
        }

        private void chkShowInvoice_CheckedChanged(object sender, EventArgs e)
        {
            SelectCheckBoxShowInvoice();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Preview();
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            Cancel();
        }        

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void FrmSettingReportStandard_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsShortcutKey(Keys.F10, e))
            {
                Preview();

                e.SuppressKeyPress = true;
            }                
        }

        private void FrmSettingReportStandard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                Cancel();
        }

        private void txtKeyword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Find();
        }

        private void txtKeyword_Leave(object sender, EventArgs e)
        {
            var txt = (AdvancedTextbox)sender;

            if (txt.Text.Length == 0)
                txt.Text = _toolTip;
        }

        private void txtKeyword_Enter(object sender, EventArgs e)
        {
            ((AdvancedTextbox)sender).Clear();
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            var txt = (AdvancedTextbox)sender;

            if (txt.Text.Length == 0)
                Find();
        }
    }
}
