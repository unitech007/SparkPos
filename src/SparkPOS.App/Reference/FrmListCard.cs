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
using System.IO;
using System.Diagnostics;
using MultilingualApp;
using System.Resources;
using System.Globalization;

namespace SparkPOS.App.Reference
{
    public partial class FrmListCard : FrmListStandard, IListener
    {        
        private ICardBll _bll; // deklarasi objek business logic layer 
        private IList<Card> _listOfCard = new List<Card>();
        private ILog _log;

        public FrmListCard(string header, User user, string menuId)
            : base(header)
        {
             InitializeComponent(); 

            _log = MainProgram.log;
            _bll = new CardBll(MainProgram.isUseWebAPI, MainProgram.baseUrl, _log);

            // set Right Access untuk SELECT
            var role = user.GetRoleByMenuAndGrant(menuId, GrantState.SELECT);
            if (role != null)
                if (role.is_grant)
                {
                    LoadData();
                }                    

            InitGridList();
            // set Right Access selain SELECT (ADD, PERBAIKI dan DELETE)
            RolePrivilegeHelper.SetRightAccess(this, user, menuId, _listOfCard.Count);
            MainProgram.GlobalLanguageChange(this);

        }

     

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "Name Card", Width = 600 });
            gridListProperties.Add(new GridListControlProperties { Header = "Description" });

            LanguageHelper.TranslateGridListControlHeaders(gridListProperties,MainProgram.currentLanguage);

            GridListControlHelper.InitializeGridListControl<Card>(this.gridList, _listOfCard, gridListProperties);

            if (_listOfCard.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfCard.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfCard.Count)
                        {
                            var card = _listOfCard[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = card.card_name;
                                    break;

                                case 3:
                                    e.Style.CellValue = card.is_debit ? "Debit Card" : "Credit Card";
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
         //   List<string> columnHeaders = LanguageHelper.ProcessGridListControl(this.gridList,  MainProgram.currentLanguage);
        }

        private void LoadData()
        {
            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                _listOfCard = _bll.GetAll();

                GridListControlHelper.Refresh<Card>(this.gridList, _listOfCard);
            }

            ResetButton();
        }

        private void ResetButton()
        {
            base.SetActiveBtnPerbaikiAndHapus(_listOfCard.Count > 0);
        }

        protected override void Add()
        {
             //string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
             string formTitle = LanguageHelper.GetTranslatedAddData(MainProgram.currentLanguage);
            var frm = new FrmEntryCard(formTitle + this.TabText, _bll);
            frm.Listener = this;
            frm.ShowDialog();
        }

        protected override void Edit()
        {
            var index = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(index, this.Text))
                return;

            var card = _listOfCard[index];
            string formTitle = LanguageHelper.GetTranslatedEditData(MainProgram.currentLanguage);
            var frm = new FrmEntryCard(formTitle + this.TabText, card, _bll);
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
                var card = _listOfCard[index];

                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var result = _bll.Delete(card);
                    if (result > 0)
                    {
                        GridListControlHelper.RemoveObject<Card>(this.gridList, _listOfCard, card);
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
            var card = (Card)data;

            if (isNewData)
            {
                GridListControlHelper.AddObject<Card>(this.gridList, _listOfCard, card);
                ResetButton();
            }
            else
                GridListControlHelper.UpdateObject<Card>(this.gridList, _listOfCard, card);
        }
    }
}
