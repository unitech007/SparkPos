namespace SparkPOS.App.Report
{
    partial class FrmLapProductStockCard
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
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkListOfProduct = new System.Windows.Forms.CheckedListBox();
            this.txtNameProduct = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkFilterOptional = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.rdoDate, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.rdoMonth, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(401, 53);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // rdoDate
            // 
            this.rdoDate.AutoSize = true;
            this.rdoDate.Checked = true;
            this.rdoDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoDate.Location = new System.Drawing.Point(3, 3);
            this.rdoDate.Name = "rdoDate";
            this.rdoDate.Size = new System.Drawing.Size(64, 19);
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
            this.rdoMonth.Size = new System.Drawing.Size(64, 19);
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(70, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(331, 25);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // dtpDateMulai
            // 
            this.dtpDateMulai.CustomFormat = "dd/MM/yyyy";
            this.dtpDateMulai.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateMulai.Location = new System.Drawing.Point(3, 3);
            this.dtpDateMulai.Name = "dtpDateMulai";
            this.dtpDateMulai.Size = new System.Drawing.Size(144, 20);
            this.dtpDateMulai.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(153, 0);
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
            this.dtpDateSelesai.Location = new System.Drawing.Point(180, 3);
            this.dtpDateSelesai.Name = "dtpDateSelesai";
            this.dtpDateSelesai.Size = new System.Drawing.Size(144, 20);
            this.dtpDateSelesai.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.cmbMonth);
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.cmbYear);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(70, 25);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(331, 25);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // cmbMonth
            // 
            this.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonth.FormattingEnabled = true;
            this.cmbMonth.Location = new System.Drawing.Point(3, 3);
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size(144, 21);
            this.cmbMonth.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(153, 0);
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
            this.cmbYear.Location = new System.Drawing.Point(197, 3);
            this.cmbYear.Name = "cmbYear";
            this.cmbYear.Size = new System.Drawing.Size(127, 21);
            this.cmbYear.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 141F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(407, 238);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkFilterOptional);
            this.groupBox1.Controls.Add(this.chkListOfProduct);
            this.groupBox1.Controls.Add(this.txtNameProduct);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(401, 173);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "   ";
            // 
            // chkListOfProduct
            // 
            this.chkListOfProduct.CheckOnClick = true;
            this.chkListOfProduct.Enabled = false;
            this.chkListOfProduct.FormattingEnabled = true;
            this.chkListOfProduct.Location = new System.Drawing.Point(117, 43);
            this.chkListOfProduct.Name = "chkListOfProduct";
            this.chkListOfProduct.Size = new System.Drawing.Size(277, 124);
            this.chkListOfProduct.TabIndex = 1;
            // 
            // txtNameProduct
            // 
            this.txtNameProduct.AutoEnter = false;
            this.txtNameProduct.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNameProduct.Enabled = false;
            this.txtNameProduct.EnterFocusColor = System.Drawing.Color.White;
            this.txtNameProduct.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNameProduct.LetterOnly = false;
            this.txtNameProduct.Location = new System.Drawing.Point(117, 17);
            this.txtNameProduct.Name = "txtNameProduct";
            this.txtNameProduct.NumericOnly = false;
            this.txtNameProduct.SelectionText = false;
            this.txtNameProduct.Size = new System.Drawing.Size(237, 20);
            this.txtNameProduct.TabIndex = 0;
            this.txtNameProduct.ThousandSeparator = false;
            this.txtNameProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNameProduct_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(360, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Enter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Code/Name Product";
            // 
            // chkFilterOptional
            // 
            this.chkFilterOptional.AutoSize = true;
            this.chkFilterOptional.Location = new System.Drawing.Point(13, -1);
            this.chkFilterOptional.Name = "chkFilterOptional";
            this.chkFilterOptional.Size = new System.Drawing.Size(102, 17);
            this.chkFilterOptional.TabIndex = 2;
            this.chkFilterOptional.Text = "Filter Optional";
            this.chkFilterOptional.UseVisualStyleBackColor = true;
            this.chkFilterOptional.CheckedChanged += new System.EventHandler(this.chkFilterOptional_CheckedChanged);
            // 
            // FrmLapProductStockCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 320);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Name = "FrmLapProductStockCard";
            this.Text = "FrmLapProductStockCard";
            this.Controls.SetChildIndex(this.tableLayoutPanel4, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton rdoDate;
        private System.Windows.Forms.RadioButton rdoMonth;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.DateTimePicker dtpDateMulai;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDateSelesai;
        private System.Windows.Forms.ComboBox cmbMonth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbYear;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox chkListOfProduct;
        private Helper.UserControl.AdvancedTextbox txtNameProduct;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkFilterOptional;
    }
}