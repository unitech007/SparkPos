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
    public class SettingsLabelPrice
    {
        public string name_printer { get; set; }
        public string font_name { get; set; }
        public float batas_atas_row1 { get; set; }
        public float batas_atas_row2 { get; set; }
        public float batas_atas_row3 { get; set; }
        public float batas_atas_row4 { get; set; }
        public float batas_atas_row5 { get; set; }
        public float batas_atas_row6 { get; set; }
        public float batas_atas_row7 { get; set; }
        public float batas_atas_row8 { get; set; }

        public float batas_kiri_column1 { get; set; }
        public float batas_kiri_column2 { get; set; }
        public float batas_kiri_column3 { get; set; }
        public float batas_kiri_column4 { get; set; }
    }
}