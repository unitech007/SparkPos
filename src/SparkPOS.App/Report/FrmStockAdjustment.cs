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
    public partial class FrmStockAdjustment : FrmSettingReportStandard
    {
        private IList<ReasonAdjustmentStock> _listOfReasonAdjustmentStock = new List<ReasonAdjustmentStock>();
        private ILog _log;
        
        public FrmStockAdjustment(string header)
        {
             InitializeComponent(); 
        
            base.SetCheckBoxTitle("Select reasonStock Adjustment");
            base.ReSize(120);
            this.txtKeyword.Visible = false;

            _log = MainProgram.log;

            chkShowInvoice.Visible = false;

            LoadReasonAdjustmentStock();
            LoadMonthDanYear();
            MainProgram.GlobalLanguageChange(this);
            base.SetHeader(header);
        }

        private void LoadReasonAdjustmentStock()
        {
            IReasonAdjustmentStockBll bll = new ReasonAdjustmentStockBll(_log);
            _listOfReasonAdjustmentStock = bll.GetAll();

            FillDataHelper.FillReasonAdjustmentStock(chkListBox, _listOfReasonAdjustmentStock);
        }

        private void LoadMonthDanYear()
        {
            FillDataHelper.FillMonth(cmbMonth, true);
            FillDataHelper.FillYear(cmbYear, true);
        }        

        protected override void Preview()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                PreviewReport();
            }
        }

        private IList<string> GetReasonAdjustmentStockId(IList<ReasonAdjustmentStock> listOfReasonAdjustmentStock)
        {
            var listOfReasonAdjustmentStockId = new List<string>();

            for (var i = 0; i < chkListBox.Items.Count; i++)
            {
                if (chkListBox.GetItemChecked(i))
                {
                    var reason = listOfReasonAdjustmentStock[i];
                    listOfReasonAdjustmentStockId.Add(reason.stock_adjustment_reason_id);
                }
            }

            return listOfReasonAdjustmentStockId;
        }

        private void PreviewReport()
        {
            var periode = string.Empty;

            IReportStockProductBll reportBll = new ReportStockProductBll(_log);

            IList<ReportAdjustmentStockProduct> listOfReportAdjustmentStockProduct = new List<ReportAdjustmentStockProduct>();
            IList<string> listOfReasonAdjustmentStockId = new List<string>();

            if (chkBoxTitle.Checked)
            {
                listOfReasonAdjustmentStockId = GetReasonAdjustmentStockId(_listOfReasonAdjustmentStock);

                if (listOfReasonAdjustmentStockId.Count == 0)
                {
                    MsgHelper.MsgWarning("Minimum 1 reason must selected");
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

                listOfReportAdjustmentStockProduct = reportBll.GetAdjustmentStockByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportAdjustmentStockProduct = reportBll.GetAdjustmentStockByMonth(month, year);
            }

            if (listOfReasonAdjustmentStockId.Count > 0 && listOfReportAdjustmentStockProduct.Count > 0)
            {
                listOfReportAdjustmentStockProduct = listOfReportAdjustmentStockProduct.Where(f => listOfReasonAdjustmentStockId.Contains(f.stock_adjustment_reason_id))
                                                                   .ToList();
            }
            
            if (listOfReportAdjustmentStockProduct.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportAdjustmentStock",
                    Value = listOfReportAdjustmentStockProduct
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvAdjustmentStock", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report dataStock Adjustment not found");
            }
        }
    }
}
