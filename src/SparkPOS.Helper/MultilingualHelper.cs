using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SparkPOS.Helper
{
    public static class MultilingualHelper
    {
        //public static Stream LoadAndCallMethods(Stream stream,string languageToLoad)
        //{
        //    string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Multilingual.dll");
        //    Assembly multilingualAssembly = Assembly.LoadFrom(dllPath);

        //    // Replace "MultilingualNamespace" with the actual namespace of the Multilingual project
        //    Type multilingualType = multilingualAssembly.GetType("MultilingualApp.RdlcReportHelper");
        //    var paramters = new object[] { stream, languageToLoad };
        //    var result = new MemoryStream();
        //    var methodInfo = multilingualType.GetMethod("TranslateReport");
        //    result = (MemoryStream)methodInfo.Invoke(null, paramters);

        //    return result;
        //}

        public static Stream LoadAndCallMethods(Stream stream, string languageToLoad)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Multilingual.dll");
            Assembly multilingualAssembly = Assembly.LoadFrom(dllPath);

            // Replace "MultilingualNamespace" with the actual namespace of the Multilingual project
            Type multilingualType = multilingualAssembly.GetType("MultilingualNamespace.RdlcReportHelper");

            var parameters = new object[] { stream, languageToLoad };
            var result = new MemoryStream();

            foreach (Type type in multilingualAssembly.GetExportedTypes())
            {
                var method = type.GetMethod("TranslateReport", BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                {
                    var methodResult = method.Invoke(null, parameters) as Stream;
                    if (methodResult != null)
                    {
                        result = (MemoryStream)methodResult;
                    }
                }
            }

            return result;
        }
        
        public static string LoadAndCallMsgHelper(string msg, string languageToLoad)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Multilingual.dll");
            Assembly multilingualAssembly = Assembly.LoadFrom(dllPath);
            
            // Replace "MultilingualNamespace" with the actual namespace of the Multilingual project
            Type multilingualType = multilingualAssembly.GetType("MultilingualNamespace.WarningMessageHandler");
            
            var parameters = new object[] { msg, languageToLoad };
            var result = msg;

            foreach (Type type in multilingualAssembly.GetExportedTypes())
            {
                var method = type.GetMethod("ShowTranslatedWarning", BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                {
                    var methodResult = method.Invoke(null, parameters) as string;
                    if (methodResult != null)
                    {
                        result = (string)methodResult;
                    }
                }
            }

            return result;
        }


        public static string LoadAndCallHeaderHelper(string msg, string languageToLoad)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Multilingual.dll");
            Assembly multilingualAssembly = Assembly.LoadFrom(dllPath);

            // Replace "MultilingualNamespace" with the actual namespace of the Multilingual project
            Type multilingualType = multilingualAssembly.GetType("MultilingualNamespace.LanguageHelper");

            var parameters = new object[] { msg, languageToLoad };
            var result = msg;

            foreach (Type type in multilingualAssembly.GetExportedTypes())
            {
                var method = type.GetMethod("ShowTranslatedWarnings", BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                {
                    var methodResult = method.Invoke(null, parameters) as string;
                    if (methodResult != null)
                    {
                        result = (string)methodResult;
                    }
                }
            }

            return result;
        }

    }
}

