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
using SparkPOS.Model;
using SparkPOS.Model.Report;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Api.Report;

namespace SparkPOS.Repository.Service.Report
{
    public class ReportCashierMachineRepository : IReportCashierMachineRepository
    {
        private IDapperContext _context;
        private ILog _log;

        public ReportCashierMachineRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportCashierMachine> PerCashierGetByUserId(string userId)
        {
            IList<ReportCashierMachine> oList = new List<ReportCashierMachine>();

            try
            {
                var sql = @"SELECT t_machine.machine_id, t_machine.date, t_machine.starting_balance, t_machine.system_date,
                            m_user.user_id, m_user.name_user
                            FROM public.t_machine INNER JOIN public.m_user ON t_machine.user_id = m_user.user_id
                            WHERE t_machine.date = CURRENT_DATE AND m_user.user_id = @userId
                            ORDER BY t_machine.system_date";

                oList = _context.db.Query<ReportCashierMachine, User, ReportCashierMachine>(sql, (m, p) =>
                {
                    m.user_id = p.user_id; m.User = p;
                    return m;
                }, new { userId }, splitOn: "user_id").ToList();


                foreach (var item in oList)
                {
                    item.sale = GetSelling(item.machine_id);

                    if (item.sale != null)
                        item.item_jual = GetItemSelling(item.machine_id);
                }

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private ReportSalesProductHeader GetSelling(string mesinId)
        {            
            ReportSalesProductHeader obj = null;

            try
            {
                var sql = @"SELECT SUM(t_product_sales.tax) AS tax, SUM(t_product_sales.discount) AS discount, SUM(t_product_sales.total_invoice) AS total_invoice
                            FROM public.t_machine INNER JOIN public.t_product_sales ON t_product_sales.machine_id= t_machine.machine_id
                            WHERE t_machine.date = CURRENT_DATE AND t_machine.machine_id= @mesinId";
                obj = _context.db.QuerySingleOrDefault<ReportSalesProductHeader>(sql, new { mesinId });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        private IList<ReportSalesProduct> GetItemSelling(string mesinId)
        {
            IList<ReportSalesProduct> oList = new List<ReportSalesProduct>();

            try
            {
                var sql = @"SELECT m_product.product_id, m_product.product_name, t_sales_order_item.selling_price, t_sales_order_item.discount,
                            SUM(t_sales_order_item.quantity - t_sales_order_item.return_quantity) AS quantity
                            FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id 
                            INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                            WHERE t_product_sales.machine_id= @mesinId
                            GROUP BY m_product.product_id, m_product.product_name, t_sales_order_item.selling_price, t_sales_order_item.discount
                            ORDER BY m_product.product_name";
                oList = _context.db.Query<ReportSalesProduct>(sql, new { mesinId }).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
