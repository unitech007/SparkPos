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
    public class MenuBllTest
    {
		private ILog _log;
        private IMenuBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(MenuBllTest));
            _bll = new MenuBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "5df78447-219a-47c8-8a28-53b8e71ffb9d";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("5df78447-219a-47c8-8a28-53b8e71ffb9d", obj.menu_id);
            Assert.AreEqual("mnuTrxPurchaseBahanBaku", obj.name_menu);
            Assert.AreEqual("Purchase Bahan Baku", obj.menu_title);
            Assert.AreEqual("cc69c800-5e36-4dc5-9242-4191f1373983", obj.parent_id);
            Assert.AreEqual(1, obj.order_number);
            Assert.IsTrue(obj.is_active);
            Assert.AreEqual("FrmListTransactionsPurchaseBahanBaku", obj.name_form);
            Assert.IsTrue(obj.is_enabled);
                     
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var menuName = "mnuTrxPurchaseBahanBaku";
            
            var obj = _bll.GetByName(menuName);
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("5df78447-219a-47c8-8a28-53b8e71ffb9d", obj.menu_id);
            Assert.AreEqual("mnuTrxPurchaseBahanBaku", obj.name_menu);
            Assert.AreEqual("Purchase Bahan Baku", obj.menu_title);
            Assert.AreEqual("cc69c800-5e36-4dc5-9242-4191f1373983", obj.parent_id);
            Assert.AreEqual(1, obj.order_number);
            Assert.IsTrue(obj.is_active);
            Assert.AreEqual("FrmListTransactionsPurchaseBahanBaku", obj.name_form);
            Assert.IsTrue(obj.is_enabled);                              
                     
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("5df78447-219a-47c8-8a28-53b8e71ffb9d", obj.menu_id);
            Assert.AreEqual("mnuTrxPurchaseBahanBaku", obj.name_menu);
            Assert.AreEqual("Purchase Bahan Baku", obj.menu_title);
            Assert.AreEqual("cc69c800-5e36-4dc5-9242-4191f1373983", obj.parent_id);
            Assert.AreEqual(1, obj.order_number);
            Assert.IsTrue(obj.is_active);
            Assert.AreEqual("FrmListTransactionsPurchaseBahanBaku", obj.name_form);
            Assert.IsTrue(obj.is_enabled);                               
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new MenuApplication
            {
                name_menu = "mnuRefCategory",
                menu_title = "Category",
                parent_id = "5df78447-219a-47c8-8a28-53b8e71ffb9d",
                order_number = 1,
                is_active = true,
                name_form = "FrmCategory",
                is_enabled = true
            };

            var result = _bll.Save(obj);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.menu_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.menu_id, newObj.menu_id);
            Assert.AreEqual(obj.name_menu, newObj.name_menu);
            Assert.AreEqual(obj.menu_title, newObj.menu_title);
            Assert.AreEqual(obj.parent_id, newObj.parent_id);
            Assert.AreEqual(obj.order_number, newObj.order_number);
            Assert.AreEqual(obj.is_active, newObj.is_active);
            Assert.AreEqual(obj.name_form, newObj.name_form);
            Assert.AreEqual(obj.is_enabled, newObj.is_enabled);                                
            
		}
    }
}     
