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
using System.Data;
 
namespace SparkPOS.Repository.Service
{        
    public class PaymentCreditProductRepository : IPaymentCreditProductRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_product_receivable_payment.pay_sale_id, t_product_receivable_payment.user_id, t_product_receivable_payment.date, t_product_receivable_payment.description, t_product_receivable_payment.system_date, t_product_receivable_payment.invoice, t_product_receivable_payment.is_cash,
                                              m_customer.customer_id, m_customer.name_customer
                                              FROM public.m_customer RIGHT JOIN public.t_product_receivable_payment ON t_product_receivable_payment.customer_id = m_customer.customer_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;
        private string _sql;
		
        public PaymentCreditProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<PaymentCreditProduct> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<PaymentCreditProduct> oList = _context.db.Query<PaymentCreditProduct, Customer, PaymentCreditProduct>(sql, (pp, c) =>
            {
                if (c != null)
                {
                    pp.customer_id = c.customer_id; pp.Customer = c;
                }

                return pp;
            }, param, splitOn: "customer_id");

            return oList;
        }

        private IList<ItemPaymentCreditProduct> GetItemPayment(string id)
        {
            IList<ItemPaymentCreditProduct> oList = new List<ItemPaymentCreditProduct>();

            try
            {
                var sql = @"SELECT t_credit_payment_item.pay_sale_item_id, t_credit_payment_item.pay_sale_id, t_credit_payment_item.amount, t_credit_payment_item.description, 
                            t_credit_payment_item.system_date, 1 as entity_state, (SELECT COUNT(*) FROM t_credit_payment_itemWHERE sale_id = t_product_sales.sale_id) AS jumlah_angsuran,
                            t_product_sales.sale_id, t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, t_product_sales.tax, t_product_sales.shipping_cost, t_product_sales.discount, t_product_sales.total_invoice, t_product_sales.total_payment
                            FROM public.t_credit_payment_itemINNER JOIN public.t_product_sales ON t_credit_payment_item.sale_id = t_product_sales.sale_id
                            WHERE t_credit_payment_item.pay_sale_id = @id
                            ORDER BY t_credit_payment_item.system_date";

                oList = _context.db.Query<ItemPaymentCreditProduct, SellingProduct, ItemPaymentCreditProduct>(sql, (ip, j) =>
                {
                    ip.sale_id = j.sale_id; ip.SellingProduct = j;

                    return ip;
                }, new { id }, splitOn: "sale_id").ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public PaymentCreditProduct GetByID(string id)
        {
            PaymentCreditProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_receivable_payment.pay_sale_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
                if (obj != null)
                    obj.item_payment_credit = GetItemPayment(obj.pay_sale_id).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new PaymentCreditProduct().GetTableName());
        }

        public ItemPaymentCreditProduct GetBySellingID(string id)
        {
            ItemPaymentCreditProduct obj = null;

            try
            {
                var sql = @"SELECT t_credit_payment_item.pay_sale_item_id, t_credit_payment_item.sale_id, t_credit_payment_item.amount, t_credit_payment_item.description, 1 as entity_state, 
                            t_product_receivable_payment.pay_sale_id, t_product_receivable_payment.customer_id, t_product_receivable_payment.user_id, t_product_receivable_payment.date, t_product_receivable_payment.description, t_product_receivable_payment.invoice, t_product_receivable_payment.is_cash
                            FROM public.t_credit_payment_item INNER JOIN public.t_product_receivable_payment ON t_credit_payment_item.pay_sale_id = t_product_receivable_payment.pay_sale_id
                            WHERE t_credit_payment_item.sale_id = @id";

                obj = _context.db.Query<ItemPaymentCreditProduct, PaymentCreditProduct, ItemPaymentCreditProduct>(sql, (ipp, pp) =>
                {
                    ipp.pay_sale_id = pp.pay_sale_id;
                    ipp.PaymentCreditProduct = pp;

                    return ipp;
                }, new { id }, splitOn: "pay_sale_id").SingleOrDefault();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<PaymentCreditProduct> GetByName(string name)
        {
            IList<PaymentCreditProduct> oList = new List<PaymentCreditProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_product_receivable_payment.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_receivable_payment.date DESC, t_product_receivable_payment.invoice");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();

                foreach (var item in oList)
                {
                    item.item_payment_credit = GetItemPayment(item.pay_sale_id).ToList();
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }        

        public IList<PaymentCreditProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<PaymentCreditProduct> oList = new List<PaymentCreditProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_receivable_payment.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_receivable_payment.date DESC, t_product_receivable_payment.invoice");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();

                foreach (var item in oList)
                {
                    item.item_payment_credit = GetItemPayment(item.pay_sale_id).ToList();
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PaymentCreditProduct> GetAll()
        {
            IList<PaymentCreditProduct> oList = new List<PaymentCreditProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_receivable_payment.date DESC, t_product_receivable_payment.invoice");

                oList = MappingRecordToObject(_sql).ToList();

                foreach (var item in oList)
                {
                    item.item_payment_credit = GetItemPayment(item.pay_sale_id).ToList();
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ItemPaymentCreditProduct> GetPaymentHistory(string jualId)
        {
            IList<ItemPaymentCreditProduct> oList = new List<ItemPaymentCreditProduct>();

            try
            {
                var sql = @"SELECT t_credit_payment_item.pay_sale_item_id, t_credit_payment_item.amount, t_credit_payment_item.description, 
                            t_product_receivable_payment.pay_sale_id, t_product_receivable_payment.date, t_product_receivable_payment.invoice, m_user.user_id, m_user.name_user
                            FROM public.t_credit_payment_itemINNER JOIN public.t_product_receivable_payment ON t_credit_payment_item.pay_sale_id = t_product_receivable_payment.pay_sale_id
                            INNER JOIN public.m_user ON t_product_receivable_payment.user_id = m_user.user_id
                            WHERE t_credit_payment_item.sale_id = @jualId
                            ORDER BY t_product_receivable_payment.date";

                oList = _context.db.Query<ItemPaymentCreditProduct, PaymentCreditProduct, User, ItemPaymentCreditProduct>(sql, (ip, pp, p) =>
                {
                    pp.User = p;
                    ip.PaymentCreditProduct = pp;

                    return ip;
                }, new { jualId }, splitOn: "pay_sale_id, user_id").ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(PaymentCreditProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Save(PaymentCreditProduct obj, bool isSaveFromSales)
        {
            var result = 0;

            try
            {
                IDbTransaction transaction = null;

                if (!isSaveFromSales)
                {
                    _context.BeginTransaction();

                    transaction = _context.transaction;
                }

                if (obj.pay_sale_id == null)
                    obj.pay_sale_id = _context.GetGUID();

                if (obj.invoice == null || obj.invoice.Length == 0)
                {
                    obj.invoice = this.GetLastInvoice();
                }

                // insert header
                _context.db.Insert<PaymentCreditProduct>(obj, transaction);

                // insert detil
                foreach (var item in obj.item_payment_credit.Where(f => f.SellingProduct != null))
                {
                    if (item.sale_id.Length > 0)
                    {
                        if (item.pay_sale_item_id == null)
                            item.pay_sale_item_id = _context.GetGUID();

                        item.pay_sale_id = obj.pay_sale_id;

                        _context.db.Insert<ItemPaymentCreditProduct>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                if (!isSaveFromSales)
                    _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Add data");

                result = 1;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }        

        public int Update(PaymentCreditProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Update(PaymentCreditProduct obj, bool isUpdateFromSales)
        {
            var result = 0;

            try
            {
                IDbTransaction transaction = null;

                if (!isUpdateFromSales)
                {
                    _context.BeginTransaction();

                    transaction = _context.transaction;
                }

                // update header
                result = _context.db.Update<PaymentCreditProduct>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_payment_credit_deleted)
                {
                    result = _context.db.Delete<ItemPaymentCreditProduct>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_payment_credit.Where(f => f.SellingProduct != null))
                {
                    item.pay_sale_id = obj.pay_sale_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.pay_sale_item_id == null)
                            item.pay_sale_item_id = _context.GetGUID();

                        _context.db.Insert<ItemPaymentCreditProduct>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemPaymentCreditProduct>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                if (!isUpdateFromSales)
                    _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Update data");

                result = 1;

            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result; 
        }        

        public int Delete(PaymentCreditProduct obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<PaymentCreditProduct>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["OldValue"] = obj.ToJson();
                    _log.Info("Delete data");
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }        
    }
}     
