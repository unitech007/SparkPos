
using System;
using System.Collections.Generic;
using System.Globalization;
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
    }
}
