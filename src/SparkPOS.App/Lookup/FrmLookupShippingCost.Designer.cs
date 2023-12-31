﻿namespace SparkPOS.App.Lookup
{
    partial class FrmLookupShippingCost
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
            this.gridList = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRegencyAsal = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtBerat = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.btnCekOngkir = new System.Windows.Forms.Button();
            this.txtRegencyTujuan = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.gridList, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 63);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(776, 622);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // gridList
            // 
            this.gridList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            this.gridList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridList.ItemHeight = 17;
            this.gridList.Location = new System.Drawing.Point(4, 133);
            this.gridList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gridList.Name = "gridList";
            this.gridList.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridList.SelectedIndex = -1;
            this.gridList.Size = new System.Drawing.Size(768, 446);
            this.gridList.TabIndex = 1;
            this.gridList.TopIndex = 0;
            this.gridList.DoubleClick += new System.EventHandler(this.gridList_DoubleClick);
            this.gridList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridList_KeyPress);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.txtRegencyAsal, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.txtRegencyTujuan, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 5);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(768, 118);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "City/Regency Asal";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(4, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 38);
            this.label2.TabIndex = 0;
            this.label2.Text = "City/Regency Tujuan";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 76);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 42);
            this.label3.TabIndex = 0;
            this.label3.Text = "Berat shippingan (gram)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRegencyAsal
            // 
            this.txtRegencyAsal.AutoEnter = false;
            this.txtRegencyAsal.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtRegencyAsal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRegencyAsal.EnterFocusColor = System.Drawing.Color.White;
            this.txtRegencyAsal.LeaveFocusColor = System.Drawing.Color.White;
            this.txtRegencyAsal.LetterOnly = false;
            this.txtRegencyAsal.Location = new System.Drawing.Point(191, 5);
            this.txtRegencyAsal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtRegencyAsal.Name = "txtRegencyAsal";
            this.txtRegencyAsal.NumericOnly = false;
            this.txtRegencyAsal.SelectionText = false;
            this.txtRegencyAsal.Size = new System.Drawing.Size(573, 26);
            this.txtRegencyAsal.TabIndex = 0;
            this.txtRegencyAsal.ThousandSeparator = false;
            this.txtRegencyAsal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRegencyAsal_KeyPress);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.txtBerat);
            this.flowLayoutPanel1.Controls.Add(this.btnCekOngkir);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(187, 76);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(489, 42);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // txtBerat
            // 
            this.txtBerat.AutoEnter = true;
            this.txtBerat.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtBerat.EnterFocusColor = System.Drawing.Color.White;
            this.txtBerat.LeaveFocusColor = System.Drawing.Color.White;
            this.txtBerat.LetterOnly = false;
            this.txtBerat.Location = new System.Drawing.Point(4, 5);
            this.txtBerat.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBerat.MaxLength = 5;
            this.txtBerat.Name = "txtBerat";
            this.txtBerat.NumericOnly = true;
            this.txtBerat.SelectionText = false;
            this.txtBerat.Size = new System.Drawing.Size(85, 26);
            this.txtBerat.TabIndex = 0;
            this.txtBerat.Text = "0";
            this.txtBerat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBerat.ThousandSeparator = false;
            // 
            // btnCekOngkir
            // 
            this.btnCekOngkir.Location = new System.Drawing.Point(97, 5);
            this.btnCekOngkir.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCekOngkir.Name = "btnCekOngkir";
            this.btnCekOngkir.Size = new System.Drawing.Size(134, 35);
            this.btnCekOngkir.TabIndex = 1;
            this.btnCekOngkir.Text = "Check Ongkir";
            this.btnCekOngkir.UseVisualStyleBackColor = true;
            this.btnCekOngkir.Click += new System.EventHandler(this.btnCekOngkir_Click);
            // 
            // txtRegencyTujuan
            // 
            this.txtRegencyTujuan.AutoEnter = false;
            this.txtRegencyTujuan.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtRegencyTujuan.EnterFocusColor = System.Drawing.Color.White;
            this.txtRegencyTujuan.LeaveFocusColor = System.Drawing.Color.White;
            this.txtRegencyTujuan.LetterOnly = false;
            this.txtRegencyTujuan.Location = new System.Drawing.Point(191, 43);
            this.txtRegencyTujuan.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtRegencyTujuan.Name = "txtRegencyTujuan";
            this.txtRegencyTujuan.NumericOnly = false;
            this.txtRegencyTujuan.SelectionText = false;
            this.txtRegencyTujuan.Size = new System.Drawing.Size(478, 26);
            this.txtRegencyTujuan.TabIndex = 1;
            this.txtRegencyTujuan.ThousandSeparator = false;
            this.txtRegencyTujuan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRegencyTujuan_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Location = new System.Drawing.Point(4, 584);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(443, 38);
            this.label4.TabIndex = 2;
            this.label4.Text = "Info: Untuk saat ini cek ongkir hanya sampai city atau regency";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmLookupShippingCost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 748);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "FrmLookupShippingCost";
            this.Text = "FrmLookupShippingCost";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Syncfusion.Windows.Forms.Grid.GridListControl gridList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtBerat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCekOngkir;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtRegencyAsal;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtRegencyTujuan;
    }
}