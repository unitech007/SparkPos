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
    public class PaymentCreditProductDTO
    {
        public PaymentCreditProductDTO()
        {
            item_payment_credit = new List<ItemPaymentCreditProductDTO>();
            item_payment_credit_deleted = new List<ItemPaymentCreditProductDTO>();
        }

		[Display(Name = "pay_sale_id")]		
		public string pay_sale_id { get; set; }
		
		[Display(Name = "Customer")]
		public string customer_id { get; set; }

        //[JsonIgnore]
        public CustomerDTO Customer { get; set; }

		[Display(Name = "user_id")]
		public string user_id { get; set; }

        [JsonIgnore]
        public UserDTO User { get; set; }

		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "Description")]
		public string description { get; set; }

        [JsonIgnore]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }
		
		[Display(Name = "Invoice")]
		public string invoice { get; set; }
		
		[Display(Name = "is_cash")]
		public bool is_cash { get; set; }

        public double total_payment { get; set; }

        public IList<ItemPaymentCreditProductDTO> item_payment_credit { get; set; }
        public IList<ItemPaymentCreditProductDTO> item_payment_credit_deleted { get; set; }
    }

    public class PaymentCreditProductDTOValidator : AbstractValidator<PaymentCreditProductDTO>
    {
        public PaymentCreditProductDTOValidator()
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
                RuleFor(c => c.pay_sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.pay_sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
        }

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.customer_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
        }
    }
}