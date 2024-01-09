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
using System.Linq.Expressions;
 
namespace SparkPOS.Repository.Service
{        
    public class CustomerRepository : ICustomerRepository
    {
        private const string SQL_TEMPLATE = @"SELECT m_customer.customer_id, m_customer.name_customer, COALESCE(m_customer.regency, m_customer.regency, m_customer.city) AS regency_old, m_customer.subdistrict AS subdistrict_old, 
                                              m_customer.address, m_customer.contact, m_customer.phone, m_customer.credit_limit, m_customer.total_credit, 
                                              m_customer.total_receivable_payment, m_customer.postal_code, m_customer.discount, m_customer.cr_no,
                                              m_province2.province_id, m_province2.name_province, m_regency2.regency_id, m_regency2.name_regency, 
                                              m_subdistrict.subdistrict_id, m_subdistrict.name_subdistrict
                                              FROM public.m_customer LEFT JOIN public.m_province2 ON m_customer.province_id = m_province2.province_id
                                              LEFT JOIN public.m_regency2 ON m_customer.regency_id = m_regency2.regency_id
                                              LEFT JOIN public.m_subdistrict ON m_customer.subdistrict_id = m_subdistrict.subdistrict_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;
        private string _sql;

        public CustomerRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<Customer> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Customer> oList = _context.db.Query<Customer, Provinsi, Regency, subdistrict, Customer>(sql, (cus, prov, kab, kec) =>
            {
                if (prov != null)
                {
                    cus.province_id = prov.province_id; cus.Provinsi = prov;
                }

                if (kab != null)
                {
                    cus.regency_id = kab.regency_id; cus.Regency = kab;
                }

                if (kec != null)
                {
                    cus.subdistrict_id = kec.subdistrict_id; cus.subdistrict = kec;
                }                

                return cus;
            }, param, splitOn: "province_id, regency_id, subdistrict_id");

            return oList;
        }

        public Customer GetByID(string id)
        {
            Customer obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.customer_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<Customer> GetByName(string name)
        {
            IList<Customer> oList = new List<Customer>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_customer.name_customer) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_customer.name_customer");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<Customer> GetAll()
        {
            IList<Customer> oList = new List<Customer>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_customer.name_customer");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<Customer> GetAll(bool isReseller)
        {
            IList<Customer> oList = new List<Customer>();

            try
            {
                /*
                Func<Customer, bool> predicate = p => p.discount <= 0;

                if (isReseller)
                    predicate = p => p.discount > 0;

                oList = _context.db.GetAll<Customer>()
                                .Where(predicate)
                                .OrderBy(f => f.name_customer)
                                .ToList();*/

                if (isReseller)
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.discount > 0");
                }
                else
                {
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_customer.discount <= 0");                                      
                }

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_customer.name_customer");  
                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public int Save(Customer obj)
        {
            var result = 0;

            try
            {
                if (obj.customer_id == null)
                    obj.customer_id = _context.GetGUID();

                _context.db.Insert<Customer>(obj);
                result = 1;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Update(Customer obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Update<Customer>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }

        public int Delete(Customer obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<Customer>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return result;
        }        
    }
}     
