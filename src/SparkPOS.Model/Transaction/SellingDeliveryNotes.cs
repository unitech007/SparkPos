
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
 * License for the specific language governing  and limitations under
 * the License.permissions
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
using SparkPOS.Model.Transaction;

namespace SparkPOS.Model
{
    [Table("t_delivery_notes")]
    public class SellingDeliveryNotes
    {
        private Nullable<DateTime> _tanggal_creditTerm;

        public SellingDeliveryNotes()
        {
           // is_sdac = true;
            item_jual = new List<ItemSellingDeliveryNotes>();
            item_jual_deleted = new List<ItemSellingDeliveryNotes>();
        }

        [ExplicitKey]
        [Display(Name = "delivery_id")]
        public string delivery_id { get; set; }

        [Display(Name = "user_id")]
        public string user_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public User User { get; set; }

        [Display(Name = "Customer")]
        public string customer_id { get; set; }

        //[JsonIgnore]
        [Write(false)]
        public Customer Customer { get; set; }

        //[Display(Name = "Quotation")]
        //public string quotation_id { get; set; }


        [Display(Name = "delivery")]
        public string delivery { get; set; }

        [Display(Name = "delivery_date")]
        public Nullable<DateTime> delivery_date { get; set; }

        //[JsonIgnore]
        [Write(false)]
        public ItemSellingDeliveryNotes ItemSellingDeliveryNotes { get; set; }

        [Display(Name = "invoice")]
        public string sale_id { get; set; }

        //[Display(Name = "Invoice")]
        //public string invoice { get; set; }

        //[Display(Name = "date")]
        //public Nullable<DateTime> date { get; set; }

        //[Display(Name = "date CreditTerm")]
        //public Nullable<DateTime> due_date
        //{
        //    get { return _tanggal_creditTerm.IsNull() ? null : _tanggal_creditTerm; }
        //    set { _tanggal_creditTerm = value; }
        //}

        [JsonIgnore]
        [Write(false)]
        public Nullable<DateTime> tanggal_creditTerm_old { get; set; }

        [Display(Name = "tax")]
        public double tax { get; set; }

        [Display(Name = "discount")]
        public double discount { get; set; }

        [Display(Name = "courier")]
        public string courier { get; set; }

        [Display(Name = "Cost Shipping")]
        public double shipping_cost { get; set; }

        [Computed]
        [Display(Name = "total_invoice")]
        public double total_invoice { get; set; }

        [Computed]
        [Display(Name = "total_payment")]
        public double total_payment { get; set; }

        [Display(Name = "description")]
        public string description { get; set; }

        /// <summary>
        /// Property untuk menyimpan Information apakah The shipping address is the same as the customer's address
        /// </summary>
        //public bool is_sdac { get; set; }

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

        [JsonIgnore]
        [Display(Name = "Pay Card")]
        public double payment_card { get; set; }

        //[JsonIgnore]
        [Computed]
        public double jumlah_pay
        {
            get { return payment_cash + payment_card; }
        }

        [JsonIgnore]
        [Write(false)]
        [Display(Name = "system_date")]
        public Nullable<DateTime> system_date { get; set; }

        //[Display(Name = "return_sale_id")]
        //public string return_sale_id { get; set; }

        //[JsonIgnore]
        //[Write(false)]
        //public ReturnSellingDeliveryNotes ReturnSellingDeliveryNotes { get; set; }

        //[Display(Name = "shift_id")]
        //public string shift_id { get; set; }

        //[JsonIgnore]
        //[Write(false)]
        //public Shift Shift { get; set; }

        [Display(Name = "machine_id")]
        public string machine_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public CashierMachine Machine { get; set; }

        [Display(Name = "card_id")]
        public string card_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public Card Card { get; set; }

        [Display(Name = "dropshipper_id")]
        public string dropshipper_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public Dropshipper Dropshipper { get; set; }

        [Display(Name = "Nomor Card")]
        public string card_number { get; set; }

        [Write(false)]
        public bool is_cash { get; set; }

        //[JsonIgnore]
        [Computed]
        public double total_repayment_old { get; set; }

        /// <summary>
        /// total invoice setelah dilessi discount kemudian ditambah tax
        /// </summary>
        //[JsonIgnore]
        [Computed]
        public double grand_total
        {
            get { return total_invoice - discount + shipping_cost + tax; }
        }

        //[JsonIgnore]
        [Computed]
        public double remaining_nota
        {
            get { return grand_total - total_payment; }
        }

        [Write(false)]
        public List<ItemSellingDeliveryNotes> item_jual { get; set; }

        [Write(false)]
        public List<ItemSellingDeliveryNotes> item_jual_deleted { get; set; }
    }

    public class SellingDeliveryNotesValidator : AbstractValidator<SellingDeliveryNotes>
    {
        public SellingDeliveryNotesValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.delivery).NotEmpty().WithMessage(msgError1).Length(1, 20).WithMessage(msgError2);
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
