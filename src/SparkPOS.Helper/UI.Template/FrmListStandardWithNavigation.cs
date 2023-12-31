﻿/**
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
using WeifenLuo.WinFormsUI.Docking;
using SparkPOS.Helper;

namespace SparkPOS.Helper.UI.Template
{
    /// <summary>
    /// Base class form untuk menampilkan data
    /// </summary>
    public partial class FrmListStandardWithNavigation : BaseFrmList
    {
        public FrmListStandardWithNavigation()
        {
             InitializeComponent();  
            
           // //MainProgram.GlobalLanguageChange(this);

            ColorManagerHelper.SetTheme(this, this);
        }

        public FrmListStandardWithNavigation(string header) 
            : this()
        {
            this.Text = header;
            this.TabText = header;
            this.lblHeader.Text = header;
        }

        protected void SetInfoHalaman(int pageNumber, int pagesCount)
        {
            if (pageNumber > 0 && pagesCount == 0)
                pagesCount = 1;

          //  this.lblInfoNavigasi.Text = string.Format("Hal ke : {0} dari {1}", pageNumber, pagesCount);
           
            if (MainProgram.currentLanguage == "en-US")
            {
                this.lblInfoNavigasi.Text = string.Format("Page : {0} of {1}", pageNumber, pagesCount);
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.lblInfoNavigasi.Text = string.Format("الصفحة: {0} من {1}", pageNumber, pagesCount);
            }
        }

        protected void SetStateBtnNavigation(int pageNumber, int pagesCount)
        {
            if ((pageNumber == 0 || pagesCount == 0) || (pageNumber == 1 && pagesCount == 1))
            {
                btnMoveFirst.Enabled = false;
                btnMovePrevious.Enabled = false;
                btnMoveNext.Enabled = false;
                btnMoveLast.Enabled = false;
            }
            else if (pageNumber == 1)
            {
                btnMoveFirst.Enabled = false;
                btnMovePrevious.Enabled = false;
                btnMoveNext.Enabled = true;
                btnMoveLast.Enabled = true;
            }
            else if (pageNumber == pagesCount && pageNumber > 1)
            {
                btnMoveFirst.Enabled = true;
                btnMovePrevious.Enabled = true;
                btnMoveNext.Enabled = false;
                btnMoveLast.Enabled = false;
            }
            else
            {
                btnMoveFirst.Enabled = true;
                btnMovePrevious.Enabled = true;
                btnMoveNext.Enabled = true;
                btnMoveLast.Enabled = true;
            }
        }

        protected override void SetActiveBtnPerbaikiAndHapus(bool status)
        {
            btnPerbaiki.Enabled = status;
            btnHapus.Enabled = status;
        }

        protected virtual void SetActiveStatusBtnNavigasi(bool status)
        {
            btnMoveFirst.Enabled = status;
            btnMovePrevious.Enabled = status;
            btnMoveNext.Enabled = status;
            btnMoveLast.Enabled = status;
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void btnPerbaiki_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void gridList_DoubleClick(object sender, EventArgs e)
        {
            if (btnPerbaiki.Enabled)
                Edit();
        }        

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var btnSender = (Button)sender;

            Point ptLowerLeft = new Point(-mnuPopupImportData.Size.Width + btnSender.Size.Width, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);

            mnuPopupImportData.Show(ptLowerLeft);
        }

        private void mnuBukaFileMaster_Click(object sender, EventArgs e)
        {
            OpenFileMaster();
        }

        private void mnuImportFileMaster_Click(object sender, EventArgs e)
        {
            ImportData();
        }

        private void FrmListStandardWithNavigation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F8:
                    if (btnTambah.Enabled)
                        Add();

                    break;

                case Keys.F9:
                    if (btnPerbaiki.Enabled)
                        Edit();

                    break;

                case Keys.F10:
                    if (btnHapus.Enabled)
                        Delete();

                    break;

                default:
                    break;
            }
        }

        private void FrmListStandardWithNavigation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                Cancel();
        }

        private void btnMoveFirst_Click(object sender, EventArgs e)
        {
            MoveFirst();
        }

        private void btnMovePrevious_Click(object sender, EventArgs e)
        {
            MovePrevious();
        }

        private void btnMoveNext_Click(object sender, EventArgs e)
        {
            MoveNext();
        }

        private void btnMoveLast_Click(object sender, EventArgs e)
        {
            MoveLast();
        }

        private void mnuExportData_Click(object sender, EventArgs e)
        {
            ExportData();
        }

        private void updLimit_ValueChanged(object sender, EventArgs e)
        {
            LimitRowChanged();
        }
    }
}
