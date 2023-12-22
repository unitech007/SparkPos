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
    public class PriceWholesaleDTO
    {
		[Display(Name = "wholesale_price_id")]		
		public string wholesale_price_id { get; set; }
		
		[Display(Name = "product_id")]
		public string product_id { get; set; }

		[JsonIgnore]
        public ProductDTO Product { get; set; }

		[Display(Name = "retail_price")]
		public int retail_price { get; set; }
		
		[Display(Name = "wholesale_price")]
		public double wholesale_price { get; set; }
		
		[Display(Name = "minimum_quantity")]
		public double minimum_quantity { get; set; }
		
		[Display(Name = "discount")]
		public double discount { get; set; }
		
	}

    public class PriceWholesaleDTOValidator : AbstractValidator<PriceWholesaleDTO>
    {
        public PriceWholesaleDTOValidator()
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
                RuleFor(c => c.wholesale_price_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.wholesale_price_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);			
            });
		}

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.retail_price).NotEmpty().WithMessage(msgError1);
            RuleFor(c => c.wholesale_price).NotEmpty().WithMessage(msgError1);
            RuleFor(c => c.minimum_quantity).NotEmpty().WithMessage(msgError1);
        }
	}
}
