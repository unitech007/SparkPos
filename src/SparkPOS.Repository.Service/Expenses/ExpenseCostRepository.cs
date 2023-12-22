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

using log4net;
using Dapper;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class ExpenseCostRepository : IExpenseCostRepository
    {
        private IDapperContext _context;
		private ILog _log;
		
        public ExpenseCostRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ItemExpenseCost> GetItemExpenseCost(string expenseCostId)
        {
            IList<ItemExpenseCost> oList = new List<ItemExpenseCost>();

            try
            {
                var sql = @"SELECT t_expense_item.expense_item_id, t_expense_item.expense_id, t_expense_item.user_id, 
                            t_expense_item.quantity, t_expense_item.price, 1 as entity_state,
                            m_expense_type.expense_type_id, m_expense_type.name_expense_type
                            FROM public.t_expense_item INNER JOIN public.m_expense_type ON t_expense_item.expense_type_id = m_expense_type.expense_type_id
                            WHERE t_expense_item.expense_id = @expenseCostId
                            ORDER BY t_expense_item.system_date";

                oList = _context.db.Query<ItemExpenseCost, TypeExpense, ItemExpenseCost>(sql, (ip, jp) =>
                {
                    ip.expense_type_id = jp.expense_type_id; ip.TypeExpense = jp;
                    return ip;
                }, new { expenseCostId }, splitOn: "expense_type_id").ToList();
            }
            catch
            {
            }

            return oList;
        }

        public ExpenseCost GetByID(string id)
        {
            ExpenseCost obj = null;

            try
            {
                obj = _context.db.Get<ExpenseCost>(id);

                // load item expense
                if (obj != null)
                    obj.item_expense_cost = GetItemExpenseCost(obj.expense_id).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<ExpenseCost> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ExpenseCost> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ExpenseCost> oList = new List<ExpenseCost>();

            try
            {
                oList = _context.db.GetAll<ExpenseCost>()
                                .Where(f => f.date >= tanggalMulai && f.date <= tanggalSelesai)
                                .OrderBy(f => f.date)
                                .ToList();

                // load item expense
                foreach (var item in oList)
                {
                    item.item_expense_cost = GetItemExpenseCost(item.expense_id).ToList();
                }

            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ExpenseCost> GetAll()
        {
            IList<ExpenseCost> oList = new List<ExpenseCost>();

            try
            {
                oList = _context.db.GetAll<ExpenseCost>()
                                .OrderBy(f => f.date)
                                .ToList();

                // load item expense
                foreach (var item in oList)
                {
                    item.item_expense_cost = GetItemExpenseCost(item.expense_id).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private double GetTotalInvoice(ExpenseCost obj)
        {
            var total = obj.item_expense_cost.Where(f => f.TypeExpense != null && f.entity_state != EntityState.Deleted)
                                                  .Sum(f => f.quantity * f.price);

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        public int Save(ExpenseCost obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                if (obj.expense_id == null)
                    obj.expense_id = _context.GetGUID();

                obj.total = GetTotalInvoice(obj);

                // insert header
                _context.db.Insert<ExpenseCost>(obj, transaction);

                // insert detail
                foreach (var item in obj.item_expense_cost.Where(f => f.TypeExpense != null))
                {
                    if (item.expense_type_id.Length > 0)
                    {
                        if (item.expense_item_id == null)
                            item.expense_item_id = _context.GetGUID();

                        item.expense_id = obj.expense_id;
                        item.user_id = obj.user_id;
                        
                        _context.db.Insert<ItemExpenseCost>(item, transaction);

                        // update entity state
                        item.entity_state = EntityState.Unchanged;
                    }
                }

                _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Add data");

                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(ExpenseCost obj)
        {
            var result = 0;

            try
            {
                _context.BeginTransaction();

                var transaction = _context.transaction;

                obj.total = GetTotalInvoice(obj);
                
                // update header
                result = _context.db.Update<ExpenseCost>(obj, transaction) ? 1 : 0;

                // delete detail
                foreach (var item in obj.item_expense_cost_deleted)
                {
                    result = _context.db.Delete<ItemExpenseCost>(item, transaction) ? 1 : 0;
                }

                // insert/update detail
                foreach (var item in obj.item_expense_cost.Where(f => f.TypeExpense != null))
                {
                    item.expense_id = obj.expense_id;
                    item.user_id = obj.user_id;

                    if (item.entity_state == EntityState.Added)
                    {
                        if (item.expense_item_id == null)
                            item.expense_item_id = _context.GetGUID();

                        _context.db.Insert<ItemExpenseCost>(item, transaction);

                        result = 1;
                    }
                    else if (item.entity_state == EntityState.Modified)
                    {
                        result = _context.db.Update<ItemExpenseCost>(item, transaction) ? 1 : 0;
                    }

                    // update entity state
                    item.entity_state = EntityState.Unchanged;
                }

                _context.Commit();

                LogicalThreadContext.Properties["NewValue"] = obj.ToJson();
                _log.Info("Update data");

                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(ExpenseCost obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<ExpenseCost>(obj) ? 1 : 0;

                if (result > 0)
                {
                    LogicalThreadContext.Properties["OldValue"] = obj.ToJson();
                    _log.Info("Delete data");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public string GetLastInvoice()
        {
            return _context.GetLastInvoice(new ExpenseCost().GetTableName());
        }
    }
}     
