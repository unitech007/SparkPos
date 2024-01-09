using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SparkPOS.Bll.Service
{
    public class Config
    {
        public string SecurityCode { get; set; }

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
    }

 
}
