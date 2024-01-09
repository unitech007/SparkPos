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
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class HeaderInvoiceRepository : IHeaderInvoiceRepository
    {
        private IDapperContext _context;
		private ILog _log;
		
        public HeaderInvoiceRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<HeaderInvoice> GetAll()
        {
            IList<HeaderInvoice> oList = new List<HeaderInvoice>();

            try
            {
                oList = _context.db.GetAll<HeaderInvoice>()
                                .Where(f => f.is_active == true)
                                .OrderBy(f => f.order_number)
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(HeaderInvoice obj)
        {
            throw new NotImplementedException();
        }

        public int Update(HeaderInvoice obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<HeaderInvoice>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(HeaderInvoice obj)
        {
            throw new NotImplementedException();
        }
    }
}     
