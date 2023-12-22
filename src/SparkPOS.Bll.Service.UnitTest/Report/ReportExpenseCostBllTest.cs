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

using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;

namespace SparkPOS.Bll.Service.UnitTest.Report
{
    [TestClass]
    public class ReportExpenseCostBllTest
    {
        private ILog _log;
        private IReportExpenseCostBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportExpenseCostBllTest));
            _bll = new ReportExpenseCostBll(_log);
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
            Assert.AreEqual("201703270018", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual("caf8e482-3f96-4fb9-b2fb-8fe7aad8f4cc", obj.expense_type_id);
            Assert.AreEqual("cost baru", obj.name_expense_type);
            Assert.AreEqual(3, obj.quantity);
            Assert.AreEqual(2000, obj.price);
            Assert.AreEqual(6000, obj.sub_total);
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
            Assert.AreEqual("201703270018", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual("caf8e482-3f96-4fb9-b2fb-8fe7aad8f4cc", obj.expense_type_id);
            Assert.AreEqual("cost baru", obj.name_expense_type);
            Assert.AreEqual(3, obj.quantity);
            Assert.AreEqual(2000, obj.price);
            Assert.AreEqual(6000, obj.sub_total);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 27);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("201703270018", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual("caf8e482-3f96-4fb9-b2fb-8fe7aad8f4cc", obj.expense_type_id);
            Assert.AreEqual("cost baru", obj.name_expense_type);
            Assert.AreEqual(3, obj.quantity);
            Assert.AreEqual(2000, obj.price);
            Assert.AreEqual(6000, obj.sub_total);
        }
    }
}
