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

namespace SparkPOS.Helper
{
    public static class NumberHelper
    {
        /// <summary>
        /// Method untuk mengkonversi nilai angka ke dalam format pemisah ribuan
        /// </summary>
        /// <param name="number">Nilai angka yang ingin dikonversi</param>
        /// <returns></returns>
        public static string NumberToString(long number)
        {
            return string.Format("{0:N0}", number);
        }

        public static string NumberToString(float number)
        {
            return string.Format("{0:N0}", number);
        }

        public static string NumberToString(double number)
        {
            return string.Format("{0:N0}", number);
        }

        private static string RemoveSeparator(string s, bool isUseDecimal = false)
        {
            s = s.Replace(",", string.Empty);

            if (!isUseDecimal)
                s = s.Replace(".", string.Empty);

            return s;
        }

        /// <summary>
        /// Method untuk menghilangkan pengaruh format pemisah ribuan
        /// </summary>
        /// <param name="s">Nilai string yang ingin dikonversi</param>
        /// <returns></returns>
        public static long StringToNumber(string s)
        {
            try
            {
                s = RemoveSeparator(s);

                if (s.Length == 0)
                    s = "0";

            }
            catch (Exception)
            {
                s = "0";
            }

            return Convert.ToInt64(s);
        }

        public static float StringToFloat(string s)
        {
            try
            {
                s = RemoveSeparator(s);

                if (s.Length == 0)
                    s = "0";
            }
            catch (Exception)
            {
                s = "0";
            }

            return float.Parse(s);
        }

        public static double StringToDouble_old(string s, bool isUseDecimal = false)
        {
            try
            {

                s = RemoveSeparator(s, isUseDecimal);

                if (s.Length == 0)
                    s = "0";
            }
            catch (Exception)
            {
                s = "0";
            }

            return double.Parse(s);
        }

        public static double StringToDouble(string s, bool isUseDecimal = false)
        {
            try
            {
                // Remove separators (thousands separator, decimal separator) based on the isUseDecimal flag
                s = RemoveSeparator(s, isUseDecimal);

                // If the string is empty after removing separators, set it to "0"
                if (s.Length == 0)
                    s = "0";

                // Parse the modified string to a double and return the result
                return double.Parse(s);
            }
            catch (FormatException)
            {
                // If an exception occurs during parsing due to an invalid format, set the string to "0"
                return 0;
            }
        }

    }
}
