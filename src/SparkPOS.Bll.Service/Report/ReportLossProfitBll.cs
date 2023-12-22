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

using log4net;
using SparkPOS.Model;
using SparkPOS.Model.Report;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
using SparkPOS.Helper;

namespace SparkPOS.Bll.Service.Report
{
    public class ReportLossProfitBll : IReportLossProfitBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportLossProfitBll(ILog log)
        {
            _log = log;
        }

        public ReportLossProfit GetByMonth(int month, int year)
        {
            ReportLossProfit obj = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                obj = _unitOfWork.ReportLossProfitRepository.GetByMonth(month, year);
            }

            return obj;
        }

        public ReportLossProfit GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            ReportLossProfit obj = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                obj = _unitOfWork.ReportLossProfitRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            return obj;
        }
    }
}
