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
    public class MenuRepository : IMenuRepository
    {
        private IDapperContext _context;
		private ILog _log;
		
        public MenuRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public MenuApplication GetByID(string id)
        {
            MenuApplication obj = null;

            try
            {
                obj = _context.db.Get<MenuApplication>(id);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public MenuApplication GetByName(string name)
        {
            MenuApplication obj = null;

            try
            {
                obj = _context.db.GetAll<MenuApplication>()
                              .Where(f => f.name_menu.ToLower() == name.ToLower())
                              .SingleOrDefault();
            }
            catch
            {
            }

            return obj;
        }

        public IList<MenuApplication> GetAll()
        {
            IList<MenuApplication> oList = new List<MenuApplication>();

            try
            {
                oList = _context.db.GetAll<MenuApplication>()
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(MenuApplication obj)
        {
            var result = 0;

            try
            {
                obj.menu_id = _context.GetGUID();

                _context.db.Insert<MenuApplication>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(MenuApplication obj)
        {
            throw new NotImplementedException();
        }

        public int Delete(MenuApplication obj)
        {
            throw new NotImplementedException();
        }
    }
}     
