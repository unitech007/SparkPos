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
    public class ReportSalaryEmployee
    {
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string name_job_titles { get; set; }
        public TypePayment payment_type { get; set; }

        public DateTime date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int attendance { get; set; }
        public int absence { get; set; }
        public double basic_salary { get; set; }
        public double overtime { get; set; }
        public double bonus { get; set; }
        public double deductions { get; set; }
        public int time { get; set; }
        public int days_worked { get; set; }
        public double allowance { get; set; }
        
        public double gaji_akhir
        {
            get
            {
                double result = payment_type == TypePayment.Weekly ? days_worked * basic_salary : basic_salary;

                return result;
            }
        }

        public double lembur_akhir
        {
            get
            {
                double result = payment_type == TypePayment.Weekly ? time * overtime : overtime;

                return result;
            }
        }

        public double total_gaji
        {
            get
            {
                return gaji_akhir + allowance + lembur_akhir + bonus - deductions;
            }
        }
    }
}
