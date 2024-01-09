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

namespace SparkPOS.Bll.Service
{
    public class SellingProductBll : ISellingProductBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;
        private SellingProductValidator _validator;

        private bool _isUseWebAPI;
        private string _baseUrl;
      //  private ILogger _ILogger = Logger.Logger.GetInstance;
        public SellingProductBll(ILog log)
        {
            _log = log;
            _validator = new SellingProductValidator();
        }

        public SellingProductBll(bool isUseWebAPI, string baseUrl, ILog log)
            : this(log)
        {
            _isUseWebAPI = isUseWebAPI;
            _baseUrl = baseUrl;
        }

        public SellingProduct GetByID(string id)
        {
            SellingProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.SellingProductRepository.GetByID(id);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.SellingProductRepository.GetByID(id);
                }
            }

            return obj;
        }

        public SellingProduct GetListItemInvoiceTerakhir(string userId, string mesinId)
        {
            SellingProduct obj = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                obj = _unitOfWork.SellingProductRepository.GetListItemInvoiceTerakhir(userId, mesinId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    obj = _unitOfWork.SellingProductRepository.GetListItemInvoiceTerakhir(userId, mesinId);
                }
            }

            return obj;
        }

        public IList<SellingProduct> GetByName(string name)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetByName(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetByName(name);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetByName(name, isCekKeteranganItemSelling, pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetByName(name, isCekKeteranganItemSelling, pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetAll()
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetAll();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetAll();
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetAll(pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetAll(pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public int Save(SellingProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                obj.sale_id = Guid.NewGuid().ToString();

                foreach (var item in obj.item_jual)
                {
                    item.sale_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingProductRepository.Save(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingProductRepository.Save(obj);
                }
            }

            return result;
        }

        public int Save(SellingProduct obj, ref ValidationError validationError)
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

        public int Update(SellingProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                foreach (var item in obj.item_jual.Where(f => f.entity_state == EntityState.Added))
                {
                    item.sale_item_id = Guid.NewGuid().ToString();
                }

                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingProductRepository.Update(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingProductRepository.Update(obj);
                }
            }

            return result;
        }

        public int Update(SellingProduct obj, ref ValidationError validationError)
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

        public int Delete(SellingProduct obj)
        {
            var result = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                result = _unitOfWork.SellingProductRepository.Delete(obj);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    result = _unitOfWork.SellingProductRepository.Delete(obj);
                }
            }

            return result;
        }
        //private void LoadQuotations()
        //{
        //    using (IDapperContext dbContext = new DapperContext())
        //    {
        //        List<string> quotations = dbContext.t_sales_quotation.Select(q => q.quotation).ToList();

        //        cmbQuotation.Items.Clear();
        //        cmbQuotation.Items.Add("Select");

        //        foreach (string quotation in quotations)
        //        {
        //            cmbQuotation.Items.Add(quotation);
        //        }

        //        if (!string.IsNullOrEmpty(this._jual.SellingQuotation?.quotation))
        //        {
        //            cmbQuotation.Text = this._jual.SellingQuotation.quotation;
        //        }
        //        else
        //        {
        //            cmbQuotation.SelectedIndex = 0;
        //        }
        //    }
        //}

        public string GetLastInvoice()
        {
            var lastInvoice = string.Empty;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                lastInvoice = _unitOfWork.SellingProductRepository.GetLastInvoice();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    lastInvoice = _unitOfWork.SellingProductRepository.GetLastInvoice();
                }
            }

            return lastInvoice;
        }


        public List<string> GetQuotationsByCustomerId(string customerId)
        {
            List<string> quotations = new List<string>();

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                quotations = _unitOfWork.SellingProductRepository.GetQuotationsByCustomerId(customerId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    quotations = _unitOfWork.SellingProductRepository.GetQuotationsByCustomerId(customerId);
                }
            }

            return quotations;
        }

        public List<Tax> GetTaxNames()
        {
            List<Tax> taxNames = new List<Tax>();

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                taxNames = _unitOfWork.SellingProductRepository.GetTaxNames();
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    taxNames = _unitOfWork.SellingProductRepository.GetTaxNames();
                }
            }

            return taxNames;
        }


        public List<ItemSellingQuotation> GetProductDetailsByQUotation(string quotationNumber)
        {
            List<ItemSellingQuotation> quotations = new List<ItemSellingQuotation>();

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                quotations = _unitOfWork.SellingProductRepository.GetProductDetailsByQUotation(quotationNumber);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    quotations = _unitOfWork.SellingProductRepository.GetProductDetailsByQUotation(quotationNumber);
                }
                }

            return quotations;
        }

        public double GetTaxRate(string taxName)
        {
            double taxRate = 0;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                taxRate = _unitOfWork.SellingProductRepository.GetTaxRate(taxName);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    taxRate = _unitOfWork.SellingProductRepository.GetTaxRate(taxName);
                }
            }

            return taxRate;
        }

        public IList<SellingProduct> GetAll(string name)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetAll(name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetAll(name);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetInvoiceCustomer(string id, string invoice)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetInvoiceCustomer(id, invoice);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetInvoiceCustomer(id, invoice);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetInvoiceKreditByCustomer(string id, bool isLunas)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetInvoiceKreditByCustomer(id, isLunas);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetInvoiceKreditByCustomer(id, isLunas);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetInvoiceKreditByInvoice(string id, string invoice)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetInvoiceKreditByInvoice(id, invoice);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetInvoiceKreditByInvoice(id, invoice);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai, pageNumber, pageSize, ref pagesCount);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetByDate(tanggalMulai, tanggalSelesai, name);
                }
            }

            return oList;
        }

        public IList<SellingProduct> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            IList<SellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetByLimit(tanggalMulai, tanggalSelesai, limit);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetByLimit(tanggalMulai, tanggalSelesai, limit);
                }
            }

            return oList;
        }

        public IList<ItemSellingProduct> GetItemSelling(string jualId)
        {
            IList<ItemSellingProduct> oList = null;

            if (_isUseWebAPI)
            {
                _unitOfWork = new UnitOfWork(_isUseWebAPI, _baseUrl, _log);
                oList = _unitOfWork.SellingProductRepository.GetItemSelling(jualId);
            }
            else
            {
                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    oList = _unitOfWork.SellingProductRepository.GetItemSelling(jualId);
                }
            }

            return oList;
        }

       
    }
}
