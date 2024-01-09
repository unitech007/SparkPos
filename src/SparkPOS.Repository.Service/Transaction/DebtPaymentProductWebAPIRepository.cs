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
    public class DebtPaymentProductWebAPIRepository : IDebtPaymentProductRepository
    {
        private string _apiUrl = string.Empty;
        private ILog _log;

        public DebtPaymentProductWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/payment_debt/";
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

        public DebtPaymentProduct GetByID(string id)
        {
            DebtPaymentProduct obj = null;

            try
            {
                var api = string.Format("get_by_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<DebtPaymentProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public ItemDebtPaymentProduct GetByPurchaseID(string id)
        {
            ItemDebtPaymentProduct obj = null;

            try
            {
                var api = string.Format("get_by_beli_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ItemDebtPaymentProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<DebtPaymentProduct> GetByName(string name)
        {
            IList<DebtPaymentProduct> oList = new List<DebtPaymentProduct>();

            try
            {
                var api = string.Format("get_by_name?name={0}", name);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<DebtPaymentProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<DebtPaymentProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<DebtPaymentProduct> oList = new List<DebtPaymentProduct>();

            try
            {
                var api = string.Format("get_by_tanggal?tanggalMulai={0}&tanggalSelesai={1}", tanggalMulai, tanggalSelesai);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<DebtPaymentProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ItemDebtPaymentProduct> GetPaymentHistory(string beliId)
        {
            IList<ItemDebtPaymentProduct> oList = new List<ItemDebtPaymentProduct>();

            try
            {
                var api = string.Format("get_history_payment?beliId={0}", beliId);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ItemDebtPaymentProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<DebtPaymentProduct> GetAll()
        {
            IList<DebtPaymentProduct> oList = new List<DebtPaymentProduct>();

            try
            {
                var api = "get_all";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<DebtPaymentProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(DebtPaymentProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Save(DebtPaymentProduct obj, bool isSaveFromPurchase)
        {
            var result = 0;

            try
            {
                obj.date = obj.date.ToUtc();

                var api = string.Format("save?isSaveFromPurchase={0}", isSaveFromPurchase);
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);

                if (result > 0)
                {
                    foreach (var item in obj.item_payment_debt.Where(f => f.PurchaseProduct != null))
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

        public int Update(DebtPaymentProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Update(DebtPaymentProduct obj, bool isUpdateFromPurchase)
        {
            var result = 0;

            try
            {
                obj.date = obj.date.ToUtc();

                var api = string.Format("update?isUpdateFromPurchase={0}", isUpdateFromPurchase);
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);

                if (result > 0)
                {
                    foreach (var item in obj.item_payment_debt.Where(f => f.PurchaseProduct != null))
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

        public int Delete(DebtPaymentProduct obj)
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
    }
}
