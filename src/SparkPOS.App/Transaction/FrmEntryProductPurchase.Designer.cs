namespace SparkPOS.App.Transactions
{
    partial class FrmEntryProductPurchase
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoCash = new System.Windows.Forms.RadioButton();
            this.rdoKredit = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpDateCreditTerm = new System.Windows.Forms.DateTimePicker();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.txtInvoice = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtSupplier = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtKeterangan = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gridControl = new Syncfusion.Windows.Forms.Grid.GridControl();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.chkPrintInvoicePurchase = new System.Windows.Forms.CheckBox();
            this.txtDiskon = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtPPN = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbTaxName = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 63);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1238, 740);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.dtpDate, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.txtInvoice, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtSupplier, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.txtKeterangan, 1, 4);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 87);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 6;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1230, 190);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(4, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 38);
            this.label2.TabIndex = 0;
            this.label2.Text = "Invoice";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 76);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 38);
            this.label3.TabIndex = 0;
            this.label3.Text = "Status";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(4, 114);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 38);
            this.label4.TabIndex = 0;
            this.label4.Text = "Supplier";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(4, 152);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 38);
            this.label5.TabIndex = 0;
            this.label5.Text = "Description";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.rdoCash);
            this.flowLayoutPanel1.Controls.Add(this.rdoKredit);
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.dtpDateCreditTerm);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(97, 76);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1133, 38);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // rdoCash
            // 
            this.rdoCash.AutoSize = true;
            this.rdoCash.Checked = true;
            this.rdoCash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoCash.Location = new System.Drawing.Point(4, 5);
            this.rdoCash.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoCash.Name = "rdoCash";
            this.rdoCash.Size = new System.Drawing.Size(71, 26);
            this.rdoCash.TabIndex = 0;
            this.rdoCash.TabStop = true;
            this.rdoCash.Text = "Cash";
            this.rdoCash.UseVisualStyleBackColor = true;
            this.rdoCash.CheckedChanged += new System.EventHandler(this.rdoCash_CheckedChanged);
            // 
            // rdoKredit
            // 
            this.rdoKredit.AutoSize = true;
            this.rdoKredit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoKredit.Location = new System.Drawing.Point(83, 5);
            this.rdoKredit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoKredit.Name = "rdoKredit";
            this.rdoKredit.Size = new System.Drawing.Size(76, 26);
            this.rdoKredit.TabIndex = 1;
            this.rdoKredit.Text = "Credit";
            this.rdoKredit.UseVisualStyleBackColor = true;
            this.rdoKredit.CheckedChanged += new System.EventHandler(this.rdoKredit_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(167, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 36);
            this.label6.TabIndex = 2;
            this.label6.Text = "Due Date";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtpDateCreditTerm
            // 
            this.dtpDateCreditTerm.CustomFormat = "dd/MM/yyyy";
            this.dtpDateCreditTerm.Enabled = false;
            this.dtpDateCreditTerm.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateCreditTerm.Location = new System.Drawing.Point(253, 5);
            this.dtpDateCreditTerm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpDateCreditTerm.Name = "dtpDateCreditTerm";
            this.dtpDateCreditTerm.Size = new System.Drawing.Size(148, 26);
            this.dtpDateCreditTerm.TabIndex = 2;
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "dd/MM/yyyy";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(101, 43);
            this.dtpDate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(148, 26);
            this.dtpDate.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(4, 38);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 38);
            this.label7.TabIndex = 4;
            this.label7.Text = "date";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtInvoice
            // 
            this.txtInvoice.AutoEnter = true;
            this.txtInvoice.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtInvoice.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtInvoice.LeaveFocusColor = System.Drawing.Color.White;
            this.txtInvoice.LetterOnly = false;
            this.txtInvoice.Location = new System.Drawing.Point(101, 5);
            this.txtInvoice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.NumericOnly = false;
            this.txtInvoice.SelectionText = false;
            this.txtInvoice.Size = new System.Drawing.Size(148, 26);
            this.txtInvoice.TabIndex = 0;
            this.txtInvoice.Tag = "invoice";
            this.txtInvoice.ThousandSeparator = false;
            // 
            // txtSupplier
            // 
            this.txtSupplier.AutoEnter = false;
            this.txtSupplier.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtSupplier.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtSupplier.LeaveFocusColor = System.Drawing.Color.White;
            this.txtSupplier.LetterOnly = false;
            this.txtSupplier.Location = new System.Drawing.Point(101, 119);
            this.txtSupplier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSupplier.Name = "txtSupplier";
            this.txtSupplier.NumericOnly = false;
            this.txtSupplier.SelectionText = false;
            this.txtSupplier.Size = new System.Drawing.Size(434, 26);
            this.txtSupplier.TabIndex = 3;
            this.txtSupplier.Tag = "";
            this.txtSupplier.ThousandSeparator = false;
            this.txtSupplier.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplier_KeyPress);
            // 
            // txtKeterangan
            // 
            this.txtKeterangan.AutoEnter = false;
            this.txtKeterangan.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtKeterangan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeterangan.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtKeterangan.LeaveFocusColor = System.Drawing.Color.White;
            this.txtKeterangan.LetterOnly = false;
            this.txtKeterangan.Location = new System.Drawing.Point(101, 157);
            this.txtKeterangan.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtKeterangan.Name = "txtKeterangan";
            this.txtKeterangan.NumericOnly = false;
            this.txtKeterangan.SelectionText = false;
            this.txtKeterangan.Size = new System.Drawing.Size(1125, 26);
            this.txtKeterangan.TabIndex = 4;
            this.txtKeterangan.Tag = "description";
            this.txtKeterangan.ThousandSeparator = false;
            this.txtKeterangan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtKeterangan_KeyPress);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gridControl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 287);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(8);
            this.panel1.Size = new System.Drawing.Size(1230, 365);
            this.panel1.TabIndex = 1;
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(8, 8);
            this.gridControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gridControl.Name = "gridControl";
            this.gridControl.SerializeCellsBehavior = Syncfusion.Windows.Forms.Grid.GridSerializeCellsBehavior.SerializeIntoCode;
            this.gridControl.Size = new System.Drawing.Size(1214, 349);
            this.gridControl.SmartSizeBox = false;
            this.gridControl.TabIndex = 0;
            this.gridControl.Text = "gridControl1";
            this.gridControl.UseRightToLeftCompatibleTextBox = true;
            this.gridControl.CurrentCellValidated += new System.EventHandler(this.gridControl_CurrentCellValidated);
            this.gridControl.CellClick += new Syncfusion.Windows.Forms.Grid.GridCellClickEventHandler(this.gridControl_CellClick);
            this.gridControl.CurrentCellKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridControl_CurrentCellKeyPress);
            this.gridControl.CurrentCellKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl_CurrentCellKeyDown);
            this.gridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl_KeyDown);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.label9, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.chkPrintInvoicePurchase, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.txtDiskon, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.txtPPN, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.cmbTaxName, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(4, 662);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1230, 73);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(541, 38);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(529, 38);
            this.label9.TabIndex = 0;
            this.label9.Text = "tax";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(4, 0);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(529, 38);
            this.label10.TabIndex = 2;
            this.label10.Text = "F1 : Add data product  |  F2 : Add data supplier";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkPrintInvoicePurchase
            // 
            this.chkPrintInvoicePurchase.AutoSize = true;
            this.chkPrintInvoicePurchase.Checked = true;
            this.chkPrintInvoicePurchase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrintInvoicePurchase.Location = new System.Drawing.Point(4, 43);
            this.chkPrintInvoicePurchase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkPrintInvoicePurchase.Name = "chkPrintInvoicePurchase";
            this.chkPrintInvoicePurchase.Size = new System.Drawing.Size(189, 24);
            this.chkPrintInvoicePurchase.TabIndex = 3;
            this.chkPrintInvoicePurchase.Text = "Print purchase invoice";
            this.chkPrintInvoicePurchase.UseVisualStyleBackColor = true;
            // 
            // txtDiskon
            // 
            this.txtDiskon.AutoEnter = true;
            this.txtDiskon.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtDiskon.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDiskon.LeaveFocusColor = System.Drawing.Color.White;
            this.txtDiskon.LetterOnly = false;
            this.txtDiskon.Location = new System.Drawing.Point(1078, 5);
            this.txtDiskon.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDiskon.MaxLength = 20;
            this.txtDiskon.Name = "txtDiskon";
            this.txtDiskon.NumericOnly = true;
            this.txtDiskon.SelectionText = false;
            this.txtDiskon.Size = new System.Drawing.Size(148, 26);
            this.txtDiskon.TabIndex = 0;
            this.txtDiskon.Text = "0";
            this.txtDiskon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiskon.ThousandSeparator = true;
            this.txtDiskon.TextChanged += new System.EventHandler(this.txtDiskon_TextChanged);
            // 
            // txtPPN
            // 
            this.txtPPN.AutoEnter = false;
            this.txtPPN.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtPPN.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPPN.LeaveFocusColor = System.Drawing.Color.White;
            this.txtPPN.LetterOnly = false;
            this.txtPPN.Location = new System.Drawing.Point(1078, 43);
            this.txtPPN.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPPN.MaxLength = 20;
            this.txtPPN.Name = "txtPPN";
            this.txtPPN.NumericOnly = true;
            this.txtPPN.SelectionText = false;
            this.txtPPN.Size = new System.Drawing.Size(148, 26);
            this.txtPPN.TabIndex = 1;
            this.txtPPN.Text = "0";
            this.txtPPN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPPN.ThousandSeparator = true;
            this.txtPPN.TextChanged += new System.EventHandler(this.txtPPN_TextChanged);
            this.txtPPN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPPN_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(4, 76);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(529, 31);
            this.label8.TabIndex = 0;
            this.label8.Text = "discount";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbTaxName
            // 
            this.cmbTaxName.FormattingEnabled = true;
            this.cmbTaxName.Location = new System.Drawing.Point(541, 5);
            this.cmbTaxName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbTaxName.Name = "cmbTaxName";
            this.cmbTaxName.Size = new System.Drawing.Size(226, 28);
            this.cmbTaxName.TabIndex = 9;
            this.cmbTaxName.SelectedIndexChanged += new System.EventHandler(this.cmbTaxName_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.tableLayoutPanel6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 5);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1230, 72);
            this.panel2.TabIndex = 4;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.lblTotal, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1228, 70);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = "TOTAL";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Red;
            this.lblTotal.Location = new System.Drawing.Point(1172, 0);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(52, 70);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "0";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmEntryProductPurchase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1238, 866);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "FrmEntryProductPurchase";
            this.Text = "FrmEntryProductPurchase";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEntryProductPurchase_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmEntryProductPurchase_KeyDown);
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rdoCash;
        private System.Windows.Forms.RadioButton rdoKredit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpDateCreditTerm;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.Grid.GridControl gridControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkPrintInvoicePurchase;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTotal;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtDiskon;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtPPN;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtInvoice;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtSupplier;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtKeterangan;
        private System.Windows.Forms.ComboBox cmbTaxName;
    }
}