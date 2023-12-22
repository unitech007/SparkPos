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
    public class SettingApplicationBllTest
    {
        private ISettingApplicationBll _bll;

        [TestInitialize]
        public void Init()
        {
            _bll = new SettingApplicationBll();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "1ef7cbcb-bf73-44be-aad2-b9d8e1b1a178";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("1ef7cbcb-bf73-44be-aad2-b9d8e1b1a178", obj.application_setting_id);
            Assert.IsTrue(obj.is_update_selling_price_of_master_products);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 0;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("1ef7cbcb-bf73-44be-aad2-b9d8e1b1a178", obj.application_setting_id);
            Assert.IsTrue(obj.is_update_selling_price_of_master_products);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new SettingApplication
            {
                application_setting_id = "1ef7cbcb-bf73-44be-aad2-b9d8e1b1a178",
                is_update_selling_price_of_master_products = true
            };

            var result = _bll.Update(obj);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.application_setting_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.application_setting_id, updatedObj.application_setting_id);
            Assert.IsTrue(obj.is_update_selling_price_of_master_products);
        }
    }
}
