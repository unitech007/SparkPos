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

using log4net;
using GodSharp;
using SparkPOS.Model;
using SparkPOS.Helper;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using Syncfusion.Windows.Forms.Grid;
using WeifenLuo.WinFormsUI.Docking;
using SparkPOS.App.Cashier.Lookup;
using SparkPOS.Helper.UserControl;
using SparkPOS.Helper.RAWPrinting;

namespace SparkPOS.App.Cashier.Transactions
{
    public partial class FrmSales : DockContent, IListener
    {
        private ISellingProductBll _bll = null;
        private SellingProduct _jual = null;
        private Customer _customer = null;
        private IList<ItemSellingProduct> _listOfItemSelling = new List<ItemSellingProduct>();

        private int _rowIndex = 0;
        private int _colIndex = 0;

        private ILog _log;
        private User _user;
        private Profil _profil;
        private GeneralSupplier _GeneralSupplier;
        private SettingPort _settingPort;
        private SettingCustomerDisplay _settingCustomerDisplay;
        private SettingLebarColumnTabelTransactions _settingLebarColumnTabelTransactions;

        private bool _isPrintStruk = true;
        private string _currentInvoice;        

        public FrmSales(string header, User user, string menuId)
        {
             InitializeComponent();  ////MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);

            this.Text = header;
            this._log = MainProgram.log;
            this._bll = new SellingProductBll(_log);            
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;
            this._settingPort = MainProgram.settingPort;
            this._settingCustomerDisplay = MainProgram.settingCustomerDisplay;
            this._settingLebarColumnTabelTransactions = MainProgram.settingLebarColumnTabelTransactions;

            _currentInvoice = this._bll.GetLastInvoice();

            _listOfItemSelling.Add(new ItemSellingProduct()); // add dummy objek

            InitGridControl(gridControl);

            SetStatusBar();
            ShowInfoDate(_currentInvoice);
            txtCashier.Text = this._user.name_user;

            DisplayOpeningSentence();
            tmrDisplayKalimatPenutup.Interval = _settingCustomerDisplay.delay_display_closing_sentence * 1000;
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridControl(GridControl grid)
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = _settingLebarColumnTabelTransactions.lebar_column_no < 65 ? 65 : _settingLebarColumnTabelTransactions.lebar_column_no });
            gridListProperties.Add(new GridListControlProperties { Header = "Code Product", Width = _settingLebarColumnTabelTransactions.lebar_column_code_produk });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Product", Width = _settingLebarColumnTabelTransactions.lebar_column_name_produk });

            gridListProperties.Add(new GridListControlProperties
                {
                    Header = _GeneralSupplier.additional_sales_item_information,
                    Width = _GeneralSupplier.is_show_additional_sales_item_information ? _settingLebarColumnTabelTransactions.lebar_column_keterangan : 0
                }
            );

            gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = _settingLebarColumnTabelTransactions.lebar_column_jumlah });
            gridListProperties.Add(new GridListControlProperties { Header = "discount", Width = _settingLebarColumnTabelTransactions.lebar_column_diskon });
            gridListProperties.Add(new GridListControlProperties { Header = "price", Width = _settingLebarColumnTabelTransactions.lebar_column_harga });
            gridListProperties.Add(new GridListControlProperties { Header = "Sub Total" });

            GridListControlHelper.InitializeGridListControl<ItemSellingProduct>(grid, _listOfItemSelling, gridListProperties);
            
            grid.QueryRowHeight += delegate(object sender, GridRowColSizeEventArgs e)
            {
                e.Size = 27;
                e.Handled = true;
            };

            grid.ResizingColumns += delegate(object sender, GridResizingColumnsEventArgs e)
            {
                try
                {
                    var appConfigFile = string.Format("{0}\\SparkPOSCashier.exe.config", Utils.GetAppPath());
                    var columnWidth = grid.ColWidths[e.Columns.Left];

                    switch (e.Columns.Left)
                    {
                        case 1:
                            _settingLebarColumnTabelTransactions.lebar_column_no = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnNo", columnWidth.ToString(), appConfigFile);
                            break;

                        case 2:
                            _settingLebarColumnTabelTransactions.lebar_column_code_produk = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnCodeProduct", columnWidth.ToString(), appConfigFile);
                            break;

                        case 3:
                            _settingLebarColumnTabelTransactions.lebar_column_name_produk = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnNameProduct", columnWidth.ToString(), appConfigFile);
                            break;

                        case 4:
                            _settingLebarColumnTabelTransactions.lebar_column_keterangan = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnKeterangan", columnWidth.ToString(), appConfigFile);
                            break;

                        case 5:
                            _settingLebarColumnTabelTransactions.lebar_column_jumlah = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnJumlah", columnWidth.ToString(), appConfigFile);
                            break;

                        case 6:
                            _settingLebarColumnTabelTransactions.lebar_column_diskon = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnDiskon", columnWidth.ToString(), appConfigFile);
                            break;

                        case 7:
                            _settingLebarColumnTabelTransactions.lebar_column_harga = columnWidth;
                            AppConfigHelper.SaveValue("lebarColumnPrice", columnWidth.ToString(), appConfigFile);
                            break;

                        default:
                            break;
                    }
                }
                catch { }
            };

            grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                // Make sure the cell falls inside the grid
                if (e.RowIndex > 0)
                {
                    if (!(_listOfItemSelling.Count > 0))
                        return;

                    var itemSelling = _listOfItemSelling[e.RowIndex - 1];
                    var product = itemSelling.Product;

                    if (e.RowIndex % 2 == 0)
                        e.Style.BackColor = ColorCollection.BACK_COLOR_ALTERNATE;

                    double hargaPurchase = 0;
                    double hargaSelling = 0;
                    double quantity = 0;

                    var isReturn = itemSelling.return_quantity > 0;
                    if (isReturn)
                    {
                        e.Style.BackColor = Color.Red;
                        e.Style.Enabled = false;
                    }

                    e.Style.Font = new GridFontInfo(new Font("Arial", 15f));
                    
                    switch (e.ColIndex)
                    {
                        case 1: // no urut
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.Enabled = false;
                            e.Style.CellValue = e.RowIndex.ToString();
                            break;

                        case 2:
                            if (product != null)
                                e.Style.CellValue = product.product_code;

                            break;

                        case 3: // name product
                            if (product != null)
                                e.Style.CellValue = product.product_name;

                            break;

                        case 4: // description
                            e.Style.CellValue = itemSelling.description;

                            break;

                        case 5: // quantity
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemSelling.quantity - itemSelling.return_quantity;

                            break;

                        case 6: // discount
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemSelling.discount;

                            break;

                        case 7: // price
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;

                            hargaPurchase = itemSelling.purchase_price;
                            hargaSelling = itemSelling.selling_price;

                            if (product != null)
                            {
                                if (!(hargaPurchase > 0))
                                    hargaPurchase = product.purchase_price;

                                if (!(hargaSelling > 0))
                                {
                                    quantity = itemSelling.quantity - itemSelling.return_quantity;
                                    hargaSelling = GetPriceSellingFix(product, quantity, product.selling_price);
                                }
                            }

                            e.Style.CellValue = NumberHelper.NumberToString(hargaSelling);

                            break;

                        case 8: // subtotal
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.Enabled = false;

                            quantity = itemSelling.quantity - itemSelling.return_quantity;

                            hargaPurchase = itemSelling.purchase_price;
                            hargaSelling = itemSelling.harga_setelah_diskon;

                            if (product != null)
                            {
                                if (!(hargaPurchase > 0))
                                    hargaPurchase = product.purchase_price;

                                if (!(hargaSelling > 0))
                                {
                                    double discount = itemSelling.discount;
                                    double diskonRupiah = 0;

                                    if (!(discount > 0))
                                    {
                                        if (_customer != null)
                                        {
                                            discount = _customer.discount;
                                        }

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

                            e.Style.CellValue = NumberHelper.NumberToString(quantity * hargaSelling);
                            break;

                        default:
                            break;
                    }

                    e.Handled = true; // we handled it, let the grid know
                }
            };

            var colIndex = 2; // column name product
            grid.CurrentCell.MoveTo(1, colIndex, GridSetCurrentCellOptions.BeginEndUpdate);
        }

        private PriceWholesale GetPriceWholesale(Product product, double quantity)
        {
            PriceWholesale hargaWholesale = null;

            if (product.list_of_harga_grosir.Count(f => f.minimum_quantity > 0) > 0)
            {
                hargaWholesale = product.list_of_harga_grosir
                                    .Where(f => f.product_id == product.product_id && (f.minimum_quantity > 0 && f.minimum_quantity <= quantity))
                                    .LastOrDefault();
                
                // price grosir tidak there yang cocok, set price retil
                if (hargaWholesale == null)
                    hargaWholesale = new PriceWholesale { retail_price = 1, wholesale_price = product.selling_price, discount = product.discount };
            }            

            return hargaWholesale;
        }

        private double GetPriceSellingFix(Product product, double quantity, double hargaSellingRetail)
        {
            var result = hargaSellingRetail;

            if (quantity >= 1)
            {
                var grosir = GetPriceWholesale(product, quantity);
                if (grosir != null)
                {
                    if (grosir.wholesale_price > 0)
                        result = grosir.wholesale_price;
                }
            }

            return result;
        }

        private double GetDiskonSellingFix(Product product, double quantity, double diskonSellingRetail)
        {
            var result = diskonSellingRetail;

            if (quantity >= 1)
            {
                var grosir = GetPriceWholesale(product, quantity);
                if (grosir != null)
                {
                    if (grosir.discount > 0)
                        result = grosir.discount;
                }
            }

            return result;
        }

        private void SetStatusBar()
        {
            var infoStatus = "F3 : Input Product | F4 : Find Pelanggan | F5 : Edit quantity | F6 : Edit discount | F7 : Edit price | F8 : Check Invoice Terakhir | F10 : Pay" +
                             "\r\nCTRL + B : Pembatalan Transactions | CTRL + D: Delete Item Transactions | CTRL + L : Report Sales | CTRL + N : Tanpa Invoice/Struk | CTRL + P : Setting Printer | CTRL + X : Tutup Form Transactions";

            lblStatusBar.Text = infoStatus;
        }

        private void ShowInfoDate(string invoice)
        {
            var dt = DateTime.Now;

            var date = string.Format("{0}, {1}", DayMonthHelper.GetDayIndonesia(dt), dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);
            var time = string.Format("{0:HH:mm:ss}", dt);

            txtInvoiceDanDate.Text = string.Format("{0} / {1} {2}", invoice, date, time);
        }

        private void tmrDateJam_Tick(object sender, EventArgs e)
        {
            ShowInfoDate(_currentInvoice);
        }

        private void tmrResetPesan_Tick(object sender, EventArgs e)
        {
            lblPesan.Text = "";
            ((Timer)sender).Enabled = false;
        }

        private void tmrDisplayKalimatPenutup_Tick(object sender, EventArgs e)
        {
            DisplayKalimatPenutup();
            ((Timer)sender).Enabled = false;
        }

        private void SetItemProduct(GridControl grid, int rowIndex, Product product,
            double quantity = 1, double price = 0, double discount = 0, string description = "")
        {
            var itemSelling = new ItemSellingProduct();
            itemSelling.product_id = product.product_id;
            itemSelling.Product = product;
            itemSelling.description = description;
            itemSelling.quantity = quantity;
            itemSelling.purchase_price = product.purchase_price;
            itemSelling.selling_price = price > 0 ? price : product.selling_price;
            itemSelling.discount = discount;

            _listOfItemSelling[rowIndex - 1] = itemSelling;
        }

        private void UpdateItemProduct(GridControl grid, int rowIndex)
        {
            var itemSelling = _listOfItemSelling[rowIndex];

            if (itemSelling.entity_state == EntityState.Unchanged)
                itemSelling.entity_state = EntityState.Modified;

            itemSelling.quantity += 1;
            itemSelling.discount = GetDiskonSellingFix(itemSelling.Product, itemSelling.quantity, itemSelling.discount);
            itemSelling.selling_price = GetPriceSellingFix(itemSelling.Product, itemSelling.quantity, itemSelling.selling_price);

            _listOfItemSelling[rowIndex] = itemSelling;
        }

        private ItemSellingProduct GetExistItemProduct(string produkId)
        {
            var obj = _listOfItemSelling.Where(f => f.product_id == produkId)
                                     .FirstOrDefault();
            return obj;
        }

        private double SumGrid(IList<ItemSellingProduct> listOfItemSelling)
        {
            double total = 0;
            foreach (var item in _listOfItemSelling.Where(f => f.Product != null))
            {
                double price = 0;
                var quantity = item.quantity - item.return_quantity;

                if (item.selling_price > 0)
                {
                    price = item.harga_setelah_diskon;
                }
                else
                {
                    if (item.Product != null)
                    {
                        var hargaSelling = GetPriceSellingFix(item.Product, quantity, item.Product.selling_price);
                        var discount = GetDiskonSellingFix(item.Product, quantity, item.discount);

                        double diskonRupiah = discount / 100 * hargaSelling;

                        price = hargaSelling - diskonRupiah;
                    }
                }

                total += price * quantity;
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemSelling));
        }

        private void gridControl_CurrentCellKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                Shortcut(sender, e);
                return;
            }

            if (KeyPressHelper.IsEnter(e))
            {
                if (lblRefund.Text.Length > 0)
                    lblRefund.Text = "";

                var grid = (GridControl)sender;

                var rowIndex = grid.CurrentCell.RowIndex;
                var colIndex = grid.CurrentCell.ColIndex;

                IProductBll bll = new ProductBll(_log);
                Product product = null;
                GridCurrentCell cc;

                switch (colIndex)
                {
                    case 2: // code product
                        cc = grid.CurrentCell;
                        var codeProduct = cc.Renderer.ControlValue.ToString();

                        if (codeProduct.Length == 0) // code product empty
                        {
                            // fokus ke column name product
                            GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                        }
                        else
                        {
                            // pencarian based code product
                            product = bll.GetByCode(codeProduct, true);

                            if (product == null)
                            {
                                ShowMessage("Product data not found", true);
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                return;
                            }

                            if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                            {
                                if (product.is_stock_minus)
                                {
                                    ShowMessage("Sorry stock product should not minus", true);
                                    GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                    return;
                                }
                            }

                            ShowMessage("");

                            double discount = 0;

                            if (_customer != null)
                            {
                                discount = _customer.discount;
                            }

                            if (!(discount > 0))
                            {
                                var diskonProduct = GetDiskonSellingFix(product, 1, product.discount);
                                discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                            }

                            ItemSellingProduct itemSelling = null;

                            // cek item product already entered atau Not yet ?
                            var itemProduct = GetExistItemProduct(product.product_id);

                            if (itemProduct != null) // already there, tinggal update quantity
                            {
                                var index = _listOfItemSelling.IndexOf(itemProduct);

                                UpdateItemProduct(grid, index);
                                cc.Renderer.ControlText = string.Empty;

                                itemSelling = _listOfItemSelling[index];
                            }
                            else
                            {
                                SetItemProduct(grid, rowIndex, product, discount: discount);
                                itemSelling = _listOfItemSelling[rowIndex - 1];

                                if (grid.RowCount == rowIndex)
                                {
                                    _listOfItemSelling.Add(new ItemSellingProduct());
                                    grid.RowCount = _listOfItemSelling.Count;
                                }
                            }
                            
                            grid.Refresh();
                            RefreshTotal();
                            DisplayItemProduct(itemSelling);

                            if (_GeneralSupplier.is_show_additional_sales_item_information)
                            {
                                // fokus ke column description
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, 4);
                            }
                            else
                            {
                                if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                                {
                                    GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // fokus ke column quantity
                                }
                                else
                                {
                                    GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                                }
                            }                                                        
                        }

                        break;

                    case 3: // pencarian based name product

                        cc = grid.CurrentCell;
                        var nameProduct = cc.Renderer.ControlValue.ToString();

                        if (nameProduct.Length == 0)
                        {
                            ShowMessage("Name product should not empty", true);
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                            return;
                        }

                        var listOfProduct = bll.GetByName(nameProduct, false, true);

                        if (listOfProduct.Count == 0)
                        {
                            ShowMessage("Product data not found", true);
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                        }
                        else if (listOfProduct.Count == 1)
                        {
                            ShowMessage("");
                            product = listOfProduct[0];

                            IPriceWholesaleBll hargaWholesaleBll = new PriceWholesaleBll(_log);
                            product.list_of_harga_grosir = hargaWholesaleBll.GetListPriceWholesale(product.product_id).ToList();	

                            if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                            {
                                if (product.is_stock_minus)
                                {
                                    ShowMessage("Sorry stock product should not minus", true);
                                    GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                                    return;
                                }
                            }

                            double discount = 0;

                            if (_customer != null)
                            {
                                discount = _customer.discount;
                            }

                            if (!(discount > 0))
                            {
                                var diskonProduct = GetDiskonSellingFix(product, 1, product.discount);
                                discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                            }

                            ItemSellingProduct itemSelling = null;

                            // cek item product already entered atau Not yet ?
                            var itemProduct = GetExistItemProduct(product.product_id);

                            if (itemProduct != null) // already there, tinggal update quantity
                            {
                                var index = _listOfItemSelling.IndexOf(itemProduct);

                                UpdateItemProduct(grid, index);
                                cc.Renderer.ControlText = string.Empty;

                                itemSelling = _listOfItemSelling[index];
                            }
                            else
                            {
                                SetItemProduct(grid, rowIndex, product, discount: discount);

                                itemSelling = _listOfItemSelling[rowIndex - 1];

                                if (grid.RowCount == rowIndex)
                                {
                                    _listOfItemSelling.Add(new ItemSellingProduct());
                                    grid.RowCount = _listOfItemSelling.Count;
                                }
                            }
                            
                            grid.Refresh();
                            RefreshTotal();
                            DisplayItemProduct(itemSelling);

                            if (_GeneralSupplier.is_show_additional_sales_item_information)
                            {
                                // fokus ke column description
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, 4);
                            }
                            else
                            {
                                if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                                {
                                    GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // fokus ke column quantity
                                }
                                else
                                {
                                    GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                                }
                            }                            
                        }
                        else // data lebih dari one
                        {
                            ShowMessage("");
                            _rowIndex = rowIndex;
                            _colIndex = colIndex;

                            var frmLookup = new FrmLookupReference("Data Product", listOfProduct);
                            frmLookup.Listener = this;
                            frmLookup.ShowDialog();
                        }

                        break;

                    case 4: // description
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                        {
                            GridListControlHelper.SetCurrentCell(grid, rowIndex, 5); // fokus ke column quantity
                        }
                        else
                        {
                            GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                        }

                        break;

                    case 5: // quantity
                        if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                        {
                            gridControl_CurrentCellValidated(sender, new EventArgs());

                            var itemSelling = _listOfItemSelling[rowIndex - 1];
                            product = itemSelling.Product;

                            var isValidStock = (product.remaining_stock - itemSelling.quantity) >= 0;

                            if (!isValidStock)
                            {
                                ShowMessage("Sorry stock product should not minus", true);
                                GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);

                                return;
                            }
                        }

                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // Transfer kerow berikutnya
                        break;

                    case 6: // discount
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2);
                        break;

                    case 7:
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemSelling.Add(new ItemSellingProduct());
                            grid.RowCount = _listOfItemSelling.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, _listOfItemSelling.Count, 2); // fokus ke column code product
                        break;

                    default:
                        break;
                }
            }
        }

        private void gridControl_CurrentCellKeyPress(object sender, KeyPressEventArgs e)
        {
            var grid = (GridControl)sender;
            GridCurrentCell cc = grid.CurrentCell;

            // validasi input angka untuk column quantity, discount dan price
            switch (cc.ColIndex)
            {
                case 5: // quantity
                case 6: // discount
                case 7: // price
                    e.Handled = KeyPressHelper.NumericOnly(e);
                    break;

                default:
                    break;
            }
        }

        private void gridControl_CurrentCellValidated(object sender, EventArgs e)
        {
            var grid = (GridControl)sender;

            GridCurrentCell cc = grid.CurrentCell;

            Product product = null;
            ItemSellingProduct itemSelling = null;

            var rowIndex = cc.RowIndex - 1;

            if (_listOfItemSelling.Count > rowIndex)
            {
                itemSelling = _listOfItemSelling[rowIndex];
                product = itemSelling.Product;
            }            

            if (product != null)
            {
                switch (cc.ColIndex)
                {
                    case 4: // column description
                        itemSelling.description = cc.Renderer.ControlValue.ToString();
                        break;

                    case 5: // column quantity
                        itemSelling.quantity = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);

                        itemSelling.discount = GetDiskonSellingFix(product, itemSelling.quantity, itemSelling.discount);
                        itemSelling.selling_price = GetPriceSellingFix(product, itemSelling.quantity, itemSelling.selling_price);
                        break;

                    case 6: // column discount
                        itemSelling.discount = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    case 7: // column price
                        itemSelling.selling_price = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    default:
                        break;
                }

                SetItemProduct(grid, cc.RowIndex, product, itemSelling.quantity, itemSelling.selling_price, itemSelling.discount, itemSelling.description);
                grid.Refresh();

                RefreshTotal();
                
                if (cc.ColIndex == 5 || cc.ColIndex == 6 || cc.ColIndex == 7)
                {
                    itemSelling = _listOfItemSelling[cc.RowIndex - 1];
                    DisplayItemProduct(itemSelling);
                }
            }
        }

        private void gridControl_KeyDown(object sender, KeyEventArgs e)
        {
            Shortcut(sender, e);
        }

        private void FrmSales_KeyDown(object sender, KeyEventArgs e)
        {
            Shortcut(sender, e);
        }

        private void ShowMessage(string msg, bool isAutoReset = false)
        {
            lblPesan.Text = msg;
            tmrResetPesan.Enabled = isAutoReset;
        }

        private void ResetTransactions(bool isShowConfirm = true)
        {
            if (isShowConfirm)
            {
                var msg = "Do you want to cancel the current transaction?";

                if (!MsgHelper.MsgConfirmation(msg))
                    return;
            }

            _listOfItemSelling.Clear();
            _listOfItemSelling.Add(new ItemSellingProduct()); // add dummy objek
            _jual = null;

            gridControl.RowCount = _listOfItemSelling.Count();
            gridControl.Refresh();

            RefreshTotal();

            _isPrintStruk = true;
            ShowMessage("");
            lblStatusBar.Text = lblStatusBar.Text.Replace("Reset Pelanggan", "Find Pelanggan");

            txtCustomer.Clear();
            _customer = null;

            _currentInvoice = this._bll.GetLastInvoice();
            ShowInfoDate(_currentInvoice);

            gridControl.Focus();
            GridListControlHelper.SetCurrentCell(gridControl, _listOfItemSelling.Count, 2); // fokus ke column code product
        }

        private void Shortcut(object sender, KeyEventArgs e)
        {
            double total = 0;

            try
            {
                
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.B) // pembatalan transactions
                {                    
                    total = SumGrid(_listOfItemSelling);

                    if (total > 0)
                    {
                        ResetTransactions(); // reset transactions dengan menampilkan pesan konfirmasi
                    }
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D) // hapus item transactions
                {
                    total = SumGrid(_listOfItemSelling);

                    if (total > 0)
                    {
                        HapusItemTransactions(); // hapus item transactions
                    }
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N) // tanpa invoice/struk
                {
                    if (_isPrintStruk)
                    {
                        _isPrintStruk = false;
                        ShowMessage("Tanpa invoice/struk transactions", true);
                    }

                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.X) // tutup form active
                {
                    this.Close();
                }
                else
                {                    
                    if (KeyPressHelper.IsShortcutKey(Keys.F5, e) || KeyPressHelper.IsShortcutKey(Keys.F6, e) || 
                        KeyPressHelper.IsShortcutKey(Keys.F7, e))
                    {
                        var colIndex = 5;
                        var rowIndex = this.gridControl.CurrentCell.RowIndex;

                        switch (e.KeyCode)
                        {
                            case Keys.F5: // edit quantity
                                colIndex = 5;
                                break;

                            case Keys.F6: // edit discount
                                colIndex = 6;
                                break;

                            case Keys.F7: // edit price
                                colIndex = 7;
                                break;

                            default:
                                break;
                        }

                        if (gridControl.RowCount > 1 && gridControl.RowCount == rowIndex)
                        {
                            gridControl.Focus();
                            GridListControlHelper.SetCurrentCell(gridControl, _listOfItemSelling.Count - 1, colIndex);
                        }
                    }
                    else
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F3: // input product
                                gridControl.Focus();
                                GridListControlHelper.SetCurrentCell(gridControl, _listOfItemSelling.Count, 2); // fokus ke column code product

                                break;

                            case Keys.F4: // Find/reset pelanggan

                                if (_customer == null) // Find pelanggan
                                {
                                    txtCustomer.Enabled = true;
                                    txtCustomer.Focus();
                                }
                                else // reset pelanggan
                                {
                                    _customer = null;
                                    txtCustomer.Clear();

                                    lblStatusBar.Text = lblStatusBar.Text.Replace("Reset Pelanggan", "Find Pelanggan");
                                }                                

                                break;

                            case Keys.F8: // cek invoice terakhir
                                var limit = 1000;

                                var tglMulai = DateTime.Today.AddDays(-7);

                                var listOfSelling = _bll.GetByLimit(tglMulai, DateTime.Today, limit);
                                if (!(listOfSelling.Count > 0))
                                {
                                    ShowMessage("Not yet there info invoice terakhir", true);
                                    return;
                                }

                                var frmListInvoice = new FrmLookupInvoice("Invoice List", listOfSelling);
                                frmListInvoice.ShowDialog();

                                break;

                            case Keys.F10: // pay

                                e.SuppressKeyPress = true;

                                if (this._jual == null)
                                    _jual = new SellingProduct();

                                _jual.total_invoice = SumGrid(_listOfItemSelling);

                                if (!(_jual.total_invoice > 0))
                                {
                                    ShowMessage("You haven't completed the product data input yet!", true);
                                    return;
                                }                                

                                _jual.user_id = this._user.user_id;
                                _jual.User = this._user;

                                if (this._customer != null)
                                {
                                    _jual.customer_id = this._customer.customer_id;
                                    _jual.Customer = this._customer;
                                }

                                _jual.invoice = _currentInvoice;
                                _jual.date = DateTime.Today;
                                _jual.due_date = DateTimeHelper.GetNullDateTime();
                                _jual.is_cash = true;
                                
                                _jual.item_jual = this._listOfItemSelling.Where(f => f.Product != null).ToList();
                                foreach (var item in _jual.item_jual)
                                {
                                    if (!(item.purchase_price > 0))
                                        item.purchase_price = item.Product.purchase_price;

                                    if (!(item.selling_price > 0))
                                        item.selling_price = GetPriceSellingFix(item.Product, item.quantity - item.return_quantity, item.Product.selling_price);
                                }

                                DisplayTotal(lblTotal.Text);

                                var frmPay = new FrmPay("Payment", _jual, _bll);
                                frmPay.Listener = this;
                                frmPay.ShowDialog();

                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }
        }

        private void HapusItemTransactions()
        {
            var sale = new SellingProduct();
            sale.item_jual = this._listOfItemSelling.Where(f => f.Product != null).ToList();

            var frmHapusTransactions = new FrmDeleteItemTransactions("Delete Item Transactions", sale);
            frmHapusTransactions.Listener = this;
            frmHapusTransactions.ShowDialog();
        }

        public void Ok(object sender, object data)
        {
            // filter based data
            if (data is Product) // pencarian product baku
            {
                var product = (Product)data;

                IPriceWholesaleBll hargaWholesaleBll = new PriceWholesaleBll(_log);
                product.list_of_harga_grosir = hargaWholesaleBll.GetListPriceWholesale(product.product_id).ToList();	

                if (!_GeneralSupplier.is_negative_stock_allowed_for_products)
                {
                    if (product.is_stock_minus)
                    {
                        ShowMessage("Sorry stock product should not minus", true);
                        GridListControlHelper.SelectCellText(this.gridControl, _rowIndex, 3);
                        return;
                    }
                }

                double discount = 0;
                if (_customer != null)
                {
                    discount = _customer.discount;
                }

                if (!(discount > 0))
                {
                    var diskonProduct = GetDiskonSellingFix(product, 1, product.discount);
                    discount = diskonProduct > 0 ? diskonProduct : product.Category.discount;
                }

                ItemSellingProduct itemSelling = null;

                // cek item product already entered atau Not yet ?
                var itemProduct = GetExistItemProduct(product.product_id);

                if (itemProduct != null) // already there, tinggal update quantity
                {
                    var index = _listOfItemSelling.IndexOf(itemProduct);

                    UpdateItemProduct(this.gridControl, index);
                    this.gridControl.GetCellRenderer(_rowIndex, _colIndex).ControlText = string.Empty;

                    itemSelling = _listOfItemSelling[index];
                }
                else
                {
                    SetItemProduct(this.gridControl, _rowIndex, product, discount: discount);
                    itemSelling = _listOfItemSelling[_rowIndex - 1];

                    if (this.gridControl.RowCount == _rowIndex)
                    {
                        _listOfItemSelling.Add(new ItemSellingProduct());
                        this.gridControl.RowCount = _listOfItemSelling.Count;
                    }
                }
                
                this.gridControl.Refresh();
                RefreshTotal();
                DisplayItemProduct(itemSelling);

                if (_GeneralSupplier.is_show_additional_sales_item_information)
                {
                    // fokus ke column description
                    GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 4);
                }
                else
                {
                    if (_GeneralSupplier.is_focus_on_inputting_quantity_column)
                    {
                        GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 5); // fokus ke column quantity
                    }
                    else
                    {
                        if (itemProduct != null)
                            GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, 2); // fokus ke column code
                        else
                            GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex + 1, 2); // fokus kerow berikutnya
                    }  
                }                              
            }
            else if (data is Customer) // pencarian customer
            {
                this._customer = (Customer)data;
                txtCustomer.Text = this._customer.name_customer;

                ShowMessage("");
                lblStatusBar.Text = lblStatusBar.Text.Replace("Find Pelanggan", "Reset Pelanggan");

                KeyPressHelper.NextFocus();
            }
            else if (data is SellingProduct) // payment
            {
                var sale = (SellingProduct)data;

                if (_GeneralSupplier.is_auto_print)
                {
                    if (_isPrintStruk)
                    {
                        switch (_GeneralSupplier.type_printer)
                        {
                            case TypePrinter.DotMatrix:
                                PrintInvoiceDotMatrix(_jual);
                                break;

                            case TypePrinter.MiniPOS:
                                PrintInvoiceMiniPOS(_jual);
                                break;

                            default:
                                // do nothing
                                break;
                        }
                    }                    
                }

                var refund = Math.Abs(sale.jumlah_pay - sale.grand_total);
                DisplayRefund(NumberHelper.NumberToString(refund));
                tmrDisplayKalimatPenutup.Enabled = true;

                lblRefund.Text = string.Format("Refund: {0}", NumberHelper.NumberToString(refund));

                ResetTransactions(false);                
            }
            else // filter bardasarkan name form
            {
                var frmName = sender.GetType().Name;

                switch (frmName)
                {
                    case "FrmDeleteItemTransactions":
                        var noTransactions = (int)((dynamic)data).noTransactions;

                        var itemSelling = _listOfItemSelling[noTransactions - 1];
                        itemSelling.entity_state = EntityState.Deleted;

                        _listOfItemSelling.Remove(itemSelling);

                        gridControl.RowCount = _listOfItemSelling.Count();
                        gridControl.Refresh();

                        RefreshTotal();

                        GridListControlHelper.SetCurrentCell(gridControl, _listOfItemSelling.Count, 2);
                        break;

                    default:
                        break;
                }
            }
        }
        
        public void Ok(object sender, bool isNewData, object data)
        {
            throw new NotImplementedException();
        }

        private void txtCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var customerName = ((AdvancedTextbox)sender).Text;

                if (customerName.Length == 0)
                {
                    ShowMessage("Name pelanggan should not empty", true);
                    return;
                }

                ICustomerBll bll = new CustomerBll(_log);
                var listOfCustomer = bll.GetByName(customerName);

                if (listOfCustomer.Count == 0)
                {
                    ShowMessage("Data pelanggan not found", true);

                    txtCustomer.Focus();
                    txtCustomer.SelectAll();

                }
                else if (listOfCustomer.Count == 1)
                {
                    _customer = listOfCustomer[0];
                    txtCustomer.Text = _customer.name_customer;

                    ShowMessage("");
                    lblStatusBar.Text = lblStatusBar.Text.Replace("Find Customer", "Reset Customer");

                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Data Customer", listOfCustomer);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }

        private void txtCustomer_Leave(object sender, EventArgs e)
        {
            var obj = (AdvancedTextbox)sender;

            obj.Enabled = false;
            obj.BackColor = Color.FromArgb(232, 235, 242);

            if (_customer == null)
                obj.Clear();
        }

        private void FrmSales_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Utils.IsRunningUnderIDE())
            {
                e.Cancel = !CloseCancel();
            }            
        }

        private bool CloseCancel()
        {
            var total = SumGrid(_listOfItemSelling);

            if (total > 0)
            {
                ShowMessage("already there transactions, form tidak bisa ditutup", true);
                return false;
            }
            else
            {
                return true;
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
                _GeneralSupplier.jumlah_karakter, _GeneralSupplier.jumlah_gulung, _customer != null, FontSize: _GeneralSupplier.size_font,
                autocutCode: autocutCode, openCashDrawerCode: openCashDrawerCode);
        }        
    }
}
