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

namespace SparkPOS.Model
{    
	[Table("m_product")]
    public class Product
    {
        public Product()
        {
            list_of_harga_grosir = new List<PriceWholesale>();
        }

		[ExplicitKey]
		[Display(Name = "product_id")]		
		public string product_id { get; set; }
		
		[Display(Name = "Name Product")]
		public string product_name { get; set; }
		
		[Display(Name = "unit")]
		public string unit { get; set; }
		
		[Display(Name = "Stock")]
		public double stock { get; set; }
		
		[Display(Name = "Buying Price")]
		public double purchase_price { get; set; }
		
		[Display(Name = "price Selling")]
		public double selling_price { get; set; }

        [Display(Name = "discount")]
        public double discount { get; set; }

        /// <summary>
        /// Untuk menentukan price sale Automatic based persentasi keuntungan
        /// </summary>
        [Display(Name = "Profit Percentage")]
        public double profit_percentage { get; set; }

		[Display(Name = "Code Product")]
		public string product_code { get; set; }

        [Write(false)]
        public string code_produk_old { get; set; }

        [Display(Name = "Category")]
		public string category_id { get; set; }

		[Write(false)]
        public Category Category { get; set; }

		[Display(Name = "Minimum Stock")]
		public double minimal_stock { get; set; }

		[Display(Name = "Stock Warehouse")]
		public double warehouse_stock { get; set; }

        [Display(Name = "Is Active")]
        public bool is_active { get; set; }

        [Computed]
        public bool is_stock_minus 
        {
            get { return (stock + warehouse_stock) <= 0; }
        }

        [Computed]
        public double remaining_stock
        {
            get { return (stock + warehouse_stock); }
        }

		[Display(Name = "Minimum Stock Warehouse")]
		public double minimal_stock_warehouse { get; set; }

        [Computed]
        public double asset
        {
            get { return (stock + warehouse_stock) > 0 ? (stock + warehouse_stock) * selling_price : 0; }
        }

        [Write(false)]
        public List<PriceWholesale> list_of_harga_grosir { get; set; }

        [Write(false)]
        public Nullable<DateTime> last_update { get; set; }
	}

    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.product_code).NotEmpty().WithMessage(msgError1).Length(1, 15).WithMessage(msgError2);
            RuleFor(c => c.product_name).NotEmpty().WithMessage(msgError1).Length(1, 300).WithMessage(msgError2);
			RuleFor(c => c.unit).Length(0, 20).WithMessage(msgError2);			
		}
	}
}
