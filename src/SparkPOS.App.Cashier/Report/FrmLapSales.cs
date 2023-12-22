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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SparkPOS.Helper;
using SparkPOS.Model;
using SparkPOS.Model.Report;
using SparkPOS.Bll.Api.Report;
using SparkPOS.Bll.Service.Report;
using log4net;
using SparkPOS.Helper.RAWPrinting;

namespace SparkPOS.App.Cashier.Report
{
    public partial class FrmLapSales : Form
    {
        private ILog _log;
        private GeneralSupplier _GeneralSupplier = null;
        private User _user;
        private IList<ReportCashierMachine> _listOfCashierMachine;

        public FrmLapSales(string header, User user, GeneralSupplier GeneralSupplier)
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            this._log = MainProgram.log;
            this._user = user;
            this._GeneralSupplier = GeneralSupplier;            

            this.Text = header;
            this.lblHeader.Text = header;

            txtOutput.Text = GenerateReport();
            MainProgram.GlobalLanguageChange(this);
        }        

        private string GenerateReport()
        {
            var txtOutput = new StringBuilder();
            var garisPemisah = StringHelper.PrintChar('=', 40);
            
            var totalSaldoAwal = 0d;
            var totalItem = 0;
            var totalDiskon = 0d;
            var totalPPN = 0d;
            var grandTotal = 0d;
            var maxFormatNumber = 10;

            txtOutput.Append("Report Sales Per Cashier").Append(Environment.NewLine);
            txtOutput.Append("Per date: ").Append(DateTimeHelper.DateToString(DateTime.Today)).Append(Environment.NewLine).Append(Environment.NewLine);

            txtOutput.Append("Cashier      : ").Append(_user.name_user).Append(Environment.NewLine);
            txtOutput.Append(garisPemisah).Append(Environment.NewLine).Append(Environment.NewLine);

            IReportCashierMachineBll bll = new ReportCashierMachineBll(_log);

            var isAdaTransactions = false;

            _listOfCashierMachine = bll.PerCashierGetByUserId(_user.user_id);
            foreach (var mesin in _listOfCashierMachine.Where(f => f.starting_balance > 0 || (f.sale != null && f.sale.total_invoice > 0)))
            {
                isAdaTransactions = true;

                txtOutput.Append("Login      : ").Append(DateTimeHelper.DateToString(mesin.system_date, "dd/MM/yyyy HH:mm:ss")).Append(Environment.NewLine);
                txtOutput.Append("Saldo Awal : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.starting_balance), maxFormatNumber)).Append(Environment.NewLine);
                txtOutput.Append(garisPemisah).Append(Environment.NewLine);

                if (mesin.sale.total_invoice > 0)
                {
                    foreach (var product in mesin.item_jual)
                    {
                        txtOutput.Append(StringHelper.FixedLength(product.product_name, garisPemisah.Length)).Append(Environment.NewLine);

                        var quantity = StringHelper.RightAlignment(product.quantity.ToString(), 4);
                        txtOutput.Append(quantity);

                        txtOutput.Append("  " + StringHelper.FixedLength("x", 3));

                        var price = StringHelper.RightAlignment(NumberHelper.NumberToString(product.selling_price), maxFormatNumber);
                        txtOutput.Append(price);

                        var discount = StringHelper.RightAlignment(product.discount.ToString(), 7);
                        txtOutput.Append(discount);

                        var subTotal = product.quantity * product.selling_price_setelah_diskon;

                        var sSubTotal = StringHelper.RightAlignment(NumberHelper.NumberToString(subTotal), garisPemisah.Length - 26);
                        txtOutput.Append(sSubTotal).Append(Environment.NewLine);
                    }

                    txtOutput.Append(garisPemisah).Append(Environment.NewLine);
                    txtOutput.Append("Total item : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.item_jual.Count), maxFormatNumber)).Append(Environment.NewLine);
                    txtOutput.Append("discount     : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.sale.discount), maxFormatNumber)).Append(Environment.NewLine);
                    txtOutput.Append("tax        : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.sale.tax), maxFormatNumber)).Append(Environment.NewLine);
                    txtOutput.Append("Total      : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(mesin.sale.grand_total), maxFormatNumber)).Append(Environment.NewLine);
                    txtOutput.Append(garisPemisah).Append(Environment.NewLine);

                    totalItem += mesin.item_jual.Count;
                    totalDiskon += mesin.sale.discount;
                    totalPPN += mesin.sale.tax;
                    grandTotal += mesin.sale.grand_total;
                }
                else
                {
                    txtOutput.Append(">> Not yet there transactions <<").Append(Environment.NewLine);
                }

                txtOutput.Append(Environment.NewLine);
                totalSaldoAwal += mesin.starting_balance;
            }

            if (isAdaTransactions)
            {
                txtOutput.Append("Grand Total:").Append(Environment.NewLine);
                txtOutput.Append("Saldo Awal : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalSaldoAwal), maxFormatNumber)).Append(Environment.NewLine);
                txtOutput.Append("Total item : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalItem), maxFormatNumber)).Append(Environment.NewLine);
                txtOutput.Append("discount     : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalDiskon), maxFormatNumber)).Append(Environment.NewLine);
                txtOutput.Append("tax        : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(totalPPN), maxFormatNumber)).Append(Environment.NewLine);
                txtOutput.Append("Total      : ").Append(StringHelper.RightAlignment(NumberHelper.NumberToString(grandTotal), maxFormatNumber)).Append(Environment.NewLine);
            }
            else
            {
                txtOutput.Append(">> Not yet there transactions <<").Append(Environment.NewLine);
            }            

            return txtOutput.ToString();
        }

        private void PrintReport()
        {
            if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
            {
                var autocutCode = _GeneralSupplier.is_autocut ? _GeneralSupplier.autocut_code : string.Empty;

                IRAWPrinting printerMiniPos = new PrinterMiniPOS(_GeneralSupplier.name_printer);

                printerMiniPos.Print(_listOfCashierMachine, _GeneralSupplier.list_of_header_nota_mini_pos, _GeneralSupplier.jumlah_karakter, 
                    _GeneralSupplier.jumlah_gulung, FontSize: _GeneralSupplier.size_font, autocutCode: autocutCode);

                this.Close();
            }            
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintReport();
        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLapSales_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                this.Close();
        }

        private void FrmLapSales_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F11:
                    if (btnPrint.Enabled)
                        PrintReport();

                    break;

                default:
                    break;
            }
        }
    }
}
