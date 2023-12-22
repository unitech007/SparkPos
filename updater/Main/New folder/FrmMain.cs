/**
 * Copyright (C) 2017 Kamarudin (http://coding4ever.net/)
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
 * The latest version of this file can be found at https://github.com/rudi-krsoftware/open-retail
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

using OpenRetail.App.Reference;
using OpenRetail.App.Transactions;
using OpenRetail.App.Expense;
using OpenRetail.App.Report;
using OpenRetail.App.Settings;
using OpenRetail.Helper;
using ConceptCave.WaitCursor;
using OpenRetail.Model;
using OpenRetail.Bll.Api;
using OpenRetail.Bll.Service;
using log4net;
using AutoUpdaterDotNET;
using System.Threading;

namespace OpenRetail.App.Main
{
    public partial class FrmMain : Form, IListener
    {
        //Disable close button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;

        private FrmListCard _FrmListCard;
        private FrmListCategory _frmListCategory;
        private FrmListProductWithNavigation _frmListProduct;
        private FrmSalesperCategory _FrmSalesperCategory;

        private FrmListCustomer _frmListCustomer;
        private FrmListSupplier _frmListSupplier;
        private FrmListDropshipper _frmListDropshipper;

        private FrmListTitles _frmListTitles;
        private FrmListEmployee _frmListEmployee;

        private FrmListTypeExpense _frmListTypeExpense;

        private FrmListProductPurchaseWithNavigation _frmListProductPurchase;
        private FrmListProductPurchaseDebtPayment _FrmListProductPurchaseDebtPayment;
        private FrmListProductProductReturn _FrmListProductProductReturn;

        private FrmListSalesProductWithNavigation _frmListSalesProduct;
        private FrmListPaymentCreditSalesProduct _frmListPaymentCreditSalesProduct;
        private FrmListReturnProductSales _FrmListReturnProductSales;

        private FrmListExpenseType _FrmListExpenseType;
        private FrmListloan _FrmListloan;
        private FrmListEmployeeSalaryPayment _FrmListEmployeeSalaryPayment;

        private FrmListApplicationAccessRights _FrmListApplicationAccessRights;
        private FrmListOperator _frmListOperator;

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
            InitializeComponent();
            mainDock.BackColor = Color.FromArgb(255, 255, 255);

            _log = MainProgram.log;

            AddEventToolbar();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitializeStatusBar();
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
                    var msg = "Update Latest versi {0} sudah tersedia. Saat ini Anda sedang menggunakan Versi {1}\n\nApakah Anda ingin memperbarui application ini sekarang ?";

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
                    MessageBox.Show("Tidak ada update yang tersedia, silahkan dicoba lagi nanti.", "Update belum tersedia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Gagal melakukan koneksi ke server, silahkan dicoba lagi nanti.", "Check update Latest gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Method untuk menonaktifkan menu dan toolbar yang belum aktif (membaca setting tabel m_menu)
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
            sbTanggal.Text = string.Format("{0}, {1}", DayMonthHelper.GetHariIndonesia(dt), dt.Day + " " + DayMonthHelper.GetBulanIndonesia(dt.Month) + " " + dt.Year);

            if (MainProgram.user != null)
                sbOperator.Text = string.Format("Operator : {0}", MainProgram.user.name_user);

            var firstReleaseYear = 2017;
            var currentYear = DateTime.Today.Year;
            var copyright = currentYear > firstReleaseYear ? string.Format("{0} - {1}", firstReleaseYear, currentYear) : firstReleaseYear.ToString();

            var appName = string.Format(MainProgram.appName, MainProgram.currentVersion, MainProgram.stageOfDevelopment, copyright);

            this.Text = appName;
            sbNamaApplication.Text = appName.Replace("&", "&&");
        }

        private void AddEventToolbar()
        {
            tbCategory.Click += mnuCategory_Click;
            tbProduct.Click += mnuProduct_Click;
            tbAdjustmentStock.Click += mnuStockAdjustment_Click;
            tbSupplier.Click += mnuSupplier_Click;
            tbCustomer.Click += mnuCustomer_Click;
            tbProductPurchase.Click += mnuProductPurchase_Click;
            tbSalesProduct.Click += mnuSalesProduct_Click;
            tbExpenseCost.Click += mnuExpenseCost_Click;
            tbSalary.Click += mnuSalary_Click;
            tbLapProductPurchase.Click += mnuLapProductPurchase_Click;
            tbLapSalesProduct.Click += mnuSalesProduct_Click;
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
                MsgHelper.MsgWarning("Maaf Anda tidak mempunyai otoritas untuk mengakses menu ini");
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

        private void mnuCategory_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListCategory>(sender, ref _frmListCategory);
        }        

        private void mnuProduct_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductWithNavigation>(sender, ref _frmListProduct);
        }

        private void mnuSupplier_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListSupplier>(sender, ref _frmListSupplier);
        }

        private void mnuCustomer_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListCustomer>(sender, ref _frmListCustomer);
        }

        private void mnuTitles_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListTitles>(sender, ref _frmListTitles);
        }        

        private void mnuProductPurchase_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductPurchaseWithNavigation>(sender, ref _frmListProductPurchase);
        }

        private void mnuTypeExpense_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListTypeExpense>(sender, ref _frmListTypeExpense);
        }

        private void mnuEmployee_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListEmployee>(sender, ref _frmListEmployee);
        }

        private void mnuStockAdjustment_Click(object sender, EventArgs e)
        {
            ShowForm<FrmSalesperCategory>(sender, ref _FrmSalesperCategory);
        }

        private void mnuManajementOperator_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListOperator>(sender, ref _frmListOperator);
        }

        private void mnuRightAccessApplication_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListApplicationAccessRights>(sender, ref _FrmListApplicationAccessRights);
        }

        private void mnuDebtPaymentProductPurchase_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductPurchaseDebtPayment>(sender, ref _FrmListProductPurchaseDebtPayment);
        }

        private void mnuReturnProductPurchase_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductProductReturn>(sender, ref _FrmListProductProductReturn);
        }

        private void mnuSalesProduct_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListSalesProductWithNavigation>(sender, ref _frmListSalesProduct);
        }

        private void mnuPaymentCreditSalesProduct_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListPaymentCreditSalesProduct>(sender, ref _frmListPaymentCreditSalesProduct);
        }

        private void mnuReturnSalesProduct_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListReturnProductSales>(sender, ref _FrmListReturnProductSales);
        }

        private void mnuCompanyProfile_Click(object sender, EventArgs e)
        {
            var header = GetMenuTitle(sender);
            var menuName = GetMenuName(sender);

            if (RolePrivilegeHelper.IsHaveRightAccess(menuName, MainProgram.user, GrantState.UPDATE))
            {
                var frmProfil = new FrmCompanyProfile(header, MainProgram.profil);
                frmProfil.Listener = this;
                frmProfil.ShowDialog();
            }
            else
                MsgHelper.MsgWarning("Maaf Anda tidak mempunyai otoritas untuk mengakses menu ini");
        }

        private void mnuGeneralSupplier_Click(object sender, EventArgs e)
        {
            var header = GetMenuTitle(sender);
            var menuName = GetMenuName(sender);

            if (RolePrivilegeHelper.IsHaveRightAccess(menuName, MainProgram.user, GrantState.UPDATE))
            {
                var frmSettings = new FrmGeneralSupplier(header, MainProgram.GeneralSupplier, MainProgram.settingPort, MainProgram.settingCustomerDisplay);
                frmSettings.ShowDialog();
            }
            else
                MsgHelper.MsgWarning("Maaf Anda tidak mempunyai otoritas untuk mengakses menu ini");
        }

        private void mnuChangeUser_Click(object sender, EventArgs e)
        {            
            if (MsgHelper.MsgKonfirmasi("Apakah proses ingin dilanjutkan ?"))
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
            if (MsgHelper.MsgKonfirmasi("Apakah proses ingin dilanjutkan ?"))
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

        private void mnuFanPageOpenRetail_Click(object sender, EventArgs e)
        {
            var url = "https://www.facebook.com/openretail/";
            OpenUrl(url);
        }

        private void mnuGroupOpenRetail_Click(object sender, EventArgs e)
        {
            var url = "https://web.facebook.com/groups/openretail/";
            OpenUrl(url);
        }

        private void mnuInstructionsUsageOpenRetail_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/open-retail/wiki/";
            OpenUrl(url);
        }

        private void mnuRegistration_Click(object sender, EventArgs e)
        {
            var url = "https://openretailblog.wordpress.com/registrasi/";
            OpenUrl(url);
        }        

        private void mnuSupportDevelopmentOpenRetail_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/open-retail/wiki/Cara-Berkontribusi/";
            OpenUrl(url);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            var frmAbout = new FrmAbout();
            frmAbout.ShowDialog();
        }

        private void mnuLapProductPurchase_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductPurchase>(sender);
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

        private void mnuLapProductPurchaseDebt_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductPurchaseDebt>(sender);
        }

        private void mnuLapProductPurchaseDebtPayment_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapDebtPaymentProductPurchase>(sender);
        }

        private void mnuLapCardDebtProductPurchase_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapCardDebtProductPurchase>(sender);
        }

        private void mnuLapReturnProductPurchase_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapReturnProductPurchase>(sender);
        }

        //private void mnuSalesProduct_Click(object sender, EventArgs e)
        //{
        //    ShowFormDialog<FrmLapProductSales>(sender);
        //}

        private void mnuLapSalesperProduct_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapSalesPerProduct>(sender);
        }

        private void mnuLapProductSalesCredit_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductSalesCredit>(sender);
        }

        private void mnuLapProductSalesCreditPayment_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductSalesCreditPayment>(sender);
        }

        private void mnuLapCardCreditSalesProduct_click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductSalesCreditCard>(sender);
        }

        private void mnuLapProductSalesReturn_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapReturnProductSales>(sender);
        }

        private void mnuLapProductStock_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmProductStock>(sender);
        }

        private void mnuLapAdjustmentStock_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmStockAdjustment>(sender);
        }

        private void mnuExpenseCost_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListExpenseType>(sender, ref _FrmListExpenseType);
        }

        private void mnuLapExpenseCost_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapExpenseCost>(sender);
        }

        private void mnuLoan_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListloan>(sender, ref _FrmListloan);
        }

        private void mnuLapLoan_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapLoan>(sender);
        }

        private void mnuSalary_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListEmployeeSalaryPayment>(sender, ref _FrmListEmployeeSalaryPayment);
        }

        private void mnuLapSalary_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapSalaryEmployee>(sender);
        }

        private void mnuCheckUpdateLatest_Click(object sender, EventArgs e)
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
                MsgHelper.MsgWarning("Maaf link/url Online Update belum diset !!!\nProses cek update Latest batal.");
        }

        private void mnuCard_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListCard>(sender, ref _FrmListCard);
        }

        private void mnuIncomeandExpense_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmIncomeandExpense>(sender);
        }

        private void mnuLapBestSellingProducts_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmBestSellingProducts>(sender);
        }

        private void mnuLapSalesPerCashier_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmSalesPerCashier>(sender);
        }

        private void mnuDropshipper_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListDropshipper>(sender, ref _frmListDropshipper);
        }

        private void mnuLapCustomerProduct_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapCustomerProduct>(sender);
        }

        private void mnuProductStockCard_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmProductStockCard>(sender);
        }

        private void mnuPrintingLabelBarcodeProduct_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmPrintingLabelBarcodeProduct>(sender);
        }

        private void mnuSalesperCategory_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapSalesPerCategory>(sender);
        }

        private void mnuCetakLabelHargaProduct_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmCetakLabelHargaProduct>(sender);
        }

        private void mnuLapSalesProfitLoss_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapSalesProfitLoss>(sender);
        }

        private void mnuReference_Click(object sender, EventArgs e)
        {

        }

        private void FrmMain_Load_1(object sender, EventArgs e)
        {

        }
    }
}
