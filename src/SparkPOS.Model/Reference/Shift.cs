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
	[Table("m_shift")]
    public class Shift
    {
		[ExplicitKey]
		[Display(Name = "shift_id")]		
		public string shift_id { get; set; }
		
		[Display(Name = "name_shift")]
		public string name_shift { get; set; }
		
		[Display(Name = "start_time")]
		public Nullable<DateTime> start_time { get; set; }
		
		[Display(Name = "end_time")]
		public Nullable<DateTime> end_time { get; set; }
		
		[Display(Name = "is_active")]
		public bool is_active { get; set; }
		
	}

    public class ShiftValidator : AbstractValidator<Shift>
    {
        public ShiftValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.name_shift).NotEmpty().WithMessage(msgError1).Length(1, 100).WithMessage(msgError2);
		}
	}
}
