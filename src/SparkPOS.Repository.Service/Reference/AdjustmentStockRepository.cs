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
using System.Threading.Tasks;

using log4net;
using Dapper;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class AdjustmentStockRepository : IAdjustmentStockRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_stock_adjustment.stock_adjustment_id, t_stock_adjustment.date, t_stock_adjustment.stock_addition, t_stock_adjustment.stock_reduction, 
                                              t_stock_adjustment.warehouse_stock_addition, t_stock_adjustment.warehouse_stock_reduction, t_stock_adjustment.description, t_stock_adjustment.system_date, 
                                              m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.stock, m_product.warehouse_stock,
                                              m_alasan_penyesuaian_stock.stock_adjustment_reason_id, m_alasan_penyesuaian_stock.reason
                                              FROM public.m_adjustment_reason INNER JOIN public.t_stock_adjustment ON t_stock_adjustment.adjustment_reason_id = m_alasan_penyesuaian_stock.stock_adjustment_reason_id 
                                              INNER JOIN public.m_product ON t_stock_adjustment.product_id = m_product.product_id
                                              {WHERE}
                                              {ORDER BY}";

        private IDapperContext _context;
		private ILog _log;
        private string _sql;

        public AdjustmentStockRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<AdjustmentStock> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<AdjustmentStock> oList = _context.db.Query<AdjustmentStock, Product, ReasonAdjustmentStock, AdjustmentStock>(sql, (ps, p, ap) =>
            {
                ps.product_id = p.product_id; ps.Product = p;
                ps.adjustment_reason_id = ap.stock_adjustment_reason_id; ps.ReasonAdjustmentStock = ap;

                return ps;
            }, param, splitOn: "product_id, stock_adjustment_reason_id");

            return oList;
        }

        public AdjustmentStock GetByID(string id)
        {
            AdjustmentStock obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_stock_adjustment.stock_adjustment_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<AdjustmentStock> GetByName(string name)
        {
            IList<AdjustmentStock> oList = new List<AdjustmentStock>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_product.product_name) LIKE @name OR LOWER(t_stock_adjustment.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_stock_adjustment.date");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<AdjustmentStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<AdjustmentStock> oList = new List<AdjustmentStock>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_stock_adjustment.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_stock_adjustment.date");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<AdjustmentStock> GetAll()
        {
            IList<AdjustmentStock> oList = new List<AdjustmentStock>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_stock_adjustment.date");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(AdjustmentStock obj)
        {
            var result = 0;

            try
            {
                if (obj.stock_adjustment_id == null)
                    obj.stock_adjustment_id = _context.GetGUID();

                _context.db.Insert<AdjustmentStock>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Update(AdjustmentStock obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<AdjustmentStock>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(AdjustmentStock obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<AdjustmentStock>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }        
    }
}     
