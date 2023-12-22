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
    public class PaymentLoanBll : IPaymentLoanBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private PaymentLoanValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

        public PaymentLoanBll(ILog log)
        {
            _log = log;
            _validator = new PaymentLoanValidator();
        }

        public PaymentLoanBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public string GetLastInvoice()
        {
            var lastInvoice = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastInvoice = _unitOfWork.PaymentLoanRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.PaymentLoanRepository.GetLastInvoice();
                }
            }

            return lastInvoice;
        }

        public PaymentLoan GetByID(string id)
        {
            PaymentLoan obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.PaymentLoanRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.PaymentLoanRepository.GetByID(id);
                }
            }

            return obj;
        }

        public IList<PaymentLoan> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<PaymentLoan> GetByLoanId(string kasbonId)
        {
            IList<PaymentLoan> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PaymentLoanRepository.GetByLoanId(kasbonId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PaymentLoanRepository.GetByLoanId(kasbonId);
                }
            }

            return oList;
        }

        public IList<PaymentLoan> GetBySalaryEmployee(string gajiEmployeeId)
        {
            IList<PaymentLoan> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.PaymentLoanRepository.GetBySalaryEmployee(gajiEmployeeId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.PaymentLoanRepository.GetBySalaryEmployee(gajiEmployeeId);
                }
            }

            return oList;
        }

        public IList<PaymentLoan> GetAll()
        {
            throw new NotImplementedException();
        }

		public int Save(PaymentLoan obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
				obj.payment_loan_id = Guid.NewGuid().ToString();
            
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PaymentLoanRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
				{
					_unitOfWork = new UnitOfWork(context, _log);
					result = _unitOfWork.PaymentLoanRepository.Save(obj);
				}
            }

            return result;
        }

        public int Save(PaymentLoan obj, ref ValidationError validationError)
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

		public int Update(PaymentLoan obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PaymentLoanRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PaymentLoanRepository.Update(obj);
                }
            }

            return result;
        }

        public int Update(PaymentLoan obj, ref ValidationError validationError)
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

        public int Delete(PaymentLoan obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.PaymentLoanRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.PaymentLoanRepository.Delete(obj);
                }
            }

            return result;
        }        
    }
}     
