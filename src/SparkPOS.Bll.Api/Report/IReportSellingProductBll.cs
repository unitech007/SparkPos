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

using SparkPOS.Model;
using SparkPOS.Model.Report;

namespace SparkPOS.Bll.Api.Report
{
    public interface IReportSellingProductBll : IBaseReportBll<ReportSalesProductHeader>
    {
        IList<ReportSalesProductDetail> DetailGetByMonth(int month, int year);
        IList<ReportSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);

        IList<ReportSalesProduct> PerProductGetByMonth(int month, int year);
        IList<ReportSalesProduct> PerProductGetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportSalesProduct> PerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);

        IList<ReportProductFavourite> ProductFavouriteGetByMonth(int month, int year, int limit);
        IList<ReportProductFavourite> ProductFavouriteGetByMonth(int StartingMonth, int EndingMonth, int year, int limit);
        IList<ReportProductFavourite> ProductFavouriteGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int limit);

        IList<ReportSalesPerCashier> PerCashierGetByMonth(int month, int year);
        IList<ReportSalesPerCashier> PerCashierGetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportSalesPerCashier> PerCashierGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);

        IList<ReportCustomerProduct> CustomerProductGetByMonth(int month, int year);
        IList<ReportCustomerProduct> CustomerProductGetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportCustomerProduct> CustomerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);

        IList<ReportSalesProductPerCategory> PerCategoryGetByMonth(int month, int year);
        IList<ReportSalesProductPerCategory> PerCategoryGetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportSalesProductPerCategory> PerCategoryGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);

        IList<ReportSalesProduct> PerCategoryDetailGetByMonth(int month, int year);
        IList<ReportSalesProduct> PerCategoryDetailGetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportSalesProduct> PerCategoryDetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
    }
}
