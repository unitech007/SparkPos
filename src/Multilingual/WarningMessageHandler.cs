using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MultilingualApp
{
    public static class WarningMessageHandler
    {
        public static ResourceManager GetResourceManager(string languageToLoad)
        {
            ResourceManager rm;

            if (languageToLoad == "en-US")
            {
                rm = new ResourceManager("MultilingualApp.MsgWarnings_en-US",
                    Assembly.GetExecutingAssembly());
            }
            else
            {
                rm = new ResourceManager("MultilingualApp.MsgWarnings_ar-SA", Assembly.GetExecutingAssembly());
            }

            return rm;
        }

        public static string TranslateWarningMessages(string message, string languageToLoad)
        {
            ResourceManager rm = GetResourceManager(languageToLoad);
            // Translate the message
            string translatedMessage = rm.GetString(message, new CultureInfo(languageToLoad));
            if (!string.IsNullOrEmpty(translatedMessage))
            {
                return translatedMessage;
            }

            // If the translation is not available, return the original message
            return message;
        }
        public static string ShowTranslatedWarning(string message, string languageToLoad)
        {
            string translatedMsg = TranslateWarningMessages(message, languageToLoad);
            return translatedMsg;
        }

        public static void SetToolTip(Control control, string caption, string languageToLoad)
        {
            ResourceManager rm = GetResourceManager(languageToLoad);
            string translatedCaption = TranslateWarningMessages(caption, languageToLoad);

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(control, translatedCaption);
        }


        public static void MsgWarning(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }


        // Define warning message properties
        public static string MinimumReasonSelectedWarning { get; set; }

    }
}
