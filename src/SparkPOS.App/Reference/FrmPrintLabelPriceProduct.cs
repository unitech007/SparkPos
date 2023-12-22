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
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Lookup;
using System.Drawing.Text;

namespace SparkPOS.App.Reference
{
    public partial class FrmPrintLabelPriceProduct : Form, IListener
    {
        private Product _produk = null;
        private SettingsLabelPrice _settingsLabelPrice = null;
        private IList<CheckBox> _listOfCheckboxPositionLabel = new List<CheckBox>();
        private IList<Panel> _listOfPanelPositionLabel = new List<Panel>();
        private IList<Panel> _listOfPanelPositionLabel2 = new List<Panel>();
        private IList<LabelPriceProduct> _listOfLabelPriceProduct = new List<LabelPriceProduct>();

        private ILog _log;

        public FrmPrintLabelPriceProduct(string header)
        {
             InitializeComponent();              
            ColorManagerHelper.SetTheme(this, this);

            this.Text = header;
            this.lblHeader.Text = header;
            this._log = MainProgram.log;
            this._settingsLabelPrice = MainProgram.settingsLabelPrice;

            InitializeList();
            LoadSettingsLabelPriceProduct();
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadSettingsLabelPriceProduct()
        {
            LoadPrinter(_settingsLabelPrice.name_printer);

            txtBatasAtasRow1.Text = _settingsLabelPrice.batas_atas_row1.ToString();
            txtBatasAtasRow2.Text = _settingsLabelPrice.batas_atas_row2.ToString();
            txtBatasAtasRow3.Text = _settingsLabelPrice.batas_atas_row3.ToString();
            txtBatasAtasRow4.Text = _settingsLabelPrice.batas_atas_row4.ToString();
            txtBatasAtasRow5.Text = _settingsLabelPrice.batas_atas_row5.ToString();
            txtBatasAtasRow6.Text = _settingsLabelPrice.batas_atas_row6.ToString();
            txtBatasAtasRow7.Text = _settingsLabelPrice.batas_atas_row7.ToString();
            txtBatasAtasRow8.Text = _settingsLabelPrice.batas_atas_row8.ToString();

            txtBatasKiriColumn1.Text = _settingsLabelPrice.batas_kiri_column1.ToString();
            txtBatasKiriColumn2.Text = _settingsLabelPrice.batas_kiri_column2.ToString();
            txtBatasKiriColumn3.Text = _settingsLabelPrice.batas_kiri_column3.ToString();
            txtBatasKiriColumn4.Text = _settingsLabelPrice.batas_kiri_column4.ToString();
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
            _listOfPanelPositionLabel.Add(pnlPosition13);
            _listOfPanelPositionLabel.Add(pnlPosition14);
            _listOfPanelPositionLabel.Add(pnlPosition15);
            _listOfPanelPositionLabel.Add(pnlPosition16);
            _listOfPanelPositionLabel.Add(pnlPosition17);
            _listOfPanelPositionLabel.Add(pnlPosition18);
            _listOfPanelPositionLabel.Add(pnlPosition19);
            _listOfPanelPositionLabel.Add(pnlPosition20);
            _listOfPanelPositionLabel.Add(pnlPosition21);
            _listOfPanelPositionLabel.Add(pnlPosition22);
            _listOfPanelPositionLabel.Add(pnlPosition23);
            _listOfPanelPositionLabel.Add(pnlPosition24);
            _listOfPanelPositionLabel.Add(pnlPosition25);
            _listOfPanelPositionLabel.Add(pnlPosition26);
            _listOfPanelPositionLabel.Add(pnlPosition27);
            _listOfPanelPositionLabel.Add(pnlPosition28);
            _listOfPanelPositionLabel.Add(pnlPosition29);
            _listOfPanelPositionLabel.Add(pnlPosition30);
            _listOfPanelPositionLabel.Add(pnlPosition31);
            _listOfPanelPositionLabel.Add(pnlPosition32);

            _listOfPanelPositionLabel2.Add(panel14);
            _listOfPanelPositionLabel2.Add(panel18);
            _listOfPanelPositionLabel2.Add(panel22);
            _listOfPanelPositionLabel2.Add(panel41);
            _listOfPanelPositionLabel2.Add(panel15);
            _listOfPanelPositionLabel2.Add(panel19);
            _listOfPanelPositionLabel2.Add(panel23);
            _listOfPanelPositionLabel2.Add(panel43);
            _listOfPanelPositionLabel2.Add(panel16);
            _listOfPanelPositionLabel2.Add(panel20);
            _listOfPanelPositionLabel2.Add(panel24);
            _listOfPanelPositionLabel2.Add(panel45);
            _listOfPanelPositionLabel2.Add(panel17);
            _listOfPanelPositionLabel2.Add(panel21);
            _listOfPanelPositionLabel2.Add(panel37);
            _listOfPanelPositionLabel2.Add(panel47);
            _listOfPanelPositionLabel2.Add(panel28);
            _listOfPanelPositionLabel2.Add(panel30);
            _listOfPanelPositionLabel2.Add(panel32);
            _listOfPanelPositionLabel2.Add(panel49);
            _listOfPanelPositionLabel2.Add(panel34);
            _listOfPanelPositionLabel2.Add(panel36);
            _listOfPanelPositionLabel2.Add(panel39);
            _listOfPanelPositionLabel2.Add(panel51);
            _listOfPanelPositionLabel2.Add(panel53);
            _listOfPanelPositionLabel2.Add(panel55);
            _listOfPanelPositionLabel2.Add(panel57);
            _listOfPanelPositionLabel2.Add(panel59);
            _listOfPanelPositionLabel2.Add(panel61);
            _listOfPanelPositionLabel2.Add(panel63);
            _listOfPanelPositionLabel2.Add(panel65);
            _listOfPanelPositionLabel2.Add(panel67);

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
            _listOfCheckboxPositionLabel.Add(chkPosition13);
            _listOfCheckboxPositionLabel.Add(chkPosition14);
            _listOfCheckboxPositionLabel.Add(chkPosition15);
            _listOfCheckboxPositionLabel.Add(chkPosition16);
            _listOfCheckboxPositionLabel.Add(chkPosition17);
            _listOfCheckboxPositionLabel.Add(chkPosition18);
            _listOfCheckboxPositionLabel.Add(chkPosition19);
            _listOfCheckboxPositionLabel.Add(chkPosition20);
            _listOfCheckboxPositionLabel.Add(chkPosition21);
            _listOfCheckboxPositionLabel.Add(chkPosition22);
            _listOfCheckboxPositionLabel.Add(chkPosition23);
            _listOfCheckboxPositionLabel.Add(chkPosition24);
            _listOfCheckboxPositionLabel.Add(chkPosition25);
            _listOfCheckboxPositionLabel.Add(chkPosition26);
            _listOfCheckboxPositionLabel.Add(chkPosition27);
            _listOfCheckboxPositionLabel.Add(chkPosition28);
            _listOfCheckboxPositionLabel.Add(chkPosition29);
            _listOfCheckboxPositionLabel.Add(chkPosition30);
            _listOfCheckboxPositionLabel.Add(chkPosition31);
            _listOfCheckboxPositionLabel.Add(chkPosition32);

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

        private void PreviewLabelPriceProduct(string fontName = "Courier New")
        {
            if (txtCodeProduct.Text.Length > 0 && _produk != null)
            {
                labelPriceProductPanel.FontName = fontName;
                labelPriceProductPanel.CodeProduct = _produk.product_code;
                labelPriceProductPanel.NameProduct = _produk.product_name;
                labelPriceProductPanel.PriceProduct = _produk.selling_price;
                labelPriceProductPanel.LastUpdate = _produk.last_update;                

                labelPriceProductPanel.GenerateLabel();
            }
        }

        private void ResetLabelPrice(bool resetAll = false)
        {            
            labelPriceProductPanel.BackgroundImage = null;

            if (!resetAll) return;

            var index = 0;
            foreach (var panel in _listOfPanelPositionLabel)
            {
                panel.BackgroundImage = labelPriceProductPanel.BackgroundImage;
                _listOfPanelPositionLabel2[index].BackgroundImage = labelPriceProductPanel.BackgroundImage;

                index++;
            }

            foreach (var checkbox in _listOfCheckboxPositionLabel)
            {
                checkbox.Enabled = false;
                checkbox.Checked = false;
            }

            _listOfLabelPriceProduct.Clear();

            txtCodeProduct.Clear();
            txtCodeProduct.Focus();
            txtNameProduct.Clear();
            txtPriceSelling.Text = "0";

            updJumlahPrint.Enabled = false;
            btnPrint.Enabled = false;
            chkSelectAll.Enabled = false;
            chkSelectAll.Checked = false;
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

            this._settingsLabelPrice.name_printer = cmbPrinter.Text;
            this._settingsLabelPrice.batas_atas_row1 = Convert.ToSingle(txtBatasAtasRow1.Text);
            this._settingsLabelPrice.batas_atas_row2 = Convert.ToSingle(txtBatasAtasRow2.Text);
            this._settingsLabelPrice.batas_atas_row3 = Convert.ToSingle(txtBatasAtasRow3.Text);
            this._settingsLabelPrice.batas_atas_row4 = Convert.ToSingle(txtBatasAtasRow4.Text);
            this._settingsLabelPrice.batas_atas_row5 = Convert.ToSingle(txtBatasAtasRow5.Text);
            this._settingsLabelPrice.batas_atas_row6 = Convert.ToSingle(txtBatasAtasRow6.Text);
            this._settingsLabelPrice.batas_atas_row7 = Convert.ToSingle(txtBatasAtasRow7.Text);
            this._settingsLabelPrice.batas_atas_row8 = Convert.ToSingle(txtBatasAtasRow8.Text);

            this._settingsLabelPrice.batas_kiri_column1 = Convert.ToSingle(txtBatasKiriColumn1.Text);
            this._settingsLabelPrice.batas_kiri_column2 = Convert.ToSingle(txtBatasKiriColumn2.Text);
            this._settingsLabelPrice.batas_kiri_column3 = Convert.ToSingle(txtBatasKiriColumn3.Text);
            this._settingsLabelPrice.batas_kiri_column4 = Convert.ToSingle(txtBatasKiriColumn4.Text);

            AppConfigHelper.SaveValue("printerLabelPrice", cmbPrinter.Text, appConfigFile);

            AppConfigHelper.SaveValue("batasAtasRow1LabelPrice", txtBatasAtasRow1.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow2LabelPrice", txtBatasAtasRow2.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow3LabelPrice", txtBatasAtasRow3.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow4LabelPrice", txtBatasAtasRow4.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow5LabelPrice", txtBatasAtasRow5.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow6LabelPrice", txtBatasAtasRow6.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow7LabelPrice", txtBatasAtasRow7.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasAtasRow8LabelPrice", txtBatasAtasRow8.Text, appConfigFile);

            AppConfigHelper.SaveValue("batasKiriColumn1LabelPrice", txtBatasKiriColumn1.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasKiriColumn2LabelPrice", txtBatasKiriColumn2.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasKiriColumn3LabelPrice", txtBatasKiriColumn3.Text, appConfigFile);
            AppConfigHelper.SaveValue("batasKiriColumn4LabelPrice", txtBatasKiriColumn4.Text, appConfigFile);
        }

        /// <summary>
        /// Method untuk mengecek Minimum 1 position label price must selected sebelum diprint
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
                        ResetLabelPrice();
                    }
                    else if (listOfProduct.Count == 1)
                    {
                        this._produk = listOfProduct[0];

                        SetDataProduct(this._produk);
                        PreviewLabelPriceProduct();
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
                    PreviewLabelPriceProduct();
                }
            }
        }        

