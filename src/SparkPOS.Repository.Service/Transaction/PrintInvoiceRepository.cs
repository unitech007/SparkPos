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
using SparkPOS.Model.Invoice;
using SparkPOS.Repository.Api;
using SparkPOS.Model.quotation;

namespace SparkPOS.Repository.Service
{
    public class PrintInvoiceRepository : IPrintInvoiceRepository
    {
        private IDapperContext _context;
        private ILog _log;
        
        public PrintInvoiceRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<InvoicePurchase> GetInvoicePurchase(string beliProductId)
        {
            IList<InvoicePurchase> oList = new List<InvoicePurchase>();

            try
            {
                var sql = @"SELECT m_supplier.name_supplier, m_supplier.address, m_supplier.contact, m_supplier.phone, m_supplier.cr_no,m_supplier.vat_no,
                            t_purchase_product.invoice, t_purchase_product.date, t_purchase_product.due_date, t_purchase_product.tax, t_purchase_product.discount AS diskon_nota, t_purchase_product.total_invoice,
                            m_product.product_code, m_product.product_name, m_product.unit,
                            t_purchase_order_item.price, t_purchase_order_item.quantity, t_purchase_order_item.return_quantity, t_purchase_order_item.discount
                            FROM public.t_purchase_product INNER JOIN public.t_purchase_order_item ON t_purchase_order_item.purchase_id = t_purchase_product.purchase_id
                            INNER JOIN public.m_product ON t_purchase_order_item.product_id = m_product.product_id
                            INNER JOIN public.m_supplier ON t_purchase_product.supplier_id = m_supplier.supplier_id
                            WHERE t_purchase_product.purchase_id = @beliProductId
                            ORDER BY t_purchase_order_item.system_date";

                oList = _context.db.Query<InvoicePurchase>(sql, new { beliProductId }).ToList();

            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<InvoiceSales> GetInvoiceSales(string jualProductId)
        {
            IList<InvoiceSales> oList = new List<InvoiceSales>();

            try
            {
                var sql = @"SELECT m_customer.name_customer, m_customer.address, m_customer.postal_code, m_customer.contact, m_customer.phone, m_customer.cr_no,
                            m_province2.name_province AS provinsi, m_regency2.name_regency AS regency, m_subdistrict.name_subdistrict AS subdistrict, m_customer.vat_no,
                            t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, t_product_sales.description, t_product_sales.tax, t_product_sales.courier, t_product_sales.shipping_cost, t_product_sales.discount AS diskon_nota, t_product_sales.total_invoice,
                            t_product_sales.is_sdac, t_product_sales.is_dropship, t_product_sales.shipping_to, t_product_sales.shipping_address, t_product_sales.shipping_subdistrict, t_product_sales.shipping_country, t_product_sales.shipping_regency, t_product_sales.shipping_village, t_product_sales.shipping_city, t_product_sales.shipping_postal_code, t_product_sales.shipping_phone,
                            t_product_sales.from_label1, t_product_sales.from_label2, t_product_sales.from_label3, t_product_sales.from_label4,
                            t_product_sales.to_label1, t_product_sales.to_label2, t_product_sales.to_label3, t_product_sales.to_label4,
                            m_product.product_code, m_product.product_name, m_product.unit,
                            COALESCE(t_sales_order_item.description, t_sales_order_item.description, '') AS keterangan_item, t_sales_order_item.selling_price AS price, t_sales_order_item.quantity, t_sales_order_item.return_quantity, t_sales_order_item.discount
                            FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                            INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                            INNER JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                            LEFT JOIN public.m_province2 ON m_customer.province_id = m_province2.province_id
                            LEFT JOIN public.m_regency2 ON m_customer.regency_id = m_regency2.regency_id
                            LEFT JOIN public.m_subdistrict ON m_customer.subdistrict_id = m_subdistrict.subdistrict_id
                            WHERE t_product_sales.sale_id = @jualProductId
                            ORDER BY t_sales_order_item.system_date";

                oList = _context.db.Query<InvoiceSales>(sql, new { jualProductId }).ToList();

            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

    }
}
