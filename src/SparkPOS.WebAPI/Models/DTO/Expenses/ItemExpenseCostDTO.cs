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
using SparkPOS.Model;

namespace SparkPOS.WebAPI.Models.DTO
{        
    public class ItemExpenseCostDTO
    {
		[Display(Name = "expense_item_id")]		
		public string expense_item_id { get; set; }
		
		[Display(Name = "expense_id")]
		public string expense_id { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

		[Display(Name = "quantity")]
		public double quantity { get; set; }
		
		[Display(Name = "price")]
		public double price { get; set; }
		
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
		
		[Display(Name = "expense_type_id")]
		public string expense_type_id { get; set; }

        public TypeExpenseDTO TypeExpense { get; set; }

        public EntityState entity_state { get; set; }
	}

    public class ItemExpenseCostDTOValidator : AbstractValidator<ItemExpenseCostDTO>
    {
        public ItemExpenseCostDTOValidator()
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
                RuleFor(c => c.expense_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.expense_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
		}

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.expense_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.expense_type_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);			
            RuleFor(c => c.quantity).NotEmpty().WithMessage(msgError1);
            RuleFor(c => c.price).NotEmpty().WithMessage(msgError1);            
        }
	}
}
