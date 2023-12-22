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
    public class ReportCardStockRepository : IReportCardStockRepository
    {
        private const string SQL_TEMPLATE = @"SELECT 1 AS type_nota, m_product.product_id, m_product.product_name, m_product.stock + m_product.warehouse_stock AS stock_akhir, t_purchase_product.invoice, t_purchase_product.date, 
                                              m_supplier.name_supplier AS supplier_or_customer, SUM(t_purchase_order_item.quantity) AS qty, COALESCE(t_purchase_product.description, t_purchase_product.description, '') AS description
                                              FROM public.t_purchase_product INNER JOIN public.t_purchase_order_item ON t_purchase_order_item.purchase_id = t_purchase_product.purchase_id
                                              INNER JOIN public.m_supplier ON t_purchase_product.supplier_id = m_supplier.supplier_id
                                              INNER JOIN public.m_product ON t_purchase_order_item.product_id = m_product.product_id
                                              {WHERE_1}
                                              GROUP BY m_product.product_id, m_product.product_name, t_purchase_product.invoice, t_purchase_product.date, m_supplier.name_supplier, COALESCE(t_purchase_product.description, t_purchase_product.description, '')
                                              UNION
                                              SELECT 2 AS type_nota, m_product.product_id, m_product.product_name, m_product.stock + m_product.warehouse_stock AS stock_akhir, t_sales_return.invoice, t_sales_return.date, 
                                              m_customer.name_customer AS supplier_or_customer, SUM(t_sales_return_item.return_quantity) AS qty, COALESCE(t_sales_return.description, t_sales_return.description, '') AS description
                                              FROM public.t_sales_return INNER JOIN public.t_sales_return_item ON t_sales_return_item.return_sale_id = t_sales_return.return_sale_id
                                              INNER JOIN public.m_product ON t_sales_return_item.product_id = m_product.product_id
                                              INNER JOIN public.m_customer ON t_sales_return.customer_id = m_customer.customer_id
                                              {WHERE_2}
                                              GROUP BY m_product.product_id, m_product.product_name, t_sales_return.invoice, t_sales_return.date, m_customer.name_customer, COALESCE(t_sales_return.description, t_sales_return.description, '')
                                              UNION 
                                              SELECT 3 AS type_nota, m_product.product_id, m_product.product_name, m_product.stock + m_product.warehouse_stock AS stock_akhir, t_product_sales.invoice, t_product_sales.date, 
                                              m_customer.name_customer AS supplier_or_customer, SUM(t_sales_order_item.quantity) AS qty, COALESCE(t_product_sales.description, t_product_sales.description, '') AS description
                                              FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                              INNER JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                              INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                              {WHERE_3}
                                              GROUP BY m_product.product_id, m_product.product_name, t_product_sales.invoice, t_product_sales.date, m_customer.name_customer, COALESCE(t_product_sales.description, t_product_sales.description, '')
                                              UNION
                                              SELECT 4 AS type_nota, m_product.product_id, m_product.product_name, m_product.stock + m_product.warehouse_stock AS stock_akhir, t_purchase_return.invoice, t_purchase_return.date, 
                                              m_supplier.name_supplier AS supplier_or_customer, SUM(t_purchase_return_item.return_quantity) AS qty, COALESCE(t_purchase_return.description, t_purchase_return.description, '') AS description
                                              FROM public.t_purchase_return INNER JOIN public.t_purchase_return_item ON t_purchase_return_item.purchase_return_id = t_purchase_return.purchase_return_id
                                              INNER JOIN public.m_product ON t_purchase_return_item.product_id = m_product.product_id
                                              INNER JOIN public.m_supplier ON t_purchase_return.supplier_id = m_supplier.supplier_id
                                              {WHERE_4}
                                              GROUP BY m_product.product_id, m_product.product_name, t_purchase_return.invoice, t_purchase_return.date, m_supplier.name_supplier, COALESCE(t_purchase_return.description, t_purchase_return.description, '')
                                              ORDER BY 3, 5, 1";

        private IDapperContext _context;
        private ILog _log;
        private string _sql;
        private string _where;

        public ReportCardStockRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportCardStock> GetSaldoAwal(DateTime date)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            try
            {
                _where = @"WHERE t_purchase_product.date < @date";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE t_sales_return.date < @date";
                _sql = _sql.Replace("{WHERE_2}", _where);

                _where = @"WHERE t_product_sales.date < @date";
                _sql = _sql.Replace("{WHERE_3}", _where);

                _where = @"WHERE t_purchase_return.date < @date";
                _sql = _sql.Replace("{WHERE_4}", _where);

                oList = _context.db.Query<ReportCardStock>(_sql, new { date })
                                .ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCardStock> GetByMonth(int month, int year)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            try
            {
                _where = @"WHERE EXTRACT(MONTH FROM t_purchase_product.date) = @month AND EXTRACT(YEAR FROM t_purchase_product.date) = @year";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE EXTRACT(MONTH FROM t_sales_return.date) = @month AND EXTRACT(YEAR FROM t_sales_return.date) = @year";
                _sql = _sql.Replace("{WHERE_2}", _where);

                _where = @"WHERE EXTRACT(MONTH FROM t_product_sales.date) = @month AND EXTRACT(YEAR FROM t_product_sales.date) = @year";
                _sql = _sql.Replace("{WHERE_3}", _where);

                _where = @"WHERE EXTRACT(MONTH FROM t_purchase_return.date) = @month AND EXTRACT(YEAR FROM t_purchase_return.date) = @year";
                _sql = _sql.Replace("{WHERE_4}", _where);

                oList = _context.db.Query<ReportCardStock>(_sql, new { month, year })
                                .ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCardStock> GetByMonth(int month, int year, IList<string> listOfCode)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            try
            {
                var sb = new StringBuilder();

                foreach (var item in listOfCode)
                {
                    sb.Append("'").Append(item).Append("'").Append(",");
                }

                var param = sb.ToString();
                param = param.Substring(0, param.Length - 1);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND EXTRACT(MONTH FROM t_purchase_product.date) = @month AND EXTRACT(YEAR FROM t_purchase_product.date) = @year";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND EXTRACT(MONTH FROM t_sales_return.date) = @month AND EXTRACT(YEAR FROM t_sales_return.date) = @year";
                _sql = _sql.Replace("{WHERE_2}", _where);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND EXTRACT(MONTH FROM t_product_sales.date) = @month AND EXTRACT(YEAR FROM t_product_sales.date) = @year";
                _sql = _sql.Replace("{WHERE_3}", _where);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND EXTRACT(MONTH FROM t_purchase_return.date) = @month AND EXTRACT(YEAR FROM t_purchase_return.date) = @year";
                _sql = _sql.Replace("{WHERE_4}", _where);

                oList = _context.db.Query<ReportCardStock>(_sql, new { month, year })
                                .ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCardStock> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportCardStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            try
            {
                _where = @"WHERE t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = @"WHERE t_sales_return.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = _sql.Replace("{WHERE_2}", _where);

                _where = @"WHERE t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = _sql.Replace("{WHERE_3}", _where);

                _where = @"WHERE t_purchase_return.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = _sql.Replace("{WHERE_4}", _where);

                oList = _context.db.Query<ReportCardStock>(_sql, new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCardStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, IList<string> listOfCode)
        {            
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            try
            {
                var sb = new StringBuilder();

                foreach (var item in listOfCode)
                {
                    sb.Append("'").Append(item).Append("'").Append(",");
                }

                var param = sb.ToString();
                param = param.Substring(0, param.Length - 1);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", _where);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND t_sales_return.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = _sql.Replace("{WHERE_2}", _where);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND  t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = _sql.Replace("{WHERE_3}", _where);

                _where = "WHERE LOWER(m_product.product_code) IN (" + param + ") AND t_purchase_return.date BETWEEN @tanggalMulai AND @tanggalSelesai";
                _sql = _sql.Replace("{WHERE_4}", _where);

                oList = _context.db.Query<ReportCardStock>(_sql, new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCardStock> GetAll(IList<string> listOfProductId)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE_1}", "");
                _sql = _sql.Replace("{WHERE_2}", "");
                _sql = _sql.Replace("{WHERE_3}", "");
                _sql = _sql.Replace("{WHERE_4}", "");

                oList = _context.db.Query<ReportCardStock>(_sql)
                                .Where(f => listOfProductId.Contains(f.product_id))
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
