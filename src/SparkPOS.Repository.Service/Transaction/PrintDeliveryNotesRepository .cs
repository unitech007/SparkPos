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
using SparkPOS.Model;

namespace SparkPOS.Repository.Service
{
    public class PrintDeliveryNotesRepository  : IPrintDeliveryNotesRepository 
    {
        private IDapperContext _context;
        private ILog _log;
        
        public PrintDeliveryNotesRepository (IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<DeliveryNotesSales> GetDeliveryNotes(string jualProductId)
        {
            throw new NotImplementedException();
        }

        public IList<DeliveryNotesSales> GetSellingDeliveryNotes(string jualProductId)
        {
            IList<DeliveryNotesSales> oList = new List<DeliveryNotesSales>();

            try
            {
                var sql = @"SELECT m_customer.name_customer, m_customer.address, m_customer.postal_code, m_customer.contact, m_customer.phone, 
                            m_province2.name_province AS provinsi, m_regency2.name_regency AS regency, m_subdistrict.name_subdistrict AS subdistrict, 
                            t_delivery_notes.delivery, t_delivery_notes.delivery_date, t_delivery_notes.description, t_delivery_notes.tax, t_delivery_notes.courier, t_delivery_notes.shipping_cost, t_delivery_notes.discount AS diskon_nota, t_delivery_notes.total_invoice,
                            t_delivery_notes.is_dropship, t_delivery_notes.shipping_to, t_delivery_notes.shipping_address, t_delivery_notes.shipping_subdistrict, t_delivery_notes.shipping_country, t_delivery_notes.shipping_regency, t_delivery_notes.shipping_village, t_delivery_notes.shipping_city, t_delivery_notes.shipping_postal_code, t_delivery_notes.shipping_phone,
                            t_delivery_notes.from_label1, t_delivery_notes.from_label2, t_delivery_notes.from_label3, t_delivery_notes.from_label4,
                            t_delivery_notes.to_label1, t_delivery_notes.to_label2, t_delivery_notes.to_label3, t_delivery_notes.to_label4,
                            m_product.product_code, m_product.product_name, m_product.unit,
                            COALESCE(t_delivery_items.description, t_delivery_items.description, '') AS keterangan_item, t_delivery_items.selling_price AS price, t_delivery_items.quantity, t_delivery_items.return_quantity, t_delivery_items.discount
                            FROM public.t_delivery_notes INNER JOIN public.t_delivery_items ON t_delivery_items.delivery_id = t_delivery_notes.delivery_id
                            INNER JOIN public.m_product ON t_delivery_items.product_id = m_product.product_id
                            INNER JOIN public.m_customer ON t_delivery_notes.customer_id = m_customer.customer_id
                            LEFT JOIN public.m_province2 ON m_customer.province_id = m_province2.province_id
                            LEFT JOIN public.m_regency2 ON m_customer.regency_id = m_regency2.regency_id
                            LEFT JOIN public.m_subdistrict ON m_customer.subdistrict_id = m_subdistrict.subdistrict_id
                            WHERE t_delivery_notes.delivery_id = @jualProductId
                            ORDER BY t_delivery_items.system_date";

                oList = _context.db.Query<DeliveryNotesSales>(sql, new { jualProductId }).ToList();

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
