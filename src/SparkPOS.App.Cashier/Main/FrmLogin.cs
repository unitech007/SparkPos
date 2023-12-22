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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using log4net;
using ConceptCave.WaitCursor;

using SparkPOS.Helper;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Model;
using System.IO.Ports;

namespace SparkPOS.App.Cashier.Main
{    
    public partial class FrmLogin : Form
    {
        private ILog _log;
        private string _appConfigFile = string.Format("{0}\\SparkPOSCashier.exe.config", Utils.GetAppPath());

        public FrmLogin()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            _log = MainProgram.log;
            
            LoadAppConfig();
            MainProgram.GlobalLanguageChange(this);
        }

        private void LoadAppConfig()
        {            
            txtServer.Text = AppConfigHelper.GetValue("server", _appConfigFile);

            if (Utils.IsRunningUnderIDE()) // mode debug, set user dan password default untuk development
            {
                txtUserName.Text = "admin";
                txtPassword.Text = "admin";
            }

        }

        private void SaveAppConfig()
        {            
            AppConfigHelper.SaveValue("server", txtServer.Text, _appConfigFile);
        }

        private void SetProfil()
        {
            IProfilBll profilBll = new ProfilBll(_log);
            MainProgram.profil = profilBll.GetProfil();
        }

        private void SetGeneralSupplier()
        {
            // set settings lokal (setting di save di file app.config)            
            MainProgram.GeneralSupplier = new GeneralSupplier();
            MainProgram.GeneralSupplier.name_printer = AppConfigHelper.GetValue("printerName", _appConfigFile);
            MainProgram.GeneralSupplier.is_auto_print = AppConfigHelper.GetValue("isAutoPrinter", _appConfigFile).ToLower() == "true" ? true : false;

            // set info printer mini pos
            var jumlahKarakter = AppConfigHelper.GetValue("jumlahKarakter", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("jumlahKarakter", _appConfigFile)) : 40;
            var jumlahGulung = AppConfigHelper.GetValue("jumlahGulung", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("jumlahGulung", _appConfigFile)) : 5;
            var FontSize = AppConfigHelper.GetValue("FontSize", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("FontSize", _appConfigFile)) : 0;

            MainProgram.GeneralSupplier.is_autocut = AppConfigHelper.GetValue("isAutocut", _appConfigFile, "false").ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.autocut_code = AppConfigHelper.GetValue("autocutCode", _appConfigFile, "27,112,0,75,250");

            MainProgram.GeneralSupplier.is_open_cash_drawer = AppConfigHelper.GetValue("isOpenCashDrawer", _appConfigFile, "false").ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.open_cash_drawer_code = AppConfigHelper.GetValue("openCashDrawerCode", _appConfigFile, "27,112,0,25,250");

            MainProgram.GeneralSupplier.type_printer = AppConfigHelper.GetValue("type_printer", _appConfigFile).Length > 0 ? (TypePrinter)Convert.ToInt32(AppConfigHelper.GetValue("type_printer", _appConfigFile)) : TypePrinter.MiniPOS;
            MainProgram.GeneralSupplier.jumlah_karakter = jumlahKarakter;
            MainProgram.GeneralSupplier.jumlah_gulung = jumlahGulung;
            MainProgram.GeneralSupplier.size_font = FontSize;

            MainProgram.GeneralSupplier.default_ppn = Convert.ToDouble(AppConfigHelper.GetValue("defaultPPN", _appConfigFile, "0"));

            // set settings global (setting disave di database)
            ISettingApplicationBll settingApplicationBll = new SettingApplicationBll();
            var settingApplication = settingApplicationBll.GetAll().SingleOrDefault();

            if (settingApplication != null)
            {
                MainProgram.GeneralSupplier.is_negative_stock_allowed_for_products = settingApplication.is_negative_stock_allowed_for_products;
                MainProgram.GeneralSupplier.is_focus_on_inputting_quantity_column = settingApplication.is_focus_on_inputting_quantity_column;
                MainProgram.GeneralSupplier.is_show_additional_sales_item_information = settingApplication.is_show_additional_sales_item_information;
                MainProgram.GeneralSupplier.additional_sales_item_information = settingApplication.additional_sales_item_information;
            }

            // set header invoice
            IHeaderInvoiceBll headerInvoiceBll = new HeaderInvoiceBll();
            MainProgram.GeneralSupplier.list_of_header_nota = headerInvoiceBll.GetAll();

            // set header invoice minipos
            IHeaderInvoiceMiniPosBll headerInvoiceMiniPosBll = new HeaderInvoiceMiniPosBll();
            MainProgram.GeneralSupplier.list_of_header_nota_mini_pos = headerInvoiceMiniPosBll.GetAll();

            // set footer invoice minipos
            IFooterInvoiceMiniPosBll footerInvoiceMiniPosBll = new FooterInvoiceMiniPosBll();
            MainProgram.GeneralSupplier.list_of_footer_nota_mini_pos = footerInvoiceMiniPosBll.GetAll();

            // set label invoice
            ILabelInvoiceBll labelInvoiceBll = new LabelInvoiceBll();
            MainProgram.GeneralSupplier.list_of_label_nota = labelInvoiceBll.GetAll();
        }

