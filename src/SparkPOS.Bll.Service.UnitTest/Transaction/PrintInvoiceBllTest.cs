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
using System.Threading.Tasks;
using System.Reflection;

using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;

namespace SparkPOS.Bll.Service.UnitTest
{
    [TestClass]
    public class PrintInvoiceBllTest
    {
        private ILog _log;
        private IPrintInvoiceBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(PrintInvoiceBllTest));
            _bll = new PrintInvoiceBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetInvoicePurchaseTest()
        {
            var beliProductId = "6f59a7de-70d0-4aeb-8d8a-042041290a3f";

            var index = 2;
            var oList = _bll.GetInvoicePurchase(beliProductId);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("Sigma komputer", obj.name_supplier);
            Assert.AreEqual("Yogyakarta", obj.address);
            Assert.AreEqual("201701310073", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 31), obj.date);
            Assert.AreEqual(new DateTime(2017, 2, 4), obj.due_date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.diskon_nota);
            Assert.AreEqual(980000, obj.total_invoice);
            Assert.AreEqual("201607000000055", obj.product_code);
            Assert.AreEqual("CD ROM ALL Merk 2nd", obj.product_name);
            Assert.AreEqual(20000, obj.price);
            Assert.AreEqual(5, obj.quantity);
            Assert.AreEqual(0, obj.return_quantity);
            Assert.AreEqual(0, obj.discount);
        }

        [TestMethod]
        public void GetInvoiceSalesTest()
        {
            var jualProductId = "422ae0ed-41c2-4b9c-8f5d-63a24cd7363d";

            var index = 2;
            var oList = _bll.GetInvoiceSales(jualProductId);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("Swalayan Citrouli", obj.name_customer);
            Assert.AreEqual("Seturan", obj.address);
            Assert.AreEqual("201703210056", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 21), obj.date);
            Assert.AreEqual(new DateTime(2017, 3, 31), obj.due_date);
            Assert.AreEqual(0, obj.tax);
            Assert.AreEqual(0, obj.diskon_nota);
            Assert.AreEqual(1046000, obj.total_invoice);
            Assert.AreEqual("201607000000052", obj.product_code);
            Assert.AreEqual("Access Point TPLINK TL-MR3220 3,5G", obj.product_name);
            Assert.AreEqual(350000, obj.price);
            Assert.AreEqual(3, obj.quantity);
            Assert.AreEqual(1, obj.return_quantity);
            Assert.AreEqual(0, obj.discount);
        }
    }
}
