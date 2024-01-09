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
using RestSharp;
using Newtonsoft.Json;
using SparkPOS.Helper;
using SparkPOS.Model;
using SparkPOS.Model.WebAPI;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class ExpenseCostWebAPIRepository : IExpenseCostRepository
    {
        private string _apiUrl = string.Empty;
        private ILog _log;
		
        public ExpenseCostWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/expense_cost/";
            this._log = log;
        }

        public string GetLastInvoice()
        {
            var result = string.Empty;

            try
            {
                var api = "get_last_nota";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<string>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    result = response.Results[0];
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public ExpenseCost GetByID(string id)
        {
            ExpenseCost obj = null;

			try
            {
                var api = string.Format("get_by_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ExpenseCost>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
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
                var api = string.Format("get_by_tanggal?tanggal_mulai={0}&tanggal_selesai={1}", tanggalMulai, tanggalSelesai);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ExpenseCost>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ExpenseCost> GetAll()
        {
            IList<ExpenseCost> oList = new List<ExpenseCost>();

			try
            {
                var api = "get_all";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ExpenseCost>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        private double GetTotalInvoice(ExpenseCost obj)
        {
            var total = obj.item_expense_cost.Where(f => f.TypeExpense != null && f.entity_state != EntityState.Deleted)
                                                  .Sum(f => f.quantity * f.price);

            total = Math.Ceiling(total);
            return total;
        }

        public int Save(ExpenseCost obj)
        {
            var result = 0;

			try
            {
                obj.date = obj.date.ToUtc();

                var api = "save";
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);

                if (result > 0)
                {
                    obj.total = GetTotalInvoice(obj);

                    foreach (var item in obj.item_expense_cost.Where(f => f.TypeExpense != null))
                    {
                        item.entity_state = EntityState.Unchanged;
                    }
                }
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Update(ExpenseCost obj)
        {
            var result = 0;

			try
            {
                obj.date = obj.date.ToUtc();

                var api = "update";
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);

                if (result > 0)
                {
                    obj.total = GetTotalInvoice(obj);

                    foreach (var item in obj.item_expense_cost.Where(f => f.TypeExpense != null))
                    {
                        item.entity_state = EntityState.Unchanged;
                    }
                }
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(ExpenseCost obj)
        {
            var result = 0;

			try
            {
                var api = "delete";
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return result;
        }                

        public IList<ItemExpenseCost> GetItemExpenseCost(string expenseCostId)
        {
            throw new NotImplementedException();
        }
    }
}     
