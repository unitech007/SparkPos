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
	[Table("m_supplier")]
    public class Supplier
    {
		[ExplicitKey]
		[Display(Name = "supplier_id")]		
		public string supplier_id { get; set; }
		
		[Display(Name = "Supplier")]
		public string name_supplier { get; set; }
		
		[Display(Name = "Address")]
		public string address { get; set; }
		
		[Display(Name = "Contact")]
		public string contact { get; set; }

		[Display(Name = "CR No")]
		public string cr_no { get; set; }

		[Display(Name = "VAt No")]
		public string vat_no  { get; set; }

		[Display(Name = "phone")]
		public string phone { get; set; }

        [Computed]
		[Display(Name = "total_debt")]
		public double total_debt { get; set; }

        [Computed]
		[Display(Name = "total_debt_payment")]
		public double total_debt_payment { get; set; }
		
	}

    public class SupplierValidator : AbstractValidator<Supplier>
    {
        public SupplierValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.name_supplier).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
			RuleFor(c => c.address).Length(0, 100).WithMessage(msgError2);
			RuleFor(c => c.contact).Length(0, 50).WithMessage(msgError2);
			RuleFor(c => c.phone).Length(0, 20).WithMessage(msgError2);
		}
	}
}
