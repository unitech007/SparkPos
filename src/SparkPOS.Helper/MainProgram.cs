
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SparkPOS.Helper
{
    public class MainProgram
    {
        public static string currentLanguage = "en-US";

        //public static void GlobalLanguageChange(Form frmtoChange)
        //{
        //    if (MainProgram.currentLanguage == "ar-SA")
        //    {
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-SA");
        //        LanguageHelper.ChangeLanguage(frmtoChange, "ar-SA");
        //    }
        //    else if (MainProgram.currentLanguage == "en-US")
        //    {
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        //        LanguageHelper.ChangeLanguage(frmtoChange, "en-US");

        //    }
        //}

      

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

        }
        
    }
}
