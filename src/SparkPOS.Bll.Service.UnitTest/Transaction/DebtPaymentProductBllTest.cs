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
    public class DebtPaymentProductBllTest
    {
        private ILog _log;
        private IDebtPaymentProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(DebtPaymentProductBllTest));
            _bll = new DebtPaymentProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByPurchaseIDTest()
        {
            var id = "0983d9b8-7abe-4be2-9383-16607fcfc91a";
            var obj = _bll.GetByPurchaseID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("6d1c2bb2-1e08-4c9c-b740-e12f2e6dbcfa", obj.pay_purchase_item_id);
            Assert.AreEqual("0983d9b8-7abe-4be2-9383-16607fcfc91a", obj.purchase_id);
            Assert.AreEqual(500000, obj.amount);
            Assert.AreEqual("description #1", obj.description);
            Assert.AreEqual("ket header", obj.DebtPaymentProduct.description);
            Assert.AreEqual("BB-123456", obj.DebtPaymentProduct.invoice);
            Assert.IsTrue(obj.DebtPaymentProduct.is_cash);
                     
        }

        [TestMethod]
        public void GetAllTest()
        {            
            var index = 0;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("d4e66a6c-c0b2-49e1-be4d-b33e7b1bd565", obj.pay_purchase_id);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);                                
            Assert.AreEqual(new DateTime(2017, 1, 9), obj.date);
            Assert.AreEqual("ket header", obj.description);                                
            Assert.AreEqual("BB-123456", obj.invoice);                                
            Assert.IsTrue(obj.is_cash);

            var supplier = obj.Supplier;
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", supplier.supplier_id);
            Assert.AreEqual("Pixel Computer", supplier.name_supplier);                     
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 0;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 9);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("d4e66a6c-c0b2-49e1-be4d-b33e7b1bd565", obj.pay_purchase_id);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual(new DateTime(2017, 1, 9), obj.date);
            Assert.AreEqual("ket header", obj.description);
            Assert.AreEqual("BB-123456", obj.invoice);
            Assert.IsTrue(obj.is_cash);

            var supplier = obj.Supplier;
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", supplier.supplier_id);
            Assert.AreEqual("Pixel Computer", supplier.name_supplier);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new DebtPaymentProduct
            {
                supplier_id = "e6201c8e-74e3-467c-a463-c8ea1763668e",
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f",
                date = DateTime.Today,
                description = "description header",
                invoice = "BB-12345",
                is_cash = false
            };

            var listOfItemDebtPayment = new List<ItemDebtPaymentProduct>();
            listOfItemDebtPayment.Add(new ItemDebtPaymentProduct { PurchaseProduct = new PurchaseProduct { purchase_id = "0983d9b8-7abe-4be2-9383-16607fcfc91a" }, purchase_id = "0983d9b8-7abe-4be2-9383-16607fcfc91a", amount = 500000, description = "description #1" });
            listOfItemDebtPayment.Add(new ItemDebtPaymentProduct { PurchaseProduct = new PurchaseProduct { purchase_id = "70c46d69-ca7c-46b2-bd18-ebf03a28d02b" }, purchase_id = "70c46d69-ca7c-46b2-bd18-ebf03a28d02b", amount = 700000, description = "description #2" });

            obj.item_payment_debt = listOfItemDebtPayment;

            var validationError = new ValidationError();

            var result = _bll.Save(obj, false, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.pay_purchase_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.pay_purchase_id, newObj.pay_purchase_id);                                
            Assert.AreEqual(obj.supplier_id, newObj.supplier_id);                                
            Assert.AreEqual(obj.user_id, newObj.user_id);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.description, newObj.description);                                
            Assert.AreEqual(obj.invoice, newObj.invoice);                                
            Assert.AreEqual(obj.is_cash, newObj.is_cash);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("d4e66a6c-c0b2-49e1-be4d-b33e7b1bd565");
            obj.date = new DateTime(2017, 1, 9);
            obj.description = "ket header";
            obj.invoice = "BB-123456";
            obj.is_cash = true;

            var itemPayment = obj.item_payment_debt[1];
            itemPayment.amount = 750000;
            itemPayment.description = "description #22";
            itemPayment.entity_state = EntityState.Modified;

            var validationError = new ValidationError();

            var result = _bll.Update(obj, false, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.pay_purchase_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.pay_purchase_id, updatedObj.pay_purchase_id);
            Assert.AreEqual(obj.supplier_id, updatedObj.supplier_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.is_cash, updatedObj.is_cash);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new DebtPaymentProduct
            {
                pay_purchase_id = "d4e66a6c-c0b2-49e1-be4d-b33e7b1bd565"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.pay_purchase_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
