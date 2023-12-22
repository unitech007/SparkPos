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

namespace SparkPOS.Model.Report
{
    public class ReportSalesProduct
    {
        public string customer_id { get; set; }

        public string category_id { get; set; }
        public string name_category { get; set; }

        public DateTime date { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string description { get; set; }
        public string unit { get; set; }
        public double quantity { get; set; }
        public double discount { get; set; }
        public double return_quantity { get; set; }
        public double purchase_price { get; set; }
        public double selling_price { get; set; }

        public double diskon_rupiah_selling_price
        {
            get { return discount / 100 * selling_price; }
        }

        public double selling_price_setelah_diskon
        {
            get { return selling_price - diskon_rupiah_selling_price; }
        }

        public double sub_total
        {
            get { return (quantity - return_quantity) * selling_price_setelah_diskon; }
        }

        public double laba
        {
            get { return (quantity - return_quantity) * (selling_price_setelah_diskon - purchase_price); }
        }
    }
}
