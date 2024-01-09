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
    public class ReportCreditSellingProductRepository : IReportCreditSellingProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT m_customer.customer_id, m_customer.name_customer, SUM(t_product_sales.tax) AS tax, SUM(t_product_sales.shipping_cost) AS shipping_cost, SUM(t_product_sales.discount) AS discount, 
                                                     SUM(t_product_sales.total_invoice) AS total_invoice, SUM(t_product_sales.total_payment) AS total_payment
                                                     FROM public.m_customer RIGHT JOIN public.t_product_sales ON t_product_sales.customer_id = m_customer.customer_id
                                                     {WHERE}
                                                     GROUP BY m_customer.customer_id, m_customer.name_customer
                                                     HAVING SUM(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - SUM(t_product_sales.total_payment) <> 0
                                                     ORDER BY m_customer.name_customer";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT m_customer.customer_id, m_customer.name_customer, t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, 
                                                     t_product_sales.tax, t_product_sales.shipping_cost, t_product_sales.discount, t_product_sales.total_invoice, t_product_sales.total_payment
                                                     FROM public.m_customer RIGHT JOIN public.t_product_sales ON t_product_sales.customer_id = m_customer.customer_id
                                                     {WHERE}
                                                     ORDER BY m_customer.name_customer, t_product_sales.date, t_product_sales.invoice";

        private const string SQL_TEMPLATE_PER_PRODUK = @"SELECT m_customer.customer_id, m_customer.name_customer,
                                                         t_product_sales.sale_id, t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, t_product_sales.tax, t_product_sales.shipping_cost, t_product_sales.discount AS diskon_nota, 
                                                         t_product_sales.total_invoice, t_product_sales.total_payment,
                                                         m_product.product_id, m_product.product_name, m_product.unit, t_sales_order_item.quantity, t_sales_order_item.return_quantity, t_sales_order_item.discount, t_sales_order_item.selling_price
                                                         FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                         LEFT JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                                         INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                                         {WHERE}
                                                         ORDER BY m_customer.name_customer, t_product_sales.date, t_product_sales.invoice, m_product.product_name";

        private IDapperContext _context;
        private ILog _log;

        public ReportCreditSellingProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportCreditSalesProductHeader> GetByMonth(int month, int year)
        {
            IList<ReportCreditSalesProductHeader> oList = new List<ReportCreditSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportCreditSalesProductHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCreditSalesProductHeader> oList = new List<ReportCreditSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(EXTRACT(MONTH FROM t_product_sales.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");
                
                oList = _context.db.Query<ReportCreditSalesProductHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCreditSalesProductHeader> oList = new List<ReportCreditSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportCreditSalesProductHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportCreditSalesProductDetail> oList = new List<ReportCreditSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - t_product_sales.total_payment <> 0");
                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportCreditSalesProductDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCreditSalesProductDetail> oList = new List<ReportCreditSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - t_product_sales.total_payment <> 0");
                whereBuilder.Add("(EXTRACT(MONTH FROM t_product_sales.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportCreditSalesProductDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCreditSalesProductDetail> oList = new List<ReportCreditSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - t_product_sales.total_payment <> 0");
                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportCreditSalesProductDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProduct> PerProductGetByMonth(int month, int year)
        {
            IList<ReportCreditSalesProduct> oList = new List<ReportCreditSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_PRODUK);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - t_product_sales.total_payment <> 0");
                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportCreditSalesProduct>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProduct> PerProductGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCreditSalesProduct> oList = new List<ReportCreditSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_PRODUK);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - t_product_sales.total_payment <> 0");
                whereBuilder.Add("(EXTRACT(MONTH FROM t_product_sales.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportCreditSalesProduct>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCreditSalesProduct> PerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCreditSalesProduct> oList = new List<ReportCreditSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_PRODUK);

                whereBuilder.Add("t_product_sales.due_date IS NOT NULL");
                whereBuilder.Add("(t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.tax + t_product_sales.shipping_cost) - t_product_sales.total_payment <> 0");
                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportCreditSalesProduct>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
