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

namespace SparkPOS.Model.Invoice
{
    public class InvoicePurchase
    {
        public string name_supplier { get; set; }
        public string address { get; set; }
        public string contact { get; set; }
        public string phone { get; set; }

        public string invoice { get; set; }
        public string cr_no { get; set; }
        public string vat_no { get; set; }
        public DateTime date { get; set; }
        public DateTime due_date { get; set; }
        public double tax { get; set; }
        public double diskon_nota { get; set; }
        public double total_invoice { get; set; }

        public string product_code { get; set; }
        public string product_name { get; set; }
        public string unit { get; set; }
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
