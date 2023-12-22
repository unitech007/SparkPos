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
using SparkPOS.Model.Report;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;

namespace SparkPOS.Bll.Service.Report
{
    public class ReportStockProductBll : IReportStockProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportStockProductBll(ILog log)
        {
            _log = log;
        }
        
        public IList<ReportStockProduct> GetStockByStatus(StatusStock statusStock)
        {
            IList<ReportStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetStockByStatus(statusStock);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockLessFrom(double stock)
        {
            IList<ReportStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetStockLessFrom(stock);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedSupplier(string supplierId)
        {
            IList<ReportStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetStockBasedSupplier(supplierId);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedCategory(string golonganId)
        {
            IList<ReportStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetStockBasedCategory(golonganId);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedCode(IList<string> listOfCode)
        {
            IList<ReportStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetStockBasedCode(listOfCode);
            }

            return oList;
        }

        public IList<ReportStockProduct> GetStockBasedName(string name)
        {
            IList<ReportStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetStockBasedName(name);
            }

            return oList;
        }

        public IList<ReportAdjustmentStockProduct> GetAdjustmentStockByMonth(int month, int year)
        {
            IList<ReportAdjustmentStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetAdjustmentStockByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportAdjustmentStockProduct> GetAdjustmentStockByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportAdjustmentStockProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportStockProductRepository.GetAdjustmentStockByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }        
    }
}
