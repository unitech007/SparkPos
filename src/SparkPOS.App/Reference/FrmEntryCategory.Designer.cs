namespace SparkPOS.App.Reference
{
    partial class FrmEntryCategory
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
            this.txtCategory = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDiskon = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProfit = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtCategory, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtDiskon, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtProfit, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(347, 79);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Category";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtCategory
            // 
            this.txtCategory.AutoEnter = true;
            this.txtCategory.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCategory.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtCategory.LeaveFocusColor = System.Drawing.Color.White;
            this.txtCategory.LetterOnly = false;
            this.txtCategory.Location = new System.Drawing.Point(58, 3);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.NumericOnly = false;
            this.txtCategory.SelectionText = false;
            this.txtCategory.Size = new System.Drawing.Size(286, 20);
            this.txtCategory.TabIndex = 0;
            this.txtCategory.Tag = "name_category";
            this.txtCategory.ThousandSeparator = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "discount";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDiskon
            // 
            this.txtDiskon.AutoEnter = false;
            this.txtDiskon.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtDiskon.EnterFocusColor = System.Drawing.Color.White;
            this.txtDiskon.LeaveFocusColor = System.Drawing.Color.White;
            this.txtDiskon.LetterOnly = false;
            this.txtDiskon.Location = new System.Drawing.Point(58, 53);
            this.txtDiskon.MaxLength = 5;
            this.txtDiskon.Name = "txtDiskon";
            this.txtDiskon.NumericOnly = true;
            this.txtDiskon.SelectionText = false;
            this.txtDiskon.Size = new System.Drawing.Size(40, 20);
            this.txtDiskon.TabIndex = 2;
            this.txtDiskon.Text = "0";
            this.txtDiskon.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDiskon.ThousandSeparator = false;
            this.txtDiskon.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiskon_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Profit (%)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtProfit
            // 
            this.txtProfit.AutoEnter = true;
            this.txtProfit.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtProfit.EnterFocusColor = System.Drawing.Color.White;
            this.txtProfit.LeaveFocusColor = System.Drawing.Color.White;
            this.txtProfit.LetterOnly = false;
            this.txtProfit.Location = new System.Drawing.Point(58, 28);
            this.txtProfit.MaxLength = 5;
            this.txtProfit.Name = "txtProfit";
            this.txtProfit.NumericOnly = true;
            this.txtProfit.SelectionText = false;
            this.txtProfit.Size = new System.Drawing.Size(40, 20);
            this.txtProfit.TabIndex = 1;
            this.txtProfit.Text = "0";
            this.txtProfit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtProfit.ThousandSeparator = false;
            // 
            // FrmEntryCategory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 161);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmEntryCategory";
            this.Text = "FrmEntryCategory";
            this.Load += new System.EventHandler(this.FrmEntryCategory_Load);
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtCategory;
        private System.Windows.Forms.Label label2;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtDiskon;
        private System.Windows.Forms.Label label3;
        private Helper.UserControl.AdvancedTextbox txtProfit;
    }
}