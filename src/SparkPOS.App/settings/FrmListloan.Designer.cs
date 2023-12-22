namespace SparkPOS.App.Expense
{
    partial class FrmListloan
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.gridList = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.gridListPaymentHistory = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.pnlFooter2 = new System.Windows.Forms.Panel();
            this.btnHapusPayment = new System.Windows.Forms.Button();
            this.btnPerbaikiPayment = new System.Windows.Forms.Button();
            this.btnTambahPayment = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.filterRangeDate = new SparkPOS.Helper.UserControl.FilterRangeDate();
            this.chkShowYangBelumLunas = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridListPaymentHistory)).BeginInit();
            this.pnlFooter2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(789, 367);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel4.Controls.Add(this.gridList, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.groupBox1, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 32);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(783, 332);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // gridList
            // 
            this.gridList.BackColor = System.Drawing.SystemColors.Control;
            this.gridList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridList.ItemHeight = 17;
            this.gridList.Location = new System.Drawing.Point(3, 3);
            this.gridList.MultiColumn = false;
            this.gridList.Name = "gridList";
            this.gridList.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridList.Properties.ForceImmediateRepaint = false;
            this.gridList.Properties.MarkColHeader = false;
            this.gridList.Properties.MarkRowHeader = false;
            this.gridList.SelectedIndex = -1;
            this.gridList.Size = new System.Drawing.Size(502, 326);
            this.gridList.TabIndex = 1;
            this.gridList.TopIndex = 0;
            this.gridList.DoubleClick += new System.EventHandler(this.gridList_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(511, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 326);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " [ History Payment ] ";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.gridListPaymentHistory, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.pnlFooter2, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(263, 307);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // gridListPaymentHistory
            // 
            this.gridListPaymentHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gridListPaymentHistory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridListPaymentHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridListPaymentHistory.ItemHeight = 17;
            this.gridListPaymentHistory.Location = new System.Drawing.Point(3, 3);
            this.gridListPaymentHistory.MultiColumn = false;
            this.gridListPaymentHistory.Name = "gridListPaymentHistory";
            this.gridListPaymentHistory.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridListPaymentHistory.Properties.ForceImmediateRepaint = false;
            this.gridListPaymentHistory.Properties.MarkColHeader = false;
            this.gridListPaymentHistory.Properties.MarkRowHeader = false;
            this.gridListPaymentHistory.SelectedIndex = -1;
            this.gridListPaymentHistory.Size = new System.Drawing.Size(257, 261);
            this.gridListPaymentHistory.TabIndex = 0;
            this.gridListPaymentHistory.TopIndex = 0;
            // 
            // pnlFooter2
            // 
            this.pnlFooter2.Controls.Add(this.btnHapusPayment);
            this.pnlFooter2.Controls.Add(this.btnPerbaikiPayment);
            this.pnlFooter2.Controls.Add(this.btnTambahPayment);
            this.pnlFooter2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFooter2.Location = new System.Drawing.Point(3, 267);
            this.pnlFooter2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.pnlFooter2.Name = "pnlFooter2";
            this.pnlFooter2.Size = new System.Drawing.Size(257, 40);
            this.pnlFooter2.TabIndex = 1;
            // 
            // btnHapusPayment
            // 
            this.btnHapusPayment.Enabled = false;
            this.btnHapusPayment.Location = new System.Drawing.Point(171, 8);
            this.btnHapusPayment.Name = "btnHapusPayment";
            this.btnHapusPayment.Size = new System.Drawing.Size(75, 23);
            this.btnHapusPayment.TabIndex = 0;
            this.btnHapusPayment.Text = "Delete";
            this.btnHapusPayment.UseVisualStyleBackColor = true;
            this.btnHapusPayment.Click += new System.EventHandler(this.btnHapusPayment_Click);
            // 
            // btnPerbaikiPayment
            // 
            this.btnPerbaikiPayment.Enabled = false;
            this.btnPerbaikiPayment.Location = new System.Drawing.Point(90, 8);
            this.btnPerbaikiPayment.Name = "btnPerbaikiPayment";
            this.btnPerbaikiPayment.Size = new System.Drawing.Size(75, 23);
            this.btnPerbaikiPayment.TabIndex = 0;
            this.btnPerbaikiPayment.Text = "Edit";
            this.btnPerbaikiPayment.UseVisualStyleBackColor = true;
            this.btnPerbaikiPayment.Click += new System.EventHandler(this.btnPerbaikiPayment_Click);
            // 
            // btnTambahPayment
            // 
            this.btnTambahPayment.Location = new System.Drawing.Point(9, 8);
            this.btnTambahPayment.Name = "btnTambahPayment";
            this.btnTambahPayment.Size = new System.Drawing.Size(75, 23);
            this.btnTambahPayment.TabIndex = 0;
            this.btnTambahPayment.Text = "Add";
            this.btnTambahPayment.UseVisualStyleBackColor = true;
            this.btnTambahPayment.Click += new System.EventHandler(this.btnTambahPayment_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.filterRangeDate);
            this.flowLayoutPanel1.Controls.Add(this.chkShowYangBelumLunas);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(789, 29);
            this.flowLayoutPanel1.TabIndex = 4;
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
            // chkShowYangBelumLunas
            // 
            this.chkShowYangBelumLunas.AutoSize = true;
            this.chkShowYangBelumLunas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowYangBelumLunas.Location = new System.Drawing.Point(478, 6);
            this.chkShowYangBelumLunas.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.chkShowYangBelumLunas.Name = "chkShowYangBelumLunas";
            this.chkShowYangBelumLunas.Size = new System.Drawing.Size(160, 20);
            this.chkShowYangBelumLunas.TabIndex = 3;
            this.chkShowYangBelumLunas.Text = "shows that have not been paid in full";
            this.chkShowYangBelumLunas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkShowYangBelumLunas.UseVisualStyleBackColor = true;
            this.chkShowYangBelumLunas.CheckedChanged += new System.EventHandler(this.chkShowYangBelumLunas_CheckedChanged);
            // 
            // FrmListloan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 449);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmListloan";
            this.Text = "FrmListloan";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridListPaymentHistory)).EndInit();
            this.pnlFooter2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Syncfusion.Windows.Forms.Grid.GridListControl gridList;
        private SparkPOS.Helper.UserControl.FilterRangeDate filterRangeDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private Syncfusion.Windows.Forms.Grid.GridListControl gridListPaymentHistory;
        private System.Windows.Forms.Panel pnlFooter2;
        private System.Windows.Forms.Button btnTambahPayment;
        private System.Windows.Forms.Button btnHapusPayment;
        private System.Windows.Forms.Button btnPerbaikiPayment;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chkShowYangBelumLunas;
    }
}