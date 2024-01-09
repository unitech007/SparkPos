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
using log4net;
using Dapper;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
using System.Data;
using SparkPOS.Model.Transaction;

namespace SparkPOS.Repository.Service
{
    public class SellingProductRepository : ISellingProductRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_product_sales.sale_id, t_product_sales.return_sale_id, t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, 
                                              t_product_sales.tax, t_product_sales.courier, t_product_sales.shipping_cost, t_product_sales.discount, t_product_sales.total_invoice, t_product_sales.total_payment, t_product_sales.total_payment AS total_repayment_old, t_product_sales.description, t_product_sales.system_date, 
                                              t_product_sales.is_sdac, t_product_sales.is_dropship, t_product_sales.shipping_to, t_product_sales.shipping_address, t_product_sales.shipping_subdistrict, t_product_sales.shipping_country, t_product_sales.shipping_regency, t_product_sales.shipping_village, t_product_sales.shipping_city, t_product_sales.shipping_postal_code, t_product_sales.shipping_phone,
                                              t_product_sales.from_label1, t_product_sales.from_label2, t_product_sales.from_label3, t_product_sales.from_label4,
                                              t_product_sales.to_label1, t_product_sales.to_label2, t_product_sales.to_label3, t_product_sales.to_label4,
                                              m_customer.customer_id, m_customer.name_customer, m_customer.province_id, m_customer.regency_id, m_customer.subdistrict_id, m_customer.address, m_customer.postal_code, m_customer.phone, m_customer.discount, m_customer.credit_limit,
                                              m_user.user_id, m_user.name_user,
                                              t_machine.machine_id, t_machine.starting_balance,
                                              m_dropshipper.dropshipper_id, m_dropshipper.name_dropshipper, m_dropshipper.address, m_dropshipper.phone
                                              FROM public.t_product_sales LEFT JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                              LEFT JOIN m_user ON m_user.user_id = t_product_sales.user_id
                                              LEFT JOIN t_machine ON t_machine.machine_id= t_product_sales.machine_id
                                              LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_product_sales.dropshipper_id
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT DISTINCT t_product_sales.sale_id, t_product_sales.return_sale_id, t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, 
                                                     t_product_sales.tax, t_product_sales.courier, t_product_sales.shipping_cost, t_product_sales.discount, t_product_sales.total_invoice, t_product_sales.total_payment, t_product_sales.total_payment AS total_repayment_old, t_product_sales.description, t_product_sales.system_date, 
                                                     t_product_sales.is_sdac, t_product_sales.is_dropship, t_product_sales.shipping_to, t_product_sales.shipping_address, t_product_sales.shipping_subdistrict, t_product_sales.shipping_country, t_product_sales.shipping_regency, t_product_sales.shipping_village, t_product_sales.shipping_city, t_product_sales.shipping_postal_code, t_product_sales.shipping_phone,
                                                     t_product_sales.from_label1, t_product_sales.from_label2, t_product_sales.from_label3, t_product_sales.from_label4,
                                                     t_product_sales.to_label1, t_product_sales.to_label2, t_product_sales.to_label3, t_product_sales.to_label4,
                                                     m_customer.customer_id, m_customer.name_customer, m_customer.province_id, m_customer.regency_id, m_customer.subdistrict_id, m_customer.address, m_customer.postal_code, m_customer.phone, m_customer.discount, m_customer.credit_limit,
                                                     m_user.user_id, m_user.name_user,
                                                     t_machine.machine_id, t_machine.starting_balance,
                                                     m_dropshipper.dropshipper_id, m_dropshipper.name_dropshipper, m_dropshipper.address, m_dropshipper.phone
                                                     FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_product_sales.sale_id = t_sales_order_item.sale_id
                                                     LEFT JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                                     LEFT JOIN m_user ON m_user.user_id = t_product_sales.user_id
                                                     LEFT JOIN t_machine ON t_machine.machine_id= t_product_sales.machine_id
                                                     LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_product_sales.dropshipper_id
                                                     {WHERE}
                                                     {ORDER BY}
                                                     {OFFSET}";

        private const string SQL_TEMPLATE_FOR_PAGING = @"SELECT COUNT(*)
                                                         FROM public.t_product_sales LEFT JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                                         LEFT JOIN m_user ON m_user.user_id = t_product_sales.user_id
                                                         LEFT JOIN t_machine ON t_machine.machine_id= t_product_sales.machine_id
                                                         LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_product_sales.dropshipper_id
                                                         {WHERE}";

        private const string SQL_TEMPLATE_DETAIL_FOR_PAGING = @"SELECT COUNT(DISTINCT t_product_sales.sale_id)
                                                                FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_product_sales.sale_id = t_sales_order_item.sale_id
                                                                LEFT JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                                                LEFT JOIN m_user ON m_user.user_id = t_product_sales.user_id
                                                                LEFT JOIN t_machine ON t_machine.machine_id= t_product_sales.machine_id
                                                                LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_product_sales.dropshipper_id
                                                                {WHERE}";

