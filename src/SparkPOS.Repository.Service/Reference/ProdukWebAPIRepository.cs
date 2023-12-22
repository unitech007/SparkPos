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
    public class ProductWebAPIRepository : IProductRepository
    {
        private string _apiUrl = string.Empty;
        private ILog _log;
		
        public ProductWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/product/";
            this._log = log;
        }

        public Product GetByID(string id)
        {
            Product obj = null;

			try
            {
                var api = string.Format("get_by_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public Product GetByCode(string codeProduct, bool isCekStatusActive = false)
        {
            Product obj = null;

            try
            {
                var api = string.Format("get_by_code?codeProduct={0}&isCekStatusActive={1}", codeProduct, isCekStatusActive);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public string GetLastCodeProduct()
        {
            var result = string.Empty;

            try
            {
                var api = "get_last_code_produk";
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

        public IList<Product> GetByName(string name, bool isLoadPriceWholesale = true, bool isCekStatusActive = false)
        {
            IList<Product> oList = new List<Product>();

			try
            {
                var api = string.Format("get_by_name?name={0}&isLoadPriceWholesale={1}&isCekStatusActive={2}", name, isLoadPriceWholesale, isCekStatusActive);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetByName(string name, string sortBy, int pageNumber, int pageSize, ref int pagesCount, bool isLoadPriceWholesale = true)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var api = string.Format("get_by_name_with_paging?name={0}&sortBy={1}&pageNumber={2}&pageSize={3}&isLoadPriceWholesale={4}", name, sortBy, pageNumber, pageSize, isLoadPriceWholesale);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                {
                    pagesCount = response.Status.PagesCount;
                    oList = response.Results;
                }                    
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetByCategory(string golonganId)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var api = string.Format("get_by_golongan?golonganId={0}", golonganId);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetByCategory(string golonganId, string sortBy, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var api = string.Format("get_by_golongan_with_paging?golonganId={0}&sortBy={1}&pageNumber={2}&pageSize={3}", golonganId, sortBy, pageNumber, pageSize);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                {
                    pagesCount = response.Status.PagesCount;
                    oList = response.Results;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetInfoMinimalStock()
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var api = "get_info_minimal_stock";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }        

        public IList<Product> GetAll()
        {
            IList<Product> oList = new List<Product>();

			try
            {
                var api = "get_all";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetAll(string sortBy)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var api = string.Format("get_all_sort_by?sortBy={0}", sortBy);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetAll(string sortBy, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var api = string.Format("get_all_with_paging?sortBy={0}&pageNumber={1}&pageSize={2}", sortBy, pageNumber, pageSize);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<Product>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                {
                    pagesCount = response.Status.PagesCount;
                    oList = response.Results;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(Product obj)
        {
            var result = 0;

			try
            {
                var api = "save";
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(Product obj)
        {
            var result = 0;

			try
            {
                var api = "update";
                var response = RestSharpHelper<SparkPOSWebApiPostResponse>.PostRequest(_apiUrl, api, obj);

                result = Convert.ToInt32(response.Results);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(Product obj)
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
