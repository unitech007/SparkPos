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
using System.Drawing.Printing;

using log4net;
using Zen.Barcode;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Lookup;

namespace SparkPOS.App.Reference
{
    public partial class FrmPrintingLabelBarcodeProduct : Form, IListener
    {
        private Product _produk = null;
        private SettingsBarcode _settingsBarcode = null;
        private IList<CheckBox> _listOfCheckboxPositionLabel = new List<CheckBox>();
        private IList<Panel> _listOfPanelPositionLabel = new List<Panel>();
        private ILog _log;

        public FrmPrintingLabelBarcodeProduct(string header)
        {
             InitializeComponent();  MainProgram.GlobalLanguageChange(this);            
            ColorManagerHelper.SetTheme(this, this);

            this.Text = header;
            this.lblHeader.Text = header;
            this._log = MainProgram.log;
            this._settingsBarcode = MainProgram.settingsBarcode;

            InitializeList();
            LoadSettingsBarcode();
        }

        private void LoadSettingsBarcode()
        {
            txtHeaderBarcode.Text = _settingsBarcode.header_label;
            LoadPrinter(_settingsBarcode.name_printer);

            txtBatasAtasRow1.Text = _settingsBarcode.batas_atas_row1.ToString();
            txtBatasAtasRow2.Text = _settingsBarcode.batas_atas_row2.ToString();
            txtBatasAtasRow3.Text = _settingsBarcode.batas_atas_row3.ToString();
            txtBatasAtasRow4.Text = _settingsBarcode.batas_atas_row4.ToString();

            txtBatasKiriColumn1.Text = _settingsBarcode.batas_kiri_column1.ToString();
            txtBatasKiriColumn2.Text = _settingsBarcode.batas_kiri_column2.ToString();
            txtBatasKiriColumn3.Text = _settingsBarcode.batas_kiri_column3.ToString();
        }

        private void InitializeList()
        {
            _listOfPanelPositionLabel.Add(pnlPosition1);
            _listOfPanelPositionLabel.Add(pnlPosition2);
            _listOfPanelPositionLabel.Add(pnlPosition3);
            _listOfPanelPositionLabel.Add(pnlPosition4);
            _listOfPanelPositionLabel.Add(pnlPosition5);
            _listOfPanelPositionLabel.Add(pnlPosition6);
            _listOfPanelPositionLabel.Add(pnlPosition7);
            _listOfPanelPositionLabel.Add(pnlPosition8);
            _listOfPanelPositionLabel.Add(pnlPosition9);
            _listOfPanelPositionLabel.Add(pnlPosition10);
            _listOfPanelPositionLabel.Add(pnlPosition11);
            _listOfPanelPositionLabel.Add(pnlPosition12);

            _listOfCheckboxPositionLabel.Add(chkPosition1);
            _listOfCheckboxPositionLabel.Add(chkPosition2);
            _listOfCheckboxPositionLabel.Add(chkPosition3);
            _listOfCheckboxPositionLabel.Add(chkPosition4);
            _listOfCheckboxPositionLabel.Add(chkPosition5);
            _listOfCheckboxPositionLabel.Add(chkPosition6);
            _listOfCheckboxPositionLabel.Add(chkPosition7);
            _listOfCheckboxPositionLabel.Add(chkPosition8);
            _listOfCheckboxPositionLabel.Add(chkPosition9);
            _listOfCheckboxPositionLabel.Add(chkPosition10);
            _listOfCheckboxPositionLabel.Add(chkPosition11);
            _listOfCheckboxPositionLabel.Add(chkPosition12);

            // non Activate checkbox pilihan position label
            foreach (var checkbox in _listOfCheckboxPositionLabel)
            {
                checkbox.Enabled = false;
            }
        }

        private void LoadPrinter(string defaultPrinter)
        {
            foreach (var printer in PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(printer);
            }

            if (defaultPrinter.Length > 0)
                cmbPrinter.Text = defaultPrinter;
            else
            {
                if (cmbPrinter.Items.Count > 0)
                    cmbPrinter.SelectedIndex = 0;
            }
        }

