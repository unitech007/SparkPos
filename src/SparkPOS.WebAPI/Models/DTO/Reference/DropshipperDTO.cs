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

namespace SparkPOS.WebAPI.Models.DTO
{        
    public class DropshipperDTO
    {
		[Display(Name = "dropshipper_id")]		
		public string dropshipper_id { get; set; }
		
		[Display(Name = "name_dropshipper")]
		public string name_dropshipper { get; set; }
		
		[Display(Name = "address")]
		public string address { get; set; }
		
		[Display(Name = "contact")]
		public string contact { get; set; }
		
		[Display(Name = "phone")]
		public string phone { get; set; }
		
	}

    public class DropshipperDTOValidator : AbstractValidator<DropshipperDTO>
    {
        public DropshipperDTOValidator()
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
                RuleFor(c => c.dropshipper_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);						
                DefaultRule(msgError1, msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.dropshipper_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);						
            });
		}

        private void DefaultRule(string msgError1, string msgError2)
        {
            RuleFor(c => c.name_dropshipper).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
            RuleFor(c => c.address).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.contact).Length(0, 50).WithMessage(msgError2);
            RuleFor(c => c.phone).Length(0, 20).WithMessage(msgError2);			
        }
	}
}
