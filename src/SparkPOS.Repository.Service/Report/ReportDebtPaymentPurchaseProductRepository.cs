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
    public class ReportDebtPaymentPurchaseProductRepository : IReportDebtPaymentPurchaseProductRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT s.supplier_id, s.name_supplier, p.date, SUM(i.amount) AS total_payment, p.description
                                                     FROM public.t_product_payable_payment p INNER JOIN public.m_supplier s ON p.supplier_id = s.supplier_id
                                                     INNER JOIN public.t_debt_payment_item i ON i.pay_purchase_id = p.pay_purchase_id
                                                     {WHERE}
                                                     GROUP BY s.supplier_id, s.name_supplier, p.date, p.description
                                                     ORDER BY p.date, s.name_supplier";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT s.supplier_id, s.name_supplier, b.invoice AS nota_beli, p.invoice AS nota_pay, p.date, b.tax, b.discount, b.total_invoice, i.amount AS pelunasan, b.description AS keterangan_beli, p.description AS keterangan_pay
                                                     FROM public.t_purchase_product b INNER JOIN public.t_debt_payment_item i ON i.purchase_id = b.purchase_id
                                                     INNER JOIN public.m_supplier s ON b.supplier_id = s.supplier_id
                                                     INNER JOIN public.t_product_payable_payment p ON i.pay_purchase_id = p.pay_purchase_id
                                                     {WHERE}
                                                     ORDER BY s.name_supplier, p.date, p.invoice";

        private IDapperContext _context;
        private ILog _log;

        public ReportDebtPaymentPurchaseProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportDebtPaymentProductPurchaseHeader> GetByMonth(int month, int year)
        {
            IList<ReportDebtPaymentProductPurchaseHeader> oList = new List<ReportDebtPaymentProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM p.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportDebtPaymentProductPurchaseHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportDebtPaymentProductPurchaseHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportDebtPaymentProductPurchaseHeader> oList = new List<ReportDebtPaymentProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM p.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportDebtPaymentProductPurchaseHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportDebtPaymentProductPurchaseHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportDebtPaymentProductPurchaseHeader> oList = new List<ReportDebtPaymentProductPurchaseHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("p.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportDebtPaymentProductPurchaseHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportDebtPaymentProductPurchaseDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportDebtPaymentProductPurchaseDetail> oList = new List<ReportDebtPaymentProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM p.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportDebtPaymentProductPurchaseDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportDebtPaymentProductPurchaseDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportDebtPaymentProductPurchaseDetail> oList = new List<ReportDebtPaymentProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM p.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM p.date) = @year");

                oList = _context.db.Query<ReportDebtPaymentProductPurchaseDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportDebtPaymentProductPurchaseDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportDebtPaymentProductPurchaseDetail> oList = new List<ReportDebtPaymentProductPurchaseDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("p.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportDebtPaymentProductPurchaseDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
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