        private void SetSettingPort()
        {
            MainProgram.settingPort = new SettingPort();
            MainProgram.settingPort.portNumber = AppConfigHelper.GetValue("portNumber", _appConfigFile, "COM1");
            MainProgram.settingPort.baudRate = Convert.ToInt32(AppConfigHelper.GetValue("baudRate", _appConfigFile, "9600"));
            MainProgram.settingPort.parity = (Parity)Convert.ToInt32(AppConfigHelper.GetValue("parity", _appConfigFile, "1"));
            MainProgram.settingPort.dataBits = Convert.ToInt32(AppConfigHelper.GetValue("dataBits", _appConfigFile, "8"));
            MainProgram.settingPort.stopBits = (StopBits)Convert.ToInt32(AppConfigHelper.GetValue("stopBits", _appConfigFile, "1"));
        }

        private void SetSettingCustomerDisplay()
        {
            MainProgram.settingCustomerDisplay = new SettingCustomerDisplay();
            MainProgram.settingCustomerDisplay.is_active_customer_display = AppConfigHelper.GetValue("isActiveCustomerDisplay", _appConfigFile, "false").ToLower() == "true" ? true : false;
            MainProgram.settingCustomerDisplay.opening_sentence_line1 = AppConfigHelper.GetValue("customerDisplayOpeningSentenceLine1", _appConfigFile, "Thank you for being here");
            MainProgram.settingCustomerDisplay.opening_sentence_line2 = AppConfigHelper.GetValue("customerDisplayOpeningSentenceLine2", _appConfigFile, "KR Software");
            MainProgram.settingCustomerDisplay.closing_sentence_line1 = AppConfigHelper.GetValue("customerDisplayClosingSentenceLine1", _appConfigFile, "Thank you");
            MainProgram.settingCustomerDisplay.closing_sentence_line2 = AppConfigHelper.GetValue("customerDisplayClosingSentenceLine2", _appConfigFile, "Welcome back");
            MainProgram.settingCustomerDisplay.delay_display_closing_sentence = Convert.ToInt32(AppConfigHelper.GetValue("customerDisplayDelayDisplayClosingSentence", _appConfigFile, "5"));
        }

