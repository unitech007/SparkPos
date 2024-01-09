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
    public class RegencyShippingCostsByRajaRepository : IRegencyShippingCostsByRajaRepository
    {
        private const string SQL_TEMPLATE = @"SELECT m_regency.regency_id, m_regency.type, m_regency.name_regency, m_regency.postal_code, 
                                              m_province.province_id, m_province.name_province
                                              FROM public.m_regency INNER JOIN public.m_province ON m_regency.province_id = m_province.province_id                                              
                                              {WHERE}
                                              ORDER BY m_regency.name_regency";
        private IDapperContext _context;
        private ILog _log;

        private string _sql;
		
        public RegencyShippingCostsByRajaRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<RegencyShippingCostsByRaja> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<RegencyShippingCostsByRaja> oList = _context.db.Query<RegencyShippingCostsByRaja, ProvinsiRajaOngkir, RegencyShippingCostsByRaja>(sql, (k, p) =>
            {
                if (p != null)
                {
                    k.province_id = p.province_id; k.Provinsi = p;
                }

                return k;
            }, param, splitOn: "province_id");

            return oList;
        }

        public RegencyShippingCostsByRaja GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public IList<RegencyShippingCostsByRaja> GetByName(string name)
        {
            IList<RegencyShippingCostsByRaja> oList = new List<RegencyShippingCostsByRaja>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_regency.name_regency) LIKE @name OR LOWER(m_province.name_province) LIKE @name");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<RegencyShippingCostsByRaja> GetAll()
        {
            IList<RegencyShippingCostsByRaja> oList = new List<RegencyShippingCostsByRaja>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(RegencyShippingCostsByRaja obj)
        {
            throw new NotImplementedException();
        }

        public int Update(RegencyShippingCostsByRaja obj)
        {
            throw new NotImplementedException();
        }

        public int Delete(RegencyShippingCostsByRaja obj)
        {
            throw new NotImplementedException();
        }
    }
}     