        private void PreviewBarcode()
        {
            if (txtCodeProduct.Text.Length > 0 && _produk != null)
            {
                barcodePanel.Symbology = BarcodeSymbology.Code128;
                barcodePanel.MaxBarHeight = 50;

                barcodePanel.HeaderLabel = txtHeaderBarcode.Text;
                barcodePanel.Text = txtCodeProduct.Text;
                barcodePanel.PriceLabel = NumberHelper.StringToNumber(txtPriceSelling.Text);
                barcodePanel.IsDisplayPriceLabel = chkPrintPriceSelling.Checked;

                foreach (var panel in _listOfPanelPositionLabel)
                {
                    panel.BackgroundImage = barcodePanel.BackgroundImage;
                }

                foreach (var checkbox in _listOfCheckboxPositionLabel)
                {
                    checkbox.Enabled = true;
                }

                chkSelectAll.Enabled = true;
                updJumlahPrint.Enabled = true;
                btnPrint.Enabled = true;
            }
        }

        private void ResetBarcode()
        {
            barcodePanel.Text = "";
            barcodePanel.BackgroundImage = null;

            foreach (var panel in _listOfPanelPositionLabel)
            {
                panel.BackgroundImage = barcodePanel.BackgroundImage;
            }

            foreach (var checkbox in _listOfCheckboxPositionLabel)
            {
                checkbox.Enabled = false;
            }

            txtNameProduct.Clear();
            txtPriceSelling.Text = "0";

            updJumlahPrint.Enabled = false;
            btnPrint.Enabled = false;
            chkSelectAll.Enabled = false;
        }

        private void SetDataProduct(Product product)
        {
            txtCodeProduct.Text = product.product_code;
            txtNameProduct.Text = product.product_name;
            txtPriceSelling.Text = product.selling_price.ToString();
        }

        private void SaveAppConfig()
        {
            var appConfigFile = string.Format("{0}\\SparkPOS.exe.config", Utils.GetAppPath());

            this._settingsBarcode.header_label = txtHeaderBarcode.Text;
            this._settingsBarcode.name_printer = cmbPrinter.Text;
            this._settingsBarcode.batas_atas_row1 = Convert.ToSingle(txtBatasAtasRow1.Text);
            this._settingsBarcode.batas_atas_row2 = Convert.ToSingle(txtBatasAtasRow2.Text);
            this._settingsBarcode.batas_atas_row3 = Convert.ToSingle(txtBatasAtasRow3.Text);
            this._settingsBarcode.batas_atas_row4 = Convert.ToSingle(txtBatasAtasRow4.Text);

            this._settingsBarcode.batas_kiri_column1 = Convert.ToSingle(txtBatasKiriColumn1.Text);
            this._settingsBarcode.batas_kiri_column2 = Convert.ToSingle(txtBatasKiriColumn2.Text);
            this._settingsBarcode.batas_kiri_column3 = Convert.ToSingle(txtBatasKiriColumn3.Text);

            AppConfigHelper.SaveValue("headerLabel", txtHeaderBarcode.Text, appConfigFile);
            AppConfigHelper.SaveValue("printerBarcode", cmbPrinter.Text, appConfigFile);

            AppConfigHelper.SaveValue("batasAtasRow1", txtBatasAtasRow1.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow2", txtBatasAtasRow2.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow3", txtBatasAtasRow3.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow4", txtBatasAtasRow4.Text, appConfigFile);

            AppConfigHelper.SaveValue("batasKiriColumn1", txtBatasKiriColumn1.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasKiriColumn2", txtBatasKiriColumn2.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasKiriColumn3", txtBatasKiriColumn3.Text, appConfigFile);
        }

        /// <summary>
        /// Method untuk mengecek Minimum 1 position label barcode must selected sebelum diprint
        /// </summary>
        /// <returns></returns>
        private bool IsSelect()
        {
            foreach (var checkbox in _listOfCheckboxPositionLabel)
            {
                if (checkbox.Checked)
                    return true;
            }

            return false;
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
                    var listOfProduct = produkBll.GetByName(keyword);

                    if (listOfProduct.Count == 0)
                    {
                        MsgHelper.MsgWarning("Product data not found");
                        txtCodeProduct.Focus();
                        txtCodeProduct.SelectAll();

                        ResetBarcode();
                    }
                    else if (listOfProduct.Count == 1)
                    {
                        this._produk = listOfProduct[0];

                        SetDataProduct(this._produk);
                        PreviewBarcode();
                    }
                    else // data lebih dari one, tampilkan form lookup
                    {
                        var frmLookup = new FrmLookupReference("Data Product", listOfProduct);
                        frmLookup.Listener = this;
                        frmLookup.ShowDialog();
                    }
                }
                else
                {
                    SetDataProduct(this._produk);
                    PreviewBarcode();
                }
            }
        }        

