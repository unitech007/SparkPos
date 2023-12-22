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
    public class ReportCreditSellingProductBll : IReportCreditSellingProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportCreditSellingProductBll(ILog log)
        {
            _log = log;
        }

        public IList<ReportCreditSalesProductHeader> GetByMonth(int month, int year)
        {
            IList<ReportCreditSalesProductHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.GetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCreditSalesProductHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.GetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCreditSalesProductHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportCreditSalesProductDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.DetailGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCreditSalesProductDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.DetailGetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportCreditSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCreditSalesProductDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.DetailGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        private void HitungGrandTotalCustomer(IList<ReportCreditSalesProduct> listOfReportCredit)
        {
            var isFirstRecord = false;
            var jualId = string.Empty;

            foreach (var item in listOfReportCredit)
            {
                if (jualId != item.sale_id)
                {
                    jualId = item.sale_id;
                    isFirstRecord = true;
                }

                if (isFirstRecord)
                {
                    item.total_repayment_customer = item.total_payment;
                    item.grand_total_customer = item.grand_total;

                    isFirstRecord = false;
                }
            }
        }

        public IList<ReportCreditSalesProduct> PerProductGetByMonth(int month, int year)
        {
            IList<ReportCreditSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.PerProductGetByMonth(month, year);
            }

            if (oList.Count > 0)
            {
                HitungGrandTotalCustomer(oList);
            }

            return oList;
        }        

        public IList<ReportCreditSalesProduct> PerProductGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCreditSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.PerProductGetByMonth(StartingMonth, EndingMonth, year);
            }

            if (oList.Count > 0)
            {
                HitungGrandTotalCustomer(oList);
            }

            return oList;
        }

        public IList<ReportCreditSalesProduct> PerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCreditSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCreditSellingProductRepository.PerProductGetByDate(tanggalMulai, tanggalSelesai);
            }

            if (oList.Count > 0)
            {
                HitungGrandTotalCustomer(oList);
            }

            return oList;
        }        
    }
}
