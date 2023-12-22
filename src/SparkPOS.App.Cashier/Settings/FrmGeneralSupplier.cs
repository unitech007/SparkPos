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
using ConceptCave.WaitCursor;
using SparkPOS.Helper.UserControl;
using System.Drawing.Printing;
using System.IO.Ports;
using GodSharp;
using MultilingualApp;

namespace SparkPOS.App.Cashier.Settings
{
    public partial class FrmGeneralSupplier : FrmEntryStandard
    {        
        private IList<AdvancedTextbox> _listOfTxtHeaderInvoice = new List<AdvancedTextbox>();
        private IList<AdvancedTextbox> _listOfTxtFooterInvoice = new List<AdvancedTextbox>();
        private GeneralSupplier _GeneralSupplier = null;
        private SettingPort _settingPort = null;
        private SettingCustomerDisplay _settingCustomerDisplay = null;

        public FrmGeneralSupplier(string header, GeneralSupplier GeneralSupplier, 
            SettingPort settingPort, SettingCustomerDisplay settingCustomerDisplay) : base()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._GeneralSupplier = GeneralSupplier;
            this._settingPort = settingPort;
            this._settingCustomerDisplay = settingCustomerDisplay;

            SetInfoPrinter();
            SetInfoPort(_settingPort.portNumber);
            SetInfoCustomerDisplay();
            LoadHeaderInvoice();
            LoadFooterInvoice();
            MainProgram.GlobalLanguageChange(this);
            LanguageHelper.TranslateToolTripTitle(this);
        }

        private void LoadHeaderInvoice()
        {
            _listOfTxtHeaderInvoice.Add(txtHeaderMiniPOS1);
            _listOfTxtHeaderInvoice.Add(txtHeaderMiniPOS2);
            _listOfTxtHeaderInvoice.Add(txtHeaderMiniPOS3);
            _listOfTxtHeaderInvoice.Add(txtHeaderMiniPOS4);
            _listOfTxtHeaderInvoice.Add(txtHeaderMiniPOS5);

            IHeaderInvoiceMiniPosBll bll = new HeaderInvoiceMiniPosBll();
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

        private void LoadFooterInvoice()
        {
            _listOfTxtFooterInvoice.Add(txtFooterMiniPOS1);
            _listOfTxtFooterInvoice.Add(txtFooterMiniPOS2);
            _listOfTxtFooterInvoice.Add(txtFooterMiniPOS3);

            IFooterInvoiceMiniPosBll bll = new FooterInvoiceMiniPosBll();
            var listOfFooterInvoice = bll.GetAll();

            var index = 0;
            foreach (var item in listOfFooterInvoice)
            {
                var txtFooter = _listOfTxtFooterInvoice[index];
                txtFooter.Tag = item.footer_invoice_id;
                txtFooter.Text = item.description;

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
                    break;
            }

            // setting khusus printer mini pos            
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

        protected override void Save()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                // save settings lokal (app.config)
                SaveSettingsLokal();

                // save header invoice
                SaveHeaderInvoice();

                // save footer invoice minipos
                SaveFooterInvoice();

                this.Close();    
            }            
        }

        /// <summary>
        /// Save settings application di masing-masing pc (app.config)
        /// </summary>
        private void SaveSettingsLokal()
        {
            var appConfigFile = string.Format("{0}\\SparkPOSCashier.exe.config", Utils.GetAppPath());

            _GeneralSupplier.name_printer = cmbPrinter.Text;
            _GeneralSupplier.is_auto_print = chkPrintAutomatic.Checked;

            var typePrinter = TypePrinter.InkJet;

            if (rdoTypePrinterDotMatrix.Checked)
                typePrinter = TypePrinter.DotMatrix;
            else if (rdoTypePrinterMiniPOS.Checked)
                typePrinter = TypePrinter.MiniPOS;

            _GeneralSupplier.type_printer = typePrinter;
            _GeneralSupplier.jumlah_karakter = Convert.ToInt32(txtJumlahKarakter.Text);
            _GeneralSupplier.jumlah_gulung = Convert.ToInt32(txtJumlahGulung.Text);
            _GeneralSupplier.size_font = Convert.ToInt32(txtFontSize.Text);
            _GeneralSupplier.is_autocut = chkAutocut.Checked;
            _GeneralSupplier.is_open_cash_drawer = chkOpenCashDrawer.Checked;

            // save info printer
            AppConfigHelper.SaveValue("printerName", _GeneralSupplier.name_printer, appConfigFile);
            AppConfigHelper.SaveValue("isAutoPrinter", _GeneralSupplier.is_auto_print.ToString(), appConfigFile);
            AppConfigHelper.SaveValue("type_printer", Convert.ToString((int)typePrinter), appConfigFile);

            // save info printer mini pos
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

        private void SaveHeaderInvoice()
        {
            IHeaderInvoiceMiniPosBll headerInvoiceBll = new HeaderInvoiceMiniPosBll();

            var index = 0;
            foreach (var item in _listOfTxtHeaderInvoice)
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

        private void SaveFooterInvoice()
        {
            IFooterInvoiceMiniPosBll footerInvoiceBll = new FooterInvoiceMiniPosBll();

            var index = 0;
            foreach (var item in _listOfTxtFooterInvoice)
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

        private void rdoTypePrinterDotMatrix_CheckedChanged(object sender, EventArgs e)
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

            groupBox4.Enabled = false;            
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

            groupBox4.Enabled = true;

        }

        private void chkFontSize_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            txtFontSize.Enabled = chk.Checked;

            txtFontSize.Text = "0";
            if (chk.Checked)
                txtFontSize.Text = _GeneralSupplier.size_font.ToString();
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

        private void btnTesKoneksi_Click(object sender, EventArgs e)
        {
            const int MAX_LENGTH = 20;

            var appName = "SparkPOS Cashier";
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

        private void chkIsActiveCustomerDisplay_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            cmbPort.Enabled = chk.Checked;
            btnTesKoneksi.Enabled = chk.Checked;
            grpOpeningSentence.Enabled = chk.Checked;
            grpKalimatPenutup.Enabled = chk.Checked;
        }
    }
}
