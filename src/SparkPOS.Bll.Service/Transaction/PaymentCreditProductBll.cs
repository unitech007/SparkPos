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
    public class PaymentCreditProductBll : IPaymentCreditProductBll
    {
		private ILog _log;
        private IUnitOfWork _unitOfWork;
		private PaymentCreditProductValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

		public PaymentCreditProductBll(ILog log)
        {
			_log = log;
            _validator = new PaymentCreditProductValidator();
        }

        public PaymentCreditProductBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public PaymentCreditProduct GetByID(string id)
        {
            PaymentCreditProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.PaymentCreditProductRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.PaymentCreditProductRepository.GetByID(id);
                }
            }            

            return obj;
        }

        public string GetLastInvoice()
        {
            var lastInvoice = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastInvoice = _unitOfWork.PaymentCreditProductRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.PaymentCreditProductRepository.GetLastInvoice();
                }
            }            

            return lastInvoice;
        }

        public ItemPaymentCreditProduct GetBySellingID(string id)
        {
            ItemPaymentCreditProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.PaymentCreditProductRepository.GetBySellingID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.PaymentCreditProductRepository.GetBySellingID(id);
                }
            }            

            return obj;
        }

        public IList<PaymentCreditProduct> GetByName(string name)
        {
            IList<PaymentCreditProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PaymentCreditProductRepository.GetByName(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PaymentCreditProductRepository.GetByName(name);
                }
            }             

            return oList;
        }        

        public IList<PaymentCreditProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<PaymentCreditProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PaymentCreditProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PaymentCreditProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }            

            return oList;
        }

        public IList<PaymentCreditProduct> GetAll()
        {
            IList<PaymentCreditProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PaymentCreditProductRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PaymentCreditProductRepository.GetAll();
                }
            }            

            return oList;
        }

        public IList<ItemPaymentCreditProduct> GetPaymentHistory(string jualId)
        {
            IList<ItemPaymentCreditProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PaymentCreditProductRepository.GetPaymentHistory(jualId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PaymentCreditProductRepository.GetPaymentHistory(jualId);
                }
            }            

            return oList;
        }

		public int Save(PaymentCreditProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Save(PaymentCreditProduct obj, bool isSaveFromSales, ref ValidationError validationError)
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

            var result = 0;

            if (_isUseWebAPI)
            {
                obj.pay_sale_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_payment_credit)
                {
                    item.pay_sale_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PaymentCreditProductRepository.Save(obj, isSaveFromSales);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PaymentCreditProductRepository.Save(obj, isSaveFromSales);
                }
            }            

            return result;
        }

		public int Update(PaymentCreditProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Update(PaymentCreditProduct obj, bool isSaveFromSales, ref ValidationError validationError)
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

            var result = 0;

            if (_isUseWebAPI)
            {
                foreach (var item in obj.item_payment_credit.Where(f => f.entity_state == EntityState.Added))
                {
                    item.pay_sale_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PaymentCreditProductRepository.Update(obj, isSaveFromSales);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PaymentCreditProductRepository.Update(obj, isSaveFromSales);
                }
            }            

            return result;
        }

        public int Delete(PaymentCreditProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PaymentCreditProductRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PaymentCreditProductRepository.Delete(obj);
                }
            }            

            return result;
        }        
    }
}     
