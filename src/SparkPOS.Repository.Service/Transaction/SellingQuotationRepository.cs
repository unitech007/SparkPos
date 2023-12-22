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
using SparkPOS.Model.Transaction;

namespace SparkPOS.Repository.Service
{
    public class SellingQuotationRepository : ISellingQuotationRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_sales_quotation.quotation_id, t_sales_quotation.quotation, t_sales_quotation.date, 
                                              t_sales_quotation.tax, t_sales_quotation.courier, t_sales_quotation.shipping_cost, t_sales_quotation.discount, t_sales_quotation.total_quotation, t_sales_quotation.total_payment,  t_sales_quotation.description, t_sales_quotation.system_date, 
                                              t_sales_quotation.is_dropship, t_sales_quotation.shipping_to, t_sales_quotation.shipping_address, t_sales_quotation.shipping_subdistrict, t_sales_quotation.shipping_country, t_sales_quotation.shipping_regency, t_sales_quotation.shipping_village, t_sales_quotation.shipping_city, t_sales_quotation.shipping_postal_code, t_sales_quotation.shipping_phone,
                                              t_sales_quotation.from_label1, t_sales_quotation.from_label2, t_sales_quotation.from_label3, t_sales_quotation.from_label4,
                                              t_sales_quotation.to_label1, t_sales_quotation.to_label2, t_sales_quotation.to_label3, t_sales_quotation.to_label4,
                                              m_customer.customer_id, m_customer.name_customer, m_customer.province_id, m_customer.regency_id, m_customer.subdistrict_id, m_customer.address, m_customer.postal_code, m_customer.phone, m_customer.discount,
                                              m_user.user_id, m_user.name_user,
                                              t_machine.machine_id, t_machine.starting_balance,
                                              m_dropshipper.dropshipper_id, m_dropshipper.name_dropshipper, m_dropshipper.address, m_dropshipper.phone
                                              FROM public.t_sales_quotation LEFT JOIN public.m_customer ON t_sales_quotation.customer_id = m_customer.customer_id
                                              LEFT JOIN m_user ON m_user.user_id = t_sales_quotation.user_id
                                              LEFT JOIN t_machine ON t_machine.machine_id= t_sales_quotation.machine_id
                                              LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_sales_quotation.dropshipper_id
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT DISTINCT t_sales_quotation.quotation_id, t_sales_quotation.return_quotation_id, t_sales_quotation.quotation, t_sales_quotation.date, t_sales_quotation.due_date, 
                                                     t_sales_quotation.tax, t_sales_quotation.courier, t_sales_quotation.shipping_cost, t_sales_quotation.discount, t_sales_quotation.total_quotation, t_sales_quotation.total_payment, t_sales_quotation.total_payment AS total_repayment_old, t_sales_quotation.description, t_sales_quotation.system_date, 
                                                     t_sales_quotation.is_sdac, t_sales_quotation.is_dropship, t_sales_quotation.shipping_to, t_sales_quotation.shipping_address, t_sales_quotation.shipping_subdistrict, t_sales_quotation.shipping_country, t_sales_quotation.shipping_regency, t_sales_quotation.shipping_village, t_sales_quotation.shipping_city, t_sales_quotation.shipping_postal_code, t_sales_quotation.shipping_phone,
                                                     t_sales_quotation.from_label1, t_sales_quotation.from_label2, t_sales_quotation.from_label3, t_sales_quotation.from_label4,
                                                     t_sales_quotation.to_label1, t_sales_quotation.to_label2, t_sales_quotation.to_label3, t_sales_quotation.to_label4,
                                                     m_customer.customer_id, m_customer.name_customer, m_customer.province_id, m_customer.regency_id, m_customer.subdistrict_id, m_customer.address, m_customer.postal_code, m_customer.phone, m_customer.discount, m_customer.credit_limit,
                                                     m_user.user_id, m_user.name_user,
                                                     t_machine.machine_id, t_machine.starting_balance,
                                                     m_dropshipper.dropshipper_id, m_dropshipper.name_dropshipper, m_dropshipper.address, m_dropshipper.phone
                                                     FROM public.t_sales_quotation INNER JOIN public.t_sales_quotation_item ON t_sales_quotation.quotation_id = t_sales_quotation_item.quotation_id
                                                     LEFT JOIN public.m_customer ON t_sales_quotation.customer_id = m_customer.customer_id
                                                     LEFT JOIN m_user ON m_user.user_id = t_sales_quotation.user_id
                                                     LEFT JOIN t_machine ON t_machine.machine_id= t_sales_quotation.machine_id
                                                     LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_sales_quotation.dropshipper_id
                                                     {WHERE}
                                                     {ORDER BY}
                                                     {OFFSET}";

