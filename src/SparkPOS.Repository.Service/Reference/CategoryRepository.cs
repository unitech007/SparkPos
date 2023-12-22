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
    public class CategoryRepository : ICategoryRepository
    {
        private IDapperContext _context;
        private ILog _log;

        public CategoryRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public Category GetByID(string id)
        {
            Category obj = null;

            try
            {
                obj = _context.db.Get<Category>(id);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);                
            }

            return obj;
        }

        public IList<Category> GetByName(string name, bool useLikeOperator = true)
        {
            IList<Category> oList = new List<Category>();

            try
            {
                oList = _context.db.GetAll<Category>()
                                .Where(f => useLikeOperator ? f.name_category.ToLower().Contains(name.ToLower()) : f.name_category.ToLower() == name.ToLower())
                                .OrderBy(f => f.name_category)
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Category> GetAll()
        {
            IList<Category> oList = new List<Category>();

            try
            {
                oList = _context.db.GetAll<Category>()
                                .OrderBy(f => f.name_category)
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(Category obj)
        {
            var result = 0;

            try
            {
                if (obj.category_id == null)
                    obj.category_id = _context.GetGUID();

                _context.db.Insert<Category>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(Category obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<Category>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(Category obj)
        {
            var result = 0;
            
            try
            {
                result = _context.db.Delete<Category>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }
    }
}     
