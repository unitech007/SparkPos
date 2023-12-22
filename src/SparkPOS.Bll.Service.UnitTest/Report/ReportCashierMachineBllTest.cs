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
    public class ReportCashierMachineBllTest
    {
        private ILog _log;
        private IReportCashierMachineBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportCashierMachineBllTest));
            _bll = new ReportCashierMachineBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void PerCashierGetByUserIdTest()
        {
            var userId = "00b5acfa-b533-454b-8dfd-e7881edd180f";
            var oList = _bll.PerCashierGetByUserId(userId);

            var index = 0;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("d1208856-cbd3-4b59-880c-717192064ad0", obj.machine_id);
            Assert.AreEqual(new DateTime(2017, 10, 19), obj.date);
            Assert.AreEqual(0, obj.starting_balance);

            // cek data sale
            var sale = obj.sale;
            Assert.IsNotNull(sale);
            Assert.AreEqual(0, sale.tax);
            Assert.AreEqual(0, sale.discount);
            Assert.AreEqual(90303, sale.total_invoice);

            // cek item product
            var product = obj.item_jual[1];
            Assert.IsNotNull(product);
            Assert.AreEqual("0150360f-b039-4980-a399-960f1d0beebc", product.product_id);
            Assert.AreEqual("Flashdisk 4 Gb PQI U273", product.product_name);
            Assert.AreEqual(49000, product.selling_price);
            Assert.AreEqual(1, product.quantity);
            Assert.AreEqual(0.5, product.discount);
        }
    }
}
