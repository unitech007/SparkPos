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

using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SparkPOS.Model;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;

namespace SparkPOS.Bll.Service.UnitTest.Report
{
    [TestClass]
    public class ReportSalaryEmployeeBllTest
    {
        private ILog _log;
        private IReportSalaryEmployeeBll _bll;

        [TestInitialize]
        public void Init()
        {
            _log = LogManager.GetLogger(typeof(ReportSalaryEmployeeBllTest));
            _bll = new ReportSalaryEmployeeBll(_log);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _bll = null;
        }

        [TestMethod]
        public void GetByMonthAndYearTest()
        {
            var month = 3;
            var year = 2017;

            var oList = _bll.GetByMonth(month, year);

            var index = 2;
            var obj = oList[index];

            Assert.IsNotNull(obj);
            Assert.AreEqual("d3506b64-df74-4283-b17a-6c5dbb770e85", obj.employee_id);
            Assert.AreEqual("Doni", obj.employee_name);
            Assert.AreEqual("Cashier", obj.name_job_titles);
            Assert.AreEqual(TypePayment.Weekly, obj.payment_type);
            Assert.AreEqual(new DateTime(2017, 3, 31), obj.date);
            Assert.AreEqual(3, obj.month);
            Assert.AreEqual(2017, obj.year);
            Assert.AreEqual(20, obj.attendance);
            Assert.AreEqual(5, obj.absence);
            Assert.AreEqual(1500000, obj.basic_salary);
            Assert.AreEqual(1000, obj.overtime);
            Assert.AreEqual(150000, obj.bonus);
            Assert.AreEqual(50000, obj.deductions);
            Assert.AreEqual(1, obj.time);
            Assert.AreEqual(6, obj.days_worked);
            Assert.AreEqual(0, obj.allowance);
        }
    }
}
