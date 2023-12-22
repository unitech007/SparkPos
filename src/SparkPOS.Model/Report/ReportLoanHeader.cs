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
    public class ReportLoanHeader
    {
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public DateTime date { get; set; }
        public string invoice { get; set; }
        public double amount { get; set; }
        public double total_payment { get; set; }
        
        public double remaining
        {
            get { return amount - total_payment; }
        }

        public string description { get; set; }
    }
}
