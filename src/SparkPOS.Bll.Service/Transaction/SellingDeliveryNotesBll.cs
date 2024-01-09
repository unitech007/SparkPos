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
//using SparkPOS.Logger;
using SparkPOS.Model.Transaction;

namespace SparkPOS.Bll.Service.Transaction
{
    public class SellingDeliveryNotesBll : ISellingDeliveryNotesBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private SellingDeliveryNotesValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;
     //   private ILogger _ILogger = Logger.Logger.GetInstance;
        public SellingDeliveryNotesBll(ILog log)
        {
            _log = log;
            _validator = new SellingDeliveryNotesValidator();
        }

        public SellingDeliveryNotesBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public int Delete(SellingDeliveryNotes obj)
        {
            throw new NotImplementedException();
        }

        public string GetLastDeliveryNotes()
        {
            var lastquotation = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastquotation = _unitOfWork.SellingDeliveryNotesRepository.GetLastDeliveryNotes();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastquotation = _unitOfWork.SellingDeliveryNotesRepository.GetLastDeliveryNotes();
                }
            }

            return lastquotation;
        }

        


        public List<string> GetInvoiceByCustomerId(string customerId)
        {
            List<string> quotations = new List<string>();

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                quotations = _unitOfWork.SellingDeliveryNotesRepository.GetInvoiceByCustomerId(customerId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    quotations = _unitOfWork.SellingDeliveryNotesRepository.GetInvoiceByCustomerId(customerId);
                }
            }

            return quotations;
        }

        public List<ItemSellingProduct> GetProductDetailsByInvoice(string invoiceNumber)
        {
            List<ItemSellingProduct> quotations = new List<ItemSellingProduct>();

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                quotations = _unitOfWork.SellingDeliveryNotesRepository.GetProductDetailsByInvoice(invoiceNumber);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    quotations = _unitOfWork.SellingDeliveryNotesRepository.GetProductDetailsByInvoice(invoiceNumber);
                }
            }

            return quotations;
        }
        ////public int Delete(SellingDeliveryNotes obj)
        ////{
        ////    var result = 0;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        result = _unitOfWork.SellingDeliveryNotesRepository.Delete(obj);
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            result = _unitOfWork.SellingDeliveryNotesRepository.Delete(obj);
        ////        }
        ////    }

        ////    return result;
        ////}

        ////public int Delete(SellingDeliveryNotes obj)
        ////{
        ////    throw new NotImplementedException();
        ////}

        ////public IList<SellingDeliveryNotes> GetAll()
        ////{
        ////    IList<SellingDeliveryNotes> oList = null;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll();
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll();
        ////        }
        ////    }

        ////    return oList;
        ////}

        ////public IList<SellingDeliveryNotes> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        ////{
        ////    IList<SellingDeliveryNotes> oList = null;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(pageNumber, pageSize, ref pagesCount);
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(pageNumber, pageSize, ref pagesCount);
        ////        }
        ////    }

        ////    return oList;
        ////}

        ////public IList<SellingDeliveryNotes> GetAll(string name)
        ////{
        ////    IList<SellingDeliveryNotes> oList = null;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(name);
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(name);
        ////        }
        ////    }

        ////    return oList;
        ////}

        ////public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        ////{
        ////    IList<SellingDeliveryNotes> oList = null;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai);
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai);
        ////        }
        ////    }

        ////    return oList;
        ////}

        ////public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        ////{
        ////    IList<SellingDeliveryNotes> oList = null;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
        ////        }
        ////    }

        ////    return oList;
        ////}

        ////public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        ////{
        ////    IList<SellingDeliveryNotes> oList = null;

        ////    if (_isUseWebAPI)
        ////    {
        ////        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        ////        oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
        ////    }
        ////    else
        ////    {
        ////        using (IDapperContext context = new DapperContext())
        ////        {
        ////            _unitOfWork = new UnitOfWork(context, _log);
        ////            oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
        ////        }
        ////    }

        ////    return oList;
        ////}

        //public SellingDeliveryNotes GetByID(string id)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<SellingDeliveryNotes> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<SellingDeliveryNotes> GetByName(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<SellingDeliveryNotes> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<SellingDeliveryNotes> GetInvoiceCustomer(string id, string invoice)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<SellingDeliveryNotes> GetInvoiceKreditByCustomer(string id, bool isLunas)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<SellingDeliveryNotes> GetInvoiceKreditByInvoice(string id, string invoice)
        //{
        //    throw new NotImplementedException();
        //}
        //public IList<ItemSellingDeliveryNotes> GetItemSelling(string jualId)
        //{
        //    IList<ItemSellingDeliveryNotes> oList = null;

        //    if (_isUseWebAPI)
        //    {
        //        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        //        oList = _unitOfWork.SellingDeliveryNotesRepository.GetItemSelling(jualId);
        //    }
        //    else
        //    {
        //        using (IDapperContext context = new DapperContext())
        //        {
        //            _unitOfWork = new UnitOfWork(context, _log);
        //            oList = _unitOfWork.SellingDeliveryNotesRepository.GetItemSelling(jualId);
        //        }
        //    }

        //    return oList;
        //}


        //public string GetLastDeliveryNotes()
        //{
        //    var lastDeliveryNotes = string.Empty;

        //    if (_isUseWebAPI)
        //    {
        //        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        //        lastDeliveryNotes = _unitOfWork.SellingDeliveryNotesRepository.GetLastDeliveryNotes();
        //    }
        //    else
        //    {
        //        using (IDapperContext context = new DapperContext())
        //        {
        //            _unitOfWork = new UnitOfWork(context, _log);
        //            lastDeliveryNotes = _unitOfWork.SellingDeliveryNotesRepository.GetLastDeliveryNotes();
        //        }
        //    }

        //    return lastDeliveryNotes;
        //}

        public SellingDeliveryNotes GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            throw new NotImplementedException();
        }

        //public int Save(SellingDeliveryNotes obj, ref ValidationError validationError)
        //{
        //    throw new NotImplementedException();
        //}

        public int Save(SellingDeliveryNotes obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.sale_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_jual)
                {
                    item.delivery_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingDeliveryNotesRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingDeliveryNotesRepository.Save(obj);
                }
            }

            return result;
        }

        public int Save(SellingDeliveryNotes obj, ref ValidationError validationError)
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
                //_ILogger.LogError(ex);

                return 0;
            }


        }

   


        public int Update(SellingDeliveryNotes obj, ref ValidationError validationError)
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

            return Update(obj);
        }
        public int Update(SellingDeliveryNotes obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                foreach (var item in obj.item_jual.Where(f => f.entity_state == EntityState.Added))
                {
                    item.delivery_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingDeliveryNotesRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingDeliveryNotesRepository.Update(obj);
                }
            }

            return result;
        }

        //public int Save(SellingDeliveryNotes obj, ref ValidationError validationError)
        //{
        //    try
        //    {
        //        var validatorResults = _validator.Validate(obj);

        //        if (!validatorResults.IsValid)
        //        {
        //            foreach (var failure in validatorResults.Errors)
        //            {
        //                validationError.Message = failure.ErrorMessage;
        //                validationError.PropertyName = failure.PropertyName;
        //                return 0;
        //            }
        //        }
        //        _ILogger.LogMessage("Save method is working");


        //        return Save(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ILogger.LogError(ex);

        //        return 0;
        //    }


        ////}
        //public int Save(SellingDeliveryNotes obj)
        //{
        //    var result = 0;

        //    if (_isUseWebAPI)
        //    {
        //        obj.DeliveryNotes_id = Guid.NewGuid().ToString();

        //        foreach (var item in obj.item_jual)
        //        {
        //            item.DeliveryNotes_item_id = Guid.NewGuid().ToString();
        //        }

        //        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        //        result = _unitOfWork.SellingDeliveryNotesRepository.Save(obj);
        //    }
        //    else
        //    {
        //        using (IDapperContext context = new DapperContext())
        //        {
        //            _unitOfWork = new UnitOfWork(context, _log);
        //            result = _unitOfWork.SellingDeliveryNotesRepository.Save(obj);
        //        }
        //    }

        //    return result;
        //}

        //public int Save(SellingDeliveryNotes obj, ref ValidationError validationError)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Save(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Update(SellingDeliveryNotes obj, ref ValidationError validationError)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Update(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Update(SellingDeliveryNotes obj, ref ValidationError validationError)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Update(SellingDeliveryNotes obj)
        //{
        //    throw new NotImplementedException();
        //}

        public IList<SellingDeliveryNotes> GetAll(string name)
        {
            IList<SellingDeliveryNotes> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(name);
                }
            }

            return oList;
        }


        public IList<SellingDeliveryNotes> GetAll()
        {
            IList<SellingDeliveryNotes> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll();
                }
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingDeliveryNotes> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingDeliveryNotesRepository.GetAll(pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }


        public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<SellingDeliveryNotes> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingDeliveryNotes> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<SellingDeliveryNotes> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingDeliveryNotesRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
                }
            }

            return oList;
        }

        //public IList<ItemSellingDeliveryNotes> GetItemSelling(string jualId)
        //{
        //    IList<ItemSellingDeliveryNotes> oList = null;

        //    if (_isUseWebAPI)
        //    {
        //        _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
        //        oList = _unitOfWork.SellingDeliveryNotesRepository.GetItemSelling(jualId);
        //    }
        //    else
        //    {
        //        using (IDapperContext context = new DapperContext())
        //        {
        //            _unitOfWork = new UnitOfWork(context, _log);
        //            oList = _unitOfWork.SellingDeliveryNotesRepository.GetItemSelling(jualId);
        //        }
        //    }

        //    return oList;
        //}
        SellingDeliveryNotes ISellingDeliveryNotesBll.GetByID(string id)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesBll.GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesBll.GetByName(string name)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesBll.GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesBll.GetInvoiceCustomer(string id, string invoice)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesBll.GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            throw new NotImplementedException();
        }

        IList<SellingDeliveryNotes> ISellingDeliveryNotesBll.GetInvoiceKreditByInvoice(string id, string invoice)
        {
            throw new NotImplementedException();
        }

     

      

        SellingDeliveryNotes ISellingDeliveryNotesBll.GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            throw new NotImplementedException();
        }

        //public IList<ItemSellingDeliveryNotes> GetItemSelling(string jualId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
