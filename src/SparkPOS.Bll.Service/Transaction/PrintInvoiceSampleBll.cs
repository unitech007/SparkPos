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

using log4net;
using SparkPOS.Model.Invoice;
using SparkPOS.Bll.Api;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
using SparkPOS.Model.quotation;

namespace SparkPOS.Bll.Service
{
    public class PrintInvoiceSampleBll :IPrintInvoiceBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public IList<InvoicePurchase> GetInvoicePurchase(string beliProductId)
        {
            throw new NotImplementedException();
        }

        public IList<InvoiceSales> GetInvoiceSales(string jualProductId)
        {
            IList<InvoiceSales> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.PrintInvoiceSampleRepository.GetInvoiceSales(jualProductId);
            }

            return oList;
        }

        public IList<QuotationSales> GetQuotationSales(string jualProductId)
        {
            IList<QuotationSales> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.PrintQuotationSampleRepository.GetQuotationSales(jualProductId);
            }

            return oList;
        }
    }
}
