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
    public class ExpenseCostBll : IExpenseCostBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private ExpenseCostValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

		public ExpenseCostBll(ILog log)
        {
			_log = log;
            _validator = new ExpenseCostValidator();
        }

        public ExpenseCostBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public IList<ItemExpenseCost> GetItemExpenseCost(string expenseCostId)
        {
            throw new NotImplementedException();
        }

        public ExpenseCost GetByID(string id)
        {
            ExpenseCost obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.ExpenseCostRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.ExpenseCostRepository.GetByID(id);
                }
            }

            return obj;
        }

        public IList<ExpenseCost> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ExpenseCost> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ExpenseCost> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ExpenseCostRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ExpenseCostRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }

            return oList;
        }        

        public IList<ExpenseCost> GetAll()
        {
            IList<ExpenseCost> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ExpenseCostRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ExpenseCostRepository.GetAll();
                }
            }

            return oList;
        }

		public int Save(ExpenseCost obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.expense_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_expense_cost)
                {
                    item.expense_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ExpenseCostRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ExpenseCostRepository.Save(obj);
                }
            }

            return result;
        }

        public int Save(ExpenseCost obj, ref ValidationError validationError)
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

		public int Update(ExpenseCost obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                foreach (var item in obj.item_expense_cost.Where(f => f.entity_state == EntityState.Added))
                {
                    item.expense_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ExpenseCostRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ExpenseCostRepository.Update(obj);
                }
            }

            return result;
        }

        public int Update(ExpenseCost obj, ref ValidationError validationError)
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

        public int Delete(ExpenseCost obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ExpenseCostRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ExpenseCostRepository.Delete(obj);
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
                lastInvoice = _unitOfWork.ExpenseCostRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.ExpenseCostRepository.GetLastInvoice();
                }
            }

            return lastInvoice;
        }
    }
}     
