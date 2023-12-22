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
    public class CategoryBllTest
    {
        private ILog _log;
        private ICategoryBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(CategoryBllTest));
            _bll = new CategoryBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "6ae85958-80c6-4f3a-bc01-53a715e25bf1";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("6ae85958-80c6-4f3a-bc01-53a715e25bf1", obj.category_id);
            Assert.AreEqual("Hardware new", obj.name_category);
                     
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "hard";

            var index = 1;
            var oList = _bll.GetByName(name);
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("6ae85958-80c6-4f3a-bc01-53a715e25bf1", obj.category_id);
            Assert.AreEqual("Hardware new", obj.name_category);                                               
                     
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("6ae85958-80c6-4f3a-bc01-53a715e25bf1", obj.category_id);
            Assert.AreEqual("Hardware new", obj.name_category);                    
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new Category
            {
                name_category = "other"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.category_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.category_id, newObj.category_id);                                
            Assert.AreEqual(obj.name_category, newObj.name_category);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new Category
            {
                category_id = "4758c3ec-e931-40d5-a903-75b7dc48bee1",
                name_category = "Lain-lain"
        	};

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.category_id);
			Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.category_id, updatedObj.category_id);                                
            Assert.AreEqual(obj.name_category, updatedObj.name_category);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new Category
            {
                category_id = "4758c3ec-e931-40d5-a903-75b7dc48bee1"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.category_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
