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
using Dapper;
using SparkPOS.Model.Report;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Api.Report;

namespace SparkPOS.Repository.Service.Report
{
    public class ReportRevenueExpenseRepository : IReportRevenueExpenseRepository
    {
        private const string SQL_TEMPLATE_PENJUALAN = @"SELECT SUM(total_invoice - discount + shipping_cost + tax)
                                                        FROM t_product_sales
                                                        {WHERE}";

        private const string SQL_TEMPLATE_purchase = @"SELECT SUM(total_invoice - discount + tax)
                                                        FROM t_purchase_product
                                                        {WHERE}";

        private const string SQL_TEMPLATE_GAJI_KARYAWAN = @"SELECT 
                                                              SUM(CASE
                                                                  WHEN days_worked > 0 THEN days_worked * basic_salary
                                                                  ELSE basic_salary
                                                              END) AS basic_salary,
                                                              SUM(CASE
                                                                  WHEN time > 0 THEN time * overtime
                                                                  ELSE overtime
                                                              END) AS overtime,
                                                            SUM(allowance) AS allowance, SUM(bonus) AS bonus, SUM(deductions) AS deductions
                                                            FROM t_employee_salary
                                                            {WHERE}";

        private const string SQL_TEMPLATE_BEBAN = @"SELECT m_expense_type.name_expense_type AS description, SUM(t_expense_item.quantity * t_expense_item.price) AS quantity
                                                    FROM public.t_expence INNER JOIN public.t_expense_item ON t_expense_item.expense_id = t_expence.expense_id
                                                    INNER JOIN public.m_expense_type ON t_expense_item.expense_type_id = m_expense_type.expense_type_id  
                                                    {WHERE}
                                                    GROUP BY m_expense_type.name_expense_type
                                                    ORDER BY m_expense_type.name_expense_type";

        private IDapperContext _context;
        private ILog _log;

        public ReportRevenueExpenseRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }        

        public ReportRevenueExpense GetByMonth(int month, int year)
        {
            ReportRevenueExpense obj = null;

            try
            {
                obj = new ReportRevenueExpense();
                obj.sales = GetSales(month, year);
                obj.Purchase = GetPurchase(month, year);
                obj.list_of_beban = GetBeban(month, year);

                var gajiEmployee = GetSalaryEmployee(month, year);
                if (gajiEmployee > 0)
                {
                    if (obj.list_of_beban == null)
                    {
                        obj.list_of_beban = new List<ReportBebanUsaha>();
                    }

                    obj.list_of_beban.Add(new ReportBebanUsaha { description = "Cost Salary Employee", quantity = gajiEmployee });
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public ReportRevenueExpense GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            ReportRevenueExpense obj = null;

            try
            {
                obj = new ReportRevenueExpense();
                obj.sales = GetSales(tanggalMulai, tanggalSelesai);
                obj.Purchase = GetPurchase(tanggalMulai, tanggalSelesai);
                obj.list_of_beban = GetBeban(tanggalMulai, tanggalSelesai);

                var gajiEmployee = GetSalaryEmployee(tanggalMulai, tanggalSelesai);
                if (gajiEmployee > 0)
                {
                    if (obj.list_of_beban == null)
                    {
                        obj.list_of_beban = new List<ReportBebanUsaha>();
                    }

                    obj.list_of_beban.Add(new ReportBebanUsaha { description = "Cost Salary Employee", quantity = gajiEmployee });
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        private double GetSales(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PENJUALAN);
                
                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetSales(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PENJUALAN);

                whereBuilder.Add("EXTRACT(MONTH FROM date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetPurchase(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_purchase);

                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetPurchase(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_purchase);

                whereBuilder.Add("EXTRACT(MONTH FROM date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM date) = @year");

                result = _context.db.QuerySingleOrDefault<double>(whereBuilder.ToSql(), new { month, year });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetSalaryEmployee(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_GAJI_KARYAWAN);

                whereBuilder.Add("date BETWEEN @tanggalMulai AND @tanggalSelesai");

                var obj = _context.db.QuerySingleOrDefault<dynamic>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai });

                if (obj != null)
                {
                    result = (double)(obj.basic_salary + obj.allowance + obj.overtime + obj.bonus - obj.deductions);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private double GetSalaryEmployee(int month, int year)
        {
            double result = 0;

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_GAJI_KARYAWAN);

                whereBuilder.Add("month = @month");
                whereBuilder.Add("year = @year");

                var obj = _context.db.QuerySingleOrDefault<dynamic>(whereBuilder.ToSql(), new { month, year });

                if (obj != null)
                {
                    result = (double)(obj.basic_salary + obj.allowance + obj.overtime + obj.bonus - obj.deductions);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        private IList<ReportBebanUsaha> GetBeban(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportBebanUsaha> oList = new List<ReportBebanUsaha>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_BEBAN);

                whereBuilder.Add("t_expence.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportBebanUsaha>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                   .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private IList<ReportBebanUsaha> GetBeban(int month, int year)
        {
            IList<ReportBebanUsaha> oList = new List<ReportBebanUsaha>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_BEBAN);

                whereBuilder.Add("EXTRACT(MONTH FROM t_expence.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_expence.date) = @year");

                oList = _context.db.Query<ReportBebanUsaha>(whereBuilder.ToSql(), new { month, year })
                                   .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
