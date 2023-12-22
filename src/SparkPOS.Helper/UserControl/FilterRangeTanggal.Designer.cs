namespace SparkPOS.Helper.UserControl
{
    partial class FilterRangeDate
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDateMulai = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpDateSelesai = new System.Windows.Forms.DateTimePicker();
            this.btnShow = new System.Windows.Forms.Button();
            this.chkShowAllData = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.dtpDateMulai);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.dtpDateSelesai);
            this.flowLayoutPanel1.Controls.Add(this.btnShow);
            this.flowLayoutPanel1.Controls.Add(this.chkShowAllData);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(469, 28);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 27);
            this.label1.TabIndex = 0;
            if (MainProgram.currentLanguage == "en-US")
            {
                this.label1.Text = "Date";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.label1.Text = "تاريخ";
            }
          
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpDateMulai
            // 
            this.dtpDateMulai.CustomFormat = "dd/MM/yyyy";
            this.dtpDateMulai.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateMulai.Location = new System.Drawing.Point(55, 3);
            this.dtpDateMulai.Name = "dtpDateMulai";
            this.dtpDateMulai.Size = new System.Drawing.Size(98, 20);
            this.dtpDateMulai.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(159, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 27);
            this.label2.TabIndex = 0;
            if (MainProgram.currentLanguage == "en-US")
            {
                this.label2.Text = "to";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.label2.Text = "ل";
            }

           
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpDateSelesai
            // 
            this.dtpDateSelesai.CustomFormat = "dd/MM/yyyy";
            this.dtpDateSelesai.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateSelesai.Location = new System.Drawing.Point(186, 3);
            this.dtpDateSelesai.Name = "dtpDateSelesai";
            this.dtpDateSelesai.Size = new System.Drawing.Size(98, 20);
            this.dtpDateSelesai.TabIndex = 1;
            // 
            // btnShow
            // 
            this.btnShow.Image = global::SparkPOS.Helper.Properties.Resources.search16;
            this.btnShow.Location = new System.Drawing.Point(290, 3);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(37, 21);
            this.btnShow.TabIndex = 2;
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // chkShowAllData
            // 
            this.chkShowAllData.AutoSize = true;
            this.chkShowAllData.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkShowAllData.Location = new System.Drawing.Point(333, 3);
            this.chkShowAllData.Name = "chkShowAllData";
            this.chkShowAllData.Size = new System.Drawing.Size(133, 21);
            this.chkShowAllData.TabIndex = 3;
            if (MainProgram.currentLanguage == "en-US")
            {
                this.chkShowAllData.Text = "Display all data";
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this.chkShowAllData.Text = "عرض جميع البيانات";
            }
           
            this.chkShowAllData.UseVisualStyleBackColor = true;
            this.chkShowAllData.CheckedChanged += new System.EventHandler(this.chkShowAllData_CheckedChanged);
            // 
            // FilterRangeDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "FilterRangeDate";
            this.Size = new System.Drawing.Size(469, 28);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDateMulai;
        private System.Windows.Forms.DateTimePicker dtpDateSelesai;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.CheckBox chkShowAllData;

    }
}
