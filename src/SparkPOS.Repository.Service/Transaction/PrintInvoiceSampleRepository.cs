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
using Dapper;
using SparkPOS.Model.Invoice;
using SparkPOS.Repository.Api;
using SparkPOS.Model.quotation;

namespace SparkPOS.Repository.Service
{
    public class PrintInvoiceSampleRepository : IPrintInvoiceRepository
    {
        private IList<InvoiceSales> _listOfInvoiceSelling;
       // private IList<QuotationSales> _listOfQuotationSelling;

        public PrintInvoiceSampleRepository()
        {
            InisialisasiDataSample();
        } 
        
        

        private void InisialisasiDataSample()
        {
            _listOfInvoiceSelling = new List<InvoiceSales>();

            var itemInvoice1 = new InvoiceSales
            {
                name_customer = "Adhi Jaya",
                address = "Jl. Wonosari Km. 11",
                provinsi = "DI Yogayakrta",
                regency = "Kab. Bantul",
                subdistrict = "Piyungan",
                postal_code = "55792",
                phone = "0813 8176 9915",
                invoice = "201703210056",
                date = DateTime.Today,
                courier = "Tiki reguler",
                shipping_cost = 25000,
                total_invoice = 1542000,
                is_sdac = true,
                from_label1 = "PIXEL KOMPUTER",
                from_label2 = "HP: 0813 81769915",
                to_label1 = "Bpk. Sunardi",
                to_label2 = "Jl. Ring Road Utara",
                to_label3 = "Condong Catur - Sleman - Yogyakarta - 55283",
                to_label4 = "HP: 0813 2828282",
                product_code = "201704070001",
                product_name = "Flashdisk 2 Gb DEAM",
                price = 50000,
                quantity = 5
            };

            var itemInvoice2 = new InvoiceSales
            {
                name_customer = "Adhi Jaya",
                address = "Jl. Wonosari Km. 11",
                provinsi = "DI Yogayakrta",
                regency = "Kab. Bantul",
                subdistrict = "Piyungan",
                postal_code = "55792",
                phone = "0813 8176 9915",
                invoice = "201703210056",
                date = DateTime.Today,
                courier = "Tiki reguler",
                shipping_cost = 25000,
                total_invoice = 1542000,
                is_sdac = true,
                from_label1 = "PIXEL KOMPUTER",
                from_label2 = "HP: 0813 81769915",
                to_label1 = "Bpk. Sunardi",
                to_label2 = "Jl. Ring Road Utara",
                to_label3 = "Condong Catur - Sleman - Yogyakarta - 55283",
                to_label4 = "HP: 0813 2828282",
                product_code = "201704070002", product_name = "HDD 160 Gb SATA Seagate", price = 500000, quantity = 1
            };

            var itemInvoice3 = new InvoiceSales
            {
                name_customer = "Adhi Jaya",
                address = "Jl. Wonosari Km. 11",
                provinsi = "DI Yogayakrta",
                regency = "Kab. Bantul",
                subdistrict = "Piyungan",
                postal_code = "55792",
                phone = "0813 8176 9915",
                invoice = "201703210056",
                date = DateTime.Today,
                courier = "Tiki reguler",
                shipping_cost = 25000,
                total_invoice = 1542000,
                is_sdac = true,
                from_label1 = "PIXEL KOMPUTER",
                from_label2 = "HP: 0813 81769915",
                to_label1 = "Bpk. Sunardi",
                to_label2 = "Jl. Ring Road Utara",
                to_label3 = "Condong Catur - Sleman - Yogyakarta - 55283",
                to_label4 = "HP: 0813 2828282",
                product_code = "201704070003", product_name = "LCD 16 in Samsung 633NW", price = 800000, quantity = 1, discount = 1
            };

            _listOfInvoiceSelling.Add(itemInvoice1);
            _listOfInvoiceSelling.Add(itemInvoice2);
            _listOfInvoiceSelling.Add(itemInvoice3);
        }

        public IList<InvoicePurchase> GetInvoicePurchase(string beliProductId)
        {
            throw new NotImplementedException();
        }

        public IList<InvoiceSales> GetInvoiceSales(string jualProductId)
        {
            return _listOfInvoiceSelling;
        }

       
    }
}
