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
using System.Linq;

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
using SparkPOS.Helper;
using System.IO;
using Newtonsoft.Json;

namespace SparkPOS.Bll.Service
{    
    public class UserBll : IUserBll
    {
		private ILog _log;
        private IUnitOfWork _unitOfWork;
		private UserValidator _validator;

		public UserBll(ILog log)
        {
			_log = log;
            _validator = new UserValidator();
        }

        public User GetByID(string userName)
        {
            User obj = null;
            
            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                obj = _unitOfWork.UserRepository.GetByID(userName);
            }

            return obj;
        }

        public bool IsValidUser(string userName, string password)
        {
            var result = false;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.UserRepository.IsValidUser(userName, password);
            }

            return result;
        }

        //public bool IsValidUser(string userName, string password)
        //{
        //    var result = false;

        //    using (IDapperContext context = new DapperContext())
        //    {
        //        _unitOfWork = new UnitOfWork(context, _log);
        //        var user = _unitOfWork.UserRepository.GetByID(userName);
        //        if (user != null)
        //        {
        //            var configJson = File.ReadAllText("config.json");
        //            var config = JsonConvert.DeserializeObject<Config>(configJson);

        //            // Convert the stored password to bytes before hashing
        //            var storedPasswordBytes = StringToByteArray(user.user_password);

        //            var hashedPassword = CryptoHelper.GetMD5Hash(password, config.SecurityCode);
        //            var hashedPasswordBytes = Encoding.UTF8.GetBytes(hashedPassword);
        //            result = (user.is_active == true) && hashedPasswordBytes.SequenceEqual(storedPasswordBytes);


        //           // result = (user.is_active == true) && hashedPassword.SequenceEqual(storedPasswordBytes);
        //        }
        //    }

        //    return result;
        //}

        // Helper method to convert a hexadecimal string to a byte array
        private static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        //public bool IsValidUser(string userName, string password)
        //{
        //    var result = false;

        //    using (IDapperContext context = new DapperContext())
        //    {
        //        _unitOfWork = new UnitOfWork(context, _log);
        //        var user = _unitOfWork.UserRepository.GetByID(userName);
        //        if (user != null)
        //        {
        //            var configJson = File.ReadAllText("config.json");
        //            var config = JsonConvert.DeserializeObject<Config>(configJson);

        //            var hashedPassword = CryptoHelper.GetMD5Hash(password, config.SecurityCode);
        //            //var hashedPassword = CryptoHelper.GetMD5Hash(password, MainProgram.securityCode);
        //            result = (user.is_active == true) && (hashedPassword == user.user_password);
        //        }
        //    }

        //    return result;
        //}

        //public bool IsValidUser(string userName, string password)
        //{
        //    var result = false;

        //    using (IDapperContext context = new DapperContext())
        //    {
        //        _unitOfWork = new UnitOfWork(context, _log);
        //        result = _unitOfWork.UserRepository.IsValidUser(userName, password);
        //    }

        //    return result;
        //}
        public IList<User> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetAll()
        {
            IList<User> oList = null;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.UserRepository.GetAll();
            }

            return oList;
        }

		public int Save(User obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.UserRepository.Save(obj);
            }

            return result;
        }

        public int Save(User obj, ref ValidationError validationError)
        {
			var validatorResults = _validator.Validate(obj);

            if (!validatorResults.IsValid)
            {
                foreach (var failure in validatorResults.Errors)
                {
                    validationError.Message = failure.ErrorMessage;
                    validationError.PropertyName = failure.PropertyName;
                    return 0;
                }
            }

            obj.user_password = obj.konf_user_password;
            return Save(obj);
        }

		public int Update(User obj)
        {
            var result = 0;

            obj.user_password = obj.konf_user_password;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.UserRepository.Update(obj);
            }

            return result;
        }

        public int Delete(User obj)
        {
            var result = 0;

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                result = _unitOfWork.UserRepository.Delete(obj);
            }

            return result;
        }        
    }
}     
