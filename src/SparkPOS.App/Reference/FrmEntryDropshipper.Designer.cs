namespace SparkPOS.App.Reference
{
    partial class FrmEntryDropshipper
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
            this.txtDropshipper = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAddress = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtphone = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtDropshipper, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtAddress, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtphone, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(347, 77);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dropshipper";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDropshipper
            // 
            this.txtDropshipper.AutoEnter = true;
            this.txtDropshipper.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtDropshipper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDropshipper.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDropshipper.LeaveFocusColor = System.Drawing.Color.White;
            this.txtDropshipper.LetterOnly = false;
            this.txtDropshipper.Location = new System.Drawing.Point(73, 3);
            this.txtDropshipper.Name = "txtDropshipper";
            this.txtDropshipper.NumericOnly = false;
            this.txtDropshipper.SelectionText = false;
            this.txtDropshipper.Size = new System.Drawing.Size(271, 20);
            this.txtDropshipper.TabIndex = 0;
            this.txtDropshipper.Tag = "name_supplier";
            this.txtDropshipper.ThousandSeparator = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Address";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 25);
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
            this.txtAddress.Location = new System.Drawing.Point(73, 28);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.NumericOnly = false;
            this.txtAddress.SelectionText = false;
            this.txtAddress.Size = new System.Drawing.Size(271, 20);
            this.txtAddress.TabIndex = 1;
            this.txtAddress.Tag = "address";
            this.txtAddress.ThousandSeparator = false;
            // 
            // txtphone
            // 
            this.txtphone.AutoEnter = false;
            this.txtphone.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtphone.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtphone.LeaveFocusColor = System.Drawing.Color.White;
            this.txtphone.LetterOnly = false;
            this.txtphone.Location = new System.Drawing.Point(73, 53);
            this.txtphone.Name = "txtphone";
            this.txtphone.NumericOnly = false;
            this.txtphone.SelectionText = false;
            this.txtphone.Size = new System.Drawing.Size(115, 20);
            this.txtphone.TabIndex = 2;
            this.txtphone.Tag = "phone";
            this.txtphone.ThousandSeparator = false;
            this.txtphone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtphone_KeyPress);
            // 
            // FrmEntryDropshipper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 159);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmEntryDropshipper";
            this.Text = "FrmEntryDropshipper";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtDropshipper;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtAddress;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtphone;
    }
}