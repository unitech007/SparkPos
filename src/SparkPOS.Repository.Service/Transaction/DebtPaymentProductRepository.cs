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
    public class DebtPaymentProductRepository : IDebtPaymentProductRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_product_payable_payment.pay_purchase_id, t_product_payable_payment.user_id, t_product_payable_payment.date, 
                                              t_product_payable_payment.description, t_product_payable_payment.system_date, t_product_payable_payment.invoice, t_product_payable_payment.is_cash,
                                              m_supplier.supplier_id, m_supplier.name_supplier
                                              FROM public.t_product_payable_payment INNER JOIN public.m_supplier ON t_product_payable_payment.supplier_id = m_supplier.supplier_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public DebtPaymentProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<DebtPaymentProduct> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<DebtPaymentProduct> oList = _context.db.Query<DebtPaymentProduct, Supplier, DebtPaymentProduct>(sql, (p, s) =>
            {
                p.supplier_id = s.supplier_id; p.Supplier = s;
                return p;
            }, param, splitOn: "supplier_id");

            return oList;
        }

        private IList<ItemDebtPaymentProduct> GetItemPayment(string id)
        {
            IList<ItemDebtPaymentProduct> oList = new List<ItemDebtPaymentProduct>();

            try
            {
                var sql = @"SELECT t_debt_payment_item.pay_purchase_item_id, t_debt_payment_item.pay_purchase_id, t_debt_payment_item.amount, 
                            t_debt_payment_item.description, t_debt_payment_item.system_date, 1 as entity_state, (SELECT COUNT(*) FROM t_debt_payment_item WHERE purchase_id = t_purchase_product.purchase_id) AS jumlah_angsuran,
                            t_purchase_product.purchase_id, t_purchase_product.invoice, t_purchase_product.date, t_purchase_product.due_date, t_purchase_product.tax, t_purchase_product.discount, t_purchase_product.total_invoice, t_purchase_product.total_payment
                            FROM public.t_debt_payment_item INNER JOIN public.t_purchase_product ON t_debt_payment_item.purchase_id = t_purchase_product.purchase_id
                            WHERE t_debt_payment_item.pay_purchase_id = @id
                            ORDER BY t_debt_payment_item.system_date";

                oList = _context.db.Query<ItemDebtPaymentProduct, PurchaseProduct, ItemDebtPaymentProduct>(sql, (ip, b) =>
                {
                    ip.purchase_id = b.purchase_id; ip.PurchaseProduct = b;

                    return ip;
                }, new { id }, splitOn: "purchase_id").ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public DebtPaymentProduct GetByID(string id)
        {
            DebtPaymentProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_payable_payment.pay_purchase_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
                if (obj != null)
                    obj.item_payment_debt = GetItemPayment(obj.pay_purchase_id).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<DebtPaymentProduct> GetByName(string name)
        {
            IList<DebtPaymentProduct> oList = new List<DebtPaymentProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_supplier.name_supplier) LIKE @name OR LOWER(t_product_payable_payment.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_payable_payment.date DESC, t_product_payable_payment.invoice");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();

                foreach (var item in oList)
                {
                    item.item_payment_debt = GetItemPayment(item.pay_purchase_id).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<DebtPaymentProduct> GetAll()
        {
            IList<DebtPaymentProduct> oList = new List<DebtPaymentProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_payable_payment.date DESC, t_product_payable_payment.invoice");

                oList = MappingRecordToObject(_sql).ToList();

                foreach (var item in oList)
                {
                    item.item_payment_debt = GetItemPayment(item.pay_purchase_id).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ItemDebtPaymentProduct> GetPaymentHistory(string beliId)
        {
            IList<ItemDebtPaymentProduct> oList = new List<ItemDebtPaymentProduct>();

            try
            {
                var sql = @"SELECT t_debt_payment_item.pay_purchase_item_id, t_debt_payment_item.amount, t_debt_payment_item.description, 
                            t_product_payable_payment.pay_purchase_id, t_product_payable_payment.date, t_product_payable_payment.invoice, m_user.user_id, m_user.name_user
                            FROM public.t_debt_payment_item INNER JOIN public.t_product_payable_payment ON t_debt_payment_item.pay_purchase_id = t_product_payable_payment.pay_purchase_id
                            INNER JOIN public.m_user ON t_product_payable_payment.user_id = m_user.user_id
                            WHERE t_debt_payment_item.purchase_id = @beliId
                            ORDER BY t_product_payable_payment.date";

                oList = _context.db.Query<ItemDebtPaymentProduct, DebtPaymentProduct, User, ItemDebtPaymentProduct>(sql, (ip, ph, p) =>
                {
                    ph.User = p;
                    ip.DebtPaymentProduct = ph;

                    return ip;
                }, new { beliId }, splitOn: "pay_purchase_id, user_id").ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<DebtPaymentProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<DebtPaymentProduct> oList = new List<DebtPaymentProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_payable_payment.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_payable_payment.date DESC, t_product_payable_payment.invoice");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();

                foreach (var item in oList)
                {
                    item.item_payment_debt = GetItemPayment(item.pay_purchase_id).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(DebtPaymentProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Save(DebtPaymentProduct obj, bool isSaveFromPurchase)
        {
            var result = 0;

            try
            {
                IDbTransaction transaction = null;

                if (!isSaveFromPurchase)
                {
                    _context.BeginTransaction();

                    transaction = _context.transaction;
                }

                if (obj.pay_purchase_id == null)
                    obj.pay_purchase_id = _context.GetGUID();

                if (obj.invoice == null || obj.invoice.Length == 0)
                {
                    obj.invoice = this.GetLastInvoice();
                }

                // insert header
                _context.db.Insert<DebtPaymentProduct>(obj, transaction);

                // insert detil
                foreach (var item in obj.item_payment_debt.Where(f => f.PurchaseProduct != null))
                {
                    if (item.purchase_id.Length > 0)
                    {
                        if (item.pay_purchase_item_id == null)
                            item.pay_purchase_item_id = _context.GetGUID();

                        item.pay_purchase_id = obj.pay_purchase_id;

                        _context.db.Insert<ItemDebtPaymentProduct>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                if (!isSaveFromPurchase)
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

        public int Update(DebtPaymentProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Update(DebtPaymentProduct obj, bool isUpdateFromPurchase)
        {
            var result = 0;

            try
            {
                IDbTransaction transaction = null;

                if (!isUpdateFromPurchase)
                {
                    _context.BeginTransaction();

                    transaction = _context.transaction;
                }

                // update header
                result = _context.db.Update<DebtPaymentProduct>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_payment_debt_deleted)
                {
                    result = _context.db.Delete<ItemDebtPaymentProduct>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_payment_debt.Where(f => f.PurchaseProduct != null))
                {
                    item.pay_purchase_id = obj.pay_purchase_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.pay_purchase_item_id == null)
                            item.pay_purchase_item_id = _context.GetGUID();

                        _context.db.Insert<ItemDebtPaymentProduct>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemDebtPaymentProduct>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                if (!isUpdateFromPurchase)
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

        public int Delete(DebtPaymentProduct obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<DebtPaymentProduct>(obj) ? 1 : 0;

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

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new DebtPaymentProduct().GetTableName());
        }

        public ItemDebtPaymentProduct GetByPurchaseID(string id)
        {
            ItemDebtPaymentProduct obj = null;

            try
            {
                var sql = @"SELECT t_debt_payment_item.pay_purchase_item_id, t_debt_payment_item.purchase_id, t_debt_payment_item.amount, t_debt_payment_item.description, 1 as entity_state, 
                            t_product_payable_payment.pay_purchase_id, t_product_payable_payment.supplier_id, t_product_payable_payment.user_id, t_product_payable_payment.date, t_product_payable_payment.description, t_product_payable_payment.invoice, t_product_payable_payment.is_cash
                            FROM public.t_debt_payment_item INNER JOIN public.t_product_payable_payment ON t_debt_payment_item.pay_purchase_id = t_product_payable_payment.pay_purchase_id
                            WHERE t_debt_payment_item.purchase_id = @id";

                obj = _context.db.Query<ItemDebtPaymentProduct, DebtPaymentProduct, ItemDebtPaymentProduct>(sql, (iph, ph) =>
                {
                    iph.pay_purchase_id = ph.pay_purchase_id;
                    iph.DebtPaymentProduct = ph;

                    return iph;
                }, new { id }, splitOn: "pay_purchase_id").SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }        
    }
}     
