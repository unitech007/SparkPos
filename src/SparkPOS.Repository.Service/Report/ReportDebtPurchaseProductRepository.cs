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
    public class ReportDebtPurchaseProductRepository : IReportDebtPurchaseProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT m_supplier.supplier_id, m_supplier.name_supplier, SUM(t_purchase_product.tax) AS tax, SUM(t_purchase_product.discount) AS discount, SUM(t_purchase_product.total_invoice) AS total_invoice, SUM(t_purchase_product.total_payment) AS total_payment
                                                     FROM public.m_supplier INNER JOIN public.t_purchase_product ON t_purchase_product.supplier_id = m_supplier.supplier_id                                                     
                                                     {WHERE}
                                                     GROUP BY m_supplier.supplier_id, m_supplier.name_supplier
                                                     HAVING (SUM(t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) - SUM(t_purchase_product.total_payment)) <> 0
                                                     ORDER BY m_supplier.name_supplier";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT t_purchase_product.purchase_id, t_purchase_product.invoice, t_purchase_product.date, t_purchase_product.due_date, t_purchase_product.tax, t_purchase_product.discount, t_purchase_product.total_invoice, t_purchase_product.total_payment, 
                                                     m_supplier.supplier_id, m_supplier.name_supplier
                                                     FROM public.m_supplier INNER JOIN public.t_purchase_product ON t_purchase_product.supplier_id = m_supplier.supplier_id
                                                     {WHERE}
                                                     ORDER BY m_supplier.name_supplier, t_purchase_product.date, t_purchase_product.invoice";

        private IDapperContext _context;
        private ILog _log;

        public ReportDebtPurchaseProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportDebtProductPurchaseHeader> GetByMonth(int month, int year)
        {
            IList<ReportDebtProductPurchaseHeader> oList = new List<ReportDebtProductPurchaseHeader>();
            
            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_purchase_product.due_date IS NOT NULL");
                whereBuilder.Add("EXTRACT(MONTH FROM t_purchase_product.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportDebtProductPurchaseHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportDebtProductPurchaseHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportDebtProductPurchaseHeader> oList = new List<ReportDebtProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_purchase_product.due_date IS NOT NULL");
                whereBuilder.Add("(EXTRACT(MONTH FROM t_purchase_product.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportDebtProductPurchaseHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportDebtProductPurchaseHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportDebtProductPurchaseHeader> oList = new List<ReportDebtProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_purchase_product.due_date IS NOT NULL");
                whereBuilder.Add("t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportDebtProductPurchaseHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportDebtProductPurchaseDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportDebtProductPurchaseDetail> oList = new List<ReportDebtProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_purchase_product.due_date IS NOT NULL");
                whereBuilder.Add("((t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) - t_purchase_product.total_payment) <> 0");
                whereBuilder.Add("EXTRACT(MONTH FROM t_purchase_product.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportDebtProductPurchaseDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportDebtProductPurchaseDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportDebtProductPurchaseDetail> oList = new List<ReportDebtProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_purchase_product.due_date IS NOT NULL");
                whereBuilder.Add("((t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) - t_purchase_product.total_payment) <> 0");
                whereBuilder.Add("(EXTRACT(MONTH FROM t_purchase_product.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_purchase_product.date) = @year");

                oList = _context.db.Query<ReportDebtProductPurchaseDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportDebtProductPurchaseDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportDebtProductPurchaseDetail> oList = new List<ReportDebtProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_purchase_product.due_date IS NOT NULL");
                whereBuilder.Add("((t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) - t_purchase_product.total_payment) <> 0");
                whereBuilder.Add("t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportDebtProductPurchaseDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
