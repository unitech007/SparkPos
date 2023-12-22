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
using WeifenLuo.WinFormsUI.Docking;

using SparkPOS.Helper;
using ConceptCave.WaitCursor;
using SparkPOS.App.Cashier.Transactions;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using log4net;
using AutoUpdaterDotNET;
using System.Threading;
using SparkPOS.App.Cashier.Settings;
using SparkPOS.App.Cashier.Report;

namespace SparkPOS.App.Cashier.Main
{
    public partial class FrmMain : Form, IListener
    {
        //Disable close button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;

        /// <summary>
        /// Variabel lokal untuk menampung menu id. 
        /// Menu id digunakan untuk mengeset Right Access masing-masing form yang diakses
        /// </summary>
        private Dictionary<string, string> _getMenuID;
        private ILog _log;

        private ThreadHelper _lightSleeper = new ThreadHelper();

        public bool IsLogout { get; private set; }

        public FrmMain()
        {
             InitializeComponent();  //MainProgram.GlobalLanguageChange(this);
            mainDock.BackColor = Color.FromArgb(255, 255, 255);

            _log = MainProgram.log;

            AddEventToolbar();
            MainProgram.GlobalLanguageChange(this);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitializeStatusBar();
            MainProgram.GlobalLanguageChange(this);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                if (Utils.IsRunningUnderIDE())
                {
                    return base.CreateParams;
                }
                else
                {
                    var cp = base.CreateParams;
                    cp.ClassStyle = cp.ClassStyle | CP_DISABLE_CLOSE_BUTTON;

                    // bug fixed: flicker
                    // http://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls
                    //cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED

                    return cp;
                }
            }
        }        

