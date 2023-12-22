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
    public class ReportExpenseCostRepository : IReportExpenseCostRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_expence.invoice, t_expence.date, m_expense_type.expense_type_id, m_expense_type.name_expense_type, t_expense_item.quantity, t_expense_item.price
                                              FROM public.t_expence INNER JOIN public.t_expense_item ON t_expense_item.expense_id = t_expence.expense_id
                                              INNER JOIN public.m_expense_type ON t_expense_item.expense_type_id = m_expense_type.expense_type_id
                                              {WHERE}
                                              ORDER BY t_expence.date";


        private IDapperContext _context;
        private ILog _log;
        
        public ReportExpenseCostRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportExpenseCost> GetByMonth(int month, int year)
        {
            IList<ReportExpenseCost> oList = new List<ReportExpenseCost>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE);

                whereBuilder.Add("EXTRACT(MONTH FROM t_expence.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_expence.date) = @year");

                oList = _context.db.Query<ReportExpenseCost>(whereBuilder.ToSql(), new { month, year }).ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportExpenseCost> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportExpenseCost> oList = new List<ReportExpenseCost>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_expence.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_expence.date) = @year");

                oList = _context.db.Query<ReportExpenseCost>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportExpenseCost> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportExpenseCost> oList = new List<ReportExpenseCost>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE);

                whereBuilder.Add("t_expence.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportExpenseCost>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
