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
	[Table("t_sales_order_item")]
    public class ItemSellingProduct
    {
        public ItemSellingProduct()
        {
            entity_state = EntityState.Added;
        }

		[ExplicitKey]
		[Display(Name = "sale_item_id")]		
		public string sale_item_id { get; set; }
		
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

        [Display(Name = "tax_id")]
        public string tax_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public Tax Tax { get; set; }

        [Display(Name = "product_id")]
		public string product_id { get; set; }

		[Write(false)]
        public Product Product { get; set; }

        [Display(Name = "Description Optional")]
        public string description { get; set; }

		[Display(Name = "Buying Price")]
		public double purchase_price { get; set; }
		
		[Display(Name = "price Selling")]
		public double selling_price { get; set; }

        [Write(false)]
        [Display(Name = "Old quantity")]
        public double old_jumlah { get; set; }

		[Display(Name = "quantity")]
		public double quantity { get; set; }
		
		[Display(Name = "discount")]
		public double discount { get; set; }

        [JsonIgnore]
        [Write(false)]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
        
        [Write(false)]
		[Display(Name = "return_quantity")]
		public double return_quantity { get; set; }

        [Computed]
        public double sub_total_with_tax { get; set; }

        //[JsonIgnore]
        [Computed]
        public double diskon_rupiah
        {
            get { return discount / 100 * selling_price; }
        }

        //[JsonIgnore]
        [Computed]
        public double harga_setelah_diskon
        {
            get { return selling_price - diskon_rupiah; }
        }

        //[JsonIgnore]
        [Computed]
        public double sub_total
        {
            get { return (quantity - return_quantity) * harga_setelah_diskon; }
        }

        [Write(false)]
        public EntityState entity_state { get; set; }
	}

    public class ItemSellingProductValidator : AbstractValidator<ItemSellingProduct>
    {
        public ItemSellingProductValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
		}
	}
}
