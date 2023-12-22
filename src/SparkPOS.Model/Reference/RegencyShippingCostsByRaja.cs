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
	[Table("m_regency")]
    public class RegencyShippingCostsByRaja
    {
		[ExplicitKey]
		[Display(Name = "regency_id")]		
		public int regency_id { get; set; }
		
		[Display(Name = "province_id")]
		public int province_id { get; set; }

		[Write(false)]
        public ProvinsiRajaOngkir Provinsi { get; set; }

		[Display(Name = "type")]
		public string type { get; set; }
		
		[Display(Name = "name_regency")]
		public string name_regency { get; set; }
		
		[Display(Name = "postal_code")]
		public string postal_code { get; set; }
	}

    public class RegencyShippingCostsByRajaValidator : AbstractValidator<RegencyShippingCostsByRaja>
    {
        public RegencyShippingCostsByRajaValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.type).NotEmpty().WithMessage(msgError1).Length(1, 15).WithMessage(msgError2);
			RuleFor(c => c.name_regency).NotEmpty().WithMessage(msgError1).Length(1, 100).WithMessage(msgError2);
			RuleFor(c => c.postal_code).NotEmpty().WithMessage(msgError1).Length(0, 6).WithMessage(msgError2);
		}
	}
}
