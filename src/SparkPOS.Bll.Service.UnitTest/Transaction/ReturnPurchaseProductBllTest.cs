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
    public class ReturnPurchaseProductBllTest
    {
		private ILog _log;
        private IReturnPurchaseProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReturnPurchaseProductBllTest));
            _bll = new ReturnPurchaseProductBll(_log);
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
            Assert.AreEqual("201701210002", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201701210003", lastInvoice);
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
            Assert.AreEqual("2fb82570-d64e-4f2d-bd03-aab5bdf75884", obj.purchase_return_id);
            Assert.AreEqual("27d40236-c8ab-44be-bc47-7a9bbd68c31e", obj.purchase_id);
            Assert.AreEqual("af01c916-7976-4518-a563-9d2a1851a912", obj.supplier_id);
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
            Assert.AreEqual(3500, itemReturn.price);

            // tes supplier
            var supplier = obj.Supplier;
            Assert.AreEqual("af01c916-7976-4518-a563-9d2a1851a912", supplier.supplier_id);
            Assert.AreEqual("TE Shop", supplier.name_supplier);
            Assert.AreEqual("Yogyakarta", supplier.address);

            // tes beli
            var beli = obj.PurchaseProduct;
            Assert.AreEqual("27d40236-c8ab-44be-bc47-7a9bbd68c31e", beli.purchase_id);
            Assert.AreEqual("201701200045", beli.invoice);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 0;
            var oList = _bll.GetAll();
            var obj = oList[index];
            
            // tes return     
            Assert.IsNotNull(obj);
            Assert.AreEqual("2fb82570-d64e-4f2d-bd03-aab5bdf75884", obj.purchase_return_id);
            Assert.AreEqual("27d40236-c8ab-44be-bc47-7a9bbd68c31e", obj.purchase_id);
            Assert.AreEqual("af01c916-7976-4518-a563-9d2a1851a912", obj.supplier_id);
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
            Assert.AreEqual(3500, itemReturn.price);

            // tes supplier
            var supplier = obj.Supplier;
            Assert.AreEqual("af01c916-7976-4518-a563-9d2a1851a912", supplier.supplier_id);
            Assert.AreEqual("TE Shop", supplier.name_supplier);
            Assert.AreEqual("Yogyakarta", supplier.address);

            // tes beli
            var beli = obj.PurchaseProduct;
            Assert.AreEqual("27d40236-c8ab-44be-bc47-7a9bbd68c31e", beli.purchase_id);
            Assert.AreEqual("201701200045", beli.invoice);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new ReturnPurchaseProduct
            {
                purchase_id = "27d40236-c8ab-44be-bc47-7a9bbd68c31e",
                supplier_id = "af01c916-7976-4518-a563-9d2a1851a912",
                invoice = _bll.GetLastInvoice(),
                date = DateTime.Today,
                description = "description header"
            };

            var listOfItemPurchase = new List<ItemReturnPurchaseProduct>();
            listOfItemPurchase.Add(new ItemReturnPurchaseProduct { purchase_item_id = "a53a632c-2759-4d85-acc6-0cbb18a0c88b", Product = new Product { product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd" }, product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd", price = 50000, quantity = 4, return_quantity = 2 });
            listOfItemPurchase.Add(new ItemReturnPurchaseProduct { purchase_item_id = "c414c56c-fd01-4e88-bae1-96bfe0f8196a", Product = new Product { product_id = "d7e888eb-6f9b-43ef-9a72-212588d2fb38" }, product_id = "d7e888eb-6f9b-43ef-9a72-212588d2fb38", price = 2500,quantity = 2, return_quantity = 1 });

            obj.item_return = listOfItemPurchase; // menghubungkan return dan item return
            
            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.purchase_return_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.purchase_return_id, newObj.purchase_return_id);                                
            Assert.AreEqual(obj.purchase_id, newObj.purchase_id);                                
            Assert.AreEqual(obj.user_id, newObj.user_id);                                
            Assert.AreEqual(obj.supplier_id, newObj.supplier_id);                                
            Assert.AreEqual(obj.invoice, newObj.invoice);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.description, newObj.description);                                
            Assert.AreEqual(obj.total_invoice, newObj.total_invoice);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("2fb82570-d64e-4f2d-bd03-aab5bdf75884");
            obj.invoice = "201701210010";
            obj.description = "description";

            foreach (var itemReturn in obj.item_return)
            {
                itemReturn.return_quantity += 1;
                itemReturn.price += 1000;
                itemReturn.entity_state = EntityState.Modified;
            }

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.purchase_return_id);
            Assert.IsNotNull(updatedObj);

            Assert.AreEqual(obj.purchase_return_id, updatedObj.purchase_return_id);
            Assert.AreEqual(obj.purchase_id, updatedObj.purchase_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.supplier_id, updatedObj.supplier_id);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.total_invoice, updatedObj.total_invoice);

        }
    }
}     