        private IEnumerable<ToolStripMenuItem> GetItems(ToolStripMenuItem menuItem)
        {
            foreach (var item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    var dropDownItem = (ToolStripMenuItem)item;

                    if (dropDownItem.HasDropDownItems)
                    {
                        foreach (ToolStripMenuItem subItem in GetItems(dropDownItem))
                            yield return subItem;
                    }

                    yield return (ToolStripMenuItem)item;
                }
            }
        }

        public void InisialisasiData()
        {
            SetMenuId();
            SetDisabledMenuAndToolbar(menuStrip1, toolStrip1);
            
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;            
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            _lightSleeper.Cancel();

            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {                    
                    var msg = "Key-LatestVersion";

                    var installedVersion = string.Format("{0}.{1}.{2}.{3} (v{0}.{1}.{2}{4})", args.InstalledVersion.Major, args.InstalledVersion.Minor, args.InstalledVersion.Build, args.InstalledVersion.Revision, MainProgram.stageOfDevelopment);
                    var currentVersion = string.Format("{0}.{1}.{2}.{3}", args.CurrentVersion.Major, args.CurrentVersion.Minor, args.CurrentVersion.Build, args.CurrentVersion.Revision);

                    var dialogResult = MessageBox.Show(string.Format(msg, currentVersion, installedVersion), "Update Tersedia",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Information);

                    if (dialogResult.Equals(DialogResult.Yes))
                    {
                        try
                        {
                            AutoUpdater.DownloadUpdate();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No updates are available, please try again later", "Failed to check for the latest update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Failed to connect to the server, please try again later", "Failed to check for the latest update", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetMenuId()
        {
            IMenuBll menuBll = new MenuBll(_log);
            var listOfMenu = menuBll.GetAll().Where(f => f.parent_id != null && f.name_form.Length > 0)
                                             .ToList();
            _getMenuID = new Dictionary<string, string>();

            foreach (var item in listOfMenu)
            {
                _getMenuID.Add(item.name_form, item.menu_id);
            }
        }

        /// <summary>
        /// Method untuk deactivate menu dan toolbar yang Not yet active (membaca setting tabel m_menu)
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="toolStrip"></param>
        private void SetDisabledMenuAndToolbar(MenuStrip menuStrip, ToolStrip toolStrip)
        {
            IMenuBll menuBll = new MenuBll(_log);
            var listOfMenu = menuBll.GetAll()
                                    .Where(f => f.parent_id != null && f.name_form.Length > 0)
                                    .ToList();
            
            // perulangan untuk mengecek menu dan sub menu
            foreach (ToolStripMenuItem parentMenu in menuStrip.Items)
            {
                var listOfChildMenu = GetItems(parentMenu);

                foreach (var childMenu in listOfChildMenu)
                {
                    var menu = listOfMenu.Where(f => f.name_menu == childMenu.Name)
                                         .SingleOrDefault();
                    if (menu != null)
                    {
                        childMenu.Enabled = menu.is_enabled;
                    }
                }
            }

            // perulangan untuk mengecek item toolbar
            foreach (ToolStripItem item in toolStrip.Items)
            {
                var menu = listOfMenu.Where(f => f.name_menu.Substring(3) == item.Name.Substring(2))
                                     .SingleOrDefault();
                if (menu != null)
                {
                    item.Enabled = menu.is_enabled;
                }
            }
        }

        public void InitializeStatusBar()
        {
            var dt = DateTime.Now;

            sbJam.Text = string.Format("{0:HH:mm:ss}", dt);
            sbDate.Text = string.Format("{0}, {1}", DayMonthHelper.GetDayIndonesia(dt), dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);

            if (MainProgram.user != null)
                sbOperator.Text = string.Format("Operator : {0}", MainProgram.user.name_user);

            var firstReleaseYear = 2017;
            var currentYear = DateTime.Today.Year;
            var copyright = currentYear > firstReleaseYear ? string.Format("{0} - {1}", firstReleaseYear, currentYear) : firstReleaseYear.ToString();

            var appName = string.Format(MainProgram.appName, MainProgram.currentVersion, MainProgram.stageOfDevelopment, copyright);

            this.Text = appName;
            sbNameApplication.Text = appName.Replace("&", "&&");
        }

        private void AddEventToolbar()
        {
            tbSalesProduct.Click += mnuProductSales_Click;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sbJam.Text = string.Format("{0:HH:mm:ss}", DateTime.Now);
        }

        private bool IsChildFormExists(Form frm)
        {
            return !(frm == null || frm.IsDisposed);
        }        

        private void CloseAllDocuments()
        {
            foreach (var form in MdiChildren)
            {
                form.Close();
            }                
        }

        private void ShowForm<T>(object sender, ref T form)
        {
            var header = GetMenuTitle(sender);
            var menuId = _getMenuID[GetFormName(sender)];

            if (!IsChildFormExists((DockContent)(object)form))
                form = (T)Activator.CreateInstance(typeof(T), header, MainProgram.user, menuId);

            ((DockContent)(object)form).Show(this.mainDock);
        }

        private void ShowFormDialog<T>(object sender)
        {
            var header = GetMenuTitle(sender);
            var menuName = GetMenuName(sender);

            if (menuName.Substring(0, 6) == "mnuLap")
            {
                header = string.Format("Report {0}", GetMenuTitle(sender));
            }

            if (RolePrivilegeHelper.IsHaveRightAccess(menuName, MainProgram.user, GrantState.SELECT))
            {
                var form = (T)Activator.CreateInstance(typeof(T), header);
                ((Form)(object)form).ShowDialog();
            }
            else
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
        }

        private string GetMenuTitle(object sender)
        {
            var title = string.Empty;

            if (sender is ToolStripMenuItem)
            {
                title = ((ToolStripMenuItem)sender).Text;
            }
            else
            {
                title = ((ToolStripButton)sender).Text;
            }

            return title;
        }

        private string GetMenuName(object sender)
        {
            var menuName = string.Empty;

            if (sender is ToolStripMenuItem)
            {
                menuName = ((ToolStripMenuItem)sender).Name;
            }
            else
            {
                menuName = ((ToolStripButton)sender).Name;
                menuName = string.Format("mnu{0}", menuName.Substring(2));
            }

            return menuName;
        }

        private string GetFormName(object sender)
        {
            var formName = string.Empty;

            if (sender is ToolStripMenuItem)
            {
                formName = ((ToolStripMenuItem)sender).Tag.ToString();
            }
            else
            {
                formName = ((ToolStripButton)sender).Tag.ToString();
            }

            return formName;
        }

        private void mnuProductSales_Click(object sender, EventArgs e)
        {
            var header = GetMenuTitle(sender);
            var menuId = _getMenuID[GetFormName(sender)];

            var frmSales = new FrmSales(header, MainProgram.user, menuId);
            frmSales.Show(this.mainDock);
        }

        private void mnuLapSalesProduct_Click(object sender, EventArgs e)
        {
            var header = GetMenuTitle(sender);

            var frmReport = new FrmLapSales(string.Format("Report {0}", header), MainProgram.user, MainProgram.GeneralSupplier);
            frmReport.ShowDialog();
        }

        private void mnuSettingsApplication_Click(object sender, EventArgs e)
        {
            var header = GetMenuTitle(sender);

            var frmSettings = new FrmGeneralSupplier(header, MainProgram.GeneralSupplier, MainProgram.settingPort, MainProgram.settingCustomerDisplay);
            frmSettings.ShowDialog();
        }

        private void mnuChangeUser_Click(object sender, EventArgs e)
        {            
            if (MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                    CloseAllDocuments();

                    this.IsLogout = true;
                    this.Close();
                }
            }
        }

        private void mnuExitFromProgram_Click(object sender, EventArgs e)
        {
            if (MsgHelper.MsgConfirmation("Do you want the process to continue ?"))
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    CloseAllDocuments();
                    this.Close();
                }
            }
        }

        private void OpenUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        private void mnuFanPageSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://www.facebook.com/openretail/";
            OpenUrl(url);
        }

        private void mnuGroupSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://web.facebook.com/groups/openretail/";
            OpenUrl(url);
        }

        private void mnuInstructionsUsageSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/spark-pos/wiki/";
            OpenUrl(url);
        }

        private void mnuRegistration_Click(object sender, EventArgs e)
        {
            var url = "https://openretailblog.wordpress.com/registrasi/";
            OpenUrl(url);
        }        

        private void mnuSupportDevelopmentSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/spark-pos/wiki/Cara-Berkontribusi/";
            OpenUrl(url);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            var frmAbout = new FrmAbout();
            frmAbout.ShowDialog();
        }

        public void Ok(object sender, object data)
        {
            if (data is Profil)
            {
                MainProgram.profil = (Profil)data;
                InitializeStatusBar();
            }
        }

        public void Ok(object sender, bool isNewData, object data)
        {
            throw new NotImplementedException();
        }


       

        private void mnuCekUpdateLatest_Click(object sender, EventArgs e)
        {
            if (MainProgram.onlineUpdateUrlInfo.Length > 0)
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    AutoUpdater.Start(MainProgram.onlineUpdateUrlInfo);

                    while (!_lightSleeper.HasBeenCanceled)
                    {
                        _lightSleeper.Sleep(10000);
                    } 
                }
            }
            else
                MsgHelper.MsgWarning("Key-UpdateFailed");
        }        
    }
}
