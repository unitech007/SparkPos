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
    public class CashierMachineBllTest
    {
		private ILog _log;
        private ICashierMachineBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(CashierMachineBllTest));
            _bll = new CashierMachineBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new CashierMachine
            {
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f",
                date = DateTime.Today,
                starting_balance = 200000
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.machine_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.machine_id, newObj.machine_id);                                
            Assert.AreEqual(obj.user_id, newObj.user_id);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.starting_balance, newObj.starting_balance);                                
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("ebfdae76-2577-4070-aaac-16fffc09d6f5");
            obj.starting_balance = 300000;
            obj.cash_out = 100000;

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.machine_id);
			Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.machine_id, updatedObj.machine_id);                                
            Assert.AreEqual(obj.user_id, updatedObj.user_id);                                
            Assert.AreEqual(obj.date, updatedObj.date);                                
            Assert.AreEqual(obj.starting_balance, updatedObj.starting_balance);                                
            Assert.AreEqual(obj.cash_in, updatedObj.cash_in);                                
            Assert.AreEqual(obj.system_date, updatedObj.system_date);                                
            Assert.AreEqual(obj.shift_id, updatedObj.shift_id);                                
            Assert.AreEqual(obj.cash_out, updatedObj.cash_out);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new CashierMachine
            {
                machine_id= "6870dec2-3f4b-4952-9174-d6d40f254573"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.machine_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
