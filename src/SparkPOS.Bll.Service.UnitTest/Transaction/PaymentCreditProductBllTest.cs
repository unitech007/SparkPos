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
    public class PaymentCreditProductBllTest
    {
		private ILog _log;
        private IPaymentCreditProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(PaymentCreditProductBllTest));
            _bll = new PaymentCreditProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetBySellingIDTest()
        {
            var id = "376625eb-13ba-4620-bc12-e8260501b689";
            var obj = _bll.GetBySellingID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("e547aa43-82db-466e-9701-5936ffa951cc", obj.pay_sale_item_id);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual(500000, obj.amount);
            Assert.AreEqual("description #1", obj.description);
            Assert.AreEqual("tesss", obj.PaymentCreditProduct.description);
            Assert.AreEqual("BP-12345", obj.PaymentCreditProduct.invoice);
            Assert.IsTrue(obj.PaymentCreditProduct.is_cash);

        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 0;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 19);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("10479db1-699e-40d0-b6f9-1f030792ca24", obj.pay_sale_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual(DateTime.Today, obj.date);
            Assert.AreEqual("tesss", obj.description);
            Assert.AreEqual("BP-12345", obj.invoice);
            Assert.IsTrue(obj.is_cash);

            var customer = obj.Customer;
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", customer.name_customer);   
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 0;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("10479db1-699e-40d0-b6f9-1f030792ca24", obj.pay_sale_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);                                
            Assert.AreEqual(DateTime.Today, obj.date);
            Assert.AreEqual("tesss", obj.description);
            Assert.AreEqual("BP-12345", obj.invoice);                                
            Assert.IsTrue(obj.is_cash);

            var customer = obj.Customer;
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", customer.name_customer);                            
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new PaymentCreditProduct
            {
                customer_id = "c7b1ac7f-d201-474f-b018-1dc363d5d7f3",
                user_id = "960a9111-a077-4e0e-a440-cef77293038a",
                date = DateTime.Today,
                description = "tesss",
                invoice = "BP-12345",
                is_cash = true
            };

            var listOfItemPaymentCredit = new List<ItemPaymentCreditProduct>();
            listOfItemPaymentCredit.Add(new ItemPaymentCreditProduct { SellingProduct = new SellingProduct { sale_id = "376625eb-13ba-4620-bc12-e8260501b689" }, sale_id = "376625eb-13ba-4620-bc12-e8260501b689", amount = 500000, description = "description #1" });
            listOfItemPaymentCredit.Add(new ItemPaymentCreditProduct { SellingProduct = new SellingProduct { sale_id = "e4c2c4e7-5236-44ac-98e0-b53171bc2386" }, sale_id = "e4c2c4e7-5236-44ac-98e0-b53171bc2386", amount = 700000, description = "description #2" });

            obj.item_payment_credit = listOfItemPaymentCredit;

            var validationError = new ValidationError();

            var result = _bll.Save(obj, false, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.pay_sale_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.pay_sale_id, newObj.pay_sale_id);
            Assert.AreEqual(obj.customer_id, newObj.customer_id);
            Assert.AreEqual(obj.user_id, newObj.user_id);
            Assert.AreEqual(obj.date, newObj.date);
            Assert.AreEqual(obj.description, newObj.description);
            Assert.AreEqual(obj.invoice, newObj.invoice);
            Assert.AreEqual(obj.is_cash, newObj.is_cash);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("72c0bace-02d0-4c80-91f2-10c08431347e");
            obj.date = new DateTime(2017, 1, 9);
            obj.description = "ket header";
            obj.invoice = "BX-123456";
            obj.is_cash = true;

            var itemPayment = obj.item_payment_credit[1];
            itemPayment.amount = 750000;
            itemPayment.description = "description #22";
            itemPayment.entity_state = EntityState.Modified;

            var validationError = new ValidationError();

            var result = _bll.Update(obj, false, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.pay_sale_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.pay_sale_id, updatedObj.pay_sale_id);
            Assert.AreEqual(obj.customer_id, updatedObj.customer_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.is_cash, updatedObj.is_cash);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new PaymentCreditProduct
            {
                pay_sale_id = "72c0bace-02d0-4c80-91f2-10c08431347e"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.pay_sale_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
