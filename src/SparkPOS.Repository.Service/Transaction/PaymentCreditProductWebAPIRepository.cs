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
    public class PaymentCreditProductWebAPIRepository : IPaymentCreditProductRepository
    {        
        private string _apiUrl = string.Empty;
        private ILog _log;

        public PaymentCreditProductWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/payment_credit/";
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
                _log.Error("Error:", ex);
            }

            return result;
        }

        public PaymentCreditProduct GetByID(string id)
        {
            PaymentCreditProduct obj = null;

            try
            {
                var api = string.Format("get_by_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PaymentCreditProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public ItemPaymentCreditProduct GetBySellingID(string id)
        {
            ItemPaymentCreditProduct obj = null;

            try
            {
                var api = string.Format("get_by_jual_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ItemPaymentCreditProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<PaymentCreditProduct> GetByName(string name)
        {
            IList<PaymentCreditProduct> oList = new List<PaymentCreditProduct>();

            try
            {
                var api = string.Format("get_by_name?name={0}", name);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PaymentCreditProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PaymentCreditProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<PaymentCreditProduct> oList = new List<PaymentCreditProduct>();

            try
            {
                var api = string.Format("get_by_tanggal?tanggalMulai={0}&tanggalSelesai={1}", tanggalMulai, tanggalSelesai);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PaymentCreditProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ItemPaymentCreditProduct> GetPaymentHistory(string jualId)
        {
            IList<ItemPaymentCreditProduct> oList = new List<ItemPaymentCreditProduct>();

            try
            {
                var api = string.Format("get_history_payment?jualId={0}", jualId);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ItemPaymentCreditProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<PaymentCreditProduct> GetAll()
        {
            IList<PaymentCreditProduct> oList = new List<PaymentCreditProduct>();

            try
            {
                var api = "get_all";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PaymentCreditProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(PaymentCreditProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Save(PaymentCreditProduct obj, bool isSaveFromSales)
        {
            var result = 0;

            try
            {
                obj.date = obj.date.ToUtc();

                var api = string.Format("save?isSaveFromSales={0}", isSaveFromSales);
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);

                if (result > 0)
                {
                    foreach (var item in obj.item_payment_credit.Where(f => f.SellingProduct != null))
                    {
                        item.entity_state = EntityState.Unchanged;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }        

        public int Update(PaymentCreditProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Update(PaymentCreditProduct obj, bool isUpdateFromSales)
        {
            var result = 0;

            try
            {
                obj.date = obj.date.ToUtc();

                var api = string.Format("update?isUpdateFromSales={0}", isUpdateFromSales);
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);

                if (result > 0)
                {
                    foreach (var item in obj.item_payment_credit.Where(f => f.SellingProduct != null))
                    {
                        item.entity_state = EntityState.Unchanged;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(PaymentCreditProduct obj)
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
                _log.Error("Error:", ex);
            }

            return result;
        }        
    }
}
