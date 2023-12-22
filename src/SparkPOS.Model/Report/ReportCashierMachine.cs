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
    public class ReportCashierMachine
    {
        public ReportCashierMachine()
        {
            item_jual = new List<ReportSalesProduct>();
        }

        public string machine_id{ get; set; }
        public Nullable<DateTime> date { get; set; }
        public double starting_balance { get; set; }
        public double cash_in { get; set; }

        public string user_id { get; set; }
        public User User { get; set; }

        public string shift_id { get; set; }
        public Shift Shift { get; set; }

        public double cash_out { get; set; }
        public Nullable<DateTime> system_date { get; set; }

        public ReportSalesProductHeader sale { get; set; }
        public IList<ReportSalesProduct> item_jual { get; set; }
    }
}
