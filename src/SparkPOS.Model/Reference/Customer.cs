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
	[Table("m_customer")]
    public class Customer
    {
		[ExplicitKey]
		[Display(Name = "customer_id")]		
		public string customer_id { get; set; }
		
		[Display(Name = "Customer")]
		public string name_customer { get; set; }

        public string province_id { get; set; }

        [Write(false)]
        public Provinsi Provinsi { get; set; }

        public string regency_id { get; set; }

        [Write(false)]
        public string regency_old { get; set; }

        [Write(false)]
        public Regency Regency { get; set; }

        public string subdistrict_id { get; set; }

        [Write(false)]
        public string subdistrict_old { get; set; }

        [Write(false)]
        public subdistrict subdistrict { get; set; }

		[Display(Name = "Address")]
		public string address { get; set; }

        [Display(Name = "country")]
        public string country { get; set; }

        [Display(Name = "village")]
        public string village { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "Code Pos")]
        public string postal_code { get; set; }

		[Display(Name = "Contact")]
		public string contact { get; set; }
		
		[Display(Name = "phone")]
		public string phone { get; set; }

        [Display(Name = "discount")]
        public double discount { get; set; }

		[Display(Name = "credit_limit")]
		public double credit_limit { get; set; }

        [Display(Name = "cr_no")]
        public string cr_no{ get; set; }

        [Display(Name = "vat_no")]
        public string vat_no { get; set; }

        [JsonIgnore]
        [Write(false)]
        [Display(Name = "get_region_lengkap")]
        public string get_region_lengkap
        {
            get
            {
                var provinsi = this.Provinsi != null ? this.Provinsi.name_province : string.Empty;
                var regency = this.Regency != null ? this.Regency.name_regency : this.regency_old.NullToString();
                var subdistrict = this.subdistrict != null ? this.subdistrict.name_subdistrict : this.subdistrict_old.NullToString();

                var codePos = (string.IsNullOrEmpty(this.postal_code) || this.postal_code == "0") ? string.Empty : this.postal_code;

                var sb = new StringBuilder();

                if (provinsi.Length > 0)
                    sb.Append(string.Format("{0}", provinsi)).Append(", ");

                if (regency.Length > 0)
                    sb.Append(string.Format("{0}", regency)).Append(", ");

                if (subdistrict.Length > 0)
                    sb.Append(string.Format("{0}", subdistrict)).Append(", ");

                if (codePos.Length > 0)
                    sb.Append(codePos);

                var regionLengkap = sb.ToString();

                if (regionLengkap.Length > 2)
                {
                    if (regionLengkap.Right(2) == ", ")
                    {
                        regionLengkap = regionLengkap.Left(regionLengkap.Length - 2);
                    }
                }

                return regionLengkap;
            }
        }

        [Computed]
		[Display(Name = "total_credit")]
		public double total_credit { get; set; }

        [Computed]
		[Display(Name = "total_receivable_payment")]
		public double total_receivable_payment { get; set; }

        [Computed]
        public double remaining_credit
        {
            get { return total_credit - total_receivable_payment; }
        }

       
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.name_customer).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
            RuleFor(c => c.address).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.country).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.village).Length(0, 100).WithMessage(msgError2);
            // RuleFor(c => c.subdistrict).Length(0, 100).WithMessage(msgError2);            
            RuleFor(c => c.city).Length(0, 100).WithMessage(msgError2);
            // RuleFor(c => c.regency).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.postal_code).Length(0, 6).WithMessage(msgError2);
			RuleFor(c => c.contact).Length(0, 50).WithMessage(msgError2);
			RuleFor(c => c.phone).Length(0, 20).WithMessage(msgError2);
		}
	}
}
