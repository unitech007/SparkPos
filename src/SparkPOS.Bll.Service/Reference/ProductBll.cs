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
    public class ProductBll : IProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private ProductValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

		public ProductBll(ILog log)
        {
            _log = log;
            _validator = new ProductValidator();
        }

        public ProductBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public Product GetByID(string id)
        {
            Product obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.ProductRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.ProductRepository.GetByID(id);
                }
            }

            return obj;
        }

        public Product GetByCode(string codeProduct, bool isCekStatusActive = false)
        {
            Product obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.ProductRepository.GetByCode(codeProduct, isCekStatusActive);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.ProductRepository.GetByCode(codeProduct, isCekStatusActive);
                }
            }

            return obj;
        }        

        public string GetLastCodeProduct()
        {
            var lastInvoice = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastInvoice = _unitOfWork.ProductRepository.GetLastCodeProduct();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.ProductRepository.GetLastCodeProduct();
                }
            }

            return lastInvoice;
        }

        public IList<Product> GetByName(string name, bool isLoadPriceWholesale = true, bool isCekStatusActive = false)
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetByName(name, isLoadPriceWholesale, isCekStatusActive);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetByName(name, isLoadPriceWholesale, isCekStatusActive);
                }
            }

            return oList;
        }

        private string GetSortByFieldName(int sortByIndex)
        {
            return sortByIndex == 0 ? "m_product.product_code" : "m_product.product_name";
        }

        public IList<Product> GetByName(string name, int sortByIndex, int pageNumber, int pageSize, ref int pagesCount, bool isLoadPriceWholesale = true)
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetByName(name, GetSortByFieldName(sortByIndex), pageNumber, pageSize, ref pagesCount, isLoadPriceWholesale);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetByName(name, GetSortByFieldName(sortByIndex), pageNumber, pageSize, ref pagesCount, isLoadPriceWholesale);
                }
            }

            return oList;
        }

        public IList<Product> GetByCategory(string golonganId)
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetByCategory(golonganId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetByCategory(golonganId);
                }
            }

            return oList;
        }

        public IList<Product> GetByCategory(string golonganId, int sortByIndex, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetByCategory(golonganId, GetSortByFieldName(sortByIndex), pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetByCategory(golonganId, GetSortByFieldName(sortByIndex), pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public IList<Product> GetInfoMinimalStock()
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetInfoMinimalStock();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetInfoMinimalStock();
                }
            }

            return oList;
        }

        public IList<Product> GetAll()
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetAll();
                }
            }

            return oList;
        }

        public IList<Product> GetAll(int sortByIndex)
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetAll(GetSortByFieldName(sortByIndex));
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetAll(GetSortByFieldName(sortByIndex));
                }
            }

            return oList;
        }

        public IList<Product> GetAll(int sortByIndex, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<Product> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ProductRepository.GetAll(GetSortByFieldName(sortByIndex), pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ProductRepository.GetAll(GetSortByFieldName(sortByIndex), pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

		public int Save(Product obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.product_id = Guid.NewGuid().ToString();

                foreach (var item in obj.list_of_harga_grosir)
                {
                    item.wholesale_price_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ProductRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ProductRepository.Save(obj);
                }
            }

            return result;
        }

        public int Save(Product obj, ref ValidationError validationError)
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

		public int Update(Product obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ProductRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ProductRepository.Update(obj);
                }
            }

            return result;
        }

        public int Update(Product obj, ref ValidationError validationError)
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

        public int Delete(Product obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ProductRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ProductRepository.Delete(obj);
                }
            }

            return result;
        }        
    }
}     
