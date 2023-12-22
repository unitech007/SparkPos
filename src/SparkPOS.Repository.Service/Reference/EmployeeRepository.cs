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
    public class EmployeeRepository : IEmployeeRepository
    {
        private const string SQL_TEMPLATE = @"SELECT m_employee.employee_id, m_employee.employee_name, m_employee.address, m_employee.phone, 
                                              m_employee.payment_type, m_employee.basic_salary, m_employee.overtime_salary, m_employee.total_loan, m_employee.total_loan_payment, m_employee.is_active, m_employee.description,
                                              m_job_titles.job_titles_id, m_job_titles.name_job_titles, m_job_titles.description
                                              FROM public.m_employee INNER JOIN public.m_job_titles ON m_employee.job_titles_id = m_job_titles.job_titles_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public EmployeeRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<Employee> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Employee> oList = _context.db.Query<Employee, job_titles, Employee>(sql, (k, j) =>
            {
                k.job_titles_id = j.job_titles_id; k.job_titles = j;
                return k;
            }, param, splitOn: "job_titles_id");

            return oList;
        }

        public Employee GetByID(string id)
        {
            Employee obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_employee.employee_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<Employee> GetByName(string name)
        {
            IList<Employee> oList = new List<Employee>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_employee.employee_name) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_employee.employee_name");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Employee> GetAll()
        {
            IList<Employee> oList = new List<Employee>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_employee.employee_name");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(Employee obj)
        {
            var result = 0;

            try
            {
                if (obj.employee_id == null)
                    obj.employee_id = _context.GetGUID();

                _context.db.Insert<Employee>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(Employee obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<Employee>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(Employee obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<Employee>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }
    }
}     
