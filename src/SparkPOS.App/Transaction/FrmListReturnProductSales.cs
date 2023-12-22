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
using MultilingualApp;

namespace SparkPOS.App.Transactions
{
    public partial class FrmListReturnProductSales : FrmListEmptyBody, IListener
    {        
        private IReturnSellingProductBll _bll; // deklarasi objek business logic layer 
        private IList<ReturnSellingProduct> _listOfReturn = new List<ReturnSellingProduct>();
        private ILog _log;
        private User _user;
        private string _menuId;

        public FrmListReturnProductSales(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  

            base.SetHeader(header);
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new ReturnSellingProductBll(_log);
            _user = user;
            _menuId = menuId;
            
            // set Right Access untuk SELECT
            var role = _user.GetRoleByMenuAndGrant(_menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                    LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);

                filterRangeDate.Enabled = role.is_grant;
            }

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfReturn.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "date", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice Return", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Invoice Selling", Width = 100 });
            gridListProperties.Add(new GridListControlProperties { Header = "Customer", Width = 400 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 500 });
            gridListProperties.Add(new GridListControlProperties { Header = "Total", Width = 150 });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<ReturnSellingProduct>(this.gridList, _listOfReturn, gridListProperties);

            if (_listOfReturn.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfReturn.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfReturn.Count)
                        {
                            double totalInvoice = 0;

                            var retur = _listOfReturn[rowIndex];

                            if (retur != null)
                                totalInvoice = retur.total_invoice;

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                                    e.Style.CellValue = DateTimeHelper.DateToString(retur.date);
                                    break;

                                case 3:
                                    e.Style.CellValue = retur.invoice;
                                    break;

                                case 4:
                                    var sale = retur.SellingProduct;
                                    if (sale != null)
                                        e.Style.CellValue = sale.invoice;

                                    break;

                                case 5:
                                    if (retur.Customer != null)
                                        e.Style.CellValue = retur.Customer.name_customer;

                                    break;

                                case 6:
                                    e.Style.CellValue = retur.description;
                                    break;

                                case 7:
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(totalInvoice);
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
                _listOfReturn = _bll.GetAll();
                GridListControlHelper.Refresh<ReturnSellingProduct>(this.gridList, _listOfReturn);
            }

            ResetButton();
        }

        private void LoadData(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfReturn = _bll.GetByDate(tanggalMulai, tanggalSelesai);
                GridListControlHelper.Refresh<ReturnSellingProduct>(this.gridList, _listOfReturn);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfReturn.Count > 0);

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, _user, _menuId, _listOfReturn.Count);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryReturnSalesProduct(formTitle + this.Text, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var retur = _listOfReturn[index];

            LogicalThreadContext.Properties["OldValue"] = retur.ToJson();
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryReturnSalesProduct(formTitle + this.Text, retur, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            if (MsgHelper.MsgDelete())
            {
                var retur = _listOfReturn[index];

                var result = _bll.Delete(retur);
                if (result > 0)
                {
                    GridListControlHelper.RemoveObject<ReturnSellingProduct>(this.gridList, _listOfReturn, retur);
                    ResetButton();
                }
                else
                    MsgHelper.MsgDeleteError();
            }
        }

        public void Ok(object sender, object data)
        {
            throw new NotImplementedException();
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            var retur = (ReturnSellingProduct)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<ReturnSellingProduct>(this.gridList, _listOfReturn, retur);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<ReturnSellingProduct>(this.gridList, _listOfReturn, retur);
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

            LoadData(tanggalMulai, tanggalSelesai);
        }

        private void filterRangeDate_ChkShowAllDataClicked(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            if (chk.Checked)
                LoadData();
            else
                LoadData(filterRangeDate.DateMulai, filterRangeDate.DateSelesai);
        }
    }
}
