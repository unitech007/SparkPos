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
	[Table("m_alasan_penyesuaian_stock")]
    public class ReasonAdjustmentStock
    {
        [ExplicitKey]
		[Display(Name = "stock_adjustment_reason_id")]		
		public string stock_adjustment_reason_id { get; set; }
		
		[Display(Name = "reason")]
		public string reason { get; set; }
	}

    public class ReasonAdjustmentStockValidator : AbstractValidator<ReasonAdjustmentStock>
    {
        public ReasonAdjustmentStockValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.reason).NotEmpty().WithMessage(msgError1).Length(1, 100).WithMessage(msgError2);
		}
	}
}
