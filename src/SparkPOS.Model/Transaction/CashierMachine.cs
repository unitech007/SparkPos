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
using Newtonsoft.Json;
using SparkPOS.Model.Report;

namespace SparkPOS.Model
{        
	[Table("t_machine")]
    public class CashierMachine
    {
		[ExplicitKey]
		[Display(Name = "machine_id")]		
		public string machine_id{ get; set; }
		
		[Display(Name = "user_id")]
		public string user_id { get; set; }

		[Write(false)]
        public User User { get; set; }

        [Write(false)]
		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "starting_balance")]
		public double starting_balance { get; set; }
		
		[Display(Name = "cash_in")]
		public double cash_in { get; set; }        
		
		[Display(Name = "shift_id")]
		public string shift_id { get; set; }
		
		[Display(Name = "cash_out")]
		public double cash_out { get; set; }

        [JsonIgnore]
        [Write(false)]
        [Display(Name = "system_date")]
        public Nullable<DateTime> system_date { get; set; }
	}

    public class MachineValidator : AbstractValidator<CashierMachine>
    {
        public MachineValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			//RuleFor(c => c.shift_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
		}
	}
}
