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
    public class AdjustmentStockBllTest
    {
		private ILog _log;
        private IAdjustmentStockBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(AdjustmentStockBllTest));
            _bll = new AdjustmentStockBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac", obj.stock_adjustment_id);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", obj.adjustment_reason_id);
            Assert.AreEqual(new DateTime(2017, 1, 14), obj.date);
            Assert.AreEqual(1, obj.stock_addition);
            Assert.AreEqual(2, obj.stock_reduction);            
            Assert.AreEqual(3, obj.warehouse_stock_addition);
            Assert.AreEqual(4, obj.warehouse_stock_reduction);
            Assert.AreEqual("tesss", obj.description);

            var product = obj.Product;
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", product.product_id);
            Assert.AreEqual("201607000000053", product.product_code);
            Assert.AreEqual("Adaptor NB ACER", product.product_name);

            var alasanAdjustment = obj.ReasonAdjustmentStock;
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", alasanAdjustment.stock_adjustment_reason_id);
            Assert.AreEqual("Transfer stock gudang ke etalase", alasanAdjustment.reason);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "adaptor";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac", obj.stock_adjustment_id);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", obj.adjustment_reason_id);
            Assert.AreEqual(new DateTime(2017, 1, 14), obj.date);
            Assert.AreEqual(1, obj.stock_addition);
            Assert.AreEqual(2, obj.stock_reduction);
            Assert.AreEqual(3, obj.warehouse_stock_addition);
            Assert.AreEqual(4, obj.warehouse_stock_reduction);
            Assert.AreEqual("tesss", obj.description);

            var product = obj.Product;
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", product.product_id);
            Assert.AreEqual("201607000000053", product.product_code);
            Assert.AreEqual("Adaptor NB ACER", product.product_name);

            var alasanAdjustment = obj.ReasonAdjustmentStock;
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", alasanAdjustment.stock_adjustment_reason_id);
            Assert.AreEqual("Transfer stock gudang ke etalase", alasanAdjustment.reason);                              
                     
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var tanggalMulai = new DateTime(2017, 1, 1);
            var tanggalSelesai = new DateTime(2017, 1, 15);

            var index = 1;
            var oList = _bll.GetByDate(tanggalMulai, tanggalSelesai);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac", obj.stock_adjustment_id);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", obj.adjustment_reason_id);
            Assert.AreEqual(new DateTime(2017, 1, 14), obj.date);
            Assert.AreEqual(1, obj.stock_addition);
            Assert.AreEqual(2, obj.stock_reduction);
            Assert.AreEqual(3, obj.warehouse_stock_addition);
            Assert.AreEqual(4, obj.warehouse_stock_reduction);
            Assert.AreEqual("tesss", obj.description);

            var product = obj.Product;
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", product.product_id);
            Assert.AreEqual("201607000000053", product.product_code);
            Assert.AreEqual("Adaptor NB ACER", product.product_name);

            var alasanAdjustment = obj.ReasonAdjustmentStock;
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", alasanAdjustment.stock_adjustment_reason_id);
            Assert.AreEqual("Transfer stock gudang ke etalase", alasanAdjustment.reason);

        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("561f8b6e-9c6a-47d4-b4cd-e1c2cf3552ac", obj.stock_adjustment_id);
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", obj.product_id);
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", obj.adjustment_reason_id);
            Assert.AreEqual(new DateTime(2017, 1, 14), obj.date);
            Assert.AreEqual(1, obj.stock_addition);
            Assert.AreEqual(2, obj.stock_reduction);
            Assert.AreEqual(3, obj.warehouse_stock_addition);
            Assert.AreEqual(4, obj.warehouse_stock_reduction);
            Assert.AreEqual("tesss", obj.description);

            var product = obj.Product;
            Assert.AreEqual("eafc755f-cab6-4066-a793-660fcfab20d0", product.product_id);
            Assert.AreEqual("201607000000053", product.product_code);
            Assert.AreEqual("Adaptor NB ACER", product.product_name);

            var alasanAdjustment = obj.ReasonAdjustmentStock;
            Assert.AreEqual("f9b35798-6725-244f-fec0-fdee38c5ad44", alasanAdjustment.stock_adjustment_reason_id);
            Assert.AreEqual("Transfer stock gudang ke etalase", alasanAdjustment.reason);                               
                     
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new AdjustmentStock
            {
                product_id = "eafc755f-cab6-4066-a793-660fcfab20d0",
                adjustment_reason_id = "f9b35798-6725-244f-fec0-fdee38c5ad44",
                date = DateTime.Today,
                stock_addition = 1,
                stock_reduction = 2,
                description = "tesss",
                warehouse_stock_addition = 3,
                warehouse_stock_reduction = 4
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.stock_adjustment_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.stock_adjustment_id, newObj.stock_adjustment_id);                                
            Assert.AreEqual(obj.product_id, newObj.product_id);                                
            Assert.AreEqual(obj.adjustment_reason_id, newObj.adjustment_reason_id);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.stock_addition, newObj.stock_addition);                                
            Assert.AreEqual(obj.stock_reduction, newObj.stock_reduction);                                
            Assert.AreEqual(obj.description, newObj.description);                                
            Assert.AreEqual(obj.warehouse_stock_addition, newObj.warehouse_stock_addition);                                
            Assert.AreEqual(obj.warehouse_stock_reduction, newObj.warehouse_stock_reduction);                                
            
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new AdjustmentStock
            {
                stock_adjustment_id = "6daeacbb-85a0-4b60-8ead-a77984448110",
                product_id = "53b63dc2-4505-4276-9886-3639b53b7458",
                adjustment_reason_id = "1c23364b-e65d-62ef-4180-b2f3f7f560c1",
                date = new DateTime(2017, 1, 10),
                stock_addition = 5,
                stock_reduction = 4,
                description = "tess description",
                warehouse_stock_addition = 1,
                warehouse_stock_reduction = 5
        	};

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.stock_adjustment_id);
			Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.stock_adjustment_id, updatedObj.stock_adjustment_id);                                
            Assert.AreEqual(obj.product_id, updatedObj.product_id);                                
            Assert.AreEqual(obj.adjustment_reason_id, updatedObj.adjustment_reason_id);                                
            Assert.AreEqual(obj.date, updatedObj.date);                                
            Assert.AreEqual(obj.stock_addition, updatedObj.stock_addition);                                
            Assert.AreEqual(obj.stock_reduction, updatedObj.stock_reduction);                                
            Assert.AreEqual(obj.description, updatedObj.description);                                
            Assert.AreEqual(obj.warehouse_stock_addition, updatedObj.warehouse_stock_addition);                                
            Assert.AreEqual(obj.warehouse_stock_reduction, updatedObj.warehouse_stock_reduction);                                
            
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new AdjustmentStock
            {
                stock_adjustment_id = "2a143e0d-d8c0-4f15-8216-813ec8ddf64c"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.stock_adjustment_id);
			Assert.IsNull(deletedObj);
        }
    }
}     
