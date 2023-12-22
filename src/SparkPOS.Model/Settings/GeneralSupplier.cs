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

namespace SparkPOS.Model
{
    public enum TypePrinter
    {
        /// <summary>
        /// Type printer inkjet/lasetjet
        /// </summary>
        InkJet = 1, 

        /// <summary>
        /// Type printer dot matrix
        /// </summary>
        DotMatrix = 2, 

        /// <summary>
        /// Type printer mini pos
        /// </summary>
        MiniPOS = 3
    }

    public class GeneralSupplier
    {
        public string name_printer { get; set; }
        public bool is_auto_print { get; set; }
        public bool is_auto_print_label_nota { get; set; }
        public TypePrinter type_printer { get; set; }
        public bool is_print_customer { get; set; }
        public bool is_show_minimal_stock { get; set; }
        public bool is_customer_required { get; set; }
        public bool is_print_keterangan_nota { get; set; }
        public bool is_focus_on_inputting_quantity_column { get; set; }

        public bool is_autocut { get; set; }
        public string autocut_code { get; set; }

        public bool is_open_cash_drawer { get; set; }
        public string open_cash_drawer_code { get; set; }

        /// <summary>
        /// Validasi stock product boleh minus ketika sales
        /// </summary>
        public bool is_negative_stock_allowed_for_products { get; set; }

        /// <summary>
        /// Update price sale master product jika terjadi perubahan price pada saat sales
        /// </summary>
        public bool is_update_selling_price { get; set; }

        public double default_ppn { get; set; }
        public bool is_singkat_penulisan_ongkir { get; set; }
        public bool is_show_additional_sales_item_information { get; set; }
        public string additional_sales_item_information { get; set; }
        public int jumlah_karakter { get; set; }
        public int jumlah_gulung { get; set; }
        public int size_font { get; set; }
        public IList<HeaderInvoice> list_of_header_nota { get; set; }
        public IList<HeaderInvoiceMiniPos> list_of_header_nota_mini_pos { get; set; }
        public IList<FooterInvoiceMiniPos> list_of_footer_nota_mini_pos { get; set; }
        public IList<LabelInvoice> list_of_label_nota { get; set; }        
    }
}
