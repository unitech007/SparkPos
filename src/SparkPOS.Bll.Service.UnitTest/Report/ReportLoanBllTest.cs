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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SparkPOS.Model;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;

namespace SparkPOS.Bll.Service.UnitTest.Report
{
    [TestClass]
    public class ReportLoanBllTest
    {
        private ILog _log;
        private IReportLoanBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportLoanBllTest));
            _bll = new ReportLoanBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByMonthAndYearTest()
        {
            var month = 3;
            var year = 2017;

            var oList = _bll.GetByMonth(month, year);

            var index = 3;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("Andi", obj.employee_name);
            Assert.AreEqual("201703280009", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.date);
            Assert.AreEqual(200000, obj.amount);
            Assert.AreEqual(200000, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.description);
        }

        [TestMethod]
        public void GetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.GetByMonth(StartingMonth, EndingMonth, year);

            var index = 3;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("Andi", obj.employee_name);
            Assert.AreEqual("201703280009", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.date);
            Assert.AreEqual(200000, obj.amount);
            Assert.AreEqual(200000, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.description);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 3, 1);
            var tanggalSelesai = new DateTime(2017, 3, 30);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 3;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("Andi", obj.employee_name);
            Assert.AreEqual("201703280009", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.date);
            Assert.AreEqual(200000, obj.amount);
            Assert.AreEqual(200000, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.description);
        }

        [TestMethod]
        public void DetailGetByMonthAndYearTest()
        {
            var month = 3;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(month, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            // header loan
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("Andi", obj.employee_name);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.tanggal_kasbon);
            Assert.AreEqual("201703280009", obj.nota_kasbon);
            Assert.AreEqual(200000, obj.jumlah_kasbon);
            Assert.AreEqual(200000, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.keterangan_kasbon);

            // detail loan
            Assert.AreEqual(new DateTime(2017, 3, 30), obj.tanggal_payment);
            Assert.AreEqual("201703300030", obj.nota_payment);
            Assert.AreEqual(50000, obj.jumlah_payment);
            Assert.AreEqual("pelunasan", obj.keterangan_payment);
        }

        [TestMethod]
        public void DetailGetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(StartingMonth, EndingMonth, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            // header loan
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("Andi", obj.employee_name);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.tanggal_kasbon);
            Assert.AreEqual("201703280009", obj.nota_kasbon);
            Assert.AreEqual(200000, obj.jumlah_kasbon);
            Assert.AreEqual(200000, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.keterangan_kasbon);

            // detail loan
            Assert.AreEqual(new DateTime(2017, 3, 30), obj.tanggal_payment);
            Assert.AreEqual("201703300030", obj.nota_payment);
            Assert.AreEqual(50000, obj.jumlah_payment);
            Assert.AreEqual("pelunasan", obj.keterangan_payment);
        }

        [TestMethod]
        public void DetailGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 30);

            var oList = _bll.DetailGetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            // header loan
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("Andi", obj.employee_name);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.tanggal_kasbon);
            Assert.AreEqual("201703280009", obj.nota_kasbon);
            Assert.AreEqual(200000, obj.jumlah_kasbon);
            Assert.AreEqual(200000, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.keterangan_kasbon);

            // detail loan
            Assert.AreEqual(new DateTime(2017, 3, 30), obj.tanggal_payment);
            Assert.AreEqual("201703300030", obj.nota_payment);
            Assert.AreEqual(50000, obj.jumlah_payment);
            Assert.AreEqual("pelunasan", obj.keterangan_payment);
        }
    }
}
