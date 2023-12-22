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
using System.Data;
 
namespace SparkPOS.Repository.Api
{    
    public interface IDapperContext : IDisposable
    {
        IDbConnection db { get; }
		IDbTransaction transaction { get; }
		bool IsOpenConnection();
        bool ExecSQL(string sql);
        bool IsTrialExpired();
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
        List<string> GetQuotationsByCustomerId(string customerId);
        string GetGUID();
        string GetLastInvoice(string tableName, IDbTransaction transaction = null);

        string GetLastQuotation(string tableName, IDbTransaction transaction = null);
         string GetLastDeliveryNotes(string tableName, IDbTransaction transaction = null);

        int GetPagesCount(string sql, int pageSize, object param = null);
    }
}
