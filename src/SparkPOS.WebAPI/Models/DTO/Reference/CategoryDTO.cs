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
using System.Web;

using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace SparkPOS.WebAPI.Models.DTO
{
    public class CategoryDTO
    {
        [Display(Name = "category_id")]
        public string category_id { get; set; }

        [Display(Name = "Category")]
        public string name_category { get; set; }

        [Display(Name = "discount")]
        public double discount { get; set; }
    }

    public class CategoryDTOValidator : AbstractValidator<CategoryDTO>
    {
        public CategoryDTOValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "'{PropertyName}' maksimal {MaxLength} karakter !";            

            RuleSet("save", () =>
            {
                RuleFor(c => c.name_category).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
            });

            RuleSet("update", () =>
            {
                RuleFor(c => c.category_id).NotEmpty().WithMessage(msgError1);
                RuleFor(c => c.name_category).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
            });

            RuleSet("delete", () =>
            {
                RuleFor(c => c.category_id).NotEmpty().WithMessage(msgError1);
            });
        }
    }
}