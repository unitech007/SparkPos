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
    public class SettingApplicationRepository : ISettingApplicationRepository
    {
        private IDapperContext _context;
        private ILog _log;

        public SettingApplicationRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public SettingApplication GetByID(string id)
        {
            SettingApplication obj = null;

            try
            {
                obj = _context.db.Get<SettingApplication>(id);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public int Save(SettingApplication obj)
        {
            var result = 0;

            try
            {
                obj.application_setting_id = _context.GetGUID();

                _context.db.Insert<SettingApplication>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(SettingApplication obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<SettingApplication>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(SettingApplication obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<SettingApplication>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public IList<SettingApplication> GetAll()
        {
            IList<SettingApplication> oList = new List<SettingApplication>();

            try
            {
                oList = _context.db.GetAll<SettingApplication>()
                                .ToList();
            }
            catch (Exception ex)
            {
                
                
                
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
