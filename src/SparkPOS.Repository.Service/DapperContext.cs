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
using System.Data;
using System.Data.Common;
using System.Configuration;
using SparkPOS.Repository.Api;
using Dapper;
using log4net;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using System.Security.Cryptography.X509Certificates;

namespace SparkPOS.Repository.Service
{    
    public class DapperContext : IDapperContext
    {
        private IDbConnection _db;
        private IDbTransaction _transaction;
        private readonly ILog _log = LogManager.GetLogger(typeof(DapperContext));

        private readonly string _providerName;
        private readonly string _connectionString;
        //  private const int TrialDurationDays = 15;
        //public DapperContext()
        //{
        //    var server = ConfigurationManager.AppSettings["server"];
        //    var port = ConfigurationManager.AppSettings["port"];
        //    var dbName = ConfigurationManager.AppSettings["dbName"];
        //    var appName = "SparkPOSApp";
        //    var userId = "postgres";
        //    var userPassword = "masterkey";

        //    _providerName = "Npgsql";
        //    _connectionString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};ApplicationName={5}", server, port, userId, userPassword, dbName, appName);

        //    if (_db == null)
        //    {
        //        _db = GetOpenConnection(_providerName, _connectionString);
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


        public DapperContext()
        {
            var server = ConfigurationManager.AppSettings["server"];
            var port = ConfigurationManager.AppSettings["port"];
            var dbName = ConfigurationManager.AppSettings["dbName"];
            var appName = "SparkPOSApp";
            var userId = "postgres";
            var userPassword = "Poiu1234##";
           // var userPassword = ""74521f341a6473f2bea7fa0ef052e7a8";

            _providerName = "Npgsql";
            _connectionString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};ApplicationName={5}", server, port, userId, userPassword, dbName, appName);

            if (_db == null)
            {
                _db = GetOpenConnection(_providerName, _connectionString);
            }
        }
        
        //public bool IsOpenConnection()
        //{
        //    var isConnected = false;

        //    try
        //    {
        //        using (var conn = GetOpenConnection(_providerName, _connectionString))
        //        {
        //            isConnected = conn.State == ConnectionState.Open;
        //        }
        //    }
        //    catch
        //    {
        //    }

        //    return isConnected;
        //}

        public bool IsOpenConnection()
        {
            var isConnected = false;

            try
            {
                using (var conn = GetOpenConnection(_providerName, _connectionString))
                {
                    isConnected = conn.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                DapperContext.LogException(ex);
                // Error handling and logging
              //  LogError(ex, "IsOpenConnection");
            }

            return isConnected;
        }


      

        public bool ExecSQL(string sql)
        {
            var result = false;

            try
            {
                _db.Execute(sql);
                result = true;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        //private IDbConnection GetOpenConnection(string providerName, string connectionString)
        //{
        //    DbConnection conn = null;

        //    try
        //    {
        //        DbProviderFactory provider = DbProviderFactories.GetFactory(providerName);
        //        conn = provider.CreateConnection();
        //        conn.ConnectionString = connectionString;
        //        conn.Open();
        //        // LogError(ex, "IsOpenConnection");
        // //       string logFilePath = Path.Combine(
        // //    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
        // //    "OPENCONNECTION" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log"
        // //);

        // //       File.WriteAllText(logFilePath, "Connection String: " + connectionString);

        //        //
        //    }
        //    catch (Exception ex)
        //    {
        //        // Error handling and logging
        //        //LogError(ex, "IsOpenConnection");
        //    }

        //    return conn;
        //}

        private IDbConnection GetOpenConnection(string providerName, string connectionString)
        {
          //  connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=Poiu1234##;Database=sparkposdb;ApplicationName=SparkPOSApp;";

            IDbConnection conn = null;

            try
            {
                DbProviderFactory provider = DbProviderFactories.GetFactory(providerName);
                conn = provider.CreateConnection();

                if (conn is NpgsqlConnection npgsqlConn)
                {
                    // Npgsql connection-specific settings
                    // If your password contains special characters, you may need to escape them in the connection string
                }

                conn.ConnectionString = connectionString;
                conn.Open();
            }
            catch (Exception ex)
            {
                DapperContext.LogException(ex);
                // Handle the exception (e.g., log the error)
                // LogError(ex, "IsOpenConnection");
            }

            return conn;
        }

        public bool IsTrialExpired()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString)) // Replace '_connectionString' with your actual connection string
            {
                string query = "SELECT installed_date, trial_period FROM trialinfo WHERE is_trial = true";

                var result = connection.QuerySingleOrDefault<(DateTime installedDate, int trialPeriod)>(query);
                if (result != default)
                {
                    DateTime trialEndDate = result.installedDate.AddDays(result.trialPeriod);

                    if (trialEndDate < DateTime.Now)
                    {
                        // Trial period has expired
                        return true;
                    }
                }
            }

            // No trial information found or trial period has not expired
            return false;
        }
        public IDbConnection db
        {
            get { return _db ?? (_db = GetOpenConnection(_providerName, _connectionString)); }
        }

