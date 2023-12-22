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
using System.Windows.Forms;

using SparkPOS.Model;

namespace SparkPOS.Helper
{
    public static class FillDataHelper
    {
        public static void FillProduct(CheckedListBox chkListbox, IList<Product> listOfProduct, bool isShowSatuan = true)
        {
            chkListbox.Items.Clear();

            foreach (var product in listOfProduct)
            {
                if (isShowSatuan)
                    chkListbox.Items.Add(string.Format("{0} ({1})", product.product_name, product.unit));
                else
                    chkListbox.Items.Add(product.product_name);
            }
        }

        public static void FillSupplier(CheckedListBox chkListbox, IList<Supplier> listOfSupplier)
        {
            chkListbox.Items.Clear();

            foreach (var supplier in listOfSupplier)
            {
                chkListbox.Items.Add(supplier.name_supplier);
            }
        }

        public static void FillSupplier(ComboBox comboBox, IList<Supplier> listOfSupplier)
        {
            comboBox.Items.Clear();

            foreach (var supplier in listOfSupplier)
            {
                comboBox.Items.Add(supplier.name_supplier);
            }
        }

        public static void FillCategory(ComboBox comboBox, IList<Category> listOfCategory)
        {
            comboBox.Items.Clear();

            foreach (var category in listOfCategory)
            {
                comboBox.Items.Add(category.name_category);
            }
        }

        public static void FillCategory(CheckedListBox chkListbox, IList<Category> listOfCategory)
        {
            chkListbox.Items.Clear();

            foreach (var category in listOfCategory)
            {
                chkListbox.Items.Add(category.name_category);
            }
        }

        public static void FillCustomer(CheckedListBox chkListbox, IList<Customer> listOfCustomer)
        {
            chkListbox.Items.Clear();

            foreach (var customer in listOfCustomer)
            {
                chkListbox.Items.Add(customer.name_customer);
            }
        }

        public static void FillEmployee(CheckedListBox chkListbox, IList<Employee> listOfEmployee)
        {
            chkListbox.Items.Clear();

            foreach (var employee in listOfEmployee)
            {
                chkListbox.Items.Add(employee.employee_name);
            }
        }

        public static void FillEmployee(ComboBox cmbBox, IList<Employee> listOfEmployee, bool isClearItem = true)
        {
            if (isClearItem)
                cmbBox.Items.Clear();

            foreach (var employee in listOfEmployee)
            {
                cmbBox.Items.Add(employee.employee_name);
            }
        }

        public static void FillUser(CheckedListBox chkListbox, IList<User> listOfUser)
        {
            chkListbox.Items.Clear();

            foreach (var user in listOfUser)
            {
                chkListbox.Items.Add(user.name_user);
            }
        }

        public static void FillReasonAdjustmentStock(CheckedListBox chkListbox, IList<ReasonAdjustmentStock> listOfReasonAdjustmentStock)
        {
            chkListbox.Items.Clear();

            foreach (var reason in listOfReasonAdjustmentStock)
            {
                chkListbox.Items.Add(reason.reason);
            }
        }

        public static void FillTypeExpenseCost(CheckedListBox chkListbox, IList<TypeExpense> listOfTypeExpenseCost)
        {
            chkListbox.Items.Clear();

            foreach (var typeExpense in listOfTypeExpenseCost)
            {
                chkListbox.Items.Add(typeExpense.name_expense_type);
            }
        }

        public static void FillMonth(ComboBox obj, bool isSetDefaultMonth = false)
        {
            obj.Items.Clear();

            for (int i = 1; i < 13; i++)
            {
                obj.Items.Add(DayMonthHelper.GetMonthIndonesia(i));
            }

            if (isSetDefaultMonth)
                obj.SelectedItem = DayMonthHelper.GetMonthIndonesia(DateTime.Today.Month);
        }

        public static void FillYear(ComboBox obj, bool isSetDefaultYear = false, int startYear = 2015)
        {
            obj.Items.Clear();

            for (int i = startYear; i <= DateTime.Today.Year + 1; i++)
            {
                obj.Items.Add(i.ToString());
            }

            if (isSetDefaultYear)
                obj.SelectedItem = DateTime.Today.Year.ToString();
        }
    }
}
