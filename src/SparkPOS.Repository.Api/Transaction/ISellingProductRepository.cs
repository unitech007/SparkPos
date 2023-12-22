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

using SparkPOS.Model;
using SparkPOS.Model.Transaction;

namespace SparkPOS.Repository.Api
{        
    public interface ISellingProductRepository : IBaseRepository<SellingProduct>
    {
        double GetTaxRate(string taxName);
        List<Tax> GetTaxNames();
       // List<object> GetTaxNames();
        string GetLastInvoice();
		SellingProduct GetByID(string id);
       List<string> GetQuotationsByCustomerId(string customerId);
        /// <summary>
        /// Method untuk mendapatkan Information item invoice terakhir untuk keperluan application Cashier
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mesinId"></param>
        /// <returns></returns>
        SellingProduct GetListItemInvoiceTerakhir(string userId, string mesinId);

        List<ItemSellingQuotation> GetProductDetailsByQUotation(string quotationNumber);
        IList<SellingProduct> GetAll(string name);
      //  List<string> GetAllQuotations(string name);
        IList<SellingProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase masing-masing supplier
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        IList<SellingProduct> GetInvoiceCustomer(string id, string invoice);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase Credit yang lunas/Not yet masing-masing supplier
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="isLunas"></param>
        /// <returns></returns>
        IList<SellingProduct> GetInvoiceKreditByCustomer(string id, bool isLunas);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase Credit yang lunas/Not yet masing-masing customer based invoice
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        IList<SellingProduct> GetInvoiceKreditByInvoice(string id, string invoice);

        IList<SellingProduct> GetByName(string name);
        IList<SellingProduct> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name);

        IList<SellingProduct> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit);

        IList<ItemSellingProduct> GetItemSelling(string jualId);
    }
}     