        private const string SQL_TEMPLATE_QUOTATION_FOR_PAGING = @" SELECT  t_sales_quotation_item.product_id AS product_id, m_product.product_name,m_product.product_code,
                                                                    t_sales_quotation_item.discount, t_sales_quotation_item.quantity,  t_sales_quotation_item.selling_price,t_sales_quotation_item.quotation_id 
                                                                    FROM    public.t_sales_quotation  INNER JOIN public.t_sales_quotation_item ON t_sales_quotation_item.quotation_id = t_sales_quotation.quotation_id
                                                                    INNER JOIN public.m_product ON m_product.product_id = t_sales_quotation_item.product_id
                                                                    WHERE  t_sales_quotation.quotation = @quotationNumber";

        private const string SQL_TEMPLATE_SELECT_Tax = @"SELECT name_tax,tax_percentage , tax_id, is_default_tax FROM m_tax ORDER BY is_default_tax desc;";

        private const string SQL_TEMPLATE_QUOTATION_CUSTOMER_FOR_PAGING = @"SELECT quotation FROM t_sales_quotation WHERE customer_id = @CustomerId";
        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public SellingProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<SellingProduct> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<SellingProduct> oList = _context.db.Query<SellingProduct, Customer, User, CashierMachine, Dropshipper, SellingProduct>(sql, (j, c, p, m, d) =>
            {
                if (c != null)
                {
                    j.customer_id = c.customer_id; j.Customer = c;
                }

                if (p != null)
                {
                    j.user_id = p.user_id; j.User = p;
                }

                if (m != null)
                {
                    j.machine_id = m.machine_id; j.Machine = m;
                }

                if (d != null)
                {
                    j.dropshipper_id = d.dropshipper_id; j.Dropshipper = d;
                }

                return j;
            }, param, splitOn: "customer_id, user_id, machine_id, dropshipper_id");

