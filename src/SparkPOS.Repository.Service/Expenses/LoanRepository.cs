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
using Dapper;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class LoanRepository : ILoanRepository
    {
        //private const string SQL_TEMPLATE = @"SELECT t_loan.loan_id, t_loan.invoice, t_loan.date, t_loan.amount, t_loan.total_payment, t_loan.description, t_loan.user_id,
        //                                      m_employee.employee_id, m_employee.employee_name
        //                                      FROM public.m_employee INNER JOIN public.t_loanON m_employee.employee_id = t_loan.employee_id
        //                                      {WHERE}
        //                                      {ORDER BY}";
        private const string SQL_TEMPLATE = @"SELECT t_loan.loan_id, t_loan.invoice, t_loan.date, t_loan.amount, t_loan.total_payment, t_loan.description, t_loan.user_id,
                                              m_employee.employee_id, m_employee.employee_name
                                              FROM public.m_employee INNER JOIN public.t_loan ON m_employee.employee_id = t_loan.employee_id
                                              {WHERE}
                                              {ORDER BY}";

        private IDapperContext _context;
        private ILog _log;

        private string _sql;
		
        public LoanRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<loan> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<loan> oList = _context.db.Query<loan, Employee, loan>(sql, (kas, kar) =>
            {
                kas.employee_id = kar.employee_id; kas.Employee = kar;
                return kas;
            }, param, splitOn: "employee_id");

            return oList;
        }

        public loan GetByID(string id)
        {
            loan obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_loan.loan_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<loan> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<loan> GetByEmployeeId(string employeeId)
        {
            IList<loan> oList = new List<loan>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_loan.employee_id = @employeeId");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_loan.date");

                oList = MappingRecordToObject(_sql, new { employeeId }).ToList();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<loan> GetByStatus(bool isLunas)
        {
            IList<loan> oList = new List<loan>();

            try
            {
                if (isLunas)
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE (t_loan.amount  - t_loan.total_payment) <= 0");
                }
                else
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE (t_loan.amount  - t_loan.total_payment) > 0");
                }
                
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_loan.date");

                oList = MappingRecordToObject(_sql).ToList();

                if (oList.Count > 0)
                {
                    IPaymentLoanRepository paymentLoanRepo = new PaymentLoanRepository(_context, _log);

                    foreach (var loan in oList)
                    {
                        loan.item_payment_loan = paymentLoanRepo.GetByLoanId(loan.loan_id).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<loan> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<loan> oList = new List<loan>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_loan.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_loan.date");

                //oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
                oList = MappingRecordToObject(_sql, new { tanggalMulai = tanggalMulai, tanggalSelesai = tanggalSelesai }).ToList();

                if (oList.Count > 0)
                {
                    IPaymentLoanRepository paymentLoanRepo = new PaymentLoanRepository(_context, _log);

                    foreach (var loan in oList)
                    {
                        loan.item_payment_loan = paymentLoanRepo.GetByLoanId(loan.loan_id).ToList();
                    }
                }                
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<loan> GetAll()
        {
            IList<loan> oList = new List<loan>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_loan.date");

                oList = MappingRecordToObject(_sql).ToList();

                if (oList.Count > 0)
                {
                    IPaymentLoanRepository paymentLoanRepo = new PaymentLoanRepository(_context, _log);

                    foreach (var loan in oList)
                    {
                        loan.item_payment_loan = paymentLoanRepo.GetByLoanId(loan.loan_id).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(loan obj)
        {
            var result = 0;

            try
            {
                if (obj.loan_id == null)
                    obj.loan_id = _context.GetGUID();

                _context.db.Insert<loan>(obj);

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Add data");

                result = 1;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Update(loan obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<loan>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                    _log.Info("Update data");
                }
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(loan obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<loan>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["OldValue"] = obj.ToJson();
                    _log.Info("Delete data");
                }
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new loan().GetTableName());
        }        
    }
}     
