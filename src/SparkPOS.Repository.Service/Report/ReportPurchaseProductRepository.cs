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
    public class ReportPurchaseProductRepository : IReportPurchaseProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT t_purchase_product.date, t_purchase_product.due_date, t_purchase_product.invoice, m_supplier.supplier_id, m_supplier.name_supplier, 
                                                     t_purchase_product.total_invoice, t_purchase_product.discount, t_purchase_product.tax, t_purchase_product.total_payment
                                                     FROM public.t_purchase_product INNER JOIN public.m_supplier ON t_purchase_product.supplier_id = m_supplier.supplier_id
                                                     {WHERE}
                                                     ORDER BY t_purchase_product.date, t_purchase_product.invoice";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT m_supplier.supplier_id, m_supplier.name_supplier, t_purchase_product.date, t_purchase_product.invoice, m_product.product_id, m_product.product_name, m_product.unit, t_purchase_order_item.quantity, t_purchase_order_item.return_quantity, t_purchase_order_item.price, t_purchase_order_item.discount 
                                                     FROM public.t_purchase_product INNER JOIN public.m_supplier ON m_supplier.supplier_id = t_purchase_product.supplier_id
                                                     INNER JOIN public.t_purchase_order_item ON t_purchase_order_item.purchase_id = t_purchase_product.purchase_id
                                                     INNER JOIN public.m_product ON t_purchase_order_item.product_id = m_product.product_id
                                                     {WHERE}
                                                     ORDER BY t_purchase_product.date, t_purchase_product.invoice, m_product.product_name";

        private IDapperContext _context;
        private ILog _log;

        public ReportPurchaseProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportProductPurchaseHeader> GetByMonth(int month, int year)
        {
            IList<ReportProductPurchaseHeader> oList = new List<ReportProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM t_purchase_product.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportProductPurchaseHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportProductPurchaseHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportProductPurchaseHeader> oList = new List<ReportProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_purchase_product.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportProductPurchaseHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportProductPurchaseHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportProductPurchaseHeader> oList = new List<ReportProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportProductPurchaseHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportProductPurchaseDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportProductPurchaseDetail> oList = new List<ReportProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM t_purchase_product.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportProductPurchaseDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportProductPurchaseDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportProductPurchaseDetail> oList = new List<ReportProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_purchase_product.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportProductPurchaseDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportProductPurchaseDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportProductPurchaseDetail> oList = new List<ReportProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportProductPurchaseDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