        private void chkPrintPriceSelling_CheckedChanged(object sender, EventArgs e)
        {
            PreviewBarcode();
        }

        private void txtHeaderBarcode_TextChanged(object sender, EventArgs e)
        {
            PreviewBarcode();
        }        

        public void Ok(object sender, object data)
        {
            if (data is Product) // pencarian product baku
            {
                this._produk = (Product)data;

                SetDataProduct(this._produk);
                PreviewBarcode();
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            throw new NotImplementedException();
        }        

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!IsSelect())
            {
                MsgHelper.MsgWarning("Minimum one position label barcode must selected !");
                return;
            }

            if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
            {
                SaveAppConfig();

                PrintDocument printBarcode = new PrintDocument();
                printBarcode.PrinterSettings.PrinterName = cmbPrinter.Text;
                printBarcode.PrinterSettings.Copies = (short)updJumlahPrint.Value;
                printBarcode.PrintPage += printBarcode_PrintPage;
                printBarcode.Print();
            }
        }

        private void printBarcode_PrintPage(object sender, PrintPageEventArgs e)
        {
            var barcodeColumns = 3;
            var barcodesPerPage = 12;

            // Determine printable region for each barcode and label
            var numLines = barcodesPerPage / barcodeColumns;

            if ((barcodesPerPage % barcodeColumns) != 0)
            {
                ++numLines;
            }

            var barcodeArea = new SizeF();

            barcodeArea.Width = (e.MarginBounds.Width / barcodeColumns);
            barcodeArea.Height = (e.MarginBounds.Height / numLines);

            var listOfPosition = new Dictionary<int, PointF>();
            
            // row 1
            listOfPosition.Add(0, new PointF(_settingsBarcode.batas_kiri_column1, _settingsBarcode.batas_atas_row1));
            listOfPosition.Add(1, new PointF(_settingsBarcode.batas_kiri_column2, listOfPosition[0].Y));
            listOfPosition.Add(2, new PointF(_settingsBarcode.batas_kiri_column3, listOfPosition[0].Y));

            // row 2
            listOfPosition.Add(3, new PointF(listOfPosition[0].X, _settingsBarcode.batas_atas_row2));
            listOfPosition.Add(4, new PointF(listOfPosition[1].X, listOfPosition[3].Y));
            listOfPosition.Add(5, new PointF(listOfPosition[2].X, listOfPosition[3].Y));

            // row 3
            listOfPosition.Add(6, new PointF(listOfPosition[0].X, _settingsBarcode.batas_atas_row3));
            listOfPosition.Add(7, new PointF(listOfPosition[1].X, listOfPosition[6].Y));
            listOfPosition.Add(8, new PointF(listOfPosition[2].X, listOfPosition[6].Y));

            // row 4
            listOfPosition.Add(9, new PointF(listOfPosition[0].X, _settingsBarcode.batas_atas_row4));
            listOfPosition.Add(10, new PointF(listOfPosition[1].X, listOfPosition[9].Y));
            listOfPosition.Add(11, new PointF(listOfPosition[2].X, listOfPosition[9].Y));

            for (var index = 0; index < barcodesPerPage; index++)
            {
                var isPrint = _listOfCheckboxPositionLabel[index].Checked;
                
                if (isPrint)
                {
                    var position = listOfPosition[index];

                    var drawRectangle = new RectangleF(position, barcodeArea);
                    var barcodeImageLocation = new PointF(position.X, position.Y);
                    
                    barcodeImageLocation.X += (drawRectangle.Width - barcodePanel.BackgroundImage.Width) / 2;

                    e.Graphics.DrawImage(barcodePanel.BackgroundImage, barcodeImageLocation);
                }
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var checkbox in _listOfCheckboxPositionLabel)
            {
                checkbox.Checked = ((CheckBox)sender).Checked;
            }
        }

        private void FrmPrintingLabelBarcodeProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                this.Close();
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
