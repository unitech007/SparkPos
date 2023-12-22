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
    public class PrinterMiniPOS : IRAWPrinting
    {
        private string _printerName = string.Empty;

        public PrinterMiniPOS(string printerName)
        {
            _printerName = printerName;
        }

        public void Print(SellingProduct sale, IList<HeaderInvoice> listOfHeaderInvoice, int jumlahRow = 29, int jumlahKarakter = 80, bool isPrintKeteranganInvoice = true, string infoCopyright = "")
        {
            throw new NotImplementedException();
        }

        public void Print(IList<ReportCashierMachine> listOfCashierMachine, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, int jumlahKarakter, int lineFeed, int FontSize = 0,
            string autocutCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
        {
            var garisPemisah = StringHelper.PrintChar('=', jumlahKarakter);
            var isPrinterMiniPos58mm = jumlahKarakter <= 32;
            var textToPrint = new StringBuilder();            
            
            var totalSaldoAwal = 0d;
            var totalItem = 0;
            var totalDiskon = 0d;
            var totalPPN = 0d;
            var grandTotal = 0d;
            var maxFormatNumber = isPrinterMiniPos58mm ? 9 : 10;

            if (!Utils.IsRunningUnderIDE())
            {
                textToPrint.Append(ESCCommandHelper.InitializePrinter());

                if (FontSize > 0)
                    textToPrint.Append(ESCCommandHelper.FontNormal(FontSize));
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

                    textToPrint.Append(CenterText(header.description.Length, jumlahKarakter)).Append(header.description).Append(ESCCommandHelper.LineFeed(1));
                }
            }

            textToPrint.Append(ESCCommandHelper.LineFeed(1));

            textToPrint.Append("Report Sales Per Cashier").Append(ESCCommandHelper.LineFeed(1));
            textToPrint.Append("Per date: ").Append(DateTimeHelper.DateToString(DateTime.Today)).Append(ESCCommandHelper.LineFeed(2));

            if (!Utils.IsRunningUnderIDE())
                textToPrint.Append(ESCCommandHelper.LeftText());

            var Cashier = listOfCashierMachine[0].User.name_user;

            textToPrint.Append("Cashier      : ").Append(Cashier).Append(ESCCommandHelper.LineFeed(1));
            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(2));

            var isAdaTransactions = false;

            foreach (var mesin in listOfCashierMachine.Where(f => f.starting_balance > 0 || (f.sale != null && f.sale.total_invoice > 0)))
            {
                isAdaTransactions = true;

                textToPrint.Append("Login      : ").Append(DateTimeHelper.DateToString(mesin.system_date, "dd/MM/yyyy HH:mm:ss")).Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append("Saldo Awal : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.starting_balance), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

                if (mesin.sale.total_invoice > 0)
                {
                    foreach (var product in mesin.item_jual)
                    {
                        textToPrint.Append(StringHelper.FixedLength(product.product_name, garisPemisah.Length)).Append(ESCCommandHelper.LineFeed(1));

                        var quantity = StringHelper.RightAlignment(product.quantity.ToString(), 4);
                        textToPrint.Append(quantity);

                        textToPrint.Append("  " + StringHelper.FixedLength("x", 3));

                        var price = StringHelper.RightAlignment(NumberHelper.NumberToString(product.selling_price), maxFormatNumber);
                        textToPrint.Append(price);

                        var discount = StringHelper.RightAlignment(product.discount.ToString(),
                                isPrinterMiniPos58mm ? 3 : 7);

                        textToPrint.Append(discount);

                        var subTotal = product.quantity * product.selling_price_setelah_diskon;
                        var sSubTotal = StringHelper.RightAlignment(NumberHelper.NumberToString(subTotal),
                                isPrinterMiniPos58mm ? garisPemisah.Length - 21 : garisPemisah.Length - 26);

                        textToPrint.Append(sSubTotal).Append(ESCCommandHelper.LineFeed(1));
                    }

                    textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));
                    textToPrint.Append("Total item : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.item_jual.Count), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));

                    if (mesin.sale.discount > 0)
                        textToPrint.Append("discount     : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.sale.discount), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));

                    if (mesin.sale.tax > 0)
                        textToPrint.Append("tax        : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.sale.tax), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));

                    textToPrint.Append("Total      : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.sale.grand_total), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));
                    textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

                    totalItem += mesin.item_jual.Count;
                    totalDiskon += mesin.sale.discount;
                    totalPPN += mesin.sale.tax;
                    grandTotal += mesin.sale.grand_total;
                }
                else
                {
                    textToPrint.Append(">> Not yet there transactions <<").Append(ESCCommandHelper.LineFeed(1));
                }

                textToPrint.Append(ESCCommandHelper.LineFeed(1));

                totalSaldoAwal += mesin.starting_balance;
            }

            if (isAdaTransactions)
            {
                textToPrint.Append("Grand Total:").Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append("Saldo Awal : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalSaldoAwal), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append("Total item : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalItem), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));

                if (totalDiskon > 0)
                    textToPrint.Append("discount     : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalDiskon), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));

                if (totalPPN > 0)
                    textToPrint.Append("tax        : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalPPN), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));

                textToPrint.Append("Total      : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(grandTotal), maxFormatNumber)).Append(ESCCommandHelper.LineFeed(1));
            }
            else
            {
                textToPrint.Append(">> Not yet there transactions <<").Append(ESCCommandHelper.LineFeed(1));
            }            

            if (infoCopyright1.Length > 0)
            {
                textToPrint.Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append(CenterText(infoCopyright1.Length, jumlahKarakter)).Append(infoCopyright1).Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append(CenterText(infoCopyright2.Length, jumlahKarakter)).Append(infoCopyright2).Append(ESCCommandHelper.LineFeed(1));
            }

            textToPrint.Append(ESCCommandHelper.LineFeed(lineFeed));

            if (!Utils.IsRunningUnderIDE())
            {
                if (autocutCode.Length > 0)
                    textToPrint.Append(ESCCommandHelper.CustomeCode(autocutCode));
            }                

            if (!Utils.IsRunningUnderIDE())
            {
                RawPrintHelper.SendStringToPrinter(_printerName, textToPrint.ToString());
            }
            else
            {
                RawPrintHelper.SendStringToFile(textToPrint.ToString());
            }
        }

        public void Print(SellingProduct sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice,
            int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0,
            string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
        {
            var garisPemisah = StringHelper.PrintChar('=', jumlahKarakter);
            var isPrinterMiniPos58mm = jumlahKarakter <= 32;
            var textToPrint = new StringBuilder();

            if (!Utils.IsRunningUnderIDE())
            {
                textToPrint.Append(ESCCommandHelper.InitializePrinter());

                if (openCashDrawerCode.Length > 0)
                    textToPrint.Append(ESCCommandHelper.CustomeCode(openCashDrawerCode));

                if (FontSize > 0)
                    textToPrint.Append(ESCCommandHelper.FontNormal(FontSize));
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

                    textToPrint.Append(CenterText(header.description.Length, jumlahKarakter)).Append(header.description).Append(ESCCommandHelper.LineFeed(1));
                }
            }

            // print garis
            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            if (!Utils.IsRunningUnderIDE())
                textToPrint.Append(ESCCommandHelper.LeftText());

            // set date, time, user
            var date = StringHelper.FixedLength(DateTimeHelper.DateToString(DateTime.Now), 10);
            var time = DateTimeHelper.TimeToString(DateTime.Now);

            var Cashier = StringHelper.PrintChar(' ', (garisPemisah.Length - 18 - sale.User.name_user.Length) / 2) + sale.User.name_user;
            Cashier = StringHelper.FixedLength(Cashier, garisPemisah.Length - 18);

            // print date, Cashier, time
            textToPrint.Append(date);
            textToPrint.Append(Cashier); // user
            textToPrint.Append(time).Append(ESCCommandHelper.LineFeed(2)); // time

            // print info invoice
            textToPrint.Append("Invoice   : ").Append(sale.invoice).Append(ESCCommandHelper.LineFeed(1));
            textToPrint.Append("date: ").Append(DateTimeHelper.DateToString(sale.date)).Append(ESCCommandHelper.LineFeed(1));

            if (sale.due_date != null)
            {
                textToPrint.Append("CreditTerm  : ").Append(DateTimeHelper.DateToString(sale.due_date)).Append(ESCCommandHelper.LineFeed(1));
            }            

            if (isPrintCustomer)
            {
                textToPrint.Append(ESCCommandHelper.LineFeed(1));

                // print info customer
                textToPrint.Append("to: ").Append(ESCCommandHelper.LineFeed(1));

                var nameCustomer = sale.is_sdac == true ? sale.Customer.name_customer : sale.shipping_to;
                var address = sale.is_sdac == true ? sale.Customer.address.NullToString() : sale.shipping_address.NullToString();
                var phone = sale.is_sdac == true ? sale.Customer.phone.NullToString() : sale.shipping_phone.NullToString();

                textToPrint.Append(nameCustomer).Append(ESCCommandHelper.LineFeed(1));

                if (address.Length > 0)
                    textToPrint.Append(address).Append(ESCCommandHelper.LineFeed(1));

                if (phone.Length > 0)
                    textToPrint.Append("HP: " + phone).Append(ESCCommandHelper.LineFeed(1));
            }

            // print garis
            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            // print item
            foreach (var item in sale.item_jual)
            {
                var product = StringHelper.FixedLength(item.Product.product_name, garisPemisah.Length);
                textToPrint.Append(product).Append(ESCCommandHelper.LineFeed(1));

                if (item.description.Length > 0)
                    textToPrint.Append(item.description).Append(ESCCommandHelper.LineFeed(1));

                var quantity = StringHelper.RightAlignment(item.quantity.ToString(), 4);
                textToPrint.Append(quantity);

                textToPrint.Append("  " + StringHelper.FixedLength("x", 3));

                var price = StringHelper.RightAlignment(NumberHelper.NumberToString(item.selling_price),
                        isPrinterMiniPos58mm ? 9 : 10);

                textToPrint.Append(price);

                var discount = StringHelper.RightAlignment(item.discount.ToString(), 
                        isPrinterMiniPos58mm ? 3 : 7);
                textToPrint.Append(discount);

                var subTotal = (item.quantity - item.return_quantity) * item.harga_setelah_diskon;
                var sSubTotal = StringHelper.RightAlignment(NumberHelper.NumberToString(subTotal), 
                        isPrinterMiniPos58mm ? garisPemisah.Length - 21 : garisPemisah.Length - 26);

                textToPrint.Append(sSubTotal).Append(ESCCommandHelper.LineFeed(1));
            }

            // print garis
            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(1));

            var fixedLengthLabelFooter = 12;
            var fixedLengthValueFooter = garisPemisah.Length - fixedLengthLabelFooter - 3;

            // print footer
            if (sale.shipping_cost > 0)
            {
                textToPrint.Append(StringHelper.FixedLength("courier", fixedLengthLabelFooter));
                textToPrint.Append(" : " + sale.courier).Append(ESCCommandHelper.LineFeed(1));

                textToPrint.Append(StringHelper.FixedLength("Cost Shipping", fixedLengthLabelFooter));
                textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.shipping_cost), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));
            }

            textToPrint.Append(StringHelper.FixedLength("Total Item", fixedLengthLabelFooter));
            textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.item_jual.Count), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));

            if (sale.discount > 0)
            {
                textToPrint.Append(StringHelper.FixedLength("discount", fixedLengthLabelFooter));
                textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.discount), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));
            }            

            if (sale.tax > 0)
            {
                textToPrint.Append(StringHelper.FixedLength("tax", fixedLengthLabelFooter));
                textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.tax), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));
            }            

            textToPrint.Append(StringHelper.FixedLength("Total", fixedLengthLabelFooter));
            textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.grand_total), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));

            if (sale.jumlah_pay > 0)
            {
                textToPrint.Append(StringHelper.FixedLength("quantity Pay", fixedLengthLabelFooter));
                textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.jumlah_pay), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));

                textToPrint.Append(StringHelper.FixedLength("Return", fixedLengthLabelFooter));
                textToPrint.Append(" : " + StringHelper.RightAlignment(NumberHelper.NumberToString(sale.jumlah_pay - sale.grand_total), fixedLengthValueFooter)).Append(ESCCommandHelper.LineFeed(1));
            }

            // print garis
            textToPrint.Append(garisPemisah).Append(ESCCommandHelper.LineFeed(2));

            // print footer
            foreach (var footer in listOfFooterInvoice)
            {
                if (footer.description.Length > 0)
                {
                    if (footer.description.Length > garisPemisah.Length)
                    {
                        footer.description = StringHelper.FixedLength(footer.description, garisPemisah.Length);
                    }
                    
                    textToPrint.Append(CenterText(footer.description.Length, jumlahKarakter)).Append(footer.description).Append(ESCCommandHelper.LineFeed(1));
                }
            }

            if (infoCopyright1.Length > 0)
            {
                textToPrint.Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append(CenterText(infoCopyright1.Length, jumlahKarakter)).Append(infoCopyright1).Append(ESCCommandHelper.LineFeed(1));
                textToPrint.Append(CenterText(infoCopyright2.Length, jumlahKarakter)).Append(infoCopyright2).Append(ESCCommandHelper.LineFeed(1));
            }

            textToPrint.Append(ESCCommandHelper.LineFeed(lineFeed));

            if (!Utils.IsRunningUnderIDE())
            {
                if (autocutCode.Length > 0)
                    textToPrint.Append(ESCCommandHelper.CustomeCode(autocutCode));                

                RawPrintHelper.SendStringToPrinter(_printerName, textToPrint.ToString());
            }
            else
            {
                RawPrintHelper.SendStringToFile(textToPrint.ToString());
            }
        }

        public void Print(SellingQuotation sale, IList<HeaderInvoiceMiniPos> listOfHeaderInvoice, IList<FooterInvoiceMiniPos> listOfFooterInvoice, int jumlahKarakter, int lineFeed, bool isPrintCustomer = true, bool isPrintKeteranganInvoice = true, int FontSize = 0, string autocutCode = "", string openCashDrawerCode = "", string infoCopyright1 = "", string infoCopyright2 = "")
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

        private string CenterText(int panjangString, int jumlahKarakter)
        {
            var div = (double)(jumlahKarakter - panjangString) / 2;
            var posisiTengah = Math.Ceiling(div);
            var result = StringHelper.PrintChar(' ', Convert.ToInt32(posisiTengah));

            return result;
        }
    }
}
