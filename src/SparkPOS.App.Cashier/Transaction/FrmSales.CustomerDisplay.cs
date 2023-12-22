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
using SparkPOS.Helper;
using GodSharp;

namespace SparkPOS.App.Cashier.Transactions
{
    public partial class FrmSales
    {
        private const int MAX_LENGTH = 20; // maksimal karakter customer display

        private void DisplayItemProduct(ItemSellingProduct itemSelling)
        {
            var product = itemSelling.Product;

            var quantity = itemSelling.quantity - itemSelling.return_quantity;
            var hargaSelling = itemSelling.harga_setelah_diskon;

            if (product != null)
            {
                if (!(hargaSelling > 0))
                {
                    double discount = itemSelling.discount;
                    double diskonRupiah = 0;

                    if (!(discount > 0))
                    {
                        if (_customer != null)
                            discount = _customer.discount;

                        if (!(discount > 0))
                        {
                            var diskonProduct = GetDiskonSellingFix(product, quantity, product.discount);
                            discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                        }
                    }

                    hargaSelling = GetPriceSellingFix(product, quantity, product.selling_price);

                    diskonRupiah = discount / 100 * hargaSelling;
                    hargaSelling -= diskonRupiah;
                }
            }

            var subTotal = StringHelper.RightAlignment(Convert.ToString(quantity * hargaSelling), MAX_LENGTH - (quantity.ToString().Length + hargaSelling.ToString().Length + 1));

            var displayLine1 = StringHelper.FixedLength(product.product_name, MAX_LENGTH);
            var displayLine2 = string.Format("{0}x{1}{2}", quantity, hargaSelling, subTotal);

            System.Diagnostics.Debug.Print("displayLine1: {0}", displayLine1);
            System.Diagnostics.Debug.Print("displayLine2: {0}", displayLine2);

            if (!Utils.IsRunningUnderIDE() && _settingCustomerDisplay.is_active_customer_display)
            {
                GodSerialPort serialPort = null;

                if (!GodSerialPortHelper.IsConnected(serialPort, _settingPort))
                {
                    return;
                }

                GodSerialPortHelper.SendStringToCustomerDisplay(displayLine1, displayLine2, serialPort);
            }
        }

        private void DisplayOpeningSentence()
        {
            var displayLine1 = string.Format("{0}{1}", StringHelper.CenterAlignment(_settingCustomerDisplay.opening_sentence_line1.Length, MAX_LENGTH),
                _settingCustomerDisplay.opening_sentence_line1);

            var displayLine2 = string.Format("{0}{1}", StringHelper.CenterAlignment(_settingCustomerDisplay.opening_sentence_line2.Length, MAX_LENGTH),
                _settingCustomerDisplay.opening_sentence_line2);

            System.Diagnostics.Debug.Print("displayLine1: {0}", displayLine1);
            System.Diagnostics.Debug.Print("displayLine2: {0}", displayLine2);

            if (!Utils.IsRunningUnderIDE() && _settingCustomerDisplay.is_active_customer_display)
            {
                GodSerialPort serialPort = null;

                if (!GodSerialPortHelper.IsConnected(serialPort, _settingPort))
                {
                    return;
                }

                GodSerialPortHelper.SendStringToCustomerDisplay(displayLine1, displayLine2, serialPort);
            }
        }

        private void DisplayKalimatPenutup()
        {
            var displayLine1 = string.Format("{0}{1}", StringHelper.CenterAlignment(_settingCustomerDisplay.closing_sentence_line1.Length, MAX_LENGTH),
                _settingCustomerDisplay.closing_sentence_line1);

            var displayLine2 = string.Format("{0}{1}", StringHelper.CenterAlignment(_settingCustomerDisplay.closing_sentence_line2.Length, MAX_LENGTH),
                _settingCustomerDisplay.closing_sentence_line2);

            System.Diagnostics.Debug.Print("displayLine1: {0}", displayLine1);
            System.Diagnostics.Debug.Print("displayLine2: {0}", displayLine2);

            if (!Utils.IsRunningUnderIDE() && _settingCustomerDisplay.is_active_customer_display)
            {
                GodSerialPort serialPort = null;

                if (!GodSerialPortHelper.IsConnected(serialPort, _settingPort))
                {
                    return;
                }

                GodSerialPortHelper.SendStringToCustomerDisplay(displayLine1, displayLine2, serialPort);
            }
        }

        private void DisplayTotal(string total)
        {
            var displayLine1 = "Total";
            var displayLine2 = StringHelper.RightAlignment(total, MAX_LENGTH);

            System.Diagnostics.Debug.Print("displayLine1: {0}", displayLine1);
            System.Diagnostics.Debug.Print("displayLine2: {0}", displayLine2);

            if (!Utils.IsRunningUnderIDE() && _settingCustomerDisplay.is_active_customer_display)
            {
                GodSerialPort serialPort = null;

                if (!GodSerialPortHelper.IsConnected(serialPort, _settingPort))
                {
                    return;
                }

                GodSerialPortHelper.SendStringToCustomerDisplay(displayLine1, displayLine2, serialPort);
            }
        }

        private void DisplayRefund(string refund)
        {
            var displayLine1 = "Refund";
            var displayLine2 = StringHelper.RightAlignment(refund, MAX_LENGTH);

            System.Diagnostics.Debug.Print("displayLine1: {0}", displayLine1);
            System.Diagnostics.Debug.Print("displayLine2: {0}", displayLine2);

            if (!Utils.IsRunningUnderIDE() && _settingCustomerDisplay.is_active_customer_display)
            {
                GodSerialPort serialPort = null;

                if (!GodSerialPortHelper.IsConnected(serialPort, _settingPort))
                {
                    return;
                }

                GodSerialPortHelper.SendStringToCustomerDisplay(displayLine1, displayLine2, serialPort);
            }
        }
    }
}
