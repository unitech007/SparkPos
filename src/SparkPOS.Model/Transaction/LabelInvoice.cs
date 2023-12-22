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
	[Table("m_invoice_label")]
    public class LabelInvoice
    {
		[ExplicitKey]
		[Display(Name = "invoice_label_id")]		
		public string invoice_label_id { get; set; }
		
		[Display(Name = "Description Label")]
		public string description { get; set; }

        [Write(false)]
		[Display(Name = "order_number")]
		public int order_number { get; set; }

        [Write(false)]
		[Display(Name = "is_active")]
		public bool is_active { get; set; }
	}

    public class LabelInvoiceValidator : AbstractValidator<LabelInvoice>
    {
        public LabelInvoiceValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError);
		}
	}
}
