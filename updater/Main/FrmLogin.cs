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

using ConceptCave.WaitCursor;
using log4net;
using SparkPOS.Helper;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Configuration;

namespace SparkPOS.App.Main
{
    public partial class FrmLogin : Form
    {
        private ILog _log;
        private string _appConfigFile = string.Format("{0}\\SparkPOS.exe.config", Utils.GetAppPath());

        public FrmLogin()
        {
            InitializeComponent();
            ColorManagerHelper.SetTheme(this, this);

            _log = MainProgram.log;

            LoadAppConfig();
        }

        private void LoadAppConfig()
        {
            txtServer.Text = AppConfigHelper.GetValue("server", _appConfigFile);

            if (Utils.IsRunningUnderIDE()) // mode debug, set user dan password default untuk development
            {
                txtUserName.Text = "admin";
                txtPassword.Text = "admin";
            }

            // baca setting pageSize
            var pageSize = AppConfigHelper.GetValue("pageSize", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("pageSize", _appConfigFile)) : 0;

            if (pageSize > 0)
                MainProgram.pageSize = pageSize;
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
            MainProgram.GeneralSupplier.is_auto_print_label_nota = AppConfigHelper.GetValue("isAutoPrinterLabelInvoice", _appConfigFile).ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.is_show_minimal_stock = AppConfigHelper.GetValue("isShowMinimalStock", _appConfigFile).ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.is_customer_required = AppConfigHelper.GetValue("isCustomerRequired", _appConfigFile).ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.is_print_keterangan_nota = AppConfigHelper.GetValue("isPrintKeteranganInvoice", _appConfigFile, "true").ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.is_singkat_penulisan_ongkir = AppConfigHelper.GetValue("isSingkatPenulisanOngkir", _appConfigFile).ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.default_ppn = Convert.ToDouble(AppConfigHelper.GetValue("defaultPPN", _appConfigFile, "0"));

            // set info printer mini pos
            var jumlahKarakter = AppConfigHelper.GetValue("jumlahKarakter", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("jumlahKarakter", _appConfigFile)) : 40;
            var jumlahGulung = AppConfigHelper.GetValue("jumlahGulung", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("jumlahGulung", _appConfigFile)) : 5;
            var isPrintCustomer = AppConfigHelper.GetValue("isPrintCustomer", _appConfigFile).Length > 0 ? Convert.ToBoolean(AppConfigHelper.GetValue("isPrintCustomer", _appConfigFile)) : true;
            var FontSize = AppConfigHelper.GetValue("FontSize", _appConfigFile).Length > 0 ? Convert.ToInt32(AppConfigHelper.GetValue("FontSize", _appConfigFile)) : 0;

            MainProgram.GeneralSupplier.is_autocut = AppConfigHelper.GetValue("isAutocut", _appConfigFile, "false").ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.autocut_code = AppConfigHelper.GetValue("autocutCode", _appConfigFile, "27,112,0,75,250");

            MainProgram.GeneralSupplier.is_open_cash_drawer = AppConfigHelper.GetValue("isOpenCashDrawer", _appConfigFile, "false").ToLower() == "true" ? true : false;
            MainProgram.GeneralSupplier.open_cash_drawer_code = AppConfigHelper.GetValue("openCashDrawerCode", _appConfigFile, "27,112,0,25,250");

            MainProgram.GeneralSupplier.type_printer = AppConfigHelper.GetValue("type_printer", _appConfigFile).Length > 0 ? (TypePrinter)Convert.ToInt32(AppConfigHelper.GetValue("type_printer", _appConfigFile)) : TypePrinter.InkJet;
            MainProgram.GeneralSupplier.is_print_customer = isPrintCustomer;
            MainProgram.GeneralSupplier.jumlah_karakter = jumlahKarakter;
            MainProgram.GeneralSupplier.jumlah_gulung = jumlahGulung;
            MainProgram.GeneralSupplier.size_font = FontSize;

            // set settings global (setting disave di database)
            ISettingApplicationBll settingApplicationBll = new SettingApplicationBll();
            var settingApplication = settingApplicationBll.GetAll().SingleOrDefault();

            if (settingApplication != null)
            {
                MainProgram.GeneralSupplier.is_negative_stock_allowed_for_products = settingApplication.is_negative_stock_allowed_for_products;
                MainProgram.GeneralSupplier.is_update_selling_price = settingApplication.is_update_selling_price_of_master_products;
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

        private void SetSettingsBarcode()
        {
            MainProgram.settingsBarcode = new SettingsBarcode();
            MainProgram.settingsBarcode.name_printer = AppConfigHelper.GetValue("printerBarcode", _appConfigFile);

            MainProgram.settingsBarcode.header_label = AppConfigHelper.GetValue("headerLabel", _appConfigFile).Length == 0 ? MainProgram.profil.name_profile.NullToString()
                                                                                                                            : AppConfigHelper.GetValue("headerLabel", _appConfigFile);

            MainProgram.settingsBarcode.batas_atas_row1 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow1", _appConfigFile, "43"));
            MainProgram.settingsBarcode.batas_atas_row2 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow2", _appConfigFile, "187"));
            MainProgram.settingsBarcode.batas_atas_row3 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow3", _appConfigFile, "344"));
            MainProgram.settingsBarcode.batas_atas_row4 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow4", _appConfigFile, "496"));

            MainProgram.settingsBarcode.batas_kiri_column1 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn1", _appConfigFile, "11"));
            MainProgram.settingsBarcode.batas_kiri_column2 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn2", _appConfigFile, "277"));
            MainProgram.settingsBarcode.batas_kiri_column3 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn3", _appConfigFile, "540"));
        }

        private void SetSettingsLabelPrice()
        {
            MainProgram.settingsLabelPrice = new SettingsLabelPrice();
            MainProgram.settingsLabelPrice.name_printer = AppConfigHelper.GetValue("printerLabelPrice", _appConfigFile);
            MainProgram.settingsLabelPrice.font_name = AppConfigHelper.GetValue("fontName", _appConfigFile, "Courier New");

            MainProgram.settingsLabelPrice.batas_atas_row1 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow1LabelPrice", _appConfigFile, "43"));
            MainProgram.settingsLabelPrice.batas_atas_row2 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow2LabelPrice", _appConfigFile, "187"));
            MainProgram.settingsLabelPrice.batas_atas_row3 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow3LabelPrice", _appConfigFile, "344"));
            MainProgram.settingsLabelPrice.batas_atas_row4 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow4LabelPrice", _appConfigFile, "496"));
            MainProgram.settingsLabelPrice.batas_atas_row5 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow5LabelPrice", _appConfigFile, "650"));
            MainProgram.settingsLabelPrice.batas_atas_row6 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow6LabelPrice", _appConfigFile, "805"));
            MainProgram.settingsLabelPrice.batas_atas_row7 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow7LabelPrice", _appConfigFile, "960"));
            MainProgram.settingsLabelPrice.batas_atas_row8 = Convert.ToSingle(AppConfigHelper.GetValue("batasAtasRow8LabelPrice", _appConfigFile, "1115"));

            MainProgram.settingsLabelPrice.batas_kiri_column1 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn1LabelPrice", _appConfigFile, "30"));
            MainProgram.settingsLabelPrice.batas_kiri_column2 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn2LabelPrice", _appConfigFile, "225"));
            MainProgram.settingsLabelPrice.batas_kiri_column3 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn3LabelPrice", _appConfigFile, "425"));
            MainProgram.settingsLabelPrice.batas_kiri_column4 = Convert.ToSingle(AppConfigHelper.GetValue("batasKiriColumn4LabelPrice", _appConfigFile, "625"));
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

        /// <summary>
        /// Load data regency untuk keperluan pengecekan cost shipping
        /// </summary>
        private void LoadRegency()
        {
            IRegencyShippingCostsByRajaBll bll = new RegencyShippingCostsByRajaBll();
            MainProgram.ListOfRegency = bll.GetAll();
        }

        /// <summary>
        /// Load data region (provinsi, regency dan subdistrict) untuk address customer
        /// </summary>
        private void LoadArea()
        {
            IAreaBll bll = new AreaBll();
            MainProgram.ListOfArea = bll.GetAll();
        }

        private void LoadInfoMinimalStockProduct()
        {
            IProductBll bll = new ProductBll(_log);
            MainProgram.listOfMinimalStockProduct = bll.GetInfoMinimalStock();
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

        //private void btnLogin_Click(object sender, EventArgs e)
        //{
        //    var isConnected = false;

        //    SaveAppConfig();

        //    // Connection test ke server
        //    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
        //    {
        //        IDbConnectionHelper dbConn = new DbConnectionHelper();
        //        isConnected = dbConn.IsOpenConnection();
        //    }

        //    if (!isConnected)
        //    {
        //        var msg = "Sorry connection to database failed !!!\n\n" +
        //                   "It is recommended to install SparkPOS on 'Drive D'.\n" +
        //                   "Please uninstall SparkPOS first, then install it again on 'Drive D'.";

        //        MsgHelper.MsgError(msg);
        //        return;
        //    }

        //    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
        //    {
        //        IUserBll userBll = new UserBll(_log);

        //        var pass = CryptoHelper.GetMD5Hash(txtPassword.Text, MainProgram.securityCode);
        //        var isLogin = userBll.IsValidUser(txtUserName.Text, pass);

        //        if (isLogin)
        //        {
        //            UpgradeDatabase(DatabaseVersionHelper.DatabaseVersion);

        //            log4net.GlobalContext.Properties["UserName"] = txtUserName.Text;
        //            MainProgram.user = userBll.GetByID(txtUserName.Text);

        //            SetProfil();
        //            SetGeneralSupplier();
        //            SetSettingsBarcode();
        //            SetSettingsLabelPrice();
        //            SetSettingPort();
        //            SetSettingCustomerDisplay();
        //            LoadRegency();
        //            LoadArea();

        //            if (MainProgram.GeneralSupplier.is_show_minimal_stock)
        //            {
        //                LoadInfoMinimalStockProduct();
        //            }                        

        //            this.DialogResult = DialogResult.OK;
        //            this.Close();
        //        }
        //        else
        //        {
        //            MsgHelper.MsgWarning("Incorrect user name or password !!!");
        //            txtUserName.Focus();
        //        }
        //    }
        //}



        private void btnLogin_Click(object sender, EventArgs e)
        {
            var isConnected = false;

            SaveAppConfig();

            try
            {
                // Connection test ke server
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    IDbConnectionHelper dbConn = new DbConnectionHelper();
                    isConnected = dbConn.IsOpenConnection();
                }

                if (!isConnected)
                {
                    var msg = "Sorry connection to the database failed !!!\n\n" +
                              "It is recommended to install SparkPOS on 'Drive D'.\n" +
                              "Please uninstall SparkPOS first, then install it again on 'Drive D'.";

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
                        SetSettingsBarcode();
                        SetSettingsLabelPrice();
                        SetSettingPort();
                        SetSettingCustomerDisplay();
                        LoadRegency();
                        LoadArea();

                        if (MainProgram.GeneralSupplier.is_show_minimal_stock)
                        {
                            LoadInfoMinimalStockProduct();
                        }

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
            catch (Exception ex)
            {
                // Error handling and logging
                LogError(ex, "IsOpenConnection");
                MsgHelper.MsgError("An error occurred. Please check the error log for details.");
            }
        }

        private void LogError(Exception ex, string methodName)
        {
            try
            {
                string logFilePath = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Error" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log"
                );

                string server = ConfigurationManager.AppSettings["server"];
                string port = ConfigurationManager.AppSettings["port"];
                string dbName = ConfigurationManager.AppSettings["dbName"];
                string userId = "postgres";
                string userPassword = "Poiu1234##";
                string connectionString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4}", server, port, userId, userPassword, dbName);

                string errorText = $"[{DateTime.Now}] Error occurred in method '{methodName}':\n" +
                    $"Server: {server}\n" +
                    $"Port: {port}\n" +
                    $"Connection String: {connectionString}\n" +
                    $"{ex.ToString()}{Environment.NewLine}";

                // Append the error to the log file
                File.AppendAllText(logFilePath, errorText);
            }
            catch (Exception)
            {
                // Error occurred while logging the error, handle it as desired
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

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnLogin_Click(sender, e);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
