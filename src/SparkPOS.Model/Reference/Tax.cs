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
	[Table("m_tax")]
    public class Tax
    {
		[ExplicitKey]
		[Display(Name = "tax_id")]		
		public string tax_id { get; set; }
		
		[Display(Name = "tax")]
		public string name_tax { get; set; }

        [Display(Name = "Tax(%)")]
        public int tax_percentage { get; set; }

        /// <summary>
        /// Untuk menentukan price sale Automatic based persentasi keuntungan
        /// </summary>
        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "is_default_tax")]
        public bool is_default_tax { get; set; }

        [Computed]
        public string CombinedDisplay
        {
            get { return $"{name_tax} - Tax: {tax_percentage}%"; }
        }
    }

   
    public class TaxValidator : AbstractValidator<Tax>
    {
        public TaxValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.name_tax).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
		}
	}

    //public string CombinedDisplay
    //{
    //    get { return $"{name_tax} ({tax_percentage}%)"; }
    //}

}
