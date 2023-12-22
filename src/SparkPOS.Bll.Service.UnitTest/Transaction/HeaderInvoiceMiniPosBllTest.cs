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
    public class HeaderInvoiceMiniPosBllTest
    {
        private IHeaderInvoiceMiniPosBll _bll;

        [TestInitialize]
        public void Init()
        {
            _bll = new HeaderInvoiceMiniPosBll();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("089d446a-fd39-cf69-7ce7-d734bf845a16", obj.header_invoice_id);
            Assert.AreEqual("NPWP: 1234567890", obj.description);                                
            Assert.AreEqual(3, obj.order_number);
            Assert.IsTrue(obj.is_active);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var listOfHeaderInvoice = _bll.GetAll();
            Assert.AreEqual(5, listOfHeaderInvoice.Count);

            var headerInvoice = listOfHeaderInvoice[1];
            headerInvoice.description = "Jl. Wonosari Km 10";

            var validationError = new ValidationError();

            var result = _bll.Update(headerInvoice, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);                              
        }
    }
}     
