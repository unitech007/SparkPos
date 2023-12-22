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
    public class ProductDTO
    {
        public ProductDTO()
        {
            list_of_harga_grosir = new List<PriceWholesaleDTO>();
        }

		[Display(Name = "product_id")]		
		public string product_id { get; set; }
		
		[Display(Name = "product_name")]
		public string product_name { get; set; }
		
		[Display(Name = "unit")]
		public string unit { get; set; }
		
		[Display(Name = "stock")]
		public double stock { get; set; }
		
		[Display(Name = "purchase_price")]
		public double purchase_price { get; set; }
		
		[Display(Name = "selling_price")]
		public double selling_price { get; set; }
		
		[Display(Name = "product_code")]
		public string product_code { get; set; }

        [Display(Name = "code_produk_old")]
        public string code_produk_old { get; set; }

		[Display(Name = "category_id")]
		public string category_id { get; set; }

        [JsonIgnore]
        public CategoryDTO Category { get; set; }

		[Display(Name = "minimal_stock")]
		public double minimal_stock { get; set; }
		
		[Display(Name = "warehouse_stock")]
		public double warehouse_stock { get; set; }
		
		[Display(Name = "minimal_stock_warehouse")]
		public double minimal_stock_warehouse { get; set; }
		
		[Display(Name = "discount")]
		public double discount { get; set; }

        public IList<PriceWholesaleDTO> list_of_harga_grosir { get; set; }
	}

    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
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
                RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);			
            });
		}

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.product_name).NotEmpty().WithMessage(msgError1).Length(1, 300).WithMessage(msgError2);
            RuleFor(c => c.unit).Length(0, 20).WithMessage(msgError2);
            RuleFor(c => c.product_code).NotEmpty().WithMessage(msgError1).Length(1, 15).WithMessage(msgError2);
            RuleFor(c => c.category_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);			
        }
	}
}