        private void SetSettingLebarColumnTabelTransactions()
        {
            MainProgram.settingLebarColumnTabelTransactions = new SettingLebarColumnTabelTransactions();
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_no = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnNo", _appConfigFile, "30"));
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_code_produk = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnCodeProduct", _appConfigFile, "190"));
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_name_produk = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnNameProduct", _appConfigFile, "720"));
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_keterangan = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnKeterangan", _appConfigFile, "200"));
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_jumlah = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnJumlah", _appConfigFile, "75"));
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_diskon = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnDiskon", _appConfigFile, "75"));
            MainProgram.settingLebarColumnTabelTransactions.lebar_column_harga = Convert.ToInt32(AppConfigHelper.GetValue("lebarColumnPrice", _appConfigFile, "120"));
        }

        /// <summary>
        /// Load data card untuk payment via card
        /// </summary>
        private void LoadCard()
        {
            ICardBll bll = new CardBll(_log);
            MainProgram.listOfCard = bll.GetAll();
        }

        private void SaveSaldoAwal(string userId, int saldoAwal)
        {
            ICashierMachineBll mesinBll = new CashierMachineBll(_log);

            var obj = new CashierMachine
            {
                user_id = userId,
                starting_balance = saldoAwal
            };

            var result = mesinBll.Save(obj);
            if (result > 0)
                MainProgram.mesinId = obj.machine_id;
        }

        private bool ExecSQL(string fileName)
        {
            var result = false;
            var fileSql = string.Format(@"{0}\sql\{1}", Utils.GetAppPath(), fileName);

            if (File.Exists(fileSql))
            {
                IDbConnectionHelper dbHelper = new DbConnectionHelper();

                using (var reader = new StreamReader(fileSql))
                {
                    var sql = reader.ReadToEnd();
                    result = dbHelper.ExecSQL(sql);
                }
            }

            return result;
        }

        private void UpgradeDatabase(int newDatabaseVersion)
        {
            IDatabaseVersionBll bll = new DatabaseVersionBll(_log);
            
            var dbVersion = bll.Get();
            if (dbVersion != null)
            {
                var result = true;
                var upgradeTo = dbVersion.version_number + 1;
                
                while (upgradeTo <= newDatabaseVersion)
                {
                    var scriptUpgrade = DatabaseVersionHelper.ListOfUpgradeDatabaseScript[upgradeTo];
                    result = ExecSQL(scriptUpgrade);

                    if (!result)
                        break;

                    upgradeTo++;
                    if (!(bll.UpdateVersion() > 0))
                        break;
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var isConnected = false;

            SaveAppConfig();

            // Connection test ke server
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                IDbConnectionHelper dbConn = new DbConnectionHelper();
                isConnected = dbConn.IsOpenConnection();
            }

            if (!isConnected)
            {
                var msg = "Key-ConnectionFailed";

                MsgHelper.MsgError(msg);
                return;
            }

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                IUserBll userBll = new UserBll(_log);

                var pass = CryptoHelper.GetMD5Hash(txtPassword.Text, MainProgram.securityCode);
                var isLogin = userBll.IsValidUser(txtUserName.Text, pass);

                if (isLogin)
                {
                    UpgradeDatabase(DatabaseVersionHelper.DatabaseVersion);

                    log4net.GlobalContext.Properties["UserName"] = txtUserName.Text;
                    MainProgram.user = userBll.GetByID(txtUserName.Text);

                    SetProfil();
                    SetGeneralSupplier();
                    SetSettingPort();
                    SetSettingCustomerDisplay();
                    SetSettingLebarColumnTabelTransactions();
                    LoadCard();

                    var saldoAwal = NumberHelper.StringToDouble(txtSaldoAwal.Text);
                    SaveSaldoAwal(MainProgram.user.user_id, (int)saldoAwal);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MsgHelper.MsgWarning("Incorrect user name or password !!!");
                    txtUserName.Focus();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabControl_Click(object sender, EventArgs e)
        {
            switch (((TabControl)sender).SelectedIndex)
            {
                case 0:
                    txtUserName.Focus();
                    break;

                case 1:
                    txtServer.Focus();
                    break;

                default:
                    break;
            }
        }

        private void txtSaldoAwal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnLogin_Click(sender, e);
        }        
    }
}
