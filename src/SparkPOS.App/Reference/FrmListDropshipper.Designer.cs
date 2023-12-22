namespace SparkPOS.App.Reference
{
    partial class FrmListDropshipper
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtNameDropshipper = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.gridList = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.gridList, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(820, 359);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnFind);
            this.flowLayoutPanel1.Controls.Add(this.txtNameDropshipper);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(820, 28);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnFind
            // 
            this.btnFind.Image = global::SparkPOS.App.Properties.Resources.search16;
            this.btnFind.Location = new System.Drawing.Point(780, 0);
            this.btnFind.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(37, 23);
            this.btnFind.TabIndex = 1;
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtNameDropshipper
            // 
            this.txtNameDropshipper.AutoEnter = false;
            this.txtNameDropshipper.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNameDropshipper.EnterFocusColor = System.Drawing.Color.White;
            this.txtNameDropshipper.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNameDropshipper.LetterOnly = false;
            this.txtNameDropshipper.Location = new System.Drawing.Point(552, 3);
            this.txtNameDropshipper.Name = "txtNameDropshipper";
            this.txtNameDropshipper.NumericOnly = false;
            this.txtNameDropshipper.SelectionText = false;
            this.txtNameDropshipper.Size = new System.Drawing.Size(222, 20);
            this.txtNameDropshipper.TabIndex = 0;
            this.txtNameDropshipper.Text = "Find name dropshipper ...";

            this.txtNameDropshipper.ThousandSeparator = false;
            this.txtNameDropshipper.Enter += new System.EventHandler(this.txtNameDropshipper_Enter);
            this.txtNameDropshipper.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNameDropshipper_KeyPress);
            this.txtNameDropshipper.Leave += new System.EventHandler(this.txtNameDropshipper_Leave);
            // 
            // gridList
            // 
            this.gridList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gridList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridList.ItemHeight = 17;
            this.gridList.Location = new System.Drawing.Point(3, 31);
            this.gridList.Name = "gridList";
            this.gridList.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridList.SelectedIndex = -1;
            this.gridList.Size = new System.Drawing.Size(814, 325);
            this.gridList.TabIndex = 0;
            this.gridList.TopIndex = 0;
            this.gridList.DoubleClick += new System.EventHandler(this.gridList_DoubleClick);
            // 
            // FrmListDropshipper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 441);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmListDropshipper";
            this.Text = "FrmListDropshipper";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnFind;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtNameDropshipper;
        private Syncfusion.Windows.Forms.Grid.GridListControl gridList;
    }
}