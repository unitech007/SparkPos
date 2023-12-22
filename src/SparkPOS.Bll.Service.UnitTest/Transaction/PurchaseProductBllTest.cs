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
    public class PurchaseProductBllTest
    {
        private ILog _log;
        private IPurchaseProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(PurchaseProductBllTest));
            _bll = new PurchaseProductBll(_log);
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
            Assert.AreEqual("201701100003", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201701100004", lastInvoice);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "pix";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];
            
            // tes header table beli                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("70c46d69-ca7c-46b2-bd18-ebf03a28d02b", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("22222", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.AreEqual(new DateTime(2017, 1, 25), obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.Supplier.supplier_id);
            Assert.AreEqual("Pixel Computer", obj.Supplier.name_supplier);
            Assert.AreEqual("Solo", obj.Supplier.address);    
            
            // tes detail table item beli  
            index = 2;
            Assert.AreEqual(3, obj.item_beli.Count);

            var itemPurchase = obj.item_beli[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);
                     
        }

        [TestMethod]
        public void GetAllTest()
        {            
            var index = 0;
            var oList = _bll.GetAll();
            var obj = oList[index];

            // tes header table (beli)     
            Assert.IsNotNull(obj);
            Assert.AreEqual("3d4ce868-3f9d-4a6a-88ba-a88ee70ef013", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.supplier_id);                                
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("1234447", obj.invoice);                                
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);                                
            Assert.IsNull(obj.due_date);                                
            Assert.AreEqual(20000, obj.tax);                                
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);                                
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.Supplier.supplier_id);
            Assert.AreEqual("Toko Komputer \"XYZ\"", obj.Supplier.name_supplier);

            // tes detail table (item beli)
            index = 2;
            Assert.AreEqual(3, obj.item_beli.Count);

            var itemPurchase = obj.item_beli[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);
        }

        [TestMethod]
        public void GetAllAndNameTest()
        {
            var index = 0;
            var oList = _bll.GetAll("xyz");
            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("3d4ce868-3f9d-4a6a-88ba-a88ee70ef013", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.supplier_id);
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("1234447", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.Supplier.supplier_id);
            Assert.AreEqual("Toko Komputer \"XYZ\"", obj.Supplier.name_supplier);

            // tes detail table (item beli)
            index = 2;
            Assert.AreEqual(3, obj.item_beli.Count);

            var itemPurchase = obj.item_beli[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);

        }

        [TestMethod]
        public void GetInvoiceSupplierTest()
        {
            var index = 0;
            var supplierId = "7560fd72-0538-4307-8f15-14ef32cf5158";
            var oList = _bll.GetInvoiceSupplier(supplierId, "123");

            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("3d4ce868-3f9d-4a6a-88ba-a88ee70ef013", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.supplier_id);
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("1234447", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.Supplier.supplier_id);
            Assert.AreEqual("Toko Komputer \"XYZ\"", obj.Supplier.name_supplier);

            // tes detail table (item beli)
            index = 2;
            Assert.AreEqual(3, obj.item_beli.Count);

            var itemPurchase = obj.item_beli[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);

        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 0;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 10);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);

            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("3d4ce868-3f9d-4a6a-88ba-a88ee70ef013", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.supplier_id);
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("1234447", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.Supplier.supplier_id);
            Assert.AreEqual("Toko Komputer \"XYZ\"", obj.Supplier.name_supplier);

            // tes detail table (item beli)
            index = 2;
            Assert.AreEqual(3, obj.item_beli.Count);

            var itemPurchase = obj.item_beli[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);

        }

        [TestMethod]
        public void GetByDateAndNameTest()
        {
            var index = 0;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 1, 10);
            var name = "komputer";

            var oList = _bll.GetByDate(tglMulai, tglSelesai, name);

            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("3d4ce868-3f9d-4a6a-88ba-a88ee70ef013", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.supplier_id);
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("1234447", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.IsNull(obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("7560fd72-0538-4307-8f15-14ef32cf5158", obj.Supplier.supplier_id);
            Assert.AreEqual("Toko Komputer \"XYZ\"", obj.Supplier.name_supplier);

            // tes detail table (item beli)
            index = 2;
            Assert.AreEqual(3, obj.item_beli.Count);

            var itemPurchase = obj.item_beli[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);

        }

        [TestMethod]
        public void GetInvoiceKreditBySupplierTest()
        {
            var index = 0;
            var supplierId = "e6201c8e-74e3-467c-a463-c8ea1763668e";
            var oList = _bll.GetInvoiceKreditBySupplier(supplierId, false);

            var obj = oList[index];

            // tes header table (beli)
            Assert.IsNotNull(obj);
            Assert.AreEqual("70c46d69-ca7c-46b2-bd18-ebf03a28d02b", obj.purchase_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.supplier_id);
            Assert.IsNull(obj.purchase_return_id);
            Assert.AreEqual("22222", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 1, 1), obj.date);
            Assert.AreEqual(new DateTime(2017, 1, 25), obj.due_date);
            Assert.AreEqual(20000, obj.tax);
            Assert.AreEqual(7500, obj.discount);
            Assert.AreEqual(2021000, obj.total_invoice);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tesssss", obj.description);

            Assert.AreEqual("e6201c8e-74e3-467c-a463-c8ea1763668e", obj.Supplier.supplier_id);
            Assert.AreEqual("Pixel Computer", obj.Supplier.name_supplier);
            Assert.AreEqual("Solo", obj.Supplier.address);
        }

        [TestMethod]
        public void GetItemPurchaseTest()
        {
            var index = 2;
            var beliId = "70c46d69-ca7c-46b2-bd18-ebf03a28d02b";

            var oList = _bll.GetItemPurchase(beliId);

            var itemPurchase = oList[index];
            Assert.AreEqual("7f09a4aa-e660-4de3-a3aa-4b3244675f9f", itemPurchase.Product.product_id);
            Assert.AreEqual("201607000000051", itemPurchase.Product.product_code);
            Assert.AreEqual("Access Point TPLINK TC-WA 500G", itemPurchase.Product.product_name);

            Assert.AreEqual(71000, itemPurchase.price);
            Assert.AreEqual(16, itemPurchase.quantity);

        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new PurchaseProduct
            {
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f",
                supplier_id = "e6201c8e-74e3-467c-a463-c8ea1763668e",
                invoice = "12345",
                date = DateTime.Today,
                tax = 15000,
                discount = 5000,
                description = "Purchase cash"
            };

            var listOfItemPurchase = new List<ItemPurchaseProduct>();
            listOfItemPurchase.Add(new ItemPurchaseProduct { Product = new Product { product_id = "eafc755f-cab6-4066-a793-660fcfab20d0" }, product_id = "eafc755f-cab6-4066-a793-660fcfab20d0", price = 53000, quantity = 5, discount = 2 });
            listOfItemPurchase.Add(new ItemPurchaseProduct { Product = new Product { product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd" }, product_id = "6e587b32-9d87-4ec3-8e7c-ce15c7b0aecd", price = 50000, quantity = 10, discount = 0 });
            listOfItemPurchase.Add(new ItemPurchaseProduct { Product = new Product { product_id = "7f09a4aa-e660-4de3-a3aa-4b3244675f9f" }, product_id = "7f09a4aa-e660-4de3-a3aa-4b3244675f9f", price = 70000, quantity = 15, discount = 5 });

            obj.item_beli = listOfItemPurchase; // menghubungkan beli dan item beli

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            // tes hasil penyimpanan ke tabel beli
            var newObj = _bll.GetByID(obj.purchase_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.purchase_id, newObj.purchase_id);                                
            Assert.AreEqual(obj.user_id, newObj.user_id);                                
            Assert.AreEqual(obj.supplier_id, newObj.supplier_id);                                
            Assert.AreEqual(obj.invoice, newObj.invoice);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.due_date, newObj.due_date);                                
            Assert.AreEqual(obj.tax, newObj.tax);                                
            Assert.AreEqual(obj.discount, newObj.discount);                                
            Assert.AreEqual(obj.total_invoice, newObj.total_invoice);                                
            //Assert.AreEqual(obj.total_payment, newObj.total_payment);                                
            Assert.AreEqual(obj.description, newObj.description);

            // tes hasil penyimpanan ke tabel item beli
            Assert.AreEqual(3, newObj.item_beli.Count);

            var index = 0;
            foreach (var itemPurchase in newObj.item_beli)
            {
                Assert.AreEqual(listOfItemPurchase[index].product_id, itemPurchase.product_id);
                Assert.AreEqual(listOfItemPurchase[index].price, itemPurchase.price);
                Assert.AreEqual(listOfItemPurchase[index].quantity, itemPurchase.quantity);
                Assert.AreEqual(listOfItemPurchase[index].discount, itemPurchase.discount);

                index++;
            }
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("70c46d69-ca7c-46b2-bd18-ebf03a28d02b");
            obj.invoice = "22222";
            obj.date = new DateTime(2017, 1, 1);
            obj.due_date = new DateTime(2017, 1, 25);
            obj.tax = 20000;
            obj.discount = 7500;
            obj.description = "tesssss";

            foreach (var itemPurchase in obj.item_beli)
            {                
                itemPurchase.quantity = itemPurchase.quantity + 1;
                itemPurchase.price = itemPurchase.price + 1000;
                itemPurchase.discount = 0;
                itemPurchase.entity_state = EntityState.Modified;
            }

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.purchase_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.purchase_id, updatedObj.purchase_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.supplier_id, updatedObj.supplier_id);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.due_date, updatedObj.due_date);
            Assert.AreEqual(obj.tax, updatedObj.tax);
            Assert.AreEqual(obj.discount, updatedObj.discount);
            Assert.AreEqual(obj.total_invoice, updatedObj.total_invoice);
            //Assert.AreEqual(obj.total_payment, newObj.total_payment);                                
            Assert.AreEqual(obj.description, updatedObj.description);

            // tes hasil update ke tabel item beli
            Assert.AreEqual(3, updatedObj.item_beli.Count);

            var index = 0;
            foreach (var itemPurchaseUpdated in updatedObj.item_beli)
            {
                Assert.AreEqual(obj.item_beli[index].product_id, itemPurchaseUpdated.product_id);
                Assert.AreEqual(obj.item_beli[index].price, itemPurchaseUpdated.price);
                Assert.AreEqual(obj.item_beli[index].quantity, itemPurchaseUpdated.quantity);
                Assert.AreEqual(obj.item_beli[index].discount, itemPurchaseUpdated.discount);

                index++;
            }                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new PurchaseProduct
            {
                purchase_id = "9fdd5459-f9cb-4361-bce7-7edd32f4eb13"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.purchase_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
