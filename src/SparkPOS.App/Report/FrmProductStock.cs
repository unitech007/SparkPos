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
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Lookup;

namespace SparkPOS.App.Report
{
    public partial class FrmProductStock : FrmSettingReportEmptyBody, IListener
    {
        private ILog _log;
        private Product _produk = null;
        private IList<Supplier> _listOfSupplier = new List<Supplier>();
        private IList<Category> _listOfCategory = new List<Category>();
        private IList<Product> _listOfProduct = new List<Product>();
        
        public FrmProductStock(string header)
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            _log = MainProgram.log;

           
            cmbStatusStock.SelectedIndex = 0;

            LoadSupplier();
            LoadCategory();
            AddHandler();
            MainProgram.GlobalLanguageChange(this);
            cmbStatusStock.Items.Clear();
            base.SetHeader(header);
            if (MainProgram.currentLanguage == "en-US")
            {
                this.cmbStatusStock.Items.AddRange(new object[] {
            "All",
            "there",
            "Empty"});

            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.cmbStatusStock.Items.AddRange(new object[] {
            "الجميع",
            "هناك",
            "فارغ"});

            }
        }

        private void AddHandler()
        {
            rdoStockLessFrom.CheckedChanged += rdoStatusStock_CheckedChanged;
            rdoStockBasedSupplier.CheckedChanged += rdoStatusStock_CheckedChanged;
            rdoStockBasedCategory.CheckedChanged += rdoStatusStock_CheckedChanged;
        }

        private void LoadSupplier()
        {
            ISupplierBll bll = new SupplierBll(_log);
            _listOfSupplier = bll.GetAll();

            FillDataHelper.FillSupplier(cmbSupplier, _listOfSupplier);

            if (_listOfSupplier.Count > 0)
                cmbSupplier.SelectedIndex = 0;
            else
                rdoStockBasedSupplier.Enabled = false;
        }

        private void LoadCategory()
        {
            ICategoryBll bll = new CategoryBll(_log);
            _listOfCategory = bll.GetAll();

            FillDataHelper.FillCategory(cmbCategory, _listOfCategory);

            if (_listOfCategory.Count > 0)
                cmbCategory.SelectedIndex = 0;
            else
                rdoStockBasedCategory.Enabled = false;
        }

        protected override void Preview()
        {
            var description = string.Empty;

            IReportStockProductBll reportBll = new ReportStockProductBll(_log);
            IList<ReportStockProduct> listOfReportStockProduct = new List<ReportStockProduct>();            

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (rdoStatusStock.Checked)
                {
                    description = string.Format("Stock based status stock {0}", cmbStatusStock.Text);

                    var statusStock = (StatusStock)cmbStatusStock.SelectedIndex + 1;
                    listOfReportStockProduct = reportBll.GetStockByStatus(statusStock);
                }
                else if (rdoStockLessFrom.Checked)
                {
                    description = string.Format("quantity stock less than {0}", txtStock.Text);
                    listOfReportStockProduct = reportBll.GetStockLessFrom(NumberHelper.StringToDouble(txtStock.Text));
                }
                else if (rdoStockBasedSupplier.Checked)
                {
                    description = string.Format("Stock based supplier {0}", cmbSupplier.Text);

                    var supplierId = _listOfSupplier[cmbSupplier.SelectedIndex].supplier_id;
                    listOfReportStockProduct = reportBll.GetStockBasedSupplier(supplierId);
                }
                else if (rdoStockBasedCategory.Checked)
                {
                    description = string.Format("Stock based category {0}", cmbCategory.Text);

                    var golonganId = _listOfCategory[cmbCategory.SelectedIndex].category_id;
                    listOfReportStockProduct = reportBll.GetStockBasedCategory(golonganId);
                }
                else if (rdoStockBasedProduct.Checked)
                {
                    description = "Stock based product";

                    IList<string> listOfCode = GetListCodeProduct(_listOfProduct);

                    if (listOfCode.Count == 0)
                    {
                        MsgHelper.MsgWarning("Minimum one name product must selected !");
                        txtNameProduct.Focus();
                        return;
                    }

                    listOfReportStockProduct = reportBll.GetStockBasedCode(listOfCode);
                }

                PreviewReport(listOfReportStockProduct, description);                   
            }
        }

        private IList<String> GetListCodeProduct(IList<Product> listOfProduct)
        {
            var result = new List<string>();

            for (int i = 0; i < listOfProduct.Count; i++)
            {
                if (chkListOfProduct.GetItemChecked(i))
                {
                    result.Add(listOfProduct[i].product_code);
                }
            }

            return result;
        }

        private void PreviewReport(IList<ReportStockProduct> listOfReportStockProduct, string description)
        {
            var periode = string.Empty;

            if (listOfReportStockProduct.Count > 0)
            {
                var reportDataSource = new ReportDataSource
                {
                    Name = "ReportProduct",
                    Value = listOfReportStockProduct
                };

                var parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("description", description));
                SparkPOS.Helper.MainProgram.currentLanguage = MainProgram.currentLanguage;
                base.ShowReport(this.Text, "RvStockProduct", reportDataSource, parameters);
            }
            else
            {
                MsgHelper.MsgInfo("Sorry report data stock product not found");
            }
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

        private void rdoStockBasedProduct_CheckedChanged(object sender, EventArgs e)
        {
            txtNameProduct.Focus();            
        }

        private void rdoStatusStock_CheckedChanged(object sender, EventArgs e)
        {
            _produk = null;
            _listOfProduct.Clear();
            txtNameProduct.Clear();
            chkListOfProduct.Items.Clear();
        }
    }
}
