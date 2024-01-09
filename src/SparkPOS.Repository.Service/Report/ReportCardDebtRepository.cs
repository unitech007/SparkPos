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
    public class ReportCardDebtRepository : IReportCardDebtRepository
    {
        private const string SQL_TEMPLATE = @"SELECT m_supplier.supplier_id, m_supplier.name_supplier, t_purchase_product.date, m_product.product_name, m_product.unit, 
                                              SUM(t_purchase_order_item.quantity - t_purchase_order_item.return_quantity) AS quantity, 
                                              (SUM((t_purchase_order_item.price - (t_purchase_order_item.price * t_purchase_order_item.discount / 100)) * (t_purchase_order_item.quantity - t_purchase_order_item.return_quantity)) - t_purchase_product.discount) + t_purchase_product.tax AS total, 1 AS type
                                              FROM public.t_purchase_order_item INNER JOIN public.m_product ON t_purchase_order_item.product_id = m_product.product_id
                                              INNER JOIN public.t_purchase_product ON t_purchase_order_item.purchase_id = t_purchase_product.purchase_id
                                              INNER JOIN public.m_supplier ON m_supplier.supplier_id = t_purchase_product.supplier_id
                                              {WHERE_1}
                                              GROUP BY m_supplier.supplier_id, m_supplier.name_supplier, t_purchase_product.date, t_purchase_product.discount, t_purchase_product.tax, m_product.product_name, m_product.unit                                                   
                                              UNION
                                              SELECT m_supplier.supplier_id, m_supplier.name_supplier, t_product_payable_payment.date, t_product_payable_payment.description AS product_name, '' AS unit, 0 AS quantity, SUM(t_debt_payment_item.amount) AS total, 2 AS type
                                              FROM public.t_product_payable_payment INNER JOIN public.t_debt_payment_item ON t_debt_payment_item.pay_purchase_id = t_product_payable_payment.pay_purchase_id
                                              INNER JOIN public.m_supplier ON m_supplier.supplier_id = t_product_payable_payment.supplier_id
                                              {WHERE_2}
                                              GROUP BY m_supplier.supplier_id, m_supplier.name_supplier, t_product_payable_payment.date, t_product_payable_payment.description
                                              ORDER BY 2, 3, 8";



        private IDapperContext _context;
        private ILog _log;
        private string _sql;
        private string _where;

        public ReportCardDebtRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }        

        public IList<ReportCardDebt> GetSaldoAwal(DateTime date)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            try
            {
                _where = @"WHERE t_purchase_product.due_date IS NOT NULL AND t_purchase_product.date < @date";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE t_product_payable_payment.date < @date AND t_product_payable_payment.is_cash = 'f'";                
                _sql = _sql.Replace("{WHERE_2}", _where);

                oList = _context.db.Query<ReportCardDebt>(_sql, new { date })
                                .ToList();

            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ReportCardDebt> GetByMonth(int month, int year)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            try
            {
                _where = @"WHERE t_purchase_product.due_date IS NOT NULL AND 
                           EXTRACT(MONTH FROM t_purchase_product.date) = @month AND EXTRACT(YEAR FROM t_purchase_product.date) = @year";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE EXTRACT(MONTH FROM t_product_payable_payment.date) = @month AND EXTRACT(YEAR FROM t_product_payable_payment.date) = @year AND 
                           t_product_payable_payment.is_cash = 'f'";
                _sql = _sql.Replace("{WHERE_2}", _where);

                oList = _context.db.Query<ReportCardDebt>(_sql, new { month, year })
                                .ToList();                
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }        

        public IList<ReportCardDebt> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            try
            {                
                _where = @"WHERE t_purchase_product.due_date IS NOT NULL AND 
                           (EXTRACT(MONTH FROM t_purchase_product.date) BETWEEN @StartingMonth AND @EndingMonth) AND EXTRACT(YEAR FROM t_purchase_product.date) = @year";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE (EXTRACT(MONTH FROM t_product_payable_payment.date) BETWEEN @StartingMonth AND @EndingMonth) AND EXTRACT(YEAR FROM t_product_payable_payment.date) = @year AND 
                           t_product_payable_payment.is_cash = 'f'";
                _sql = _sql.Replace("{WHERE_2}", _where);

                oList = _context.db.Query<ReportCardDebt>(_sql, new { StartingMonth, EndingMonth, year })
                                .ToList();                                
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }        

        public IList<ReportCardDebt> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            try
            {
                _where = @"WHERE t_purchase_product.due_date IS NOT NULL AND t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE t_product_payable_payment.date BETWEEN @tanggalMulai AND @tanggalSelesai AND t_product_payable_payment.is_cash = 'f'";
                _sql = _sql.Replace("{WHERE_2}", _where);

                oList = _context.db.Query<ReportCardDebt>(_sql, new { tanggalMulai, tanggalSelesai })
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
