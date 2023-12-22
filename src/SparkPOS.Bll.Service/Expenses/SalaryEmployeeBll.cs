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
    public class SalaryEmployeeBll : ISalaryEmployeeBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private SalaryEmployeeValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

        public SalaryEmployeeBll(ILog log)
        {
            _log = log;
            _validator = new SalaryEmployeeValidator();
        }

        public SalaryEmployeeBll(bool isUseWebAPI, string baseUrl, ILog log)
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
                lastInvoice = _unitOfWork.SalaryEmployeeRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.SalaryEmployeeRepository.GetLastInvoice();
                }
            }

            return lastInvoice;
        }

        public SalaryEmployee GetByID(string id)
        {
            SalaryEmployee obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.SalaryEmployeeRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.SalaryEmployeeRepository.GetByID(id);
                }
            }

            return obj;
        }

        public IList<SalaryEmployee> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<SalaryEmployee> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<SalaryEmployee> GetByMonthAndYear(int month, int year)
        {
            IList<SalaryEmployee> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SalaryEmployeeRepository.GetByMonthAndYear(month, year);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SalaryEmployeeRepository.GetByMonthAndYear(month, year);
                }
            }

            return oList;
        }

		public int Save(SalaryEmployee obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.gaji_employee_id = Guid.NewGuid().ToString();

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SalaryEmployeeRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SalaryEmployeeRepository.Save(obj);
                }
            }

            return result;
        }

        public int Save(SalaryEmployee obj, ref ValidationError validationError)
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

		public int Update(SalaryEmployee obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SalaryEmployeeRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SalaryEmployeeRepository.Update(obj);
                }
            }

            return result;
        }

        public int Update(SalaryEmployee obj, ref ValidationError validationError)
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

        public int Delete(SalaryEmployee obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SalaryEmployeeRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SalaryEmployeeRepository.Delete(obj);
                }
            }

            return result;
        }        
    }
}     
