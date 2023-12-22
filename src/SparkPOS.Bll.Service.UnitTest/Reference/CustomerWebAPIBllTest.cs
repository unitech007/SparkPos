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
    public class CustomerWebAPIBllTest
    {
        private ILog _log;
        private ICustomerBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(CustomerWebAPIBllTest));
            _bll = new CustomerBll(true, baseUrl, _log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "a16c67a0-bb93-459c-8765-27cac61e4e83";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("a16c67a0-bb93-459c-8765-27cac61e4e83", obj.customer_id);
            Assert.AreEqual("Callista Distribution JKT", obj.name_customer);
            Assert.AreEqual("Harco Mangga Dua Elektronik Blok F no.10", obj.address);
            Assert.AreEqual("", obj.contact);
            Assert.AreEqual("0813 8176 9915", obj.phone);
            Assert.AreEqual(0, obj.credit_limit);
            Assert.AreEqual(0, obj.total_credit);
            Assert.AreEqual(0, obj.total_receivable_payment);
            Assert.AreEqual("Sleman", obj.subdistrict.name_subdistrict);
            Assert.AreEqual("Condong Catur", obj.village);
            Assert.AreEqual("Yogyakarta", obj.city);
            Assert.AreEqual("28115", obj.postal_code);
            Assert.AreEqual(0, obj.discount);

        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "jkt";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("a16c67a0-bb93-459c-8765-27cac61e4e83", obj.customer_id);
            Assert.AreEqual("Callista Distribution JKT", obj.name_customer);
            Assert.AreEqual("Harco Mangga Dua Elektronik Blok F no.10", obj.address);
            Assert.AreEqual("", obj.contact);
            Assert.AreEqual("0813 8176 9915", obj.phone);
            Assert.AreEqual(0, obj.credit_limit);
            Assert.AreEqual(0, obj.total_credit);
            Assert.AreEqual(0, obj.total_receivable_payment);
            Assert.AreEqual("Sleman", obj.subdistrict.name_subdistrict);
            Assert.AreEqual("Condong Catur", obj.village);
            Assert.AreEqual("Yogyakarta", obj.city);
            Assert.AreEqual("28115", obj.postal_code);
            Assert.AreEqual(0, obj.discount);

        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("a16c67a0-bb93-459c-8765-27cac61e4e83", obj.customer_id);
            Assert.AreEqual("Callista Distribution JKT", obj.name_customer);
            Assert.AreEqual("Harco Mangga Dua Elektronik Blok F no.10", obj.address);
            Assert.AreEqual("", obj.contact);
            Assert.AreEqual("0813 8176 9915", obj.phone);
            Assert.AreEqual(0, obj.credit_limit);
            Assert.AreEqual(0, obj.total_credit);
            Assert.AreEqual(0, obj.total_receivable_payment);
            Assert.AreEqual("Sleman", obj.subdistrict.name_subdistrict);
            Assert.AreEqual("Condong Catur", obj.village);
            Assert.AreEqual("Yogyakarta", obj.city);
            Assert.AreEqual("28115", obj.postal_code);
            Assert.AreEqual(0, obj.discount);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new Customer
            {
                name_customer = "Swalayan WS",
                address = "Jl. Magelang",
                contact = "",
                phone = "08138383838"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.customer_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.customer_id, newObj.customer_id);
            Assert.AreEqual(obj.name_customer, newObj.name_customer);
            Assert.AreEqual(obj.address, newObj.address);
            Assert.AreEqual(obj.contact, newObj.contact);
            Assert.AreEqual(obj.phone, newObj.phone);

        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new Customer
            {
                customer_id = "34881f91-33c6-48a8-a19b-75b01ed36ff2",
                name_customer = "Swalayan WS Medari",
                address = "Jl. Magelang",
                contact = "Mas Adi",
                phone = "0274-4444433"
            };

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.customer_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.customer_id, updatedObj.customer_id);
            Assert.AreEqual(obj.name_customer, updatedObj.name_customer);
            Assert.AreEqual(obj.address, updatedObj.address);
            Assert.AreEqual(obj.contact, updatedObj.contact);
            Assert.AreEqual(obj.phone, updatedObj.phone);

        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new Customer
            {
                customer_id = "34881f91-33c6-48a8-a19b-75b01ed36ff2"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.customer_id);
            Assert.IsNull(deletedObj);
        }
    }
}
