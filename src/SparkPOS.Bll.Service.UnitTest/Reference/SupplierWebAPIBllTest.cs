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
    public class SupplierWebAPIBllTest
    {
        private ILog _log;
        private ISupplierBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(SupplierWebAPIBllTest));
            _bll = new SupplierBll(true, baseUrl, _log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "cc793c38-f5d0-408e-bb1a-1b98953340b6";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("cc793c38-f5d0-408e-bb1a-1b98953340b6", obj.supplier_id);
            Assert.AreEqual("Aneka Komputer", obj.name_supplier);
            Assert.AreEqual("Solo", obj.address);
            Assert.AreEqual("", obj.contact);
            Assert.AreEqual("", obj.phone);
            Assert.AreEqual(470000, obj.total_debt);
            Assert.AreEqual(0, obj.total_debt_payment);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "kom";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("cc793c38-f5d0-408e-bb1a-1b98953340b6", obj.supplier_id);
            Assert.AreEqual("Aneka Komputer", obj.name_supplier);
            Assert.AreEqual("Solo", obj.address);
            Assert.AreEqual("", obj.contact);
            Assert.AreEqual("", obj.phone);
            Assert.AreEqual(470000, obj.total_debt);
            Assert.AreEqual(0, obj.total_debt_payment);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("cc793c38-f5d0-408e-bb1a-1b98953340b6", obj.supplier_id);
            Assert.AreEqual("Aneka Komputer", obj.name_supplier);
            Assert.AreEqual("Solo", obj.address);
            Assert.AreEqual("", obj.contact);
            Assert.AreEqual("", obj.phone);
            Assert.AreEqual(470000, obj.total_debt);
            Assert.AreEqual(0, obj.total_debt_payment);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new Supplier
            {
                name_supplier = "Pixel Computer",
                address = "Yogyakarta",
                contact = "Rudi",
                phone = "08138383838"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.supplier_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.supplier_id, newObj.supplier_id);
            Assert.AreEqual(obj.name_supplier, newObj.name_supplier);
            Assert.AreEqual(obj.address, newObj.address);
            Assert.AreEqual(obj.contact, newObj.contact);
            Assert.AreEqual(obj.phone, newObj.phone);

        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new Supplier
            {
                supplier_id = "5b3704f5-83a4-4b98-a7a7-1b09fd62d356",
                name_supplier = "Sigma Computer",
                address = "Sleman - Yogyakarta",
                contact = "Andi",
                phone = ""
            };

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.supplier_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.supplier_id, updatedObj.supplier_id);
            Assert.AreEqual(obj.name_supplier, updatedObj.name_supplier);
            Assert.AreEqual(obj.address, updatedObj.address);
            Assert.AreEqual(obj.contact, updatedObj.contact);
            Assert.AreEqual(obj.phone, updatedObj.phone);

        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new Supplier
            {
                supplier_id = "5b3704f5-83a4-4b98-a7a7-1b09fd62d356"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.supplier_id);
            Assert.IsNull(deletedObj);
        }
    }
}
