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
    public class UserRepository : IUserRepository
    {
        private const string SQL_TEMPLATE = @"SELECT m_user.user_id, m_user.name_user, m_user.user_password, m_user.is_active, m_user.status_user, 
                                              m_role.role_id, m_role.name_role
                                              FROM public.m_user LEFT JOIN public.m_role ON m_user.role_id = m_role.role_id
                                              {WHERE}
                                              {ORDER BY}";
        private IDapperContext _context;
        private ILog _log;

        private string _sql;
		
        public UserRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<User> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<User> oList = _context.db.Query<User, Role, User>(sql, (p, r) =>
            {
                if (r != null)
                    p.role_id = r.role_id; p.Role = r;

                return p;
            }, param, splitOn: "role_id");

            return oList;
        }

        public User GetByID(string userName)
        {
            User obj = null;

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_user.name_user) = LOWER(@userName)");
                _sql = _sql.Replace("{ORDER BY}", "");

                obj = MappingRecordToObject(_sql, new { userName }).SingleOrDefault();

                if (obj != null)
                {
                    IRolePrivilegeRepository rolePrivilegeRepository = new RolePrivilegeRepository(_context, _log);
                    obj.role_privileges = rolePrivilegeRepository.GetByRole(obj.role_id);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public bool IsValidUser(string userName, string password)
        {
            var result = false;

            var user = GetByID(userName);

            if (user != null)
            {
                result = (user.is_active == true) && (password == user.user_password);
            }

            return result;
        }

        public IList<User> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetAll()
        {
            IList<User> oList = new List<User>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_user.name_user");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(User obj)
        {
            var result = 0;

            try
            {
                var user = GetByID(obj.name_user);
                if (user != null)
                    return 0; // name user already terlist

                obj.user_id = _context.GetGUID();

                // password already dienkripsi dari application
                _context.db.Insert<User>(obj);

                result = 1;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(User obj)
        {
            var result = 0;

            try
            {
                // password already dienkripsi dari application
                if (obj.user_password != null && obj.user_password.Length > 0)
                {
                    _sql = @"UPDATE m_user SET name_user = @name_user, user_password = @user_password, role_id = @role_id, is_active = @is_active,
                             status_user = @status_user
                             WHERE user_id = @user_id";
                }
                else
                {
                    _sql = @"UPDATE m_user SET name_user = @name_user, role_id = @role_id, is_active = @is_active, status_user = @status_user
                             WHERE user_id = @user_id";
                }

                result = _context.db.Execute(_sql, obj);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(User obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<User>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }        
    }
}     
