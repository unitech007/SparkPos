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
    public class ReportCardStock
    {
        /// <summary>
        /// 1 = Purchase, 2 = Return Sales, 3 = Sales, 4 = Return Purchase
        /// </summary>
        public int type_nota { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string invoice { get; set; }
        public DateTime date { get; set; }
        public string supplier_or_customer { get; set; }
        public double qty { get; set; }

        public double masuk
        {
            get 
            {
                return type_nota == 1 || type_nota == 2 ? qty : 0; 
            }
        }


        public double Exit
        {
            get 
            {
                return type_nota == 3 || type_nota == 4 ? qty : 0; 
            }
        }

        public double stock_awal { get; set; }
        public double stock_akhir { get; set; }

        public double starting_balance { get; set; }
        public double saldo { get; set; }
        public double saldo_akhir { get; set; }

        public string description { get; set; }
    }
}
