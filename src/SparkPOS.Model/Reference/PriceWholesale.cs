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

using Dapper.Contrib.Extensions;

namespace SparkPOS.Model
{        
	[Table("m_wholesale_price")]
    public class PriceWholesale
    {
        [ExplicitKey]
        public string wholesale_price_id { get; set; }

		public string product_id { get; set; }

		[Write(false)]
        public Product Product { get; set; }

		public int retail_price { get; set; }
		public double wholesale_price { get; set; }
		public double minimum_quantity { get; set; }
		public double discount { get; set; }
	}    
}
