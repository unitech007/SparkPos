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
    public class ReturnSellingProductBll : IReturnSellingProductBll
    {
		private ILog _log;
        private IUnitOfWork _unitOfWork;
		private ReturnSellingProductValidator _validator;

		public ReturnSellingProductBll(ILog log)
        {
			_log = log;
            _validator = new ReturnSellingProductValidator();
        }

        public string GetLastInvoice()
        {
            var lastInvoice = string.Empty;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                lastInvoice = _unitOfWork.ReturnSellingProductRepository.GetLastInvoice();
            }

            return lastInvoice;
        }

        public ReturnSellingProduct GetByID(string id)
        {
            ReturnSellingProduct obj = null;
            
            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                obj = _unitOfWork.ReturnSellingProductRepository.GetByID(id);
            }

            return obj;
        }

        public IList<ReturnSellingProduct> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ReturnSellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReturnSellingProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReturnSellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }

            return oList;
        }

        public IList<ReturnSellingProduct> GetAll()
        {
            IList<ReturnSellingProduct> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.ReturnSellingProductRepository.GetAll();
            }

            return oList;
        }

		public int Save(ReturnSellingProduct obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.ReturnSellingProductRepository.Save(obj);
            }

            return result;
        }

        public int Save(ReturnSellingProduct obj, ref ValidationError validationError)
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

		public int Update(ReturnSellingProduct obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.ReturnSellingProductRepository.Update(obj);
            }

            return result;
        }

        public int Update(ReturnSellingProduct obj, ref ValidationError validationError)
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

        public int Delete(ReturnSellingProduct obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.ReturnSellingProductRepository.Delete(obj);
            }

            return result;
        }                
    }
}     
