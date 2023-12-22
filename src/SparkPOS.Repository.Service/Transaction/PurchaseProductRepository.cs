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
    public class PurchaseProductRepository : IPurchaseProductRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_purchase_product.purchase_id, t_purchase_product.user_id, t_purchase_product.purchase_return_id, t_purchase_product.invoice, t_purchase_product.date, 
                                              t_purchase_product.due_date, t_purchase_product.tax, t_purchase_product.discount, t_purchase_product.total_invoice, t_purchase_product.total_payment, 
                                              t_purchase_product.total_payment AS total_repayment_old, t_purchase_product.description, t_purchase_product.system_date, 
                                              m_supplier.supplier_id, m_supplier.name_supplier, m_supplier.address
                                              FROM public.t_purchase_product INNER JOIN public.m_supplier ON t_purchase_product.supplier_id = m_supplier.supplier_id
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private const string SQL_TEMPLATE_FOR_PAGING = @"SELECT COUNT(*)
                                                         FROM public.t_purchase_product INNER JOIN public.m_supplier ON t_purchase_product.supplier_id = m_supplier.supplier_id
                                                         {WHERE}";

        private const string SQL_TEMPLATE_SELECT_Tax = @"SELECT name_tax,tax_percentage , tax_id, is_default_tax FROM m_tax ORDER BY is_default_tax desc;";

        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public PurchaseProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<PurchaseProduct> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<PurchaseProduct> oList = _context.db.Query<PurchaseProduct, Supplier, PurchaseProduct>(sql, (bl, sup) =>
            {
                bl.supplier_id = sup.supplier_id; bl.Supplier = sup;
                return bl;
            }, param, splitOn: "supplier_id");

            return oList;
        }

        public IList<ItemPurchaseProduct> GetItemPurchase(string beliId)
                    {
            IList<ItemPurchaseProduct> oList = new List<ItemPurchaseProduct>();

            try
            {
                var sql = @"SELECT t_purchase_order_item.purchase_item_id, t_purchase_order_item.purchase_id, t_purchase_order_item.user_id, t_purchase_order_item.price, 
                            t_purchase_order_item.quantity, t_purchase_order_item.return_quantity, t_purchase_order_item.discount, t_purchase_order_item.system_date, 1 as entity_state,
                            m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.purchase_price, m_product.selling_price, m_product.discount
                            FROM public.t_purchase_order_item INNER JOIN public.m_product ON t_purchase_order_item.product_id = m_product.product_id
                            WHERE t_purchase_order_item.purchase_id = @beliId
                            ORDER BY t_purchase_order_item.system_date";

                oList = _context.db.Query<ItemPurchaseProduct, Product, ItemPurchaseProduct>(sql, (ib, p) =>
                {
                    ib.product_id = p.product_id; ib.Product = p;
                    return ib;
                }, new { beliId }, splitOn: "product_id").ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

                return oList;
        }

        public PurchaseProduct GetByID(string id)
        {
            PurchaseProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_purchase_product.purchase_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<PurchaseProduct> GetByName(string name)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_supplier.name_supplier) LIKE @name OR LOWER(t_purchase_product.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByName(string name, int pageNumber, int pageSize, ref int pagesCount)
        {            
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                name = "%" + name.ToLower() + "%";

                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_supplier.name_supplier) LIKE @name OR LOWER(t_purchase_product.description) LIKE @name");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { name });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_supplier.name_supplier) LIKE @name OR LOWER(t_purchase_product.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { name, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetAll()
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetAll(string name)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_supplier.name_supplier) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public List<Tax> GetTaxNames()
        {

        List<Tax> taxNames = new List<Tax>();

            string query = SQL_TEMPLATE_SELECT_Tax;
            taxNames = _context.db.Query<Tax>(query).ToList();

            return taxNames;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new PurchaseProduct().GetTableName());
        }

        public IList<PurchaseProduct> GetInvoiceSupplier(string id, string invoice)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                object param = null;

                if (invoice.Length > 0)
                {
                    invoice = invoice.ToLower() + "%";
                    param = new { id, invoice };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_supplier.supplier_id = @id AND LOWER(t_purchase_product.invoice) LIKE @invoice");
                }
                else
                {
                    param = new { id };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_supplier.supplier_id = @id");                    
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, param).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceKreditBySupplier(string id, bool isLunas)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                if (isLunas)
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_supplier.supplier_id = @id AND t_purchase_product.due_date IS NOT NULL AND (t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) <= t_purchase_product.total_payment");                    
                }
                else
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_supplier.supplier_id = @id AND t_purchase_product.due_date IS NOT NULL AND (t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) > t_purchase_product.total_payment");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");
                
                oList = MappingRecordToObject(_sql, new { id }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceKreditByInvoice(string id, string invoice)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_supplier.supplier_id = @id AND LOWER(t_purchase_product.invoice) LIKE @invoice AND t_purchase_product.due_date IS NOT NULL AND (t_purchase_product.total_invoice - t_purchase_product.discount + t_purchase_product.tax) > t_purchase_product.total_payment");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                invoice = invoice.ToLower() + "%";
                oList = MappingRecordToObject(_sql, new { id, invoice }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { tanggalMulai, tanggalSelesai });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                name = "%" + name.ToLower() + "%";

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_purchase_product.date BETWEEN @tanggalMulai AND @tanggalSelesai AND LOWER(m_supplier.name_supplier) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_purchase_product.date DESC, t_purchase_product.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, name }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private double GetTotalInvoice(PurchaseProduct obj)
        {
            var total = obj.item_beli.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                     .Sum(f => (f.quantity - f.return_quantity) * (f.price - (f.discount / 100 * f.price)));

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        public int Save(PurchaseProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                if (obj.purchase_id == null)
                    obj.purchase_id = _context.GetGUID();

                obj.total_invoice = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<PurchaseProduct>(obj, transaction);
 
                // insert detail
                foreach (var item in obj.item_beli.Where(f => f.Product != null))
                {
                    if (item.product_id.Length > 0)
                    {
                        if (item.purchase_item_id == null)
                            item.purchase_item_id = _context.GetGUID();
                        
                        item.purchase_id = obj.purchase_id;
                        item.user_id = obj.user_id;

                        _context.db.Insert<ItemPurchaseProduct>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }
                
                // jika Purchase cash, langsung insert ke Dept Payment
                if (obj.due_date.IsNull())
                {
                    result = SaveDebtPayment(obj);
                    if (result > 0)
                        obj.total_payment = obj.grand_total;

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

        public int Update(PurchaseProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total_invoice = GetTotalInvoice(obj);

                // update header
                result = _context.db.Update<PurchaseProduct>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_beli_deleted)
                {
                    result = _context.db.Delete<ItemPurchaseProduct>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_beli.Where(f => f.Product != null))
                {
                    item.purchase_id = obj.purchase_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.purchase_item_id == null)
                            item.purchase_item_id = _context.GetGUID();                        

                        _context.db.Insert<ItemPurchaseProduct>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemPurchaseProduct>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                // jika terjadi perubahan status invoice dari cash ke Credit
                if (obj.tanggal_creditTerm_old.IsNull() && !obj.due_date.IsNull())
                {
                    result = HapusDebtPayment(obj);
                    if (result > 0)
                        obj.total_payment = 0;
                }
                else if (obj.due_date.IsNull()) // jika Purchase cash, langsung update ke Dept Payment
                {
                    result = SaveDebtPayment(obj);
                    if (result > 0)
                        obj.total_payment = obj.grand_total;
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

        /// <summary>
        /// Method khusus untuk menyimpan Dept Payment Purchase cash
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int SaveDebtPayment(PurchaseProduct obj)
        {
            DebtPaymentProduct DebtPayment;
            ItemDebtPaymentProduct itemDebtPayment;
            IDebtPaymentProductRepository DebtPaymentRepo = new DebtPaymentProductRepository(_context, _log);

            var result = 0;

            // set detail
            itemDebtPayment = DebtPaymentRepo.GetByPurchaseID(obj.purchase_id);
            if (itemDebtPayment != null) // already there pelunasan
            {
                itemDebtPayment.amount = obj.grand_total;
                itemDebtPayment.PurchaseProduct = new PurchaseProduct { purchase_id = itemDebtPayment.purchase_id };
                itemDebtPayment.entity_state = EntityState.Modified;

                // set header by detail
                DebtPayment = itemDebtPayment.DebtPaymentProduct;
                DebtPayment.is_cash = obj.is_cash;

                // set item payment
                DebtPayment.item_payment_debt.Add(itemDebtPayment);

                result = DebtPaymentRepo.Update(DebtPayment, true);
            }
            else // Not yet there pelunasan debt
            {
                DebtPayment = new DebtPaymentProduct();

                // set header
                DebtPayment.supplier_id = obj.supplier_id;
                DebtPayment.user_id = obj.user_id;
                DebtPayment.date = obj.date;
                DebtPayment.description = "Purchase cash product";
                DebtPayment.is_cash = obj.is_cash;

                // set item
                itemDebtPayment = new ItemDebtPaymentProduct();
                itemDebtPayment.purchase_id = obj.purchase_id;
                itemDebtPayment.PurchaseProduct = obj;
                itemDebtPayment.amount = obj.grand_total; // GetTotalInvoiceSetelahDiskonDanPPN(obj);
                itemDebtPayment.description = string.Empty;

                // set item payment
                DebtPayment.item_payment_debt.Add(itemDebtPayment);

                // save item payment
                result = DebtPaymentRepo.Save(DebtPayment, true);
            }

            return result;
        }        

        /// <summary>
        /// Method untuk menghapus Dept Payment jika terjadi perubahan status invoice dari cash ke Credit
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int HapusDebtPayment(PurchaseProduct obj)
        {
            DebtPaymentProduct DebtPayment;
            ItemDebtPaymentProduct itemDebtPayment;
            IDebtPaymentProductRepository DebtPaymentRepo = new DebtPaymentProductRepository(_context, _log);

            var result = 0;

            // set detail
            itemDebtPayment = DebtPaymentRepo.GetByPurchaseID(obj.purchase_id);
            if (itemDebtPayment != null)
            {
                DebtPayment = itemDebtPayment.DebtPaymentProduct;
                result = DebtPaymentRepo.Delete(DebtPayment);
            }

            return result;
        }

        public int Delete(PurchaseProduct obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<PurchaseProduct>(obj) ? 1 : 0;

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
