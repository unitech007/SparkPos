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
    public class ReasonAdjustmentStockRepository : IReasonAdjustmentStockRepository
    {
        private IDapperContext _context;
        private ILog _log;

        public ReasonAdjustmentStockRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public ReasonAdjustmentStock GetByID(string id)
        {
            ReasonAdjustmentStock obj = null;
            
            try
            {
                obj = _context.db.Get<ReasonAdjustmentStock>(id);
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<ReasonAdjustmentStock> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<ReasonAdjustmentStock> GetAll()
        {
            IList<ReasonAdjustmentStock> oList = new List<ReasonAdjustmentStock>();

            try
            {
                oList = _context.db.GetAll<ReasonAdjustmentStock>()
                                .OrderBy(f => f.reason)
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(ReasonAdjustmentStock obj)
        {
            var result = 0;

            try
            {
                if (obj.stock_adjustment_reason_id == null)
                    obj.stock_adjustment_reason_id = _context.GetGUID();

                _context.db.Insert<ReasonAdjustmentStock>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Update(ReasonAdjustmentStock obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<ReasonAdjustmentStock>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(ReasonAdjustmentStock obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<ReasonAdjustmentStock>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }
    }
}     
