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
    public class ProfilBllTest
    {
		private ILog _log;
        private IProfilBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ProfilBllTest));
            _bll = new ProfilBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetProfilTest()
        {
            var obj = _bll.GetProfil();

            Assert.IsNotNull(obj);
            Assert.AreEqual("4874663b-e365-4905-bb2c-e8a716577ade", obj.profile_id);
            Assert.AreEqual("PT. XYZ", obj.name_profile);
            Assert.AreEqual("Ringroad Utara", obj.address);
            Assert.AreEqual("Yogyakarta", obj.city);
            Assert.AreEqual("0274-123456789", obj.phone);
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new Profil
            {
                name_profile = "PT. XYZ",
                address = "Ringroad Utara",
                city = "Yogyakarta",
                phone = "0274-123456789"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetProfil();
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.profile_id, newObj.profile_id);
            Assert.AreEqual(obj.name_profile, newObj.name_profile);
            Assert.AreEqual(obj.address, newObj.address);
            Assert.AreEqual(obj.city, newObj.city);
            Assert.AreEqual(obj.phone, newObj.phone);

        }
    }
}     
