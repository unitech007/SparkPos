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
    public class ReportCardDebtBll : IReportCardDebtBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportCardDebtBll(ILog log)
        {
            _log = log;
        }

        private void HitungSaldoAwal(IList<ReportCardDebt> oList)
        {
            var currentSupplierId = string.Empty;
            double saldo = 0;

            foreach (var item in oList)
            {
                if (currentSupplierId != item.supplier_id)
                {
                    if (currentSupplierId.Length > 0)
                    {
                        var oldSupplier = oList.LastOrDefault(f => f.supplier_id == currentSupplierId);
                        oldSupplier.saldo_akhir = oldSupplier.saldo;
                    }

                    currentSupplierId = item.supplier_id;
                    saldo = 0;
                }

                if (item.type == 1) // Purchase Credit
                {
                    saldo += item.total;
                }
                else // Dept Payment
                {
                    saldo -= item.total;
                }

                item.saldo = saldo;
            }

            var lastSupplier = oList.LastOrDefault();
            if (lastSupplier != null)
                lastSupplier.saldo_akhir = lastSupplier.saldo;
        }

        private void HitungSaldoAkhir(IList<ReportCardDebt> listOfSaldoAwal, IList<ReportCardDebt> listOfSaldoAkhir)
        {
            var currentSupplierId = string.Empty;
            var isFirstRecord = false;
            double saldo = 0;            

            foreach (var item in listOfSaldoAkhir)
            {
                if (currentSupplierId != item.supplier_id)
                {
                    if (currentSupplierId.Length > 0)
                    {
                        var oldSupplier = listOfSaldoAkhir.LastOrDefault(f => f.supplier_id == currentSupplierId);
                        oldSupplier.saldo_akhir = oldSupplier.saldo;
                    }

                    saldo = 0;
                    currentSupplierId = item.supplier_id;                    
                    isFirstRecord = true;
                }

                if (isFirstRecord)
                {
                    // copy saldo awal
                    var supplierSaldoAwal = listOfSaldoAwal.LastOrDefault(f => f.supplier_id == currentSupplierId);
                    if (supplierSaldoAwal != null)
                    {
                        item.starting_balance = supplierSaldoAwal.saldo_akhir;
                        saldo = item.starting_balance;
                    }

                    isFirstRecord = false;
                }

                if (item.type == 1) // Purchase Credit
                {
                    saldo += item.total;
                }
                else // Dept Payment
                {
                    saldo -= item.total;
                }

                item.saldo = saldo;
            }

            var lastSupplier = listOfSaldoAkhir.LastOrDefault();
            if (lastSupplier != null)
                lastSupplier.saldo_akhir = lastSupplier.saldo;
        }

        private IList<ReportCardDebt> GetSaldoAwal(DateTime date)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardDebtRepository.GetSaldoAwal(date);
            }

            return oList;
        }

        public IList<ReportCardDebt> GetByMonth(int month, int year)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardDebtRepository.GetByMonth(month, year);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                // hitung saldo awal masing-masing supplier
                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList);
            }

            return oList;
        }

        public IList<ReportCardDebt> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardDebtRepository.GetByMonth(StartingMonth, EndingMonth, year);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                // hitung saldo awal masing-masing supplier
                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList);
            }

            return oList;
        }

        public IList<ReportCardDebt> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCardDebt> oList = new List<ReportCardDebt>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardDebtRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                // hitung saldo awal masing-masing supplier
                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList);
            }

            return oList;
        }        
    }
}
