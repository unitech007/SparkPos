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

using SparkPOS.Model;
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Bll.Service;
using SparkPOS.Bll.Api;
using log4net;
using SparkPOS.Helper.RAWPrinting;

namespace SparkPOS.App.Cashier.Lookup
{
    public partial class FrmLookupInvoice : FrmLookupEmptyBody
    {
        private IList<SellingProduct> _listOfSelling = null;
        private GeneralSupplier _GeneralSupplier;
        private ILog _log;

        public FrmLookupInvoice(string header, IList<SellingProduct> listOfSelling)
            : base()
        {
             InitializeComponent();  ////MainProgram.GlobalLanguageChange(this);

            base.SetHeader(header);

            this._GeneralSupplier = MainProgram.GeneralSupplier;
            this._log = MainProgram.log;
            this._listOfSelling = listOfSelling;
            
            InitGridList();
            base.SetActiveBtnSelect(listOfSelling.Count > 0);
            base.SetTitleBtnSelect("F10 Print");
            MainProgram.GlobalLanguageChange(this);
        }        

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 120 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice", Width = 170 });
            gridListProperties.Add(new GridListControlProperties { Header = "amount", Width = 150 });
            gridListProperties.Add(new GridListControlProperties { Header = "Cashier" });

            GridListControlHelper.InitializeGridListControl<SellingProduct>(this.gridList, _listOfSelling, gridListProperties);

            this.gridList.Grid.QueryRowHeight += delegate(object sender, GridRowColSizeEventArgs e)
            {
                e.Size = 27;
                e.Handled = true;
            };

            this.gridList.Grid.QueryCellInfo += GridInvoice_QueryCellInfo;

            if (_listOfSelling.Count > 0)
                this.gridList.SetSelected(0, true);
        }

        private void GridInvoice_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (_listOfSelling.Count > 0)
            {
                if (e.RowIndex > 0)
                {
                    e.Style.Font = new GridFontInfo(new Font("Arial", 14f));

                    var rowIndex = e.RowIndex - 1;

                    if (rowIndex < _listOfSelling.Count)
                    {
                        var sale = _listOfSelling[rowIndex];

                        switch (e.ColIndex)
                        {
                            case 2:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = DateTimeHelper.DateToString(sale.date);
                                break;

                            case 3:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                e.Style.CellValue = sale.invoice;
                                break;

                            case 4:
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                e.Style.CellValue = NumberHelper.NumberToString(sale.grand_total);
                                break;

                            case 5:
                                var Cashier = sale.User;
                                if (Cashier != null) e.Style.CellValue = Cashier.name_user;
                                break;

                            default:
                                break;
                        }

                        // we handled it, let the grid know
                        e.Handled = true;
                    }
                }
            }
        }

        protected override void Select()
        {
            var rowIndex = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(rowIndex, this.Text))
                return;

            if (_GeneralSupplier.is_auto_print)
            {
                if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
                {
                    var sale = _listOfSelling[rowIndex];
                    if (sale != null)
                    {
                        ISellingProductBll bll = new SellingProductBll(_log);
                        sale.item_jual = bll.GetItemSelling(sale.sale_id).ToList();
                    }

                    switch (_GeneralSupplier.type_printer)
                    {
                        case TypePrinter.DotMatrix:
                            PrintInvoiceDotMatrix(sale);
                            break;

                        case TypePrinter.MiniPOS:
                            PrintInvoiceMiniPOS(sale);
                            break;

                        default:
                            // do nothing
                            break;
                    }
                }
            }            
        }

        private void PrintInvoiceDotMatrix(SellingProduct sale)
        {
            IRAWPrinting printerMiniPos = new PrinterDotMatrix(_GeneralSupplier.name_printer);
            printerMiniPos.Print(sale, _GeneralSupplier.list_of_header_nota);
        }

        private void PrintInvoiceMiniPOS(SellingProduct sale)
        {
            var autocutCode = _GeneralSupplier.is_autocut ? _GeneralSupplier.autocut_code : string.Empty;
            var openCashDrawerCode = _GeneralSupplier.is_open_cash_drawer ? _GeneralSupplier.open_cash_drawer_code : string.Empty;

            IRAWPrinting printerMiniPos = new PrinterMiniPOS(_GeneralSupplier.name_printer);

            printerMiniPos.Print(sale, _GeneralSupplier.list_of_header_nota_mini_pos, _GeneralSupplier.list_of_footer_nota_mini_pos,
                _GeneralSupplier.jumlah_karakter, _GeneralSupplier.jumlah_gulung, _GeneralSupplier.is_print_customer, FontSize: _GeneralSupplier.size_font,
                autocutCode: autocutCode, openCashDrawerCode: openCashDrawerCode);
        }
    }
}
