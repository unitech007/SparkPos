namespace SparkPOS.App.Reference
{
    partial class FrmEntryCard
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
            this.txtNameCard = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoCardDebit = new System.Windows.Forms.RadioButton();
            this.rdoCardKredit = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtNameCard, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(347, 52);
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
            this.label1.Text = "Name Card";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNameCard
            // 
            this.txtNameCard.AutoEnter = true;
            this.txtNameCard.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNameCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNameCard.EnterFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtNameCard.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNameCard.LetterOnly = false;
            this.txtNameCard.Location = new System.Drawing.Point(69, 3);
            this.txtNameCard.Name = "txtNameCard";
            this.txtNameCard.NumericOnly = false;
            this.txtNameCard.SelectionText = false;
            this.txtNameCard.Size = new System.Drawing.Size(275, 20);
            this.txtNameCard.TabIndex = 0;
            this.txtNameCard.Tag = "name_category";
            this.txtNameCard.ThousandSeparator = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Type";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.rdoCardDebit);
            this.flowLayoutPanel1.Controls.Add(this.rdoCardKredit);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(66, 25);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(281, 25);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // rdoCardDebit
            // 
            this.rdoCardDebit.AutoSize = true;
            this.rdoCardDebit.Checked = true;
            this.rdoCardDebit.Location = new System.Drawing.Point(3, 3);
            this.rdoCardDebit.Name = "rdoCardDebit";
            this.rdoCardDebit.Size = new System.Drawing.Size(75, 17);
            this.rdoCardDebit.TabIndex = 0;
            this.rdoCardDebit.TabStop = true;
            this.rdoCardDebit.Text = "Debit Card";
            this.rdoCardDebit.UseVisualStyleBackColor = true;
            // 
            // rdoCardKredit
            // 
            this.rdoCardKredit.AutoSize = true;
            this.rdoCardKredit.Location = new System.Drawing.Point(84, 3);
            this.rdoCardKredit.Name = "rdoCardKredit";
            this.rdoCardKredit.Size = new System.Drawing.Size(77, 17);
            this.rdoCardKredit.TabIndex = 1;
            this.rdoCardKredit.Text = "Credit Card";
            this.rdoCardKredit.UseVisualStyleBackColor = true;
            // 
            // FrmEntryCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 134);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmEntryCard";
            this.Text = "FrmEntryCard";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtNameCard;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rdoCardDebit;
        private System.Windows.Forms.RadioButton rdoCardKredit;
    }
}