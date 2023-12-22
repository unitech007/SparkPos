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

namespace SparkPOS.App.Settings
{
    public partial class FrmListOperator : FrmListStandard, IListener
    {
        private IUserBll _bll; // deklarasi objek business logic layer 
        private IList<User> _listOfOperator = new List<User>();
        private IList<Role> _listOfRole = new List<Role>();
        private ILog _log;

        public FrmListOperator(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent(); 
            _log = MainProgram.log;
            _bll = new UserBll(_log);

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                    LoadData();

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfOperator.Count);
            MainProgram.GlobalLanguageChange(this);

        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Operator", Width = 800 });
            gridListProperties.Add(new GridListControlProperties { Header = "Role", Width = 300 });
            gridListProperties.Add(new GridListControlProperties { Header = "Status Active" });

            GridListControlHelper.InitializeGridListControl<User>(this.gridList, _listOfOperator, gridListProperties);
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            if (_listOfOperator.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfOperator.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfOperator.Count)
                        {
                            var userOperator = _listOfOperator[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = userOperator.name_user;
                                    break;

                                case 3:
                                    var role = userOperator.Role;
                                    if (role != null)
                                        e.Style.CellValue = role.name_role;

                                    break;

                                case 4:
                                    e.Style.CellValue = userOperator.is_active ? "Active" : "Non Active";
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
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
                _listOfOperator = _bll.GetAll();

                IRoleBll roleBll = new RoleBll(_log);
                _listOfRole = roleBll.GetByStatus(true);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfOperator.Count > 0);
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryOperator(formTitle + this.TabText, _listOfRole, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var userOperator = _listOfOperator[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryOperator(formTitle + this.TabText, userOperator, _listOfRole, _bll);
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
                var userOperator = _listOfOperator[index];

                var result = _bll.Delete(userOperator);
                if (result > 0)
                {
                    GridListControlHelper.RemoveObject<User>(this.gridList, _listOfOperator, userOperator);
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
            var userOperator = (User)data;
            
            if (isNewData)
            {
                GridListControlHelper.AddObject<User>(this.gridList, _listOfOperator, userOperator);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<User>(this.gridList, _listOfOperator, userOperator);
        }
    }
}
