namespace SparkPOS.App.Cashier.Transactions
{
    partial class FrmPay
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
            this.txtTotal = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkPayViaCard = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDiskon = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtPPN = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtGrandTotal = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtPayCash = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.cmbCard = new System.Windows.Forms.ComboBox();
            this.txtPayCard = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtNoCard = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRefund = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtTotal, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.chkPayViaCard, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.txtDiskon, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtPPN, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtGrandTotal, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtPayCash, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.cmbCard, 1, 6);
            this.tableLayoutPanel3.Controls.Add(this.txtPayCard, 1, 7);
            this.tableLayoutPanel3.Controls.Add(this.txtNoCard, 1, 8);
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 10);
            this.tableLayoutPanel3.Controls.Add(this.txtRefund, 1, 10);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 12;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(433, 340);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTotal
            // 
            this.txtTotal.AutoEnter = true;
            this.txtTotal.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTotal.Enabled = false;
            this.txtTotal.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.LeaveFocusColor = System.Drawing.Color.White;
            this.txtTotal.LetterOnly = false;
            this.txtTotal.Location = new System.Drawing.Point(173, 3);
            this.txtTotal.MaxLength = 20;
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.NumericOnly = true;
            this.txtTotal.SelectionText = false;
            this.txtTotal.Size = new System.Drawing.Size(257, 30);
            this.txtTotal.TabIndex = 0;
            this.txtTotal.Tag = "";
            this.txtTotal.Text = "0";
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTotal.ThousandSeparator = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 35);
            this.label2.TabIndex = 1;
            this.label2.Text = "discount";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 35);
            this.label3.TabIndex = 2;
            this.label3.Text = "tax";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "Grand Total";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 35);
            this.label5.TabIndex = 2;
            this.label5.Text = "Pay Cash";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkPayViaCard
            // 
            this.chkPayViaCard.AutoSize = true;
            this.chkPayViaCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkPayViaCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPayViaCard.Location = new System.Drawing.Point(3, 188);
            this.chkPayViaCard.Name = "chkPayViaCard";
            this.chkPayViaCard.Size = new System.Drawing.Size(164, 31);
            this.chkPayViaCard.TabIndex = 5;
            this.chkPayViaCard.Text = "Pay via Card";
            this.chkPayViaCard.UseVisualStyleBackColor = true;
            this.chkPayViaCard.CheckedChanged += new System.EventHandler(this.chkPayViaCard_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(164, 35);
            this.label6.TabIndex = 2;
            this.label6.Text = "quantity";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 257);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(164, 35);
            this.label7.TabIndex = 2;
            this.label7.Text = "No. Card";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDiskon
            // 
            this.txtDiskon.AutoEnter = true;
            this.txtDiskon.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtDiskon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDiskon.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDiskon.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiskon.LeaveFocusColor = System.Drawing.Color.White;
            this.txtDiskon.LetterOnly = false;
            this.txtDiskon.Location = new System.Drawing.Point(173, 38);
            this.txtDiskon.MaxLength = 20;
            this.txtDiskon.Name = "txtDiskon";
            this.txtDiskon.NumericOnly = true;
            this.txtDiskon.SelectionText = true;
            this.txtDiskon.Size = new System.Drawing.Size(257, 30);
            this.txtDiskon.TabIndex = 1;
            this.txtDiskon.Tag = "";
            this.txtDiskon.Text = "0";
            this.txtDiskon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiskon.ThousandSeparator = true;
            // 
            // txtPPN
            // 
            this.txtPPN.AutoEnter = true;
            this.txtPPN.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtPPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPPN.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPPN.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPPN.LeaveFocusColor = System.Drawing.Color.White;
            this.txtPPN.LetterOnly = false;
            this.txtPPN.Location = new System.Drawing.Point(173, 73);
            this.txtPPN.MaxLength = 20;
            this.txtPPN.Name = "txtPPN";
            this.txtPPN.NumericOnly = true;
            this.txtPPN.SelectionText = true;
            this.txtPPN.Size = new System.Drawing.Size(257, 30);
            this.txtPPN.TabIndex = 2;
            this.txtPPN.Tag = "";
            this.txtPPN.Text = "0";
            this.txtPPN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPPN.ThousandSeparator = true;
            // 
            // txtGrandTotal
            // 
            this.txtGrandTotal.AutoEnter = true;
            this.txtGrandTotal.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtGrandTotal.Enabled = false;
            this.txtGrandTotal.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtGrandTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGrandTotal.LeaveFocusColor = System.Drawing.Color.White;
            this.txtGrandTotal.LetterOnly = false;
            this.txtGrandTotal.Location = new System.Drawing.Point(173, 108);
            this.txtGrandTotal.MaxLength = 20;
            this.txtGrandTotal.Name = "txtGrandTotal";
            this.txtGrandTotal.NumericOnly = true;
            this.txtGrandTotal.SelectionText = false;
            this.txtGrandTotal.Size = new System.Drawing.Size(257, 30);
            this.txtGrandTotal.TabIndex = 3;
            this.txtGrandTotal.Tag = "";
            this.txtGrandTotal.Text = "0";
            this.txtGrandTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotal.ThousandSeparator = true;
            // 
            // txtPayCash
            // 
            this.txtPayCash.AutoEnter = false;
            this.txtPayCash.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtPayCash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPayCash.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPayCash.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPayCash.LeaveFocusColor = System.Drawing.Color.White;
            this.txtPayCash.LetterOnly = false;
            this.txtPayCash.Location = new System.Drawing.Point(173, 153);
            this.txtPayCash.MaxLength = 20;
            this.txtPayCash.Name = "txtPayCash";
            this.txtPayCash.NumericOnly = true;
            this.txtPayCash.SelectionText = true;
            this.txtPayCash.Size = new System.Drawing.Size(257, 30);
            this.txtPayCash.TabIndex = 4;
            this.txtPayCash.Tag = "";
            this.txtPayCash.Text = "0";
            this.txtPayCash.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPayCash.ThousandSeparator = true;
            this.txtPayCash.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPayCash_KeyPress);
            // 
            // cmbCard
            // 
            this.cmbCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCard.Enabled = false;
            this.cmbCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCard.FormattingEnabled = true;
            this.cmbCard.Location = new System.Drawing.Point(173, 188);
            this.cmbCard.Name = "cmbCard";
            this.cmbCard.Size = new System.Drawing.Size(257, 33);
            this.cmbCard.TabIndex = 6;
            // 
            // txtPayCard
            // 
            this.txtPayCard.AutoEnter = true;
            this.txtPayCard.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtPayCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPayCard.Enabled = false;
            this.txtPayCard.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPayCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPayCard.LeaveFocusColor = System.Drawing.Color.White;
            this.txtPayCard.LetterOnly = false;
            this.txtPayCard.Location = new System.Drawing.Point(173, 225);
            this.txtPayCard.MaxLength = 20;
            this.txtPayCard.Name = "txtPayCard";
            this.txtPayCard.NumericOnly = true;
            this.txtPayCard.SelectionText = false;
            this.txtPayCard.Size = new System.Drawing.Size(257, 30);
            this.txtPayCard.TabIndex = 7;
            this.txtPayCard.Tag = "";
            this.txtPayCard.Text = "0";
            this.txtPayCard.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPayCard.ThousandSeparator = true;
            // 
            // txtNoCard
            // 
            this.txtNoCard.AutoEnter = true;
            this.txtNoCard.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNoCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNoCard.Enabled = false;
            this.txtNoCard.EnterFocusColor = System.Drawing.Color.White;
            this.txtNoCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoCard.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNoCard.LetterOnly = false;
            this.txtNoCard.Location = new System.Drawing.Point(173, 260);
            this.txtNoCard.MaxLength = 20;
            this.txtNoCard.Name = "txtNoCard";
            this.txtNoCard.NumericOnly = false;
            this.txtNoCard.SelectionText = false;
            this.txtNoCard.Size = new System.Drawing.Size(257, 30);
            this.txtNoCard.TabIndex = 8;
            this.txtNoCard.ThousandSeparator = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 302);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(164, 35);
            this.label8.TabIndex = 2;
            this.label8.Text = "Refund";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRefund
            // 
            this.txtRefund.AutoEnter = true;
            this.txtRefund.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtRefund.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRefund.Enabled = false;
            this.txtRefund.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtRefund.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRefund.LeaveFocusColor = System.Drawing.Color.White;
            this.txtRefund.LetterOnly = false;
            this.txtRefund.Location = new System.Drawing.Point(173, 305);
            this.txtRefund.MaxLength = 20;
            this.txtRefund.Name = "txtRefund";
            this.txtRefund.NumericOnly = true;
            this.txtRefund.SelectionText = false;
            this.txtRefund.Size = new System.Drawing.Size(257, 30);
            this.txtRefund.TabIndex = 3;
            this.txtRefund.Tag = "";
            this.txtRefund.Text = "0";
            this.txtRefund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRefund.ThousandSeparator = true;
            // 
            // FrmPay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 422);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmPay";
            this.Text = "FrmPay";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtTotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkPayViaCard;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private Helper.UserControl.AdvancedTextbox txtDiskon;
        private Helper.UserControl.AdvancedTextbox txtPPN;
        private Helper.UserControl.AdvancedTextbox txtGrandTotal;
        private Helper.UserControl.AdvancedTextbox txtPayCash;
        private System.Windows.Forms.ComboBox cmbCard;
        private Helper.UserControl.AdvancedTextbox txtPayCard;
        private Helper.UserControl.AdvancedTextbox txtNoCard;
        private System.Windows.Forms.Label label8;
        private Helper.UserControl.AdvancedTextbox txtRefund;
    }
}