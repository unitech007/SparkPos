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

namespace SparkPOS.Model
{
    [Table("m_application_setting")]
    public class SettingApplication
    {
        [ExplicitKey]
        public string application_setting_id { get; set; }
        public bool is_update_selling_price_of_master_products { get; set; }
        public bool is_negative_stock_allowed_for_products { get; set; }
        public bool is_focus_on_inputting_quantity_column { get; set; }
        public bool is_show_additional_sales_item_information { get; set; }
        public string additional_sales_item_information { get; set; }
    }
}
