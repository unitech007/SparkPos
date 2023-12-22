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
    public class SalaryEmployeeRepository : ISalaryEmployeeRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_employee_salary.gaji_employee_id, t_employee_salary.user_id, t_employee_salary.month, t_employee_salary.year, 
                                              t_employee_salary.attendance, t_employee_salary.absence, t_employee_salary.basic_salary, t_employee_salary.overtime, t_employee_salary.bonus, 
                                              t_employee_salary.deductions, t_employee_salary.time, t_employee_salary.other, t_employee_salary.description, 
                                              t_employee_salary.days_worked, t_employee_salary.allowance, t_employee_salary.loan, t_employee_salary.date, t_employee_salary.invoice, 
                                              m_employee.employee_id, m_employee.employee_name, m_employee.basic_salary, m_employee.payment_type, m_employee.overtime_salary, m_employee.total_loan, m_employee.total_loan_payment, 
                                              m_job_titles.job_titles_id, m_job_titles.name_job_titles
                                              FROM public.t_employee_salary INNER JOIN public.m_employee ON t_employee_salary.employee_id = m_employee.employee_id
                                              INNER JOIN public.m_job_titles ON m_employee.job_titles_id = m_job_titles.job_titles_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;

        private string _sql;

        public SalaryEmployeeRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<SalaryEmployee> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<SalaryEmployee> oList = _context.db.Query<SalaryEmployee, Employee, job_titles, SalaryEmployee>(sql, (g, k, j) =>
            {
                g.employee_id = k.employee_id; g.Employee = k;
                k.job_titles_id = j.job_titles_id; k.job_titles = j;

                return g;
            }, param, splitOn: "employee_id, job_titles_id");

            return oList;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new SalaryEmployee().GetTableName());
        }

        public SalaryEmployee GetByID(string id)
        {
            SalaryEmployee obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_employee_salary.gaji_employee_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<SalaryEmployee> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<SalaryEmployee> GetByMonthAndYear(int month, int year)
        {
            IList<SalaryEmployee> oList = new List<SalaryEmployee>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_employee_salary.month = @month AND t_employee_salary.year = @year");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_employee.employee_name");

                oList = MappingRecordToObject(_sql, new { month, year }).ToList();

                IPaymentLoanRepository paymentLoanRepo = new PaymentLoanRepository(_context, _log);

                foreach (var gaji in oList)
                {
                    gaji.item_payment_loan = paymentLoanRepo.GetBySalaryEmployee(gaji.gaji_employee_id).ToList();
                }

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SalaryEmployee> GetAll()
        {
            throw new NotImplementedException();
        }

        private bool IsExist(string employeeId, int month, int year)
        {
            var count = _context.db.GetAll<SalaryEmployee>()
                                .Where(f => f.employee_id == employeeId && f.month == month && f.year == year)
                                .Count();

            return count > 0;
        }

        public int Save(SalaryEmployee obj)
        {
            var result = 0;

            try
            {
                if (IsExist(obj.employee_id, obj.month, obj.year)) // data gaji employee already entered
                {
                    return 0;
                }

                _context.BeginTransaction();

                var transaction = _context.transaction;

                if (obj.gaji_employee_id == null)
                    obj.gaji_employee_id = _context.GetGUID();

                // insert header
                _context.db.Insert<SalaryEmployee>(obj, transaction);

                // insert detail
                foreach (var item in obj.item_payment_loan.Where(f => f.loan != null))
                {
                    if (item.loan_id.Length > 0)
                    {
                        item.payment_loan_id = _context.GetGUID();
                        item.user_id = obj.user_id;
                        item.gaji_employee_id = obj.gaji_employee_id;                        
                        item.date = obj.date;
                        item.invoice = obj.invoice;

                        _context.db.Insert<PaymentLoan>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                _context.Commit();

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

        public int Update(SalaryEmployee obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                // update header
                result = _context.db.Update<SalaryEmployee>(obj, transaction) ? 1 : 0;

                // insert/update detail
                foreach (var item in obj.item_payment_loan.Where(f => f.loan != null))
                {
                    item.gaji_employee_id = obj.gaji_employee_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        item.payment_loan_id = _context.GetGUID();

                        _context.db.Insert<PaymentLoan>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<PaymentLoan>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Update data");

                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(SalaryEmployee obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<SalaryEmployee>(obj) ? 1 : 0;

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
