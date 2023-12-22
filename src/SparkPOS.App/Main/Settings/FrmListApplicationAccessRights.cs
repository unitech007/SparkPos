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
using SparkPOS.Helper;
using SparkPOS.Helper.UserControl;
using ConceptCave.WaitCursor;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Tools;
using log4net;
using SparkPOS.Helper.UI.Template;
using MultilingualApp;

namespace SparkPOS.App.Settings
{
    public partial class FrmListApplicationAccessRights : FrmListEmptyBody, IListener
    {
         private IList<Role> _listOfRole = new List<Role>();
     private IRoleBll _bll; // deklarasi objek business logic layer 
          private IList<RolePrivilege> _listOfRolePrivilege = null;
        private IList<MenuApplication> _listOfMenuApplication = null;
        private ILog _log;

        public FrmListApplicationAccessRights(string header, User user, string menuId)
            : base()
        {
             InitializeComponent();  
          
            base.WindowState = FormWindowState.Maximized;

            _log = MainProgram.log;
            _bll = new RoleBll(_log);            

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
            {
                if (role.is_grant)
                {
                    LoadMenuParent();
                    SetMenuParent(cmbMenu);

                    LoadData();
                }          

                cmbMenu.Enabled = role.is_grant;
                chkSelectAll.Enabled = role.is_grant;
                btnSave.Enabled = role.is_grant;
            }    

            InitGridList();
            
            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfRole.Count);
           
            if (MainProgram.currentLanguage == "ar-SA")
            MainProgram.GlobalLanguageChange(this);
            base.SetHeader(header);
            //  LanguageHelper.TranslateText(header, MainProgram.currentLanguage);

        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Right Access", Width = 400 });
            gridListProperties.Add(new GridListControlProperties { Header = "Status", Width = 100 });

            GridListControlHelper.InitializeGridListControl<Role>(this.gridList, _listOfRole, gridListProperties);
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            if (_listOfRole.Count > 0)
            {
                this.gridList.SetSelected(0, true);
                HandleSelectionChanged(this.gridList);
            }                

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {

                if (_listOfRole.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {

                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfRole.Count)
                        {
                            var role = _listOfRole[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = role.name_role;
                                    break;

                                case 3:
                                    e.Style.CellValue = role.is_active ? "Active" : "Non Active";
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

        private bool IsAdministrator(string roleName)
        {
            return roleName.ToLower() == "administrator";
        }

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfRole = _bll.GetAll();

                GridListControlHelper.Refresh<Role>(this.gridList, _listOfRole);
            }

            ResetButton();
        }

        private void LoadMenuParent()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                IMenuBll menuBll = new MenuBll(_log);
                _listOfMenuApplication = menuBll.GetAll();
            }
        }

        private void SetMenuParent(ComboBox combo)
        {
            foreach (var menu in _listOfMenuApplication.Where(f => f.parent_id == null && f.is_active == true))
            {
                cmbMenu.Items.Add(menu.menu_title);
            }

            cmbMenu.SelectedIndex = 0;
        }

        private void LoadMenuChild(string menuParentId)
        {

            treeViewAdv.Nodes.Clear();
        //    MainProgram.GlobalLanguageChange(this);
            var menuChild = _listOfMenuApplication.Where(f => f.parent_id == menuParentId && f.is_active == true && f.name_form.Length > 0)
                                               .OrderBy(f => f.order_number)
                                               .ToList();

            foreach (var itemMenuChild in menuChild)
            {
                var nodeChild = new TreeNodeAdv();
                nodeChild.Text = itemMenuChild.menu_title;
                nodeChild.Tag = itemMenuChild.menu_id;
                nodeChild.ShowCheckBox = true;
                nodeChild.InteractiveCheckBox = true;
                nodeChild.ExpandAll();

                IItemMenuBll itemMenuBll = new ItemMenuBll(_log);
                var listOfItemMenu = itemMenuBll.GetByMenu(itemMenuChild.menu_id);

                // filter menu report yg ditampilkan hanya Right Access SELECT
                if (itemMenuChild.name_menu.Substring(0, 6) == "mnuLap")
                {
                    listOfItemMenu = listOfItemMenu.Where(f => f.grant_id == Convert.ToInt32(GrantState.SELECT)).ToList();
                }

                foreach (var itemMenu in listOfItemMenu)
                {
                    var nodeTag = new TreeNodeAdv();
                    nodeTag.Text = itemMenu.description;
                    nodeTag.Tag = itemMenu.grant_id;
                    nodeTag.ShowCheckBox = true;
                    nodeTag.InteractiveCheckBox = true;

                    nodeChild.Nodes.Add(nodeTag);
                }

                treeViewAdv.Nodes.Add(nodeChild);
            }

        }

