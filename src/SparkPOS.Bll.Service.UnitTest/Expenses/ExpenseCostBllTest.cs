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
    public class ExpenseCostBllTest
    {
        private ILog _log;
        private IExpenseCostBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ExpenseCostBllTest));
            _bll = new ExpenseCostBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetLastInvoiceTest()
        {
            var lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201703250001", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201703250002", lastInvoice);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("6b93603b-7cac-470e-8772-e138afc53dab", obj.expense_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("22222", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual(1088000, obj.total);
            Assert.AreEqual("description", obj.description);

            // tes detail expense
            index = 2;
            Assert.AreEqual(3, obj.item_expense_cost.Count);

            var itemExpense = obj.item_expense_cost[index];
            Assert.AreEqual("75277a68-15cd-454f-87f7-ad68dea87b19", itemExpense.expense_item_id);
            Assert.AreEqual("2d921654-2646-4e38-b09c-d691a40469b4", itemExpense.TypeExpense.expense_type_id);
            Assert.AreEqual("Cost Alat Tulis Kantor", itemExpense.TypeExpense.name_expense_type);

            Assert.AreEqual(31000, itemExpense.price);
            Assert.AreEqual(16, itemExpense.quantity);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 1;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 3, 27);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("6b93603b-7cac-470e-8772-e138afc53dab", obj.expense_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("22222", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual(1088000, obj.total);
            Assert.AreEqual("description", obj.description);

            // tes detail expense
            index = 2;
            Assert.AreEqual(3, obj.item_expense_cost.Count);

            var itemExpense = obj.item_expense_cost[index];
            Assert.AreEqual("75277a68-15cd-454f-87f7-ad68dea87b19", itemExpense.expense_item_id);
            Assert.AreEqual("2d921654-2646-4e38-b09c-d691a40469b4", itemExpense.TypeExpense.expense_type_id);
            Assert.AreEqual("Cost Alat Tulis Kantor", itemExpense.TypeExpense.name_expense_type);

            Assert.AreEqual(31000, itemExpense.price);
            Assert.AreEqual(16, itemExpense.quantity);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new ExpenseCost
            {
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f",
                invoice = _bll.GetLastInvoice(),
                date = DateTime.Today,
                description = "tes description"
            };
            
            var listOfItemExpense = new List<ItemExpenseCost>();
            listOfItemExpense.Add(new ItemExpenseCost { TypeExpense = new TypeExpense { expense_type_id = "7fde2c41-5187-4fe9-a274-b96ad8e79451" }, expense_type_id = "7fde2c41-5187-4fe9-a274-b96ad8e79451", user_id = obj.user_id, price = 50000, quantity = 5 });
            listOfItemExpense.Add(new ItemExpenseCost { TypeExpense = new TypeExpense { expense_type_id = "b7968f37-5a92-4ea3-bff0-2909aed18d9d" }, expense_type_id = "b7968f37-5a92-4ea3-bff0-2909aed18d9d", user_id = obj.user_id, price = 25000, quantity = 10 });
            listOfItemExpense.Add(new ItemExpenseCost { TypeExpense = new TypeExpense { expense_type_id = "2d921654-2646-4e38-b09c-d691a40469b4" }, expense_type_id = "2d921654-2646-4e38-b09c-d691a40469b4", user_id = obj.user_id, price = 30000, quantity = 15 });

            obj.item_expense_cost = listOfItemExpense; // menghubungkan sale dan item sale

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.expense_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.expense_id, newObj.expense_id);
            Assert.AreEqual(obj.user_id, newObj.user_id);
            Assert.AreEqual(obj.invoice, newObj.invoice);
            Assert.AreEqual(obj.date, newObj.date);
            Assert.AreEqual(obj.total, newObj.total);
            Assert.AreEqual(obj.description, newObj.description);

            var index = 0;
            foreach (var itemExpense in newObj.item_expense_cost)
            {
                Assert.AreEqual(listOfItemExpense[index].expense_type_id, itemExpense.expense_type_id);
                Assert.AreEqual(listOfItemExpense[index].price, itemExpense.price);
                Assert.AreEqual(listOfItemExpense[index].quantity, itemExpense.quantity);

                index++;
            }
        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("0a6ca3a2-bfdc-4707-99b4-5f10326d8c75");
            obj.invoice = "11111";
            obj.date = new DateTime(2017, 3, 27);
            obj.description = "description";

            foreach (var itemExpense in obj.item_expense_cost)
            {
                itemExpense.quantity = itemExpense.quantity + 2;
                itemExpense.price = itemExpense.price + 1500;
                itemExpense.entity_state = EntityState.Modified;
            }

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.expense_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.expense_id, updatedObj.expense_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.total, updatedObj.total);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.system_date, updatedObj.system_date);

            var index = 0;
            foreach (var itemSellingUpdated in updatedObj.item_expense_cost)
            {
                Assert.AreEqual(obj.item_expense_cost[index].price, itemSellingUpdated.price);
                Assert.AreEqual(obj.item_expense_cost[index].quantity, itemSellingUpdated.quantity);

                index++;
            }
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new ExpenseCost
            {
                expense_id = "25683f15-251a-4d64-9956-d20d07b2f732"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.expense_id);
            Assert.IsNull(deletedObj);
        }
    }
}
