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
    public class ItemPurchaseProductDTO
    {
        [Display(Name = "purchase_item_id")]
        public string purchase_item_id { get; set; }

        [Display(Name = "purchase_id")]
        public string purchase_id { get; set; }

        [JsonIgnore]        
        public PurchaseProductDTO PurchaseProduct { get; set; }

        [Display(Name = "user_id")]
        public string user_id { get; set; }

        [JsonIgnore]
        public UserDTO User { get; set; }

        [Display(Name = "product_id")]
        public string product_id { get; set; }

        public ProductDTO Product { get; set; }

        [Display(Name = "price")]
        public double price { get; set; }

        [Display(Name = "quantity")]
        public double quantity { get; set; }

        [Display(Name = "discount")]
        public double discount { get; set; }

        [JsonIgnore]
        [Display(Name = "system_date")]
        public Nullable<DateTime> system_date { get; set; }

        [Display(Name = "return_quantity")]
        public double return_quantity { get; set; }

        public double diskon_rupiah
        {
            get { return discount / 100 * price; }
        }

        public double harga_setelah_diskon
        {
            get { return price - diskon_rupiah; }
        }

        public double sub_total
        {
            get { return (quantity - return_quantity) * harga_setelah_diskon; }
        }

        public EntityState entity_state { get; set; }
    }

    public class ItemPurchaseProductDTOValidator : AbstractValidator<ItemPurchaseProductDTO>
    {
        public ItemPurchaseProductDTOValidator()
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
                RuleFor(c => c.purchase_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.purchase_item_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            });
        }

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.purchase_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
        }
    }
}