namespace SparkPOS.App.Transactions
{
    partial class FrmEntryAddressShipping
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
            this.chkIsSdac = new System.Windows.Forms.CheckBox();
            this.pnlAddressShipping = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtto1 = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtto2 = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtto3 = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtto4 = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.pnlAddressShipping.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.chkIsSdac, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.pnlAddressShipping, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(459, 133);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // chkIsSdac
            // 
            this.chkIsSdac.AutoSize = true;
            this.chkIsSdac.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkIsSdac.Location = new System.Drawing.Point(3, 3);
            this.chkIsSdac.Name = "chkIsSdac";
            this.chkIsSdac.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.chkIsSdac.Size = new System.Drawing.Size(314, 19);
            this.chkIsSdac.TabIndex = 0;
            this.chkIsSdac.Text = "The shipping address is the same as the customer\'s address";
            this.chkIsSdac.UseVisualStyleBackColor = true;
            this.chkIsSdac.CheckedChanged += new System.EventHandler(this.chkIsSdac_CheckedChanged);
            // 
            // pnlAddressShipping
            // 
            this.pnlAddressShipping.Controls.Add(this.tableLayoutPanel4);
            this.pnlAddressShipping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAddressShipping.Location = new System.Drawing.Point(3, 28);
            this.pnlAddressShipping.Name = "pnlAddressShipping";
            this.pnlAddressShipping.Size = new System.Drawing.Size(453, 102);
            this.pnlAddressShipping.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.txtto1, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtto2, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.txtto3, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.txtto4, 1, 3);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(453, 102);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "to #1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "to #2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Location = new System.Drawing.Point(3, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 25);
            this.label3.TabIndex = 0;
            this.label3.Text = "to #3";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Location = new System.Drawing.Point(3, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "to #4";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtto1
            // 
            this.txtto1.AutoEnter = true;
            this.txtto1.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtto1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtto1.EnterFocusColor = System.Drawing.Color.White;
            this.txtto1.LeaveFocusColor = System.Drawing.Color.White;
            this.txtto1.LetterOnly = false;
            this.txtto1.Location = new System.Drawing.Point(41, 3);
            this.txtto1.MaxLength = 50;
            this.txtto1.Name = "txtto1";
            this.txtto1.NumericOnly = false;
            this.txtto1.SelectionText = false;
            this.txtto1.Size = new System.Drawing.Size(409, 20);
            this.txtto1.TabIndex = 0;
            this.txtto1.Tag = "to";
            this.txtto1.ThousandSeparator = false;
            // 
            // txtto2
            // 
            this.txtto2.AutoEnter = true;
            this.txtto2.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtto2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtto2.EnterFocusColor = System.Drawing.Color.White;
            this.txtto2.LeaveFocusColor = System.Drawing.Color.White;
            this.txtto2.LetterOnly = false;
            this.txtto2.Location = new System.Drawing.Point(41, 28);
            this.txtto2.MaxLength = 250;
            this.txtto2.Name = "txtto2";
            this.txtto2.NumericOnly = false;
            this.txtto2.SelectionText = false;
            this.txtto2.Size = new System.Drawing.Size(409, 20);
            this.txtto2.TabIndex = 1;
            this.txtto2.Tag = "address";
            this.txtto2.ThousandSeparator = false;
            // 
            // txtto3
            // 
            this.txtto3.AutoEnter = true;
            this.txtto3.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtto3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtto3.EnterFocusColor = System.Drawing.Color.White;
            this.txtto3.LeaveFocusColor = System.Drawing.Color.White;
            this.txtto3.LetterOnly = false;
            this.txtto3.Location = new System.Drawing.Point(41, 53);
            this.txtto3.MaxLength = 250;
            this.txtto3.Name = "txtto3";
            this.txtto3.NumericOnly = false;
            this.txtto3.SelectionText = false;
            this.txtto3.Size = new System.Drawing.Size(409, 20);
            this.txtto3.TabIndex = 2;
            this.txtto3.Tag = "subdistrict";
            this.txtto3.ThousandSeparator = false;
            // 
            // txtto4
            // 
            this.txtto4.AutoEnter = true;
            this.txtto4.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtto4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtto4.EnterFocusColor = System.Drawing.Color.White;
            this.txtto4.LeaveFocusColor = System.Drawing.Color.White;
            this.txtto4.LetterOnly = false;
            this.txtto4.Location = new System.Drawing.Point(41, 78);
            this.txtto4.MaxLength = 250;
            this.txtto4.Name = "txtto4";
            this.txtto4.NumericOnly = false;
            this.txtto4.SelectionText = false;
            this.txtto4.Size = new System.Drawing.Size(409, 20);
            this.txtto4.TabIndex = 3;
            this.txtto4.Tag = "village";
            this.txtto4.ThousandSeparator = false;
            // 
            // FrmEntryAddressShipping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 215);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmEntryAddressShipping";
            this.Text = "FrmEntryAddressShipping";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.pnlAddressShipping.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox chkIsSdac;
        private System.Windows.Forms.Panel pnlAddressShipping;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtto1;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtto2;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtto3;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtto4;
    }
}