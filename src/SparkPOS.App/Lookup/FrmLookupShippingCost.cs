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

using Syncfusion.Windows.Forms.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SparkPOS.Helper;
using SparkPOS.Model;
using SparkPOS.Model.RajaOngkir;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using RestSharp;
using Newtonsoft.Json;
using ConceptCave.WaitCursor;
using SparkPOS.Helper.UserControl;
using SparkPOS.Helper.UI.Template;
using MultilingualApp;

namespace SparkPOS.App.Lookup
{
    public partial class FrmLookupShippingCost : FrmLookupEmptyBody, IListener
    {
        private IList<costs> _listOfCost = new List<costs>();
        private IList<RegencyShippingCostsByRaja> _listOfregency = new List<RegencyShippingCostsByRaja>();
        private RegencyAsalRajaOngkir _regencyAsal = null;
        private RegencyTujuanRajaOngkir _regencyTujuan = null;

        public IListener Listener { private get; set; }

        public FrmLookupShippingCost(string header)
            : base()
        {
             InitializeComponent();  
            ColorManagerHelper.SetTheme(this, this);

            base.SetHeader(header);
            this._listOfregency = MainProgram.ListOfRegency;

            InitGridList();
            MainProgram.GlobalLanguageChange(this);
        }

        private void InitGridList()
        {
            var gridListProperties = new List<GridListControlProperties>();

            gridListProperties.Add(new GridListControlProperties { Header = "No", Width = 30 });
            gridListProperties.Add(new GridListControlProperties { Header = "courier", Width = 70 });
            gridListProperties.Add(new GridListControlProperties { Header = "Service Types", Width = 330 });
            gridListProperties.Add(new GridListControlProperties { Header = "Rates" });
            LanguageHelper.TranslateGridListControlHeaders(gridListProperties, MainProgram.currentLanguage);

            GridListControlHelper.InitializeGridListControl<costs>(this.gridList, _listOfCost, gridListProperties);

            if (_listOfCost.Count > 0)
                this.gridList.SetSelected(0, true);

            this.gridList.Grid.QueryCellInfo += delegate(object sender, GridQueryCellInfoEventArgs e)
            {
                if (_listOfCost.Count > 0)
                {
                    if (e.RowIndex > 0)
                    {
                        var rowIndex = e.RowIndex - 1;

                        if (rowIndex < _listOfCost.Count)
                        {
                            var cost = _listOfCost[rowIndex];

                            switch (e.ColIndex)
                            {
                                case 2:
                                    e.Style.CellValue = cost.kurir_code;
                                    break;

                                case 3:
                                    e.Style.CellValue = string.Format("{0} ({1})", cost.service, cost.description);
                                    break;

                                case 4:
                                    var costDetail = cost.cost[0];
                                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                                    e.Style.CellValue = NumberHelper.NumberToString(costDetail.value);
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

        protected override void Select()
        {
            var rowIndex = this.gridList.SelectedIndex;

            if (!base.IsSelectedItem(rowIndex, this.Text))
                return;

            var ongkir = _listOfCost[rowIndex];
            this.Listener.Ok(this, ongkir);

            this.Close();
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (base.IsButtonSelectEnabled)
                Select();
        }

        private void gridList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                if (base.IsButtonSelectEnabled)
                    Select();
            }
        }

        private void btnCekOngkir_Click(object sender, EventArgs e)
        {
            if (this._regencyAsal == null || txtRegencyAsal.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'City/Regency Origin' should not empty !");
                txtRegencyAsal.Focus();

                return;
            }

            if (this._regencyTujuan == null || txtRegencyTujuan.Text.Length == 0)
            {
                MsgHelper.MsgWarning("'Destination City/Regency' should not be empty !");
                txtRegencyTujuan.Focus();

                return;
            }

            var berat = (int)NumberHelper.StringToDouble(txtBerat.Text);

            if (!(berat > 0))
            {
                MsgHelper.MsgWarning("'Shipping weight' should not be empty !");
                txtBerat.Focus();
                return;
            }

            if (MainProgram.rajaOngkirKey.Length == 0)
            {
                MsgHelper.MsgWarning("key-costShipping");
                return;
            }

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                CekOngkir();
            }            
        }

        private void CekOngkir()
        {
            var key = MainProgram.rajaOngkirKey; // api key raja ongkir

            var baseUrl = "http://api.rajaongkir.com/starter/";

            var client = new RestClient(baseUrl);
            //ToDo : need to uncomment
           // client.BaseUrl = new Uri(baseUrl);

            // Version gratis raja ongkir, hanya menSupportpengecekan jne, tiki dan pos
            var listOfKurir = new string[] { "jne", "tiki", "pos" };

            _listOfCost.Clear();
            foreach (var courier in listOfKurir)
            {
                var request = new RestRequest("cost", Method.Post);
                request.AddHeader("key", key);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                var query = new query
                {
                    origin = _regencyAsal.regency_id.ToString(),
                    destination = _regencyTujuan.regency_id.ToString(),
                    weight = (int)NumberHelper.StringToDouble(txtBerat.Text),
                    courier = courier
                };

                request.RequestFormat = DataFormat.Json;
                request.AddBody(query);

                try
                {
                    var response = client.Execute(request);

                    var ongkir = JsonConvert.DeserializeObject<root>(response.Content);
                    if (ongkir.rajaongkir.results[0].costs.Count > 0)
                    {
                        foreach (var costs in ongkir.rajaongkir.results[0].costs)
                        {
                            costs.kurir_code = courier.ToUpper();
                            _listOfCost.Add(costs);
                        }
                    }
                }
                catch (Exception ex)
            {
                MainProgram.LogException(ex);
                // Error handling and logging
                var msg =  MainProgram.GlobalWarningMessage();
                MsgHelper.MsgWarning(msg);
                //WarningMessageHandler.ShowTranslatedWarning(msg, MainProgram.currentLanguage);
            }
            }
            
            GridListControlHelper.Refresh<costs>(this.gridList, _listOfCost);
            base.SetActiveBtnSelect(_listOfCost.Count > 0);
        }

        public void Ok(object sender, object data)
        {
            if (data is RegencyAsalRajaOngkir) // hasil pencarian regency asal
            {
                this._regencyAsal = (RegencyAsalRajaOngkir)data;
                txtRegencyAsal.Text = this._regencyAsal.name_regency;
                KeyPressHelper.NextFocus();
            }
            else if (data is RegencyTujuanRajaOngkir) // hasil pencarian regency tujuan
            {
                this._regencyTujuan = (RegencyTujuanRajaOngkir)data;
                txtRegencyTujuan.Text = this._regencyTujuan.name_regency;
                KeyPressHelper.NextFocus();
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            throw new NotImplementedException();
        }

        private IList<T> GetRegencyByName<T>(string name)
        {
            var result = _listOfregency.Where(f => f.name_regency.ToLower().Contains(name.ToLower()) ||
                                                f.Provinsi.name_province.ToLower().Contains(name.ToLower()))
                                         .OrderBy(f => f.Provinsi.name_province)
                                         .ThenBy(f => f.name_regency);

            var serializedParent = JsonConvert.SerializeObject(result);
            IList<T> listOfChild = JsonConvert.DeserializeObject<IList<T>>(serializedParent);

            return listOfChild;
        }

        private void txtRegencyAsal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var regency = ((AdvancedTextbox)sender).Text;

                IList<RegencyAsalRajaOngkir> listOfRegency = GetRegencyByName<RegencyAsalRajaOngkir>(regency);

                if (listOfRegency.Count == 0)
                {
                    MsgHelper.MsgWarning("Data city/regency origin not found");
                    txtRegencyAsal.Focus();
                    txtRegencyAsal.SelectAll();

                }
                else if (listOfRegency.Count == 1)
                {
                    _regencyAsal = listOfRegency[0];
                    txtRegencyAsal.Text = _regencyAsal.name_regency;
                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Data City/Regency Origin", listOfRegency);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }

        private void txtRegencyTujuan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEnter(e))
            {
                var regency = ((AdvancedTextbox)sender).Text;

                IList<RegencyTujuanRajaOngkir> listOfRegency = GetRegencyByName<RegencyTujuanRajaOngkir>(regency);

                if (listOfRegency.Count == 0)
                {
                    MsgHelper.MsgWarning("Destination city/regency data not found");
                    txtRegencyTujuan.Focus();
                    txtRegencyTujuan.SelectAll();

                }
                else if (listOfRegency.Count == 1)
                {
                    _regencyTujuan = listOfRegency[0];
                    txtRegencyTujuan.Text = _regencyTujuan.name_regency;
                    KeyPressHelper.NextFocus();
                }
                else // data lebih dari one
                {
                    var frmLookup = new FrmLookupReference("Destinstion Data City/Regency", listOfRegency);
                    frmLookup.Listener = this;
                    frmLookup.ShowDialog();
                }
            }
        }
    }
}
