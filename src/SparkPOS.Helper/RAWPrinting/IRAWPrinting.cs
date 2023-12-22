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

using SparkPOS.Model;
using SparkPOS.Model.Report;
using SparkPOS.Model.Transaction;

namespace SparkPOS.Helper.RAWPrinting
{
    public interface IRAWPrinting
    {
        /// <summary>
        /// Override method untuk menprint invoice sale menggunakan printer mini pos
        /// </summary>
        /// <param name="sale">objek sale</param>
        /// <param name="listOfHeaderInvoice">list objek header invoice</param>
        /// <param name="listOfFooterInvoice">list objek footer invoice</param>        
        /// <param name="jumlahKarakter">maksimal quantity karakter yang terprint</param>
        /// <param name="lineFeed">quantity gulung kertas setelah penprintan selesai</param>
        void Print(SellingProduct sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice,
            int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0, 
            string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "");

        void Print(SellingQuotation sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice,
            int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0,
            string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "");

        void Print(SellingDeliveryNotes sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice,
         int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0,
         string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "");


        /// <summary>
        /// Override method untuk menprint report Cashier menggunakan printer mini pos
        /// </summary>
        /// <param name="listOfCashierMachine"></param>
        /// <param name="listOfHeaderInvoice"></param>
        /// <param name="jumlahKarakter"></param>
        void Print(IList<ReportCashierMachine> listOfCashierMachine, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, int jumlahKarakter, int lineFeed, int FontSize = 0,
            string autocutCode = "", string infoCopyright1 = "", string infoCopyright2 = "");

        /// <summary>
        /// Override method untuk menprint invoice sale menggunakan printer dot matrix
        /// </summary>
        /// <param name="sale">Objek sale</param>
        /// <param name="listOfHeaderInvoice">List of header invoice</param>
        /// <param name="jumlahRow">Maksimal quantity row yang terprint dalam one halaman</param>
        /// <param name="jumlahKarakter">Maksimal quantity karakter/column</param>
        void Print(SellingProduct sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "");

        void Print(SellingQuotation sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "");
        void Print(SellingDeliveryNotes sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "");
    }
}
