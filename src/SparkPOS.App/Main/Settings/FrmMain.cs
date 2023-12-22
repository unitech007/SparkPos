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

using SparkPOS.App.Reference;
using SparkPOS.App.Transactions;
using SparkPOS.App.Expense;
using SparkPOS.App.Report;
using SparkPOS.App.Settings;
using SparkPOS.Helper;
using ConceptCave.WaitCursor;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using log4net;
using AutoUpdaterDotNET;
using System.Threading;

namespace SparkPOS.App.Main
{
    public partial class FrmMain : Form, IListener
    {
        //Disable close button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;

        private FrmListCard _FrmListCard;
        private FrmListCategory _frmListCategory; 
        private FrmListTax _frmListTax;
        private FrmListProductWithNavigation _frmListProduct;
        private FrmSalesperCategory _FrmSalesperCategory;

        private FrmListCustomer _frmListCustomer;
        private FrmListSupplier _frmListSupplier;
        private FrmListDropshipper _frmListDropshipper;

        private FrmListTitles _frmListTitles;
        private FrmListEmployee _frmListEmployee;

        private FrmListTypeExpense _frmListTypeExpense;

        private FrmListProductPurchaseWithNavigation _frmListProductPurchase;
        private FrmListProductPurchaseDebitPayment _FrmListProductPurchaseDebitPayment;
        private FrmReturnProductPurchase _FrmReturnProductPurchase;

        private FrmListProductSalesWithNavigation _FrmListProductSales;
        private FrmListSalesQuotationWithNavigation _FrmListSalesQuotation;
        private FrmListSalesDeliveryNotesWithNavigation _FrmListSalesDeliveryNotes;
        private FrmListPaymentCreditSalesProduct _frmListPaymentCreditSalesProduct;
        private FrmListReturnProductSales _FrmListReturnProductSales;

        private FrmListExpense _FrmListExpenseType;
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
            
           
            //mainDock?.BackColor = Color.FromArgb(255, 255, 255);

            // mainDock.BackColor = Color.FromArgb(255, 255, 255);

            Control[] dockPanelControls = this.Controls.Find("mainDock", true);

            if (dockPanelControls.Length > 0)
            {
                mainDock = (DockPanel)dockPanelControls[0];
                mainDock.BackColor = Color.FromArgb(255, 255, 255);
            }



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
                    var msg = "Update Latest Version {0} already tersedia. Saat ini Anda sedang menggunakan Version {1}\n\nApakah Anda ingin memperbarui application ini sekarang ?";

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
                MessageBox.Show("Failed to connect to the server, please try again later", "Check update Latest gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            sbNamaApplication.Text = appName.Replace("&", "&&");
        }

        private void AddEventToolbar()
        {

            // repeat for all objects
        tbCategory.Click += mnuCategory_Click;
            tbProduct.Click += mnuProduct_Click;
            tbAdjustmentStock.Click += mnuStockAdjustment_Click;
            tbSupplier.Click += mnuSupplier_Click;
            tbCustomer.Click += mnuCustomer_Click;
            tbProductPurchase.Click += mnuProductPurchase_Click;
            tbSalesProduct.Click += mnuProductSales_Click;
            tbExpenseCost.Click += mnuExpenseType_Click;
            tbSalary.Click += mnuEmployeeSalaryPayment_Click;
            tbLapProductPurchase.Click += mnuLapProductPurchase_Click;
            tbLapSalesProduct.Click += mnuLapProductPurchase_Click;
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

        private void mnuCategory_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListCategory>(sender, ref _frmListCategory);
        } 
        
        private void mnutax_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListTax>(sender, ref _frmListTax);
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

        //private void mnuProductPurchase_Click(object sender, EventArgs e)
        //{
        //    ShowForm<FrmListProductPurchaseWithNavigation>(sender, ref _frmListProductPurchase);
        //}

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

        private void mnuApplicationAccessRights_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListApplicationAccessRights>(sender, ref _FrmListApplicationAccessRights);
        }

