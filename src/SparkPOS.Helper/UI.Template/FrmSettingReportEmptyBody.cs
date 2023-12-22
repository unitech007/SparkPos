﻿/**
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

namespace SparkPOS.Helper.UI.Template
{
    public partial class FrmSettingReportEmptyBody : Form
    {
        public FrmSettingReportEmptyBody()
        {
             InitializeComponent();  ////MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);
        }

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
        /// Method protected untuk mengeset ulang size form
        /// </summary>
        /// <param name="newSize"></param>
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

        /// <summary>
        /// Method override untuk menghandle proses preview
        /// </summary>
        protected virtual void Preview()
        {
        }

        /// <summary>
        /// Method override untuk menghandle proses selesai
        /// </summary>
        protected virtual void Cancel()
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Preview();
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void FrmSettingReportEmptyBody_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsShortcutKey(Keys.F10, e))
            {
                Preview();

                e.SuppressKeyPress = true;
            }                
        }

        private void FrmSettingReportEmptyBody_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                Cancel();
        }        
    }
}
