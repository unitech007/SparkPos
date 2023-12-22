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
    public class ReportCardCredit
    {
        public string customer_id { get; set; }
        public string name_customer { get; set; }
        public DateTime date { get; set; }
        public string product_name { get; set; }
        public string unit { get; set; }
        public double quantity { get; set; }
        public double total { get; set; }
        public double starting_balance { get; set; }
        public double saldo { get; set; }
        public double saldo_akhir { get; set; }

        /// <summary>
        /// Type 1 untuk sales dan 2 untuk payment/pelunasan
        /// </summary>
        public int type { get; set; }
    }
}
