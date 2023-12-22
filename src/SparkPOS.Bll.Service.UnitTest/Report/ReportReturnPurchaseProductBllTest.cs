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
    public class ReportReturnPurchaseProductBllTest
    {
        private ILog _log;
        private IReportReturnPurchaseProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportReturnPurchaseProductBllTest));
            _bll = new ReportReturnPurchaseProductBll(_log);
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

            var index = 2;
            var obj = oList[index];            

            Assert.IsNotNull(obj);
            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201703180121", obj.nota_beli);
            Assert.AreEqual("201703200030", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 20), obj.tanggal_return);
            Assert.AreEqual(270000, obj.total_return);            
        }

        [TestMethod]
        public void GetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.GetByMonth(StartingMonth, EndingMonth, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201703180121", obj.nota_beli);
            Assert.AreEqual("201703200030", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 20), obj.tanggal_return);
            Assert.AreEqual(270000, obj.total_return);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 3, 1);
            var tanggalSelesai = new DateTime(2017, 3, 20);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201703180121", obj.nota_beli);
            Assert.AreEqual("201703200030", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 20), obj.tanggal_return);
            Assert.AreEqual(270000, obj.total_return);
        }

        [TestMethod]
        public void DetailGetByMonthAndYearTest()
        {
            var month = 3;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(month, year);

            var index = 3;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(4, oList.Count);
            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201703180121", obj.nota_beli);
            Assert.AreEqual("201703200030", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 20), obj.tanggal_return);

            Assert.AreEqual("Access Point TPLINK TL-MR3220 3,5G", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(1, obj.return_quantity);
            Assert.AreEqual(200000, obj.price);
            Assert.AreEqual(200000, obj.sub_total);

        }

        [TestMethod]
        public void DetailGetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(StartingMonth, EndingMonth, year);

            var index = 3;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(4, oList.Count);
            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201703180121", obj.nota_beli);
            Assert.AreEqual("201703200030", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 20), obj.tanggal_return);

            Assert.AreEqual("Access Point TPLINK TL-MR3220 3,5G", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(1, obj.return_quantity);
            Assert.AreEqual(200000, obj.price);
            Assert.AreEqual(200000, obj.sub_total);
        }

        [TestMethod]
        public void DetailGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 20);

            var oList = _bll.DetailGetByDate(tanggalMulai, tanggalSelesai);

            var index = 3;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(4, oList.Count);
            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201703180121", obj.nota_beli);
            Assert.AreEqual("201703200030", obj.nota_return);
            Assert.AreEqual(new DateTime(2017, 3, 20), obj.tanggal_return);

            Assert.AreEqual("Access Point TPLINK TL-MR3220 3,5G", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(1, obj.return_quantity);
            Assert.AreEqual(200000, obj.price);
            Assert.AreEqual(200000, obj.sub_total);
        }
    }
}
