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
    public class SalaryEmployeeWebAPIBllTest
    {
		private ILog _log;
        private ISalaryEmployeeBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(SalaryEmployeeWebAPIBllTest));
            _bll = new SalaryEmployeeBll(true, baseUrl, _log);
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
            Assert.AreEqual("201703310001", lastInvoice);

            lastInvoice = _bll.GetLastInvoice();
            Assert.AreEqual("201703310002", lastInvoice);
        }

        [TestMethod]
        public void GetByMonthAndYearTest()
        {
            var month = 3;
            var year = 2017;

            var index = 1;
            var oList = _bll.GetByMonthAndYear(month, year);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("229d712c-a1c5-45e4-be20-2c07bff86406", obj.gaji_employee_id);            
            Assert.AreEqual("00b5acfa-b533-454b-8dfd-e7881edd180f", obj.user_id);
            Assert.AreEqual(3, obj.month);
            Assert.AreEqual(2017, obj.year);
            Assert.AreEqual(20, obj.attendance);
            Assert.AreEqual(5, obj.absence);
            Assert.AreEqual(1500000, obj.basic_salary);
            Assert.AreEqual(1000, obj.overtime);
            Assert.AreEqual(150000, obj.bonus);
            Assert.AreEqual(50000, obj.deductions);
            Assert.AreEqual(1, obj.time);
            Assert.AreEqual(0, obj.other);
            Assert.AreEqual("tesss lagi", obj.description);
            Assert.AreEqual(6, obj.days_worked);
            Assert.AreEqual(0, obj.allowance);
            Assert.AreEqual(new DateTime(2017, 3, 31), obj.date);
            Assert.AreEqual("201703310004", obj.invoice);

            // tes cek data employee
            var employee = obj.Employee;
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", obj.employee_id);
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", employee.employee_id);
            Assert.AreEqual("Doni", employee.employee_name);

            // tes cek data job_titles
            var job_titles = employee.job_titles;
            Assert.AreEqual("120d3472-ea93-4e29-8abd-5bd7044d26db", job_titles.job_titles_id);
            Assert.AreEqual("Cashier", job_titles.name_job_titles);
        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new SalaryEmployee
            {
                employee_id = "72f28a4f-f364-423a-a09b-2b9571543fde",
                user_id = "00b5acfa-b533-454b-8dfd-e7881edd180f",
                date = DateTime.Today,
                invoice = _bll.GetLastInvoice(),
                month = 3,
                year = 2017,
                attendance = 24,
                absence = 1,
                basic_salary = 3000000,
                overtime = 0,
                bonus = 0,
                deductions = 0,
                time = 1,
                description = "tesss",
                days_worked = 6,
                allowance = 0             
            };

            // item payment loan
            var listOfPaymentLoan = new List<PaymentLoan>();

            var paymentLoan1 = new PaymentLoan
            {
                loan_id = "d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab",
                loan = new loan { loan_id = "d6ba5c9e-b0ba-40ba-9dc8-f631fc499aab" },
                amount = 600000,
                description = "Payment dari gaji"
            };

            var paymentLoan2 = new PaymentLoan
            {
                loan_id = "89a3fbb2-441c-4043-b858-755e112cd997",
                loan = new loan { loan_id = "89a3fbb2-441c-4043-b858-755e112cd997" },
                amount = 100000,
                description = "Payment dari gaji"
            };

            listOfPaymentLoan.Add(paymentLoan1);
            listOfPaymentLoan.Add(paymentLoan2);
            obj.item_payment_loan = listOfPaymentLoan;

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.gaji_employee_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.gaji_employee_id, newObj.gaji_employee_id);
            Assert.AreEqual(obj.employee_id, newObj.employee_id);
            Assert.AreEqual(obj.user_id, newObj.user_id);
            Assert.AreEqual(obj.date, newObj.date);
            Assert.AreEqual(obj.invoice, newObj.invoice);
            Assert.AreEqual(obj.month, newObj.month);
            Assert.AreEqual(obj.year, newObj.year);
            Assert.AreEqual(obj.attendance, newObj.attendance);
            Assert.AreEqual(obj.absence, newObj.absence);
            Assert.AreEqual(obj.basic_salary, newObj.basic_salary);
            Assert.AreEqual(obj.overtime, newObj.overtime);
            Assert.AreEqual(obj.bonus, newObj.bonus);
            Assert.AreEqual(obj.deductions, newObj.deductions);
            Assert.AreEqual(obj.time, newObj.time);
            Assert.AreEqual(obj.other, newObj.other);
            Assert.AreEqual(obj.description, newObj.description);
            Assert.AreEqual(obj.days_worked, newObj.days_worked);
            Assert.AreEqual(obj.allowance, newObj.allowance);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = _bll.GetByID("229d712c-a1c5-45e4-be20-2c07bff86406");
            obj.employee_id = "d3506b64-df74-4283-b17a-6c5dbb770e85";
            obj.attendance = 20;
            obj.absence = 5;
            obj.basic_salary = 1500000;
            obj.overtime = 1000;
            obj.bonus = 150000;
            obj.deductions = 50000;
            obj.description = "tesss lagi";

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.gaji_employee_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.gaji_employee_id, updatedObj.gaji_employee_id);
            Assert.AreEqual(obj.employee_id, updatedObj.employee_id);
            Assert.AreEqual(obj.user_id, updatedObj.user_id);
            Assert.AreEqual(obj.date, updatedObj.date);
            Assert.AreEqual(obj.invoice, updatedObj.invoice);
            Assert.AreEqual(obj.month, updatedObj.month);
            Assert.AreEqual(obj.year, updatedObj.year);
            Assert.AreEqual(obj.attendance, updatedObj.attendance);
            Assert.AreEqual(obj.absence, updatedObj.absence);
            Assert.AreEqual(obj.basic_salary, updatedObj.basic_salary);
            Assert.AreEqual(obj.overtime, updatedObj.overtime);
            Assert.AreEqual(obj.bonus, updatedObj.bonus);
            Assert.AreEqual(obj.deductions, updatedObj.deductions);
            Assert.AreEqual(obj.time, updatedObj.time);
            Assert.AreEqual(obj.other, updatedObj.other);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.days_worked, updatedObj.days_worked);
            Assert.AreEqual(obj.allowance, updatedObj.allowance);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new SalaryEmployee
            {
                gaji_employee_id = "a86d7da1-e0a0-4f4e-896c-5d6d7be8be8f"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.gaji_employee_id);
            Assert.IsNull(deletedObj);
        }
    }
}     
