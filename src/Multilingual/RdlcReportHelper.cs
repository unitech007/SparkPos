using Microsoft.Reporting.WinForms;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Xml.Linq;

namespace MultilingualApp
{

    public static class RdlcReportHelper
    {
        public static Stream TranslateReport(Stream reportStream, string languageToLoad)
        {
            XDocument reportXml = XDocument.Load(reportStream);

            ResourceManager rm = new ResourceManager("MultilingualApp." + languageToLoad, Assembly.GetExecutingAssembly());

            foreach (var element in reportXml.Descendants(XName.Get("Value", @"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition")))
            {
               // XAttribute attribute = element.Attribute(XName.Get("LocID", @"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"));
            
                if (element != null)
                {
                    string translatedValue = rm.GetString(element.Value, new CultureInfo(languageToLoad));
                    element.Value = string.IsNullOrEmpty(translatedValue) ? element.Value : translatedValue;
                }
            }

            Stream ms = new MemoryStream();
            reportXml.Save(ms, SaveOptions.OmitDuplicateNamespaces);
            ms.Position = 0;

            return ms;
        }



    }



}


