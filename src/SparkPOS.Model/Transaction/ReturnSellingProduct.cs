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
	[Table("t_sales_return")]
    public class ReturnSellingProduct
    {
        public ReturnSellingProduct()
        {
            item_return = new List<ItemReturnSellingProduct>();
            item_return_deleted = new List<ItemReturnSellingProduct>();
        }

		[ExplicitKey]
		[Display(Name = "return_sale_id")]		
		public string return_sale_id { get; set; }
		
		[Display(Name = "sale_id")]
		public string sale_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public SellingProduct SellingProduct { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public User User { get; set; }

		[Display(Name = "Customer")]
		public string customer_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public Customer Customer { get; set; }

		[Display(Name = "Invoice")]
		public string invoice { get; set; }
		
		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "Description")]
		public string description { get; set; }

        [JsonIgnore]
        [Write(false)]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }

        [JsonIgnore]
        [Computed]
		[Display(Name = "total_invoice")]
		public double total_invoice { get; set; }

        [Write(false)]
        public IList<ItemReturnSellingProduct> item_return { get; set; }

        [Write(false)]
        public IList<ItemReturnSellingProduct> item_return_deleted { get; set; }		
	}

    public class ReturnSellingProductValidator : AbstractValidator<ReturnSellingProduct>
    {
        public ReturnSellingProductValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.customer_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
			RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
		}
	}
}
