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
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using System.Drawing.Printing;
using Microsoft.Reporting.WinForms;
using ConceptCave.WaitCursor;
using SparkPOS.Helper.UserControl;
using System.IO.Ports;
using GodSharp;
using MultilingualApp;

namespace SparkPOS.App.Settings
{
    public partial class FrmGeneralSupplier : FrmEntryStandard
    {
        private IList<AdvancedTextbox> _listOfTxtHeaderInvoice = new List<AdvancedTextbox>();
        private IList<AdvancedTextbox> _listOfTxtHeaderInvoiceMiniPOS = new List<AdvancedTextbox>();
        private IList<AdvancedTextbox> _listOfTxtFooterInvoiceMiniPOS = new List<AdvancedTextbox>();
        private IList<AdvancedTextbox> _listOfTxtLabelInvoice = new List<AdvancedTextbox>();
        private GeneralSupplier _GeneralSupplier = null;
        private SettingPort _settingPort = null;
        private SettingCustomerDisplay _settingCustomerDisplay = null;

        public FrmGeneralSupplier(string header, GeneralSupplier GeneralSupplier,
            SettingPort settingPort, SettingCustomerDisplay settingCustomerDisplay) : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            
            base.SetButtonSelesaiToClose();
            this._GeneralSupplier = GeneralSupplier;
            this._settingPort = settingPort;
            this._settingCustomerDisplay = settingCustomerDisplay;

            SetInfoPrinter();
            SetInfoPort(_settingPort.portNumber);
            SetInfoCustomerDisplay();
            LoadHeaderInvoice();
            LoadHeaderInvoiceMiniPOS();
            LoadFooterInvoiceMinniPOS();
            LoadLabelInvoice();
            LoadSettingLainnya();

            MainProgram.GlobalLanguageChange(this);
            LanguageHelper.TranslateToolTripTitle(this);
            base.SetHeader(header);
        }

        private void LoadHeaderInvoice()
        {
            _listOfTxtHeaderInvoice.Add(txtHeader1);
            _listOfTxtHeaderInvoice.Add(txtHeader2);
            _listOfTxtHeaderInvoice.Add(txtHeader3);
            _listOfTxtHeaderInvoice.Add(txtHeader4);
            _listOfTxtHeaderInvoice.Add(txtHeader5);

            IHeaderInvoiceBll bll = new HeaderInvoiceBll();
            var listOfHeaderInvoice = bll.GetAll();

            var index = 0;
            foreach (var item in listOfHeaderInvoice)
            {
                var txtHeader = _listOfTxtHeaderInvoice[index];
                txtHeader.Tag = item.header_invoice_id;
                txtHeader.Text = item.description;

                index++;
            }
        }

        private void LoadHeaderInvoiceMiniPOS()
        {
            _listOfTxtHeaderInvoiceMiniPOS.Add(txtHeaderMiniPOS1);
            _listOfTxtHeaderInvoiceMiniPOS.Add(txtHeaderMiniPOS2);
            _listOfTxtHeaderInvoiceMiniPOS.Add(txtHeaderMiniPOS3);
            _listOfTxtHeaderInvoiceMiniPOS.Add(txtHeaderMiniPOS4);
            _listOfTxtHeaderInvoiceMiniPOS.Add(txtHeaderMiniPOS5);

            IHeaderInvoiceMiniPosBll bll = new HeaderInvoiceMiniPosBll();
            var listOfHeaderInvoice = bll.GetAll();

            var index = 0;
            foreach (var item in listOfHeaderInvoice)
            {
                var txtHeader = _listOfTxtHeaderInvoiceMiniPOS[index];
                txtHeader.Tag = item.header_invoice_id;
                txtHeader.Text = item.description;

                index++;
            }
        }

        private void LoadFooterInvoiceMinniPOS()
        {
            _listOfTxtFooterInvoiceMiniPOS.Add(txtFooterMiniPOS1);
            _listOfTxtFooterInvoiceMiniPOS.Add(txtFooterMiniPOS2);
            _listOfTxtFooterInvoiceMiniPOS.Add(txtFooterMiniPOS3);

            IFooterInvoiceMiniPosBll bll = new FooterInvoiceMiniPosBll();
            var listOfFooterInvoice = bll.GetAll();

            var index = 0;
            foreach (var item in listOfFooterInvoice)
            {
                var txtFooter = _listOfTxtFooterInvoiceMiniPOS[index];
                txtFooter.Tag = item.footer_invoice_id;
                txtFooter.Text = item.description;

                index++;
            }
        }

