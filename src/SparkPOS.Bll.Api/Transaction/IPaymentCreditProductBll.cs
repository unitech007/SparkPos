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
    public interface IPaymentCreditProductBll : IBaseBll<PaymentCreditProduct>
    {
        string GetLastInvoice();
        PaymentCreditProduct GetByID(string id);
        ItemPaymentCreditProduct GetBySellingID(string id);
        IList<PaymentCreditProduct> GetByName(string name);
        IList<PaymentCreditProduct> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<ItemPaymentCreditProduct> GetPaymentHistory(string jualId);

		int Save(PaymentCreditProduct obj, bool isSaveFromSales, ref ValidationError validationError);
		int Update(PaymentCreditProduct obj, bool isSaveFromSales, ref ValidationError validationError);
    }
}     
