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

using Newtonsoft.Json;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace SparkPOS.WebAPI.Models.DTO
{        
    public class SalaryEmployeeDTO
    {
		[Display(Name = "gaji_employee_id")]		
		public string gaji_employee_id { get; set; }
		
		[Display(Name = "employee_id")]
		public string employee_id { get; set; }

		[JsonIgnore]
        public EmployeeDTO Employee { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

		[JsonIgnore]
        public UserDTO User { get; set; }

		[Display(Name = "month")]
		public int month { get; set; }
		
		[Display(Name = "year")]
		public int year { get; set; }
		
		[Display(Name = "attendance")]
		public int attendance { get; set; }
		
		[Display(Name = "absence")]
		public int absence { get; set; }
		
		[Display(Name = "basic_salary")]
		public double basic_salary { get; set; }
		
		[Display(Name = "overtime")]
		public double overtime { get; set; }
		
		[Display(Name = "bonus")]
		public double bonus { get; set; }
		
		[Display(Name = "deductions")]
		public double deductions { get; set; }
		
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
		
		[Display(Name = "time")]
		public int time { get; set; }
		
		[Display(Name = "other")]
		public double other { get; set; }
		
		[Display(Name = "description")]
		public string description { get; set; }
		
		[Display(Name = "days_worked")]
		public int days_worked { get; set; }
		
		[Display(Name = "allowance")]
		public double allowance { get; set; }
		
		[Display(Name = "loan")]
		public double loan { get; set; }
		
		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "invoice")]
		public string invoice { get; set; }
		
	}

    public class SalaryEmployeeDTOValidator : AbstractValidator<SalaryEmployeeDTO>
    {
        public SalaryEmployeeDTOValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "'{PropertyName}' maksimal {MaxLength} karakter !";			

			RuleSet("save", () =>
            {
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("update", () =>
            {
                RuleFor(c => c.gaji_employee_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.gaji_employee_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
		}

        private void DefaultRule(string msgError1, string msgError2)
        {            
            RuleFor(c => c.employee_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);            
            RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
        }
	}
}
