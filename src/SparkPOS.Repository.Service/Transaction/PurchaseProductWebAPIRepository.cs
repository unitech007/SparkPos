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
    public class PurchaseProductWebAPIRepository : IPurchaseProductRepository
    {
        private string _apiUrl = string.Empty;
        private ILog _log;

        public PurchaseProductWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/beli_produk/";
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

        public PurchaseProduct GetByID(string id)
        {
            PurchaseProduct obj = null;

            try
            {
                var api = string.Format("get_by_id?id={0}", id);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<PurchaseProduct> GetAll()
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = "get_all";
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetAll(string name)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_all_filter_by?name={0}", name);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_all_with_paging?pageNumber={0}&pageSize={1}", pageNumber, pageSize);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                {
                    pagesCount = response.Status.PagesCount;
                    oList = response.Results;
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceSupplier(string id, string invoice)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_nota_supplier?id={0}&invoice={1}", id, invoice);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceKreditBySupplier(string id, bool isLunas)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_nota_kredit_supplier_by_status?id={0}&isLunas={1}", id, isLunas);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceKreditByInvoice(string id, string invoice)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_nota_kredit_supplier_by_nota?id={0}&invoice={1}", id, invoice);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByName(string name)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_by_name?name={0}", name);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByName(string name, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_by_name_with_paging?name={0}&pageNumber={1}&pageSize={2}", name, pageNumber, pageSize);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                {
                    pagesCount = response.Status.PagesCount;
                    oList = response.Results;
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_by_tanggal?tanggalMulai={0}&tanggalSelesai={1}", tanggalMulai, tanggalSelesai);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_by_tanggal_with_paging?tanggalMulai={0}&tanggalSelesai={1}&pageNumber={2}&pageSize={3}", tanggalMulai, tanggalSelesai, pageNumber, pageSize);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                {
                    pagesCount = response.Status.PagesCount;
                    oList = response.Results;
                }
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<PurchaseProduct> oList = new List<PurchaseProduct>();

            try
            {
                var api = string.Format("get_by_tanggal_with_name?tanggalMulai={0}&tanggalSelesai={1}&name={2}", tanggalMulai, tanggalSelesai, name);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<PurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<ItemPurchaseProduct> GetItemPurchase(string beliId)
        {
            IList<ItemPurchaseProduct> oList = new List<ItemPurchaseProduct>();

            try
            {
                var api = string.Format("get_item_beli?beliId={0}", beliId);
                var response = RestSharpHelper<SparkPOSWebApiGetResponse<ItemPurchaseProduct>>.GetRequest(_apiUrl, api).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        private double GetTotalInvoice(PurchaseProduct obj)
        {
            var total = obj.item_beli.Where(f => f.Product != null && f.entity_state != EntityState.Deleted)
                                     .Sum(f => (f.quantity - f.return_quantity) * (f.price - (f.discount / 100 * f.price)));

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        public int Save(PurchaseProduct obj)
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
                    obj.total_invoice = GetTotalInvoice(obj);

                    // jika Purchase cash, langsung insert ke Dept Payment
                    if (obj.due_date.IsNull())
                        obj.total_payment = obj.grand_total;

                    foreach (var item in obj.item_beli.Where(f => f.Product != null))
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

        public int Update(PurchaseProduct obj)
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
                    obj.total_invoice = GetTotalInvoice(obj);

                    // jika terjadi perubahan status invoice dari cash ke Credit
                    if (obj.tanggal_creditTerm_old.IsNull() && !obj.due_date.IsNull())
                    {
                        obj.total_payment = 0;
                    }
                    else if (obj.due_date.IsNull()) // jika sales cash, langsung update ke payment credit
                    {
                        obj.total_payment = obj.grand_total;
                    }

                    foreach (var item in obj.item_beli.Where(f => f.Product != null))
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

        public int Delete(PurchaseProduct obj)
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

        public List<Tax> GetTaxNames()
        {
            throw new NotImplementedException();
        }
    }
}
