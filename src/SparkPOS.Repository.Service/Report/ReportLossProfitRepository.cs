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
    public class ReportLossProfitRepository : IReportLossProfitRepository
    {
        private const string SQL_TEMPLATE_PENJUALAN = @"SELECT COALESCE(SUM(total_invoice), SUM(total_invoice), 0)
                                                        FROM t_product_sales
                                                        {WHERE}";

        private const string SQL_TEMPLATE_DISKON_NOTA = @"SELECT COALESCE(SUM(discount), SUM(discount), 0)
                                                          FROM t_product_sales
                                                          {WHERE}";

        private const string SQL_TEMPLATE_DISKON_PRODUK = @"SELECT COALESCE(SUM(t_sales_order_item.quantity - t_sales_order_item.return_quantity), SUM(t_sales_order_item.quantity - t_sales_order_item.return_quantity), 0) * 
                                                            COALESCE(SUM(t_sales_order_item.discount), SUM(t_sales_order_item.discount), 0)
                                                            FROM t_product_sales INNER JOIN t_sales_order_item ON t_product_sales.sale_id = t_sales_order_item.sale_id
                                                            {WHERE}";

        private const string SQL_TEMPLATE_HPP = @"SELECT SUM(t_sales_order_item.purchase_price * (t_sales_order_item.quantity - t_sales_order_item.return_quantity))
                                                  FROM t_product_sales INNER JOIN t_sales_order_item ON t_product_sales.sale_id = t_sales_order_item.sale_id
                                                  {WHERE}";

        private const string SQL_TEMPLATE_RETUR_PENJUALAN = @"SELECT COALESCE(SUM(total_invoice), SUM(total_invoice), 0)
                                                              FROM t_sales_return
                                                              {WHERE}";

        private IDapperContext _context;
        private ILog _log;

        public ReportLossProfitRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private double GetSales(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PENJUALAN);

                whereBuilder.Add("EXTRACT(MONTH FROM date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetSales(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PENJUALAN);

                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetHPP(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HPP);

                whereBuilder.Add("EXTRACT(MONTH FROM date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetHPP(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HPP);

                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetDiskonInvoice(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DISKON_NOTA);

                whereBuilder.Add("EXTRACT(MONTH FROM date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetDiskonInvoice(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DISKON_NOTA);                
                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetDiskonProduct(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DISKON_PRODUK);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetDiskonProduct(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DISKON_PRODUK);
                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetReturnSales(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_RETUR_PENJUALAN);

                whereBuilder.Add("EXTRACT(MONTH FROM date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetReturnSales(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_RETUR_PENJUALAN);
                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public ReportLossProfit GetByMonth(int month, int year)
        {
            ReportLossProfit obj = null;

            try
            {
                var diskonInvoice = GetDiskonInvoice(month, year);
                var diskonProduct = GetDiskonProduct(month, year);
                var retur = GetReturnSales(month, year);

                obj = new ReportLossProfit();
                obj.sales = GetSales(month, year) + diskonProduct + retur;
                obj.discount = diskonInvoice + diskonProduct;
                obj.hpp = GetHPP(month, year);
                obj.return_sales = retur;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public ReportLossProfit GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            ReportLossProfit obj = null;

            try
            {
                var diskonInvoice = GetDiskonInvoice(tanggalMulai, tanggalSelesai);
                var diskonProduct = GetDiskonProduct(tanggalMulai, tanggalSelesai);
                var retur = GetReturnSales(tanggalMulai, tanggalSelesai);

                obj = new ReportLossProfit();
                obj.sales = GetSales(tanggalMulai, tanggalSelesai) + diskonProduct + retur;
                obj.discount = diskonInvoice + diskonProduct;
                obj.hpp = GetHPP(tanggalMulai, tanggalSelesai);
                obj.return_sales = retur;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }
    }
}
