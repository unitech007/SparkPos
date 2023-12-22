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
	[Table("t_product_payable_payment")]
    public class DebtPaymentProduct
    {
        public DebtPaymentProduct()
        {
            item_payment_debt = new List<ItemDebtPaymentProduct>();
            item_payment_debt_deleted = new List<ItemDebtPaymentProduct>();
        }

		[ExplicitKey]
		[Display(Name = "pay_purchase_id")]		
		public string pay_purchase_id { get; set; }
		
		[Display(Name = "Supplier")]
		public string supplier_id { get; set; }

        //[JsonIgnore]
		[Write(false)]
        public Supplier Supplier { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public User User { get; set; }

		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "Description")]
		public string description { get; set; }

        [JsonIgnore]
        [Write(false)]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
		
		[Display(Name = "Invoice")]
		public string invoice { get; set; }
		
		[Display(Name = "is_cash")]
		public bool is_cash { get; set; }

        [Computed]
        public double total_payment { get; set; }

        [Write(false)]
        public List<ItemDebtPaymentProduct> item_payment_debt { get; set; }

        [Write(false)]
        public List<ItemDebtPaymentProduct> item_payment_debt_deleted { get; set; }
	}

    public class DebtPaymentProductValidator : AbstractValidator<DebtPaymentProduct>
    {
        public DebtPaymentProductValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.supplier_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
			RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
		}
	}
}
