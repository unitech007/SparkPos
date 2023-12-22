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
    public class ReportRevenueExpenseBllTest
    {
        private ILog _log;
        private IReportRevenueExpenseBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportRevenueExpenseBllTest));
            _bll = new ReportRevenueExpenseBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByMonthAndYearTest()
        {
            var month = 10;
            var year = 2017;

            var obj = _bll.GetByMonth(month, year);

            Assert.IsNotNull(obj);
            Assert.AreEqual(15710303, obj.sales);
            Assert.AreEqual(862000, obj.Purchase);

            Assert.AreEqual(5, obj.list_of_beban.Count);

            var index = 4;
            var beban = obj.list_of_beban[index];
            Assert.AreEqual("Cost Salary Employee", beban.description);
            Assert.AreEqual(1860000, beban.quantity);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 10, 24);
            var tanggalSelesai = new DateTime(2017, 10, 24);

            var obj = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            Assert.IsNotNull(obj);
            Assert.AreEqual(333000, obj.sales);
            Assert.AreEqual(862000, obj.Purchase);

            Assert.AreEqual(5, obj.list_of_beban.Count);

            var index = 4;
            var beban = obj.list_of_beban[index];
            Assert.AreEqual("Cost Salary Employee", beban.description);
            Assert.AreEqual(1860000, beban.quantity);
        }
    }
}
