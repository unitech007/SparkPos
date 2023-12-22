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
    public class DebtPaymentProductBll : IDebtPaymentProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
		private DebtPaymentProductValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

		public DebtPaymentProductBll(ILog log)
        {
            _log = log;
            _validator = new DebtPaymentProductValidator();
        }

        public DebtPaymentProductBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public DebtPaymentProduct GetByID(string id)
        {
            DebtPaymentProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.DebtPaymentProductRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.DebtPaymentProductRepository.GetByID(id);
                }
            }            

            return obj;
        }

        public IList<DebtPaymentProduct> GetByName(string name)
        {
            IList<DebtPaymentProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.DebtPaymentProductRepository.GetByName(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.DebtPaymentProductRepository.GetByName(name);
                }
            }            

            return oList;
        }

        public IList<DebtPaymentProduct> GetAll()
        {
            IList<DebtPaymentProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.DebtPaymentProductRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.DebtPaymentProductRepository.GetAll();
                }
            }            

            return oList;
        }

        public IList<ItemDebtPaymentProduct> GetPaymentHistory(string beliId)
        {
            IList<ItemDebtPaymentProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.DebtPaymentProductRepository.GetPaymentHistory(beliId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.DebtPaymentProductRepository.GetPaymentHistory(beliId);
                }
            }            

            return oList;
        }

		public int Save(DebtPaymentProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Save(DebtPaymentProduct obj, bool isSaveFromPurchase, ref ValidationError validationError)
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
                obj.pay_purchase_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_payment_debt)
                {
                    item.pay_purchase_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.DebtPaymentProductRepository.Save(obj, isSaveFromPurchase);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.DebtPaymentProductRepository.Save(obj, isSaveFromPurchase);
                }
            }            

            return result;
        }

		public int Update(DebtPaymentProduct obj)
        {
            throw new NotImplementedException();
        }

        public int Update(DebtPaymentProduct obj, bool isUpdateFromPurchase, ref ValidationError validationError)
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
                foreach (var item in obj.item_payment_debt.Where(f => f.entity_state == EntityState.Added))
                {
                    item.pay_purchase_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.DebtPaymentProductRepository.Update(obj, isUpdateFromPurchase);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.DebtPaymentProductRepository.Update(obj, isUpdateFromPurchase);
                }
            }            

            return result;
        }

        public int Delete(DebtPaymentProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.DebtPaymentProductRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.DebtPaymentProductRepository.Delete(obj);
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
                lastInvoice = _unitOfWork.DebtPaymentProductRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.DebtPaymentProductRepository.GetLastInvoice();
                }
            }            

            return lastInvoice;
        }

        public ItemDebtPaymentProduct GetByPurchaseID(string id)
        {
            ItemDebtPaymentProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.DebtPaymentProductRepository.GetByPurchaseID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.DebtPaymentProductRepository.GetByPurchaseID(id);
                }
            }            

            return obj;
        }

        public IList<DebtPaymentProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<DebtPaymentProduct> oList = null;
            
            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.DebtPaymentProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.DebtPaymentProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }             

            return oList;
        }        
    }
}     
