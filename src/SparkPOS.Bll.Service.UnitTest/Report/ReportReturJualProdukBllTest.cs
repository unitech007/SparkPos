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
    public class ReportReturnSellingProductBllTest
    {
        private ILog _log;
        private IReportReturnSellingProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportReturnSellingProductBllTest));
            _bll = new ReportReturnSellingProductBll(_log);
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
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.nota_jual);
            Assert.AreEqual("201703240018", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 24), obj.tanggal_return);
            Assert.AreEqual(456000, obj.total_return);
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
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.nota_jual);
            Assert.AreEqual("201703240018", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 24), obj.tanggal_return);
            Assert.AreEqual(456000, obj.total_return);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 3, 1);
            var tanggalSelesai = new DateTime(2017, 3, 24);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.nota_jual);
            Assert.AreEqual("201703240018", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 24), obj.tanggal_return);
            Assert.AreEqual(456000, obj.total_return);
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
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.nota_jual);
            Assert.AreEqual("201703240018", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 24), obj.tanggal_return);

            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.return_quantity);
            Assert.AreEqual(53000, obj.price);
            Assert.AreEqual(106000, obj.sub_total);

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
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.nota_jual);
            Assert.AreEqual("201703240018", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 24), obj.tanggal_return);

            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.return_quantity);
            Assert.AreEqual(53000, obj.price);
            Assert.AreEqual(106000, obj.sub_total);
        }

        [TestMethod]
        public void DetailGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 24);

            var oList = _bll.DetailGetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.nota_jual);
            Assert.AreEqual("201703240018", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 24), obj.tanggal_return);

            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.return_quantity);
            Assert.AreEqual(53000, obj.price);
            Assert.AreEqual(106000, obj.sub_total);
        }
    }
}
