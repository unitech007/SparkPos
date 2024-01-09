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
    public static class DateTimeHelper
    {

        public static DateTime GetNullDateTime()
        {
            return new DateTime(0001, 1, 1);
        }

        /// <summary>
        /// Method untuk mengecek apakah sebuah date null atau tidak
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsNull(DateTime date)
        {
            var result = true;

            try
            {
                result = date == DateTime.MinValue || date == new DateTime(1753, 1, 1) ||
                         date == new DateTime(0001, 1, 1);
            }
            catch (Exception ex)
            {
                MainProgram.LogException(ex);
                //var msg = "Something wrong check error log";
                //MsgHelper.MsgWarning(msg);

            }

            return result;
        }

        /// <summary>
        /// Untuk mengkonversi data date ke format dd/MM/yyyy
        /// </summary>
        /// <param name="date">Data date dengan type DateTime</param>
        /// <returns></returns>
        public static string DateToString(Nullable<DateTime> date, string format = "dd/MM/yyyy")
        {
            var result = string.Empty;

            try
            {
                if (!(date == DateTime.MinValue || date == new DateTime(1753, 1, 1)))
                    result = string.Format("{0:" + format + "}", date);
            }
            catch (Exception ex)
            {
                MainProgram.LogException(ex);
                //var msg = "Something wrong check error log";
                //MsgHelper.MsgWarning(msg);

            }

            return result;
        }

        /// <summary>
        /// Untuk mengkonversi data date ke format day month year
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static string DateToString(int day, int month, int year)
        {
            string result = day + " " + DayMonthHelper.GetMonthIndonesia(month) + " " + year;

            return result;
        }

        public static string TimeToString(DateTime time, string format = "HH:mm:ss")
        {
            string result = time.ToString(format);

            return result;
        }

        public static DateTime GetTime(int hour, int minute, int second = 0)
        {
            return new DateTime(1753, 1, 1, hour, minute, second);
        }

        public static DateTime GetTime(string time)
        {
            var words = time.Split(':');
            var time1 = new DateTime(1753, 1, 1, int.Parse(words[0]), int.Parse(words[1]), 0);

            return time1;
        }

        /// <summary>
        /// Untuk mengkonversi data string ke format date
        /// </summary>
        /// <param name="date">Data date dengan format dd/MM/yyyy</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string date)
        {
            var words = date.Split('/');

            var tgl = new DateTime(int.Parse(words[2]), int.Parse(words[1]), int.Parse(words[0]));

            return tgl;
        }

        public static bool IsValidTimeValue(string time)
        {
            var result = false;

            try
            {
                var words = time.Split(':');
                var time1 = new DateTime(1753, 1, 1, int.Parse(words[0]), int.Parse(words[1]), 0);

                result = true;
            }
            catch (Exception ex)
            {
                MainProgram.LogException(ex);
                //var msg = "Something wrong check error log";
                //MsgHelper.MsgWarning(msg);

            }

            return result;
        }

        public static bool IsValidRangeDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            return tanggalMulai <= tanggalSelesai;
        }
    }
}
