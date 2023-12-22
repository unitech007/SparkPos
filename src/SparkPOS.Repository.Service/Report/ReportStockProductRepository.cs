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
using SparkPOS.Model;

namespace SparkPOS.Repository.Service.Report
{
    public class ReportStockProductRepository : IReportStockProductRepository
    {
        private const string SQL_TEMPLATE_STOCK_PRODUK = @"SELECT m_product.product_id, m_product.product_name, m_product.unit, m_product.stock, m_product.warehouse_stock, m_product.purchase_price, m_product.selling_price, 
                                                          m_category.category_id, m_category.name_category
                                                          FROM public.m_category INNER JOIN public.m_product ON m_product.category_id = m_category.category_id
                                                          {WHERE}
                                                          ORDER BY m_product.product_name";

        private const string SQL_TEMPLATE_STOCK_PRODUK_BY_SUPPLIER = @"SELECT m_product.product_id, m_product.product_name, m_product.unit, m_product.stock, m_product.warehouse_stock, m_product.purchase_price, m_product.selling_price, 
                                                                      m_category.category_id, m_category.name_category
                                                                      FROM public.m_category INNER JOIN public.m_product ON m_product.category_id = m_category.category_id
                                                                      INNER JOIN public.t_purchase_order_item ON t_purchase_order_item.product_id = m_product.product_id
                                                                      INNER JOIN public.t_purchase_product ON t_purchase_order_item.purchase_id = t_purchase_product.purchase_id
                                                                      {WHERE}
                                                                      ORDER BY m_product.product_name";

        private const string SQL_TEMPLATE_PENYESUAIAN_STOCK = @"SELECT t_stock_adjustment.stock_adjustment_id, t_stock_adjustment.date, t_stock_adjustment.stock_addition, t_stock_adjustment.stock_reduction, t_stock_adjustment.warehouse_stock_addition, t_stock_adjustment.warehouse_stock_reduction, t_stock_adjustment.description, 
                                                               m_product.product_id, m_product.product_name, m_alasan_penyesuaian_stock.stock_adjustment_reason_id, m_alasan_penyesuaian_stock.reason
                                                               FROM public.m_product INNER JOIN public.t_stock_adjustment ON t_stock_adjustment.product_id = m_product.product_id
                                                               INNER JOIN public.adjustment_reason; ON t_stock_adjustment.adjustment_reason_id = m_alasan_penyesuaian_stock.stock_adjustment_reason_id
                                                               {WHERE}
                                                               ORDER BY t_stock_adjustment.date, m_product.product_name";

        private IDapperContext _context;
        private ILog _log;
        
        private string _sql;

        public ReportStockProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IList<PriceWholesale> GetListPriceWholesale(string produkId)
        {
            IPriceWholesaleRepository repo = new PriceWholesaleRepository(_context, _log);

            return repo.GetListPriceWholesale(produkId);
        }

        private void SetPriceWholesale(IList<ReportStockProduct> oList)
        {
            foreach (var item in oList)
            {
                var listOfPriceWholesale = GetListPriceWholesale(item.product_id);
                if (listOfPriceWholesale.Count == 3)
                {
                    item.harga_grosir1 = listOfPriceWholesale[0].wholesale_price;
                    item.harga_grosir2 = listOfPriceWholesale[1].wholesale_price;
                    item.harga_grosir3 = listOfPriceWholesale[2].wholesale_price;
                }
            }
        }

        public IList<ReportStockProduct> GetStockByStatus(StatusStock statusStock)
        {
            IList<ReportStockProduct> oList = new List<ReportStockProduct>();

            try
            {
                switch (statusStock)
                {
                    case StatusStock.there:
                        _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "WHERE (m_product.stock + m_product.warehouse_stock) > 0");
                        break;

                    case StatusStock.Empty:
                        _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "WHERE (m_product.stock + m_product.warehouse_stock) <= 0");
                        break;

                    default: // semua product
                        _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "");
                        break;
                }

                oList = _context.db.Query<ReportStockProduct>(_sql).ToList();

                if (oList.Count > 0)
                    SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
        
        public IList<ReportStockProduct> GetStockLessFrom(double stock)
        {
            IList<ReportStockProduct> oList = new List<ReportStockProduct>();

            try
            {
                _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "WHERE (m_product.stock + m_product.warehouse_stock) < @stock");

                oList = _context.db.Query<ReportStockProduct>(_sql, new { stock }).ToList();

                if (oList.Count > 0)
                    SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedSupplier(string supplierId)
        {
            IList<ReportStockProduct> oList = new List<ReportStockProduct>();

            try
            {
                _sql = SQL_TEMPLATE_STOCK_PRODUK_BY_SUPPLIER.Replace("{WHERE}", "WHERE t_purchase_product.supplier_id = @supplierId");

                oList = _context.db.Query<ReportStockProduct>(_sql, new { supplierId }).ToList();

                if (oList.Count > 0)
                    SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedCategory(string golonganId)
        {
            IList<ReportStockProduct> oList = new List<ReportStockProduct>();

            try
            {
                _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "WHERE m_category.category_id = @golonganId");

                oList = _context.db.Query<ReportStockProduct>(_sql, new { golonganId }).ToList();

                if (oList.Count > 0)
                    SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedCode(IList<string> listOfCode)
        {
            IList<ReportStockProduct> oList = new List<ReportStockProduct>();

            try
            {
                var sb = new StringBuilder();

                foreach (var item in listOfCode)
                {
                    sb.Append("'").Append(item).Append("'").Append(",");
                }

                var param = sb.ToString();
                param = param.Substring(0, param.Length - 1);

                _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "WHERE LOWER(m_product.product_code) IN (" + param + ")");

                oList = _context.db.Query<ReportStockProduct>(_sql ).ToList();

                if (oList.Count > 0)
                    SetPriceWholesale(oList);          
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedName(string name)
        {
            IList<ReportStockProduct> oList = new List<ReportStockProduct>();

            try
            {
                name = "%" + name.ToLower() + "%";
                _sql = SQL_TEMPLATE_STOCK_PRODUK.Replace("{WHERE}", "WHERE LOWER(m_product.product_name) LIKE @name");

                oList = _context.db.Query<ReportStockProduct>(_sql, new { name }).ToList();

                if (oList.Count > 0)
                    SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportAdjustmentStockProduct> GetAdjustmentStockByMonth(int month, int year)
        {
            IList<ReportAdjustmentStockProduct> oList = new List<ReportAdjustmentStockProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PENYESUAIAN_STOCK);

                whereBuilder.Add("EXTRACT(MONTH FROM t_stock_adjustment.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_stock_adjustment.date) = @year");

                oList = _context.db.Query<ReportAdjustmentStockProduct>(whereBuilder.ToSql(), new { month, year }).ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportAdjustmentStockProduct> GetAdjustmentStockByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportAdjustmentStockProduct> oList = new List<ReportAdjustmentStockProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PENYESUAIAN_STOCK);

                whereBuilder.Add("t_stock_adjustment.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportAdjustmentStockProduct>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }        
    }
}
