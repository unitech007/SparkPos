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
using Dapper;

namespace SparkPOS.Repository.Service
{        
    public class TaxRepository : ITaxRepository
    {
        private IDapperContext _context;
        private ILog _log;

        public TaxRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public Tax GetByID(string id)
        {
            Tax obj = null;

            try
            {
                obj = _context.db.Get<Tax>(id);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);                
            }

            return obj;
        }

        //public void ClearDefaultTax()
        //{
        //    try
        //    {
        //        string query = "UPDATE m_tax SET is_default_tax = 0 WHERE tax_id <> @currentTaxId";

        //        var result = _context.db.Query(query, new { invoiceNumber });
        //        quotations = _context.db.Query<string>(query, new { customerId }).ToList();

        //        using (IDapperContext context = new DapperContext())
        //        {
        //            context.db.Execute(query, new { currentTaxId });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception as needed
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //}

        //public void ClearDefaultTax(string  currentTaxId)
        //{
        //    try
        //    {
        //        string query = "UPDATE m_tax SET is_default_tax = false  WHERE tax_id <> @currentTaxId";

        //        using (IDapperContext context = new DapperContext())
        //        {
        //            context.db.Execute(query, new { currentTaxId });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception as needed
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //}

        //public void ClearDefaultTax(string currentTaxId)
        //{
        //    try
        //    {
        //        string query = "UPDATE m_tax SET is_default_tax = (tax_id = @currentTaxId)";

        //        using (IDapperContext context = new DapperContext())
        //        {
        //            context.db.Execute(query, new { currentTaxId });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception as needed
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //}


        public IList<Tax> GetByName(string name, bool useLikeOperator = true)
        {
            IList<Tax> oList = new List<Tax>();

            try
            {
                oList = _context.db.GetAll<Tax>()
                                .Where(f => useLikeOperator ? f.name_tax.ToLower().Contains(name.ToLower()) : f.name_tax.ToLower() == name.ToLower())
                                .OrderBy(f => f.name_tax)
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Tax> GetAll()
        {
            IList<Tax> oList = new List<Tax>();

            try
            {
                oList = _context.db.GetAll<Tax>()
                                .OrderBy(f => f.name_tax)
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(Tax obj)
        {
            var result = 0;

            try
            {
                if (obj.tax_id == null)
                    obj.tax_id = _context.GetGUID();

                _context.db.Insert<Tax>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(Tax obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<Tax>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(Tax obj)
        {
            var result = 0;
            
            try
            {
                result = _context.db.Delete<Tax>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }
    }
}     
