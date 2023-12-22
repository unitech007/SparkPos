﻿/**
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
    public class ExpenseCostWebAPIBllTest
    {
        private ILog _log;
        private IExpenseCostBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(ExpenseCostWebAPIBllTest));
            _bll = new ExpenseCostBll(true, baseUrl, _log);
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
            Assert.AreEqual("201706120033", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201706120034", lastInvoice);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("bfa85912-b32f-4846-bc8a-747811f5350a", obj.expense_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("201703270019", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual(2210000, obj.total);
            Assert.AreEqual("", obj.description);

            // tes detail expense
            index = 1;
            Assert.AreEqual(2, obj.item_expense_cost.Count);

            var itemExpense = obj.item_expense_cost[index];
            Assert.AreEqual("3b926134-93e7-4e28-aa14-6d601f7b66db", itemExpense.expense_item_id);
            Assert.AreEqual("c2116c49-a940-4385-be94-302470b67b83", itemExpense.TypeExpense.expense_type_id);
            Assert.AreEqual("Cost Penyusutan Kendaraan", itemExpense.TypeExpense.name_expense_type);

            Assert.AreEqual(2000000, itemExpense.price);
            Assert.AreEqual(1, itemExpense.quantity);
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
            Assert.AreEqual("bfa85912-b32f-4846-bc8a-747811f5350a", obj.expense_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("201703270019", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual(2210000, obj.total);
            Assert.AreEqual("", obj.description);

            // tes detail expense
            index = 1;
            Assert.AreEqual(2, obj.item_expense_cost.Count);

            var itemExpense = obj.item_expense_cost[index];
            Assert.AreEqual("3b926134-93e7-4e28-aa14-6d601f7b66db", itemExpense.expense_item_id);
            Assert.AreEqual("c2116c49-a940-4385-be94-302470b67b83", itemExpense.TypeExpense.expense_type_id);
            Assert.AreEqual("Cost Penyusutan Kendaraan", itemExpense.TypeExpense.name_expense_type);

            Assert.AreEqual(2000000, itemExpense.price);
            Assert.AreEqual(1, itemExpense.quantity);
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
            listOfItemExpense.Add(new ItemExpenseCost { TypeExpense = new TypeExpense { expense_type_id = "6c262064-6453-4bea-9e0f-5ae1810d0557" }, expense_type_id = "6c262064-6453-4bea-9e0f-5ae1810d0557", user_id = obj.user_id, price = 50000, quantity = 5 });
            listOfItemExpense.Add(new ItemExpenseCost { TypeExpense = new TypeExpense { expense_type_id = "c2116c49-a940-4385-be94-302470b67b83" }, expense_type_id = "c2116c49-a940-4385-be94-302470b67b83", user_id = obj.user_id, price = 25000, quantity = 10 });
            listOfItemExpense.Add(new ItemExpenseCost { TypeExpense = new TypeExpense { expense_type_id = "2cc2ae56-dc3b-4991-af56-7768ae10816a" }, expense_type_id = "2cc2ae56-dc3b-4991-af56-7768ae10816a", user_id = obj.user_id, price = 30000, quantity = 15 });

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
                expense_id = "5b7fb72b-5cea-407f-8654-986de89e1cf9"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.expense_id);
            Assert.IsNull(deletedObj);
        }
    }
}
