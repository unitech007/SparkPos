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

using SparkPOS.Model.Report;

namespace SparkPOS.Repository.Api.Report
{
    public interface IReportCardStockRepository
    {
        IList<ReportCardStock> GetSaldoAwal(DateTime date);

        IList<ReportCardStock> GetAll(IList<string> listOfProductId);
        IList<ReportCardStock> GetByMonth(int month, int year);
        IList<ReportCardStock> GetByMonth(int month, int year, IList<string> listOfCode);
        IList<ReportCardStock> GetByMonth(int StartingMonth, int EndingMonth, int year);
        IList<ReportCardStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<ReportCardStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, IList<string> listOfCode);
    }
}
