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
    public class SellingDeliveryNotesRepository : ISellingDeliveryNotesRepository
    {
        private const string SQL_TEMPLATE = @"SELECT t_delivery_notes.delivery_id, t_delivery_notes.delivery, t_delivery_notes.delivery_date, 
                                              t_delivery_notes.tax, t_delivery_notes.courier, t_delivery_notes.shipping_cost, t_delivery_notes.discount, t_delivery_notes.total_invoice,  t_delivery_notes.description, t_delivery_notes.system_date, 
                                              t_delivery_notes.is_dropship, t_delivery_notes.shipping_to, t_delivery_notes.shipping_address, t_delivery_notes.shipping_subdistrict, t_delivery_notes.shipping_country, t_delivery_notes.shipping_regency, t_delivery_notes.shipping_village, t_delivery_notes.shipping_city, t_delivery_notes.shipping_postal_code, t_delivery_notes.shipping_phone,
                                              t_delivery_notes.from_label1, t_delivery_notes.from_label2, t_delivery_notes.from_label3, t_delivery_notes.from_label4,
                                              t_delivery_notes.to_label1, t_delivery_notes.to_label2, t_delivery_notes.to_label3, t_delivery_notes.to_label4,
                                              m_customer.customer_id, m_customer.name_customer, m_customer.province_id, m_customer.regency_id, m_customer.subdistrict_id, m_customer.address, m_customer.postal_code, m_customer.phone, m_customer.discount,
                                              m_user.user_id, m_user.name_user,
                                              t_machine.machine_id, t_machine.starting_balance,
                                              m_dropshipper.dropshipper_id, m_dropshipper.name_dropshipper, m_dropshipper.address, m_dropshipper.phone
                                              FROM public.t_delivery_notes LEFT JOIN public.m_customer ON t_delivery_notes.customer_id = m_customer.customer_id
                                              LEFT JOIN m_user ON m_user.user_id = t_delivery_notes.user_id
                                              LEFT JOIN t_machine ON t_machine.machine_id= t_delivery_notes.machine_id
                                              LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_delivery_notes.dropshipper_id
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT DISTINCT t_delivery_notes.delivery_id, t_delivery_notes.return_delivery_id, t_delivery_notes.delivery, t_delivery_notes.delivery_date, t_delivery_notes.due_date, 
                                                     t_delivery_notes.tax, t_delivery_notes.courier, t_delivery_notes.shipping_cost, t_delivery_notes.discount, t_delivery_notes.total_invoice,   t_delivery_notes.description, t_delivery_notes.system_date, 
                                                     t_delivery_notes.is_sdac, t_delivery_notes.is_dropship, t_delivery_notes.shipping_to, t_delivery_notes.shipping_address, t_delivery_notes.shipping_subdistrict, t_delivery_notes.shipping_country, t_delivery_notes.shipping_regency, t_delivery_notes.shipping_village, t_delivery_notes.shipping_city, t_delivery_notes.shipping_postal_code, t_delivery_notes.shipping_phone,
                                                     t_delivery_notes.from_label1, t_delivery_notes.from_label2, t_delivery_notes.from_label3, t_delivery_notes.from_label4,
                                                     t_delivery_notes.to_label1, t_delivery_notes.to_label2, t_delivery_notes.to_label3, t_delivery_notes.to_label4,
                                                     m_customer.customer_id, m_customer.name_customer, m_customer.province_id, m_customer.regency_id, m_customer.subdistrict_id, m_customer.address, m_customer.postal_code, m_customer.phone, m_customer.discount, m_customer.credit_limit,
                                                     m_user.user_id, m_user.name_user,
                                                     t_machine.machine_id, t_machine.starting_balance,
                                                     m_dropshipper.dropshipper_id, m_dropshipper.name_dropshipper, m_dropshipper.address, m_dropshipper.phone
                                                     FROM public.t_delivery_notes INNER JOIN public.t_delivery_items ON t_delivery_notes.delivery_id = t_delivery_items.delivery_id
                                                     LEFT JOIN public.m_customer ON t_delivery_notes.customer_id = m_customer.customer_id
                                                     LEFT JOIN m_user ON m_user.user_id = t_delivery_notes.user_id
                                                     LEFT JOIN t_machine ON t_machine.machine_id= t_delivery_notes.machine_id
                                                     LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_delivery_notes.dropshipper_id
                                                     {WHERE}
                                                     {ORDER BY}
                                                     {OFFSET}";

        private const string SQL_TEMPLATE_FOR_PAGING = @"SELECT COUNT(*)
                                                         FROM public.t_delivery_notes LEFT JOIN public.m_customer ON t_delivery_notes.customer_id = m_customer.customer_id
                                                         LEFT JOIN m_user ON m_user.user_id = t_delivery_notes.user_id
                                                         LEFT JOIN t_machine ON t_machine.machine_id= t_delivery_notes.machine_id
                                                         LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_delivery_notes.dropshipper_id
                                                         {WHERE}";

        private const string SQL_TEMPLATE_DETAIL_FOR_PAGING = @"SELECT COUNT(DISTINCT t_delivery_notes.delivery_id)
                                                                FROM public.t_delivery_notes INNER JOIN public.t_delivery_items ON t_delivery_notes.delivery_id = t_delivery_items.delivery_id
                                                                LEFT JOIN public.m_customer ON t_delivery_notes.customer_id = m_customer.customer_id
                                                                LEFT JOIN m_user ON m_user.user_id = t_delivery_notes.user_id
                                                                LEFT JOIN t_machine ON t_machine.machine_id= t_delivery_notes.machine_id
                                                                LEFT JOIN m_dropshipper ON m_dropshipper.dropshipper_id = t_delivery_notes.dropshipper_id
                                                                {WHERE}";

        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public SellingDeliveryNotesRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<SellingDeliveryNotes> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<SellingDeliveryNotes> oList = _context.db.Query<SellingDeliveryNotes, Customer, User, CashierMachine, Dropshipper, SellingDeliveryNotes>(sql, (j, c, p, m, d) =>
            {
                if (c != null)
                {
                    j.customer_id = c.customer_id; j.Customer = c;
                }

                if (p != null)
                {
                    j.user_id = p.user_id; j.User = p;
                }

                //if (m != null)
                //{
                //    j.machine_id = m.machine_id; j.Machine = m;
                //}

                //if (d != null)
                //{
                //    j.dropshipper_id = d.dropshipper_id; j.Dropshipper = d;
                //}

                return j;
            }, param, splitOn: "customer_id, user_id, machine_id, dropshipper_id");

            return oList;
        }
      //  string GetLastDeliveryNotes();

        public string GetLastDeliveryNotes()
        {
            return _context.GetLastDeliveryNotes(new SellingProduct().GetTableName());
        }

        public List<string> GetInvoiceByCustomerId(string customerId)
        {
            List<string> quotations = new List<string>();
            string query = @"SELECT invoice FROM t_product_sales WHERE customer_id = @CustomerId"; ;
            quotations = _context.db.Query<string>(query, new { customerId }).ToList();

            return quotations;
        }
        
        public List<ItemSellingProduct> GetProductDetailsByInvoice(string invoiceNumber)
        {
            try
            {
                var query = @" SELECT  t_sales_order_item.product_id AS product_id, m_product.product_name,m_product.product_code, t_product_sales.description,
                                                                    t_sales_order_item.discount, t_sales_order_item.quantity,  t_sales_order_item.selling_price,t_sales_order_item.sale_id 
                                                                    FROM    public.t_product_sales  INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                                    INNER JOIN public.m_product ON m_product.product_id = t_sales_order_item.product_id
                                                                    WHERE  t_product_sales.invoice = @invoiceNumber";

;

                var result = _context.db.Query(query, new { invoiceNumber });

                var productDetails = new List<ItemSellingProduct>();

                foreach (var row in result)
                {
                    var itemSellingQuotation = new ItemSellingProduct
                    {
                        product_id = row.product_id,
                        discount = (double)row.discount,
                        quantity = (double)row.quantity,
                        selling_price = (double)row.selling_price,
                        sale_id = row.sale_id,
                        description = row.description,
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
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        private IList<PriceWholesale> GetListPriceWholesale(string produkId)
        {
            IPriceWholesaleRepository repo = new PriceWholesaleRepository(_context, _log);

            return repo.GetListPriceWholesale(produkId);
        }

//        public IList<ItemSellingDeliveryNotes> GetItemSelling(string jualId)
//        {
//            IList<ItemSellingDeliveryNotes> oList = new List<ItemSellingDeliveryNotes>();

//            try
//            {
//                //var sql = @"SELECT t_delivery_items.delivery_item_id, t_delivery_items.delivery_id, t_delivery_items.user_id, t_delivery_items.purchase_price, t_delivery_items.selling_price, 
//                //            t_delivery_items.quantity, t_delivery_items.quantity AS old_jumlah, t_delivery_items.return_quantity, t_delivery_items.discount, COALESCE(t_delivery_items.description, t_delivery_items.description, '') AS description, t_delivery_items.system_date, 1 as entity_state,
//                //            m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.purchase_price, m_product.selling_price, m_product.discount, m_product.stock, m_product.warehouse_stock, t_product_sales.invoice 
//                //            m_category.category_id, m_category.name_category, m_category.discount
//                //            FROM public.t_delivery_items INNER JOIN public.m_product ON t_delivery_items.product_id = m_product.product_id
//                //            INNER JOIN public.m_category ON m_category.category_id = m_product.category_id
//                //           INNER JOIN public.t_delivery_notes ON t_delivery_notes.sale_id = t_product_sales.sale_id
//                //            WHERE t_delivery_items.delivery_id = @jualId
//                //            ORDER BY t_delivery_items.system_date";

//                var sql = @"SELECT t_delivery_items.delivery_item_id, t_delivery_items.delivery_id, t_delivery_items.user_id, t_delivery_items.purchase_price, t_delivery_items.selling_price, 
//    t_delivery_items.quantity, t_delivery_items.quantity AS old_jumlah, t_delivery_items.return_quantity, t_delivery_items.discount, 
//    COALESCE(t_delivery_items.description, '') AS description, t_delivery_items.system_date, 1 AS entity_state,
//    m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.purchase_price, 
//    m_product.selling_price, m_product.discount, m_product.stock, m_product.warehouse_stock, t_product_sales.invoice,
//    m_category.category_id, m_category.name_category, m_category.discount
//FROM public.t_delivery_items
//INNER JOIN public.m_product ON t_delivery_items.product_id = m_product.product_id
//INNER JOIN public.m_category ON m_category.category_id = m_product.category_id
//INNER JOIN public.t_delivery_notes ON t_delivery_notes.sale_id = t_product_sales.sale_id 
//INNER JOIN public.t_product_sales ON t_product_sales.sale_id = t_delivery_notes.sale_id
//WHERE t_delivery_items.delivery_id = @jualId
//ORDER BY t_delivery_items.system_date";

//                oList = _context.db.Query<ItemSellingDeliveryNotes, Product, Category, ItemSellingDeliveryNotes>(sql, (ij, p, g) =>
//                {
//                    p.category_id = g.category_id; p.Category = g;
//                    ij.product_id = p.product_id; ij.Product = p;

//                    return ij;
//                }, new { jualId }, splitOn: "product_id, category_id").ToList();

//                foreach (var item in oList)
//                {
//                    item.Product.list_of_harga_grosir = GetListPriceWholesale(item.product_id).ToList();
//                }
//            }
//            catch(Exception ex)
//            {
//            }

//            return oList;
//        }


        public SellingDeliveryNotes GetByID(string id)
        {
            SellingDeliveryNotes obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_id = @id");
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

        public SellingDeliveryNotes GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            SellingDeliveryNotes obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_date = CURRENT_DATE AND t_delivery_notes.user_id = @userId AND t_delivery_notes.machine_id= @mesinId");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.system_date DESC LIMIT 1");
                _sql = _sql.Replace("{OFFSET}", "");

                obj = MappingRecordToObject(_sql, new { userId, mesinId }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<SellingDeliveryNotes> GetByName(string name)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_delivery_notes.description) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
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

        public IList<SellingDeliveryNotes> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                name = "%" + name.ToLower() + "%";

                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_delivery_notes.description) LIKE @name");

                if (isCekKeteranganItemSelling)
                    sqlPageCount = SQL_TEMPLATE_DETAIL_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_delivery_notes.description) LIKE @name OR LOWER(t_delivery_items.description) LIKE @name");

                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { name });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_delivery_notes.description) LIKE @name");

                if (isCekKeteranganItemSelling)
                    _sql = SQL_TEMPLATE_DETAIL.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name OR LOWER(t_delivery_notes.description) LIKE @name OR LOWER(t_delivery_items.description) LIKE @name");

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { name, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetAll()
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        //private double GetTotalInvoice(SellingDeliveryNotes obj)
        //{
        //    var total = obj.item_jual.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
        //                             .Sum(f => (f.quantity - f.return_quantity) * (f.selling_price - (f.discount / 100 * f.selling_price)));

        //    return Math.Round(total, MidpointRounding.AwayFromZero);
        //}

        private double GetTotalInvoice(SellingDeliveryNotes obj)
        {
            var total = obj.item_jual.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                     .Sum(f => (f.quantity - f.return_quantity) * (f.selling_price - (f.discount / 100 * f.selling_price)));

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        //public int Save(SellingDeliveryNotes obj)
        //{
        //    var result = 0;

        //    try
        //    {
        //        _context.BeginTransaction();

        //        var transaction = _context.transaction;

        //        if (obj.delivery_id == null)
        //            obj.delivery_id = _context.GetGUID();

        //        obj.total_invoice = GetTotalInvoice(obj);

        //        // insert header
        //        _context.db.Insert<SellingDeliveryNotes>(obj, transaction);

        //        // insert detail
        //        foreach (var item in obj.item_jual.Where(f => f.Product != null))
        //        {
        //            if (item.product_id.Length > 0)
        //            {
        //                if (item.delivery_item_id == null)
        //                    item.delivery_item_id = _context.GetGUID();

        //                item.delivery_id = obj.delivery_id;
        //                item.user_id = obj.user_id;

        //                _context.db.Insert<ItemSellingDeliveryNotes>(item, transaction);

        //                // update entity state
        //                item.entity_state = EntityState.Unchanged;
        //            }
        //        }

        //        // jika Purchase cash, langsung insert ke Dept Payment
        //        //if (obj.due_date.IsNull())
        //        //{
        //        //    result = SavePaymentCredit(obj);
        //        //    if (result > 0)
        //        //        obj.total_payment = obj.grand_total;

        //        //}

        //        _context.Commit();

        //        LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
        //        _log.Info("Add data");

        //        result = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error("Error:", ex);
        //    }

        //    return result;
        //}

        /// <summary>
        /// Method khusus untuk menyimpan payment sales cash
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //private int SavePaymentCredit(SellingDeliveryNotes obj)
        //{
        //    PaymentCreditProduct paymentCredit;
        //    ItemPaymentCreditProduct itemPaymentCredit;
        //    IPaymentCreditProductRepository paymentCreditRepo = new PaymentCreditProductRepository(_context, _log);

        //    var result = 0;

        //    // set detail            
        //    itemPaymentCredit = paymentCreditRepo.GetBySellingID(obj.delivery_id);
        //    if (itemPaymentCredit != null) // already there pelunasan
        //    {
        //        itemPaymentCredit.amount = obj.grand_total; // GetTotalInvoiceSetelahDiskonDanPPN(obj);
        //      //  itemPaymentCredit.SellingDeliveryNotes = new SellingDeliveryNotes { delivery_id = itemPaymentCredit.delivery_id };
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
        //        itemPaymentCredit.delivery_id = obj.delivery_id;
        //        itemPaymentCredit.SellingDeliveryNotes = obj;
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
        //private int HapusPaymentCredit(SellingDeliveryNotes obj)
        //{
        //    PaymentCreditProduct DebtPayment;
        //    ItemPaymentCreditProduct itemDebtPayment;
        //    IPaymentCreditProductRepository DebtPaymentRepo = new PaymentCreditProductRepository(_context, _log);

        //    var result = 0;

        //    // set detail
        //    itemDebtPayment = DebtPaymentRepo.GetBySellingID(obj.delivery_id);
        //    if (itemDebtPayment != null)
        //    {
        //        DebtPayment = itemDebtPayment.PaymentCreditProduct;
        //        result = DebtPaymentRepo.Delete(DebtPayment);
        //    }

        //    return result;
        //}

        //public int Update(SellingDeliveryNotes obj)
        //{
        //    var result = 0;

        //    try
        //    {
        //        _context.BeginTransaction();

        //        var transaction = _context.transaction;

        //        obj.total_invoice = GetTotalInvoice(obj);

        //        // update header
        //        result = _context.db.Update<SellingDeliveryNotes>(obj, transaction) ? 1 : 0;

        //        // delete detail
        //        foreach (var item in obj.item_jual_deleted)
        //        {
        //            result = _context.db.Delete<ItemSellingDeliveryNotes>(item, transaction) ? 1 : 0;
        //        }

        //        // insert/update detail
        //        foreach (var item in obj.item_jual.Where(f => f.Product != null))
        //        {
        //            item.delivery_id = obj.delivery_id;
        //            item.user_id = obj.user_id;

        //            if (item.entity_state == EntityState.Added)
        //            {
        //                if (item.delivery_item_id == null)
        //                    item.delivery_item_id= _context.GetGUID();

        //                _context.db.Insert<ItemSellingDeliveryNotes>(item, transaction);

        //                result = 1;
        //            }
        //            else if (item.entity_state == EntityState.Modified)
        //            {
        //                result = _context.db.Update<ItemSellingDeliveryNotes>(item, transaction) ? 1 : 0;
        //            }

        //            // update entity state
        //            item.entity_state = EntityState.Unchanged;
        //        }

        //        // jika terjadi perubahan status quotation dari cash ke Credit
        //        //if (obj.tanggal_creditTerm_old.IsNull() && !obj.due_date.IsNull())
        //        //{
        //        //    result = HapusPaymentCredit(obj);
        //        //    if (result > 0)
        //        //        obj.total_payment = 0;
        //        //}
        //        //else if (obj.due_date.IsNull()) // jika sales cash, langsung update ke payment credit
        //        //{
        //        //    result = SavePaymentCredit(obj);
        //        //    if (result > 0)
        //        //        obj.total_payment = obj.grand_total;
        //        //}

        //        _context.Commit();

        //        LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
        //        _log.Info("Update data");

        //        result = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = 0;
        //        _log.Error("Error:", ex);
        //    }

        //    return result;
        //}

        public int Delete(SellingDeliveryNotes obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<SellingDeliveryNotes>(obj) ? 1 : 0;

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

        
      

        //public string GetLastInvoice()
        //{
        //    return _context.GetLastInvoice(new SellingProduct().GetTableName());
        //}

        public IList<SellingDeliveryNotes> GetAll(string name)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
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

        public IList<SellingDeliveryNotes> GetInvoiceCustomer(string id, string quotation)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                object param = null;

                if (quotation.Length > 0)
                {
                    quotation = quotation.ToLower() + "%";
                    param = new { id, quotation };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND LOWER(t_delivery_notes.delivery) LIKE @quotation");
                }
                else
                {
                    param = new { id };

                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, param).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                if (isLunas)
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND t_delivery_notes.due_date IS NOT NULL AND (t_delivery_notes.total_invoice - t_delivery_notes.discount + t_delivery_notes.shipping_cost + t_delivery_notes.tax) <= t_delivery_notes.total_payment");
                }
                else
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND t_delivery_notes.due_date IS NOT NULL AND (t_delivery_notes.total_invoice - t_delivery_notes.discount + t_delivery_notes.shipping_cost + t_delivery_notes.tax) > t_delivery_notes.total_payment");
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { id }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetInvoiceKreditByInvoice(string id, string quotation)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id AND LOWER(t_delivery_notes.delivery) LIKE @quotation AND t_delivery_notes.due_date IS NOT NULL AND (t_delivery_notes.total_invoice - t_delivery_notes.discount + t_delivery_notes.shipping_cost + t_delivery_notes.tax) > t_delivery_notes.total_payment");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
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

        public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
         {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                string tanggalMulaiStr = tanggalMulai.ToShortDateString();
                string tanggalSelesaiStr = tanggalSelesai.ToShortDateString();


                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_date BETWEEN @tanggalMulai AND @tanggalSelesai");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { tanggalMulai, tanggalSelesai });

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, pageNumber, pageSize }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }




        public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                name = "%" + name.ToLower() + "%";

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_date BETWEEN @tanggalMulai AND @tanggalSelesai AND LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery_date DESC, t_delivery_notes.delivery");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { tanggalMulai, tanggalSelesai, name }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            IList<SellingDeliveryNotes> oList = new List<SellingDeliveryNotes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE t_delivery_notes.delivery_date BETWEEN @tanggalMulai AND @tanggalSelesai");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY t_delivery_notes.delivery DESC");
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
            return _context.GetLastQuotation(new SellingDeliveryNotes().GetTableName());
        }

        SellingDeliveryNotes ISellingDeliveryNotesRepository.GetByID(string id)
        {
            throw new NotImplementedException();
        }

        SellingDeliveryNotes ISellingDeliveryNotesRepository.GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            throw new NotImplementedException();
        }

     

        IList<SellingDeliveryNotes> ISellingDeliveryNotesRepository.GetInvoiceCustomer(string id, string quotation)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesRepository.GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesRepository.GetInvoiceKreditByInvoice(string id, string quotation)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesRepository.GetByName(string name)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesRepository.GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            throw new NotImplementedException();
        }

       
        IList<SellingDeliveryNotes> ISellingDeliveryNotesRepository.GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            throw new NotImplementedException();
        }

        //public int Save(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}


        public int Save(SellingDeliveryNotes obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                if (obj.delivery_id == null)
                    obj.delivery_id = _context.GetGUID();

                obj.total_invoice = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<SellingDeliveryNotes>(obj, transaction);

                // insert detail
                foreach (var item in obj.item_jual.Where(f => f.Product != null))
                {
                    if (item.product_id.Length > 0)
                    {
                        if (item.delivery_item_id == null)
                            item.delivery_item_id = _context.GetGUID();

                        item.delivery_id = obj.delivery_id;
                        item.user_id = obj.user_id;

                        _context.db.Insert<ItemSellingDeliveryNotes>(item, transaction);

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

      
        public int Update(SellingDeliveryNotes obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total_invoice = GetTotalInvoice(obj);

                // update header
                result = _context.db.Update<SellingDeliveryNotes>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_jual_deleted)
                {
                    result = _context.db.Delete<ItemSellingDeliveryNotes>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_jual.Where(f => f.Product != null))
                {
                    item.delivery_id = obj.sale_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.delivery_item_id == null)
                            item.delivery_item_id = _context.GetGUID();

                        _context.db.Insert<ItemSellingDeliveryNotes>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemSellingDeliveryNotes>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                //// jika terjadi perubahan status invoice dari cash ke Credit
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

        //double ISellingDeliveryNotesRepository.GetTotalInvoice(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}




        //public int Update(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Delete(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
