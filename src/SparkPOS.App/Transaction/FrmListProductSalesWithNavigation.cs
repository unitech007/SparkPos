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
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Helper.UI.Template;
using SparkPOS.Helper;
using Syncfusion.Windows.Forms.Grid;
using ConceptCave.WaitCursor;
using log4net;
using Microsoft.Reporting.WinForms;
using SparkPOS.Helper.RAWPrinting;
using SparkPOS.App.Lookup;
using MultilingualApp;

namespace SparkPOS.App.Transactions
{
    public partial class FrmListProductSalesWithNavigation : FrmListEmptyBodyWithNavigation, IListener
    {
        private ISellingProductBll _bll; // deklarasi objek business logic layer 
        private IList<SellingProduct> _listOfSelling = new List<SellingProduct>();
        private IList<Area> _listOfArea = new List<Area>();
        private ILog _log;
        private User _user;
        private GeneralSupplier _GeneralSupplier;

        private int _pageNumber = 1;
        private int _pagesCount = 0;
        private int _pageSize = 0;
        private string _menuId;

        public FrmListProductSalesWithNavigation(string header, User user, string menuId)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _pageSize = MainProgram.pageSize;
            _log = MainProgram.log;
            _listOfArea = MainProgram.ListOfArea;
            _bll = new SellingProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
            _GeneralSupplier = MainProgram.GeneralSupplier;
            _menuId = menuId;            

            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    this.updLimit.Value = _pageSize;
                    LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                }                    

                txtNameCustomer.Enabled = role.is_grant;
                btnFind.Enabled = role.is_grant;

