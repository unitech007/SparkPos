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
    public class ReturnSellingProductBllTest
    {
		private ILog _log;
        private IReturnSellingProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReturnSellingProductBllTest));
            _bll = new ReturnSellingProductBll(_log);
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
            Assert.AreEqual("201701260006", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201701260007", lastInvoice);
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 0;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 21);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);
            var obj = oList[index];

            // tes return     
            Assert.IsNotNull(obj);
            Assert.AreEqual("2fb82570-d64e-4f2d-bd03-aab5bdf75884", obj.return_sale_id);
            Assert.AreEqual("27d40236-c8ab-44be-bc47-7a9bbd68c31e", obj.sale_id);
            Assert.AreEqual("af01c916-7976-4518-a563-9d2a1851a912", obj.customer_id);
            Assert.AreEqual("201701210010", obj.invoice);
            Assert.AreEqual(DateTime.Today, obj.date);
            Assert.AreEqual("description", obj.description);
            Assert.AreEqual(160000, obj.total_invoice);

            // tes item return
            Assert.AreEqual(2, obj.item_return.Count);

            index = 1;
            var itemReturn = obj.item_return[index];
            Assert.AreEqual("d7e888eb-6f9b-43ef-9a72-212588d2fb38", itemReturn.Product.product_id);
            Assert.AreEqual("12345", itemReturn.Product.product_code);
            Assert.AreEqual("susu coklat", itemReturn.Product.product_name);
            Assert.AreEqual(2, itemReturn.return_quantity);
            Assert.AreEqual(3500, itemReturn.selling_price);

            // tes customer
            var customer = obj.Customer;
            Assert.AreEqual("af01c916-7976-4518-a563-9d2a1851a912", customer.customer_id);
            Assert.AreEqual("TE Shop", customer.name_customer);
            Assert.AreEqual("Yogyakarta", customer.address);

            // tes sale
            var sale = obj.SellingProduct;
            Assert.AreEqual("27d40236-c8ab-44be-bc47-7a9bbd68c31e", sale.sale_id);
            Assert.AreEqual("201701200045", sale.invoice);
        }

        [TestMethod]
        public void GetAllTest()
        {            
            var index = 0;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("", obj.return_sale_id);                                
            Assert.AreEqual("", obj.sale_id);                                
            Assert.AreEqual("", obj.user_id);                                
            Assert.AreEqual("", obj.customer_id);                                
            Assert.AreEqual("", obj.invoice);                                
            Assert.AreEqual("", obj.date);                                
            Assert.AreEqual("", obj.description);                                
            Assert.AreEqual("", obj.system_date);                                
            Assert.AreEqual("", obj.total_invoice);                                
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new ReturnSellingProduct
            {
                sale_id = "376625eb-13ba-4620-bc12-e8260501b689",
                customer_id = "c7b1ac7f-d201-474f-b018-1dc363d5d7f3",
                invoice = _bll.GetLastInvoice(),
                date = DateTime.Today,
                description = "description header"
            };

            var listOfItemReturn = new List<ItemReturnSellingProduct>();
            listOfItemReturn.Add(new ItemReturnSellingProduct { sale_item_id = "3db2b20c-2e31-4934-b04a-a77f7ff85419", Product = new Product { product_id = "eafc755f-cab6-4066-a793-660fcfab20d0" }, product_id = "eafc755f-cab6-4066-a793-660fcfab20d0", selling_price = 53000, quantity = 5, return_quantity = 2 });
            listOfItemReturn.Add(new ItemReturnSellingProduct { sale_item_id = "7ea1f32f-b47f-4945-a7ed-3e6da34f5108", Product = new Product { product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd" }, product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd", selling_price = 50000, quantity = 10, return_quantity = 5 });

            obj.item_return = listOfItemReturn; // menghubungkan return dan item return

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.return_sale_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.return_sale_id, newObj.return_sale_id);
            Assert.AreEqual(obj.sale_id, newObj.sale_id);
            Assert.AreEqual(obj.user_id, newObj.user_id);
            Assert.AreEqual(obj.customer_id, newObj.customer_id);
            Assert.AreEqual(obj.invoice, newObj.invoice);
            Assert.AreEqual(obj.date, newObj.date);
            Assert.AreEqual(obj.description, newObj.description);
            Assert.AreEqual(obj.total_invoice, newObj.total_invoice); 

        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("a1597295-c11d-4ea2-b074-e8c6369bf028");
            obj.invoice = "201701260011";
            obj.description = "description header";

            foreach (var itemReturn in obj.item_return)
            {
                itemReturn.return_quantity += 1;
                itemReturn.selling_price += 1000;
                itemReturn.entity_state = EntityState.Modified;
            }

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.return_sale_id);
            Assert.IsNotNull(updatedObj);

            Assert.AreEqual(obj.return_sale_id, updatedObj.return_sale_id);
            Assert.AreEqual(obj.sale_id, updatedObj.sale_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.customer_id, updatedObj.customer_id);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.total_invoice, updatedObj.total_invoice);

        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new ReturnSellingProduct
            {
                return_sale_id = "a1597295-c11d-4ea2-b074-e8c6369bf028"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.return_sale_id);
            Assert.IsNull(deletedObj);
        }
    }
}     
