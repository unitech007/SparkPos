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
    public class ProductBllTest
    {
        private ILog _log;
        private IProductBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ProductBllTest));
            _bll = new ProductBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetPriceWholesaleTest()
        {
            var obj = _bll.GetByCode("201607000000053");

            if (obj.list_of_harga_grosir.Count > 0)
            {
                var quantity = 9;
                var hargaWholesale = obj.list_of_harga_grosir
                                     .Where(f => f.product_id == obj.product_id && f.minimum_quantity <= quantity)
                                     .OrderByDescending(f => f.retail_price)
                                     .LastOrDefault();

                Assert.AreEqual(50000, hargaWholesale.wholesale_price);
                Assert.AreEqual(5, hargaWholesale.minimum_quantity);
                Assert.AreEqual(1, hargaWholesale.discount);
            }            
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "17c7626c-e5ca-43f2-b075-af6b6cbcbf83";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("17c7626c-e5ca-43f2-b075-af6b6cbcbf83", obj.product_id);
            Assert.AreEqual("201607000000055", obj.product_code);
            Assert.AreEqual("CD ROM ALL Merk 2nd", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(20000, obj.purchase_price);
            Assert.AreEqual(50000, obj.selling_price);
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", obj.category_id);
            Assert.AreEqual(0, obj.minimal_stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(0, obj.minimal_stock_warehouse);

            var category = obj.Category;
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", category.category_id);
            Assert.AreEqual("Hardward 2nd", category.name_category);                     
        }

        [TestMethod]
        public void GetByCodeProductTest()
        {
            var codeProduct = "201607000000055";
            var obj = _bll.GetByCode(codeProduct);

            Assert.IsNotNull(obj);
            Assert.AreEqual("17c7626c-e5ca-43f2-b075-af6b6cbcbf83", obj.product_id);
            Assert.AreEqual("201607000000055", obj.product_code);
            Assert.AreEqual("CD ROM ALL Merk 2nd", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(20000, obj.purchase_price);
            Assert.AreEqual(50000, obj.selling_price);
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", obj.category_id);
            Assert.AreEqual(0, obj.minimal_stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(0, obj.minimal_stock_warehouse);

            var category = obj.Category;
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", category.category_id);
            Assert.AreEqual("Hardward 2nd", category.name_category);
        }

        [TestMethod]
        public void GetLastCodeProductTest()
        {
            var lastCodeProduct = _bll.GetLastCodeProduct();
            Assert.AreEqual("201701120066", lastCodeProduct);

            lastCodeProduct = _bll.GetLastCodeProduct();
            Assert.AreEqual("201701120067", lastCodeProduct);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "cd";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("17c7626c-e5ca-43f2-b075-af6b6cbcbf83", obj.product_id);
            Assert.AreEqual("201607000000055", obj.product_code);
            Assert.AreEqual("CD ROM ALL Merk 2nd", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(20000, obj.purchase_price);
            Assert.AreEqual(50000, obj.selling_price);
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", obj.category_id);
            Assert.AreEqual(0, obj.minimal_stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(0, obj.minimal_stock_warehouse);

            var category = obj.Category;
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", category.category_id);
            Assert.AreEqual("Hardward 2nd", category.name_category);                             
                     
        }

        [TestMethod]
        public void GetByCategoryTest()
        {
            var golonganId = "2aae21ba-8954-4db6-a6dc-c648e27255ad";

            var index = 0;
            var oList = _bll.GetByCategory(golonganId);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("17c7626c-e5ca-43f2-b075-af6b6cbcbf83", obj.product_id);
            Assert.AreEqual("201607000000055", obj.product_code);
            Assert.AreEqual("CD ROM ALL Merk 2nd", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(20000, obj.purchase_price);
            Assert.AreEqual(50000, obj.selling_price);
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", obj.category_id);
            Assert.AreEqual(0, obj.minimal_stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(0, obj.minimal_stock_warehouse);

            var category = obj.Category;
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", category.category_id);
            Assert.AreEqual("Hardward 2nd", category.name_category);

        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 3;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("17c7626c-e5ca-43f2-b075-af6b6cbcbf83", obj.product_id);
            Assert.AreEqual("201607000000055", obj.product_code);
            Assert.AreEqual("CD ROM ALL Merk 2nd", obj.product_name);
            Assert.AreEqual("", obj.unit);
            Assert.AreEqual(0, obj.stock);
            Assert.AreEqual(20000, obj.purchase_price);
            Assert.AreEqual(50000, obj.selling_price);
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", obj.category_id);
            Assert.AreEqual(0, obj.minimal_stock);
            Assert.AreEqual(0, obj.warehouse_stock);
            Assert.AreEqual(0, obj.minimal_stock_warehouse);

            var category = obj.Category;
            Assert.AreEqual("2aae21ba-8954-4db6-a6dc-c648e27255ad", category.category_id);
            Assert.AreEqual("Hardward 2nd", category.name_category);                                
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var listOfPriceWholesale = new List<PriceWholesale>();
            listOfPriceWholesale.Add(new PriceWholesale { retail_price = 1, wholesale_price = 15000, minimum_quantity = 5, discount = 1 });
            listOfPriceWholesale.Add(new PriceWholesale { retail_price = 2, wholesale_price = 13000, minimum_quantity = 10, discount = 1.5 });
            listOfPriceWholesale.Add(new PriceWholesale { retail_price = 3, wholesale_price = 10000, minimum_quantity = 15, discount = 2.5 });

            var obj = new Product
            {
                product_code = "200111101234",
                product_name = "price dengan grosir",
                unit = "",
                stock = 10,
                minimal_stock = 5,
                purchase_price = 1000000,
                selling_price = 1500000,                
                category_id = "0a8b59e5-bb3b-4081-b963-9dc9584dcda6",                
                warehouse_stock = 15,
                minimal_stock_warehouse = 5
            };
            obj.list_of_harga_grosir = listOfPriceWholesale;

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.product_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.product_id, newObj.product_id);                                
            Assert.AreEqual(obj.product_name, newObj.product_name);                                
            Assert.AreEqual(obj.unit, newObj.unit);                                
            Assert.AreEqual(obj.stock, newObj.stock);                                
            Assert.AreEqual(obj.purchase_price, newObj.purchase_price);                                
            Assert.AreEqual(obj.selling_price, newObj.selling_price);                                
            Assert.AreEqual(obj.product_code, newObj.product_code);                                
            Assert.AreEqual(obj.category_id, newObj.category_id);                                
            Assert.AreEqual(obj.minimal_stock, newObj.minimal_stock);                                
            Assert.AreEqual(obj.warehouse_stock, newObj.warehouse_stock);                                
            Assert.AreEqual(obj.minimal_stock_warehouse, newObj.minimal_stock_warehouse);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("53e03588-b9a2-43df-ae59-283e72917f9a");
            obj.code_produk_old = obj.product_code;

            var index = 0;
            foreach (var item in obj.list_of_harga_grosir)
            {
                obj.list_of_harga_grosir[index].wholesale_price -= 100;
                obj.list_of_harga_grosir[index].minimum_quantity -= 2;
                obj.list_of_harga_grosir[index].discount -= 0.5;

                index++;
            }

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.product_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.product_id, updatedObj.product_id);
            Assert.AreEqual(obj.product_name, updatedObj.product_name);
            Assert.AreEqual(obj.unit, updatedObj.unit);
            Assert.AreEqual(obj.stock, updatedObj.stock);
            Assert.AreEqual(obj.purchase_price, updatedObj.purchase_price);
            Assert.AreEqual(obj.selling_price, updatedObj.selling_price);
            Assert.AreEqual(obj.product_code, updatedObj.product_code);
            Assert.AreEqual(obj.category_id, updatedObj.category_id);
            Assert.AreEqual(obj.minimal_stock, updatedObj.minimal_stock);
            Assert.AreEqual(obj.warehouse_stock, updatedObj.warehouse_stock);
            Assert.AreEqual(obj.minimal_stock_warehouse, updatedObj.minimal_stock_warehouse);                                
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new Product
            {
                product_id = "53e03588-b9a2-43df-ae59-283e72917f9a"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.product_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
