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
using System.Threading.Tasks;

using FluentValidation;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SparkPOS.Model
{        
	[Table("t_employee_salary")]
    public class SalaryEmployee
    {
        public SalaryEmployee()
        {
            item_payment_loan = new List<PaymentLoan>();
        }

		[ExplicitKey]
		[Display(Name = "gaji_employee_id")]		
		public string gaji_employee_id { get; set; }
		
		[Display(Name = "Employee")]
		public string employee_id { get; set; }

		[Write(false)]
        public Employee Employee { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public User User { get; set; }

        [Display(Name = "Invoice")]
        public string invoice { get; set; }

        [Display(Name = "date")]
        public Nullable<DateTime> date { get; set; }
      
		[Display(Name = "month")]
		public int month { get; set; }
		
		[Display(Name = "year")]
		public int year { get; set; }
		
		[Display(Name = "attendance")]
		public int attendance { get; set; }
		
		[Display(Name = "absence")]
		public int absence { get; set; }
		
		[Display(Name = "Salary Pokok")]
		public double basic_salary { get; set; }
		
		[Display(Name = "overtime")]
		public double overtime { get; set; }
		
		[Display(Name = "Bonus")]
		public double bonus { get; set; }
		
		[Display(Name = "deductions")]
		public double deductions { get; set; }        
		
		[Display(Name = "time")]
		public int time { get; set; }
		
		[Display(Name = "other")]
		public double other { get; set; }
		
		[Display(Name = "Description")]
		public string description { get; set; }
		
		[Display(Name = "quantity Day")]
		public int days_worked { get; set; }
		
		[Display(Name = "allowance")]
		public double allowance { get; set; }
		
        [Computed]
        public double gaji_akhir
        {
            get
            {
                double result = 0;

                if (Employee != null)
                {
                    result = Employee.payment_type == TypePayment.Weekly ? days_worked * basic_salary : basic_salary;
                }

                return result;
            }
        }

        [Computed]
        public double lembur_akhir
        {
            get
            {
                double result = 0;

                if (Employee != null)
                {
                    result = Employee.payment_type == TypePayment.Weekly ? time * overtime : overtime;
                }

                return result;
            }
        }

        [Computed]
        public double total_gaji
        {
            get
            {
                return gaji_akhir + allowance + lembur_akhir + bonus - deductions;
            }
        }

        [Write(false)]
        public List<PaymentLoan> item_payment_loan { get; set; }

        [JsonIgnore]
        [Write(false)]
        [Display(Name = "system_date")]
        public Nullable<DateTime> system_date { get; set; }
	}

    public class SalaryEmployeeValidator : AbstractValidator<SalaryEmployee>
    {
        public SalaryEmployeeValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.employee_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);			
			RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.basic_salary).GreaterThan(0).WithMessage(msgError1).When(c => c.Employee.payment_type == TypePayment.Monthly);
            RuleFor(c => c.days_worked).GreaterThan(0).WithMessage(msgError1).When(c => c.Employee.payment_type == TypePayment.Weekly);
		}
	}
}
