﻿namespace SparkPOS.App.Expense
{
    partial class FrmEntryLoan
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbEmployee = new System.Windows.Forms.ComboBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.txtInvoice = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtJumlah = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtKeterangan = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.cmbEmployee, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.dtpDate, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtInvoice, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtJumlah, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtKeterangan, 1, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(419, 128);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Invoice";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "date";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "Employee";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 25);
            this.label5.TabIndex = 1;
            this.label5.Text = "Description";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 25);
            this.label6.TabIndex = 5;
            this.label6.Text = "quantity";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbEmployee
            // 
            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.FormattingEnabled = true;
            this.cmbEmployee.Location = new System.Drawing.Point(69, 53);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(345, 21);
            this.cmbEmployee.TabIndex = 2;
            this.cmbEmployee.Tag = "ignore";
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(69, 28);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(200, 20);
            this.dtpDate.TabIndex = 1;
            // 
            // txtInvoice
            // 
            this.txtInvoice.AutoEnter = true;
            this.txtInvoice.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtInvoice.EnterFocusColor = System.Drawing.Color.White;
            this.txtInvoice.LeaveFocusColor = System.Drawing.Color.White;
            this.txtInvoice.LetterOnly = false;
            this.txtInvoice.Location = new System.Drawing.Point(69, 3);
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.NumericOnly = false;
            this.txtInvoice.SelectionText = false;
            this.txtInvoice.Size = new System.Drawing.Size(100, 20);
            this.txtInvoice.TabIndex = 0;
            this.txtInvoice.Tag = "invoice";
            this.txtInvoice.ThousandSeparator = false;
            // 
            // txtJumlah
            // 
            this.txtJumlah.AutoEnter = true;
            this.txtJumlah.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtJumlah.EnterFocusColor = System.Drawing.Color.White;
            this.txtJumlah.LeaveFocusColor = System.Drawing.Color.White;
            this.txtJumlah.LetterOnly = false;
            this.txtJumlah.Location = new System.Drawing.Point(69, 78);
            this.txtJumlah.MaxLength = 20;
            this.txtJumlah.Name = "txtJumlah";
            this.txtJumlah.NumericOnly = true;
            this.txtJumlah.SelectionText = false;
            this.txtJumlah.Size = new System.Drawing.Size(100, 20);
            this.txtJumlah.TabIndex = 3;
            this.txtJumlah.Tag = "amount";
            this.txtJumlah.Text = "0";
            this.txtJumlah.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtJumlah.ThousandSeparator = true;
            // 
            // txtKeterangan
            // 
            this.txtKeterangan.AutoEnter = false;
            this.txtKeterangan.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtKeterangan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeterangan.EnterFocusColor = System.Drawing.Color.White;
            this.txtKeterangan.LeaveFocusColor = System.Drawing.Color.White;
            this.txtKeterangan.LetterOnly = false;
            this.txtKeterangan.Location = new System.Drawing.Point(69, 103);
            this.txtKeterangan.Name = "txtKeterangan";
            this.txtKeterangan.NumericOnly = false;
            this.txtKeterangan.SelectionText = false;
            this.txtKeterangan.Size = new System.Drawing.Size(347, 20);
            this.txtKeterangan.TabIndex = 4;
            this.txtKeterangan.Tag = "description";
            this.txtKeterangan.ThousandSeparator = false;
            this.txtKeterangan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtKeterangan_KeyPress);
            // 
            // FrmEntryLoan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 210);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmEntryLoan";
            this.Text = "FrmEntryLoan";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbEmployee;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtInvoice;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtJumlah;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtKeterangan;


    }
}