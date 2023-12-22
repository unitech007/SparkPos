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
    public class EmployeeWebAPIBllTest
    {
        private ILog _log;
        private IEmployeeBll _bll;

        [TestInitialize]
        public void Init()
        {
            var baseUrl = "http://localhost/openretail_webapi/";

            _log = LogManager.GetLogger(typeof(EmployeeWebAPIBllTest));
            _bll = new EmployeeBll(true, baseUrl, _log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByIDTest()
        {
            var id = "72f28a4f-f364-423a-a09b-2b9571543fde";
            var obj = _bll.GetByID(id);

            Assert.IsNotNull(obj);
            Assert.AreEqual("72f28a4f-f364-423a-a09b-2b9571543fde", obj.employee_id);
            Assert.AreEqual("f1e4ea09-b777-4e56-bb90-db2bf9211468", obj.job_titles_id);
            Assert.AreEqual("Bangkit", obj.employee_name);
            Assert.AreEqual("Klaten", obj.address);
            Assert.AreEqual("0813283838383", obj.phone);
            Assert.AreEqual(200000, obj.basic_salary);
            Assert.IsTrue(obj.is_active);
            Assert.AreEqual(TypePayment.Weekly, obj.payment_type);
            Assert.AreEqual(60000, obj.overtime_salary);
            Assert.AreEqual(200000, obj.total_loan);
            Assert.AreEqual(50000, obj.total_loan_payment);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            var name = "bang";

            var index = 0;
            var oList = _bll.GetByName(name);
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("72f28a4f-f364-423a-a09b-2b9571543fde", obj.employee_id);
            Assert.AreEqual("f1e4ea09-b777-4e56-bb90-db2bf9211468", obj.job_titles_id);
            Assert.AreEqual("Bangkit", obj.employee_name);
            Assert.AreEqual("Klaten", obj.address);
            Assert.AreEqual("0813283838383", obj.phone);
            Assert.AreEqual(200000, obj.basic_salary);
            Assert.IsTrue(obj.is_active);
            Assert.AreEqual(TypePayment.Weekly, obj.payment_type);
            Assert.AreEqual(60000, obj.overtime_salary);
            Assert.AreEqual(200000, obj.total_loan);
            Assert.AreEqual(50000, obj.total_loan_payment);

        }

        [TestMethod]
        public void GetAllTest()
        {
            var index = 2;
            var oList = _bll.GetAll();
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("72f28a4f-f364-423a-a09b-2b9571543fde", obj.employee_id);
            Assert.AreEqual("f1e4ea09-b777-4e56-bb90-db2bf9211468", obj.job_titles_id);
            Assert.AreEqual("Bangkit", obj.employee_name);
            Assert.AreEqual("Klaten", obj.address);
            Assert.AreEqual("0813283838383", obj.phone);
            Assert.AreEqual(200000, obj.basic_salary);
            Assert.IsTrue(obj.is_active);
            Assert.AreEqual(TypePayment.Weekly, obj.payment_type);
            Assert.AreEqual(60000, obj.overtime_salary);
            Assert.AreEqual(200000, obj.total_loan);
            Assert.AreEqual(50000, obj.total_loan_payment);

        }

        [TestMethod]
        public void SaveTest()
        {
            var obj = new Employee
            {
                job_titles_id = "120d3472-ea93-4e29-8abd-5bd7044d26db",
                employee_name = "Doni",
                address = "Yogyakarta",
                phone = "",
                basic_salary = 100000,
                is_active = true,
                payment_type = TypePayment.Weekly,
                overtime_salary = 50000
            };

            var validationError = new ValidationError();

            var result = _bll.Save(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var newObj = _bll.GetByID(obj.employee_id);
            Assert.IsNotNull(newObj);
            Assert.AreEqual(obj.employee_id, newObj.employee_id);
            Assert.AreEqual(obj.job_titles_id, newObj.job_titles_id);
            Assert.AreEqual(obj.employee_name, newObj.employee_name);
            Assert.AreEqual(obj.address, newObj.address);
            Assert.AreEqual(obj.phone, newObj.phone);
            Assert.AreEqual(obj.basic_salary, newObj.basic_salary);
            Assert.AreEqual(obj.is_active, newObj.is_active);
            Assert.AreEqual(obj.description, newObj.description);
            Assert.AreEqual(obj.payment_type, newObj.payment_type);
            Assert.AreEqual(obj.overtime_salary, newObj.overtime_salary);

        }

        [TestMethod]
        public void UpdateTest()
        {
            var obj = new Employee
            {
                employee_id = "72f28a4f-f364-423a-a09b-2b9571543fde",
                job_titles_id = "f1e4ea09-b777-4e56-bb90-db2bf9211468",
                employee_name = "Bangkit",
                address = "Klaten",
                phone = "0813283838383",
                basic_salary = 200000,
                is_active = true,
                payment_type = TypePayment.Weekly,
                overtime_salary = 60000
            };

            var validationError = new ValidationError();

            var result = _bll.Update(obj, ref validationError);
            Console.WriteLine("Error : " + validationError.Message);

            Assert.IsTrue(result != 0);

            var updatedObj = _bll.GetByID(obj.employee_id);
            Assert.IsNotNull(updatedObj);
            Assert.AreEqual(obj.employee_id, updatedObj.employee_id);
            Assert.AreEqual(obj.job_titles_id, updatedObj.job_titles_id);
            Assert.AreEqual(obj.employee_name, updatedObj.employee_name);
            Assert.AreEqual(obj.address, updatedObj.address);
            Assert.AreEqual(obj.phone, updatedObj.phone);
            Assert.AreEqual(obj.basic_salary, updatedObj.basic_salary);
            Assert.AreEqual(obj.is_active, updatedObj.is_active);
            Assert.AreEqual(obj.description, updatedObj.description);
            Assert.AreEqual(obj.payment_type, updatedObj.payment_type);
            Assert.AreEqual(obj.overtime_salary, updatedObj.overtime_salary);
            Assert.AreEqual(obj.total_loan, updatedObj.total_loan);
            Assert.AreEqual(obj.total_loan_payment, updatedObj.total_loan_payment);

        }

        [TestMethod]
        public void DeleteTest()
        {
            var obj = new Employee
            {
                employee_id = "8b8c9988-e868-40d5-9972-42655cd28618"
            };

            var result = _bll.Delete(obj);
            Assert.IsTrue(result != 0);

            var deletedObj = _bll.GetByID(obj.employee_id);
            Assert.IsNull(deletedObj);
        }
    }
}
