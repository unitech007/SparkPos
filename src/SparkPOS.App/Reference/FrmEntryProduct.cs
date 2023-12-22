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

using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using SparkPOS.Helper.UserControl;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Reference
{
    public partial class FrmEntryProduct : FrmEntryStandard
    {        
        private IProductBll _bll = null; // deklarasi objek business logic layer 
        private Product _produk = null;
        private IList<Category> _listOfCategory;

        private IList<AdvancedTextbox> _listOfTxtPriceWholesale = new List<AdvancedTextbox>();
        private IList<AdvancedTextbox> _listOfTxtJumlahWholesale = new List<AdvancedTextbox>();
        private IList<AdvancedTextbox> _listOfTxtDiskonWholesale = new List<AdvancedTextbox>();

        private bool _isNewData = false;
        
        public IListener Listener { private get; set; }

        public FrmEntryProduct(string header, Category category, IList<Category> listOfCategory, IProductBll bll)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._listOfCategory = listOfCategory;
            this._bll = bll;
            this._isNewData = true;

            LoadDataCategory();
            LoadInputWholesale();

            if (category != null)
                cmbCategory.SelectedItem = category.name_category;

            txtCodeProduct.Text = this._bll.GetLastCodeProduct();
            txtProfit.Text = category.profit_percentage.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryProduct(string header, Product product, IList<Category> listOfCategory, IProductBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._listOfCategory = listOfCategory;
            this._bll = bll;
            this._produk = product;

            LoadInputWholesale();
            LoadDataCategory();

            if (this._produk.Category != null)
                cmbCategory.SelectedItem = this._produk.Category.name_category;

            txtCodeProduct.Text = this._produk.product_code;
            chkActive.Checked = this._produk.is_active;
            txtNameProduct.Text = this._produk.product_name;
            txtSatuan.Text = this._produk.unit;
            txtPricePurchase.Text = this._produk.purchase_price.ToString();
            txtProfit.Text = this._produk.profit_percentage.ToString();
            txtPriceSelling.Text = this._produk.selling_price.ToString();
            txtDiskon.Text = this._produk.discount.ToString();
            txtStock.Text = this._produk.stock.ToString();
            txtStockWarehouse.Text = this._produk.warehouse_stock.ToString();
            txtMinStockWarehouse.Text = this._produk.minimal_stock_warehouse.ToString();
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadInputWholesale()
        {
            _listOfTxtPriceWholesale.Add(txtPriceWholesale1);
            _listOfTxtPriceWholesale.Add(txtPriceWholesale2);
            _listOfTxtPriceWholesale.Add(txtPriceWholesale3);

            _listOfTxtJumlahWholesale.Add(txtJumlahMinimalWholesale1);
            _listOfTxtJumlahWholesale.Add(txtJumlahMinimalWholesale2);
            _listOfTxtJumlahWholesale.Add(txtJumlahMinimalWholesale3);

            _listOfTxtDiskonWholesale.Add(txtDiskonWholesale1);
            _listOfTxtDiskonWholesale.Add(txtDiskonWholesale2);
            _listOfTxtDiskonWholesale.Add(txtDiskonWholesale3);

            if (this._produk != null)
            {
                var listOfPriceWholesale = this._produk.list_of_harga_grosir;
                if (listOfPriceWholesale.Count > 0)
                {
                    var index = 0;
                    foreach (var grosir in listOfPriceWholesale)
                    {
                        var txtPriceWholesale = _listOfTxtPriceWholesale[index];
                        txtPriceWholesale.Text = grosir.wholesale_price.ToString();

                        var txtJumlahMinWholesale = _listOfTxtJumlahWholesale[index];
                        txtJumlahMinWholesale.Text = grosir.minimum_quantity.ToString();

                        var txtDiskonWholesale = _listOfTxtDiskonWholesale[index];
                        txtDiskonWholesale.Text = grosir.discount.ToString();

                        index++;
                    }
                }
            }            
        }

        private void LoadDataCategory()
        {
            cmbCategory.Items.Clear();
            foreach (var category in _listOfCategory)
            {
                cmbCategory.Items.Add(category.name_category);
            }

            if (_listOfCategory.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        protected override void Save()
        {
            if (_isNewData)
                _produk = new Product();
            
            if (_produk.list_of_harga_grosir.Count == 0)
            {
                var index = 0;
                foreach (var item in _listOfTxtPriceWholesale)
                {                    
                    var txtPriceWholesale = _listOfTxtPriceWholesale[index];
                    var txtJumlahMinWholesale = _listOfTxtJumlahWholesale[index];
                    var txtDiskonWholesale = _listOfTxtDiskonWholesale[index];

                    var hargaWholesale = new PriceWholesale
                    {
                        retail_price = index + 1,
                        wholesale_price = NumberHelper.StringToDouble(txtPriceWholesale.Text),
                        minimum_quantity = NumberHelper.StringToDouble(txtJumlahMinWholesale.Text, true),
                        discount = NumberHelper.StringToDouble(txtDiskonWholesale.Text, true)
                    };

                    _produk.list_of_harga_grosir.Add(hargaWholesale);

                    index++;
                }
            }
            else
            {
                var index = 0;
                foreach (var item in _produk.list_of_harga_grosir)
	            {
                    var txtPriceWholesale = _listOfTxtPriceWholesale[index];
                    var txtJumlahMinWholesale = _listOfTxtJumlahWholesale[index];
                    var txtDiskonWholesale = _listOfTxtDiskonWholesale[index];
                    
                    item.wholesale_price = NumberHelper.StringToDouble(txtPriceWholesale.Text);
                    item.minimum_quantity = NumberHelper.StringToDouble(txtJumlahMinWholesale.Text, true);
                    item.discount = NumberHelper.StringToDouble(txtDiskonWholesale.Text, true);

                    index++;
	            }
            }

            var category = _listOfCategory[cmbCategory.SelectedIndex];
            _produk.category_id = category.category_id;
            _produk.Category = category;

            _produk.product_code = txtCodeProduct.Text;
            _produk.is_active = chkActive.Checked;
            _produk.product_name = txtNameProduct.Text;
            _produk.unit = txtSatuan.Text;
            _produk.purchase_price = NumberHelper.StringToDouble(txtPricePurchase.Text);
            _produk.selling_price = NumberHelper.StringToDouble(txtPriceSelling.Text);
            _produk.discount = NumberHelper.StringToDouble(txtDiskon.Text, true);
            _produk.profit_percentage = NumberHelper.StringToDouble(txtProfit.Text, true);
            _produk.stock = NumberHelper.StringToDouble(txtStock.Text, true);
            _produk.warehouse_stock = NumberHelper.StringToDouble(txtStockWarehouse.Text, true);
            _produk.minimal_stock_warehouse = NumberHelper.StringToDouble(txtMinStockWarehouse.Text, true);

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_produk, ref validationError);
                else
                    result = _bll.Update(_produk, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _produk);

                    if (_isNewData)
                    {
                        base.ResetForm(this);
                        
                        chkActive.Checked = true;
                        txtCodeProduct.Text = this._bll.GetLastCodeProduct();
                        txtCodeProduct.Focus();
                    }
                    else
                        this.Close();

                }
                else
                {
                    if (validationError.Message.NullToString().Length > 0)
                    {
                        MsgHelper.MsgWarning(validationError.Message);
                        base.SetFocusObject(validationError.PropertyName, this);
                    }
                    else
                    {
                        MsgHelper.MsgDuplicate("");
                        txtCodeProduct.Focus();
                        txtCodeProduct.SelectAll();
                    }
                }         
            }                   
        }

        private void txtMinStockWarehouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void HitungPriceRetail(object sender, EventArgs e)
        {
            double hargaPurchase = NumberHelper.StringToNumber(txtPricePurchase.Text);
            double keuntungan = NumberHelper.StringToDouble(txtProfit.Text, true);

            if (keuntungan > 0)
            {
                var hargaSelling = hargaPurchase + (hargaPurchase * keuntungan / 100);
                txtPriceSelling.Text = hargaSelling.ToString();
            }
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var category = _listOfCategory[cmbCategory.SelectedIndex];
            if (category != null)
                txtProfit.Text = category.profit_percentage.ToString();
        }

        private void chkActive_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e)) KeyPressHelper.NextFocus();
        }        
    }
}
