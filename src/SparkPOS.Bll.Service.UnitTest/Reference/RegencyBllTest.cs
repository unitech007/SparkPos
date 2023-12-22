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
    public class RegencyBllTest
    {
		private ILog _log;
        private IRegencyShippingCostsByRajaBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(RegencyBllTest));
            _bll = new RegencyShippingCostsByRajaBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "aceh";

            var index = 4;
            var oList = _bll.GetByName(name);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual(5, obj.regency_id);
            Assert.AreEqual("Regency", obj.type);
            Assert.AreEqual("Aceh Selatan", obj.name_regency);
            Assert.AreEqual("23719", obj.postal_code);

            var provinsi = obj.Provinsi;
            Assert.AreEqual(21, obj.province_id);
            Assert.AreEqual(21, provinsi.province_id);
            Assert.AreEqual("Nanggroe Aceh Darussalam (NAD)", provinsi.name_province);                                
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 4;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual(5, obj.regency_id);            
            Assert.AreEqual("Regency", obj.type);
            Assert.AreEqual("Aceh Selatan", obj.name_regency);
            Assert.AreEqual("23719", obj.postal_code);

            var provinsi = obj.Provinsi;
            Assert.AreEqual(21, obj.province_id);
            Assert.AreEqual(21, provinsi.province_id);
            Assert.AreEqual("Nanggroe Aceh Darussalam (NAD)", provinsi.name_province);                                
        }
    }
}     
