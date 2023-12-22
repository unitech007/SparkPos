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

using SparkPOS.Model.Report;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;

namespace SparkPOS.Bll.Service.UnitTest.Report
{
    [TestClass]
    public class ReportStockProductBllTest
    {
        private ILog _log;
        private IReportStockProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportStockProductBllTest));
            _bll = new ReportStockProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetStockByStatusAllTest()
        {
            var oList = _bll.GetStockByStatus(StatusStock.All);

            var index = 2;
            var obj = oList[index];

            // tes product
            Assert.IsNotNull(obj);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);

            // tes category
            Assert.AreEqual("0a8b59e5-bb3b-4081-b963-9dc9584dcda6", obj.category_id);
            Assert.AreEqual("Accessories", obj.name_category);
        }

        [TestMethod]
        public void GetStockByStatusAdaTest()
        {
            var oList = _bll.GetStockByStatus(StatusStock.there);

            var index = 1;
            var obj = oList[index];

            // tes product
            Assert.IsNotNull(obj);
            Assert.AreEqual("53b63dc2-4505-4276-9886-3639b53b7458", obj.product_id);
            Assert.AreEqual("Access Point TPLINK TL-MR3220 3,5G", obj.product_name);
            Assert.AreEqual(4, obj.stock);
            Assert.AreEqual(35, obj.warehouse_stock);
            Assert.AreEqual(200000, obj.purchase_price);
            Assert.AreEqual(350000, obj.selling_price);

            // tes category
            Assert.AreEqual("0a8b59e5-bb3b-4081-b963-9dc9584dcda6", obj.category_id);
            Assert.AreEqual("Accessories", obj.name_category);
        }

        [TestMethod]
        public void GetStockByStatusEmptyTest()
        {
            var oList = _bll.GetStockByStatus(StatusStock.Empty);

            var index = 0;
            var obj = oList[index];

            // tes product
            Assert.IsNotNull(obj);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);

            // tes category
            Assert.AreEqual("0a8b59e5-bb3b-4081-b963-9dc9584dcda6", obj.category_id);
            Assert.AreEqual("Accessories", obj.name_category);
        }

        [TestMethod]
        public void GetAdjustmentStockByMonthAndYearTest()
        {
            var month = 1;
            var year = 2017;

            var oList = _bll.GetAdjustmentStockByMonth(month, year);

            var index = 1;
            var obj = oList[index];

            // tes Adjustment
            Assert.IsNotNull(obj);
            Assert.AreEqual("561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac", obj.stock_adjustment_id);
            Assert.AreEqual(new DateTime(2017, 1, 14), obj.date);
            Assert.AreEqual(1, obj.stock_addition);
            Assert.AreEqual(2, obj.stock_reduction);
            Assert.AreEqual(3, obj.warehouse_stock_addition);
            Assert.AreEqual(4, obj.warehouse_stock_reduction);
            Assert.AreEqual("tesss", obj.description);

            // tes product
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);

            // tes reason Adjustment
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", obj.stock_adjustment_reason_id);
            Assert.AreEqual("Transfer stock gudang ke etalase", obj.reason);
        }

        [TestMethod]
        public void GetAdjustmentStockByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 24);

            var oList = _bll.GetAdjustmentStockByDate(tanggalMulai, tanggalSelesai);

            var index = 1;
            var obj = oList[index];

            // tes Adjustment
            Assert.IsNotNull(obj);
            Assert.AreEqual("561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac", obj.stock_adjustment_id);
            Assert.AreEqual(new DateTime(2017, 1, 14), obj.date);
            Assert.AreEqual(1, obj.stock_addition);
            Assert.AreEqual(2, obj.stock_reduction);
            Assert.AreEqual(3, obj.warehouse_stock_addition);
            Assert.AreEqual(4, obj.warehouse_stock_reduction);
            Assert.AreEqual("tesss", obj.description);

            // tes product
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);

            // tes reason Adjustment
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", obj.stock_adjustment_reason_id);
            Assert.AreEqual("Transfer stock gudang ke etalase", obj.reason);
        }
    }
}
