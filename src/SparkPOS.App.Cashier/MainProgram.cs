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
using System.Linq;
using System.Windows.Forms;

using log4net;
using System.Globalization;
using System.Threading;
using CrashReporterDotNET;

using SparkPOS.Model;
using SparkPOS.Helper;
using SparkPOS.App.Cashier.Main;
using MultilingualApp;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace SparkPOS.App.Cashier
{
    static class MainProgram
    {
        /// <summary>
        /// Instance log4net
        /// </summary>
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Url Information update Latest, untuk petunjuknya cek: http://coding4ever.net/blog/2016/01/10/paket-nuget-yang-wajib-dicoba-bagian-number-2-autoupdater-dot-net/
        /// </summary>
        public static readonly string onlineUpdateUrlInfo = "https://raw.githubusercontent.com/rudi-krsoftware/spark-pos/master/updater/spark-pos-cashier-auto-updater.xml";

        public static readonly string stageOfDevelopment = "";
        public static readonly string appName = "Spark POS (Cashier) Version {0}{1} - Copyright © {2} ";
        public static readonly string currentVersion = Utils.GetCurrentVersion();

        /// <summary>
        /// Code unik untuk enkripsi password menggunakan metode md5
        /// Untuk reason keamanan, sebaiknya nilai ini diChange
        /// </summary>
        public static readonly string securityCode = "BhGr7YwZpdX7ubFuZCuU";

        public static Profil profil = null;
        public static User user = null;
        public static GeneralSupplier GeneralSupplier = null;
        public static SettingPort settingPort = null;
        public static SettingCustomerDisplay settingCustomerDisplay = null;
        public static SettingLebarColumnTabelTransactions settingLebarColumnTabelTransactions = null;
        public static IList<Card> listOfCard = null;
        public static string mesinId;

        private static bool _isLogout;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Utils.IsRunningUnderIDE())
            {
                Application.ThreadException += delegate(object sender, ThreadExceptionEventArgs e)
                {
                    ReportCrash(e.Exception);
                };

                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
                {
                    ReportCrash((Exception)e.ExceptionObject);
                    Environment.Exit(0);
                };
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Login();
        }

        /// <summary>
        /// Method untuk mengirim bug/error program via email menggunakan library CrashReporter.NET
        /// Untuk petunjuknya cek: http://coding4ever.net/blog/2015/04/14/paket-nuget-yang-wajib-dicoba-bagian-number-1-crashreporter-dot-net/
        /// </summary>
        /// <param name="exception"></param>
        static void ReportCrash(Exception exception)
        {
            // TODO: lengkapi property FromEmail, ToEmail, UserName dan Password
            var reportCrash = new ReportCrash
            {
                FromEmail = "",
                ToEmail = "",
                SmtpHost = "smtp.gmail.com",
                Port = 587,
                EnableSSL = true,
                UserName = "",
                Password = "",
                AnalyzeWithDoctorDump = false
            };

            reportCrash.Send(exception);
        }

        static void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isLogout = ((FrmMain)sender).IsLogout;
        }

        public static string currentLanguage = "en-US";

        public static void GlobalLanguageChange(Form frmtoChange)
        {
            if (MainProgram.currentLanguage == "ar-SA")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-SA");
                LanguageHelper.ChangeLanguage(frmtoChange, "ar-SA");
            }
            else if (MainProgram.currentLanguage == "en-US")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                LanguageHelper.ChangeLanguage(frmtoChange, "en-US");

            }
        }
        public static void LogException(Exception ex)
        {
            string logFileName = "SparkPOS_Error_log.txt";
            string logFilePath = Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                logFileName
            );

            // Check if the file exists
            if (!File.Exists(logFilePath))
            {
                // Create the file if it doesn't exist
                using (StreamWriter createFile = File.CreateText(logFilePath))
                {
                    createFile.Close();
                }
            }

            // Append exception information to the file
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                //writer.WriteLine("MachineName: " + Environment.MachineName);
                //writer.WriteLine("Exception Message: " + ex.Message);
                //writer.WriteLine("Stack Trace: " + ex.StackTrace);
                //if (ex.InnerException != null)
                //{
                //    writer.WriteLine("Inner Exception: " + ex.InnerException);
                //}
                writer.WriteLine("Timestamp: " + DateTime.Now.ToString());
                writer.WriteLine("Exception Message: " + ex.ToString());
                writer.WriteLine("-----------------------------------------------");
            }
        }
        public static string GlobalWarningMessage()
        {
            var usMsg = "An error occurred. Please check the error log for details.";
            var saMsg = "حدث خطأ. يرجى التحقق من سجل الأخطاء للحصول على التفاصيل.";
            //GetWarningMessageLanguage();
            if (MainProgram.currentLanguage == "ar-SA")
            {
                return saMsg;

            }
            else if (MainProgram.currentLanguage == "en-US")
            {

                return usMsg;
            }
            return usMsg;
        }
        static void Login()
        {
            var frmMain = new FrmMain();
            frmMain.FormClosed += frmMain_FormClosed;

            var frmLogin = new FrmLogin();
            if (frmLogin.ShowDialog(frmMain) == DialogResult.OK)
            {
                // set Default RegionalSetting menggunakan United States
                SetDefaultRegionalSetting();

                frmMain.InisialisasiData();
                Application.Run(frmMain);

                if (_isLogout)
                    Login();
                else
                    Application.Exit();
            }
            else
                Application.Exit();
        }

        static void SetDefaultRegionalSetting()
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var regionInfo = new RegionInfo(cultureInfo.LCID);

            string englishName = regionInfo.EnglishName;

            if (!(englishName == "United States"))
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                }
                catch
                {
                }
            }
        }
    }
}
