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
    public class ReportReturnPurchaseProductRepository : IReportReturnPurchaseProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT m_supplier.supplier_id, m_supplier.name_supplier, t_purchase_return.invoice AS nota_return, t_purchase_return.date AS tanggal_return, t_purchase_return.total_invoice AS total_return, t_purchase_return.description,
                                                     t_purchase_product.invoice as nota_beli
                                                     FROM public.t_purchase_return INNER JOIN public.t_purchase_product ON t_purchase_return.purchase_id = t_purchase_product.purchase_id
                                                     INNER JOIN public.m_supplier ON t_purchase_return.supplier_id = m_supplier.supplier_id
                                                     {WHERE}
                                                     ORDER BY t_purchase_return.date, t_purchase_return.invoice";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT m_supplier.supplier_id, m_supplier.name_supplier, 
                                                     t_purchase_return.invoice AS nota_return, t_purchase_return.date AS tanggal_return, t_purchase_return_item.return_quantity, t_purchase_return_item.price, 
                                                     t_purchase_product.invoice AS nota_beli, m_product.product_name, m_product.unit
                                                     FROM public.t_purchase_return INNER JOIN public.t_purchase_product ON t_purchase_return.purchase_id = t_purchase_product.purchase_id
                                                     INNER JOIN public.m_supplier ON t_purchase_return.supplier_id = m_supplier.supplier_id
                                                     INNER JOIN public.t_purchase_return_item ON t_purchase_return_item.purchase_return_id = t_purchase_return.purchase_return_id
                                                     INNER JOIN public.m_product ON t_purchase_return_item.product_id = m_product.product_id
                                                     {WHERE}
                                                     ORDER BY t_purchase_return.date, t_purchase_return.invoice, m_product.product_name";

        private IDapperContext _context;
        private ILog _log;

        public ReportReturnPurchaseProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportReturnProductPurchaseHeader> GetByMonth(int month, int year)
        {
            IList<ReportReturnProductPurchaseHeader> oList = new List<ReportReturnProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM t_purchase_return.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_return.date) = @year");

                oList = _context.db.Query<ReportReturnProductPurchaseHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportReturnProductPurchaseHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportReturnProductPurchaseHeader> oList = new List<ReportReturnProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_purchase_return.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_return.date) = @year");

                oList = _context.db.Query<ReportReturnProductPurchaseHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportReturnProductPurchaseHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportReturnProductPurchaseHeader> oList = new List<ReportReturnProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_purchase_return.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportReturnProductPurchaseHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportReturnProductPurchaseDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportReturnProductPurchaseDetail> oList = new List<ReportReturnProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM t_purchase_return.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_return.date) = @year");

                oList = _context.db.Query<ReportReturnProductPurchaseDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportReturnProductPurchaseDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportReturnProductPurchaseDetail> oList = new List<ReportReturnProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_purchase_return.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_return.date) = @year");

                oList = _context.db.Query<ReportReturnProductPurchaseDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportReturnProductPurchaseDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportReturnProductPurchaseDetail> oList = new List<ReportReturnProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_purchase_return.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportReturnProductPurchaseDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
