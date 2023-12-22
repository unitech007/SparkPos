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
using SparkPOS.Report;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;
using ConceptCave.WaitCursor;
using Microsoft.Reporting.WinForms;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;

namespace SparkPOS.App.Report
{
    public partial class FrmLapBestSellingProducts : FrmSettingReportEmptyBody
    {
        private ILog _log;

        public FrmLapBestSellingProducts(string header)
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            _log = MainProgram.log;

            
            dtpDateMulai.Value = DateTime.Today;
            dtpDateSelesai.Value = DateTime.Today;

            LoadMonthDanYear();
            MainProgram.GlobalLanguageChange(this);
            base.SetHeader(header);

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

        private void PreviewReport()
        {
            var periode = string.Empty;

            IReportSellingProductBll reportBll = new ReportSellingProductBll(_log);
            IList<ReportProductFavourite> listOfReport;

            var limit = (int)updLimit.Value;

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

                listOfReport = reportBll.ProductFavouriteGetByDate(dtpDateMulai.Value, dtpDateSelesai.Value, limit);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReport = reportBll.ProductFavouriteGetByMonth(month, year, limit);
            }

            if (listOfReport.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "DsProductFavourite",
                    Value = listOfReport
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvProductFavourite", reportDataSource, parameters);
            }            
            else
            {
                MsgHelper.MsgInfo("Sorry report data sales product favorit not found");
            }
        }
    }
}
