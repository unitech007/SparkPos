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
    public class PrinterDotMatrix : IRAWPrinting
    {
        private string _printerName = string.Empty;

        public PrinterDotMatrix(string printerName)
        {
            _printerName = printerName;
        }

        public void Print(SellingProduct sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice,
            int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0,
            string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
        {
            throw new NotImplementedException();
        }

        public void Print(IList<ReportCashierMachine> listOfCashierMachine, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, int jumlahKarakter, int lineFeed, int FontSize = 0,
            string autocutCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
        {
            throw new NotImplementedException();
        }

        public void Print(SellingProduct sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "")
        {
            var garisPemisah = StringHelper.PrintChar('=', jumlahKarakter);

            var textToPrint = new StringBuilder();

            var rowCount = 0; // quantity row yang terprint

            if (!Utils.IsRunningUnderIDE())
            {
                textToPrint.Append(ESCCommandHelper.InitializePrinter());
                textToPrint.Append(ESCCommandHelper.CenterText());
            }

            // print header
            foreach (var header in listOfHeaderInvoice)
            {
                if (header.description.Length > 0)
                {
                    if (header.description.Length > garisPemisah.Length)
                    {
                        header.description = StringHelper.FixedLength(header.description, garisPemisah.Length);
                    }

                    textToPrint.Append(header.description).Append(ESCCommandHelper.LineFeed(1));

                    rowCount++;
                }
            }

            textToPrint.Append(ESCCommandHelper.LineFeed(1));
            rowCount++;

            if (!Utils.IsRunningUnderIDE())
            {
                textToPrint.Append(ESCCommandHelper.LeftText());
            }

            // print Information invoice
            textToPrint.Append("Invoice   : ").Append(sale.invoice).Append(ESCCommandHelper.LineFeed(1));
            textToPrint.Append("date: ").Append(DateTimeHelper.DateToString(sale.date));

            if (sale.due_date != null)
            {
                textToPrint.Append(StringHelper.PrintChar(' ', 4));
                textToPrint.Append("CreditTerm: ").Append(DateTimeHelper.DateToString(sale.due_date)).Append(ESCCommandHelper.LineFeed(2));
            }
            else
            {
                textToPrint.Append(ESCCommandHelper.LineFeed(2));
            }

            rowCount += 4;

            // print Information customer
            var nameCustomer = string.Empty;

            if (sale.is_sdac)
            {
                if (sale.Customer != null)
                {
                    nameCustomer = StringHelper.FixedLength(sale.Customer.name_customer.NullToString(), jumlahKarakter - 10);
                }
            }
            else
            {
                nameCustomer = StringHelper.FixedLength(sale.shipping_to.NullToString(), jumlahKarakter - 10);
            }

            if (nameCustomer.Length > 0)
            {
                var address1 = sale.is_sdac ? sale.Customer.address.NullToString() : sale.shipping_address.NullToString();
                var address2 = string.Empty;

                var sb = new StringBuilder();

                if (sale.is_sdac)
                {
                    var customer = sale.Customer;
                    address2 = customer == null ? string.Empty : customer.get_region_lengkap;
                }
                else
                {
                    address2 = sale.shipping_subdistrict;
                }

                textToPrint.Append("to : ").Append(nameCustomer).Append(ESCCommandHelper.LineFeed(1));
                rowCount++;

                textToPrint.Append("Address : ");

                var isAddLineFeed = true;

                if (address1.Length > 0)
                {
                    textToPrint.Append(StringHelper.FixedLength(address1, jumlahKarakter - 10)).Append(ESCCommandHelper.LineFeed(1));
                    rowCount++;
                    isAddLineFeed = false;
                }

                if (address2.Length > 0)
                {
                    textToPrint.Append(StringHelper.PrintChar(' ', 9)).Append(StringHelper.FixedLength(address2, jumlahKarakter - 10)).Append(ESCCommandHelper.LineFeed(1));
                    rowCount++;
                    isAddLineFeed = false;
                }

                if (isAddLineFeed)
                {
                    textToPrint.Append(ESCCommandHelper.LineFeed(1));
                }
            }                        

            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            // header tabel
            textToPrint.Append("NO");
            textToPrint.Append(StringHelper.PrintChar(' ', 2));
            textToPrint.Append("PRODUCT");
            textToPrint.Append(StringHelper.PrintChar(' ', 34));
            textToPrint.Append("quantity");
            textToPrint.Append(StringHelper.PrintChar(' ', 2));
            textToPrint.Append("price");
            textToPrint.Append(StringHelper.PrintChar(' ', 4));
            textToPrint.Append("DISC (%)");
            textToPrint.Append(StringHelper.PrintChar(' ', 2));
            textToPrint.Append("SUB TOTAL").Append(ESCCommandHelper.LineFeed(1));

            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            rowCount += 3;

            var lengthProduct = 37;
            var lengthJumlah = 5;
            var lengthPrice = 9;
            var lengthDisc = 5;
            var lengthSubTotal = 11;

            // print item
            var noUrut = 1;
            foreach (var item in sale.item_jual)
            {
                var strNoUrut = StringHelper.RightAlignment(noUrut.ToString(), 2);
                textToPrint.Append(strNoUrut).Append(StringHelper.PrintChar(' ', 2));

                var product = StringHelper.FixedLength(item.Product.product_name, lengthProduct);
                textToPrint.Append(product).Append(StringHelper.PrintChar(' ', 2));

                var strJumlah = StringHelper.RightAlignment((item.quantity - item.return_quantity).ToString(), lengthJumlah);
                textToPrint.Append(strJumlah).Append(StringHelper.PrintChar(' ', 2));

                var strPrice = StringHelper.RightAlignment(NumberHelper.NumberToString(item.harga_setelah_diskon), lengthPrice);
                textToPrint.Append(strPrice).Append(StringHelper.PrintChar(' ', 2));

                var strDisc = StringHelper.RightAlignment((item.discount).ToString(), lengthDisc);
                textToPrint.Append(strDisc).Append(StringHelper.PrintChar(' ', 3));

                var subTotal = (item.quantity - item.return_quantity) * item.harga_setelah_diskon;
                var strSubTotal = StringHelper.RightAlignment(NumberHelper.NumberToString(subTotal), lengthSubTotal);
                textToPrint.Append(strSubTotal).Append(ESCCommandHelper.LineFeed(1));

                if (item.description.Length > 0)
                    textToPrint.Append(StringHelper.PrintChar(' ', 4)).Append(item.description).Append(ESCCommandHelper.LineFeed(1));

                noUrut++;
                rowCount++;
            }

            // print footer
            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            var lengthCostShipping = 11;

            if (sale.shipping_cost > 0)
            {
                textToPrint.Append(StringHelper.PrintChar(' ', 56));
                var strCostShipping = StringHelper.RightAlignment(NumberHelper.NumberToString(sale.shipping_cost), lengthCostShipping);
                textToPrint.Append("Cost Shipping:").Append(strCostShipping).Append(ESCCommandHelper.LineFeed(1));

                rowCount++;
            }
            
            if (sale.discount > 0)
            {
                var strDiscInvoice = StringHelper.RightAlignment(NumberHelper.NumberToString(sale.discount), lengthCostShipping);

                textToPrint.Append(StringHelper.PrintChar(' ', 56));
                textToPrint.Append("discount      :").Append(strDiscInvoice).Append(ESCCommandHelper.LineFeed(1));

                rowCount++;
            }

            if (sale.tax > 0)
            {
                var strPPN = StringHelper.RightAlignment(NumberHelper.NumberToString(sale.tax), lengthCostShipping);

                textToPrint.Append(StringHelper.PrintChar(' ', 56));
                textToPrint.Append("tax         :").Append(strPPN).Append(ESCCommandHelper.LineFeed(1));

                rowCount++;
            }

            var strGrandTotal = StringHelper.RightAlignment(NumberHelper.NumberToString(sale.grand_total), lengthCostShipping);
            textToPrint.Append(StringHelper.PrintChar(' ', 56));
            textToPrint.Append("Total       :").Append(strGrandTotal).Append(ESCCommandHelper.LineFeed(1));

            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            textToPrint.Append(StringHelper.PrintChar(' ', 8)).Append("Penerima");
            textToPrint.Append(StringHelper.PrintChar(' ', 40)).Append("Hormat Kami");
            textToPrint.Append(ESCCommandHelper.LineFeed(3));

            textToPrint.Append(StringHelper.PrintChar(' ', 6)).Append("------------");
            textToPrint.Append(StringHelper.PrintChar(' ', 37)).Append("-------------");

            if (isPrintKeteranganInvoice && sale.description.NullToString().Length > 0)
            {
                textToPrint.Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append(ESCCommandHelper.LineFeed(1)).Append("Description: ").Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append("* ");

                var splitKeterangan = StringHelper.SplitByLength(sale.description, 78);
                foreach (var ket in splitKeterangan)
                {
                    textToPrint.Append(ket).Append(ESCCommandHelper.LineFeed(1));
                }

            }

            if (infoCopyright.Length > 0)
            {
                textToPrint.Append(ESCCommandHelper.LineFeed(1));

                if (!Utils.IsRunningUnderIDE())
                {
                    textToPrint.Append(ESCCommandHelper.CenterText());
                }

                textToPrint.Append(infoCopyright).Append(ESCCommandHelper.LineFeed(1));
            }

            rowCount += 6;

            // perhitungan remaining kertas untuk keperluan line feed
            var listOfMaxRow = new Dictionary<int, int>();
            for (int i = 1; i < 11; i++)
            {
                var key = jumlahRow * i;
                var value = key + 4;

                listOfMaxRow.Add(key, value);
            }

            var maxJumlahRow = 0; // maksimal quantity row terprint dalam one halaman
            foreach (var item in listOfMaxRow)
            {
                maxJumlahRow = item.Value;

                if (rowCount <= item.Key)
                    break;
            }

            var lineFeed = maxJumlahRow - rowCount;

            textToPrint.Append(ESCCommandHelper.LineFeed(lineFeed));

            if (!Utils.IsRunningUnderIDE())
            {
                RawPrintHelper.SendStringToPrinter(_printerName, textToPrint.ToString());
            }
            else
            {
                RawPrintHelper.SendStringToFile(textToPrint.ToString());
            }
        }

        

        public  void Print(SellingQuotation sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice,
            int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0,
            string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
        {
            throw new NotImplementedException();

        }

        public void Print(SellingQuotation sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "")
        {
            throw new NotImplementedException();
        }

        public void Print(SellingDeliveryNotes sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice, int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0, string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
        {
            throw new NotImplementedException();
        }

        public void Print(SellingDeliveryNotes sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "")
        {
            throw new NotImplementedException();
        }
    }
}
