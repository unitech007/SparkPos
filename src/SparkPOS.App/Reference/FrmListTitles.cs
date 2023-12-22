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

namespace SparkPOS.App.Reference
{
    public partial class FrmListTitles : FrmListStandard, IListener
    {                
        private ITitlesBll _bll; // deklarasi objek business logic layer 
        private IList<job_titles> _listOfTitles = new List<job_titles>();
        private ILog _log;

        public FrmListTitles(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent();

            _log = MainProgram.log;
            _bll = new TitlesBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                    LoadData();

            InitGridList();

            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfTitles.Count);
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Job_Titles", Width = 250 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description", Width = 300 });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);
            GridListControlHelper.InitializeGridListControl<job_titles>(this.gridList, _listOfTitles, gridListProperties);

            if (_listOfTitles.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfTitles.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfTitles.Count)
                        {
                            var job_titles = _listOfTitles[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = job_titles.name_job_titles;
                                    break;

                                case 3:
                                    e.Style.CellValue = job_titles.description;
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
                _listOfTitles = _bll.GetAll();
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfTitles.Count > 0);
        }

        protected override void Add()
        {
          string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryTitles(formTitle + this.TabText, _bll);
            //if (MainProgram.currentLanguage == "en-US")
            //{
            //   frm = new FrmEntryTitles("Add Data " + this.TabText, _bll);
            //}
            //else if (MainProgram.currentLanguage == "ar-SA")
            //{
            //     frm = new FrmEntryTitles("إضافة عناوين وظائف البيانات", _bll);
            //}
            

            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            var job_titles = _listOfTitles[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryTitles(formTitle + this.TabText, job_titles, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Delete()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.TabText))
                return;

            if (MsgHelper.MsgDelete())
            {
                var job_titles = _listOfTitles[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(job_titles);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<job_titles>(this.gridList, _listOfTitles, job_titles);
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
            var job_titles = (job_titles)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<job_titles>(this.gridList, _listOfTitles, job_titles);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<job_titles>(this.gridList, _listOfTitles, job_titles);
        }
    }
}