                filterRangeDate.Enabled = role.is_grant;
            }            

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSelling.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "CreditTerm", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Customer", Width = _GeneralSupplier.type_printer == TypePrinter.InkJet ? 180 : 260 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 350 });
            gridListProperties.Add(new GridListControlProperties { Header = "Credit", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Remaining Credit", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "History Payment", Width = 80 });

            if (_GeneralSupplier.type_printer == TypePrinter.InkJet)
            {
                gridListProperties.Add(new GridListControlProperties { Header = "Print Invoice/Label", Width = 80 });
                gridListProperties.Add(new GridListControlProperties { Header = "" });
            }
            else
            {
                gridListProperties.Add(new GridListControlProperties { Header = "Print Invoice", Width = 80 });
            }
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<SellingProduct>(this.gridList, _listOfSelling, gridListProperties, false, rowHeight: 40);

            if (_GeneralSupplier.type_printer == TypePrinter.InkJet)
            {
                // merge header column print invoice/label
                this.gridList.Grid.CoveredRanges.Add(GridRangeInfo.Cells(0, 10, 0, 11));
            }            

            if (_listOfSelling.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.PushButtonClick += delegate(object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.RowIndex > 0)
                {
                    var index = e.RowIndex - 1;

                    switch (e.ColIndex)
                    {
                        case 9: // history payment
                            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                            {
                                var sale = _listOfSelling[index];

                                IPaymentCreditProductBll bll = new PaymentCreditProductBll(_log);
                                var listOfPaymentHistory = bll.GetPaymentHistory(sale.sale_id);

                                if (listOfPaymentHistory.Count > 0)
                                {
                                    var frmPaymentHistory = new FrmLookupPaymentHistory("History Payment Credit", sale, listOfPaymentHistory);

                                    if (MainProgram.currentLanguage == "en-US")
                                    {
                                         frmPaymentHistory = new FrmLookupPaymentHistory("History Payment Credit", sale, listOfPaymentHistory);

                                    }
                                    else if (MainProgram.currentLanguage == "ar-SA")
                                    {
                                         frmPaymentHistory = new FrmLookupPaymentHistory("ائتمان الدفع التاريخي", sale, listOfPaymentHistory);

                                    }
                                    frmPaymentHistory.ShowDialog();
                                }
                                else
                                {
                                    MsgHelper.MsgInfo("Not yet there Information history payment");
                                }
                            }

                            break;

                        case 10: // print invoice sale
                            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                            {
                                var sale = _listOfSelling[index];

                                switch (this._GeneralSupplier.type_printer)
                                {
                                    case TypePrinter.DotMatrix:
                                        if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
                                            PrintInvoiceDotMatrix(sale);
                                        break;

                                    case TypePrinter.MiniPOS:
                                        if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
                                            PrintInvoiceMiniPOS(sale);
                                        break;

                                    default:
                                        var frmPrintInvoice = new Transaction.FrmPreviewInvoiceSales("Preview Invoice Sales", sale);

                                        if (MainProgram.currentLanguage == "en-US")
                                        {
                                             frmPrintInvoice = new Transaction.FrmPreviewInvoiceSales("Preview Invoice Sales", sale);
                                        }
                                        else if (MainProgram.currentLanguage == "ar-SA")
                                        {
                                             frmPrintInvoice = new Transaction.FrmPreviewInvoiceSales("معاينة مبيعات الفاتورة", sale);
                                        }
                                       
                                        frmPrintInvoice.ShowDialog();
                                        break;
                                }
                            }

                            break;

                        case 11: // Printing Label invoice sale
                            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                            {
                                var sale = _listOfSelling[index];

                                var frmPrintLabelInvoice = new FrmPreviewLabelInvoiceSales("Preview Label Invoice Sales", sale);
                                frmPrintLabelInvoice.ShowDialog();
                            }

                            break;

                        default:
                            break;
                    }
                }                
            };

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfSelling.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfSelling.Count)
                        {
                            double totalInvoice = 0;

                            var sale = _listOfSelling[rowIndex];
                            if (sale != null)
                                totalInvoice = sale.grand_total;


                            var isReturn = sale.return_sale_id != null;
                            var oldStyleBackColor = e.Style.BackColor;

                            if (isReturn)
                                e.Style.BackColor = Color.Red;

                            switch (e.ColIndex)
                            {
                                case 1:
                                    var noUrut = (_pageNumber - 1) * _pageSize + e.RowIndex;
                                    e.Style.CellValue = noUrut;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    break;

                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(sale.date);
                                    break;

                                case 3:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(sale.due_date);
                                    break;

                                case 4:
                                    e.Style.CellValue = sale.invoice;
                                    break;

                                case 5:
                                    if (sale.Customer != null)
                                    {
                                        SetAreaCustomer(sale.Customer);
                                        e.Style.CellValue = sale.Customer.name_customer;
                                    }                                        

                                    break;

                                case 6:
                                    e.Style.CellValue = sale.description;
                                    break;

                                case 7:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(totalInvoice);
                                    break;

                                case 8:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(totalInvoice - sale.total_payment);
                                    break;

                                case 9: // button history payment
                                    e.Style.Enabled = sale.due_date != null;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellType = GridCellTypeName.PushButton;                                    
                                    e.Style.BackColor = oldStyleBackColor;

                                    if (MainProgram.currentLanguage == "en-US")
                                    {
                                        e.Style.Description = "Check History";
                                    }
                                    else if (MainProgram.currentLanguage == "ar-SA")
                                    {
                                        e.Style.Description = "تحقق من التاريخ";
                                    }
                                    

                                    break;

                                case 10: // button print invoice
                                    e.Style.Enabled = sale.Customer != null;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellType = GridCellTypeName.PushButton;                                    
                                    e.Style.BackColor = oldStyleBackColor;
                                    if (MainProgram.currentLanguage == "en-US")
                                    {
                                        e.Style.Description = "Print Invoice";
                                    }
                                    else if (MainProgram.currentLanguage == "ar-SA")
                                    {
                                        e.Style.Description = "فاتورة طباعة";
                                    }
                                    
                                    break;

                                case 11: // button Printing Label invoice
                                    if (_GeneralSupplier.type_printer == TypePrinter.InkJet)
                                    {
                                        e.Style.Enabled = sale.Customer != null;
                                        e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                        e.Style.CellType = GridCellTypeName.PushButton;
                                        e.Style.BackColor = oldStyleBackColor;
                                        if (MainProgram.currentLanguage == "en-US")
                                        {
                                            e.Style.Description = "Printing Label Invoice";

                                        }
                                        else if (MainProgram.currentLanguage == "ar-SA")
                                        {
                                            e.Style.Description = "طباعة فاتورة الملصقات";

                                        }
                                    }                                    

                                    break;

                                default:
                                    break;
                            }

                            // we handled it, let the grid know
                            e.Handled = true;
                        }
                    }
                }
            };
        }

        private void SetAreaCustomer(Customer obj)
        {
            Provinsi provinsi = null;
            Regency regency = null;
            subdistrict subdistrict = null;

            if (!string.IsNullOrEmpty(obj.province_id))
            {
                provinsi = _listOfArea.Where(f => f.province_id == obj.province_id)
                                         .Select(f => new Provinsi { province_id = f.province_id, name_province = f.name_province })
                                         .FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(obj.regency_id))
            {
                regency = _listOfArea.Where(f => f.regency_id == obj.regency_id)
                                          .Select(f => new Regency { regency_id = f.regency_id, name_regency = f.name_regency })
                                          .FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(obj.subdistrict_id))
            {
                subdistrict = _listOfArea.Where(f => f.subdistrict_id == obj.subdistrict_id)
                                          .Select(f => new subdistrict { subdistrict_id = f.subdistrict_id, name_subdistrict = f.name_subdistrict })
                                          .FirstOrDefault();
            }

            obj.Provinsi = provinsi;
            obj.Regency = regency;
            obj.subdistrict = subdistrict;
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

        private void PrintInvoiceDotMatrix(SellingProduct sale)
        {
            IRAWPrinting printerMiniPos = new PrinterDotMatrix(_GeneralSupplier.name_printer);
            printerMiniPos.Print(sale, _GeneralSupplier.list_of_header_nota, _GeneralSupplier.jumlah_gulung, isPrintKeteranganInvoice: _GeneralSupplier.is_print_keterangan_nota);
        }

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfSelling = _bll.GetAll(_pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<SellingProduct>(this.gridList, _listOfSelling);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfSelling.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void LoadData(string customerName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfSelling = _bll.GetByName(customerName, _GeneralSupplier.is_show_additional_sales_item_information, _pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<SellingProduct>(this.gridList, _listOfSelling);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfSelling.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void LoadData(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfSelling = _bll.GetByDate(tanggalMulai, tanggalSelesai, _pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<SellingProduct>(this.gridList, _listOfSelling);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfSelling.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfSelling.Count > 0);

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSelling.Count);
        }

        protected override void Add()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
                var frm = new FrmEntrySalesProduct(formTitle + this.Text, _bll);
                frm.Listener = this;
                frm.ShowDialog();
            }            
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                var sale = _listOfSelling[index];
                sale.tanggal_creditTerm_old = sale.due_date;
                sale.item_jual = _bll.GetItemSelling(sale.sale_id).ToList();

                LogicalThreadContext.Properties["OldValue"] = sale.ToJson();
                string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
                var frm = new FrmEntrySalesProduct(formTitle + this.Text, sale, _bll);
                frm.Listener = this;
                frm.ShowDialog();
            }            
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            if (MsgHelper.MsgDelete())
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var sale = _listOfSelling[index];
                    var result = _bll.Delete(sale);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<SellingProduct>(this.gridList, _listOfSelling, sale);
                        ResetButton();
                    }
                    else
                        MsgHelper.MsgDeleteError();
                }                
            }
        }

        public void Ok(object sender, object data)
        {
            throw new NotImplementedException();
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var sale = (SellingProduct)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<SellingProduct>(this.gridList, _listOfSelling, sale);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<SellingProduct>(this.gridList, _listOfSelling, sale);
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }

        private void filterRangeDate_BtnShowClicked(object sender, EventArgs e)
        {
            var tanggalMulai = filterRangeDate.DateMulai;
            var tanggalSelesai = filterRangeDate.DateSelesai;

            if (!DateTimeHelper.IsValidRangeDate(tanggalMulai, tanggalSelesai))
            {
                MsgHelper.MsgNotValidRangeDate();
                return;
            }

            _pageNumber = 1;
            txtNameCustomer.Clear();
            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeDate_ChkShowAllDataClicked(object sender, EventArgs e)
        {
            _pageNumber = 1;
            txtNameCustomer.Clear();

            var chk = (CheckBox)sender;

            if (chk.Checked)
            {
                LoadData();
                txtNameCustomer.Enabled = false;
                btnFind.Enabled = false;
            }                
            else
            {
                LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                txtNameCustomer.Enabled = true;
                btnFind.Enabled = true;
            }                
        }

        private void txtNameCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (txtNameCustomer.Text.Length > 0)
            {
                _pageNumber = 1;
                LoadData(txtNameCustomer.Text);
            }                
        }

        protected override void MoveFirst()
        {
            _pageNumber = 1;

            RefreshData();
        }

        protected override void MovePrevious()
        {
            _pageNumber--;

            RefreshData();
        }

        protected override void MoveNext()
        {
            _pageNumber++;

            RefreshData();
        }

        protected override void MoveLast()
        {
            _pageNumber = _pagesCount;

            RefreshData();
        }

        protected override void LimitRowChanged()
        {
            MainProgram.pageSize = (int)this.updLimit.Value;
            _pageSize = MainProgram.pageSize;
            _pageNumber = 1;

            RefreshData();
        }

        private void RefreshData()
        {
            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    if (filterRangeDate.IsCheckedShowAllData)
                    {
                        LoadData();
                    } 
                    else if (txtNameCustomer.Text.Length > 0)
                    {
                        LoadData(txtNameCustomer.Text);
                    }                        
                    else
                    {
                        LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                    }
                }
            }

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfSelling.Count);
        }

        private void gridList_Click(object sender, EventArgs e)
        {

        }
    }
}
