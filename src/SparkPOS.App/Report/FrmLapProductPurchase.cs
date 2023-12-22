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

using log4net;
using SparkPOS.Model;
using SparkPOS.Model.Report;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Helper;
using SparkPOS.Report;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;
using ConceptCave.WaitCursor;
using Microsoft.Reporting.WinForms;
using SparkPOS.Helper.UI.Template;

namespace SparkPOS.App.Report
{
    public partial class FrmLapProductPurchase : FrmSettingReportStandard
    {
        private IList<Supplier> _listOfSupplier = new List<Supplier>();
        private ILog _log;

        public FrmLapProductPurchase(string header)
        {
             InitializeComponent();  
            base.SetHeader(header);
            base.SetCheckBoxTitle("Select Supplier");
            base.SetToolTip("Find Supplier ...");
            base.ReSize(120);

            _log = MainProgram.log;
            
            chkShowInvoice.Visible = false;

            chkShowDetailsInvoice.Visible = true;
            chkShowDetailsInvoice.Enabled = true;
            chkShowDetailsInvoice.Text = "Show details Purchase";

            LoadSupplier();
            LoadMonthDanYear();
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadSupplier()
        {
            ISupplierBll bll = new SupplierBll(_log);
            _listOfSupplier = bll.GetAll();

            FillDataHelper.FillSupplier(chkListBox, _listOfSupplier);
        }

        private void LoadSupplier(string name)
        {
            ISupplierBll bll = new SupplierBll(_log);
            _listOfSupplier = bll.GetByName(name);

            FillDataHelper.FillSupplier(chkListBox, _listOfSupplier);
        }

        private void LoadMonthDanYear()
        {
            FillDataHelper.FillMonth(cmbMonth, true);
            FillDataHelper.FillYear(cmbYear, true);
        }

        protected override void Find()
        {
            LoadSupplier(txtKeyword.Text);
        }

        protected override void Preview()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (chkShowDetailsInvoice.Checked)
                {
                    PreviewReportDetail();
                }
                else
                {
                    PreviewReportHeader();
                }
            }
        }

        private void PreviewReportHeader()
        {
            var periode = string.Empty;

            IReportPurchaseProductBll reportBll = new ReportPurchaseProductBll(_log);
            
            IList<ReportProductPurchaseHeader> listOfReportPurchase = new List<ReportProductPurchaseHeader>();
            IList<string> listOfSupplierId = new List<string>();

            if (chkBoxTitle.Checked)
            {
                listOfSupplierId = base.GetSupplierId(_listOfSupplier);

                if (listOfSupplierId.Count == 0)
                {
                    MsgHelper.MsgWarning("Minimum 1 supplier must selected");
                    return;
                }
            }

            if (rdoDate.Checked)
            {
                if (!DateTimeHelper.IsValidRangeDate(dtpDateMulai.Value, dtpDateSelesai.Value))
                {
                    MsgHelper.MsgNotValidRangeDate();
                    return;
                }

                var tanggalMulai = DateTimeHelper.DateToString(dtpDateMulai.Value);
                var tanggalSelesai = DateTimeHelper.DateToString(dtpDateSelesai.Value);

                periode = dtpDateMulai.Value == dtpDateSelesai.Value ? string.Format("Periode : {0}", tanggalMulai) : string.Format("Periode : {0} s.d {1}", tanggalMulai, tanggalSelesai);

                listOfReportPurchase = reportBll.GetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportPurchase = reportBll.GetByMonth(month, year);
            }

            if (listOfSupplierId.Count > 0 && listOfReportPurchase.Count > 0)
            {
                listOfReportPurchase = listOfReportPurchase.Where(f => listOfSupplierId.Contains(f.supplier_id))
                                                             .ToList();
            }

            if (listOfReportPurchase.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportProductPurchaseHeader",
                    Value = listOfReportPurchase
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvProductPurchaseHeader", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry, Purchase data report was not found");
            }
        }

        private void PreviewReportDetail()
        {
            var periode = string.Empty;

            IReportPurchaseProductBll reportBll = new ReportPurchaseProductBll(_log);

            IList<ReportProductPurchaseDetail> listOfReportPurchase = new List<ReportProductPurchaseDetail>();

            IList<string> listOfSupplierId = new List<string>();

            if (chkBoxTitle.Checked)
            {
                listOfSupplierId = base.GetSupplierId(_listOfSupplier);

                if (listOfSupplierId.Count == 0)
                {
                    MsgHelper.MsgWarning("Minimum 1 supplier must selected");
                    return;
                }
            }

            if (rdoDate.Checked)
            {
                if (!DateTimeHelper.IsValidRangeDate(dtpDateMulai.Value, dtpDateSelesai.Value))
                {
                    MsgHelper.MsgNotValidRangeDate();
                    return;
                }

                var tanggalMulai = DateTimeHelper.DateToString(dtpDateMulai.Value);
                var tanggalSelesai = DateTimeHelper.DateToString(dtpDateSelesai.Value);

                periode = dtpDateMulai.Value == dtpDateSelesai.Value ? string.Format("Periode : {0}", tanggalMulai) : string.Format("Periode : {0} s.d {1}", tanggalMulai, tanggalSelesai);

                listOfReportPurchase = reportBll.DetailGetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportPurchase = reportBll.DetailGetByMonth(month, year);
            }

            if (listOfSupplierId.Count > 0 && listOfReportPurchase.Count > 0)
            {
                listOfReportPurchase = listOfReportPurchase.Where(f => listOfSupplierId.Contains(f.supplier_id))
                                                             .ToList();
            }

            if (listOfReportPurchase.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportProductPurchaseDetail",
                    Value = listOfReportPurchase
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvProductPurchaseDetail", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry, Purchase data report was not found");
            }
        }

        private void FrmLapProductPurchase_Load(object sender, EventArgs e)
        {

        }
    }
}