        public void Ok(object sender, object data)
        {
            if (data is Product) // pencarian product baku
            {
                this._produk = (Product)data;

                SetDataProduct(this._produk);
                PreviewLabelPriceProduct();
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
                MsgHelper.MsgWarning("Minimum one position label price must selected !");
                return;
            }

            if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
            {
                SaveAppConfig();

                PrintDocument printLabelPrice = new PrintDocument();
                printLabelPrice.PrinterSettings.PrinterName = cmbPrinter.Text;
                printLabelPrice.PrinterSettings.Copies = (short)updJumlahPrint.Value;
                printLabelPrice.PrintPage += printLabelPrice_PrintPage;
                printLabelPrice.Print();
            }
        }

        private void printLabelPrice_PrintPage(object sender, PrintPageEventArgs e)
        {
            var labelPriceColumns = 4;
            var labelPricePerPage = 32;

            // Determine printable region for each price label
            var numLines = labelPricePerPage / labelPriceColumns;

            if ((labelPricePerPage % labelPriceColumns) != 0)
            {
                ++numLines;
            }

            var labelPriceArea = new SizeF();

            labelPriceArea.Width = (e.MarginBounds.Width / labelPriceColumns);
            labelPriceArea.Height = (e.MarginBounds.Height / numLines);

            var listOfPosition = new Dictionary<int, PointF>();
            
            // row 1
            listOfPosition.Add(0, new PointF(_settingsLabelPrice.batas_kiri_column1, _settingsLabelPrice.batas_atas_row1));
            listOfPosition.Add(1, new PointF(_settingsLabelPrice.batas_kiri_column2, listOfPosition[0].Y));
            listOfPosition.Add(2, new PointF(_settingsLabelPrice.batas_kiri_column3, listOfPosition[0].Y));
            listOfPosition.Add(3, new PointF(_settingsLabelPrice.batas_kiri_column4, listOfPosition[0].Y));

            // row 2
            listOfPosition.Add(4, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row2));
            listOfPosition.Add(5, new PointF(listOfPosition[1].X, listOfPosition[4].Y));
            listOfPosition.Add(6, new PointF(listOfPosition[2].X, listOfPosition[4].Y));
            listOfPosition.Add(7, new PointF(listOfPosition[3].X, listOfPosition[4].Y));

