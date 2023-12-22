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
    public class ReturnSellingProductRepository : IReturnSellingProductRepository
    {
        //private const string SQL_TEMPLATE = @"SELECT t_sales_return.return_sale_id, t_sales_return.user_id, t_sales_return.invoice, t_sales_return.date, t_sales_return.description, t_sales_return.system_date, t_sales_return.total_invoice, 
        //                                      m_customer.customer_id, m_customer.name_customer, m_customer.address, t_product_sales.sale_id, t_product_sales.invoice
        //                                      FROM public.m_customer INNER JOIN public.t_sales_return ON t_sales_return.customer_id = m_customer.customer_id
        //                                      INNER JOIN public.t_product_sales ON t_sales_return.sale_id = t_product_sales.sale_id 
        //                                      {WHERE}
        //                                      {ORDER BY}";

        private const string SQL_TEMPLATE = @"SELECT t_sales_return.return_sale_id, t_sales_return.user_id, t_sales_return.invoice, t_sales_return.date, t_sales_return.description, t_sales_return.system_date, t_sales_return.total_invoice, 
                                              m_customer.customer_id, m_customer.name_customer, m_customer.address, t_product_sales.sale_id, t_product_sales.invoice
                                              FROM public.m_customer INNER JOIN public.t_sales_return ON t_sales_return.customer_id = m_customer.customer_id
                                              INNER JOIN public.t_product_sales ON t_sales_return.sale_id = t_product_sales.sale_id 
                                              {WHERE}
                                              {ORDER BY}";

        private IDapperContext _context;
        private ILog _log;
        private string _sql;
		
        public ReturnSellingProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<ReturnSellingProduct> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<ReturnSellingProduct> oList = _context.db.Query<ReturnSellingProduct, Customer, SellingProduct, ReturnSellingProduct>(sql, (rj, c, j) =>
            {
                rj.customer_id = c.customer_id; rj.Customer = c;
                rj.sale_id = j.sale_id; rj.SellingProduct = j;

                return rj;
            }, param, splitOn: "customer_id, sale_id");

            return oList;
        }

        private IList<ItemReturnSellingProduct> GetItemReturn(string returnId)
        {
            IList<ItemReturnSellingProduct> oList = new List<ItemReturnSellingProduct>();

            try
            {
                var sql = @"SELECT t_sales_return_item.return_sale_item_id, t_sales_return_item.return_sale_id, t_sales_return_item.user_id, t_sales_return_item.sale_item_id, t_sales_return_item.selling_price, t_sales_return_item.quantity, t_sales_return_item.return_quantity, t_sales_return_item.system_date, 1 as entity_state,
                            m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.selling_price
                            FROM public.t_sales_return_item INNER JOIN public.m_product ON t_sales_return_item.product_id = m_product.product_id  
                            WHERE t_sales_return_item.return_sale_id = @returnId
                            ORDER BY t_sales_return_item.system_date";

                oList = _context.db.Query<ItemReturnSellingProduct, Product, ItemReturnSellingProduct>(sql, (ir, p) =>
                {
                    ir.product_id = p.product_id; ir.Product = p;
                    return ir;

                }, new { returnId }, splitOn: "product_id").ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public ReturnSellingProduct GetByID(string id)
        {
            ReturnSellingProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_return.return_sale_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();

                if (obj != null)
                    // load item return
                    obj.item_return = GetItemReturn(obj.return_sale_id);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new ReturnSellingProduct().GetTableName());
        }

        public IList<ReturnSellingProduct> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ReturnSellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReturnSellingProduct> oList = new List<ReturnSellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_return.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_return.date DESC, t_sales_return.invoice");

                //oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
                //oList = MappingRecordToObject(_sql, new { tanggalMulai = tanggalMulai.ToString(), tanggalSelesai = tanggalSelesai.ToString() }).ToList();
                oList = MappingRecordToObject(_sql, new { tanggalMulai = tanggalMulai, tanggalSelesai = tanggalSelesai }).ToList();

                // load item beli
                foreach (var item in oList)
                {
                    item.item_return = GetItemReturn(item.return_sale_id);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReturnSellingProduct> GetAll()
        {
            IList<ReturnSellingProduct> oList = new List<ReturnSellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_return.date DESC, t_sales_return.invoice");

                oList = MappingRecordToObject(_sql).ToList();

                // load item beli
                foreach (var item in oList)
                {
                    item.item_return = GetItemReturn(item.return_sale_id);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private double GetTotalInvoice(ReturnSellingProduct obj)
        {
            var total = obj.item_return.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                      .Sum(f => f.return_quantity * f.selling_price);

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        public int Save(ReturnSellingProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.return_sale_id = _context.GetGUID();

                obj.total_invoice = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<ReturnSellingProduct>(obj, transaction);

                // insert detil
                foreach (var item in obj.item_return.Where(f => f.Product != null))
                {
                    if (item.product_id.Length > 0)
                    {
                        item.return_sale_item_id = _context.GetGUID();
                        item.return_sale_id = obj.return_sale_id;
                        item.user_id = obj.user_id;

                        _context.db.Insert<ItemReturnSellingProduct>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Add data");

                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }        

        public int Update(ReturnSellingProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total_invoice = GetTotalInvoice(obj);

                // update header
                result = _context.db.Update<ReturnSellingProduct>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_return_deleted)
                {
                    result = _context.db.Delete<ItemReturnSellingProduct>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_return.Where(f => f.Product != null))
                {
                    item.return_sale_id = obj.return_sale_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        item.return_sale_item_id = _context.GetGUID();
                        _context.db.Insert<ItemReturnSellingProduct>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemReturnSellingProduct>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Update data");

                result = 1;

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(ReturnSellingProduct obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<ReturnSellingProduct>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["OldValue"] = obj.ToJson();
                    _log.Info("Delete data");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }                
    }
}     