        private void LoadLabelInvoice()
        {
            _listOfTxtLabelInvoice.Add(txtFrom1);
            _listOfTxtLabelInvoice.Add(txtFrom2);
            _listOfTxtLabelInvoice.Add(txtFrom3);

            ILabelInvoiceBll bll = new LabelInvoiceBll();
            var listOfLabelInvoice = bll.GetAll();

            var index = 0;
            foreach (var item in listOfLabelInvoice)
            {
                var txtFrom = _listOfTxtLabelInvoice[index];
                txtFrom.Tag = item.invoice_label_id;
                txtFrom.Text = item.description;

                index++;
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

        private void SetInfoPort(string defaultPort)
        {
            cmbPort.Items.Clear();
            foreach (var port in SerialPort.GetPortNames())
            {
                cmbPort.Items.Add(port);
            }

            if (cmbPort.Items.Count == 0)
            {
                for (int i = 1; i < 10; i++)
                {
                    cmbPort.Items.Add(string.Format("COM{0}", i.ToString()));
                }
            }

            if (defaultPort.Length > 0)
                cmbPort.Text = defaultPort;
            else
            {
                if (cmbPort.Items.Count > 0)
                    cmbPort.SelectedIndex = 0;
            }
        }

        private void SetInfoCustomerDisplay()
        {
            chkIsActiveCustomerDisplay.Checked = _settingCustomerDisplay.is_active_customer_display;
            txtOpeningSentenceRow1.Text = _settingCustomerDisplay.opening_sentence_line1;
            txtOpeningSentenceRow2.Text = _settingCustomerDisplay.opening_sentence_line2;
            txtKalimatPenutupRow1.Text = _settingCustomerDisplay.closing_sentence_line1;
            txtKalimatPenutupRow2.Text = _settingCustomerDisplay.closing_sentence_line2;
            updTampilKalimatPenutup.Value = _settingCustomerDisplay.delay_display_closing_sentence;
        }

        private void SetInfoPrinter()
        {
            // setting general
            LoadPrinter(this._GeneralSupplier.name_printer);
            chkPrintAutomatic.Checked = this._GeneralSupplier.is_auto_print;

            switch (this._GeneralSupplier.type_printer)
            {
                case TypePrinter.DotMatrix:
                    rdoTypePrinterDotMatrix.Checked = true;
                    break;

                case TypePrinter.MiniPOS:
                    rdoTypePrinterMiniPOS.Checked = true;
                    break;

                default:
                    rdoTypePrinterInkJet.Checked = true;
                    break;
            }

            // setting khusus printer mini pos            
            chkPrintCustomer.Checked = _GeneralSupplier.is_print_customer;
            txtJumlahKarakter.Text = _GeneralSupplier.jumlah_karakter.ToString();
            txtJumlahGulung.Text = _GeneralSupplier.jumlah_gulung.ToString();

            if (rdoTypePrinterMiniPOS.Checked)
            {
                chkFontSize.Checked = _GeneralSupplier.size_font > 0;
                txtFontSize.Text = _GeneralSupplier.size_font.ToString();
                txtFontSize.Enabled = chkFontSize.Checked;

                chkAutocut.Checked = _GeneralSupplier.is_autocut;
                chkOpenCashDrawer.Checked = _GeneralSupplier.is_open_cash_drawer;
            }            
        }

        private void LoadSettingLainnya()
        {
            chkShowInfoMinimalStockProduct.Checked = _GeneralSupplier.is_show_minimal_stock;
            chkCustomerWajibDiisi.Checked = _GeneralSupplier.is_customer_required;
            chkPrintKeteranganInvoice.Checked = _GeneralSupplier.is_print_keterangan_nota;
            chkStockProductBolehMinus.Checked = _GeneralSupplier.is_negative_stock_allowed_for_products;
            chkFokusKeColumnJumlah.Checked = _GeneralSupplier.is_focus_on_inputting_quantity_column;
            chkUpdatePriceSelling.Checked = _GeneralSupplier.is_update_selling_price;
            chkSingkatPenulisanOngkir.Checked = _GeneralSupplier.is_singkat_penulisan_ongkir;
            chkShowKeteranganOptionalItemSelling.Checked = _GeneralSupplier.is_show_additional_sales_item_information;
            txtKeteranganOptionalItemSelling.Text = _GeneralSupplier.additional_sales_item_information;
        }

        protected override void Save()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {                
                // save settings lokal (app.config)
                SaveSettingsLokal();

                // save settings global (databases)
                SaveSettingsGlobal();

                // save header invoice
                SaveHeaderInvoice();

                // save header invoice minipos
                SaveHeaderInvoiceMiniPOS();

                // save footer invoice minipos
                SaveFooterInvoiceMiniPOS();

                // save label invoice
                SaveLabelInvoice();

                this.Close();    
            }            
        }

        /// <summary>
        /// Save settings application di masing-masing pc (app.config)
        /// </summary>
        private void SaveSettingsLokal()
        {
            var appConfigFile = string.Format("{0}\\SparkPOS.exe.config", Utils.GetAppPath());

            _GeneralSupplier.name_printer = cmbPrinter.Text;
            _GeneralSupplier.is_auto_print = chkPrintAutomatic.Checked;

            var typePrinter = TypePrinter.InkJet;

            if (rdoTypePrinterDotMatrix.Checked)
                typePrinter = TypePrinter.DotMatrix;
            else if (rdoTypePrinterMiniPOS.Checked)
                typePrinter = TypePrinter.MiniPOS;

            _GeneralSupplier.type_printer = typePrinter;
            _GeneralSupplier.is_print_customer = chkPrintCustomer.Checked;
            _GeneralSupplier.is_show_minimal_stock = chkShowInfoMinimalStockProduct.Checked;
            _GeneralSupplier.is_customer_required = chkCustomerWajibDiisi.Checked;
            _GeneralSupplier.is_print_keterangan_nota = chkPrintKeteranganInvoice.Checked;
            _GeneralSupplier.is_singkat_penulisan_ongkir = chkSingkatPenulisanOngkir.Checked;
            _GeneralSupplier.jumlah_karakter = Convert.ToInt32(txtJumlahKarakter.Text);
            _GeneralSupplier.jumlah_gulung = Convert.ToInt32(txtJumlahGulung.Text);
            _GeneralSupplier.size_font = Convert.ToInt32(txtFontSize.Text);
            _GeneralSupplier.is_autocut = chkAutocut.Checked;
            _GeneralSupplier.is_open_cash_drawer = chkOpenCashDrawer.Checked;

            // save info printer
            AppConfigHelper.SaveValue("printerName", cmbPrinter.Text, appConfigFile);
            AppConfigHelper.SaveValue("isAutoPrinter", chkPrintAutomatic.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("type_printer", Convert.ToString((int)typePrinter), appConfigFile);

            // save info printer mini pos
            AppConfigHelper.SaveValue("isPrintCustomer", chkPrintCustomer.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("isShowMinimalStock", chkShowInfoMinimalStockProduct.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("isCustomerRequired", chkCustomerWajibDiisi.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("isPrintKeteranganInvoice", chkPrintKeteranganInvoice.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("isSingkatPenulisanOngkir", chkSingkatPenulisanOngkir.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("jumlahKarakter", txtJumlahKarakter.Text, appConfigFile);
            AppConfigHelper.SaveValue("jumlahGulung", txtJumlahGulung.Text, appConfigFile);
            AppConfigHelper.SaveValue("FontSize", txtFontSize.Text, appConfigFile);
            AppConfigHelper.SaveValue("isAutocut", chkAutocut.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("autocutCode", _GeneralSupplier.autocut_code, appConfigFile);
            AppConfigHelper.SaveValue("isOpenCashDrawer", chkOpenCashDrawer.Checked.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("openCashDrawerCode", _GeneralSupplier.open_cash_drawer_code, appConfigFile);

            // save setting port
            _settingPort.portNumber = cmbPort.Text;
            AppConfigHelper.SaveValue("portNumber", cmbPort.Text, appConfigFile);

            // save setting customer display
            _settingCustomerDisplay.is_active_customer_display = chkIsActiveCustomerDisplay.Checked;
            _settingCustomerDisplay.opening_sentence_line1 = txtOpeningSentenceRow1.Text;
            _settingCustomerDisplay.opening_sentence_line2 = txtOpeningSentenceRow2.Text;
            _settingCustomerDisplay.closing_sentence_line1 = txtKalimatPenutupRow1.Text;
            _settingCustomerDisplay.closing_sentence_line2 = txtKalimatPenutupRow2.Text;
            _settingCustomerDisplay.delay_display_closing_sentence = (int)updTampilKalimatPenutup.Value;

            AppConfigHelper.SaveValue("isActiveCustomerDisplay", _settingCustomerDisplay.is_active_customer_display.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("customerDisplayOpeningSentenceLine1", _settingCustomerDisplay.opening_sentence_line1, appConfigFile);
            AppConfigHelper.SaveValue("customerDisplayOpeningSentenceLine2", _settingCustomerDisplay.opening_sentence_line2, appConfigFile);
            AppConfigHelper.SaveValue("customerDisplayClosingSentenceLine1", _settingCustomerDisplay.closing_sentence_line1, appConfigFile);
            AppConfigHelper.SaveValue("customerDisplayClosingSentenceLine2", _settingCustomerDisplay.closing_sentence_line2, appConfigFile);
            AppConfigHelper.SaveValue("customerDisplayDelayDisplayClosingSentence", _settingCustomerDisplay.delay_display_closing_sentence.ToString(), appConfigFile);
        }

        /// <summary>
        /// Save settings application di database (global)
        /// </summary>
        private void SaveSettingsGlobal()
        {
            ISettingApplicationBll settingApplicationBll = new SettingApplicationBll();
            var settingApplication = settingApplicationBll.GetAll().SingleOrDefault();

            if (settingApplication != null)
            {
                settingApplication.is_update_selling_price_of_master_products = chkUpdatePriceSelling.Checked;
                settingApplication.is_negative_stock_allowed_for_products = chkStockProductBolehMinus.Checked;
                settingApplication.is_focus_on_inputting_quantity_column = chkFokusKeColumnJumlah.Checked;
                settingApplication.is_show_additional_sales_item_information = chkShowKeteranganOptionalItemSelling.Checked;
                settingApplication.additional_sales_item_information = txtKeteranganOptionalItemSelling.Text;

                var result = settingApplicationBll.Update(settingApplication);
                if (result > 0)
                {
                    _GeneralSupplier.is_update_selling_price = chkUpdatePriceSelling.Checked;
                    _GeneralSupplier.is_negative_stock_allowed_for_products = chkStockProductBolehMinus.Checked;
                    _GeneralSupplier.is_focus_on_inputting_quantity_column = chkFokusKeColumnJumlah.Checked;
                    _GeneralSupplier.is_show_additional_sales_item_information = chkShowKeteranganOptionalItemSelling.Checked;
                    _GeneralSupplier.additional_sales_item_information = txtKeteranganOptionalItemSelling.Text;
                }
            }
        }

        private void SaveHeaderInvoice()
        {            
            IHeaderInvoiceBll headerInvoiceBll = new HeaderInvoiceBll();

            var index = 0;
            foreach (var item in _listOfTxtHeaderInvoice)
            {
                var headerInvoice = new HeaderInvoice
                {
                    header_invoice_id = item.Tag.ToString(),
                    description = item.Text
                };

                var result = headerInvoiceBll.Update(headerInvoice);
                if (result > 0)
                {
                    _GeneralSupplier.list_of_header_nota[index].header_invoice_id = headerInvoice.header_invoice_id;
                    _GeneralSupplier.list_of_header_nota[index].description = headerInvoice.description;
                }

                index++;
            }
        }

        private void SaveHeaderInvoiceMiniPOS()
        {
            IHeaderInvoiceMiniPosBll headerInvoiceBll = new HeaderInvoiceMiniPosBll();

            var index = 0;
            foreach (var item in _listOfTxtHeaderInvoiceMiniPOS)
            {
                var headerInvoice = new HeaderInvoiceMiniPos
                {
                    header_invoice_id = item.Tag.ToString(),
                    description = item.Text
                };

                var result = headerInvoiceBll.Update(headerInvoice);
                if (result > 0)
                {
                    _GeneralSupplier.list_of_header_nota_mini_pos[index].header_invoice_id = headerInvoice.header_invoice_id;
                    _GeneralSupplier.list_of_header_nota_mini_pos[index].description = headerInvoice.description;
                }

                index++;
            }
        }

        private void SaveFooterInvoiceMiniPOS()
        {
            IFooterInvoiceMiniPosBll footerInvoiceBll = new FooterInvoiceMiniPosBll();

            var index = 0;
            foreach (var item in _listOfTxtFooterInvoiceMiniPOS)
            {
                var footerInvoice = new FooterInvoiceMiniPos
                {
                    footer_invoice_id = item.Tag.ToString(),
                    description = item.Text
                };

                var result = footerInvoiceBll.Update(footerInvoice);
                if (result > 0)
                {
                    _GeneralSupplier.list_of_footer_nota_mini_pos[index].footer_invoice_id = footerInvoice.footer_invoice_id;
                    _GeneralSupplier.list_of_footer_nota_mini_pos[index].description = footerInvoice.description;
                }

                index++;
            }
        }

        private void SaveLabelInvoice()
        {
            ILabelInvoiceBll labelInvoiceBll = new LabelInvoiceBll();

            var index = 0;
            foreach (var item in _listOfTxtLabelInvoice)
            {
                var labelInvoice = new LabelInvoice
                {
                    invoice_label_id = item.Tag.ToString(),
                    description = item.Text
                };

                var result = labelInvoiceBll.Update(labelInvoice);
                if (result > 0)
                {
                    _GeneralSupplier.list_of_label_nota[index].invoice_label_id = labelInvoice.invoice_label_id;
                    _GeneralSupplier.list_of_label_nota[index].description = labelInvoice.description;
                }

                index++;
            }
        }

        private void btnLihatContohInvoiceSales_Click(object sender, EventArgs e)
        {
            var jualProductId = string.Empty;

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                IPrintInvoiceBll bll = new PrintInvoiceSampleBll();
                var listOfSelling = bll.GetInvoiceSales(jualProductId);

                if (listOfSelling.Count > 0)
                {
                    var reportDataSource = new ReportDataSource
                    {
                        Name = "InvoiceSales",
                        Value = listOfSelling
                    };
                    
                    var parameters = new List<ReportParameter>();
                    var index = 1;

                    foreach (var txtHeaderInvoice in _listOfTxtHeaderInvoice)
                    {
                        var paramName = string.Format("header{0}", index);
                        parameters.Add(new ReportParameter(paramName, txtHeaderInvoice.Text));

                        index++;
                    }

                    foreach (var item in listOfSelling)
                    {
                        item.from_label1 = txtFrom1.Text;
                        item.from_label2 = txtFrom2.Text;
                        item.from_label3 = txtFrom3.Text;

                        if (_GeneralSupplier.is_singkat_penulisan_ongkir)
                        {
                            item.shipping_cost /= 1000;
                        }
                    }

                    var dt = DateTime.Now;
                    var cityAndDate = string.Format("{0}, {1}", MainProgram.profil.city, dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);

                    parameters.Add(new ReportParameter("city", cityAndDate));
                    parameters.Add(new ReportParameter("footer", MainProgram.user.name_user));

                    var frmPreviewReport = new FrmPreviewReport("Sales invoice example", "RvInvoiceSalesProductLabel", reportDataSource, parameters);
                    frmPreviewReport.ShowDialog();
                }
            }            
        }

        private void chkPrinterMiniPOS_CheckedChanged(object sender, EventArgs e)
        {
            txtJumlahKarakter.Enabled = ((CheckBox)sender).Checked;
            txtJumlahGulung.Enabled = txtJumlahKarakter.Enabled;

            if (txtJumlahKarakter.Enabled)
            {
                txtJumlahKarakter.BackColor = Color.White;
                txtJumlahGulung.BackColor = Color.White;
            }
        }

        private void btnLihatContohInvoiceSalesMiniPOS_Click(object sender, EventArgs e)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                var parameters = new List<ReportParameter>();
                var index = 1;

                foreach (var txtHeaderInvoice in _listOfTxtHeaderInvoiceMiniPOS)
                {
                    var paramName = string.Format("header{0}", index);
                    parameters.Add(new ReportParameter(paramName, txtHeaderInvoice.Text));

                    index++;
                }

                index = 1;
                foreach (var txtFooterInvoice in _listOfTxtFooterInvoiceMiniPOS)
                {
                    var paramName = string.Format("footer{0}", index);
                    parameters.Add(new ReportParameter(paramName, txtFooterInvoice.Text));

                    index++;
                }

                var reportName = "RvInvoiceSalesMiniPOSTanpaCustomer";

                if (chkPrintCustomer.Checked)
                    reportName = "RvInvoiceSalesMiniPOS";

                var frmPreviewReport = new FrmPreviewReport("Example of a MINI POS Sales Invoice", reportName, new ReportDataSource(), parameters);
                frmPreviewReport.ShowDialog();
            }
        }

        private void rdoTypePrinterInkJet_CheckedChanged(object sender, EventArgs e)
        {
            txtJumlahKarakter.Enabled = false;
            txtJumlahGulung.Enabled = false;
            chkFontSize.Enabled = false;
            chkFontSize.Checked = false;  
            txtFontSize.Enabled = false;
            txtFontSize.Text = "0";

            chkAutocut.Enabled = false;
            chkAutocut.Checked = false;
            chkOpenCashDrawer.Enabled = false;
            chkOpenCashDrawer.Checked = false;

            btnShowAutocutCode.Enabled = false;
            btnShowOpenCashDrawerCode.Enabled = false;
        }

        private void rdoTypePrinterDotMatrix_CheckedChanged(object sender, EventArgs e)
        {
            rdoTypePrinterInkJet_CheckedChanged(sender, e);
        }

        private void rdoTypePrinterMiniPOS_CheckedChanged(object sender, EventArgs e)
        {
            txtJumlahKarakter.Enabled = true;
            txtJumlahKarakter.BackColor = Color.White;

            txtJumlahGulung.Enabled = true;
            txtJumlahGulung.BackColor = Color.White;

            chkFontSize.Enabled = true;
            chkFontSize.Checked = _GeneralSupplier.size_font > 0;

            chkAutocut.Enabled = true;
            chkAutocut.Checked = _GeneralSupplier.is_autocut;

            chkOpenCashDrawer.Enabled = true;
            chkOpenCashDrawer.Checked = _GeneralSupplier.is_open_cash_drawer;
        }

        private void chkFontSize_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            txtFontSize.Enabled = chk.Checked;

            txtFontSize.Text = "0";
            if (chk.Checked)
                txtFontSize.Text = _GeneralSupplier.size_font.ToString();
        }

        private void chkShowKeteranganOptionalItemSelling_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            txtKeteranganOptionalItemSelling.Enabled = chk.Checked;

            txtKeteranganOptionalItemSelling.Text = "Description";
            if (chk.Checked)
                txtKeteranganOptionalItemSelling.Text = _GeneralSupplier.additional_sales_item_information;
        }

        private void btnShowAutocutCode_Click(object sender, EventArgs e)
        {
            var frm = new FrmEntryCustomeCode("Edit Code Autocut", _GeneralSupplier, true);
            frm.ShowDialog();
        }

        private void btnShowOpenCashDrawerCode_Click(object sender, EventArgs e)
        {
            var frm = new FrmEntryCustomeCode("Edit Code Open Cash Drawer", _GeneralSupplier, false);
            frm.ShowDialog();
        }

        private void chkAutocut_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            btnShowAutocutCode.Enabled = chk.Checked;
        }

        private void chkOpenCashDrawer_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            btnShowOpenCashDrawerCode.Enabled = chk.Checked;
        }

        private void chkIsActiveCustomerDisplay_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            cmbPort.Enabled = chk.Checked;
            btnTesKoneksi.Enabled = chk.Checked;
            grpOpeningSentence.Enabled = chk.Checked;
            grpKalimatPenutup.Enabled = chk.Checked;
        }

        private void btnTesKoneksi_Click(object sender, EventArgs e)
        {
            const int MAX_LENGTH = 20;

            var appName = "SparkPOS Server";
            var version = string.Format("v{0}", MainProgram.currentVersion);

            var displayLine1 = string.Format("{0}{1}", StringHelper.CenterAlignment(appName.Length, MAX_LENGTH), appName);
            var displayLine2 = string.Format("{0}{1}", StringHelper.CenterAlignment(version.Length, MAX_LENGTH), version);

            System.Diagnostics.Debug.Print("displayLine1: {0}", displayLine1);
            System.Diagnostics.Debug.Print("displayLine2: {0}", displayLine2);

            if (!Utils.IsRunningUnderIDE())
            {
                GodSerialPort serialPort = null;

                if (!GodSerialPortHelper.IsConnected(serialPort, _settingPort))
                {
                    MsgHelper.MsgWarning("Connection to customer display, please try another port.");
                    return;
                }

                GodSerialPortHelper.SendStringToCustomerDisplay(displayLine1, displayLine2, serialPort);
            }
        }

        private void grpMiniPOS_Enter(object sender, EventArgs e)
        {

        }
    }
}