            // row 3
            listOfPosition.Add(8, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row3));
            listOfPosition.Add(9, new PointF(listOfPosition[1].X, listOfPosition[8].Y));
            listOfPosition.Add(10, new PointF(listOfPosition[2].X, listOfPosition[8].Y));
            listOfPosition.Add(11, new PointF(listOfPosition[3].X, listOfPosition[8].Y));

            // row 4
            listOfPosition.Add(12, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row4));
            listOfPosition.Add(13, new PointF(listOfPosition[1].X, listOfPosition[12].Y));
            listOfPosition.Add(14, new PointF(listOfPosition[2].X, listOfPosition[12].Y));
            listOfPosition.Add(15, new PointF(listOfPosition[3].X, listOfPosition[12].Y));

            // row 5
            listOfPosition.Add(16, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row5));
            listOfPosition.Add(17, new PointF(listOfPosition[1].X, listOfPosition[16].Y));
            listOfPosition.Add(18, new PointF(listOfPosition[2].X, listOfPosition[16].Y));
            listOfPosition.Add(19, new PointF(listOfPosition[3].X, listOfPosition[16].Y));

            // row 6
            listOfPosition.Add(20, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row6));
            listOfPosition.Add(21, new PointF(listOfPosition[1].X, listOfPosition[20].Y));
            listOfPosition.Add(22, new PointF(listOfPosition[2].X, listOfPosition[20].Y));
            listOfPosition.Add(23, new PointF(listOfPosition[3].X, listOfPosition[20].Y));

            // row 7
            listOfPosition.Add(24, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row7));
            listOfPosition.Add(25, new PointF(listOfPosition[1].X, listOfPosition[24].Y));
            listOfPosition.Add(26, new PointF(listOfPosition[2].X, listOfPosition[24].Y));
            listOfPosition.Add(27, new PointF(listOfPosition[3].X, listOfPosition[24].Y));

            // row 8
            listOfPosition.Add(28, new PointF(listOfPosition[0].X, _settingsLabelPrice.batas_atas_row8));
            listOfPosition.Add(29, new PointF(listOfPosition[1].X, listOfPosition[28].Y));
            listOfPosition.Add(30, new PointF(listOfPosition[2].X, listOfPosition[28].Y));
            listOfPosition.Add(31, new PointF(listOfPosition[3].X, listOfPosition[28].Y));

            for (var index = 0; index < labelPricePerPage; index++)
            {
                var isPrint = _listOfCheckboxPositionLabel[index].Checked;
                
                if (isPrint)
                {
                    var position = listOfPosition[index];

                    /*
                    var drawRectangle = new RectangleF(position, labelPriceArea);
                    var labelPriceImageLocation = new PointF(position.X, position.Y);

                    var labelPrice = _listOfPanelPositionLabel[index];

                    labelPriceImageLocation.X += (drawRectangle.Width - labelPrice.BackgroundImage.Width) / 2;

                    e.Graphics.DrawImage(labelPrice.BackgroundImage, labelPriceImageLocation);
                     */

                    using (var brush = new SolidBrush(Color.Black))
                    {
                        try
                        {
                            var nLeft = (int)position.X;
                            var nTop = (int)position.Y;

                            var labelPrice = _listOfLabelPriceProduct[index];
                            DrawString(labelPrice, e.Graphics, brush, nLeft, nTop);
                        }
                        catch
                        {
                        } 
                    }

                }
            }
        }

        private void DrawString(LabelPriceProduct label, Graphics g, SolidBrush brush, int nLeft, int nTop)
        {
            var maxLength = 23;
            var font = new Font("Courier New", 9.5f);

            g.DrawString(string.Format("{0}{1}", 
                    StringHelper.CenterAlignment(label.NameProduct1.Length, maxLength), label.NameProduct1), font, brush, nLeft, nTop);

            if (label.NameProduct2.Length > 0)
            {
                nTop += 15;
                g.DrawString(string.Format("{0}{1}", 
                        StringHelper.CenterAlignment(label.NameProduct2.Length, maxLength), label.NameProduct2), font, brush, nLeft, nTop);
            }

            nTop += 15;
            g.DrawString(string.Format("{0}{1}", 
                    StringHelper.CenterAlignment(label.Barcode.Length, maxLength), label.Barcode), font, brush, nLeft, nTop);

            nTop += 10;
            g.DrawString(string.Format("{0}{1}", 
                    StringHelper.CenterAlignment(3, maxLength - label.price.Length - 5), "Rp."), font, brush, nLeft, nTop + 5);
            g.DrawString(string.Format("{0}{1}", 
                    StringHelper.CenterAlignment(label.price.Length, maxLength - 6), label.price), new Font("Courier New", 14f, FontStyle.Bold), brush, nLeft, nTop + 2);

            if (label.DateUpdate.Length > 0)
            {
                nTop += 20;
                g.DrawString(string.Format("{0}{1}",
                        StringHelper.CenterAlignment(label.DateUpdate.Length, maxLength), label.DateUpdate), font, brush, nLeft, nTop);
            }            
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var checkbox in _listOfCheckboxPositionLabel)
            {
                if (checkbox.Enabled) checkbox.Checked = ((CheckBox)sender).Checked;
            }
        }

        private void FrmPrintLabelPriceProduct_KeyPress(object sender, KeyPressEventArgs e)
        {        
            if (KeyPressHelper.IsEsc(e)) this.Close();
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTransferToPrintList_Click(object sender, EventArgs e)
        {
            var index = 0;

            foreach (var panel in _listOfPanelPositionLabel)
            {
                if (panel.BackgroundImage == null)
                {
                    panel.BackgroundImage = labelPriceProductPanel.BackgroundImage;
                    _listOfPanelPositionLabel2[index].BackgroundImage = labelPriceProductPanel.BackgroundImage;

                    var arrNameProduct = StringHelper.SplitByLength(labelPriceProductPanel.NameProduct, 23).ToList();

                    var labelPrice = new LabelPriceProduct
                    {
                        NameProduct1 = arrNameProduct.Count > 0 ? arrNameProduct[0] : string.Empty,
                        NameProduct2 = arrNameProduct.Count > 1 ? arrNameProduct[1] : string.Empty,
                        Barcode = labelPriceProductPanel.CodeProduct,
                        price = string.Format("{0:N0}", labelPriceProductPanel.PriceProduct),
                        DateUpdate = labelPriceProductPanel.LastUpdate != null ? string.Format("{0:dd-MM-yyyy}", labelPriceProductPanel.LastUpdate) : string.Empty
                    };

                    _listOfLabelPriceProduct.Add(labelPrice);

                    break;
                }          
      
                index++;
            }

            if (index == _listOfCheckboxPositionLabel.Count) return;

            _listOfCheckboxPositionLabel[index].Enabled = true;

            chkSelectAll.Enabled = true;
            updJumlahPrint.Enabled = true;
            btnPrint.Enabled = true;
        }

        private void FrmPrintLabelPriceProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsShortcutKey(Keys.F11, e)) btnTransferToPrintList_Click(sender, new EventArgs());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetLabelPrice(true);
        }
    }
}
