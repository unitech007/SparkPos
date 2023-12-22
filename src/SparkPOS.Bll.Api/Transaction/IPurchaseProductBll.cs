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
 
namespace SparkPOS.Bll.Api
{    
    public interface IPurchaseProductBll : IBaseBll<PurchaseProduct>
    {

        List<Tax> GetTaxNames();
        string GetLastInvoice();
        PurchaseProduct GetByID(string id);

        IList<PurchaseProduct> GetAll(string name);
        IList<PurchaseProduct> GetAll(int pageNumber, int pageSize, ref int pagesCount);

        IList<PurchaseProduct> GetInvoiceSupplier(string id, string invoice);
        IList<PurchaseProduct> GetInvoiceKreditBySupplier(string id, bool isLunas);
        IList<PurchaseProduct> GetInvoiceKreditByInvoice(string id, string invoice);
        
        IList<PurchaseProduct> GetByName(string name);
        IList<PurchaseProduct> GetByName(string name, int pageNumber, int pageSize, ref int pagesCount);

        IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount);
        IList<PurchaseProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name);

        IList<ItemPurchaseProduct> GetItemPurchase(string beliId);

		int Save(PurchaseProduct obj, ref ValidationError validationError);
		int Update(PurchaseProduct obj, ref ValidationError validationError);
    }
}     
