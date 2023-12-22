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

using log4net;
using SparkPOS.Model.Invoice;
using SparkPOS.Bll.Api;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
using ZXing;
using ZXing.Common;
using System.IO;
using SparkPOS.Model.quotation;
using SparkPOS.Model;

namespace SparkPOS.Bll.Service
{
    public class PrintSellingDeliveryNotesBll : IPrintSellingDeliveryNotesBll
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        public PrintSellingDeliveryNotesBll(ILog log)
        {
            _log = log;
        }

        public IList<DeliveryNotesSales> GetSellingDeliveryNotes(string jualProductId)
        {
            IList<DeliveryNotesSales> oList = new List<DeliveryNotesSales>();

            using (IDapperContext context = new DapperContext())
            {
                _unitOfWork = new UnitOfWork(context, _log);
                oList = _unitOfWork.PrintDeliveryNotesRepository.GetSellingDeliveryNotes(jualProductId);
            }

            foreach (var item in oList)
            {

                item.provinsi = string.IsNullOrEmpty(item.provinsi) ? "" : item.provinsi;
                item.regency = string.IsNullOrEmpty(item.regency) ? "" : item.regency;
                item.subdistrict = string.IsNullOrEmpty(item.subdistrict) ? "" : item.subdistrict;

                item.postal_code = (string.IsNullOrEmpty(item.postal_code) || item.postal_code == "0") ? "" : item.postal_code;
                item.phone = string.IsNullOrEmpty(item.phone) ? "" : item.phone;
                item.shipping_country = string.IsNullOrEmpty(item.shipping_country) ? "" : item.shipping_country;
                item.shipping_regency = string.IsNullOrEmpty(item.shipping_regency) ? "" : item.shipping_regency;

                item.shipping_subdistrict = string.IsNullOrEmpty(item.shipping_subdistrict) ? "" : item.shipping_subdistrict;
                item.shipping_village = string.IsNullOrEmpty(item.shipping_village) ? "" : item.shipping_village;
                item.shipping_city = string.IsNullOrEmpty(item.shipping_city) ? "" : item.shipping_city;
                item.shipping_postal_code = string.IsNullOrEmpty(item.shipping_postal_code) ? "" : item.shipping_postal_code;
                item.shipping_phone = string.IsNullOrEmpty(item.shipping_phone) ? "" : item.shipping_phone;
                //item.is_sdac = true;
                item.from_label1 = string.IsNullOrEmpty(item.from_label1) ? "" : item.from_label1;
                item.from_label2 = string.IsNullOrEmpty(item.from_label2) ? "" : item.from_label2;

                item.to_label1 = string.IsNullOrEmpty(item.to_label1) ? item.name_customer : item.to_label1;
                item.to_label2 = string.IsNullOrEmpty(item.to_label2) ? item.address : item.to_label2;
                item.to_label3 = string.IsNullOrEmpty(item.to_label3) ? "HP: " + item.phone : item.to_label3;
                item.to_label4 = string.IsNullOrEmpty(item.to_label4) ? string.Empty : item.to_label4;
            }

            return oList;
        }

    }
}
