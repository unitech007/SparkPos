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
    public class ReasonAdjustmentStockWebAPIBllTest
    {
        private ILog _log;
        private IReasonAdjustmentStockBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(ReasonAdjustmentStockWebAPIBllTest));
            _bll = new ReasonAdjustmentStockBll(true, baseUrl, _log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "e4ef2a27-6600-365f-1e07-2963d55cc4bf";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("e4ef2a27-6600-365f-1e07-2963d55cc4bf", obj.stock_adjustment_reason_id);
            Assert.AreEqual("Koreksi (Koreksi karena kesalahan input)", obj.reason);
                     
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("e4ef2a27-6600-365f-1e07-2963d55cc4bf", obj.stock_adjustment_reason_id);
            Assert.AreEqual("Koreksi (Koreksi karena kesalahan input)", obj.reason);                               
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new ReasonAdjustmentStock
            {
                reason = "Dipake sendiri"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.stock_adjustment_reason_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.stock_adjustment_reason_id, newObj.stock_adjustment_reason_id);                                
            Assert.AreEqual(obj.reason, newObj.reason);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new ReasonAdjustmentStock
            {
                stock_adjustment_reason_id = "ab6b9e7d-f0c2-4b49-b257-cf518f7af145",
                reason = "Dipake sendiri (Prive)"
        	};

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.stock_adjustment_reason_id);
			Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.stock_adjustment_reason_id, updatedObj.stock_adjustment_reason_id);                                
            Assert.AreEqual(obj.reason, updatedObj.reason);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new ReasonAdjustmentStock
            {
                stock_adjustment_reason_id = "ab6b9e7d-f0c2-4b49-b257-cf518f7af145"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.stock_adjustment_reason_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
