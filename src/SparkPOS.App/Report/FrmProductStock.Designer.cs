namespace SparkPOS.App.Report
{
    partial class FrmProductStock
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
            this.cmbStatusStock = new System.Windows.Forms.ComboBox();
            this.rdoStatusStock = new System.Windows.Forms.RadioButton();
            this.rdoStockBasedProduct = new System.Windows.Forms.RadioButton();
            this.rdoStockLessFrom = new System.Windows.Forms.RadioButton();
            this.rdoStockBasedSupplier = new System.Windows.Forms.RadioButton();
            this.rdoStockBasedCategory = new System.Windows.Forms.RadioButton();
            this.txtStock = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.chkListOfProduct = new System.Windows.Forms.CheckedListBox();
            this.txtNameProduct = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 205F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.cmbStatusStock, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.rdoStatusStock, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.rdoStockBasedProduct, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.rdoStockLessFrom, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.rdoStockBasedSupplier, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.rdoStockBasedCategory, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtStock, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.cmbSupplier, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.cmbCategory, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 1, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 204F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(546, 308);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // cmbStatusStock
            // 
            this.cmbStatusStock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusStock.FormattingEnabled = true;
            this.cmbStatusStock.Items.AddRange(new object[] {
            "All",
            "there",
            "Empty"});
            this.cmbStatusStock.Location = new System.Drawing.Point(208, 3);
            this.cmbStatusStock.Name = "cmbStatusStock";
            this.cmbStatusStock.Size = new System.Drawing.Size(96, 21);
            this.cmbStatusStock.TabIndex = 1;
            // 
            // rdoStatusStock
            // 
            this.rdoStatusStock.AutoSize = true;
            this.rdoStatusStock.Checked = true;
            this.rdoStatusStock.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoStatusStock.Location = new System.Drawing.Point(3, 3);
            this.rdoStatusStock.Name = "rdoStatusStock";
            this.rdoStatusStock.Size = new System.Drawing.Size(78, 19);
            this.rdoStatusStock.TabIndex = 0;
            this.rdoStatusStock.TabStop = true;
            this.rdoStatusStock.Text = "Status stock";
            this.rdoStatusStock.UseVisualStyleBackColor = true;
            this.rdoStatusStock.CheckedChanged += new System.EventHandler(this.rdoStatusStock_CheckedChanged);
            // 
            // rdoStockBasedProduct
            // 
            this.rdoStockBasedProduct.AutoSize = true;
            this.rdoStockBasedProduct.Location = new System.Drawing.Point(3, 103);
            this.rdoStockBasedProduct.Name = "rdoStockBasedProduct";
            this.rdoStockBasedProduct.Size = new System.Drawing.Size(199, 17);
            this.rdoStockBasedProduct.TabIndex = 8;
            this.rdoStockBasedProduct.Text = "Stock based code/name product";
            this.rdoStockBasedProduct.UseVisualStyleBackColor = true;
            this.rdoStockBasedProduct.CheckedChanged += new System.EventHandler(this.rdoStockBasedProduct_CheckedChanged);
            // 
            // rdoStockLessFrom
            // 
            this.rdoStockLessFrom.AutoSize = true;
            this.rdoStockLessFrom.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoStockLessFrom.Location = new System.Drawing.Point(3, 28);
            this.rdoStockLessFrom.Name = "rdoStockLessFrom";
            this.rdoStockLessFrom.Size = new System.Drawing.Size(103, 19);
            this.rdoStockLessFrom.TabIndex = 2;
            this.rdoStockLessFrom.Text = "Stock less than";
            this.rdoStockLessFrom.UseVisualStyleBackColor = true;
            // 
            // rdoStockBasedSupplier
            // 
            this.rdoStockBasedSupplier.AutoSize = true;
            this.rdoStockBasedSupplier.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoStockBasedSupplier.Location = new System.Drawing.Point(3, 53);
            this.rdoStockBasedSupplier.Name = "rdoStockBasedSupplier";
            this.rdoStockBasedSupplier.Size = new System.Drawing.Size(148, 19);
            this.rdoStockBasedSupplier.TabIndex = 4;
            this.rdoStockBasedSupplier.Text = "Stock based supplier";
            this.rdoStockBasedSupplier.UseVisualStyleBackColor = true;
            // 
            // rdoStockBasedCategory
            // 
            this.rdoStockBasedCategory.AutoSize = true;
            this.rdoStockBasedCategory.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoStockBasedCategory.Location = new System.Drawing.Point(3, 78);
            this.rdoStockBasedCategory.Name = "rdoStockBasedCategory";
            this.rdoStockBasedCategory.Size = new System.Drawing.Size(156, 19);
            this.rdoStockBasedCategory.TabIndex = 6;
            this.rdoStockBasedCategory.Text = "Stock based category";
            this.rdoStockBasedCategory.UseVisualStyleBackColor = true;
            // 
            // txtStock
            // 
            this.txtStock.AutoEnter = true;
            this.txtStock.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtStock.EnterFocusColor = System.Drawing.Color.White;
            this.txtStock.LeaveFocusColor = System.Drawing.Color.White;
            this.txtStock.LetterOnly = false;
            this.txtStock.Location = new System.Drawing.Point(208, 28);
            this.txtStock.Name = "txtStock";
            this.txtStock.NumericOnly = true;
            this.txtStock.SelectionText = false;
            this.txtStock.Size = new System.Drawing.Size(48, 20);
            this.txtStock.TabIndex = 3;
            this.txtStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtStock.ThousandSeparator = false;
            // 
            // cmbSupplier
            // 
            this.cmbSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.FormattingEnabled = true;
            this.cmbSupplier.Location = new System.Drawing.Point(208, 53);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(335, 21);
            this.cmbSupplier.TabIndex = 5;
            // 
            // cmbCategory
            // 
            this.cmbCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(208, 78);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(335, 21);
            this.cmbCategory.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.chkListOfProduct);
            this.panel2.Controls.Add(this.txtNameProduct);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(205, 100);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(341, 204);
            this.panel2.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(303, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Enter";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkListOfProduct
            // 
            this.chkListOfProduct.CheckOnClick = true;
            this.chkListOfProduct.FormattingEnabled = true;
            this.chkListOfProduct.Location = new System.Drawing.Point(3, 31);
            this.chkListOfProduct.Name = "chkListOfProduct";
            this.chkListOfProduct.Size = new System.Drawing.Size(332, 169);
            this.chkListOfProduct.TabIndex = 10;
            // 
            // txtNameProduct
            // 
            this.txtNameProduct.AutoEnter = false;
            this.txtNameProduct.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNameProduct.EnterFocusColor = System.Drawing.Color.White;
            this.txtNameProduct.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNameProduct.LetterOnly = false;
            this.txtNameProduct.Location = new System.Drawing.Point(3, 5);
            this.txtNameProduct.Name = "txtNameProduct";
            this.txtNameProduct.NumericOnly = false;
            this.txtNameProduct.SelectionText = false;
            this.txtNameProduct.Size = new System.Drawing.Size(294, 20);
            this.txtNameProduct.TabIndex = 9;
            this.txtNameProduct.ThousandSeparator = false;
            this.txtNameProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNameProduct_KeyPress);
            // 
            // FrmProductStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 390);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmProductStock";
            this.Text = "FrmProductStock";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ComboBox cmbStatusStock;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtNameProduct;
        private System.Windows.Forms.RadioButton rdoStatusStock;
        private System.Windows.Forms.RadioButton rdoStockLessFrom;
        private System.Windows.Forms.RadioButton rdoStockBasedSupplier;
        private System.Windows.Forms.RadioButton rdoStockBasedCategory;
        private System.Windows.Forms.RadioButton rdoStockBasedProduct;
        private Helper.UserControl.AdvancedTextbox txtStock;
        private System.Windows.Forms.ComboBox cmbSupplier;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckedListBox chkListOfProduct;
    }
}