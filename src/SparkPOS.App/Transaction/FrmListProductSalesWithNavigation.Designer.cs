namespace SparkPOS.App.Transactions
{
    partial class FrmListProductSalesWithNavigation
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
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.gridList = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.filterRangeDate = new SparkPOS.Helper.UserControl.FilterRangeDate();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtNameCustomer = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(1735, 4);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.gridList, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(789, 332);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // gridList
            // 
            this.gridList.BackColor = System.Drawing.SystemColors.Control;
            this.gridList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridList.ItemHeight = 17;
            this.gridList.Location = new System.Drawing.Point(3, 32);
            this.gridList.Name = "gridList";
            this.gridList.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridList.SelectedIndex = -1;
            this.gridList.Size = new System.Drawing.Size(783, 297);
            this.gridList.TabIndex = 1;
            this.gridList.TopIndex = 0;
            this.gridList.Click += new System.EventHandler(this.gridList_Click);
            this.gridList.DoubleClick += new System.EventHandler(this.gridList_DoubleClick);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.7237F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.2763F));
            this.tableLayoutPanel6.Controls.Add(this.filterRangeDate, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(789, 29);
            this.tableLayoutPanel6.TabIndex = 3;
            // 
            // filterRangeDate
            // 
            this.filterRangeDate.Location = new System.Drawing.Point(3, 3);
            this.filterRangeDate.Name = "filterRangeDate";
            this.filterRangeDate.Size = new System.Drawing.Size(469, 23);
            this.filterRangeDate.TabIndex = 2;
            this.filterRangeDate.BtnShowClicked += new SparkPOS.Helper.UserControl.FilterRangeDate.EventHandler(this.filterRangeDate_BtnShowClicked);
            this.filterRangeDate.ChkShowAllDataClicked += new SparkPOS.Helper.UserControl.FilterRangeDate.EventHandler(this.filterRangeDate_ChkShowAllDataClicked);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.btnFind);
            this.flowLayoutPanel2.Controls.Add(this.txtNameCustomer);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(487, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(302, 29);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // btnFind
            // 
            this.btnFind.Image = global::SparkPOS.App.Properties.Resources.search16;
            this.btnFind.Location = new System.Drawing.Point(262, 3);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(37, 23);
            this.btnFind.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnFind, "Find name customer");
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtNameCustomer
            // 
            this.txtNameCustomer.AutoEnter = false;
            this.txtNameCustomer.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNameCustomer.EnterFocusColor = System.Drawing.Color.White;
            this.txtNameCustomer.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNameCustomer.LetterOnly = false;
            this.txtNameCustomer.Location = new System.Drawing.Point(38, 3);
            this.txtNameCustomer.Name = "txtNameCustomer";
            this.txtNameCustomer.NumericOnly = false;
            this.txtNameCustomer.SelectionText = false;
            this.txtNameCustomer.Size = new System.Drawing.Size(218, 20);
            this.txtNameCustomer.TabIndex = 0;
            this.txtNameCustomer.ThousandSeparator = false;
            this.txtNameCustomer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNameCustomer_KeyPress);
            // 
            // FrmListProductSalesWithNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 449);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Name = "FrmListProductSalesWithNavigation";
            this.Text = "FrmListProductSalesWithNavigation";
            this.Controls.SetChildIndex(this.tableLayoutPanel5, 0);
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private Syncfusion.Windows.Forms.Grid.GridListControl gridList;
        private SparkPOS.Helper.UserControl.FilterRangeDate filterRangeDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnFind;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtNameCustomer;
    }
}