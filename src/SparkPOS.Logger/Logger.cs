using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SparkPOS.Logger
{
   public sealed class Logger : ILogger
    {
        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());
        //private static readonly string logFilePath = "log.txt";
        private static readonly object lockObject = new object();
        private static readonly string logFilePath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            "loggger" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt"
        );
        private Logger()
        {
        }

        public static Logger GetInstance
        {
            get
            {
                return instance.Value;
            }
        }
       // public static Logger Instance => instance.Value;

        public void LogError(Exception ex)
        {
            lock (lockObject)
            {
                string logMessage = $"[Error] {DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n";

                File.AppendAllText(logFilePath, logMessage);
            }
        }

        public void LogMessage(string message)
        {
            lock (lockObject)
            {
                string logMessage = $"[Error] {DateTime.Now}: {message}\n";

                File.AppendAllText(logFilePath, logMessage);
            }
        }
        //public void LogError(string message)
        //{
        //    throw new NotImplementedException();
        //}

        // Add more logging methods as per your requirements (e.g., LogInfo, LogWarning, etc.)
    }

}
