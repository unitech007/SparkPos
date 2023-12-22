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
    public class ReportCardStockBllTest
    {
        private ILog _log;
        private IReportCardStockBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportCardStockBllTest));
            _bll = new ReportCardStockBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByMonthAndYearTest()
        {
            var month = 1;
            var year = 2017;

            var oList = _bll.GetByMonth(month, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.type_nota);
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", obj.product_id);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", obj.product_name);
            Assert.AreEqual("201701310073", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 31), obj.date);
            Assert.AreEqual("Sigma komputer", obj.supplier_or_customer);
            Assert.AreEqual(4, obj.qty);
            Assert.AreEqual("beli Credit", obj.description);
        }

        [TestMethod]
        public void GetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 2;
            var year = 2017;

            var oList = _bll.GetByMonth(StartingMonth, EndingMonth, year);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.type_nota);
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", obj.product_id);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", obj.product_name);
            Assert.AreEqual("201701310073", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 31), obj.date);
            Assert.AreEqual("Sigma komputer", obj.supplier_or_customer);
            Assert.AreEqual(4, obj.qty);
            Assert.AreEqual("beli Credit", obj.description);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 2, 28);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.type_nota);
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", obj.product_id);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", obj.product_name);
            Assert.AreEqual("201701310073", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 31), obj.date);
            Assert.AreEqual("Sigma komputer", obj.supplier_or_customer);
            Assert.AreEqual(4, obj.qty);
            Assert.AreEqual("beli Credit", obj.description);
        }
    }
}
