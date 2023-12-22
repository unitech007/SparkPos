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
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
 
namespace SparkPOS.Bll.Service
{    
    public class MenuBll : IMenuBll
    {
		private ILog _log;
        private IUnitOfWork _unitOfWork;

		public MenuBll(ILog log)
        {
			_log = log;
        }

        public MenuApplication GetByID(string id)
        {
            MenuApplication obj = null;
            
            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                obj = _unitOfWork.MenuRepository.GetByID(id);
            }

            return obj;
        }

        public MenuApplication GetByName(string name)
        {
            MenuApplication obj = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                obj = _unitOfWork.MenuRepository.GetByName(name);
            }

            return obj;
        }

        public IList<MenuApplication> GetAll()
        {
            IList<MenuApplication> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.MenuRepository.GetAll();
            }

            return oList;
        }

        public int Save(MenuApplication obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.MenuRepository.Save(obj);
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