            return oList;
        }

        private IList<PriceWholesale> GetListPriceWholesale(string produkId)
        {
            IPriceWholesaleRepository repo = new PriceWholesaleRepository(_context, _log);

            return repo.GetListPriceWholesale(produkId);
        }

        public IList<ItemSellingProduct> GetItemSelling(string jualId)
        {
            IList<ItemSellingProduct> oList = new List<ItemSellingProduct>();

            try
            {
                var sql = @"SELECT t_sales_order_item.sale_item_id, t_sales_order_item.sale_id, t_sales_order_item.user_id, t_sales_order_item.purchase_price, t_sales_order_item.selling_price, 
                            t_sales_order_item.quantity, t_sales_order_item.quantity AS old_jumlah, t_sales_order_item.return_quantity, t_sales_order_item.discount, COALESCE(t_sales_order_item.description, t_sales_order_item.description, '') AS description, t_sales_order_item.system_date, 1 as entity_state,
                            m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.purchase_price, m_product.selling_price, m_product.discount, m_product.stock, m_product.warehouse_stock,
                            m_category.category_id, m_category.name_category, m_category.discount
                            FROM public.t_sales_order_item INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                            INNER JOIN public.m_category ON m_category.category_id = m_product.category_id
                            WHERE t_sales_order_item.sale_id = @jualId
                            ORDER BY t_sales_order_item.system_date";

                oList = _context.db.Query<ItemSellingProduct, Product, Category, ItemSellingProduct>(sql, (ij, p, g) =>
                {
                    p.category_id = g.category_id; p.Category = g;
                    ij.product_id = p.product_id; ij.Product = p;

                    return ij;
                }, new { jualId }, splitOn: "product_id, category_id").ToList();

                foreach (var item in oList)
                {
                    item.Product.list_of_harga_grosir = GetListPriceWholesale(item.product_id).ToList();
                }
            }
            catch(Exception ex)
            {
                DapperContext.LogException(ex);
            }

            return oList;
        }


        public SellingProduct GetByID(string id)
        {
            SellingProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_sales.sale_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }


        public List<string> GetQuotationsByCustomerId(string customerId)
        {
            List<string> quotations = new List<string>();
            string query = SQL_TEMPLATE_QUOTATION_CUSTOMER_FOR_PAGING;
            quotations = _context.db.Query<string>(query, new { customerId }).ToList();

            return quotations;
        }


        public List<Tax> GetTaxNames()
        {
            List<Tax> taxNames = new List<Tax>();

            string query = SQL_TEMPLATE_SELECT_Tax;
            taxNames = _context.db.Query<Tax>(query).ToList();
           
            //Tax defaultTax = taxNames.FirstOrDefault(t => t.is_default_tax == 1);

            //// Move the default tax to the first position in the list
            //if (defaultTax != null)
            //{
            //    taxNames.Remove(defaultTax);
            //    taxNames.Insert(0, defaultTax);
            //}

            return taxNames;
        }



        public List<ItemSellingQuotation> GetProductDetailsByQUotation(string quotationNumber)
                {
            try
            {
                var query = SQL_TEMPLATE_QUOTATION_FOR_PAGING;

                var result = _context.db.Query(query, new { quotationNumber });

                var productDetails = new List<ItemSellingQuotation>();

                foreach (var row in result)
                {
                    var itemSellingQuotation = new ItemSellingQuotation
                    {
                        product_id = row.product_id,
                        discount = (double)row.discount,
                        quantity = (double)row.quantity,
                        selling_price = (double)row.selling_price,
                        quotation_id = row.quotation_id
                    };


                    var product = new Product
                    {
                        product_id = row.product_id, // Assuming the product_id is returned as item_id in the query
                        product_name = row.product_name,
                        product_code = row.product_code
                    };

                    itemSellingQuotation.Product = product;
                    productDetails.Add(itemSellingQuotation);
                }

                return productDetails;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                DapperContext.LogException(ex);
                return null;
            }
        }

        public double GetTaxRate(string taxName)
        {
            double taxRate = 0;

            try
            {
                string query = "SELECT tax_percentage FROM m_tax WHERE name_tax = @TaxName";
                taxRate = _context.db.ExecuteScalar<double>(query, new { TaxName = taxName });
               //taxRate /= 100;
                taxRate = taxRate / 100;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                DapperContext.LogException(ex);
            }

            return taxRate;
        }


        public SellingProduct GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            SellingProduct obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_sales.date = CURRENT_DATE AND t_product_sales.user_id = @userId AND t_product_sales.machine_id= @mesinId");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.system_date DESC LIMIT 1");
                _sql = _sql.Replace("{OFFSET}", "");

                obj = MappingRecordToObject(_sql, new { userId, mesinId }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<SellingProduct> GetByName(string name)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_product_sales.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                name = "%" + name.ToLower() + "%";

                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_product_sales.description) LIKE @name");

                if (isCekKeteranganItemSelling)
                    sqlPageCount = SQL_TEMPLATE_DETAIL_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_product_sales.description) LIKE @name OR LOWER(t_sales_order_item.description) LIKE @name");

                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { name });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_product_sales.description) LIKE @name");

                if (isCekKeteranganItemSelling)
                    _sql = SQL_TEMPLATE_DETAIL.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_product_sales.description) LIKE @name OR LOWER(t_sales_order_item.description) LIKE @name");

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { name, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetAll()
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        private double GetTotalInvoice(SellingProduct obj)
        {
            var total = obj.item_jual.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                     .Sum(f => (f.quantity - f.return_quantity) * (f.selling_price - (f.discount / 100 * f.selling_price)));

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        public int Save(SellingProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                if (obj.sale_id == null)
                    obj.sale_id = _context.GetGUID();

                obj.total_invoice = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<SellingProduct>(obj, transaction);

                // insert detail
                foreach (var item in obj.item_jual.Where(f => f.Product != null))
                {
                    if (item.product_id.Length > 0)
                    {
                        if (item.sale_item_id == null)
                            item.sale_item_id = _context.GetGUID();

                        item.sale_id = obj.sale_id;
                        item.user_id = obj.user_id;

                        _context.db.Insert<ItemSellingProduct>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                // jika Purchase cash, langsung insert ke Dept Payment
                if (obj.due_date.IsNull())
                {
                    result = SavePaymentCredit(obj);
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
                 DapperContext.LogException(ex);
            }

            return result;
        }

        /// <summary>
        /// Method khusus untuk menyimpan payment sales cash
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int SavePaymentCredit(SellingProduct obj)
        {
            PaymentCreditProduct paymentCredit;
            ItemPaymentCreditProduct itemPaymentCredit;
            IPaymentCreditProductRepository paymentCreditRepo = new PaymentCreditProductRepository(_context, _log);

            var result = 0;

            // set detail            
            itemPaymentCredit = paymentCreditRepo.GetBySellingID(obj.sale_id);
            if (itemPaymentCredit != null) // already there pelunasan
            {
                itemPaymentCredit.amount = obj.grand_total; // GetTotalInvoiceSetelahDiskonDanPPN(obj);
                itemPaymentCredit.SellingProduct = new SellingProduct { sale_id = itemPaymentCredit.sale_id };
                itemPaymentCredit.entity_state = EntityState.Modified;

                // set header by detail
                paymentCredit = itemPaymentCredit.PaymentCreditProduct;
                paymentCredit.is_cash = obj.is_cash;

                // set item payment
                paymentCredit.item_payment_credit.Add(itemPaymentCredit);

                result = paymentCreditRepo.Update(paymentCredit, true);
            }
            else // Not yet there pelunasan debt
            {
                paymentCredit = new PaymentCreditProduct();

                // set header
                paymentCredit.customer_id = obj.customer_id;
                paymentCredit.user_id = obj.user_id;
                paymentCredit.date = obj.date;
                paymentCredit.description = "Sales cash product";
                paymentCredit.is_cash = obj.is_cash;

                // set item
                itemPaymentCredit = new ItemPaymentCreditProduct();
                itemPaymentCredit.sale_id = obj.sale_id;
                itemPaymentCredit.SellingProduct = obj;
                itemPaymentCredit.amount = obj.grand_total;
                itemPaymentCredit.description = string.Empty;

                // set item payment
                paymentCredit.item_payment_credit.Add(itemPaymentCredit);

                // save item payment
                result = paymentCreditRepo.Save(paymentCredit, true);
            }

            return result;
        }

        /// <summary>
        /// Method untuk menghapus payment credit jika terjadi perubahan status invoice dari cash ke Credit
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int HapusPaymentCredit(SellingProduct obj)
        {
            PaymentCreditProduct DebtPayment;
            ItemPaymentCreditProduct itemDebtPayment;
            IPaymentCreditProductRepository DebtPaymentRepo = new PaymentCreditProductRepository(_context, _log);

            var result = 0;

            // set detail
            itemDebtPayment = DebtPaymentRepo.GetBySellingID(obj.sale_id);
            if (itemDebtPayment != null)
            {
                DebtPayment = itemDebtPayment.PaymentCreditProduct;
                result = DebtPaymentRepo.Delete(DebtPayment);
            }

            return result;
        }

        public int Update(SellingProduct obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total_invoice = GetTotalInvoice(obj);

                // update header
                result = _context.db.Update<SellingProduct>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_jual_deleted)
                {
                    result = _context.db.Delete<ItemSellingProduct>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_jual.Where(f => f.Product != null))
                {
                    item.sale_id = obj.sale_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.sale_item_id == null)
                            item.sale_item_id = _context.GetGUID();

                        _context.db.Insert<ItemSellingProduct>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemSellingProduct>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                // jika terjadi perubahan status invoice dari cash ke Credit
                if (obj.tanggal_creditTerm_old.IsNull() && !obj.due_date.IsNull())
                {
                    result = HapusPaymentCredit(obj);
                    if (result > 0)
                        obj.total_payment = 0;
                }
                else if (obj.due_date.IsNull()) // jika sales cash, langsung update ke payment credit
                {
                    result = SavePaymentCredit(obj);
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
                result = 0;
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(SellingProduct obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<SellingProduct>(obj) ? 1 : 0;

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

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new SellingProduct().GetTableName());
        }




        //public List<string> GetQuotationsByCustomerId(string customerId)
        //{
        //    using (IDapperContext context = new DapperContext())
        //    {
        //        return context.GetQuotationsByCustomerId(customerId);
        //    }
        //}




        public IList<SellingProduct> GetAll(string name)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetInvoiceCustomer(string id, string invoice)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                object param = null;

                if (invoice.Length > 0)
                {
                    invoice = invoice.ToLower() + "%";
                    param = new { id, invoice };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND LOWER(t_product_sales.invoice) LIKE @invoice");
                }
                else
                {
                    param = new { id };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, param).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                if (isLunas)
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND t_product_sales.due_date IS NOT NULL AND (t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.shipping_cost + t_product_sales.tax) <= t_product_sales.total_payment");
                }
                else
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND t_product_sales.due_date IS NOT NULL AND (t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.shipping_cost + t_product_sales.tax) > t_product_sales.total_payment");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { id }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetInvoiceKreditByInvoice(string id, string invoice)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND LOWER(t_product_sales.invoice) LIKE @invoice AND t_product_sales.due_date IS NOT NULL AND (t_product_sales.total_invoice - t_product_sales.discount + t_product_sales.shipping_cost + t_product_sales.tax) > t_product_sales.total_payment");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                invoice = invoice.ToLower() + "%";
                oList = MappingRecordToObject(_sql, new { id, invoice }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                string tanggalMulaiStr = tanggalMulai.ToShortDateString();
                string tanggalSelesaiStr = tanggalSelesai.ToShortDateString();


                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { tanggalMulai, tanggalSelesai });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }




        public IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                name = "%" + name.ToLower() + "%";

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai AND LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.date DESC, t_product_sales.invoice");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, name }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<SellingProduct> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            IList<SellingProduct> oList = new List<SellingProduct>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_product_sales.invoice DESC");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @limit");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, limit }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }


    }
}