        private const string SQL_TEMPLATE_FOR_PAGING = @"SELECT COUNT(*)
                                                         FROM public.t_sales_quotation LEFT JOIN public.m_customer ON t_sales_quotation.customer_id = m_customer.customer_id
                                                         LEFT JOIN m_user ON m_user.user_id = t_sales_quotation.user_id
                                                         LEFT JOIN t_machine ON t_machine.machine_id= t_sales_quotation.machine_id
                                                         LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_sales_quotation.dropshipper_id
                                                         {WHERE}";

        private const string SQL_TEMPLATE_DETAIL_FOR_PAGING = @"SELECT COUNT(DISTINCT t_sales_quotation.quotation_id)
                                                                FROM public.t_sales_quotation INNER JOIN public.t_sales_quotation_item ON t_sales_quotation.quotation_id = t_sales_quotation_item.quotation_id
                                                                LEFT JOIN public.m_customer ON t_sales_quotation.customer_id = m_customer.customer_id
                                                                LEFT JOIN m_user ON m_user.user_id = t_sales_quotation.user_id
                                                                LEFT JOIN t_machine ON t_machine.machine_id= t_sales_quotation.machine_id
                                                                LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_sales_quotation.dropshipper_id
                                                                {WHERE}";

        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public SellingQuotationRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<SellingQuotation> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<SellingQuotation> oList = _context.db.Query<SellingQuotation, Customer, User, CashierMachine, Dropshipper, SellingQuotation>(sql, (j, c, p, m, d) =>
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

        public IList<ItemSellingQuotation> GetItemSelling(string jualId)
        {
            IList<ItemSellingQuotation> oList = new List<ItemSellingQuotation>();

            try
            {
                var sql = @"SELECT t_sales_quotation_item.quotation_item_id, t_sales_quotation_item.quotation_id, t_sales_quotation_item.user_id, t_sales_quotation_item.purchase_price, t_sales_quotation_item.selling_price, 
                            t_sales_quotation_item.quantity, t_sales_quotation_item.quantity AS old_jumlah, t_sales_quotation_item.return_quantity, t_sales_quotation_item.discount, COALESCE(t_sales_quotation_item.description, t_sales_quotation_item.description, '') AS description, t_sales_quotation_item.system_date, 1 as entity_state,
                            m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.purchase_price, m_product.selling_price, m_product.discount, m_product.stock, m_product.warehouse_stock,
                            m_category.category_id, m_category.name_category, m_category.discount
                            FROM public.t_sales_quotation_item INNER JOIN public.m_product ON t_sales_quotation_item.product_id = m_product.product_id
                            INNER JOIN public.m_category ON m_category.category_id = m_product.category_id
                            WHERE t_sales_quotation_item.quotation_id = @jualId
                            ORDER BY t_sales_quotation_item.system_date";

                oList = _context.db.Query<ItemSellingQuotation, Product, Category, ItemSellingQuotation>(sql, (ij, p, g) =>
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
            }

            return oList;
        }

        public SellingQuotation GetByID(string id)
        {
            SellingQuotation obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_quotation.quotation_id = @id");
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

        public SellingQuotation GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            SellingQuotation obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_quotation.date = CURRENT_DATE AND t_sales_quotation.user_id = @userId AND t_sales_quotation.machine_id= @mesinId");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.system_date DESC LIMIT 1");
                _sql = _sql.Replace("{OFFSET}", "");

                obj = MappingRecordToObject(_sql, new { userId, mesinId }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<SellingQuotation> GetByName(string name)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_sales_quotation.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
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

        public IList<SellingQuotation> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                name = "%" + name.ToLower() + "%";

                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_sales_quotation.description) LIKE @name");

                if (isCekKeteranganItemSelling)
                    sqlPageCount = SQL_TEMPLATE_DETAIL_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_sales_quotation.description) LIKE @name OR LOWER(t_sales_quotation_item.description) LIKE @name");

                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { name });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_sales_quotation.description) LIKE @name");

                if (isCekKeteranganItemSelling)
                    _sql = SQL_TEMPLATE_DETAIL.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_sales_quotation.description) LIKE @name OR LOWER(t_sales_quotation_item.description) LIKE @name");

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { name, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetAll()
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private double GetTotalInvoice(SellingQuotation obj)
        {
            var total = obj.item_jual.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                     .Sum(f => (f.quantity - f.return_quantity) * (f.selling_price - (f.discount / 100 * f.selling_price)));

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }



        public int Save(SellingQuotation obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                if (obj.quotation_id == null)
                    obj.quotation_id = _context.GetGUID();

                obj.total_quotation = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<SellingQuotation>(obj, transaction);

                // insert detail
                foreach (var item in obj.item_jual.Where(f => f.Product != null))
                {
                    if (item.product_id.Length > 0)
                    {
                        if (item.quotation_item_id == null)
                            item.quotation_item_id = _context.GetGUID();

                        item.quotation_id = obj.quotation_id;
                        item.user_id = obj.user_id;

                        _context.db.Insert<ItemSellingQuotation>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                // jika Purchase cash, langsung insert ke Dept Payment
                //if (obj.due_date.IsNull())
                //{
                //    result = SavePaymentCredit(obj);
                //    if (result > 0)
                //        obj.total_payment = obj.grand_total;

                //}

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

        /// <summary>
        /// Method khusus untuk menyimpan payment sales cash
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //private int SavePaymentCredit(SellingQuotation obj)
        //{
        //    PaymentCreditProduct paymentCredit;
        //    ItemPaymentCreditProduct itemPaymentCredit;
        //    IPaymentCreditProductRepository paymentCreditRepo = new PaymentCreditProductRepository(_context, _log);

        //    var result = 0;

        //    // set detail            
        //    itemPaymentCredit = paymentCreditRepo.GetBySellingID(obj.quotation_id);
        //    if (itemPaymentCredit != null) // already there pelunasan
        //    {
        //        itemPaymentCredit.amount = obj.grand_total; // GetTotalInvoiceSetelahDiskonDanPPN(obj);
        //      //  itemPaymentCredit.SellingQuotation = new SellingQuotation { quotation_id = itemPaymentCredit.quotation_id };
        //        itemPaymentCredit.entity_state = EntityState.Modified;

        //        // set header by detail
        //        paymentCredit = itemPaymentCredit.PaymentCreditProduct;
        //        paymentCredit.is_cash = obj.is_cash;

        //        // set item payment
        //        paymentCredit.item_payment_credit.Add(itemPaymentCredit);

        //        result = paymentCreditRepo.Update(paymentCredit, true);
        //    }
        //    else // Not yet there pelunasan debt
        //    {
        //        paymentCredit = new PaymentCreditProduct();

        //        // set header
        //        paymentCredit.customer_id = obj.customer_id;
        //        paymentCredit.user_id = obj.user_id;
        //        paymentCredit.date = obj.date;
        //        paymentCredit.description = "Sales cash product";
        //        paymentCredit.is_cash = obj.is_cash;

        //        // set item
        //        itemPaymentCredit = new ItemPaymentCreditProduct();
        //        itemPaymentCredit.quotation_id = obj.quotation_id;
        //        itemPaymentCredit.SellingQuotation = obj;
        //        itemPaymentCredit.amount = obj.grand_total;
        //        itemPaymentCredit.description = string.Empty;

        //        // set item payment
        //        paymentCredit.item_payment_credit.Add(itemPaymentCredit);

        //        // save item payment
        //        result = paymentCreditRepo.Save(paymentCredit, true);
        //    }

        //    return result;
        //}

        /// <summary>
        /// Method untuk menghapus payment credit jika terjadi perubahan status quotation dari cash ke Credit
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int HapusPaymentCredit(SellingQuotation obj)
        {
            PaymentCreditProduct DebtPayment;
            ItemPaymentCreditProduct itemDebtPayment;
            IPaymentCreditProductRepository DebtPaymentRepo = new PaymentCreditProductRepository(_context, _log);

            var result = 0;

            // set detail
            itemDebtPayment = DebtPaymentRepo.GetBySellingID(obj.quotation_id);
            if (itemDebtPayment != null)
            {
                DebtPayment = itemDebtPayment.PaymentCreditProduct;
                result = DebtPaymentRepo.Delete(DebtPayment);
            }

            return result;
        }

        public int Update(SellingQuotation obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total_quotation = GetTotalInvoice(obj);

                // update header
                result = _context.db.Update<SellingQuotation>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_jual_deleted)
                {
                    result = _context.db.Delete<ItemSellingQuotation>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_jual.Where(f => f.Product != null))
                {
                    item.quotation_id = obj.quotation_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.quotation_item_id == null)
                            item.quotation_item_id= _context.GetGUID();

                        _context.db.Insert<ItemSellingQuotation>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemSellingQuotation>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                // jika terjadi perubahan status quotation dari cash ke Credit
                //if (obj.tanggal_creditTerm_old.IsNull() && !obj.due_date.IsNull())
                //{
                //    result = HapusPaymentCredit(obj);
                //    if (result > 0)
                //        obj.total_payment = 0;
                //}
                //else if (obj.due_date.IsNull()) // jika sales cash, langsung update ke payment credit
                //{
                //    result = SavePaymentCredit(obj);
                //    if (result > 0)
                //        obj.total_payment = obj.grand_total;
                //}

                _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Update data");

                result = 1;
            }
            catch (Exception ex)
            {
                result = 0;
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(SellingQuotation obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<SellingQuotation>(obj) ? 1 : 0;

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
            return _context.GetLastInvoice(new SellingQuotation().GetTableName());
        }

        public IList<SellingQuotation> GetAll(string name)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
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

        public IList<SellingQuotation> GetInvoiceCustomer(string id, string quotation)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                object param = null;

                if (quotation.Length > 0)
                {
                    quotation = quotation.ToLower() + "%";
                    param = new { id, quotation };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND LOWER(t_sales_quotation.quotation) LIKE @quotation");
                }
                else
                {
                    param = new { id };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, param).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                if (isLunas)
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND t_sales_quotation.due_date IS NOT NULL AND (t_sales_quotation.total_quotation - t_sales_quotation.discount + t_sales_quotation.shipping_cost + t_sales_quotation.tax) <= t_sales_quotation.total_payment");
                }
                else
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND t_sales_quotation.due_date IS NOT NULL AND (t_sales_quotation.total_quotation - t_sales_quotation.discount + t_sales_quotation.shipping_cost + t_sales_quotation.tax) > t_sales_quotation.total_payment");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { id }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetInvoiceKreditByInvoice(string id, string quotation)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND LOWER(t_sales_quotation.quotation) LIKE @quotation AND t_sales_quotation.due_date IS NOT NULL AND (t_sales_quotation.total_quotation - t_sales_quotation.discount + t_sales_quotation.shipping_cost + t_sales_quotation.tax) > t_sales_quotation.total_payment");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "");

                quotation = quotation.ToLower() + "%";
                oList = MappingRecordToObject(_sql, new { id, quotation }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_quotation.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
         {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                string tanggalMulaiStr = tanggalMulai.ToShortDateString();
                string tanggalSelesaiStr = tanggalSelesai.ToShortDateString();


                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE t_sales_quotation.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { tanggalMulai, tanggalSelesai });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_quotation.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }




        public IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                name = "%" + name.ToLower() + "%";

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_quotation.date BETWEEN @tanggalMulai AND @tanggalSelesai AND LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.date DESC, t_sales_quotation.quotation");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, name }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingQuotation> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            IList<SellingQuotation> oList = new List<SellingQuotation>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_sales_quotation.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_sales_quotation.quotation DESC");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @limit");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, limit }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public string GetLastQuotation()
        {
            return _context.GetLastQuotation(new SellingQuotation().GetTableName());
        }

        SellingQuotation ISellingQuotationRepository.GetByID(string id)
        {
            throw new NotImplementedException();
        }

        SellingQuotation ISellingQuotationRepository.GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            throw new NotImplementedException();
        }

     

        IList<SellingQuotation> ISellingQuotationRepository.GetInvoiceCustomer(string id, string quotation)
        {
            throw new NotImplementedException();
        }

        IList<SellingQuotation> ISellingQuotationRepository.GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            throw new NotImplementedException();
        }

        IList<SellingQuotation> ISellingQuotationRepository.GetInvoiceKreditByInvoice(string id, string quotation)
        {
            throw new NotImplementedException();
        }

        IList<SellingQuotation> ISellingQuotationRepository.GetByName(string name)
        {
            throw new NotImplementedException();
        }

        IList<SellingQuotation> ISellingQuotationRepository.GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            throw new NotImplementedException();
        }

       
        IList<SellingQuotation> ISellingQuotationRepository.GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            throw new NotImplementedException();
        }


       
        //public int Update(SellingQuotation obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Delete(SellingQuotation obj)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
