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
    public class ItemPaymentCreditProductDTO
    {
		[Display(Name = "pay_sale_item_id")]		
		public string pay_sale_item_id { get; set; }
		
		[Display(Name = "pay_sale_id")]
		public string pay_sale_id { get; set; }

        [JsonIgnore]
        public PaymentCreditProductDTO PaymentCreditProduct { get; set; }

		[Display(Name = "sale_id")]
		public string sale_id { get; set; }

        //[JsonIgnore]
        public SellingProductDTO SellingProduct { get; set; }

		[Display(Name = "amount")]
		public double amount { get; set; }
		
		[Display(Name = "description")]
		public string description { get; set; }

        [JsonIgnore]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }

        public EntityState entity_state { get; set; }
    }

    public class ItemPaymentCreditProductDTOValidator : AbstractValidator<ItemPaymentCreditProductDTO>
    {
        public ItemPaymentCreditProductDTOValidator()
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
                RuleFor(c => c.pay_sale_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.pay_sale_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
        }

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.pay_sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
        }
    }
}