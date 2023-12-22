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
    public class PaymentLoanBllTest
    {
		private ILog _log;
        private IPaymentLoanBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(PaymentLoanBllTest));
            _bll = new PaymentLoanBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByLoanIdTest()
        {
            var kasbonId = "d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab";

            var index = 1;
            var oList = _bll.GetByLoanId(kasbonId);
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("0b3c6fb9-7410-49a9-b248-cd32a2f6ee48", obj.payment_loan_id);
            Assert.AreEqual("d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab", obj.loan_id);                                
            Assert.IsNull(obj.gaji_employee_id);                                
            Assert.AreEqual(new DateTime(2017, 4, 17), obj.date);                                
            Assert.AreEqual(75000, obj.amount);
            Assert.AreEqual("cicilan ke #dua", obj.description);
            Assert.AreEqual("2017032800021", obj.invoice);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);                                
        }

        [TestMethod]
        public void GetBySalaryEmployeeTest()
        {
            var gajiEmployeeId = "d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab";

            var index = 1;
            var oList = _bll.GetBySalaryEmployee(gajiEmployeeId);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("0b3c6fb9-7410-49a9-b248-cd32a2f6ee48", obj.payment_loan_id);
            Assert.AreEqual("d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab", obj.loan_id);
            Assert.IsNull(obj.gaji_employee_id);
            Assert.AreEqual(new DateTime(2017, 4, 17), obj.date);
            Assert.AreEqual(75000, obj.amount);
            Assert.AreEqual("cicilan ke #dua", obj.description);
            Assert.AreEqual("2017032800021", obj.invoice);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new PaymentLoan
            {
                loan_id = "d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab",
                gaji_employee_id = null,
                date = new DateTime(2017, 4, 15),
                amount = 50000,
                description = "cicilan ke #3",
                invoice = _bll.GetLastInvoice(),
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.payment_loan_id);
			Assert.IsNotNull(newObj);
			Assert.AreEqual(obj.payment_loan_id, newObj.payment_loan_id);                                
            Assert.AreEqual(obj.loan_id, newObj.loan_id);                                
            Assert.AreEqual(obj.gaji_employee_id, newObj.gaji_employee_id);                                
            Assert.AreEqual(obj.date, newObj.date);                                
            Assert.AreEqual(obj.amount, newObj.amount);                                
            Assert.AreEqual(obj.description, newObj.description);                                
            Assert.AreEqual(obj.invoice, newObj.invoice);                                
            Assert.AreEqual(obj.user_id, newObj.user_id);                                
		}

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("0b3c6fb9-7410-49a9-b248-cd32a2f6ee48");
            obj.date = new DateTime(2017, 4, 17);
            obj.amount = 75000;
            obj.description = "cicilan ke #dua";
            obj.invoice = "2017032800021";

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.payment_loan_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.payment_loan_id, updatedObj.payment_loan_id);
            Assert.AreEqual(obj.loan_id, updatedObj.loan_id);
            Assert.AreEqual(obj.gaji_employee_id, updatedObj.gaji_employee_id);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.amount, updatedObj.amount);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new PaymentLoan
            {
                payment_loan_id = "4e646d45-b733-4961-aceb-f53be8e7b242"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.payment_loan_id);
            Assert.IsNull(deletedObj);
        }
    }
}     
