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
    public static class DayMonthHelper
    {

        /// <summary>
        /// Untuk mengecek date Minimum
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsMinDate(DateTime date)
        {
            try
            {
                return (date == DateTime.MinValue || date == new DateTime(1753, 1, 1));
            }
            catch (Exception ex)
            {
                MainProgram.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Untuk mendapatkan index day
        /// </summary>
        /// <param name="date">date yang ingin dicari index daynya</param>
        /// <returns></returns>
        private static int Weekday(DateTime date)
        {
            DayOfWeek startOfWeek = DayOfWeek.Sunday;
            return (date.DayOfWeek - startOfWeek + 7) % 7;
        }

        /// <summary>
        /// Untuk mendapatkan name day based date
        /// </summary>
        /// <param name="date">date yang ingin dicari daynya</param>
        /// <returns></returns>
        //public static string GetDayIndonesia(DateTime date)
        //{
        //  //  string[] day = { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jum'at", "Sabtu" };
        //   // string[] day = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        //    string[] day ;
        //    if (MainProgram.currentLanguage == "en-US")
        //    {
        //       day = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        //    }
        //    else if (MainProgram.currentLanguage == "ar-SA")
        //    {
        //        day = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        //    }

        //    return day[Weekday(date)];
        //}

        public static string GetDayIndonesia(DateTime date)
        {
            string[] day;

            if (MainProgram.currentLanguage == "en-US")
            {
                day = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                day = new string[] { "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت" };
            }
            else
            {
                // Default language or unsupported language
                day = new string[] { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };
            }

            return day[(int)date.DayOfWeek];
        }
        /// <summary>
        /// Untuk mendapatkan name month dalam format huruf based month angka
        /// </summary>
        /// <param name="month">Diisi dengan month angka</param>
        /// <returns></returns>
        public static string GetMonthIndonesia(int month)
        {
            string[] bulans;

            if (MainProgram.currentLanguage == "en-US")
            {
                bulans = new string[] {
            "January", "February", "March", "April", "May", "June", "July",
            "August", "September", "October", "November", "December"
        };
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                bulans = new string[] {
            "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو",
            "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
        };
            }
            else
            {
                bulans = new string[] {
            "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli",
            "Agustus", "September", "Oktober", "November", "Desember"
        };
            }

            return bulans[month - 1];
        }


        /// <summary>
        /// Untuk mendapatkan index month based name month
        /// </summary>
        /// <param name="month">Diisi dengan name month. Misal Januari</param>
        /// <returns></returns>
        public static int GetMonthAngka(string month)
        {
            Dictionary<string, string> listMonth = new Dictionary<string, string>();
            if (MainProgram.currentLanguage == "en-US")
            {
                listMonth.Clear();
                listMonth.Add("January", "1");
                listMonth.Add("Febraury", "2");
                listMonth.Add("March", "3");
                listMonth.Add("April", "4");
                listMonth.Add("May", "5");
                listMonth.Add("June", "6");
                listMonth.Add("July", "7");
                listMonth.Add("August", "8");
                listMonth.Add("September", "9");
                listMonth.Add("October", "10");
                listMonth.Add("November", "11");
                listMonth.Add("December", "12");
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                listMonth.Clear();
                listMonth.Add("يناير", "1");
                listMonth.Add("شهر فبراير", "2");
                listMonth.Add("يمشي", "3");
                listMonth.Add("أبريل", "4");
                listMonth.Add("يمكن", "5");
                listMonth.Add("يونيو", "6");
                listMonth.Add("يوليو", "7");
                listMonth.Add("أغسطس", "8");
                listMonth.Add("سبتمبر", "9");
                listMonth.Add("اكتوبر", "10");
                listMonth.Add("شهر نوفمبر", "11");
                listMonth.Add("ديسمبر", "12");
            }
        

            return Convert.ToInt32(listMonth[StringHelper.Split(month, 0)]);
        }

        /// <summary>
        /// Untuk mendapatkan Information list month dan year
        /// </summary>
        /// <param name="tahunMulai">Diisi dengan year mulai, nilai default 2011</param>
        /// <param name="isMonthOnly">Diisi dengan nilai true atau false, nilai default false</param>
        /// <returns></returns>
        public static List<string> GetListMonth(int tahunMulai = 2011, bool isMonthOnly = false)
        {
            var listMonth = new List<string>();

            for (int year = tahunMulai; year <= DateTime.Today.Year; year++)
            {
                for (int month = 1; month < 13; month++)
                {
                    if (isMonthOnly)
                    {
                        listMonth.Add(GetMonthIndonesia(month));
                    }
                    else
                    {
                        listMonth.Add(GetMonthIndonesia(month) + " " + year);
                    }
                }
            }

            return listMonth;
        }
    }
}
