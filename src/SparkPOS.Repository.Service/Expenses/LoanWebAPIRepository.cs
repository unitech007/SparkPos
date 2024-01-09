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
    public class LoanWebAPIRepository : ILoanRepository
    {
        private string _apiUrl = string.Empty;
        private ILog _log;
		
        public LoanWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/loan/";
            this._log = log;
        }

        public loan GetByID(string id)
        {
            loan obj = null;

			try
            {
                var api = string.Format("get_by_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<loan>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<loan> GetByName(string name)
        {
            throw new NotImplementedException();
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

        public IList<loan> GetByEmployeeId(string employeeId)
        {
            IList<loan> oList = new List<loan>();

            try
            {
                var api = string.Format("get_by_employee_id?employee_id={0}", employeeId);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<loan>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<loan> GetByStatus(bool isLunas)
        {
            IList<loan> oList = new List<loan>();

            try
            {
                var api = string.Format("get_by_status?is_lunas={0}", isLunas);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<loan>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<loan> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<loan> oList = new List<loan>();

            try
            {
                var api = string.Format("get_by_tanggal?tanggal_mulai={0}&tanggal_selesai={1}", tanggalMulai, tanggalSelesai);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<loan>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<loan> GetAll()
        {
            IList<loan> oList = new List<loan>();

            try
            {
                var api = "get_all";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<loan>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
               
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(loan obj)
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
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Update(loan obj)
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
               
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(loan obj)
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
