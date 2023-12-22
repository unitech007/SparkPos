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
    public class TypeExpenseWebAPIBllTest
    {
        private ILog _log;
        private ITypeExpenseBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(TypeExpenseWebAPIBllTest));
            _bll = new TypeExpenseBll(true, baseUrl, _log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "1619f95e-eac3-40e9-ba8a-af2230d3c470";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("1619f95e-eac3-40e9-ba8a-af2230d3c470", obj.expense_type_id);
            Assert.AreEqual("Cost Lain Lain Sales", obj.name_expense_type);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 4;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("1619f95e-eac3-40e9-ba8a-af2230d3c470", obj.expense_type_id);
            Assert.AreEqual("Cost Lain Lain Sales", obj.name_expense_type);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new TypeExpense
            {
                name_expense_type = "Pay Listrik Baru"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.expense_type_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.expense_type_id, newObj.expense_type_id);
            Assert.AreEqual(obj.name_expense_type, newObj.name_expense_type);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new TypeExpense
            {
                expense_type_id = "26b062ad-7469-42c4-9326-bb84feeca746",
                name_expense_type = "Cost Listrik Naik"
            };

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.expense_type_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.expense_type_id, updatedObj.expense_type_id);
            Assert.AreEqual(obj.name_expense_type, updatedObj.name_expense_type);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new TypeExpense
            {
                expense_type_id = "f896b0f7-9675-4dc2-bf03-06e1dc6977d1"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.expense_type_id);
            Assert.IsNull(deletedObj);
        }
    }
}
