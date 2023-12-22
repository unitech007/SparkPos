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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SparkPOS.Helper.UserControl
{
    [DefaultEvent("BtnShowClicked")]
    public partial class FilterRangeDate : System.Windows.Forms.UserControl
    {
        public delegate void EventHandler(object sender, EventArgs e);
        public event EventHandler BtnShowClicked;
        public event EventHandler ChkShowAllDataClicked;        

        public FilterRangeDate()
        {
            InitializeComponent();

            dtpDateMulai.Value = DateTime.Today;
            dtpDateSelesai.Value = DateTime.Today;

            this.EnabledChanged += delegate(object sender, EventArgs e)
            {
                chkShowAllData.Checked = false;
            };
        }

        public DateTime DateMulai
        {
            get { return dtpDateMulai.Value; }
        }

        public DateTime DateSelesai
        {
            get { return dtpDateSelesai.Value; }
        }

        public bool IsCheckedShowAllData
        {
            get { return chkShowAllData.Checked; }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (BtnShowClicked != null)
                BtnShowClicked(sender, e);
        }

        private void chkShowAllData_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var isEnable = false;

            if (chk.Checked)
                isEnable = false;
            else
                isEnable = true;

            dtpDateMulai.Enabled = isEnable;
            dtpDateSelesai.Enabled = isEnable;
            btnShow.Enabled = isEnable;

            if (ChkShowAllDataClicked != null)
                ChkShowAllDataClicked(sender, e);
        }
    }
}
