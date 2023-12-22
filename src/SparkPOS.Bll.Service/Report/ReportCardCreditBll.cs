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
    public class ReportCardCreditBll : IReportCardCreditBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public ReportCardCreditBll(ILog log)
        {
            _log = log;
        }
        
        private void HitungSaldoAwal(IList<ReportCardCredit> oList)
        {
            var currentCustomerId = string.Empty;
            double saldo = 0;

            foreach (var item in oList)
            {                
                if (currentCustomerId != item.customer_id)
                {
                    if (currentCustomerId.Length > 0)
                    {
                        var oldCustomer = oList.LastOrDefault(f => f.customer_id == currentCustomerId);
                        oldCustomer.saldo_akhir = oldCustomer.saldo;
                    }

                    currentCustomerId = item.customer_id;
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

            var lastCustomer = oList.LastOrDefault();
            if (lastCustomer != null)
                lastCustomer.saldo_akhir = lastCustomer.saldo;
        }

        private void HitungSaldoAkhir(IList<ReportCardCredit> listOfSaldoAwal, IList<ReportCardCredit> listOfSaldoAkhir)
        {
            var currentCustomerId = string.Empty;
            var isFirstRecord = false;
            double saldo = 0;

            foreach (var item in listOfSaldoAkhir)
            {
                if (currentCustomerId != item.customer_id)
                {
                    if (currentCustomerId.Length > 0)
                    {
                        var oldCustomer = listOfSaldoAkhir.LastOrDefault(f => f.customer_id == currentCustomerId);
                        oldCustomer.saldo_akhir = oldCustomer.saldo;
                    }

                    saldo = 0;
                    currentCustomerId = item.customer_id;
                    isFirstRecord = true;
                }

                if (isFirstRecord)
                {
                    // copy saldo awal
                    var customerSaldoAwal = listOfSaldoAwal.LastOrDefault(f => f.customer_id == currentCustomerId);
                    if (customerSaldoAwal != null)
                    {
                        item.starting_balance = customerSaldoAwal.saldo_akhir;
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

            var lastCustomer = listOfSaldoAkhir.LastOrDefault();
            if (lastCustomer != null)
                lastCustomer.saldo_akhir = lastCustomer.saldo;
        }

        private IList<ReportCardCredit> GetSaldoAwal(DateTime date)
        {
            IList<ReportCardCredit> oList = new List<ReportCardCredit>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardCreditRepository.GetSaldoAwal(date);                
            }

            return oList;
        }

        public IList<ReportCardCredit> GetByMonth(int month, int year)
        {
            IList<ReportCardCredit> oList = new List<ReportCardCredit>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardCreditRepository.GetByMonth(month, year);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                // hitung saldo awal masing-masing customer
                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList);
            }

            return oList;
        }

        public IList<ReportCardCredit> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportCardCredit> oList = new List<ReportCardCredit>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardCreditRepository.GetByMonth(StartingMonth, EndingMonth, year);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                // hitung saldo awal masing-masing customer
                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList);
            }

            return oList;
        }

        public IList<ReportCardCredit> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCardCredit> oList = new List<ReportCardCredit>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReportCardCreditRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            if (oList.Count > 0)
            {
                var tanggalAwal = oList.Min(f => f.date);

                // hitung saldo awal masing-masing customer
                var listOfSaldoAwal = GetSaldoAwal(tanggalAwal);
                HitungSaldoAwal(listOfSaldoAwal);

                // hitung saldo akhir
                HitungSaldoAkhir(listOfSaldoAwal, oList);
            }

            return oList;
        }
    }
}
