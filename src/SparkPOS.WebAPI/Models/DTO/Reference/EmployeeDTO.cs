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
    public class EmployeeDTO
    {
		[Display(Name = "employee_id")]		
		public string employee_id { get; set; }
		
		[Display(Name = "job_titles_id")]
		public string job_titles_id { get; set; }

		[JsonIgnore]
        public TitlesDTO job_titles { get; set; }

		[Display(Name = "employee_name")]
		public string employee_name { get; set; }
		
		[Display(Name = "address")]
		public string address { get; set; }
		
		[Display(Name = "phone")]
		public string phone { get; set; }
		
		[Display(Name = "basic_salary")]
		public double basic_salary { get; set; }
		
		[Display(Name = "is_active")]
		public bool is_active { get; set; }
		        
		[Display(Name = "description")]
		public string description { get; set; }
		
		[Display(Name = "payment_type")]
		public int payment_type { get; set; }
		
		[Display(Name = "overtime_salary")]
		public double overtime_salary { get; set; }
		
		[Display(Name = "total_loan")]
		public double total_loan { get; set; }
		
		[Display(Name = "total_loan_payment")]
		public double total_loan_payment { get; set; }

        [Display(Name = "remaining_kasbon")]
        public double remaining_kasbon
        {
            get { return total_loan - total_loan_payment; }
        }
	}

    public class EmployeeDTOValidator : AbstractValidator<EmployeeDTO>
    {
        public EmployeeDTOValidator()
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
                RuleFor(c => c.employee_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.employee_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
		}

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.job_titles_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.employee_name).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
            RuleFor(c => c.address).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.phone).Length(0, 20).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);			
        }
	}
}
