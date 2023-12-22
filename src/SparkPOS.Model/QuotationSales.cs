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

namespace SparkPOS.Model.quotation
{
    public class QuotationSales
    {
        public string name_customer { get; set; }
        public string address { get; set; }
        public string provinsi { get; set; }        
        public string regency { get; set; }
        public string subdistrict { get; set; }        
        public string postal_code { get; set; }

        public string contact { get; set; }
        public string phone { get; set; }

        public Byte[] QrCodeImage { get; set; }
        /// <summary>
        /// Property untuk menyimpan Information apakah The shipping address is the same as the customer's address
        /// </summary>
        public bool is_sdac { get; set; }

        public bool is_dropship { get; set; }

        public string shipping_to { get; set; }
        public string shipping_address { get; set; }
        public string shipping_country { get; set; }
        public string shipping_village { get; set; }
        public string shipping_subdistrict { get; set; }        
        public string shipping_city { get; set; }
        public string shipping_regency { get; set; }
        public string shipping_postal_code { get; set; }
        public string shipping_phone { get; set; }

        public string from_label1 { get; set; }
        public string from_label2 { get; set; }
        public string from_label3 { get; set; }
        public string from_label4 { get; set; }
        public string to_label1 { get; set; }
        public string to_label2 { get; set; }
        public string to_label3 { get; set; }
        public string to_label4 { get; set; }
        //28
        public string quotation { get; set; }
        public DateTime date { get; set; }
      //  public DateTime due_date { get; set; }
        public string description { get; set; }
        public double tax { get; set; }
        public double diskon_nota { get; set; }
        public string courier { get; set; }
        public double shipping_cost { get; set; }
        public string label_cost_shipping { get; set; }
        public double total_quotation { get; set; }        

        public string product_code { get; set; }
        public string product_name { get; set; }
        public string unit { get; set; }
        public string keterangan_item { get; set; }
        public double price { get; set; }
        public double quantity { get; set; }
        public double return_quantity { get; set; }
        public double discount { get; set; }

        public double diskon_rupiah
        {
            get { return discount / 100 * price; }
        }

        public double harga_setelah_diskon
        {
            get { return price - diskon_rupiah; }
        }

        public double sub_total
        {
            get { return (quantity - return_quantity) * harga_setelah_diskon; }
        }
    }
}
