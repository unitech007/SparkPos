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
    public class TitlesBllTest
    {
        private ILog _log;
        private ITitlesBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(TitlesBllTest));
            _bll = new TitlesBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "edb47227-da98-4d97-bff2-b7ee41ff3400";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("edb47227-da98-4d97-bff2-b7ee41ff3400", obj.job_titles_id);
            Assert.AreEqual("Owner", obj.name_job_titles);
            Assert.AreEqual("Pemilik toko", obj.description);
                     
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("edb47227-da98-4d97-bff2-b7ee41ff3400", obj.job_titles_id);
            Assert.AreEqual("Owner", obj.name_job_titles);
            Assert.AreEqual("Pemilik toko", obj.description);                           
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new job_titles
            {
                name_job_titles = "Warehouse",
                description = "Bagian Warehouse"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.job_titles_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.job_titles_id, newObj.job_titles_id);                                
            Assert.AreEqual(obj.name_job_titles, newObj.name_job_titles);                                
            Assert.AreEqual(obj.description, newObj.description);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new job_titles
            {
                job_titles_id = "a60c6afb-ea16-467d-98eb-d724c33fc578",
                name_job_titles = "KA Warehouse",
                description = "Kepala Warehouse"
        	};

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.job_titles_id);
			Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.job_titles_id, updatedObj.job_titles_id);                                
            Assert.AreEqual(obj.name_job_titles, updatedObj.name_job_titles);                                
            Assert.AreEqual(obj.description, updatedObj.description);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new job_titles
            {
                job_titles_id = "a60c6afb-ea16-467d-98eb-d724c33fc578"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.job_titles_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
