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
    public partial class FrmLapProductSalesCredit : FrmSettingReportStandard
    {
        private IList<Customer> _listOfCustomer = new List<Customer>();
        private ILog _log;

        public FrmLapProductSalesCredit(string header)
        {
             InitializeComponent(); 
           
            base.SetCheckBoxTitle("Select Customer");
            base.SetToolTip("Find Customer ...");
            base.ReSize(35);

            _log = MainProgram.log;

            chkShowInvoice.Text = "Show invoice sale";
            chkShowDetailsInvoice.Text = "Show details invoice sale";
            chkShowDetailsInvoice.Visible = true;

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

        protected override void SelectCheckBoxShowInvoice()
        {
            chkShowDetailsInvoice.Enabled = chkShowInvoice.Checked;

            if (!chkShowInvoice.Checked)
                chkShowDetailsInvoice.Checked = false;
        }

        protected override void Preview()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (chkShowInvoice.Checked)
                {
                    if (chkShowDetailsInvoice.Checked)
                    {
                        PreviewReportPerProduct();
                    }
                    else
                    {
                        PreviewReportDetail();
                    }                    
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
                        
            IReportCreditSellingProductBll reportBll = new ReportCreditSellingProductBll(_log);
            
            IList<ReportCreditSalesProductHeader> listOfReportCreditSales = new List<ReportCreditSalesProductHeader>();
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

                listOfReportCreditSales = reportBll.GetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportCreditSales = reportBll.GetByMonth(month, year);
            }

            if (listOfCustomerId.Count > 0 && listOfReportCreditSales.Count > 0)
            {
                listOfReportCreditSales = listOfReportCreditSales.Where(f => listOfCustomerId.Contains(f.customer_id))
                                                             .ToList();
            }

            if (listOfReportCreditSales.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportCreditSalesProductHeader",
                    Value = listOfReportCreditSales
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvCreditSalesProductHeader", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report data credit sales not found");
            }
        }

        private void PreviewReportDetail()
        {
            var periode = string.Empty;

            IReportCreditSellingProductBll reportBll = new ReportCreditSellingProductBll(_log);
            
            IList<ReportCreditSalesProductDetail> listOfReportCreditSales = new List<ReportCreditSalesProductDetail>();

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

                listOfReportCreditSales = reportBll.DetailGetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportCreditSales = reportBll.DetailGetByMonth(month, year);
            }

            if (listOfCustomerId.Count > 0 && listOfReportCreditSales.Count > 0)
            {
                listOfReportCreditSales = listOfReportCreditSales.Where(f => listOfCustomerId.Contains(f.customer_id))
                                                             .ToList();
            }

            if (listOfReportCreditSales.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportCreditSalesProductDetail",
                    Value = listOfReportCreditSales
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvCreditSalesProductDetail", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report data credit sales not found");
            }
        }

        private void PreviewReportPerProduct()
        {
            var periode = string.Empty;

            IReportCreditSellingProductBll reportBll = new ReportCreditSellingProductBll(_log);
            
            IList<ReportCreditSalesProduct> listOfReportCreditSales = new List<ReportCreditSalesProduct>();

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

                listOfReportCreditSales = reportBll.PerProductGetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                listOfReportCreditSales = reportBll.PerProductGetByMonth(month, year);
            }

            if (listOfCustomerId.Count > 0 && listOfReportCreditSales.Count > 0)
            {
                listOfReportCreditSales = listOfReportCreditSales.Where(f => listOfCustomerId.Contains(f.customer_id))
                                                             .ToList();
            }

            if (listOfReportCreditSales.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportCreditSalesProduct",
                    Value = listOfReportCreditSales
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvCreditSalesProduct", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report data credit sales not found");
            }
        }
    }
}
