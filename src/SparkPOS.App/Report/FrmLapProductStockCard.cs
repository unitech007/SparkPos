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
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Lookup;

namespace SparkPOS.App.Report
{
    public partial class FrmLapProductStockCard : FrmSettingReportEmptyBody, IListener
    {
        private ILog _log;
        private Product _produk = null;
        private IList<Product> _listOfProduct = new List<Product>();

        public FrmLapProductStockCard(string header)
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

            IReportCardStockBll reportBll = new ReportCardStockBll(_log);
            IList<ReportCardStock> listOfReport = new List<ReportCardStock>();

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

                if (chkFilterOptional.Checked)
                {
                    IList<string> listOfCode = GetListCodeProduct(_listOfProduct);

                    if (listOfCode.Count == 0)
                    {
                        MsgHelper.MsgWarning("Minimum one name product must selected !");
                        txtNameProduct.Focus();
                        return;
                    }

                    listOfReport = reportBll.GetByDate(dtpDateMulai.Value, dtpDateSelesai.Value, listOfCode);
                }
                else
                    listOfReport = reportBll.GetByDate(dtpDateMulai.Value, dtpDateSelesai.Value);
            }
            else
            {
                periode = string.Format("Periode : {0} {1}", cmbMonth.Text, cmbYear.Text);

                var month = cmbMonth.SelectedIndex + 1;
                var year = int.Parse(cmbYear.Text);

                if (chkFilterOptional.Checked)
                {
                    IList<string> listOfCode = GetListCodeProduct(_listOfProduct);

                    if (listOfCode.Count == 0)
                    {
                        MsgHelper.MsgWarning("Minimum one name product must selected !");
                        txtNameProduct.Focus();
                        return;
                    }

                    listOfReport = reportBll.GetByMonth(month, year, listOfCode);
                }
                else
                    listOfReport = reportBll.GetByMonth(month, year);
            }

            if (listOfReport.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "DsReportCardStock",
                    Value = listOfReport
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("periode", periode));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvCardStock", reportDataSource, parameters);
            }            
            else
            {
                MsgHelper.MsgInfo("Sorry report data card stock not found");
            }
        }

        private IList<String> GetListCodeProduct(IList<Product> listOfProduct)
        {
            var result = new List<string>();

            for (int i = 0; i < listOfProduct.Count; i++)
            {
                if (chkListOfProduct.GetItemChecked(i))
                {
                    result.Add(listOfProduct[i].product_code.ToLower());
                }
            }

            return result;
        }

        private void txtNameProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var keyword = ((AdvancedTextbox)sender).Text;

                IProductBll produkBll = new ProductBll(_log);
                this._produk = produkBll.GetByCode(keyword);

                if (this._produk == null)
                {
                    var listOfProduct = produkBll.GetByName(keyword, false);

                    if (listOfProduct.Count == 0)
                    {
                        MsgHelper.MsgWarning("Product data not found");
                        txtNameProduct.Focus();
                        txtNameProduct.SelectAll();
                    }
                    else if (listOfProduct.Count == 1)
                    {
                        this._produk = listOfProduct[0];

                        FillListProduct(this._produk);
                    }
                    else // data lebih dari one
                    {
                        var frmLookup = new FrmLookupReference("Data Product", listOfProduct);
                        frmLookup.Listener = this;
                        frmLookup.ShowDialog();
                    }
                }
                else
                {
                    FillListProduct(this._produk);
                }
            }
        }

        private void FillListProduct(Product product)
        {
            txtNameProduct.Clear();
            this._listOfProduct.Add(product);
            chkListOfProduct.Items.Add(product.product_name);
            chkListOfProduct.SetItemChecked(chkListOfProduct.Items.Count - 1, true);
        }

        public void Ok(object sender, object data)
        {
            if (data is Product) // pencarian product
            {
                this._produk = (Product)data;

                FillListProduct(this._produk);
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            throw new NotImplementedException();
        }

        private void chkFilterOptional_CheckedChanged(object sender, EventArgs e)
        {
            var chkFilter = (CheckBox)sender;

            txtNameProduct.Enabled = chkFilter.Checked;
            chkListOfProduct.Enabled = chkFilter.Checked;

            if (chkFilter.Checked)
                txtNameProduct.Focus();
            else
            {
                _produk = null;
                _listOfProduct.Clear();
                txtNameProduct.Clear();
                chkListOfProduct.Items.Clear();
            }
        }
    }
}
