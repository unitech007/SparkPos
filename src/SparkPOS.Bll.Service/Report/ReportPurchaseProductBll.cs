﻿/**
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

namespace SparkPOS.Bll.Service.Report
{
    public class ReportPurchaseProductBll : IReportPurchaseProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportPurchaseProductBll(ILog log)
        {
            _log = log;
        }

        public IList<ReportProductPurchaseHeader> GetByMonth(int month, int year)
        {
            IList<ReportProductPurchaseHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportPurchaseProductRepository.GetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportProductPurchaseHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportProductPurchaseHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportPurchaseProductRepository.GetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportProductPurchaseHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportProductPurchaseHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportPurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportProductPurchaseDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportProductPurchaseDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportPurchaseProductRepository.DetailGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportProductPurchaseDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportProductPurchaseDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportPurchaseProductRepository.DetailGetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportProductPurchaseDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportProductPurchaseDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportPurchaseProductRepository.DetailGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }        
    }
}
