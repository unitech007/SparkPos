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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class PaymentLoanRepository : IPaymentLoanRepository
    {
        private IDapperContext _context;
		private ILog _log;
		
        public PaymentLoanRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new PaymentLoan().GetTableName());
        }

        public PaymentLoan GetByID(string id)
        {
            PaymentLoan obj = null;

            try
            {
                obj = _context.db.Get<PaymentLoan>(id);
                if (obj != null)
                    obj.entity_state = EntityState.Unchanged;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<PaymentLoan> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<PaymentLoan> GetByLoanId(string kasbonId)
        {
            IList<PaymentLoan> oList = new List<PaymentLoan>();

            try
            {
                oList = _context.db.GetAll<PaymentLoan>()
                                .Where(f => f.loan_id == kasbonId)                                
                                .Select(f => { f.entity_state = EntityState.Unchanged; return f; })
                                .OrderBy(f => f.date)
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PaymentLoan> GetBySalaryEmployee(string gajiEmployeeId)
        {
            IList<PaymentLoan> oList = new List<PaymentLoan>();

            try
            {
                oList = _context.db.GetAll<PaymentLoan>()
                                .Where(f => f.gaji_employee_id == gajiEmployeeId)
                                .Select(f => { f.entity_state = EntityState.Unchanged; return f; })
                                .OrderBy(f => f.date)
                                .ToList();

                foreach (var item in oList)
                {
                    item.entity_state = EntityState.Unchanged;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PaymentLoan> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Save(PaymentLoan obj)
        {
            var result = 0;

            try
            {
                if (obj.payment_loan_id == null)
                    obj.payment_loan_id = _context.GetGUID();

                _context.db.Insert<PaymentLoan>(obj);

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Add data");

                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(PaymentLoan obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<PaymentLoan>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                    _log.Info("Update data");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(PaymentLoan obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<PaymentLoan>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["OldValue"] = obj.ToJson();
                    _log.Info("Delete data");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }        
    }
}     
