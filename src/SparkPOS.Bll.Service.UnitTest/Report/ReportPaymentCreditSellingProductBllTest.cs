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
    public class ReportPaymentCreditSellingProductBllTest
    {
        private ILog _log;
        private IReportPaymentCreditSellingProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportPaymentCreditSellingProductBllTest));
            _bll = new ReportPaymentCreditSellingProductBll(_log);
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

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual(256000, obj.total_payment);
            Assert.AreEqual("Sales cash product", obj.description);
        }

        [TestMethod]
        public void GetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.GetByMonth(StartingMonth, EndingMonth, year);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual(256000, obj.total_payment);
            Assert.AreEqual("Sales cash product", obj.description);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 3, 1);
            var tanggalSelesai = new DateTime(2017, 3, 28);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual(256000, obj.total_payment);
            Assert.AreEqual("Sales cash product", obj.description);
        }

        [TestMethod]
        public void DetailGetByMonthAndYearTest()
        {
            var month = 3;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(month, year);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);
            Assert.AreEqual("201703210055", obj.nota_jual);
            Assert.AreEqual("201703210023", obj.nota_pay);
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.discount);
            Assert.AreEqual(256000, obj.total_invoice);
            Assert.AreEqual(256000, obj.pelunasan);
            Assert.AreEqual("", obj.keterangan_jual);
            Assert.AreEqual("Sales cash product", obj.keterangan_pay);
        }

        [TestMethod]
        public void DetailGetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(StartingMonth, EndingMonth, year);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);
            Assert.AreEqual("201703210055", obj.nota_jual);
            Assert.AreEqual("201703210023", obj.nota_pay);
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.discount);
            Assert.AreEqual(256000, obj.total_invoice);
            Assert.AreEqual(256000, obj.pelunasan);
            Assert.AreEqual("", obj.keterangan_jual);
            Assert.AreEqual("Sales cash product", obj.keterangan_pay);
        }

        [TestMethod]
        public void DetailGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 28);

            var oList = _bll.DetailGetByDate(tanggalMulai, tanggalSelesai);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);
            Assert.AreEqual("201703210055", obj.nota_jual);
            Assert.AreEqual("201703210023", obj.nota_pay);
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.discount);
            Assert.AreEqual(256000, obj.total_invoice);
            Assert.AreEqual(256000, obj.pelunasan);
            Assert.AreEqual("", obj.keterangan_jual);
            Assert.AreEqual("Sales cash product", obj.keterangan_pay);
        }
    }
}
