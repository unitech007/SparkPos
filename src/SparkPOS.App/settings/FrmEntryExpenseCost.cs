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
using SparkPOS.Helper.UI.Template;
using SparkPOS.App.Lookup;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper.UserControl;
using SparkPOS.App.Reference;
using ConceptCave.WaitCursor;
using log4net;

namespace SparkPOS.App.Expense
{
    public partial class FrmEntryExpenseCost : FrmEntryStandard, IListener
    {
        private IExpenseCostBll _bll = null;
        private ExpenseCost _expense = null;
        private IList<ItemExpenseCost> _listOfItemExpense = new List<ItemExpenseCost>();
        private IList<ItemExpenseCost> _listOfItemExpenseOld = new List<ItemExpenseCost>();
        private IList<ItemExpenseCost> _listOfItemExpenseDeleted = new List<ItemExpenseCost>();
                
        private int _rowIndex = 0;
        private int _colIndex = 0;

        private bool _isNewData = false;
        private ILog _log;
        private User _user;

        public IListener Listener { private get; set; }

        public FrmEntryExpenseCost(string header, IExpenseCostBll bll) 
            : base()
        {            
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._bll = bll;
            this._isNewData = true;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = bll.GetLastInvoice();
            dtpDate.Value = DateTime.Today;

            _listOfItemExpense.Add(new ItemExpenseCost()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);
        }

        public FrmEntryExpenseCost(string header, ExpenseCost expense, IExpenseCostBll bll)
            : base()
        {
             InitializeComponent(); 
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            base.SetButtonSelesaiToClose();
            this._bll = bll;
            this._expense = expense;
            this._log = MainProgram.log;
            this._user = MainProgram.user;

            txtInvoice.Text = this._expense.invoice;
            dtpDate.Value = (DateTime)this._expense.date;

            txtKeterangan.Text = this._expense.description;

            // save data lama
            _listOfItemExpenseOld.Clear();
            foreach (var item in this._expense.item_expense_cost)
            {
                _listOfItemExpenseOld.Add(new ItemExpenseCost
                {
                    expense_item_id = item.expense_item_id,
                    quantity = item.quantity,
                    price = item.price
                });
            }
            
            _listOfItemExpense = this._expense.item_expense_cost;
            _listOfItemExpense.Add(new ItemExpenseCost()); // add dummy objek

            InitGridControl(gridControl);
            MainProgram.GlobalLanguageChange(this);

            RefreshTotal();
        }

        private void InitGridControl(GridControl grid)
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Type Expense", Width = 350 });
            gridListProperties.Add(new GridListControlProperties { Header = "quantity", Width = 50 });
            gridListProperties.Add(new GridListControlProperties { Header = "price", Width = 90 });
            gridListProperties.Add(new GridListControlProperties { Header = "Sub Total", Width = 110 });
            gridListProperties.Add(new GridListControlProperties { Header = "Action" });

            GridListControlHelper.InitializeGridListControl<ItemExpenseCost>(grid, _listOfItemExpense, gridListProperties);

            grid.PushButtonClick += delegate(object sender, GridCellPushButtonClickEventArgs e)
            {
                if (e.ColIndex == 6)
                {
                    if (grid.RowCount == 1)
                    {
                        MsgHelper.MsgWarning("Minimum 1 item expense must entered !");
                        return;
                    }

                    if (MsgHelper.MsgDelete())
                    {
                        var itemExpense = _listOfItemExpense[e.RowIndex - 1];
                        itemExpense.entity_state = EntityState.Deleted;

                        _listOfItemExpenseDeleted.Add(itemExpense);
                        _listOfItemExpense.Remove(itemExpense);

                        grid.RowCount = _listOfItemExpense.Count();
                        grid.Refresh();

                        RefreshTotal();
                    }                    
                }
            };

            grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                // Make sure the cell falls inside the grid
                if (e.RowIndex > 0)
                {
                    if (!(_listOfItemExpense.Count > 0))
                        return;

                    var itemExpense = _listOfItemExpense[e.RowIndex - 1];
                    var typeExpense = itemExpense.TypeExpense;

                    if (e.RowIndex % 2 == 0)
                        e.Style.BackColor = ColorCollection.BACK_COLOR_ALTERNATE;

                    switch (e.ColIndex)
                    {
                        case 1: // no urut
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.Enabled = false;
                            e.Style.CellValue = e.RowIndex.ToString();
                            break;

                        case 2: // name type expense
                            if (typeExpense != null)
                                e.Style.CellValue = typeExpense.name_expense_type;

                            break;

                        case 3: // quantity
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellValue = itemExpense.quantity;

                            break;

                        case 4: // price
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.CellValue = NumberHelper.NumberToString(itemExpense.price);

                            break;

                        case 5: // subtotal
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                            e.Style.Enabled = false;
                            e.Style.CellValue = NumberHelper.NumberToString(itemExpense.quantity * itemExpense.price);
                            break;

                        case 6: // button hapus
                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                            e.Style.CellType = GridCellTypeName.PushButton;
                            e.Style.Enabled = true;
                            if (MainProgram.currentLanguage == "en-US")
                            {
                                e.Style.Description = "Delete";
                            }
                            else if (MainProgram.currentLanguage == "ar-SA")
                            {
                                e.Style.Description = "يمسح";
                            }
                          
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

        private double SumGrid(IList<ItemExpenseCost> listOfItemExpense)
        {
            double total = 0;
            foreach (var item in _listOfItemExpense.Where(f => f.TypeExpense != null))
            {
                total += item.price * item.quantity;
            }

            return Math.Round(total, MidpointRounding.AwayFromZero);
        }

        private void RefreshTotal()
        {
            lblTotal.Text = NumberHelper.NumberToString(SumGrid(_listOfItemExpense));
        }

        protected override void Save()
        {
            var total = SumGrid(this._listOfItemExpense);
            if (!(total > 0))
            {
                MsgHelper.MsgWarning("You haven't completed the product data input yet!");
                return;
            }

            if (!MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
                return;

            if (_isNewData)
                _expense = new ExpenseCost();

            _expense.user_id = this._user.user_id;
            _expense.User = this._user;
            _expense.invoice = txtInvoice.Text;
            _expense.date = dtpDate.Value;
            _expense.description = txtKeterangan.Text;

            _expense.item_expense_cost = this._listOfItemExpense.Where(f => f.TypeExpense != null).ToList();

            if (!_isNewData) // update
                _expense.item_expense_cost_deleted = _listOfItemExpenseDeleted.ToList();

            var result = 0;
            var validationError = new ValidationError();

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                if (_isNewData)
                {
                    result = _bll.Save(_expense, ref validationError);
                }
                else
                {
                    result = _bll.Update(_expense, ref validationError);
                }

                if (result > 0)
                {
                    Listener.Ok(this, _isNewData, _expense);

                    _listOfItemExpense.Clear();
                    _listOfItemExpenseDeleted.Clear();                                        

                    this.Close();
                }
                else
                {
                    if (validationError.Message.NullToString().Length > 0)
                    {
                        MsgHelper.MsgWarning(validationError.Message);
                        base.SetFocusObject(validationError.PropertyName, this);
                    }
                    else
                        MsgHelper.MsgUpdateError();
                }
            }            
        }

        protected override void Cancel()
        {
            // restore data lama
            if (!_isNewData)
            {
                // restore item yang di edit
                var itemsModified = this._expense.item_expense_cost.Where(f => f.TypeExpense != null && f.entity_state == EntityState.Modified)
                                                     .ToArray();

                foreach (var item in itemsModified)
                {
                    var itemExpense = _listOfItemExpenseOld.Where(f => f.expense_item_id == item.expense_item_id)
                                                                   .SingleOrDefault();

                    if (itemExpense != null)
                    {
                        item.quantity = itemExpense.quantity;
                        item.price = itemExpense.price;
                    }
                }

                // restore item yang di delete
                var itemsDeleted = _listOfItemExpenseDeleted.Where(f => f.TypeExpense != null && f.entity_state == EntityState.Deleted)
                                                                .ToArray();
                foreach (var item in itemsDeleted)
                {
                    item.entity_state = EntityState.Unchanged;
                    this._expense.item_expense_cost.Add(item);
                }

                _listOfItemExpenseDeleted.Clear();
            }

            base.Cancel();
        }

        public void Ok(object sender, object data)
        {
            if (data is TypeExpense) // pencarian type expense
            {
                var typeExpense = (TypeExpense)data;

                if (!IsExist(typeExpense.expense_type_id))
                {
                    SetItemTypeExpense(this.gridControl, _rowIndex, _colIndex + 1, typeExpense);
                    this.gridControl.Refresh();
                    RefreshTotal();

                    GridListControlHelper.SetCurrentCell(this.gridControl, _rowIndex, _colIndex + 1);
                }
                else
                {
                    MsgHelper.MsgWarning("Data type expense already entered");
                    GridListControlHelper.SelectCellText(this.gridControl, _rowIndex, _colIndex);
                }
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            // do nothing
        }

        private void txtKeterangan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                gridControl.Focus();
                GridListControlHelper.SetCurrentCell(gridControl, 1, 2); // fokus ke column name product
            }
        }

        private bool IsExist(string typeExpenseId)
        {
            var count = _listOfItemExpense.Where(f => f.expense_type_id != null && f.expense_type_id.ToLower() == typeExpenseId.ToLower())
                                              .Count();

            return (count > 0);
        }

        private void SetItemTypeExpense(GridControl grid, int rowIndex, int colIndex, TypeExpense typeExpense, double quantity = 1, double price = 0)
        {
            ItemExpenseCost itemExpenseCost;

            if (_isNewData)
            {
                itemExpenseCost = new ItemExpenseCost();
            }
            else
            {
                itemExpenseCost = _listOfItemExpense[rowIndex - 1];

                if (itemExpenseCost.entity_state == EntityState.Unchanged)
                    itemExpenseCost.entity_state = EntityState.Modified;
            }

            itemExpenseCost.expense_type_id = typeExpense.expense_type_id;
            itemExpenseCost.TypeExpense = typeExpense;
            itemExpenseCost.quantity = quantity;
            itemExpenseCost.price = price;

            _listOfItemExpense[rowIndex - 1] = itemExpenseCost;
        }

        private void gridControl_CurrentCellKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var grid = (GridControl)sender;

                var rowIndex = grid.CurrentCell.RowIndex;
                var colIndex = grid.CurrentCell.ColIndex;
                
                TypeExpense typeExpense = null;

                switch (colIndex)
                {
                    case 2: // pencarian based name type expense

                        GridCurrentCell cc = grid.CurrentCell;
                        var nameProduct = cc.Renderer.ControlValue.ToString();

                        ITypeExpenseBll bll = new TypeExpenseBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
                        var listOfTypeExpense = bll.GetByName(nameProduct);

                        if (listOfTypeExpense.Count == 0)
                        {
                            MsgHelper.MsgWarning("Data type expense not found");
                            GridListControlHelper.SelectCellText(grid, rowIndex, colIndex);
                        }
                        else if (listOfTypeExpense.Count == 1)
                        {
                            typeExpense = listOfTypeExpense[0];

                            if (!IsExist(typeExpense.expense_type_id))
                            {
                                SetItemTypeExpense(grid, rowIndex, colIndex, typeExpense);
                                grid.Refresh();
                                RefreshTotal();

                                GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                            }
                            else
                            {
                                MsgHelper.MsgWarning("Data type expense already entered");
                                GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex);
                            }
                        }
                        else // data lebih dari one, tampilkan form lookup
                        {
                            _rowIndex = rowIndex;
                            _colIndex = colIndex;

                            var frmLookup = new FrmLookupReference("Data Type Expense", listOfTypeExpense);
                            frmLookup.Listener = this;
                            frmLookup.ShowDialog();
                        }

                        break;

                    case 3:
                        GridListControlHelper.SetCurrentCell(grid, rowIndex, colIndex + 1);
                        break;

                    case 4:
                        if (grid.RowCount == rowIndex)
                        {
                            _listOfItemExpense.Add(new ItemExpenseCost());
                            grid.RowCount = _listOfItemExpense.Count;
                        }

                        GridListControlHelper.SetCurrentCell(grid, rowIndex + 1, 2); // fokus ke column code product
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

            // validasi input angka untuk column quantity dan price
            switch (cc.ColIndex)
            {
                case 4: // quantity
                case 5: // price
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

            var itemExpense = _listOfItemExpense[cc.RowIndex - 1];
            var typeExpense = itemExpense.TypeExpense;

            if (typeExpense != null)
            {
                switch (cc.ColIndex)
                {
                    case 3: // column quantity
                        itemExpense.quantity = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString(), true);
                        break;

                    case 4: // column price
                        itemExpense.price = NumberHelper.StringToDouble(cc.Renderer.ControlValue.ToString());
                        break;

                    default:
                        break;
                }

                SetItemTypeExpense(grid, cc.RowIndex, cc.ColIndex, typeExpense, itemExpense.quantity, itemExpense.price);
                grid.Refresh();

                RefreshTotal();
            }           
        }

        private void ShowEntryTypeExpense()
        {
            var isGrant = RolePrivilegeHelper.IsHaveRightAccess("mnuTypeExpense", _user);
            if (!isGrant)
            {
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
                return;
            }

            ITypeExpenseBll typeExpenseBll = new TypeExpenseBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);
            var frmEntryTypeExpense = new FrmEntryTypeExpense("Add Data Type Cost", typeExpenseBll);
            frmEntryTypeExpense.Listener = this;
            frmEntryTypeExpense.ShowDialog();
        }

        private void FrmEntryExpenseCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressHelper.IsShortcutKey(Keys.F1, e)) // tambah data type expense
            {
                ShowEntryTypeExpense();
            }
        }

        private void txtDiskon_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void txtPPN_TextChanged(object sender, EventArgs e)
        {
            RefreshTotal();
        }

        private void FrmEntryExpenseCost_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hapus objek dumm
            if (!_isNewData)
            {
                var itemsToRemove = _expense.item_expense_cost.Where(f => f.TypeExpense == null && f.entity_state == EntityState.Added)
                                                   .ToArray();

                foreach (var item in itemsToRemove)
                {
                    _expense.item_expense_cost.Remove(item);
                }
            }
        }

        private void txtPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
                Save();
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
