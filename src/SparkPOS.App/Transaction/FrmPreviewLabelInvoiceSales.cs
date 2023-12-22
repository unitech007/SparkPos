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

using Microsoft.Reporting.WinForms;
using System.Reflection;
using System.IO;

using log4net;
using SparkPOS.Helper;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using ConceptCave.WaitCursor;
using MultilingualApp;

namespace SparkPOS.App.Transactions
{
    public partial class FrmPreviewLabelInvoiceSales : Form
    {
        private string _reportNameSpace = @"SparkPOS.Report.{0}.rdlc";
        private Assembly _assemblyReport;

        private ILog _log;
        private Customer _customer = null;
        private Dropshipper _dropshipper = null;
        private SellingProduct _jual = null;        
        private User _user;
        private Profil _profil;
        private GeneralSupplier _GeneralSupplier;

        public FrmPreviewLabelInvoiceSales()
        {
             InitializeComponent();  
           // this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.ZoomMode = ZoomMode.Percent;
            this.reportViewer1.ZoomPercent = 100;

            ColorManagerHelper.SetTheme(this, this);
            _assemblyReport = Assembly.LoadFrom("SparkPOS.Report.dll");
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmPreviewLabelInvoiceSales(string header, SellingProduct sale)
            : this()
        {
            //this.Text = header;
            //this.lblHeader.Text = header;
            this._log = MainProgram.log;
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;
            this._jual = sale;
            this._customer = this._jual.Customer;
            this._dropshipper = this._jual.Dropshipper;

            SetLabelInvoice();
            btnPreviewInvoice_Click(btnPreviewInvoice, new EventArgs());
            MainProgram.GlobalLanguageChange(this);
            this.Text = LanguageHelper.TranslateText(header, MainProgram.currentLanguage);
            this.lblHeader.Text = LanguageHelper.TranslateText(header, MainProgram.currentLanguage);

        }

        private void SetLabelInvoice()
        {
            var dari1 = this._GeneralSupplier.list_of_label_nota[0].description;
            var dari2 = this._GeneralSupplier.list_of_label_nota[1].description;
            var dari3 = this._GeneralSupplier.list_of_label_nota[2].description;

            dari1 = string.IsNullOrEmpty(this._jual.from_label1) ? dari1 : this._jual.from_label1;
            dari2 = string.IsNullOrEmpty(this._jual.from_label2) ? dari2 : this._jual.from_label2;
            dari3 = string.IsNullOrEmpty(this._jual.from_label3) ? dari3 : this._jual.from_label3;

            // info address shipping based data customer
            var to1 = _customer.name_customer;
            var to2 = _customer.address;
            var to3 = _customer.get_region_lengkap;
            var to4 = string.Format("HP: {0}", _customer.phone.NullToString());

            // info address shipping based data address yang diedit pada saat sales
            to1 = string.IsNullOrEmpty(this._jual.shipping_to) ? to1 : this._jual.shipping_to;
            to2 = string.IsNullOrEmpty(this._jual.shipping_address) ? to2 : this._jual.shipping_address;
            to3 = string.IsNullOrEmpty(this._jual.shipping_subdistrict) ? to3 : this._jual.shipping_subdistrict;
            to4 = string.IsNullOrEmpty(this._jual.shipping_village) ? to4 : this._jual.shipping_village;

            // info address shipping yang diedit di label invoice
            to1 = string.IsNullOrEmpty(this._jual.to_label1) ? to1 : this._jual.to_label1;
            to2 = string.IsNullOrEmpty(this._jual.to_label2) ? to2 : this._jual.to_label2;
            to3 = string.IsNullOrEmpty(this._jual.to_label3) ? to3 : this._jual.to_label3;
            to4 = string.IsNullOrEmpty(this._jual.to_label4) ? to4 : this._jual.to_label4;

            txtFrom1.Text = dari1;
            txtFrom2.Text = dari2;
            txtFrom3.Text = dari3;

            if (this._jual.is_dropship && this._dropshipper != null)
            {
                txtFrom1.Text = this._dropshipper.name_dropshipper.NullToString();
                txtFrom2.Text = this._dropshipper.address.NullToString();
                txtFrom3.Text = string.Format("HP: {0}", this._dropshipper.phone.NullToString());
            }

            txtto1.Text = to1;
            txtto2.Text = to2;
            txtto3.Text = to3;
            txtto4.Text = to4;
        }

        private void btnPreviewInvoice_Click(object sender, EventArgs e)
        {
            _jual.from_label1 = txtFrom1.Text;
            _jual.from_label2 = txtFrom2.Text;
            _jual.from_label3 = txtFrom3.Text;

            _jual.to_label1 = txtto1.Text;
            _jual.to_label2 = txtto2.Text;
            _jual.to_label3 = txtto3.Text;
            _jual.to_label4 = txtto4.Text;
            
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                PreviewLabelInvoice(_jual);
            }
        }

        private void PreviewLabelInvoice(SellingProduct sale, bool isPreview = true)
        {
            IPrintInvoiceBll printBll = new PrintInvoiceBll(_log);
            var listOfItemInvoice = printBll.GetInvoiceSales(sale.sale_id);

            if (listOfItemInvoice.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "InvoiceSales",
                    Value = listOfItemInvoice
                };

                foreach (var item in listOfItemInvoice)
                {
                    item.from_label1 = txtFrom1.Text;
                    item.from_label2 = txtFrom2.Text;
                    item.from_label3 = txtFrom3.Text;

                    item.to_label1 = txtto1.Text;
                    item.to_label2 = txtto2.Text;
                    item.to_label3 = txtto3.Text;
                    item.to_label4 = txtto4.Text;

                    if (_GeneralSupplier.is_singkat_penulisan_ongkir && item.shipping_cost > 0)
                    {
                        item.shipping_cost /= 1000;
                        item.label_cost_shipping = item.shipping_cost.ToString();
                    }
                    else
                    {
                        item.label_cost_shipping = NumberHelper.NumberToString(item.shipping_cost);
                    }                        
                }

                var reportName = "RvLabelInvoiceSales";

                if (isPreview)
                {
                    reportName = string.Format(_reportNameSpace, reportName);
                    var stream = _assemblyReport.GetManifestResourceStream(reportName);
                   // stream = RdlcReportHelper.TranslateReport(stream, MainProgram.currentLanguage);

                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    this.reportViewer1.LocalReport.LoadReportDefinition(stream);

                    this.reportViewer1.RefreshReport();
                }
                else
                {
                    var printReport = new ReportViewerPrintHelper(reportName, reportDataSource, printerName: _GeneralSupplier.name_printer);
                    printReport.Print();
                }                
            }
        }

        private void btnPrintLabelInvoice_Click(object sender, EventArgs e)
        {
            if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    ISellingProductBll bll = new SellingProductBll(_log);
                    var result = bll.Update(_jual);

                    PreviewLabelInvoice(_jual, false);
                }
            }
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmPreviewLabelInvoiceSales_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                this.Close();
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
