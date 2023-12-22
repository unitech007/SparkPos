namespace SparkPOS.App.Cashier.Main
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.statusStripEx1 = new Syncfusion.Windows.Forms.Tools.StatusStripEx();
            this.sbJam = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.statusStripLabel2 = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.sbDate = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.statusStripLabel4 = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.sbOperator = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.statusStripLabel6 = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.sbNameApplication = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuTransactions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProductSales = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLapSalesProduct = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBlogSparkPOS = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFanPageSparkPOS = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGroupSparkPOS = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuInstructionsUsageSparkPOS = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRegistration = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSupportDevelopmentSparkPOS = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOnlineUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCekUpdateLatest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChangeUser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExitFromProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mainDock = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbSalesProduct = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tbLapSalesProduct = new System.Windows.Forms.ToolStripButton();
            this.statusStripEx1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripEx1
            // 
            this.statusStripEx1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStripEx1.BeforeTouchSize = new System.Drawing.Size(827, 22);
            this.statusStripEx1.Dock = Syncfusion.Windows.Forms.Tools.DockStyleEx.Bottom;
            this.statusStripEx1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbJam,
            this.statusStripLabel2,
            this.sbDate,
            this.statusStripLabel4,
            this.sbOperator,
            this.statusStripLabel6,
            this.sbNameApplication});
            this.statusStripEx1.Location = new System.Drawing.Point(0, 388);
            this.statusStripEx1.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(206)))), ((int)(((byte)(255)))));
            this.statusStripEx1.Name = "statusStripEx1";
            this.statusStripEx1.OfficeColorScheme = Syncfusion.Windows.Forms.Tools.ToolStripEx.ColorScheme.Silver;
            this.statusStripEx1.Size = new System.Drawing.Size(827, 22);
            this.statusStripEx1.TabIndex = 3;
            this.statusStripEx1.Text = "statusStripEx1";
            // 
            // sbJam
            // 
            this.sbJam.Image = global::SparkPOS.App.Cashier.Properties.Resources.clock32;
            this.sbJam.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.sbJam.Name = "sbJam";
            this.sbJam.Size = new System.Drawing.Size(65, 16);
            this.sbJam.Text = "00:00:00";
            // 
            // statusStripLabel2
            // 
            this.statusStripLabel2.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.statusStripLabel2.Name = "statusStripLabel2";
            this.statusStripLabel2.Size = new System.Drawing.Size(10, 15);
            this.statusStripLabel2.Text = "|";
            // 
            // sbDate
            // 
            this.sbDate.Image = global::SparkPOS.App.Cashier.Properties.Resources.calendar32;
            this.sbDate.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.sbDate.Name = "sbDate";
            this.sbDate.Size = new System.Drawing.Size(60, 16);
            this.sbDate.Text = "Day, ...";
            // 
            // statusStripLabel4
            // 
            this.statusStripLabel4.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.statusStripLabel4.Name = "statusStripLabel4";
            this.statusStripLabel4.Size = new System.Drawing.Size(10, 15);
            this.statusStripLabel4.Text = "|";
            // 
            // sbOperator
            // 
            this.sbOperator.Image = global::SparkPOS.App.Cashier.Properties.Resources.user32;
            this.sbOperator.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.sbOperator.Name = "sbOperator";
            this.sbOperator.Size = new System.Drawing.Size(77, 16);
            this.sbOperator.Text = "operator...";
            // 
            // statusStripLabel6
            // 
            this.statusStripLabel6.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.statusStripLabel6.Name = "statusStripLabel6";
            this.statusStripLabel6.Size = new System.Drawing.Size(10, 15);
            this.statusStripLabel6.Text = "|";
            // 
            // sbNameApplication
            // 
            this.sbNameApplication.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.sbNameApplication.Name = "sbNameApplication";
            this.sbNameApplication.Size = new System.Drawing.Size(53, 15);
            this.sbNameApplication.Text = "sistem ...";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTransactions,
            this.mnuReport,
            this.mnuSettings,
            this.mnuHelp,
            this.mnuOnlineUpdate,
            this.mnuExit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(827, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuTransactions
            // 
            this.mnuTransactions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProductSales});
            this.mnuTransactions.Name = "mnuTransactions";
            this.mnuTransactions.Size = new System.Drawing.Size(67, 20);
            this.mnuTransactions.Text = "Transations";
            // 
            // mnuProductSales
            // 
            this.mnuProductSales.Name = "mnuProductSales";
            this.mnuProductSales.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.mnuProductSales.Size = new System.Drawing.Size(205, 22);
            this.mnuProductSales.Tag = "FrmListProductSales";
            this.mnuProductSales.Text = "Sales Product";
            this.mnuProductSales.Click += new System.EventHandler(this.mnuProductSales_Click);
            // 
            // mnuReport
            // 
            this.mnuReport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProductSales});
            this.mnuReport.Name = "mnuReport";
            this.mnuReport.Size = new System.Drawing.Size(62, 20);
            this.mnuReport.Text = "Report";
            // 
            // mnuProductSales
            // 
            this.mnuLapSalesProduct.Name = "mnuLapSalesProduct";
            this.mnuLapSalesProduct.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.mnuLapSalesProduct.Size = new System.Drawing.Size(214, 22);
            this.mnuLapSalesProduct.Tag = "FrmLapProductSales";
            this.mnuLapSalesProduct.Text = "Sales Per Cashier";
            this.mnuLapSalesProduct.Click += new System.EventHandler(this.mnuLapSalesProduct_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSettingsApplication});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(80, 20);
            this.mnuSettings.Text = "Settings";
            // 
            // mnuSettingsApplication
            // 
            this.mnuSettingsApplication.Name = "mnuSettingsApplication";
            this.mnuSettingsApplication.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuSettingsApplication.Size = new System.Drawing.Size(220, 22);
            this.mnuSettingsApplication.Tag = "FrmGeneralSupplier";
            this.mnuSettingsApplication.Text = "Settings Application";
            this.mnuSettingsApplication.Click += new System.EventHandler(this.mnuSettingsApplication_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBlogSparkPOS,
            this.mnuFanPageSparkPOS,
            this.mnuGroupSparkPOS,
            this.toolStripSeparator16,
            this.mnuInstructionsUsageSparkPOS,
            this.toolStripSeparator14,
            this.mnuRegistration,
            this.mnuSupportDevelopmentSparkPOS,
            this.toolStripSeparator15,
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(63, 20);
            this.mnuHelp.Text = "Help";
            // 
            // mnuBlogSparkPOS
            // 
            this.mnuBlogSparkPOS.Name = "mnuBlogSparkPOS";
            this.mnuBlogSparkPOS.Size = new System.Drawing.Size(264, 22);
            this.mnuBlogSparkPOS.Text = "Blog SparkPOS";
            this.mnuBlogSparkPOS.Visible = false;
            // 
            // mnuFanPageSparkPOS
            // 
            this.mnuFanPageSparkPOS.Name = "mnuFanPageSparkPOS";
            this.mnuFanPageSparkPOS.Size = new System.Drawing.Size(264, 22);
            this.mnuFanPageSparkPOS.Text = "Fan Page SparkPOS";
            this.mnuFanPageSparkPOS.Click += new System.EventHandler(this.mnuFanPageSparkPOS_Click);
            // 
            // mnuGroupSparkPOS
            // 
            this.mnuGroupSparkPOS.Name = "mnuGroupSparkPOS";
            this.mnuGroupSparkPOS.Size = new System.Drawing.Size(264, 22);
            this.mnuGroupSparkPOS.Text = "Group SparkPOS";
            this.mnuGroupSparkPOS.Click += new System.EventHandler(this.mnuGroupSparkPOS_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(261, 6);
            // 
            // mnuInstructionsUsageSparkPOS
            // 
            this.mnuInstructionsUsageSparkPOS.Name = "mnuInstructionsUsageSparkPOS";
            this.mnuInstructionsUsageSparkPOS.Size = new System.Drawing.Size(264, 22);
            this.mnuInstructionsUsageSparkPOS.Text = "Instructions Usage SparkPOS";
            this.mnuInstructionsUsageSparkPOS.Click += new System.EventHandler(this.mnuInstructionsUsageSparkPOS_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(261, 6);
            // 
            // mnuRegistration
            // 
            this.mnuRegistration.Name = "mnuRegistration";
            this.mnuRegistration.Size = new System.Drawing.Size(264, 22);
            this.mnuRegistration.Text = "Registration";
            this.mnuRegistration.Click += new System.EventHandler(this.mnuRegistration_Click);
            // 
            // mnuSupportDevelopmentSparkPOS
            // 
            this.mnuSupportDevelopmentSparkPOS.Name = "mnuSupportDevelopmentSparkPOS";
            this.mnuSupportDevelopmentSparkPOS.Size = new System.Drawing.Size(264, 22);
            this.mnuSupportDevelopmentSparkPOS.Text = "SupportDevelopmentSparkPOS";
            this.mnuSupportDevelopmentSparkPOS.Click += new System.EventHandler(this.mnuSupportDevelopmentSparkPOS_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(261, 6);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(264, 22);
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // mnuOnlineUpdate
            // 
            this.mnuOnlineUpdate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCekUpdateLatest});
            this.mnuOnlineUpdate.Name = "mnuOnlineUpdate";
            this.mnuOnlineUpdate.Size = new System.Drawing.Size(95, 20);
            this.mnuOnlineUpdate.Text = "Online Update";
            // 
            // mnuCekUpdateLatest
            // 
            this.mnuCekUpdateLatest.Name = "mnuCekUpdateLatest";
            this.mnuCekUpdateLatest.Size = new System.Drawing.Size(178, 22);
            this.mnuCekUpdateLatest.Text = "Check Update Latest";
            this.mnuCekUpdateLatest.Click += new System.EventHandler(this.mnuCekUpdateLatest_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuChangeUser,
            this.mnuExitFromProgram});
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(52, 20);
            this.mnuExit.Text = "Exit";
            // 
            // mnuChangeUser
            // 
            this.mnuChangeUser.Name = "mnuChangeUser";
            this.mnuChangeUser.Size = new System.Drawing.Size(174, 22);
            this.mnuChangeUser.Text = "Change User";
            this.mnuChangeUser.Click += new System.EventHandler(this.mnuChangeUser_Click);
            // 
            // mnuExitFromProgram
            // 
            this.mnuExitFromProgram.Name = "mnuExitFromProgram";
            this.mnuExitFromProgram.Size = new System.Drawing.Size(174, 22);
            this.mnuExitFromProgram.Text = "Exit dari Application";
            this.mnuExitFromProgram.Click += new System.EventHandler(this.mnuExitFromProgram_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mainDock
            // 
            this.mainDock.AllowEndUserDocking = false;
            this.mainDock.AllowEndUserNestedDocking = false;
            this.mainDock.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mainDock.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mainDock.BackgroundImage")));
            this.mainDock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainDock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainDock.Location = new System.Drawing.Point(0, 24);
            this.mainDock.Name = "mainDock";
            this.mainDock.Size = new System.Drawing.Size(827, 364);
            this.mainDock.TabIndex = 14;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbSalesProduct,
            this.toolStripSeparator12,
            this.tbLapSalesProduct});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(827, 39);
            this.toolStrip1.TabIndex = 19;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // tbSalesProduct
            // 
            this.tbSalesProduct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSalesProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSalesProduct.Name = "tbSalesProduct";
            this.tbSalesProduct.Size = new System.Drawing.Size(23, 36);
            this.tbSalesProduct.Tag = "FrmListProductSales";
            this.tbSalesProduct.Text = "Sales Product";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 39);
            // 
            // tbLapSalesProduct
            // 
            this.tbLapSalesProduct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbLapSalesProduct.Image = ((System.Drawing.Image)(resources.GetObject("tbLapSalesProduct.Image")));
            this.tbLapSalesProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbLapSalesProduct.Name = "tbLapSalesProduct";
            this.tbLapSalesProduct.Size = new System.Drawing.Size(36, 36);
            this.tbLapSalesProduct.Tag = "FrmLapProductSales";
            this.tbLapSalesProduct.Text = "Report Sales Product";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 410);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.mainDock);
            this.Controls.Add(this.statusStripEx1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStripEx1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.StatusStripEx statusStripEx1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuTransactions;
        private System.Windows.Forms.ToolStripMenuItem mnuReport;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuProductSales;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel sbJam;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel statusStripLabel2;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel sbDate;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel statusStripLabel4;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel sbOperator;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel statusStripLabel6;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel sbNameApplication;
        private System.Windows.Forms.ToolStripMenuItem mnuLapSalesProduct;
        private System.Windows.Forms.ToolStripMenuItem mnuChangeUser;
        private System.Windows.Forms.ToolStripMenuItem mnuExitFromProgram;
        private System.Windows.Forms.Timer timer1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel mainDock;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbSalesProduct;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton tbLapSalesProduct;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuRegistration;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsApplication;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem mnuBlogSparkPOS;
        private System.Windows.Forms.ToolStripMenuItem mnuSupportDevelopmentSparkPOS;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem mnuOnlineUpdate;
        private System.Windows.Forms.ToolStripMenuItem mnuCekUpdateLatest;
        private System.Windows.Forms.ToolStripMenuItem mnuInstructionsUsageSparkPOS;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem mnuFanPageSparkPOS;
        private System.Windows.Forms.ToolStripMenuItem mnuGroupSparkPOS;
    }
}