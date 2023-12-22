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
using System.Net;
using System.Web.Http;
using SparkPOS.Model;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
using SparkPOS.WebAPI.Models;
using SparkPOS.WebAPI.Models.DTO;
using SparkPOS.WebAPI.Controllers.Helper;

namespace SparkPOS.WebAPI.Controllers
{
    public interface IPurchaseProductController : IBaseApiController<PurchaseProductDTO>
    {
        IHttpActionResult GetLastInvoice();
        IHttpActionResult GetByID(string id);

        IHttpActionResult GetAll(string name);
        IHttpActionResult GetAll(int pageNumber, int pageSize);
        IHttpActionResult GetInvoiceSupplier(string id, string invoice);
        IHttpActionResult GetInvoiceKreditBySupplier(string id, bool isLunas);
        IHttpActionResult GetInvoiceKreditByInvoice(string id, string invoice);
        IHttpActionResult GetByName(string name);
        IHttpActionResult GetByName(string name, int pageNumber, int pageSize);
        IHttpActionResult GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IHttpActionResult GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize);
        IHttpActionResult GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name);
        IHttpActionResult GetItemPurchase(string beliId);
    }

    [RoutePrefix("api/beli_produk")]
    public class PurchaseProductController : BaseApiController, IPurchaseProductController
    {
        private IUnitOfWork _unitOfWork;
        private ILog _log;
        private HttpStatusCode _httpStatusCode = HttpStatusCode.BadRequest;
        private IHttpActionResult _response = null;
		
		public PurchaseProductController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public PurchaseProductController(IUnitOfWork unitOfWork, ILog log)
        {
            this._unitOfWork = unitOfWork;
            this._log = log;
        }

        [HttpGet, Route("get_last_nota")]
        public IHttpActionResult GetLastInvoice()
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = new List<string>();
                var obj = _unitOfWork.PurchaseProductRepository.GetLastInvoice();

                if (obj != null)
                    results.Add(obj);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_by_id")]
        public IHttpActionResult GetByID(string id)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = new List<PurchaseProduct>();
                var obj = _unitOfWork.PurchaseProductRepository.GetByID(id);

                if (obj != null)
                    results.Add(obj);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_all")]
        public IHttpActionResult GetAll()
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetAll();

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_all_filter_by")]
        public IHttpActionResult GetAll(string name)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetAll(name.NullToString());

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_all_with_paging")]
        public IHttpActionResult GetAll(int pageNumber, int pageSize)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var pagesCount = 0;
                var results = _unitOfWork.PurchaseProductRepository.GetAll(pageNumber, pageSize, ref pagesCount);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results, pagesCount);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_nota_supplier")]
        public IHttpActionResult GetInvoiceSupplier(string id, string invoice)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetInvoiceSupplier(id, invoice.NullToString());

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_nota_kredit_supplier_by_status")]
        public IHttpActionResult GetInvoiceKreditBySupplier(string id, bool isLunas)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetInvoiceKreditBySupplier(id, isLunas);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_nota_kredit_supplier_by_nota")]
        public IHttpActionResult GetInvoiceKreditByInvoice(string id, string invoice)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetInvoiceKreditByInvoice(id, invoice.NullToString());

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_by_name")]
        public IHttpActionResult GetByName(string name)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetByName(name.NullToString());

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_by_name_with_paging")]
        public IHttpActionResult GetByName(string name, int pageNumber, int pageSize)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var pagesCount = 0;
                var results = _unitOfWork.PurchaseProductRepository.GetByName(name, pageNumber, pageSize, ref pagesCount);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results, pagesCount);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_by_tanggal")]
        public IHttpActionResult GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_by_tanggal_with_paging")]
        public IHttpActionResult GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var pagesCount = 0;
                var results = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results, pagesCount);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_by_tanggal_with_name")]
        public IHttpActionResult GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai, name);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpGet, Route("get_item_beli")]
        public IHttpActionResult GetItemPurchase(string beliId)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            try
            {
                var results = _unitOfWork.PurchaseProductRepository.GetItemPurchase(beliId);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, results);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpPost, Route("save")]
        public IHttpActionResult Save(PurchaseProductDTO objDTO)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            if (!objDTO.IsValidate(Request, ref _response))
            {
                return _response;
            }

            try
            {
                var obj = AutoMapper.Mapper.Map<PurchaseProduct>(objDTO);

                var result = _unitOfWork.PurchaseProductRepository.Save(obj);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, result);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpPost, Route("update")]
        public IHttpActionResult Update(PurchaseProductDTO objDTO)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            if (!objDTO.IsValidate(Request, ref _response))
            {
                return _response;
            }

            try
            {
                var obj = AutoMapper.Mapper.Map<PurchaseProduct>(objDTO);

                var result = _unitOfWork.PurchaseProductRepository.Update(obj);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, result);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }

        [HttpPost, Route("delete")]
        public IHttpActionResult Delete(PurchaseProductDTO objDTO)
        {
            _httpStatusCode = HttpStatusCode.BadRequest;
            _response = Content(_httpStatusCode, new ResponsePackage(_httpStatusCode));

            if (!objDTO.IsValidate(Request, ref _response))
            {
                return _response;
            }

            try
            {                
                var obj = AutoMapper.Mapper.Map<PurchaseProduct>(objDTO);

                var result = _unitOfWork.PurchaseProductRepository.Delete(obj);

                _httpStatusCode = HttpStatusCode.OK;
                var output = GenerateOutput(_httpStatusCode, result);

                _response = Content(_httpStatusCode, output);
            }
            catch (Exception ex)
            {
                if (_log != null)
                    _log.Error("Error:", ex);
            }

            return _response;
        }        
    }
}
