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
    public class ReportReturnSellingProductRepository : IReportReturnSellingProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT m_customer.customer_id, m_customer.name_customer, t_sales_return.invoice AS nota_return, 
                                                     t_sales_return.date AS tanggal_return, t_sales_return.total_invoice AS total_return, t_sales_return.description, t_product_sales.invoice as nota_jual
                                                     FROM public.t_sales_return INNER JOIN public.t_product_sales ON t_sales_return.sale_id = t_product_sales.sale_id
                                                     INNER JOIN public.m_customer ON t_sales_return.customer_id = m_customer.customer_id
                                                     {WHERE}
                                                     ORDER BY t_sales_return.date, t_sales_return.invoice";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT m_customer.customer_id, m_customer.name_customer, t_sales_return.invoice AS nota_return, 
                                                     t_sales_return.date AS tanggal_return, t_sales_return_item.return_quantity, t_sales_return_item.selling_price AS price, 
                                                     t_product_sales.invoice AS nota_jual, m_product.product_name, m_product.unit
                                                     FROM public.t_sales_return INNER JOIN public.t_product_sales ON t_sales_return.sale_id = t_product_sales.sale_id
                                                     INNER JOIN public.m_customer ON t_sales_return.customer_id = m_customer.customer_id
                                                     INNER JOIN public.t_sales_return_item ON t_sales_return_item.return_sale_id = t_sales_return.return_sale_id
                                                     INNER JOIN public.m_product ON t_sales_return_item.product_id = m_product.product_id
                                                     {WHERE}
                                                     ORDER BY t_sales_return.date, t_sales_return.invoice, m_product.product_name";

        private IDapperContext _context;
        private ILog _log;

        public ReportReturnSellingProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportReturnSalesProductHeader> GetByMonth(int month, int year)
        {
            IList<ReportReturnSalesProductHeader> oList = new List<ReportReturnSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM t_sales_return.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_sales_return.date) = @year");

                oList = _context.db.Query<ReportReturnSalesProductHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportReturnSalesProductHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportReturnSalesProductHeader> oList = new List<ReportReturnSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_sales_return.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_sales_return.date) = @year");

                oList = _context.db.Query<ReportReturnSalesProductHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportReturnSalesProductHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportReturnSalesProductHeader> oList = new List<ReportReturnSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_sales_return.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportReturnSalesProductHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportReturnSalesProductDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportReturnSalesProductDetail> oList = new List<ReportReturnSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM t_sales_return.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_sales_return.date) = @year");

                oList = _context.db.Query<ReportReturnSalesProductDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportReturnSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportReturnSalesProductDetail> oList = new List<ReportReturnSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_sales_return.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_sales_return.date) = @year");

                oList = _context.db.Query<ReportReturnSalesProductDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportReturnSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportReturnSalesProductDetail> oList = new List<ReportReturnSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_sales_return.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportReturnSalesProductDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
