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
using Dapper;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;

namespace SparkPOS.Repository.Service
{
    public class AreaRepository : IAreaRepository
    {        
        private IDapperContext _context;
        private ILog _log;

        public AreaRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public int Save(Area obj)
        {
            throw new NotImplementedException();
        }

        public int Update(Area obj)
        {
            throw new NotImplementedException();
        }

        public int Delete(Area obj)
        {
            throw new NotImplementedException();
        }

        public IList<Area> GetAll()
        {
            IList<Area> oList = new List<Area>();

            try
            {
                var sql = @"SELECT m_province2.province_id, m_province2.name_province, 
                            m_regency2.regency_id, m_regency2.name_regency, 
                            m_subdistrict.subdistrict_id, m_subdistrict.name_subdistrict
                            FROM public.m_province2 INNER JOIN public.m_regency2 ON m_regency2.province_id = m_province2.province_id
                            INNER JOIN public.m_subdistrict ON m_subdistrict.regency_id = m_regency2.regency_id
                            ORDER BY m_province2.name_province, m_regency2.name_regency, m_subdistrict.name_subdistrict";

                oList = _context.db.Query<Area>(sql)
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public Area GetProvinsi(string name)
        {            
            Area obj = null;

            try
            {
                var sql = @"SELECT * FROM m_province2 
                            WHERE LOWER(name_province) = @name";

                name = name.ToLower();
                obj = _context.db.QuerySingleOrDefault<Area>(sql, new { name });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public Area GetRegency(string name)
        {
            Area obj = null;

            try
            {
                var sql = @"SELECT * FROM m_regency2
                            WHERE LOWER(name_regency) = @name";

                name = name.ToLower();
                obj = _context.db.QuerySingleOrDefault<Area>(sql, new { name });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public Area Getsubdistrict(string name)
        {
            Area obj = null;

            try
            {
                var sql = @"SELECT * FROM m_subdistrict
                            WHERE LOWER(name_subdistrict) = @name";

                name = name.ToLower();
                obj = _context.db.QuerySingleOrDefault<Area>(sql, new { name });
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }
    }
}
