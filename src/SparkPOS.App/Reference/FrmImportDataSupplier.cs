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
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using log4net;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using SparkPOS.Helper;
using SparkPOS.Helper.UI.Template;
using ConceptCave.WaitCursor;

namespace SparkPOS.App.Reference
{
    public partial class FrmImportDataSupplier : FrmDialogImport
    {
        private const string ImportTitle = "Supplier";
        private ILog _log;

        public IListener Listener { private get; set; }

        public FrmImportDataSupplier(string header)
            : base()
        {
             InitializeComponent(); 
            base.SetHeader(header);

            _log = MainProgram.log;
            MainProgram.GlobalLanguageChange(this);
        }

        protected override void OpenFileExcel()
        {
            var msg = "keyMasterExcel";
            if (MsgHelper.MsgConfirmation(string.Format(msg, ImportTitle)))
            {
                var fileMaster = Utils.GetAppPath() + @"\File Import Excel\Master Data\data_supplier.xlsx";

                if (!File.Exists(fileMaster))
                {
                    msg = "Sorry file master {0} not found.";
                    MsgHelper.MsgWarning(msg, ImportTitle);
                    return;
                }

                try
                {
                    using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                    {
                        Process.Start(fileMaster);
                    }
                }
                catch
                {
                    msg = "Key-FailedMaster";

                    MsgHelper.MsgWarning(msg, ImportTitle);
                }
            }
        }

        protected override void BrowseFileExcel()
        {
            cmbWorksheet.Items.Clear();

            using (var dlgOpen = new OpenFileDialog())
            {
                // Excel file only
                dlgOpen.Filter = "Microsoft Excel files (*.xlsx)|*.xlsx";
                dlgOpen.Title = string.Format("Import data master {0}", ImportTitle);

                var result = dlgOpen.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtPath.Text = dlgOpen.FileName;

                    IImportExportDataBll<Supplier> importDataBll = new ImportExportDataSupplierBll(txtPath.Text, _log);

                    if (importDataBll.IsOpened())
                    {
                        var msg = "Sorry, the master file {0} is currently open. Please close it first.";
                        MsgHelper.MsgWarning(msg, ImportTitle);
                        txtPath.Clear();

                        return;
                    }

                    var listOfWorksheet = importDataBll.GetWorksheets();

                    if (listOfWorksheet.Count > 0)
                    {
                        foreach (var workSheet in listOfWorksheet)
                        {
                            cmbWorksheet.Items.Add(workSheet);
                        }

                        cmbWorksheet.SelectedIndex = 0;
                    }
                }
            }
        }

        protected override void ImportData()
        {
            var msg = string.Empty;

            if (txtPath.Text.Length == 0)
            {
                MsgHelper.MsgWarning("Lokasi dan name file Excel Not yet selected.");
                return;
            }

            IImportExportDataBll<Supplier> importDataBll = new ImportExportDataSupplierBll(txtPath.Text, _log);

            if (importDataBll.IsOpened())
            {
                msg = "Sorry, the master file {0} is currently open. Please close it first.";
                MsgHelper.MsgWarning(msg, ImportTitle);

                return;
            }

            if (!importDataBll.IsValidFormat(cmbWorksheet.Text))
            {
                msg = "Sorry, the master file format {0} is invalid, the import process cannot continue.";
                MsgHelper.MsgWarning(msg, ImportTitle);

                return;
            }

            if (MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    var rowCount = 0;
                    var result = importDataBll.Import(cmbWorksheet.Text, ref rowCount);

                    if (result)
                    {
                        msg = "Master data import {0} was successful.";
                        MsgHelper.MsgInfo(string.Format(msg, ImportTitle));

                        Listener.Ok(this, null);
                        this.Close();
                    }
                    else
                    {
                        if (rowCount == 0)
                        {
                            msg = "Key-DataEmpty";
                            MsgHelper.MsgInfo(string.Format(msg, ImportTitle));
                        }
                    }
                }
            }
        }
    }
}
