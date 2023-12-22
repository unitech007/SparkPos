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

using log4net;
using Dapper;
using SparkPOS.Model.Report;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Api.Report;

namespace SparkPOS.Repository.Service.Report
{
    public class ReportSalaryEmployeeRepository : IReportSalaryEmployeeRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT m_employee.employee_id, m_employee.employee_name, m_employee.payment_type, m_job_titles.name_job_titles, 
                                                     t_employee_salary.date, t_employee_salary.month, t_employee_salary.year, 
                                                     t_employee_salary.attendance, t_employee_salary.absence, t_employee_salary.basic_salary, t_employee_salary.overtime, t_employee_salary.bonus, 
                                                     t_employee_salary.deductions, t_employee_salary.time, t_employee_salary.other, t_employee_salary.description, 
                                                     t_employee_salary.days_worked, t_employee_salary.allowance
                                                     FROM public.t_employee_salary INNER JOIN public.m_employee ON t_employee_salary.employee_id = m_employee.employee_id
                                                     INNER JOIN public.m_job_titles ON m_employee.job_titles_id = m_job_titles.job_titles_id
                                                     {WHERE}
                                                     ORDER BY t_employee_salary.date, m_employee.employee_name";

        private IDapperContext _context;
        private ILog _log;

        public ReportSalaryEmployeeRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportSalaryEmployee> GetByMonth(int month, int year)
        {
            IList<ReportSalaryEmployee> oList = new List<ReportSalaryEmployee>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_employee_salary.month = @month");
                whereBuilder.Add("t_employee_salary.year = @year");

                oList = _context.db.Query<ReportSalaryEmployee>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalaryEmployee> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalaryEmployee> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalaryEmployee> oList = new List<ReportSalaryEmployee>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_employee_salary.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportSalaryEmployee>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
