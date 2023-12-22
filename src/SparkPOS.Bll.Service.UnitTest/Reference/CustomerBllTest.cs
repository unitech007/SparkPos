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
    public class CustomerBllTest
    {
        private ILog _log;
        private ICustomerBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(CustomerBllTest));
            _bll = new CustomerBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "0b9940b5-33a9-415b-9d44-8ee1d47e706d";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("0b9940b5-33a9-415b-9d44-8ee1d47e706d", obj.customer_id);
            Assert.AreEqual("Swalayan WS", obj.name_customer);
            Assert.AreEqual("Jl. Magelang", obj.address);
            Assert.AreEqual("Mas Adi", obj.contact);
            Assert.AreEqual("0274-4444433", obj.phone);
            Assert.AreEqual(2000000, obj.credit_limit);
            Assert.AreEqual(1500000, obj.total_credit);
            Assert.AreEqual(500000, obj.total_receivable_payment);
                     
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "ws";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("0b9940b5-33a9-415b-9d44-8ee1d47e706d", obj.customer_id);
            Assert.AreEqual("Swalayan WS", obj.name_customer);
            Assert.AreEqual("Jl. Magelang", obj.address);
            Assert.AreEqual("Mas Adi", obj.contact);
            Assert.AreEqual("0274-4444433", obj.phone);
            Assert.AreEqual(2000000, obj.credit_limit);
            Assert.AreEqual(1500000, obj.total_credit);
            Assert.AreEqual(500000, obj.total_receivable_payment);                                                         
                     
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("0b9940b5-33a9-415b-9d44-8ee1d47e706d", obj.customer_id);
            Assert.AreEqual("Swalayan WS", obj.name_customer);
            Assert.AreEqual("Jl. Magelang", obj.address);
            Assert.AreEqual("Mas Adi", obj.contact);
            Assert.AreEqual("0274-4444433", obj.phone);
            Assert.AreEqual(2000000, obj.credit_limit);
            Assert.AreEqual(1500000, obj.total_credit);
            Assert.AreEqual(500000, obj.total_receivable_payment);                          
                     
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
                customer_id = "0b9940b5-33a9-415b-9d44-8ee1d47e706d",
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
                customer_id = "92fa404b-8a5e-4913-9abf-8575adbe2316"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.customer_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
