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
	[Table("t_stock_adjustment")]
    public class AdjustmentStock
    {
		[ExplicitKey]
		[Display(Name = "stock_adjustment_id")]		
		public string stock_adjustment_id { get; set; }
		
		[Display(Name = "Product")]
		public string product_id { get; set; }

		[Write(false)]
        public Product Product { get; set; }

		[Display(Name = "reason Adjustment")]
		public string adjustment_reason_id { get; set; }

		[Write(false)]
        public ReasonAdjustmentStock ReasonAdjustmentStock { get; set; }

		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "stock_addition")]
		public double stock_addition { get; set; }
		
		[Display(Name = "stock_reduction")]
		public double stock_reduction { get; set; }
		
		[Display(Name = "description")]
		public string description { get; set; }

        [Write(false)]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
		
		[Display(Name = "warehouse_stock_addition")]
		public double warehouse_stock_addition { get; set; }
		
		[Display(Name = "warehouse_stock_reduction")]
		public double warehouse_stock_reduction { get; set; }
		
	}

    public class AdjustmentStockValidator : AbstractValidator<AdjustmentStock>
    {
        public AdjustmentStockValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.Product.product_code).NotEmpty().WithMessage(msgError1).Length(1, 15).WithMessage(msgError2);
			RuleFor(c => c.adjustment_reason_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
		}
	}
}
