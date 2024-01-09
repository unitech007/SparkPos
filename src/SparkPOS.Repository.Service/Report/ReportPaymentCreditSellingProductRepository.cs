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
    public class ReportPaymentCreditSellingProductRepository : IReportPaymentCreditSellingProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT c.customer_id, c.name_customer, p.date, SUM(i.amount) AS total_payment, p.description
                                                     FROM public.t_product_receivable_payment p LEFT JOIN public.m_customer c ON p.customer_id = c.customer_id
                                                     INNER JOIN public.t_credit_payment_item i ON i.pay_sale_id = p.pay_sale_id
                                                     {WHERE}
                                                     GROUP BY c.customer_id, c.name_customer, p.date, p.description
                                                     ORDER BY p.date, c.name_customer";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT c.customer_id, c.name_customer, j.invoice AS nota_jual, p.invoice AS nota_pay, p.date, j.tax, j.discount, j.shipping_cost, j.total_invoice, 
                                                     i.amount AS pelunasan, j.description AS keterangan_jual, p.description AS keterangan_pay
                                                     FROM public.t_product_sales j INNER JOIN public.t_credit_payment_item i ON i.sale_id = j.sale_id
                                                     LEFT JOIN public.m_customer c ON j.customer_id = c.customer_id
                                                     INNER JOIN public.t_product_receivable_payment p ON i.pay_sale_id = p.pay_sale_id
                                                     {WHERE}
                                                     ORDER BY c.name_customer, p.date, p.invoice";






        private IDapperContext _context;
        private ILog _log;

        public ReportPaymentCreditSellingProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportPaymentCreditSalesProductHeader> GetByMonth(int month, int year)
        {
            IList<ReportPaymentCreditSalesProductHeader> oList = new List<ReportPaymentCreditSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM p.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportPaymentCreditSalesProductHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportPaymentCreditSalesProductHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportPaymentCreditSalesProductHeader> oList = new List<ReportPaymentCreditSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM p.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportPaymentCreditSalesProductHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportPaymentCreditSalesProductHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportPaymentCreditSalesProductHeader> oList = new List<ReportPaymentCreditSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("p.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportPaymentCreditSalesProductHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportPaymentCreditSalesProductDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportPaymentCreditSalesProductDetail> oList = new List<ReportPaymentCreditSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM p.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportPaymentCreditSalesProductDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportPaymentCreditSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportPaymentCreditSalesProductDetail> oList = new List<ReportPaymentCreditSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM p.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportPaymentCreditSalesProductDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportPaymentCreditSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportPaymentCreditSalesProductDetail> oList = new List<ReportPaymentCreditSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("p.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportPaymentCreditSalesProductDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }        
    }
}
