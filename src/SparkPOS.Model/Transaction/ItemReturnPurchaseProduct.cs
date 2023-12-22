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
	[Table("t_purchase_return_item")]
    public class ItemReturnPurchaseProduct
    {
        public ItemReturnPurchaseProduct()
        {
            entity_state = EntityState.Added;
        }

		[ExplicitKey]
		[Display(Name = "return_purchase_item_id")]		
		public string return_purchase_item_id { get; set; }
		
		[Display(Name = "purchase_return_id")]
		public string purchase_return_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public ReturnPurchaseProduct ReturnPurchaseProduct { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public User User { get; set; }

		[Display(Name = "Product")]
		public string product_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public Product Product { get; set; }

		[Display(Name = "price")]
		public double price { get; set; }
		
        /// <summary>
        /// quantity Purchase sebelum return
        /// </summary>
		[Display(Name = "quantity")]
		public double quantity { get; set; }

        [Display(Name = "quantity Return")]
        public double return_quantity { get; set; }

        [JsonIgnore]
        [Write(false)]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
					
		[Display(Name = "Item Purchase Id")]
		public string purchase_item_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public ItemPurchaseProduct ItemPurchaseProduct { get; set; }

        [Write(false)]
        public EntityState entity_state { get; set; }
	}

    public class ItemReturnPurchaseProductValidator : AbstractValidator<ItemReturnPurchaseProduct>
    {
        public ItemReturnPurchaseProductValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.purchase_return_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.purchase_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
		}
	}
}
