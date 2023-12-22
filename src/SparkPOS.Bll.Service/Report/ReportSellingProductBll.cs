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

namespace SparkPOS.Bll.Service.Report
{
    public class ReportSellingProductBll : IReportSellingProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportSellingProductBll(ILog log)
        {
            _log = log;
        }

        public IList<ReportSalesProductHeader> GetByMonth(int month, int year)
        {
            IList<ReportSalesProductHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.GetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportSalesProductHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportSalesProductHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.GetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportSalesProductHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProductHeader> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportSalesProductDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportSalesProductDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.DetailGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportSalesProductDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.DetailGetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProductDetail> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.DetailGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerProductGetByMonth(int month, int year)
        {
            IList<ReportSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerProductGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerProductGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerProductGetByMonth(StartingMonth, EndingMonth, year);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerProductGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportProductFavourite> ProductFavouriteGetByMonth(int month, int year, int limit)
        {
            IList<ReportProductFavourite> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.ProductFavouriteGetByMonth(month, year, limit);
            }

            return oList;
        }

        public IList<ReportProductFavourite> ProductFavouriteGetByMonth(int StartingMonth, int EndingMonth, int year, int limit)
        {
            throw new NotImplementedException();
        }

        public IList<ReportProductFavourite> ProductFavouriteGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            IList<ReportProductFavourite> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.ProductFavouriteGetByDate(tanggalMulai, tanggalSelesai, limit);
            }

            return oList;
        }


        public IList<ReportSalesPerCashier> PerCashierGetByMonth(int month, int year)
        {
            IList<ReportSalesPerCashier> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerCashierGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportSalesPerCashier> PerCashierGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalesPerCashier> PerCashierGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesPerCashier> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerCashierGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportCustomerProduct> CustomerProductGetByMonth(int month, int year)
        {
            IList<ReportCustomerProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.CustomerProductGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportCustomerProduct> CustomerProductGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportCustomerProduct> CustomerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCustomerProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.CustomerProductGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }


        public IList<ReportSalesProductPerCategory> PerCategoryGetByMonth(int month, int year)
        {
            IList<ReportSalesProductPerCategory> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerCategoryGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportSalesProductPerCategory> PerCategoryGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalesProductPerCategory> PerCategoryGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProductPerCategory> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerCategoryGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerCategoryDetailGetByMonth(int month, int year)
        {
            IList<ReportSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerCategoryDetailGetByMonth(month, year);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerCategoryDetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalesProduct> PerCategoryDetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportSellingProductRepository.PerCategoryDetailGetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }
    }
}
