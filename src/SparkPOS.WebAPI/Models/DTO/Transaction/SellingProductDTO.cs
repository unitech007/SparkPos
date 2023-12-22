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
    public class SellingProductDTO
    {
        private Nullable<DateTime> _tanggal_creditTerm;

        public SellingProductDTO()
        {
            is_sdac = true;
            item_jual = new List<ItemSellingProductDTO>();
            item_jual_deleted = new List<ItemSellingProductDTO>();
        }

		[Display(Name = "sale_id")]		
		public string sale_id { get; set; }
		
		[Display(Name = "user_id")]
		public string user_id { get; set; }

		[Display(Name = "Customer")]
		public string customer_id { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; }

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
        public Nullable<DateTime> tanggal_creditTerm_old { get; set; }

		[Display(Name = "tax")]
		public double tax { get; set; }
		
		[Display(Name = "discount")]
		public double discount { get; set; }

        [Display(Name = "courier")]
        public string courier { get; set; }

        [Display(Name = "Cost Shipping")]
        public double shipping_cost { get; set; }

		[Display(Name = "total_invoice")]
		public double total_invoice { get; set; }

		[Display(Name = "total_payment")]
		public double total_payment { get; set; }
		
		[Display(Name = "description")]
		public string description { get; set; }

        /// <summary>
        /// Property untuk menyimpan Information apakah The shipping address is the same as the customer's address
        /// </summary>
        public bool is_sdac { get; set; }

        public bool is_dropship { get; set; }

        [Display(Name = "to")]
        public string shipping_to { get; set; }

        [Display(Name = "Address")]
        public string shipping_address { get; set; }

        [Display(Name = "country")]
        public string shipping_country { get; set; }        

        [Display(Name = "village")]
        public string shipping_village { get; set; }

        [Display(Name = "subdistrict")]
        public string shipping_subdistrict { get; set; }

        [Display(Name = "City")]
        public string shipping_city { get; set; }

        [Display(Name = "Regency")]
        public string shipping_regency { get; set; }

        [Display(Name = "Code Pos")]
        public string shipping_postal_code { get; set; }

        [Display(Name = "phone")]
        public string shipping_phone { get; set; }

        [Display(Name = "Label dari #1")]
        public string from_label1 { get; set; }

        [Display(Name = "Label dari #2")]
        public string from_label2 { get; set; }

        [Display(Name = "Label dari #3")]
        public string from_label3 { get; set; }

        [Display(Name = "Label dari #4")]
        public string from_label4 { get; set; }

        [Display(Name = "Label to #1")]
        public string to_label1 { get; set; }

        [Display(Name = "Label to #2")]
        public string to_label2 { get; set; }

        [Display(Name = "Label to #3")]
        public string to_label3 { get; set; }

        [Display(Name = "Label to #4")]
        public string to_label4 { get; set; }

        [JsonIgnore]
        [Display(Name = "Pay Cash")]
        public double payment_cash { get; set; }

        [Display(Name = "Pay Card")]
        public double payment_card { get; set; }

        public double jumlah_pay
        {
            get { return payment_cash + payment_card; }
        }

        [JsonIgnore]
        [Display(Name = "system_date")]
        public Nullable<DateTime> system_date { get; set; }
		
		[Display(Name = "return_sale_id")]
		public string return_sale_id { get; set; }

        [JsonIgnore]
        public ReturnSellingProductDTO ReturnSellingProduct { get; set; }

		[Display(Name = "shift_id")]
		public string shift_id { get; set; }

        [JsonIgnore]
        public Shift Shift { get; set; }

        [Display(Name = "machine_id")]
        public string machine_id{ get; set; }

        [JsonIgnore]
        public CashierMachine Machine { get; set; }

        [Display(Name = "card_id")]
        public string card_id { get; set; }

        [JsonIgnore]
        public CardDTO Card { get; set; }

        [Display(Name = "dropshipper_id")]
        public string dropshipper_id { get; set; }

        [JsonIgnore]
        public DropshipperDTO Dropshipper { get; set; }

        [Display(Name = "Nomor Card")]
        public string card_number { get; set; }

        public bool is_cash { get; set; }

        public double total_repayment_old { get; set; }        

        /// <summary>
        /// total invoice setelah dilessi discount kemudian ditambah tax
        /// </summary>
        public double grand_total
        {
            get { return total_invoice - discount + shipping_cost + tax; }
        }

        public double remaining_nota
        {
            get { return grand_total - total_payment; }
        }

        public IList<ItemSellingProductDTO> item_jual { get; set; }

        public IList<ItemSellingProductDTO> item_jual_deleted { get; set; }
    }

    public class SellingProductDTOValidator : AbstractValidator<SellingProductDTO>
    {
        public SellingProductDTOValidator()
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
                RuleFor(c => c.sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.sale_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
        }

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.invoice).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.courier).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.card_number).Length(0, 20).WithMessage(msgError2);

            RuleFor(c => c.from_label1).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.from_label2).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.from_label3).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.from_label4).Length(0, 100).WithMessage(msgError2);

            RuleFor(c => c.to_label1).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.to_label2).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.to_label3).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.to_label4).Length(0, 250).WithMessage(msgError2);
        }
    }
}