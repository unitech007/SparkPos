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

namespace SparkPOS.Model
{        
	[Table("t_purchase_product")]
    public class PurchaseProduct
    {
        private Nullable<DateTime> _tanggal_creditTerm;

        public PurchaseProduct()
        {
            item_beli = new List<ItemPurchaseProduct>();
            item_beli_deleted = new List<ItemPurchaseProduct>();
        }

		[ExplicitKey]
		[Display(Name = "purchase_id")]		
		public string purchase_id { get; set; }
		
		[Display(Name = "user_id")]
		public string user_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public User User { get; set; }

		[Display(Name = "Supplier")]
		public string supplier_id { get; set; }

        //[JsonIgnore]
		[Write(false)]
        public Supplier Supplier { get; set; }

		[Display(Name = "purchase_return_id")]
		public string purchase_return_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public ReturnPurchaseProduct ReturnPurchaseProduct { get; set; }

		[Display(Name = "Invoice")]
		public string invoice { get; set; }
		
		[Display(Name = "date")]
		public Nullable<DateTime> date { get; set; }
		
		[Display(Name = "date CreditTerm")]
        public Nullable<DateTime> due_date
        {
            get { return _tanggal_creditTerm.IsNull() ? null : _tanggal_creditTerm; }
            set { _tanggal_creditTerm = value; }
        }

        [JsonIgnore]
        [Write(false)]
        public Nullable<DateTime> tanggal_creditTerm_old { get; set; }

		[Display(Name = "tax")]
		public double tax { get; set; }
		
		[Display(Name = "discount")]
		public double discount { get; set; }
		
        [Computed]
		[Display(Name = "total_invoice")]
		public double total_invoice { get; set; }

        [Computed]
		[Display(Name = "total_payment")]
		public double total_payment { get; set; }
		
		[Display(Name = "Description")]
		public string description { get; set; }

        [JsonIgnore]
        [Write(false)]
		[Display(Name = "system_date")]
		public Nullable<DateTime> system_date { get; set; }

        //[JsonIgnore]
        [Computed]
        public double total_repayment_old { get; set; }

        //[JsonIgnore]
        [Computed]
        public double grand_total
        {
            get { return total_invoice - discount + tax; }
        }

        //[JsonIgnore]
        [Computed]
        public double remaining_nota
        {
            get { return grand_total - total_payment; }
        }

        //[JsonIgnore]
        [Write(false)]
        public bool is_cash { get; set; }

        [Write(false)]
        public List<ItemPurchaseProduct> item_beli { get; set; }
        
        [Write(false)]
        public List<ItemPurchaseProduct> item_beli_deleted { get; set; }
	}

    public class PurchaseProductValidator : AbstractValidator<PurchaseProduct>
    {
        public PurchaseProductValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError1 = "'{PropertyName}' cannot be empty!";
            var msgError2 = "Input '{PropertyName}' maximum length is {MaxLength} characters!";

            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
			RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
		}
	}
}
