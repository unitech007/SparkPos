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

using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace SparkPOS.Model
{
    public class LabelAddressShipping
    {
        [Display(Name = "From #1")]
        public string dari1 { get; set; }

        [Display(Name = "From #2")]
        public string dari2 { get; set; }

        [Display(Name = "From #3")]
        public string dari3 { get; set; }

        [Display(Name = "From #4")]
        public string dari4 { get; set; }

        [Display(Name = "to #1")]
        public string to1 { get; set; }

        [Display(Name = "to #2")]
        public string to2 { get; set; }

        [Display(Name = "to #3")]
        public string to3 { get; set; }

        [Display(Name = "to #4")]
        public string to4 { get; set; }
    }

    public class LabelAddressShippingValidator : AbstractValidator<LabelAddressShipping>
    {
        public LabelAddressShippingValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.dari1).NotEmpty().WithMessage(msgError1).Length(1, 100).WithMessage(msgError2);
            RuleFor(c => c.dari2).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.dari3).Length(0, 100).WithMessage(msgError2);
            RuleFor(c => c.dari4).Length(0, 100).WithMessage(msgError2);

            RuleFor(c => c.to1).NotEmpty().WithMessage(msgError1).Length(1, 250).WithMessage(msgError2);
            RuleFor(c => c.to2).NotEmpty().WithMessage(msgError1).Length(1, 250).WithMessage(msgError2);
            RuleFor(c => c.to3).Length(0, 250).WithMessage(msgError2);
            RuleFor(c => c.to4).Length(0, 250).WithMessage(msgError2);
        }
    }
}
