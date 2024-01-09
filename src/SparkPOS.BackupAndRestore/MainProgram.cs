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
using System.IO;
using System.Linq;
using System.Windows.Forms;

using SparkPOS.BackupAndRestore.Main;

namespace SparkPOS.BackupAndRestore
{
    static class MainProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        
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
      
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
