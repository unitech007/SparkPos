namespace SparkPOS.Helper.UI.Template
{
    partial class FrmSettingReportStandard
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnSelesai = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkListBox = new System.Windows.Forms.CheckedListBox();
            this.chkBoxTitle = new System.Windows.Forms.CheckBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.txtKeyword = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.rdoDate = new System.Windows.Forms.RadioButton();
            this.rdoMonth = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.dtpDateMulai = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDateSelesai = new System.Windows.Forms.DateTimePicker();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.cmbMonth = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbYear = new System.Windows.Forms.ComboBox();
            this.chkShowInvoice = new System.Windows.Forms.CheckBox();
            this.chkShowDetailsInvoice = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.77215F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.22785F));
            this.tableLayoutPanel1.Controls.Add(this.pnlFooter, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pnlHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(670, 478);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.pnlFooter, 2);
            this.pnlFooter.Controls.Add(this.btnSelesai);
            this.pnlFooter.Controls.Add(this.btnPreview);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFooter.Location = new System.Drawing.Point(3, 441);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(664, 34);
            this.pnlFooter.TabIndex = 3;
            // 
            // btnSelesai
            // 
            this.btnSelesai.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelesai.Location = new System.Drawing.Point(578, 6);
            this.btnSelesai.Name = "btnSelesai";
            this.btnSelesai.Size = new System.Drawing.Size(75, 23);
            this.btnSelesai.TabIndex = 3;
            this.btnSelesai.Text = "Esc Cancel";
            this.btnSelesai.UseVisualStyleBackColor = true;
            this.btnSelesai.Click += new System.EventHandler(this.btnSelesai_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Location = new System.Drawing.Point(497, 6);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 2;
            this.btnPreview.Tag = "3";
            this.btnPreview.Text = "F10 Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.pnlHeader, 2);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(664, 34);
            this.pnlHeader.TabIndex = 3;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(3, 7);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(58, 17);
            this.lblHeader.TabIndex = 3;
            this.lblHeader.Text = "Header";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.chkListBox, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkBoxTitle, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkSelectAll, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.txtKeyword, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(273, 392);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // chkListBox
            // 
            this.chkListBox.CheckOnClick = true;
            this.chkListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkListBox.Enabled = false;
            this.chkListBox.FormattingEnabled = true;
            this.chkListBox.Location = new System.Drawing.Point(3, 54);
            this.chkListBox.Name = "chkListBox";
            this.chkListBox.Size = new System.Drawing.Size(267, 310);
            this.chkListBox.TabIndex = 2;
            // 
            // chkBoxTitle
            // 
            this.chkBoxTitle.AutoSize = true;
            this.chkBoxTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkBoxTitle.Location = new System.Drawing.Point(3, 3);
            this.chkBoxTitle.Name = "chkBoxTitle";
            this.chkBoxTitle.Size = new System.Drawing.Size(267, 19);
            this.chkBoxTitle.TabIndex = 0;
            this.chkBoxTitle.Text = "checkBox1";
            this.chkBoxTitle.UseVisualStyleBackColor = true;
            this.chkBoxTitle.CheckedChanged += new System.EventHandler(this.chkBoxTitle_CheckedChanged);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSelectAll.Enabled = false;
            this.chkSelectAll.Location = new System.Drawing.Point(3, 370);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(267, 19);
            this.chkSelectAll.TabIndex = 3;
            this.chkSelectAll.Text = "Select Options";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // txtKeyword
            // 
            this.txtKeyword.AutoEnter = false;
            this.txtKeyword.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtKeyword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyword.Enabled = false;
            this.txtKeyword.EnterFocusColor = System.Drawing.Color.White;
            this.txtKeyword.LeaveFocusColor = System.Drawing.Color.White;
            this.txtKeyword.LetterOnly = false;
            this.txtKeyword.Location = new System.Drawing.Point(3, 28);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.NumericOnly = false;
            this.txtKeyword.SelectionText = false;
            this.txtKeyword.Size = new System.Drawing.Size(267, 20);
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.ThousandSeparator = false;
            this.txtKeyword.TextChanged += new System.EventHandler(this.txtKeyword_TextChanged);
            this.txtKeyword.Enter += new System.EventHandler(this.txtKeyword_Enter);
            this.txtKeyword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtKeyword_KeyPress);
            this.txtKeyword.Leave += new System.EventHandler(this.txtKeyword_Leave);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.rdoDate, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.rdoMonth, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkShowInvoice, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkShowDetailsInvoice, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(282, 43);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(385, 392);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // rdoDate
            // 
            this.rdoDate.AutoSize = true;
            this.rdoDate.Checked = true;
            this.rdoDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoDate.Location = new System.Drawing.Point(3, 3);
            this.rdoDate.Name = "rdoDate";
            this.rdoDate.Size = new System.Drawing.Size(99, 19);
            this.rdoDate.TabIndex = 0;
            this.rdoDate.TabStop = true;
            this.rdoDate.Text = "date";
            this.rdoDate.UseVisualStyleBackColor = true;
            // 
            // rdoMonth
            // 
            this.rdoMonth.AutoSize = true;
            this.rdoMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoMonth.Location = new System.Drawing.Point(3, 28);
            this.rdoMonth.Name = "rdoMonth";
            this.rdoMonth.Size = new System.Drawing.Size(99, 19);
            this.rdoMonth.TabIndex = 1;
            this.rdoMonth.Text = "month";
            this.rdoMonth.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.dtpDateMulai);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.dtpDateSelesai);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(105, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(280, 25);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // dtpDateMulai
            // 
            this.dtpDateMulai.CustomFormat = "dd/MM/yyyy";
            this.dtpDateMulai.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateMulai.Location = new System.Drawing.Point(3, 3);
            this.dtpDateMulai.Name = "dtpDateMulai";
            this.dtpDateMulai.Size = new System.Drawing.Size(100, 20);
            this.dtpDateMulai.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(109, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "s.d";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtpDateSelesai
            // 
            this.dtpDateSelesai.CustomFormat = "dd/MM/yyyy";
            this.dtpDateSelesai.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateSelesai.Location = new System.Drawing.Point(136, 3);
            this.dtpDateSelesai.Name = "dtpDateSelesai";
            this.dtpDateSelesai.Size = new System.Drawing.Size(100, 20);
            this.dtpDateSelesai.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.cmbMonth);
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.cmbYear);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(105, 25);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(280, 25);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // cmbMonth
            // 
            this.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonth.FormattingEnabled = true;
            this.cmbMonth.Location = new System.Drawing.Point(3, 3);
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size(100, 21);
            this.cmbMonth.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(109, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "year";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbYear
            // 
            this.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYear.FormattingEnabled = true;
            this.cmbYear.Location = new System.Drawing.Point(153, 3);
            this.cmbYear.Name = "cmbYear";
            this.cmbYear.Size = new System.Drawing.Size(83, 21);
            this.cmbYear.TabIndex = 1;
            // 
            // chkShowInvoice
            // 
            this.chkShowInvoice.AutoSize = true;
            this.chkShowInvoice.Location = new System.Drawing.Point(3, 53);
            this.chkShowInvoice.Name = "chkShowInvoice";
            this.chkShowInvoice.Size = new System.Drawing.Size(99, 17);
            this.chkShowInvoice.TabIndex = 2;
            this.chkShowInvoice.Text = "Show invoice";
            this.chkShowInvoice.UseVisualStyleBackColor = true;
            this.chkShowInvoice.CheckedChanged += new System.EventHandler(this.chkShowInvoice_CheckedChanged);
            // 
            // chkShowDetailsInvoice
            // 
            this.chkShowDetailsInvoice.AutoSize = true;
            this.chkShowDetailsInvoice.Enabled = false;
            this.chkShowDetailsInvoice.Location = new System.Drawing.Point(108, 53);
            this.chkShowDetailsInvoice.Name = "chkShowDetailsInvoice";
            this.chkShowDetailsInvoice.Size = new System.Drawing.Size(133, 17);
            this.chkShowDetailsInvoice.TabIndex = 3;
            this.chkShowDetailsInvoice.Text = "Show details invoice";
            this.chkShowDetailsInvoice.UseVisualStyleBackColor = true;
            this.chkShowDetailsInvoice.Visible = false;
            // 
            // FrmSettingReportStandard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 478);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettingReportStandard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmSettingReportStandard";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmSettingReportStandard_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmSettingReportStandard_KeyPress);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        protected System.Windows.Forms.CheckedListBox chkListBox;
        protected System.Windows.Forms.CheckBox chkBoxTitle;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        protected System.Windows.Forms.RadioButton rdoDate;
        protected System.Windows.Forms.RadioButton rdoMonth;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        protected System.Windows.Forms.DateTimePicker dtpDateMulai;
        private System.Windows.Forms.Label label1;
        protected System.Windows.Forms.DateTimePicker dtpDateSelesai;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        protected System.Windows.Forms.ComboBox cmbMonth;
        private System.Windows.Forms.Label label2;
        protected System.Windows.Forms.ComboBox cmbYear;
        protected System.Windows.Forms.CheckBox chkShowInvoice;
        protected System.Windows.Forms.CheckBox chkShowDetailsInvoice;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnSelesai;
        protected System.Windows.Forms.Button btnPreview;
        protected UserControl.AdvancedTextbox txtKeyword;
    }
}