		public IDbTransaction transaction
        {
            get { return _transaction; }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction == null)
                _transaction = _db.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction = null;
            }            
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }            
        }

        public List<string> GetQuotationsByCustomerId(string customerId)
        {
            List<string> quotations = new List<string>();

            using (IDbConnection connection = new NpgsqlConnection(_connectionString)) // Replace 'connectionString' with your actual connection string
            {
                string query = "SELECT quotation FROM t_sales_quotation WHERE customer_id = @CustomerId";
                quotations = connection.Query<string>(query, new { quotations = quotations }).ToList();
            }

            return quotations;
        }

        //public bool IsTrialExpired()
        //{
        //    using (IDbConnection connection = new NpgsqlConnection(_connectionString)) // Replace '_connectionString' with your actual connection string
        //    {
        //        string query = "SELECT installed_date,trial_periode FROM trial_info";
        //        DateTime installed_date = connection.QuerySingleOrDefault<DateTime>(query);

        //        if (installed_date > default(DateTime))
        //        {
        //            DateTime currentDate = DateTime.Today;
        //            int daysPassed = (int)(currentDate - installed_date).TotalDays;
        //            return daysPassed > TrialDurationDays;
        //        }
        //    }

        //    return true;
        //}

       

        private int GetGeneratorIDByTable(string tableName, IDbTransaction transaction = null)
        {
            int result = 0;

            try
            {
                var generatorName = tableName + "_" + tableName.Substring(2) + "_id_seq";

                var strSql = String.Format("SELECT NEXTVAL('{0}')", generatorName);
				result = _db.QuerySingleOrDefault<int>(strSql, transaction);
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex); 
               
            }

            return result;
        }

        public string GetLastInvoice(string tableName, IDbTransaction transaction = null)
        {
            var lastId = GetGeneratorIDByTable(tableName, transaction);
            var lastInvoice = string.Format("{0}{1:00}{2:00}{3:0000}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, lastId);

            return lastInvoice;
        }   
        
        
        public string GetLastDeliveryNotes(string tableName, IDbTransaction transaction = null)
        {
            var lastId = GetGeneratorIDByTable(tableName, transaction);
            var lastInvoice = string.Format("{0}{1:00}{2:00}{3:0000}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, lastId);

            return lastInvoice;
        }  
        public string GetLastQuotation(string tableName, IDbTransaction transaction = null)
        {
            var lastId = GetGeneratorIDByTable(tableName, transaction);
            var lastInvoice = string.Format("{0}{1:00}{2:00}{3:0000}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, lastId);

            return lastInvoice;
        }

		public string GetGUID()
        {
            var result = string.Empty;

            try
            {
                result = Guid.NewGuid().ToString();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int GetPagesCount(string sql, int pageSize, object param = null)
        {
            var pagesCount = 0;

            try
            {
                var recordCount = _db.QuerySingleOrDefault<int>(sql, param);
                pagesCount = (int)Math.Ceiling(recordCount / (decimal)pageSize);
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return pagesCount;
        }

    

        public void Dispose()
        {
            if (_db != null)
            {
                try
                {
                    if (_db.State != ConnectionState.Closed)
                    {
                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                        }

                        _db.Close();
                    }                        
                }
                finally
                {
                    _db.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }

       
    }
}
