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
    public class ReportDebtPaymentPurchaseProductBllTest
    {
        private ILog _log;
        private IReportDebtPaymentPurchaseProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportDebtPaymentPurchaseProductBllTest));
            _bll = new ReportDebtPaymentPurchaseProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByMonthAndYearTest()
        {
            var month = 2;
            var year = 2017;

            var oList = _bll.GetByMonth(month, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.AreEqual("Pixel Computer", obj.name_supplier);
            Assert.AreEqual(new DateTime(2017, 2, 4), obj.date);
            Assert.AreEqual(1825000, obj.total_payment);
            Assert.AreEqual("Purchase cash product", obj.description);
        }

        [TestMethod]
        public void GetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 2;
            var year = 2017;

            var oList = _bll.GetByMonth(StartingMonth, EndingMonth, year);

            var index = 5;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.AreEqual("Pixel Computer", obj.name_supplier);
            Assert.AreEqual(new DateTime(2017, 2, 4), obj.date);
            Assert.AreEqual(1825000, obj.total_payment);
            Assert.AreEqual("Purchase cash product", obj.description);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 2, 1);
            var tanggalSelesai = new DateTime(2017, 2, 28);

            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.AreEqual("Pixel Computer", obj.name_supplier);
            Assert.AreEqual(new DateTime(2017, 2, 4), obj.date);
            Assert.AreEqual(1825000, obj.total_payment);
            Assert.AreEqual("Purchase cash product", obj.description);
        }

        [TestMethod]
        public void DetailGetByMonthAndYearTest()
        {
            var month = 2;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(month, year);

            var index = 4;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201702010080", obj.nota_beli);
            Assert.AreEqual("201702010047", obj.nota_pay);
            Assert.AreEqual(new DateTime(2017, 2, 1), obj.date);
            Assert.AreEqual(100000, obj.tax);
            Assert.AreEqual(150000, obj.discount);
            Assert.AreEqual(1651000, obj.total_invoice);
            Assert.AreEqual(1601000, obj.pelunasan);            
            Assert.AreEqual("", obj.keterangan_beli);
            Assert.AreEqual("Purchase cash product", obj.keterangan_pay);            
        }

        [TestMethod]
        public void DetailGetByRangeMonthAndYearTest()
        {
            var StartingMonth = 1;
            var EndingMonth = 2;
            var year = 2017;

            var oList = _bll.DetailGetByMonth(StartingMonth, EndingMonth, year);

            var index = 5;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201702010080", obj.nota_beli);
            Assert.AreEqual("201702010047", obj.nota_pay);
            Assert.AreEqual(new DateTime(2017, 2, 1), obj.date);
            Assert.AreEqual(100000, obj.tax);
            Assert.AreEqual(150000, obj.discount);
            Assert.AreEqual(1651000, obj.total_invoice);
            Assert.AreEqual(1601000, obj.pelunasan);
            Assert.AreEqual("", obj.keterangan_beli);
            Assert.AreEqual("Purchase cash product", obj.keterangan_pay);         
        }

        [TestMethod]
        public void DetailGetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 2, 28);

            var oList = _bll.DetailGetByDate(tanggalMulai, tanggalSelesai);

            var index = 5;
            var obj = oList[index];

            Assert.IsNotNull(obj);

            Assert.AreEqual("85ecb92b-3cb7-4d98-8390-cc76a942b880", obj.supplier_id);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("201702010080", obj.nota_beli);
            Assert.AreEqual("201702010047", obj.nota_pay);
            Assert.AreEqual(new DateTime(2017, 2, 1), obj.date);
            Assert.AreEqual(100000, obj.tax);
            Assert.AreEqual(150000, obj.discount);
            Assert.AreEqual(1651000, obj.total_invoice);
            Assert.AreEqual(1601000, obj.pelunasan);
            Assert.AreEqual("", obj.keterangan_beli);
            Assert.AreEqual("Purchase cash product", obj.keterangan_pay);        
        }
    }
}
