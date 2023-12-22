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
    public class ReportSellingProductBllTest
    {
        private ILog _log;
        private IReportSellingProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportSellingProductBllTest));
            _bll = new ReportSellingProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void ProductFavouriteGetByMonthAndYearTest()
        {
            var month = 10;
            var year = 2017;
            var limit = 15;

            var oList = _bll.ProductFavouriteGetByMonth(month, year, limit);
            Assert.AreEqual(15, oList.Count);

            var index = 1;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("Access Point TPLINK TL-MR3220 3,5G", obj.product_name);
            Assert.AreEqual(22, obj.quantity);
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
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 21), obj.date);
            Assert.AreEqual(new DateTime(2017, 3, 31), obj.due_date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.discount);
            Assert.AreEqual(1502000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.IsNull(obj.return_sale_id);
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
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 21), obj.date);
            Assert.AreEqual(new DateTime(2017, 3, 31), obj.due_date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.discount);
            Assert.AreEqual(1502000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.IsNull(obj.return_sale_id);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 4, 1);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("201703210056", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 21), obj.date);
            Assert.AreEqual(new DateTime(2017, 3, 31), obj.due_date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.discount);
            Assert.AreEqual(1502000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.IsNull(obj.return_sale_id);
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

            // cek customer
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);

            // cek sale
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual("201703210055", obj.invoice);

            // cek item sale
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);
            Assert.AreEqual(0, obj.discount);
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

            // cek customer
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);

            // cek sale
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual("201703210055", obj.invoice);

            // cek item sale
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);
            Assert.AreEqual(0, obj.discount);
        }

        [TestMethod]
        public void DetailGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 31);

            var oList = _bll.DetailGetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            // cek customer
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("Rudi", obj.name_customer);

            // cek sale
            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual("201703210055", obj.invoice);

            // cek item sale
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);
            Assert.AreEqual(0, obj.discount);
        }

        [TestMethod]
        public void PerProductGetByMonthTest()
        {
            var month = 3;
            var year = 2017;

            var oList = _bll.PerProductGetByMonth(month, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);
            Assert.AreEqual(0, obj.discount);
        }

        [TestMethod]
        public void PerProductGetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 3;
            var year = 2017;

            var oList = _bll.PerProductGetByMonth(StartingMonth, EndingMonth, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);
            Assert.AreEqual(0, obj.discount);
        }

        [TestMethod]
        public void PerProductGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 3, 31);

            var oList = _bll.PerProductGetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual(new DateTime(2017, 3, 2), obj.date);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("Adaptor NB ACER", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(2, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(53000, obj.purchase_price);
            Assert.AreEqual(53000, obj.selling_price);
            Assert.AreEqual(0, obj.discount);
        }
    }
}
