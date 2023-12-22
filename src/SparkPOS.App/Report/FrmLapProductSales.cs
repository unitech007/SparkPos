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
    public partial class FrmLapProductSales : FrmSettingReportStandard
    {
        private IList<Customer> _listOfCustomer = new List<Customer>();
        private ILog _log;

        public FrmLapProductSales(string header)
        {
             InitializeComponent();  
            
            base.SetCheckBoxTitle("Select Customer");
            base.SetToolTip("Find Customer ...");
            base.ReSize(120);

            _log = MainProgram.log;
            
            chkShowInvoice.Visible = false;

            chkShowDetailsInvoice.Visible = true;
            chkShowDetailsInvoice.Enabled = true;
            chkShowDetailsInvoice.Text = "Show details sales";

            LoadCustomer();
            LoadMonthDanYear();
            MainProgram.GlobalLanguageChange(this);
            base.SetHeader(header);
        }

        private void LoadCustomer()
        {
            ICustomerBll bll = new CustomerBll(_log);
            _listOfCustomer = bll.GetAll();

            FillDataHelper.FillCustomer(chkListBox, _listOfCustomer);
        }

        private void LoadCustomer(string name)
        {
            ICustomerBll bll = new CustomerBll(_log);
            _listOfCustomer = bll.GetByName(name);

            FillDataHelper.FillCustomer(chkListBox, _listOfCustomer);
        }

        private void LoadMonthDanYear()
        {
            FillDataHelper.FillMonth(cmbMonth, true);
            FillDataHelper.FillYear(cmbYear, true);
        }

        protected override void Find()
        {
            LoadCustomer(txtKeyword.Text);
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
            
            IReportSellingProductBll reportBll = new ReportSellingProductBll(_log);
            
            IList<ReportSalesProductHeader> listOfReportSales = new List<ReportSalesProductHeader>();
            IList<string> listOfCustomerId = new List<string>();

            if (chkBoxTitle.Checked)
            {
                listOfCustomerId = base.GetCustomerId(_listOfCustomer);

                if (listOfCustomerId.Count == 0)
                {
                    MsgHelper.MsgWarning("Minimum 1 customer must selected");
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

                listOfReportSales = reportBll.GetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportSales = reportBll.GetByMonth(month, year);
            }

            if (listOfCustomerId.Count > 0 && listOfReportSales.Count > 0)
            {
                listOfReportSales = listOfReportSales.Where(f => listOfCustomerId.Contains(f.customer_id))
                                                             .ToList();
            }

            if (listOfReportSales.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportSalesProductHeader",
                    Value = listOfReportSales
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvSalesProductHeader", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report data sales not found");
            }
        }

        private void PreviewReportDetail()
        {
            var periode = string.Empty;

            IReportSellingProductBll reportBll = new ReportSellingProductBll(_log);

            IList<ReportSalesProductDetail> listOfReportSales = new List<ReportSalesProductDetail>();

            IList<string> listOfCustomerId = new List<string>();

            if (chkBoxTitle.Checked)
            {
                listOfCustomerId = base.GetCustomerId(_listOfCustomer);

                if (listOfCustomerId.Count == 0)
                {
                    MsgHelper.MsgWarning("Minimum 1 customer must selected");
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

                listOfReportSales = reportBll.DetailGetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportSales = reportBll.DetailGetByMonth(month, year);
            }

            if (listOfCustomerId.Count > 0 && listOfReportSales.Count > 0)
            {
                listOfReportSales = listOfReportSales.Where(f => listOfCustomerId.Contains(f.customer_id))
                                                             .ToList();
            }

            if (listOfReportSales.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportSalesProductDetail",
                    Value = listOfReportSales
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvSalesProductDetail", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report data sales not found");
            }
        }
    }
}
