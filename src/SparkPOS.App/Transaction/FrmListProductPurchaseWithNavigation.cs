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
 * License for the specific language governing permissions and limitations GetByID
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
using SparkPOS.App.Lookup;
using MultilingualApp;

namespace SparkPOS.App.Transactions
{
    public partial class FrmListProductPurchaseWithNavigation : FrmListEmptyBodyWithNavigation, IListener
    {
        private IPurchaseProductBll _bll; // deklarasi objek business logic layer 
        private IList<PurchaseProduct> _listOfPurchase = new List<PurchaseProduct>();
        private ILog _log;
        private User _user;

        private int _pageNumber = 1;
        private int _pagesCount = 0;
        private int _pageSize = 0;
        private string _menuId;

        public FrmListProductPurchaseWithNavigation(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _pageSize = MainProgram.pageSize;
            _log = MainProgram.log;
            _bll = new PurchaseProductBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            _user = user;
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

                txtNameSupplier.Enabled = role.is_grant;
                btnFind.Enabled = role.is_grant;

                filterRangeDate.Enabled = role.is_grant;
            } 

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfPurchase.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "CreditTerm", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Supplier", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Debt", Width = 140 });
            gridListProperties.Add(new GridListControlProperties { Header = "Remaining Debt", Width = 140 });
            gridListProperties.Add(new GridListControlProperties { Header = "Payment History" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<PurchaseProduct>(this.gridList, _listOfPurchase, gridListProperties, false);

            if (_listOfPurchase.Count > 0)
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
                                var beli = _listOfPurchase[index];

                                IDebtPaymentProductBll bll = new DebtPaymentProductBll(_log);
                                var listOfPaymentHistory = bll.GetPaymentHistory(beli.purchase_id);

                                if (listOfPaymentHistory.Count > 0)
                                {
                                    var frmPaymentHistory = new FrmLookupPaymentHistory("Debit Payment History", beli, listOfPaymentHistory);

                                    if (MainProgram.currentLanguage == "en-US")
                                    {
                                         frmPaymentHistory = new FrmLookupPaymentHistory("Debit Payment History", beli, listOfPaymentHistory);

                                    }
                                    else if (MainProgram.currentLanguage == "ar-SA")
                                    {
                                         frmPaymentHistory = new FrmLookupPaymentHistory("سجل الدفع المدين", beli, listOfPaymentHistory);

                                    }
                                    frmPaymentHistory.ShowDialog();
                                }
                                else
                                {
                                    MsgHelper.MsgInfo("Not yet there Information history payment");
                                }
                            }

                            break;

                        default:
                            break;
                    }
                }
            };

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfPurchase.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfPurchase.Count)
                        {
                            double totalInvoice = 0;

                            var beli = _listOfPurchase[rowIndex];
                            if (beli != null)
                                totalInvoice = beli.grand_total;


                            var isReturn = beli.purchase_return_id != null;

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
                                    e.Style.CellValue = DateTimeHelper.DateToString(beli.date);
                                    break;

                                case 3:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(beli.due_date);
                                    break;

                                case 4:
                                    e.Style.CellValue = beli.invoice;
                                    break;

                                case 5:
                                    if (beli.Supplier != null)
                                        e.Style.CellValue = beli.Supplier.name_supplier;

                                    break;

                                case 6:
                                    e.Style.CellValue = beli.description;
                                    break;

                                case 7:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(totalInvoice);
                                    break;

                                case 8:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(totalInvoice - beli.total_payment);
                                    break;

                                case 9: // button history payment
                                    e.Style.Enabled = beli.due_date != null;
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellType = GridCellTypeName.PushButton;
                                    if (MainProgram.currentLanguage == "en-US")
                                    {
                                        e.Style.Description = "Check History";
                                    }
                                    else if (MainProgram.currentLanguage == "ar-SA")
                                    {
                                        e.Style.Description = "تحقق من التاريخ";
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

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfPurchase = _bll.GetAll(_pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<PurchaseProduct>(this.gridList, _listOfPurchase);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfPurchase.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void LoadData(string supplierName)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfPurchase = _bll.GetByName(supplierName, _pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<PurchaseProduct>(this.gridList, _listOfPurchase);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfPurchase.Count > 0))
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
                _listOfPurchase = _bll.GetByDate(tanggalMulai, tanggalSelesai, _pageNumber, _pageSize, ref _pagesCount);
                GridListControlHelper.Refresh<PurchaseProduct>(this.gridList, _listOfPurchase);

                base.SetInfoHalaman(_pageNumber, _pagesCount);
                base.SetStateBtnNavigation(_pageNumber, _pagesCount);

                if (!(_listOfPurchase.Count > 0))
                {
                    base.SetInfoHalaman(0, 0);
                    base.SetStateBtnNavigation(0, 0); // non Activate button navigasi
                }
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfPurchase.Count > 0);

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfPurchase.Count);
        }

        protected override void Add()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
                var frm = new FrmEntryProductPurchase(formTitle + this.Text, _bll);
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
                var beli = _listOfPurchase[index];
                beli.tanggal_creditTerm_old = beli.due_date;
                beli.item_beli = _bll.GetItemPurchase(beli.purchase_id).ToList();

                LogicalThreadContext.Properties["OldValue"] = beli.ToJson();
                string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
                var frm = new FrmEntryProductPurchase(formTitle + this.Text, beli, _bll);
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
                    var beli = _listOfPurchase[index];
                    var result = _bll.Delete(beli);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<PurchaseProduct>(this.gridList, _listOfPurchase, beli);
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
            var beli = (PurchaseProduct)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<PurchaseProduct>(this.gridList, _listOfPurchase, beli);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<PurchaseProduct>(this.gridList, _listOfPurchase, beli);
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
            txtNameSupplier.Clear();
            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeDate_ChkShowAllDataClicked(object sender, EventArgs e)
        {
            _pageNumber = 1;
            txtNameSupplier.Clear();

            var chk = (CheckBox)sender;

            if (chk.Checked)
            {
                LoadData();
                txtNameSupplier.Enabled = false;
                btnFind.Enabled = false;
            }                
            else
            {
                LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                txtNameSupplier.Enabled = true;
                btnFind.Enabled = true;
            }                
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (txtNameSupplier.Text.Length > 0)
            {
                _pageNumber = 1;
                LoadData(txtNameSupplier.Text);
            }                
        }

        private void txtNameSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                btnFind_Click(sender, e);
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
                    else if (txtNameSupplier.Text.Length > 0)
                    {
                        LoadData(txtNameSupplier.Text);
                    }
                    else
                    {
                        LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
                    }
                }
            }
            
            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfPurchase.Count);
        }
    }
}
