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
    public class HeaderInvoiceMiniPosBll : IHeaderInvoiceMiniPosBll
    {
		private ILog _log;
        private IUnitOfWork _unitOfWork;
		private HeaderInvoiceMiniPosValidator _validator;

		public HeaderInvoiceMiniPosBll()
        {
            _validator = new HeaderInvoiceMiniPosValidator();
        }

        public IList<HeaderInvoiceMiniPos> GetAll()
        {
            IList<HeaderInvoiceMiniPos> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.HeaderInvoiceMiniPosRepository.GetAll();
            }

            return oList;
        }

		public int Save(HeaderInvoiceMiniPos obj)
        {
            throw new NotImplementedException();
        }

		public int Update(HeaderInvoiceMiniPos obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.HeaderInvoiceMiniPosRepository.Update(obj);
            }

            return result;
        }

        public int Update(HeaderInvoiceMiniPos obj, ref ValidationError validationError)
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

        public int Delete(HeaderInvoiceMiniPos obj)
        {
            throw new NotImplementedException();
        }
    }
}     
