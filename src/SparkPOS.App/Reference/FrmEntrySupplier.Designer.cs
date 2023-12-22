namespace SparkPOS.App.Reference
{
    partial class FrmEntrySupplier
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSupplier = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAddress = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtContact = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtphone = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtcrno = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtvatno = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtSupplier, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtAddress, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtContact, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtphone, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtcrno, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.txtvatno, 1, 5);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 63);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(520, 210);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(4, 183);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Vat No";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(4, 152);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Cr No ";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Supplier";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSupplier
            // 
            this.txtSupplier.AutoEnter = true;
            this.txtSupplier.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSupplier.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtSupplier.LeaveFocusColor = System.Drawing.Color.White;
            this.txtSupplier.LetterOnly = false;
            this.txtSupplier.Location = new System.Drawing.Point(80, 5);
            this.txtSupplier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSupplier.Name = "txtSupplier";
            this.txtSupplier.NumericOnly = false;
            this.txtSupplier.SelectionText = false;
            this.txtSupplier.Size = new System.Drawing.Size(436, 26);
            this.txtSupplier.TabIndex = 0;
            this.txtSupplier.Tag = "name_supplier";
            this.txtSupplier.ThousandSeparator = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(4, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 38);
            this.label2.TabIndex = 1;
            this.label2.Text = "Address";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 76);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 38);
            this.label3.TabIndex = 1;
            this.label3.Text = "Contact";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(4, 114);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 38);
            this.label4.TabIndex = 1;
            this.label4.Text = "phone";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            this.txtAddress.AutoEnter = true;
            this.txtAddress.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtAddress.LeaveFocusColor = System.Drawing.Color.White;
            this.txtAddress.LetterOnly = false;
            this.txtAddress.Location = new System.Drawing.Point(80, 43);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.NumericOnly = false;
            this.txtAddress.SelectionText = false;
            this.txtAddress.Size = new System.Drawing.Size(436, 26);
            this.txtAddress.TabIndex = 1;
            this.txtAddress.Tag = "address";
            this.txtAddress.ThousandSeparator = false;
            // 
            // txtContact
            // 
            this.txtContact.AutoEnter = true;
            this.txtContact.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContact.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtContact.LeaveFocusColor = System.Drawing.Color.White;
            this.txtContact.LetterOnly = false;
            this.txtContact.Location = new System.Drawing.Point(80, 81);
            this.txtContact.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContact.Name = "txtContact";
            this.txtContact.NumericOnly = false;
            this.txtContact.SelectionText = false;
            this.txtContact.Size = new System.Drawing.Size(436, 26);
            this.txtContact.TabIndex = 2;
            this.txtContact.Tag = "contact";
            this.txtContact.ThousandSeparator = false;
            // 
            // txtphone
            // 
            this.txtphone.AutoEnter = false;
            this.txtphone.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtphone.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtphone.LeaveFocusColor = System.Drawing.Color.White;
            this.txtphone.LetterOnly = false;
            this.txtphone.Location = new System.Drawing.Point(80, 119);
            this.txtphone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtphone.Name = "txtphone";
            this.txtphone.NumericOnly = false;
            this.txtphone.SelectionText = false;
            this.txtphone.Size = new System.Drawing.Size(170, 26);
            this.txtphone.TabIndex = 3;
            this.txtphone.Tag = "phone";
            this.txtphone.ThousandSeparator = false;
            this.txtphone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtphone_KeyPress);
            // 
            // txtcrno
            // 
            this.txtcrno.AutoEnter = false;
            this.txtcrno.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtcrno.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtcrno.LeaveFocusColor = System.Drawing.Color.White;
            this.txtcrno.LetterOnly = false;
            this.txtcrno.Location = new System.Drawing.Point(80, 157);
            this.txtcrno.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtcrno.Name = "txtcrno";
            this.txtcrno.NumericOnly = false;
            this.txtcrno.SelectionText = false;
            this.txtcrno.Size = new System.Drawing.Size(170, 26);
            this.txtcrno.TabIndex = 5;
            this.txtcrno.Tag = "phone";
            this.txtcrno.ThousandSeparator = false;
            // 
            // txtvatno
            // 
            this.txtvatno.AutoEnter = false;
            this.txtvatno.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtvatno.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtvatno.LeaveFocusColor = System.Drawing.Color.White;
            this.txtvatno.LetterOnly = false;
            this.txtvatno.Location = new System.Drawing.Point(80, 188);
            this.txtvatno.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtvatno.Name = "txtvatno";
            this.txtvatno.NumericOnly = false;
            this.txtvatno.SelectionText = false;
            this.txtvatno.Size = new System.Drawing.Size(170, 26);
            this.txtvatno.TabIndex = 7;
            this.txtvatno.Tag = "phone";
            this.txtvatno.ThousandSeparator = false;
            // 
            // FrmEntrySupplier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 336);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "FrmEntrySupplier";
            this.Text = "FrmEntrySupplier";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtSupplier;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtAddress;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtContact;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtphone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private Helper.UserControl.AdvancedTextbox txtcrno;
        private Helper.UserControl.AdvancedTextbox txtvatno;
    }
}