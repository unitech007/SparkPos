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
using SparkPOS.Model.Transaction;

namespace SparkPOS.Bll.Service.Transaction
{
    public class SellingQuotationBll : ISellingQuotationBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private SellingQuotationValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;
       // private ILogger _ILogger = Logger.Logger.GetInstance;
        public SellingQuotationBll(ILog log)
        {
            _log = log;
            _validator = new SellingQuotationValidator();
        }

        public SellingQuotationBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public int Delete(SellingQuotation obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingQuotationRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingQuotationRepository.Delete(obj);
                }
            }

            return result;
        }



        public IList<SellingQuotation> GetAll()
        {
            IList<SellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetAll();
                }
            }

            return oList;
        }

        public IList<SellingQuotation> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetAll(pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetAll(pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public IList<SellingQuotation> GetAll(string name)
        {
            IList<SellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetAll(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetAll(name);
                }
            }

            return oList;
        }

        public IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<SellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }

            return oList;
        }

        public IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<SellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
                }
            }

            return oList;
        }

        public SellingQuotation GetByID(string id)
        {
            throw new NotImplementedException();
        }

        public IList<SellingQuotation> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            throw new NotImplementedException();
        }

        public IList<SellingQuotation> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<SellingQuotation> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            throw new NotImplementedException();
        }

        public IList<SellingQuotation> GetInvoiceCustomer(string id, string invoice)
        {
            throw new NotImplementedException();
        }

        public IList<SellingQuotation> GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            throw new NotImplementedException();
        }

        public IList<SellingQuotation> GetInvoiceKreditByInvoice(string id, string invoice)
        {
            throw new NotImplementedException();
        }
        public IList<ItemSellingQuotation> GetItemSelling(string jualId)
        {
            IList<ItemSellingQuotation> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingQuotationRepository.GetItemSelling(jualId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingQuotationRepository.GetItemSelling(jualId);
                }
            }

            return oList;
        }


        public string GetLastQuotation()
        {
            var lastquotation = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastquotation = _unitOfWork.SellingQuotationRepository.GetLastQuotation();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastquotation = _unitOfWork.SellingQuotationRepository.GetLastQuotation();
                }
            }

            return lastquotation;
        }

        public SellingQuotation GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            throw new NotImplementedException();
        }

        public int Save(SellingQuotation obj, ref ValidationError validationError)
        {
            try
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
                //_ILogger.LogMessage("Save method is working");


                return Save(obj);
            }
            catch (Exception ex)
            {
                Config.LogException(ex);
                //  _ILogger.LogError(ex);

                return 0;
            }


        }
        public int Save(SellingQuotation obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.quotation_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_jual)
                {
                    item.quotation_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingQuotationRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingQuotationRepository.Save(obj);
                }
            }

            return result;
        }

        public int Update(SellingQuotation obj, ref ValidationError validationError)
        {
            throw new NotImplementedException();
        }

        public int Update(SellingQuotation obj)
        {
            throw new NotImplementedException();
        }
    }
}
