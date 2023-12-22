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
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Lookup;
using log4net;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Reference
{
    public partial class FrmEntryAdjustmentStock : FrmEntryStandard, IListener
    {
        private IAdjustmentStockBll _bll = null; // deklarasi objek business logic layer 
        private AdjustmentStock _penyesuaianStock = null;
        private Product _produk = null;
        private IList<ReasonAdjustmentStock> _listOfReasonAdjustment;

        private bool _isNewData = false;
        private ILog _log;

        public IListener Listener { private get; set; }

        public FrmEntryAdjustmentStock(string header, IAdjustmentStockBll bll)
            : base()
        {
             InitializeComponent();  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this._isNewData = true;
            this._log = MainProgram.log;

            LoadReasonAdjustmentStock();
        }

        public FrmEntryAdjustmentStock(string header, AdjustmentStock penyesuaianStock, IAdjustmentStockBll bll)
            : base()
        {
             InitializeComponent();  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._penyesuaianStock = penyesuaianStock;
            this._log = MainProgram.log;

            this._produk = this._penyesuaianStock.Product;
            txtCodeProduct.Text = this._produk.product_code;
            txtCodeProduct.Enabled = false;
            txtNameProduct.Text = this._produk.product_name;
            txtStockShelf.Text = this._produk.stock.ToString();
            txtStockWarehouse.Text = this._produk.warehouse_stock.ToString();

            // info mutasi
            dtpDate.Value = (DateTime)this._penyesuaianStock.date;

            txtAdditionStockShelf.Text = this._penyesuaianStock.stock_addition.ToString();
            txtAdditionStockWarehouse.Text = this._penyesuaianStock.warehouse_stock_addition.ToString();

            txtReductionStockShelf.Text = this._penyesuaianStock.stock_reduction.ToString();
            txtReductionStockWarehouse.Text = this._penyesuaianStock.warehouse_stock_reduction.ToString();

            txtKeterangan.Text = this._penyesuaianStock.description;

            LoadReasonAdjustmentStock();
            if (this._penyesuaianStock.ReasonAdjustmentStock != null)
                cmbReasonAdjustment.SelectedItem = this._penyesuaianStock.ReasonAdjustmentStock.reason;
        }

        private void LoadReasonAdjustmentStock()
        {
            IReasonAdjustmentStockBll bll = new ReasonAdjustmentStockBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _listOfReasonAdjustment = bll.GetAll();

            cmbReasonAdjustment.Items.Clear();
            foreach (var reason in _listOfReasonAdjustment)
            {
                cmbReasonAdjustment.Items.Add(reason.reason);
            }

            if (_listOfReasonAdjustment.Count > 0)
                cmbReasonAdjustment.SelectedIndex = 0;
        }

        protected override void Save()
        {
            if (txtCodeProduct.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Code Product' should not empty !");
                txtCodeProduct.Focus();
                return;
            }

            if (this._produk == null)
            {
                MsgHelper.MsgWarning("'Code Product' not found !");
                txtCodeProduct.Focus();
                return;
            }

            if (_isNewData)
                _penyesuaianStock = new AdjustmentStock();

            _penyesuaianStock.product_id = this._produk.product_id;
            _penyesuaianStock.Product = this._produk;

            var alasanAdjustment = _listOfReasonAdjustment[cmbReasonAdjustment.SelectedIndex];
            _penyesuaianStock.adjustment_reason_id = alasanAdjustment.stock_adjustment_reason_id;
            _penyesuaianStock.ReasonAdjustmentStock = alasanAdjustment;

            _penyesuaianStock.date = dtpDate.Value;
            _penyesuaianStock.stock_addition = NumberHelper.StringToDouble(txtAdditionStockShelf.Text);
            _penyesuaianStock.warehouse_stock_addition = NumberHelper.StringToDouble(txtAdditionStockWarehouse.Text);
            _penyesuaianStock.stock_reduction = NumberHelper.StringToDouble(txtReductionStockShelf.Text);
            _penyesuaianStock.warehouse_stock_reduction = NumberHelper.StringToDouble(txtReductionStockWarehouse.Text);

            _penyesuaianStock.description = txtKeterangan.Text;

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                    result = _bll.Save(_penyesuaianStock, ref validationError);
                else
                    result = _bll.Update(_penyesuaianStock, ref validationError);

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _penyesuaianStock);

                    if (_isNewData)
                    {
                        base.ResetForm(this);
                        this._produk = null;
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
                        MsgHelper.MsgUpdateError();
                }       
            }                     
        }

        private void txtCategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void txtKeterangan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void txtCodeProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var keyword = ((AdvancedTextbox)sender).Text;

                IProductBll produkBll = new ProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                this._produk = produkBll.GetByCode(keyword);

                if (this._produk == null)
                {
                    var listOfProduct = produkBll.GetByName(keyword, false);

                    if (listOfProduct.Count == 0)
                    {
                        MsgHelper.MsgWarning("Product data not found");
                        txtCodeProduct.Focus();
                        txtCodeProduct.SelectAll();
                    }
                    else if (listOfProduct.Count == 1)
                    {
                        this._produk = listOfProduct[0];

                        SetDataProduct(this._produk);
                        KeyPressHelper.NextFocus();
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
                    SetDataProduct(this._produk);
                    KeyPressHelper.NextFocus();
                }
            }
        }

        private void SetDataProduct(Product product)
        {
            txtCodeProduct.Text = product.product_code;
            txtNameProduct.Text = product.product_name;
            txtStockShelf.Text = product.stock.ToString();
            txtStockWarehouse.Text = product.warehouse_stock.ToString();
        }

        public void Ok(object sender, object data)
        {
            if (data is Product) // pencarian product baku
            {
                this._produk = (Product)data;

                SetDataProduct(this._produk);
                KeyPressHelper.NextFocus();
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            throw new NotImplementedException();
        }

        private void txtCodeProduct_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
