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
    public class DropshipperBllTest
    {
		private ILog _log;
        private IDropshipperBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(DropshipperBllTest));
            _bll = new DropshipperBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "maypes";
            
            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("23649ae7-3435-4a5e-81d6-f7697a5b3775", obj.dropshipper_id);
            Assert.AreEqual("Madelena Maypes", obj.name_dropshipper);
            Assert.AreEqual("2 Nancy Place", obj.address);
            Assert.AreEqual("Maypes", obj.contact);
            Assert.AreEqual("566-561-8188", obj.phone);                                 
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("23649ae7-3435-4a5e-81d6-f7697a5b3775", obj.dropshipper_id);
            Assert.AreEqual("Madelena Maypes", obj.name_dropshipper);
            Assert.AreEqual("2 Nancy Place", obj.address);
            Assert.AreEqual("Maypes", obj.contact);
            Assert.AreEqual("566-561-8188", obj.phone);                                
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new Dropshipper
            {
                name_dropshipper = "Lacie Kaesmans",
                address = "110 Mosinee Hill",
                contact = "Lacie",
                phone = "212-367-3754"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.dropshipper_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.dropshipper_id, newObj.dropshipper_id);                                
            Assert.AreEqual(obj.name_dropshipper, newObj.name_dropshipper);                                
            Assert.AreEqual(obj.address, newObj.address);                                
            Assert.AreEqual(obj.contact, newObj.contact);                                
            Assert.AreEqual(obj.phone, newObj.phone);                                
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("23649ae7-3435-4a5e-81d6-f7697a5b3775");
            obj.name_dropshipper = "Madelena Maypes";
            obj.address = "2 Nancy Place";
            obj.contact = "Maypes";
            obj.phone = "566-561-8188";

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.dropshipper_id);
			Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.dropshipper_id, updatedObj.dropshipper_id);                                
            Assert.AreEqual(obj.name_dropshipper, updatedObj.name_dropshipper);                                
            Assert.AreEqual(obj.address, updatedObj.address);                                
            Assert.AreEqual(obj.contact, updatedObj.contact);                                
            Assert.AreEqual(obj.phone, updatedObj.phone);                                
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new Dropshipper
            {
                dropshipper_id = "810efc21-9809-4730-b18b-065e7e1b9368"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.dropshipper_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
