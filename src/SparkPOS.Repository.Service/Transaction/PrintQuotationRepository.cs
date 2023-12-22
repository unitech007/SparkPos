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
    public class PrintQuotationRepository : IPrintQuotationRepository
    {
        private IDapperContext _context;
        private ILog _log;
        
        public PrintQuotationRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<QuotationSales> GetQuotationSales(string jualProductId)
        {
            IList<QuotationSales> oList = new List<QuotationSales>();

            try
            {
                var sql = @"SELECT m_customer.name_customer, m_customer.address, m_customer.postal_code, m_customer.contact, m_customer.phone, 
                            m_province2.name_province AS provinsi, m_regency2.name_regency AS regency, m_subdistrict.name_subdistrict AS subdistrict, 
                            t_sales_quotation.quotation, t_sales_quotation.date, t_sales_quotation.description, t_sales_quotation.tax, t_sales_quotation.courier, t_sales_quotation.shipping_cost, t_sales_quotation.discount AS diskon_nota, t_sales_quotation.total_quotation,
                            t_sales_quotation.is_sdac, t_sales_quotation.is_dropship, t_sales_quotation.shipping_to, t_sales_quotation.shipping_address, t_sales_quotation.shipping_subdistrict, t_sales_quotation.shipping_country, t_sales_quotation.shipping_regency, t_sales_quotation.shipping_village, t_sales_quotation.shipping_city, t_sales_quotation.shipping_postal_code, t_sales_quotation.shipping_phone,
                            t_sales_quotation.from_label1, t_sales_quotation.from_label2, t_sales_quotation.from_label3, t_sales_quotation.from_label4,
                            t_sales_quotation.to_label1, t_sales_quotation.to_label2, t_sales_quotation.to_label3, t_sales_quotation.to_label4,
                            m_product.product_code, m_product.product_name, m_product.unit,
                            COALESCE(t_sales_quotation_item.description, t_sales_quotation_item.description, '') AS keterangan_item, t_sales_quotation_item.selling_price AS price, t_sales_quotation_item.quantity, t_sales_quotation_item.return_quantity, t_sales_quotation_item.discount
                            FROM public.t_sales_quotation INNER JOIN public.t_sales_quotation_item ON t_sales_quotation_item.quotation_id = t_sales_quotation.quotation_id
                            INNER JOIN public.m_product ON t_sales_quotation_item.product_id = m_product.product_id
                            INNER JOIN public.m_customer ON t_sales_quotation.customer_id = m_customer.customer_id
                            LEFT JOIN public.m_province2 ON m_customer.province_id = m_province2.province_id
                            LEFT JOIN public.m_regency2 ON m_customer.regency_id = m_regency2.regency_id
                            LEFT JOIN public.m_subdistrict ON m_customer.subdistrict_id = m_subdistrict.subdistrict_id
                            WHERE t_sales_quotation.quotation_id = @jualProductId
                            ORDER BY t_sales_quotation_item.system_date";

                oList = _context.db.Query<QuotationSales>(sql, new { jualProductId }).ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