        private void mnuDebtPaymentProductPurchase_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductPurchaseDebitPayment>(sender, ref _FrmListProductPurchaseDebitPayment);
        }

        private void mnuReturnProductPurchase_Click(object sender, EventArgs e)
        {
            ShowForm<FrmReturnProductPurchase>(sender, ref _FrmReturnProductPurchase);
        }

        private void mnuProductSales_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductSalesWithNavigation>(sender, ref _FrmListProductSales);
        }

        private void mnuPaymentCreditSalesProduct_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListPaymentCreditSalesProduct>(sender, ref _frmListPaymentCreditSalesProduct);
        }

        private void mnuProductSalesReturn_Click(object sender, EventArgs e)
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
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
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
                MsgHelper.MsgWarning("Sorry, you do not have the authority to access this menu");
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
            ShowWarningMessage(url);
        }

        private void mnuGroupSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://web.facebook.com/groups/openretail/";
            ShowWarningMessage(url);
        }

        private void mnuInstructionsUsageSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/spark-pos/wiki/";
            ShowWarningMessage(url);
        }

        private void mnuRegistration_Click(object sender, EventArgs e)
        {
            var url = "https://openretailblog.wordpress.com/registrasi/";
            ShowWarningMessage(url);
        }

        private void mnuSupportDevelopmentSparkPOS_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/spark-pos/wiki/Cara-Berkontribusi/";
            ShowWarningMessage(url);
        }

        private void ShowWarningMessage(string url)
        {
            //MessageBox.Show("The page is currently unavailable: " + url);
  
                        MsgHelper.MsgWarning("The page is currently unavailable: " + url); 

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

        private void mnuLapProductPurchaseDebtCard_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapCardDebtProductPurchase>(sender);
        }

        private void mnuLapProductPurchaseReturn_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapReturnProductPurchase>(sender);
        }

        //private void mnuProductSales_Click(object sender, EventArgs e)
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

        private void mnuLapProductSalesCreditCard_click(object sender, EventArgs e)
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

        private void mnuExpenseType_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListExpense>(sender, ref _FrmListExpenseType);
        }

        private void mnuLapExpense_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapExpenseCost>(sender);
        }

        private void mnuloan_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListloan>(sender, ref _FrmListloan);
        }

        private void mnuLaploan_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapLoan>(sender);
        }

        private void mnuEmployeeSalaryPayment_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListEmployeeSalaryPayment>(sender, ref _FrmListEmployeeSalaryPayment);
        }

        private void mnuLapEmployeeSalaryPayment_Click(object sender, EventArgs e)
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
                MsgHelper.MsgWarning("Sorry, the Online Update link/URL has not been set yet!!!\nLatest update check process has failed.");
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
            ShowFormDialog<FrmLapBestSellingProducts>(sender);
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

        private void mnuLapProductStockCard_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductStockCard>(sender);
        }

        private void mnuPrintingLabelBarcodeProduct_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmPrintingLabelBarcodeProduct>(sender);
        }

        private void mnuLapSalesperCategory_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapSalesPerCategory>(sender);
        }

        private void mnuPrintingLabelpriceProduct_Click(object sender, EventArgs e)
        {
            ShowFormDialog<FrmPrintLabelPriceProduct>(sender);
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

        private void tbSalesProduct_Click(object sender, EventArgs e)
        {

        }

        private void mnuProductPurchase_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductPurchaseWithNavigation>(sender, ref _frmListProductPurchase);

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListProductPurchaseWithNavigation>(sender, ref _frmListProductPurchase);
        }

        //private void salesProductToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ShowForm<FrmListProductSalesWithNavigation>(sender, ref _FrmListProductSales);

        //}


        
        private void mnuProductSales_Click_1(object sender, EventArgs e)
        {
            ShowForm<FrmListProductSalesWithNavigation>(sender, ref _FrmListProductSales);
        }
          private void mnuSalesQuotation_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListSalesQuotationWithNavigation>(sender, ref _FrmListSalesQuotation);
        }  
        private void deliveryNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FrmListSalesDeliveryNotesWithNavigation>(sender, ref _FrmListSalesDeliveryNotes);
        }
       // deliveryNotesToolStripMenuItem_Click

        private void mnuLapProductSales_Click_1(object sender, EventArgs e)
        {
            ShowFormDialog<FrmLapProductSales>(sender);
        }

        private void mainDock_ActiveContentChanged(object sender, EventArgs e)
        {

        }

        private void deliveryNotesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ShowForm<FrmListSalesDeliveryNotesWithNavigation>(sender, ref _FrmListSalesDeliveryNotes);
        }

        private void englishToArabicToolStripMenuItem_Click(object sender, EventArgs e)
        {
         //   MainProgram.currentLanguage = "ar-SA";
            if (MessageBox.Show( "Do you want to Continue with Arabic ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

            {
                //do something if YES
                MainProgram.currentLanguage = "ar-SA";
            }

            else

            {
                MainProgram.currentLanguage = "en-US";

            }
        }

        private void arabicToEnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // MainProgram.currentLanguage = "en-US";
            if (MessageBox.Show( "Do you want to Continue with English ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

            {
                //do something if YES
                MainProgram.currentLanguage = "en-US";
            }

            else

            {
                //do something if no
                MainProgram.currentLanguage = "ar-SA";

            }
        }

        private void arabicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to Continue with Arabic?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Set the current language to Arabic
                MainProgram.currentLanguage = "ar-SA";

                // Call the GlobalLanguageChange method to update the UI language
              //  MainProgram.GlobalLanguageChange(this);
            }
            else
            {
                // Set the current language to English
                MainProgram.currentLanguage = "en-US";

                // Call the GlobalLanguageChange method to update the UI language
                //MainProgram.GlobalLanguageChange(this);
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to Continue with English?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Set the current language to English
                MainProgram.currentLanguage = "en-US";

                // Call the GlobalLanguageChange method to update the UI language
               // MainProgram.GlobalLanguageChange(this);
            }
            else
            {
                // Set the current language to Arabic
                MainProgram.currentLanguage = "ar-SA";

                // Call the GlobalLanguageChange method to update the UI language
                //MainProgram.GlobalLanguageChange(this);
            }
        }

    }
}


        //private void mnuProductPurchase_Click(object sender, EventArgs e)
        //{
        //    ShowForm<FrmListProductPurchaseWithNavigation>(sender, ref _frmListProductPurchase);
        //}
    

