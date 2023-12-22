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
    public class SellingProductBllTest
    {
		private ILog _log;
        private ISellingProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(SellingProductBllTest));
            _bll = new SellingProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetListItemInvoiceTerakhirTest()
        {
            var userId = "00b5acfa-b533-454b-8dfd-e7881edd180f";
            var mesinId = "b0df248f-3cf9-4b28-95ae-1691d4465a80";

            var obj = _bll.GetListItemInvoiceTerakhir(userId, mesinId);

            Assert.IsNotNull(obj);
            Assert.AreEqual("eeb4d5c2-6dc9-49bb-af80-515bcd63d927", obj.sale_id);
            Assert.AreEqual("201710180515", obj.invoice);
            Assert.AreEqual("b0df248f-3cf9-4b28-95ae-1691d4465a80", obj.machine_id);
            Assert.AreEqual("b0df248f-3cf9-4b28-95ae-1691d4465a80", obj.Machine.machine_id);
            Assert.AreEqual(150000, obj.Machine.starting_balance);

            // cek item            
            var index = 1;
            Assert.AreEqual(2, obj.item_jual.Count);
            Assert.AreEqual("11112", obj.item_jual[index].Product.product_code);
            Assert.AreEqual("LAN Tester", obj.item_jual[index].Product.product_name);
        }

        [TestMethod]
        public void GetLastInvoiceTest()
        {
            var lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201701180010", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201701180011", lastInvoice);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "swalayan";

            var index = 1;
            var oList = _bll.GetByName(name);
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);                                
            Assert.AreEqual("12345", obj.invoice);                                
            Assert.AreEqual(DateTime.Today, obj.date);                                
            Assert.IsNull(obj.due_date);                                
            Assert.AreEqual(15000, obj.tax);                                
            Assert.AreEqual(5000, obj.discount);
            Assert.AreEqual(1757200, obj.total_invoice);                                
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("sales cash", obj.description);

            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.Customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.Customer.name_customer);
            Assert.AreEqual("Seturan", obj.Customer.address);

            // tes detail table item sale  
            index = 2;
            Assert.AreEqual(3, obj.item_jual.Count);

            var itemSelling = obj.item_jual[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemSelling.Product.product_id);
            Assert.AreEqual("201607000000051", itemSelling.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemSelling.Product.product_name);

            Assert.AreEqual(70000, itemSelling.selling_price);
            Assert.AreEqual(15, itemSelling.quantity);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("12345", obj.invoice);
            Assert.AreEqual(DateTime.Today, obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(15000, obj.tax);
            Assert.AreEqual(5000, obj.discount);
            Assert.AreEqual(1757200, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("sales cash", obj.description);

            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.Customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.Customer.name_customer);
            Assert.AreEqual("Seturan", obj.Customer.address);

            // tes detail table item sale  
            index = 2;
            Assert.AreEqual(3, obj.item_jual.Count);

            var itemSelling = obj.item_jual[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemSelling.Product.product_id);
            Assert.AreEqual("201607000000051", itemSelling.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemSelling.Product.product_name);

            Assert.AreEqual(70000, itemSelling.selling_price);
            Assert.AreEqual(15, itemSelling.quantity);
                     
        }

        [TestMethod]
        public void GetAllAndNameTest()
        {
            var index = 1;
            var oList = _bll.GetAll("swalayan");
            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("12345", obj.invoice);
            Assert.AreEqual(DateTime.Today, obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(15000, obj.tax);
            Assert.AreEqual(5000, obj.discount);
            Assert.AreEqual(1757200, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("sales cash", obj.description);

            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.Customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.Customer.name_customer);
            Assert.AreEqual("Seturan", obj.Customer.address);

            // tes detail table item sale  
            index = 2;
            Assert.AreEqual(3, obj.item_jual.Count);

            var itemSelling = obj.item_jual[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemSelling.Product.product_id);
            Assert.AreEqual("201607000000051", itemSelling.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemSelling.Product.product_name);

            Assert.AreEqual(70000, itemSelling.selling_price);
            Assert.AreEqual(15, itemSelling.quantity);

        }

        [TestMethod]
        public void GetInvoiceCustomerTest()
        {
            var index = 0;
            var customerId = "c7b1ac7f-d201-474f-b018-1dc363d5d7f3";

            var oList = _bll.GetInvoiceCustomer(customerId, "123");
            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("12345", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 18), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(15000, obj.tax);
            Assert.AreEqual(5000, obj.discount);
            Assert.AreEqual(1757200, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("sales cash", obj.description);

            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.Customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.Customer.name_customer);
            Assert.AreEqual("Seturan", obj.Customer.address);

            // tes detail table item sale  
            index = 2;
            Assert.AreEqual(3, obj.item_jual.Count);

            var itemSelling = obj.item_jual[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemSelling.Product.product_id);
            Assert.AreEqual("201607000000051", itemSelling.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemSelling.Product.product_name);

            Assert.AreEqual(70000, itemSelling.selling_price);
            Assert.AreEqual(15, itemSelling.quantity);

        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 1;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 19);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);
            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("12345", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 18), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(15000, obj.tax);
            Assert.AreEqual(5000, obj.discount);
            Assert.AreEqual(1757200, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("sales cash", obj.description);

            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.Customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.Customer.name_customer);
            Assert.AreEqual("Seturan", obj.Customer.address);

            // tes detail table item sale  
            index = 2;
            Assert.AreEqual(3, obj.item_jual.Count);

            var itemSelling = obj.item_jual[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemSelling.Product.product_id);
            Assert.AreEqual("201607000000051", itemSelling.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemSelling.Product.product_name);

            Assert.AreEqual(70000, itemSelling.selling_price);
            Assert.AreEqual(15, itemSelling.quantity);

        }

        [TestMethod]
        public void GetByDateAndNameTest()
        {
            var index = 0;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 19);
            var name = "swalayan";

            var oList = _bll.GetByDate(tglMulai, tglSelesai, name);
            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("376625eb-13ba-4620-bc12-e8260501b689", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.customer_id);
            Assert.AreEqual("12345", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 18), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(15000, obj.tax);
            Assert.AreEqual(5000, obj.discount);
            Assert.AreEqual(1757200, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("sales cash", obj.description);

            Assert.AreEqual("c7b1ac7f-d201-474f-b018-1dc363d5d7f3", obj.Customer.customer_id);
            Assert.AreEqual("Swalayan Citrouli", obj.Customer.name_customer);
            Assert.AreEqual("Seturan", obj.Customer.address);

            // tes detail table item sale  
            index = 2;
            Assert.AreEqual(3, obj.item_jual.Count);

            var itemSelling = obj.item_jual[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemSelling.Product.product_id);
            Assert.AreEqual("201607000000051", itemSelling.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemSelling.Product.product_name);

            Assert.AreEqual(70000, itemSelling.selling_price);
            Assert.AreEqual(15, itemSelling.quantity);

        }

        [TestMethod]
        public void GetInvoiceKreditByCustomerTest()
        {
            var index = 0;
            var customerId = "576c503f-69a7-46a5-b4be-107c634db7e3";

            var oList = _bll.GetInvoiceKreditByCustomer(customerId, false);
            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("d1dbd28a-592f-4841-bfb6-bc41f48acf32", obj.sale_id);
            Assert.AreEqual("960a9111-a077-4e0e-a440-cef77293038a", obj.user_id);
            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.customer_id);
            Assert.AreEqual("22222", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.AreEqual(new DateTime(2017, 1, 25), obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(3383000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("576c503f-69a7-46a5-b4be-107c634db7e3", obj.Customer.customer_id);
            Assert.AreEqual("Rudi", obj.Customer.name_customer);
            Assert.AreEqual("", obj.Customer.address);

        }

        [TestMethod]
        public void GetItemSellingTest()
        {
            var index = 2;
            var jualId = "d1dbd28a-592f-4841-bfb6-bc41f48acf32";

            var oList = _bll.GetItemSelling(jualId);

            var itemPurchase = oList[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(77000, itemPurchase.selling_price);
            Assert.AreEqual(22, itemPurchase.quantity);

        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new SellingProduct
            {
                user_id = "960a9111-a077-4e0e-a440-cef77293038a",
                customer_id = "c7b1ac7f-d201-474f-b018-1dc363d5d7f3",
                invoice = "12345",
                date = DateTime.Today,
                tax = 15000,
                discount = 5000,
                description = "sales cash"
            };

            var listOfItemSelling = new List<ItemSellingProduct>();
            listOfItemSelling.Add(new ItemSellingProduct { Product = new Product { product_id = "eafc755f-cab6-4066-a793-660fcfab20d0" }, product_id = "eafc755f-cab6-4066-a793-660fcfab20d0", selling_price = 53000, quantity = 5, discount = 2 });
            listOfItemSelling.Add(new ItemSellingProduct { Product = new Product { product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd" }, product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd", selling_price = 50000, quantity = 10, discount = 0 });
            listOfItemSelling.Add(new ItemSellingProduct { Product = new Product { product_id = "7f09a4aa-e660-4de3-a3aa-4b3244675f9f" }, product_id = "7f09a4aa-e660-4de3-a3aa-4b3244675f9f", selling_price = 70000, quantity = 15, discount = 5 });

            obj.item_jual = listOfItemSelling; // menghubungkan sale dan item sale

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.sale_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.sale_id, newObj.sale_id);                                
            Assert.AreEqual(obj.user_id, newObj.user_id);                                
            Assert.AreEqual(obj.customer_id, newObj.customer_id);                                
            Assert.AreEqual(obj.invoice, newObj.invoice);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.due_date, newObj.due_date);                                
            Assert.AreEqual(obj.tax, newObj.tax);                                
            Assert.AreEqual(obj.discount, newObj.discount);                                
            Assert.AreEqual(obj.total_invoice, newObj.total_invoice);                                
            //Assert.AreEqual(obj.total_payment, newObj.total_payment);                                
            Assert.AreEqual(obj.description, newObj.description);

            // tes hasil penyimpanan ke tabel item beli
            Assert.AreEqual(3, newObj.item_jual.Count);

            var index = 0;
            foreach (var itemSelling in newObj.item_jual)
            {
                Assert.AreEqual(listOfItemSelling[index].product_id, itemSelling.product_id);
                Assert.AreEqual(listOfItemSelling[index].selling_price, itemSelling.selling_price);
                Assert.AreEqual(listOfItemSelling[index].quantity, itemSelling.quantity);
                Assert.AreEqual(listOfItemSelling[index].discount, itemSelling.discount);

                index++;
            }
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("d1dbd28a-592f-4841-bfb6-bc41f48acf32");
            obj.invoice = "22222";
            obj.customer_id = "576c503f-69a7-46a5-b4be-107c634db7e3";
            obj.date = new DateTime(2017, 1, 1);
            obj.due_date = new DateTime(2017, 1, 25);
            obj.tax = 20000;
            obj.discount = 7500;
            obj.description = "tesssss";

            foreach (var itemSelling in obj.item_jual)
            {
                itemSelling.quantity = itemSelling.quantity + 1;
                itemSelling.selling_price = itemSelling.selling_price + 1000;
                itemSelling.discount = 0;
                itemSelling.entity_state = EntityState.Modified;
            }

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.sale_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.sale_id, updatedObj.sale_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.customer_id, updatedObj.customer_id);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.due_date, updatedObj.due_date);
            Assert.AreEqual(obj.tax, updatedObj.tax);
            Assert.AreEqual(obj.discount, updatedObj.discount);
            Assert.AreEqual(obj.total_invoice, updatedObj.total_invoice);
            //Assert.AreEqual(obj.total_payment, updatedObj.total_payment);
            Assert.AreEqual(obj.description, updatedObj.description);

            // tes hasil update ke tabel item beli
            Assert.AreEqual(3, updatedObj.item_jual.Count);

            var index = 0;
            foreach (var itemSellingUpdated in updatedObj.item_jual)
            {
                Assert.AreEqual(obj.item_jual[index].product_id, itemSellingUpdated.product_id);
                Assert.AreEqual(obj.item_jual[index].selling_price, itemSellingUpdated.selling_price);
                Assert.AreEqual(obj.item_jual[index].quantity, itemSellingUpdated.quantity);
                Assert.AreEqual(obj.item_jual[index].discount, itemSellingUpdated.discount);

                index++;
            }          
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new SellingProduct
            {
                sale_id = "94208d8c-b4c6-4e32-af37-3eec93f9ebf3"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.sale_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