        private void CheckRecursive(TreeNodeAdv treeNode, bool isSave)
        {
            var nodeParent = treeNode.Parent;

            if (isSave)
            {
                if (!base.IsSelectedItem(this.gridList.SelectedIndex, this.Text))
                    return;

                var obj = _listOfRole[this.gridList.SelectedIndex];

                if (nodeParent.Tag != null)
                {
                    var rolePrivilege = new RolePrivilege
                    {
                        role_id = obj.role_id,
                        menu_id = nodeParent.Tag.ToString(),
                        grant_id = Convert.ToInt32(treeNode.Tag),
                        is_grant = treeNode.CheckState == CheckState.Checked
                    };

                    IRolePrivilegeBll rolePrivilegeBll = new RolePrivilegeBll(_log);
                    var result = rolePrivilegeBll.Save(rolePrivilege);
                }
            }
            else
            {
                treeNode.CheckState = CheckState.Unchecked;

                if (_listOfRolePrivilege != null)
                {
                    if (nodeParent.Tag != null)
                    {
                        if (treeNode.Tag != null)
                        {
                            var role = _listOfRole[this.gridList.SelectedIndex];

                            var rolePrivilege = _listOfRolePrivilege.Where(f => f.role_id == role.role_id && f.menu_id == nodeParent.Tag.ToString() &&
                                                                          f.grant_id == Convert.ToInt32(treeNode.Tag))
                                                                          .SingleOrDefault();
                            if (rolePrivilege != null)
                                treeNode.CheckState = rolePrivilege.is_grant ? CheckState.Checked : CheckState.Unchecked;
                        }
                    }
                }
            }

            // Print each node recursively.
            foreach (TreeNodeAdv tn in treeNode.Nodes)
            {
                CheckRecursive(tn, isSave);
            }
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfRole.Count > 0);
        }

        private void EnabledObject(bool isEnabled)
        {

            cmbMenu.Enabled = isEnabled;
            treeViewAdv.Enabled = isEnabled;
            chkSelectAll.Enabled = isEnabled;
            btnSave.Enabled = isEnabled;
        }

        private void HandleSelectionChanged(GridListControl gridList)
        {
            if (gridList.SelectedIndex < 0)
                return;

            var role = _listOfRole[gridList.SelectedIndex];

            EnabledObject(true);
            lblRole.Text = "Right Access : ";

            if (role != null)
            {
                if (IsAdministrator(role.name_role))
                    EnabledObject(false);

                if (MainProgram.currentLanguage == "en-US")
                {
                    lblRole.Text = string.Format("Right Access : {0}", role.name_role);
                  

                }
                else if (MainProgram.currentLanguage == "ar-SA")
                {
                    lblRole.Text = string.Format("حق الوصول: : {0}", role.name_role);

                }

                IRolePrivilegeBll rolePrivilegeBll = new RolePrivilegeBll(_log);
                _listOfRolePrivilege = rolePrivilegeBll.GetByRole(role.role_id);

                foreach (TreeNodeAdv node in treeViewAdv.Nodes)
                {
                    CheckRecursive(node, false);
                }
            }
        }

        protected override void Add()
        {
            string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryRightAccess(formTitle + this.Text, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var role = _listOfRole[index];

            if (IsAdministrator(role.name_role))
            {
                MsgHelper.MsgWarning("Sorry Right Access 'Administrator' cannot be edited");
                return;
            }
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryRightAccess(formTitle + this.Text, role, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var role = _listOfRole[index];

            if (IsAdministrator(role.name_role))
            {
                MsgHelper.MsgWarning("Sorry Right Access 'Administrator' can not be deleted");
                return;
            }

            if (MsgHelper.MsgDelete())
            {
                var result = _bll.Delete(role);
                if (result > 0)
                {
                    GridListControlHelper.RemoveObject<Role>(this.gridList, _listOfRole, role);
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
            var role = (Role)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Role>(this.gridList, _listOfRole, role);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Role>(this.gridList, _listOfRole, role);
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (TreeNodeAdv node in treeViewAdv.Nodes)
            {
                CheckRecursive(node, true);
            }

            HandleSelectionChanged(this.gridList);
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            foreach (TreeNodeAdv node in treeViewAdv.Nodes)
            {
                SelectAll(node, chk.Checked);
            }
        }

        private void SelectAll(TreeNodeAdv treeNode, bool isChecked)
        {
            treeNode.CheckState = isChecked ? CheckState.Checked : CheckState.Unchecked;

            foreach (TreeNodeAdv tn in treeNode.Nodes)
            {
                SelectAll(tn, isChecked);
            }
        }

        private void cmbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                if (MainProgram.currentLanguage == "ar-SA")
            {
                MainProgram.GlobalLanguageChange(this);
                base.SetHeader("حقوق الوصول إلى التطبيق");
            }
                
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                var obj = (ComboBox)sender;

                var menuParent = _listOfMenuApplication.Where(f => f.parent_id == null && f.is_active == true).ToList()[obj.SelectedIndex];

                if (menuParent != null)
                    LoadMenuChild(menuParent.menu_id);

                foreach (TreeNodeAdv node in treeViewAdv.Nodes)
                {
                    CheckRecursive(node, false);
                }

                chkSelectAll.Checked = false;
            }


            chkSelectAll.Checked = false;
            if (MainProgram.currentLanguage == "ar-SA")
            {
                MainProgram.GlobalLanguageChange(this);
                base.SetHeader("حقوق الوصول إلى التطبيق");
            }
        }

        private void gridList_SelectedValueChanged(object sender, EventArgs e)
        {
            HandleSelectionChanged((GridListControl)sender);
        }
    }
}
