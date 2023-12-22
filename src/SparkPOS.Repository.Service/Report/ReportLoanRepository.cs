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
    public class ReportLoanRepository : IReportLoanRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT m_employee.employee_id, m_employee.employee_name, 
                                                     t_loan.date, t_loan.invoice, t_loan.amount, t_loan.total_payment, t_loan.description
                                                     FROM public.t_loan INNER JOIN public.m_employee ON t_loan.employee_id = m_employee.employee_id
                                                     {WHERE}
                                                     ORDER BY t_loan.date, m_employee.employee_name";

        //@"SELECT m_employee.employee_id, m_employee.employee_name, 
        //                                             t_loan.date AS tanggal_kasbon, t_loan.invoice AS nota_kasbon, t_loan.amount AS jumlah_kasbon, t_loan.total_payment, t_loan.description AS keterangan_kasbon, 
        //                                             t_payment_loan.invoice AS nota_payment, t_payment_loan.date AS tanggal_payment, t_payment_loan.amount AS jumlah_payment, t_payment_loan.description AS keterangan_payment
        //                                             FROM public.t_loan INNER JOIN public.m_employee ON t_loan.employee_id = m_employee.employee_id
        //                                             INNER JOIN public.t_cash_advance_payment ON t_payment_loan.loan_id = t_loan.loan_id
        //                                             {WHERE}
        //                                             ORDER BY m_employee.employee_name, t_loan.date, t_loan.invoice, t_payment_loan.date, t_payment_loan.invoice";


        private const string SQL_TEMPLATE_DETAIL = @"SELECT m_employee.employee_id, m_employee.employee_name,
t_loan.date AS tanggal_kasbon, t_loan.invoice AS nota_kasbon, t_loan.amount AS jumlah_kasbon,
t_loan.total_payment, t_loan.description AS keterangan_kasbon,
t_cash_advance_payment.invoice AS nota_payment, t_cash_advance_payment.date AS tanggal_payment,
t_cash_advance_payment.amount AS jumlah_payment, t_cash_advance_payment.description AS keterangan_payment
FROM public.t_loan
INNER JOIN public.m_employee ON t_loan.employee_id = m_employee.employee_id
INNER JOIN public.t_cash_advance_payment ON t_cash_advance_payment.loan_id = t_loan.loan_id
{WHERE}
    ORDER BY m_employee.employee_name, t_loan.date, t_loan.invoice, t_cash_advance_payment.date, t_cash_advance_payment.invoice";

        private IDapperContext _context;
        private ILog _log;

        public ReportLoanRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportLoanHeader> GetByMonth(int month, int year)
        {
            IList<ReportLoanHeader> oList = new List<ReportLoanHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM t_loan.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_loan.date) = @year");

                oList = _context.db.Query<ReportLoanHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportLoanHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportLoanHeader> oList = new List<ReportLoanHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_loan.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_loan.date) = @year");

                oList = _context.db.Query<ReportLoanHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportLoanHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportLoanHeader> oList = new List<ReportLoanHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_loan.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportLoanHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportLoanDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportLoanDetail> oList = new List<ReportLoanDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM t_loan.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_loan.date) = @year");

                oList = _context.db.Query<ReportLoanDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportLoanDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportLoanDetail> oList = new List<ReportLoanDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_loan.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_loan.date) = @year");

                oList = _context.db.Query<ReportLoanDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportLoanDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportLoanDetail> oList = new List<ReportLoanDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_loan.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportLoanDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
