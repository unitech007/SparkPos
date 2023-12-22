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
    public class LoanBllTest
    {
		private ILog _log;
        private ILoanBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(LoanBllTest));
            _bll = new LoanBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByDateTest()
        {
            var index = 1;
            var tglMulai = new DateTime(2017, 1, 1);
            var tglSelesai = new DateTime(2017, 4, 19);

            var oList = _bll.GetByDate(tglMulai, tglSelesai);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("5f913b90-6ff2-4210-878d-ed4b2717050e", obj.loan_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("201703270003", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);
            Assert.AreEqual(10000, obj.amount);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tessss", obj.description);

            var employee = obj.Employee;
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", obj.employee_id);
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", employee.employee_id);
            Assert.AreEqual("Doni", employee.employee_name);
        }

        [TestMethod]
        public void GetByEmployeeIdTest()
        {
            var index = 1;
            var employeeId = "0e0251ab-7c99-40fc-85ec-7974eeebc800";

            var oList = _bll.GetByEmployeeId(employeeId);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("7f4d4eed-6039-478c-b23d-3e48ebc350bb", obj.loan_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("201703280009", obj.invoice);
            Assert.AreEqual(new DateTime(2017, 3, 29), obj.date);
            Assert.AreEqual(200000, obj.amount);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("Optional loan", obj.description);

            var employee = obj.Employee;
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", obj.employee_id);
            Assert.AreEqual("0e0251ab-7c99-40fc-85ec-7974eeebc800", employee.employee_id);
            Assert.AreEqual("Andi", employee.employee_name);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 1;
            var oList = _bll.GetAll();
            var obj = oList[index];
                 
            Assert.IsNotNull(obj);
            Assert.AreEqual("5f913b90-6ff2-4210-878d-ed4b2717050e", obj.loan_id);
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual("201703270003", obj.invoice);                                
            Assert.AreEqual(new DateTime(2017, 3, 27), obj.date);                                
            Assert.AreEqual(10000, obj.amount);
            Assert.AreEqual(0, obj.total_payment);
            Assert.AreEqual("tessss", obj.description);
            
            var employee = obj.Employee;
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", obj.employee_id);
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", employee.employee_id);
            Assert.AreEqual("Doni", employee.employee_name);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new loan
            {
                employee_id = "d3506b64-df74-4283-b17a-6c5dbb770e85",
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f",
                invoice = _bll.GetLastInvoice(),
                date = DateTime.Today,
                amount = 10000,
                description = "tessss"
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.loan_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.loan_id, newObj.loan_id);
            Assert.AreEqual(obj.employee_id, newObj.employee_id);
            Assert.AreEqual(obj.invoice, newObj.invoice);
            Assert.AreEqual(obj.date, newObj.date);
            Assert.AreEqual(obj.amount, newObj.amount);
            Assert.AreEqual(obj.description, newObj.description);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("28b3f9e3-0504-4b7e-b9d5-a8984a2d0c81");
            obj.employee_id = "72f28a4f-f364-423a-a09b-2b9571543fde";
            obj.invoice = "12345";
            obj.date = new DateTime(2017, 3, 20);
            obj.amount = 250000;
            obj.description = "tess lagi";

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.loan_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.loan_id, updatedObj.loan_id);
            Assert.AreEqual(obj.employee_id, updatedObj.employee_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.amount, updatedObj.amount);
            Assert.AreEqual(obj.description, updatedObj.description);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new loan
            {
                loan_id = "034f800f-5dd6-4173-82cf-88f3879aedb9"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.loan_id);
            Assert.IsNull(deletedObj);
        }
    }
}     
