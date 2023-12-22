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
    public class ReasonAdjustmentStockBll : IReasonAdjustmentStockBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private ReasonAdjustmentStockValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;

        public ReasonAdjustmentStockBll(ILog log)
        {
            _log = log;
            _validator = new ReasonAdjustmentStockValidator();
        }

        public ReasonAdjustmentStockBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public ReasonAdjustmentStock GetByID(string id)
        {
            ReasonAdjustmentStock obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.ReasonAdjustmentStockRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.ReasonAdjustmentStockRepository.GetByID(id);
                }
            }

            return obj;
        }

        public IList<ReasonAdjustmentStock> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ReasonAdjustmentStock> GetAll()
        {
            IList<ReasonAdjustmentStock> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.ReasonAdjustmentStockRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.ReasonAdjustmentStockRepository.GetAll();
                }
            }

            return oList;
        }

        public int Save(ReasonAdjustmentStock obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.stock_adjustment_reason_id = Guid.NewGuid().ToString();

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ReasonAdjustmentStockRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ReasonAdjustmentStockRepository.Save(obj);
                }
            }

            return result;
        }

        public int Save(ReasonAdjustmentStock obj, ref ValidationError validationError)
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

        public int Update(ReasonAdjustmentStock obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ReasonAdjustmentStockRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ReasonAdjustmentStockRepository.Update(obj);
                }
            }

            return result;
        }

        public int Update(ReasonAdjustmentStock obj, ref ValidationError validationError)
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

        public int Delete(ReasonAdjustmentStock obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.ReasonAdjustmentStockRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.ReasonAdjustmentStockRepository.Delete(obj);
                }
            }

            return result;
        }
    }
}     
