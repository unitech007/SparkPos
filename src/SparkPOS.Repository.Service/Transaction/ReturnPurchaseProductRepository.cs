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
    public class ReturnPurchaseProductRepository : IReturnPurchaseProductRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_purchase_return.purchase_return_id, t_purchase_return.user_id, t_purchase_return.invoice, t_purchase_return.date, t_purchase_return.description, t_purchase_return.system_date, t_purchase_return.total_invoice, 
                                              m_supplier.supplier_id, m_supplier.name_supplier, m_supplier.address, t_purchase_product.purchase_id, t_purchase_product.invoice
                                              FROM public.m_supplier INNER JOIN public.t_purchase_return ON t_purchase_return.supplier_id = m_supplier.supplier_id
                                              INNER JOIN public.t_purchase_product ON t_purchase_return.purchase_id = t_purchase_product.purchase_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;
        private string _sql;
		
        public ReturnPurchaseProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<ReturnPurchaseProduct> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<ReturnPurchaseProduct> oList = _context.db.Query<ReturnPurchaseProduct, Supplier, PurchaseProduct, ReturnPurchaseProduct>(sql, (rb, s, b) =>
            {
                rb.supplier_id = s.supplier_id; rb.Supplier = s;
                rb.purchase_id = b.purchase_id; rb.PurchaseProduct = b;

                return rb;
            }, param, splitOn: "supplier_id, purchase_id");

            return oList;
        }

        private IList<ItemReturnPurchaseProduct> GetItemReturn(string returnId)
        {
            IList<ItemReturnPurchaseProduct> oList = new List<ItemReturnPurchaseProduct>();

            try
            {
                var sql = @"SELECT t_purchase_return_item.return_purchase_item_id, t_purchase_return_item.purchase_return_id, t_purchase_return_item.user_id, t_purchase_return_item.purchase_item_id, t_purchase_return_item.price, t_purchase_return_item.quantity, t_purchase_return_item.return_quantity, t_purchase_return_item.system_date, 1 as entity_state,
                            m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.selling_price
                            FROM public.t_purchase_return_item INNER JOIN public.m_product ON t_purchase_return_item.product_id = m_product.product_id  
                            WHERE t_purchase_return_item.purchase_return_id = @returnId
                            ORDER BY t_purchase_return_item.system_date";

                oList = _context.db.Query<ItemReturnPurchaseProduct, Product, ItemReturnPurchaseProduct>(sql, (ir, p) =>
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

        public ReturnPurchaseProduct GetByID(string id)
        {
            ReturnPurchaseProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_purchase_return.purchase_return_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();

                if (obj != null)
                    // load item return
                    obj.item_return = GetItemReturn(obj.purchase_return_id);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new ReturnPurchaseProduct().GetTableName());
        }

        public IList<ReturnPurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReturnPurchaseProduct> oList = new List<ReturnPurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_purchase_return.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_return.date DESC, t_purchase_return.invoice");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();

                // load item beli
                foreach (var item in oList)
                {
                    item.item_return = GetItemReturn(item.purchase_return_id);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReturnPurchaseProduct> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ReturnPurchaseProduct> GetAll()
        {
            IList<ReturnPurchaseProduct> oList = new List<ReturnPurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_return.date DESC, t_purchase_return.invoice");

                oList = MappingRecordToObject(_sql).ToList();

                // load item beli
                foreach (var item in oList)
                {
                    item.item_return = GetItemReturn(item.purchase_return_id);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private double GetTotalInvoice(ReturnPurchaseProduct obj)
        {
            var total = obj.item_return.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                      .Sum(f => f.return_quantity * f.price);
            
            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        public int Save(ReturnPurchaseProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.purchase_return_id = _context.GetGUID();

                obj.total_invoice = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<ReturnPurchaseProduct>(obj, transaction);

                // insert detil
                foreach (var item in obj.item_return.Where(f => f.Product != null))
                {
                    if (item.product_id.Length > 0)
                    {
                        item.return_purchase_item_id = _context.GetGUID();
                        item.purchase_return_id = obj.purchase_return_id;
                        item.user_id = obj.user_id;

                        _context.db.Insert<ItemReturnPurchaseProduct>(item, transaction);

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

        public int Update(ReturnPurchaseProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total_invoice = GetTotalInvoice(obj);

                // update header
                result = _context.db.Update<ReturnPurchaseProduct>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_return_deleted)
                {
                    result = _context.db.Delete<ItemReturnPurchaseProduct>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_return.Where(f => f.Product != null))
                {
                    item.purchase_return_id = obj.purchase_return_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        item.return_purchase_item_id = _context.GetGUID();
                        _context.db.Insert<ItemReturnPurchaseProduct>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemReturnPurchaseProduct>(item, transaction) ? 1 : 0;
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

        public int Delete(ReturnPurchaseProduct obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<ReturnPurchaseProduct>(obj) ? 1 : 0;

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
