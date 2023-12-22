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
    public class ReportCardStockBll : IReportCardStockBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportCardStockBll(ILog log)
        {
            _log = log;
        }

        private void HitungSaldoAwal(IList<ReportCardStock> oList)
        {
            var currentProductId = string.Empty;
            double saldo = 0;

            foreach (var item in oList)
            {
                if (currentProductId != item.product_id)
                {
                    if (currentProductId.Length > 0)
                    {
                        var oldProduct = oList.LastOrDefault(f => f.product_id == currentProductId);
                        oldProduct.saldo_akhir = oldProduct.saldo;
                    }

                    currentProductId = item.product_id;
                    saldo = 0;
                }

                if (item.type_nota == 1 || item.type_nota == 2) // product masuk dari transactions Purchase atau return sales
                {
                    saldo += item.masuk;
                }
                else // product Exit dari transactions sales atau return Purchase
                {
                    saldo -= item.Exit;
                }
                
                item.saldo = saldo;
            }

            var lastProduct = oList.LastOrDefault();
            if (lastProduct != null)
                lastProduct.saldo_akhir = lastProduct.saldo;
        }

        private void HitungSaldoAkhir(IList<ReportCardStock> listOfSaldoAwal, IList<ReportCardStock> listOfSaldoAkhir, IList<ReportCardStock> listOfStockAwal)
        {
            var currentProductId = string.Empty;
            var isFirstRecord = false;
            double saldo = 0;

            foreach (var item in listOfSaldoAkhir)
            {
                if (currentProductId != item.product_id)
                {
                    if (currentProductId.Length > 0)
                    {
                        var oldProduct = listOfSaldoAkhir.LastOrDefault(f => f.product_id == currentProductId);
                        oldProduct.saldo_akhir = oldProduct.saldo;
                    }

                    saldo = 0;
                    currentProductId = item.product_id;
                    isFirstRecord = true;
                }

                if (isFirstRecord)
                {
                    var stockAwal = listOfStockAwal.Where(f => f.product_id == currentProductId)
                                                 .SingleOrDefault();

                    // copy saldo awal
                    var produkSaldoAwal = listOfSaldoAwal.LastOrDefault(f => f.product_id == currentProductId);
                    if (produkSaldoAwal != null)
                    {                        
                        if (stockAwal != null)
                            item.starting_balance = produkSaldoAwal.saldo_akhir + stockAwal.stock_awal;
                        else
                            item.starting_balance = produkSaldoAwal.saldo_akhir;                        
                    }
                    else
                    {
                        if (stockAwal != null)
                            item.starting_balance = stockAwal.stock_awal;
                    }

                    saldo = item.starting_balance;
                    isFirstRecord = false;
                }

                if (item.type_nota == 1 || item.type_nota == 2) // product masuk dari transactions Purchase atau return sales
                {
                    saldo += item.masuk;
                }
                else // product Exit dari transactions sales atau return Purchase
                {
                    saldo -= item.Exit;
                }

                item.saldo = saldo;
            }

            var lastProduct = listOfSaldoAkhir.LastOrDefault();
            if (lastProduct != null)
                lastProduct.saldo_akhir = lastProduct.saldo;
        }

        private IList<ReportCardStock> GetSaldoAwal(DateTime date)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardStockRepository.GetSaldoAwal(date);
            }

            return oList;
        }

        private void HitungStockAwal(IList<ReportCardStock> listOfSaldoAkhir, ref IList<ReportCardStock> listOfDistinctProduct)
        {
            IList<ReportCardStock> listAllTransaction = new List<ReportCardStock>();
            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);

                IList<string> listOfProductId = listOfSaldoAkhir.Select(f => f.product_id).Distinct().ToList();

                listAllTransaction = _unitOfWork.ReportCardStockRepository.GetAll(listOfProductId);
            }

            listOfDistinctProduct = listAllTransaction.GroupBy(f => new { f.product_id, f.stock_akhir })
                                                     .Select(f => f.First())
                                                     .ToList();

            foreach (var item in listOfDistinctProduct)
            {
                var qtyPurchase = listAllTransaction.Where(f => f.type_nota == 1 && f.product_id == item.product_id)
                                                     .Sum(f => f.qty);

                var qtyReturnSales = listAllTransaction.Where(f => f.type_nota == 2 && f.product_id == item.product_id)
                                                          .Sum(f => f.qty);

                var qtySales = listAllTransaction.Where(f => f.type_nota == 3 && f.product_id == item.product_id)
                                                     .Sum(f => f.qty);

                var qtyReturnPurchase = listAllTransaction.Where(f => f.type_nota == 4 && f.product_id == item.product_id)
                                                          .Sum(f => f.qty);

                item.stock_awal = item.stock_akhir + qtySales + qtyReturnPurchase - qtyPurchase - qtyReturnSales;
            }
        }

        public IList<ReportCardStock> GetByMonth(int month, int year)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardStockRepository.GetByMonth(month, year);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);

                // hitung stock awal
                IList<ReportCardStock> listOfDistinctProduct = new List<ReportCardStock>();
                HitungStockAwal(oList, ref listOfDistinctProduct);
                
                // hitung saldo awal
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList, listOfDistinctProduct);
            }

            return oList;
        }

        public IList<ReportCardStock> GetByMonth(int month, int year, IList<string> listOfCode)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardStockRepository.GetByMonth(month, year, listOfCode);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);

                // hitung stock awal
                IList<ReportCardStock> listOfDistinctProduct = new List<ReportCardStock>();
                HitungStockAwal(oList, ref listOfDistinctProduct);

                // hitung saldo awal
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList, listOfDistinctProduct);
            }

            return oList;
        }

        public IList<ReportCardStock> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportCardStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardStockRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);

                // hitung stock awal
                IList<ReportCardStock> listOfDistinctProduct = new List<ReportCardStock>();
                HitungStockAwal(oList, ref listOfDistinctProduct);

                // hitung saldo awal
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList, listOfDistinctProduct);
            }

            return oList;
        }
        
        public IList<ReportCardStock> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, IList<string> listOfCode)
        {
            IList<ReportCardStock> oList = new List<ReportCardStock>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardStockRepository.GetByDate(tanggalMulai, tanggalSelesai, listOfCode);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);

                // hitung stock awal
                IList<ReportCardStock> listOfDistinctProduct = new List<ReportCardStock>();
                HitungStockAwal(oList, ref listOfDistinctProduct);

                // hitung saldo awal
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList, listOfDistinctProduct);
            }

            return oList;
        }
    }
}
