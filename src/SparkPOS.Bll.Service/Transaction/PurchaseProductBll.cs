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
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
 
namespace SparkPOS.Bll.Service
{    
    public class PurchaseProductBll : IPurchaseProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
		private PurchaseProductValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

		public PurchaseProductBll(ILog log)
        {
            _log = log;
            _validator = new PurchaseProductValidator();
        }

        public PurchaseProductBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public PurchaseProduct GetByID(string id)
        {
            PurchaseProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.PurchaseProductRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.PurchaseProductRepository.GetByID(id);
                }
            }            

            return obj;
        }

        public IList<PurchaseProduct> GetByName(string name)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetByName(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetByName(name);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetByName(string name, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetByName(name, pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetByName(name, pageNumber, pageSize, ref pagesCount);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetAll()
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetAll();
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetAll(pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetAll(pageNumber, pageSize, ref pagesCount);
                }
            }            

            return oList;
        }

		public int Save(PurchaseProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.purchase_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_beli)
                {
                    item.purchase_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PurchaseProductRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PurchaseProductRepository.Save(obj);
                }
            }            

            return result;
        }

        public int Save(PurchaseProduct obj, ref ValidationError validationError)
        {
			var validatorResults = _validator.Validate(obj);

            if (!validatorResults.IsValid)
            {
                foreach (var failure in validatorResults.Errors)
                {
                    validationError.Message = failure.ErrorMessage;
                    validationError.PropertyName = failure.PropertyName;
                    return 0;
                }
            }

            return Save(obj);
        }

		public int Update(PurchaseProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                foreach (var item in obj.item_beli.Where(f => f.entity_state == EntityState.Added))
                {
                    item.purchase_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PurchaseProductRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PurchaseProductRepository.Update(obj);
                }
            }            

            return result;
        }

        public int Update(PurchaseProduct obj, ref ValidationError validationError)
        {
            var validatorResults = _validator.Validate(obj);

            if (!validatorResults.IsValid)
            {
                foreach (var failure in validatorResults.Errors)
                {
                    validationError.Message = failure.ErrorMessage;
                    validationError.PropertyName = failure.PropertyName;
                    return 0;
                }
            }

            return Update(obj);
        }

        public int Delete(PurchaseProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PurchaseProductRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PurchaseProductRepository.Delete(obj);
                }
            }            

            return result;
        }

        public string GetLastInvoice()
        {
            var lastInvoice = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastInvoice = _unitOfWork.PurchaseProductRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.PurchaseProductRepository.GetLastInvoice();
                }
            }            

            return lastInvoice;
        }

        public List<Tax> GetTaxNames()
        {
            List<Tax> taxNames = new List<Tax>();

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                taxNames = _unitOfWork.PurchaseProductRepository.GetTaxNames();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    taxNames = _unitOfWork.PurchaseProductRepository.GetTaxNames();
                }
            }

            return taxNames;
        }


        public IList<PurchaseProduct> GetAll(string name)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetAll(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetAll(name);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceSupplier(string id, string invoice)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetInvoiceSupplier(id, invoice);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetInvoiceSupplier(id, invoice);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceKreditBySupplier(string id, bool isLunas)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetInvoiceKreditBySupplier(id, isLunas);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetInvoiceKreditBySupplier(id, isLunas);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetInvoiceKreditByInvoice(string id, string invoice)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetInvoiceKreditByInvoice(id, invoice);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetInvoiceKreditByInvoice(id, invoice);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
                }
            }            

            return oList;
        }

        public IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<PurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
                }
            }            

            return oList;
        }

        public IList<ItemPurchaseProduct> GetItemPurchase(string beliId)
        {
            IList<ItemPurchaseProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PurchaseProductRepository.GetItemPurchase(beliId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PurchaseProductRepository.GetItemPurchase(beliId);
                }
            }            

            return oList;
        }
    }
}     
