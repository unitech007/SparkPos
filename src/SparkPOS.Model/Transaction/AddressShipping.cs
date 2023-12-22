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
using System.ComponentModel.DataAnnotations;

namespace SparkPOS.Model
{
    public class AddressShipping
    {
        /// <summary>
        /// Property untuk menyimpan Information apakah The shipping address is the same as the customer's address
        /// </summary>
        public bool is_sdac { get; set; }

        [Display(Name = "to")]
        public string to { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "country")]
        public string country { get; set; }

        [Display(Name = "subdistrict")]
        public string subdistrict { get; set; }

        [Display(Name = "village")]
        public string village { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "Provinsi")]
        public string Provinsi { get; set; }

        [Display(Name = "Regency")]
        public string regency { get; set; }

        [Display(Name = "Code Pos")]
        public string postal_code { get; set; }

        [Display(Name = "phone")]
        public string phone { get; set; }
    }

    public class AddressShippingValidator : AbstractValidator<AddressShipping>
    {
        public AddressShippingValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.to).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
            RuleFor(c => c.address).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.country).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.regency).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.subdistrict).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.village).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.city).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.postal_code).Length(0, 6).WithMessage(msgError2);
            RuleFor(c => c.phone).Length(0, 20).WithMessage(msgError2);
        }
    }
}
