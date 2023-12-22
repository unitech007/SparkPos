namespace SparkPOS.App.Settings
{
    partial class FrmCompanyProfile
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNameCompany = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtAddress = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtCity = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtphone = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmail = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.txtWebsite = new SparkPOS.Helper.UserControl.AdvancedTextbox();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtNameCompany, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtAddress, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtCity, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtphone, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.txtEmail, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.txtWebsite, 1, 5);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 41);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 7;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(439, 152);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name Company";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Address";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "City";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 25);
            this.label4.TabIndex = 1;
            this.label4.Text = "phone";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNameCompany
            // 
            this.txtNameCompany.AutoEnter = true;
            this.txtNameCompany.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtNameCompany.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNameCompany.EnterFocusColor = System.Drawing.Color.White;
            this.txtNameCompany.LeaveFocusColor = System.Drawing.Color.White;
            this.txtNameCompany.LetterOnly = false;
            this.txtNameCompany.Location = new System.Drawing.Point(104, 3);
            this.txtNameCompany.Name = "txtNameCompany";
            this.txtNameCompany.NumericOnly = false;
            this.txtNameCompany.SelectionText = false;
            this.txtNameCompany.Size = new System.Drawing.Size(332, 20);
            this.txtNameCompany.TabIndex = 0;
            this.txtNameCompany.Tag = "name_profile";
            this.txtNameCompany.ThousandSeparator = false;
            // 
            // txtAddress
            // 
            this.txtAddress.AutoEnter = true;
            this.txtAddress.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.EnterFocusColor = System.Drawing.Color.White;
            this.txtAddress.LeaveFocusColor = System.Drawing.Color.White;
            this.txtAddress.LetterOnly = false;
            this.txtAddress.Location = new System.Drawing.Point(104, 28);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.NumericOnly = false;
            this.txtAddress.SelectionText = false;
            this.txtAddress.Size = new System.Drawing.Size(332, 20);
            this.txtAddress.TabIndex = 1;
            this.txtAddress.Tag = "address";
            this.txtAddress.ThousandSeparator = false;
            // 
            // txtCity
            // 
            this.txtCity.AutoEnter = true;
            this.txtCity.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtCity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCity.EnterFocusColor = System.Drawing.Color.White;
            this.txtCity.LeaveFocusColor = System.Drawing.Color.White;
            this.txtCity.LetterOnly = false;
            this.txtCity.Location = new System.Drawing.Point(104, 53);
            this.txtCity.Name = "txtCity";
            this.txtCity.NumericOnly = false;
            this.txtCity.SelectionText = false;
            this.txtCity.Size = new System.Drawing.Size(332, 20);
            this.txtCity.TabIndex = 2;
            this.txtCity.Tag = "city";
            this.txtCity.ThousandSeparator = false;
            // 
            // txtphone
            // 
            this.txtphone.AutoEnter = true;
            this.txtphone.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtphone.EnterFocusColor = System.Drawing.Color.White;
            this.txtphone.LeaveFocusColor = System.Drawing.Color.White;
            this.txtphone.LetterOnly = false;
            this.txtphone.Location = new System.Drawing.Point(104, 78);
            this.txtphone.Name = "txtphone";
            this.txtphone.NumericOnly = false;
            this.txtphone.SelectionText = false;
            this.txtphone.Size = new System.Drawing.Size(123, 20);
            this.txtphone.TabIndex = 3;
            this.txtphone.Tag = "phone";
            this.txtphone.ThousandSeparator = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 25);
            this.label5.TabIndex = 4;
            this.label5.Text = "Email";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 25);
            this.label6.TabIndex = 4;
            this.label6.Text = "Website";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEmail
            // 
            this.txtEmail.AutoEnter = true;
            this.txtEmail.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEmail.EnterFocusColor = System.Drawing.Color.White;
            this.txtEmail.LeaveFocusColor = System.Drawing.Color.White;
            this.txtEmail.LetterOnly = false;
            this.txtEmail.Location = new System.Drawing.Point(104, 103);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.NumericOnly = false;
            this.txtEmail.SelectionText = false;
            this.txtEmail.Size = new System.Drawing.Size(332, 20);
            this.txtEmail.TabIndex = 4;
            this.txtEmail.Tag = "email";
            this.txtEmail.ThousandSeparator = false;
            // 
            // txtWebsite
            // 
            this.txtWebsite.AutoEnter = false;
            this.txtWebsite.Conversion = SparkPOS.Helper.UserControl.EConversion.Normal;
            this.txtWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWebsite.EnterFocusColor = System.Drawing.Color.White;
            this.txtWebsite.LeaveFocusColor = System.Drawing.Color.White;
            this.txtWebsite.LetterOnly = false;
            this.txtWebsite.Location = new System.Drawing.Point(104, 128);
            this.txtWebsite.Name = "txtWebsite";
            this.txtWebsite.NumericOnly = false;
            this.txtWebsite.SelectionText = false;
            this.txtWebsite.Size = new System.Drawing.Size(332, 20);
            this.txtWebsite.TabIndex = 5;
            this.txtWebsite.Tag = "website";
            this.txtWebsite.ThousandSeparator = false;
            this.txtWebsite.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWebsite_KeyPress);
            // 
            // FrmCompanyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 234);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "FrmCompanyProfile";
            this.Text = "FrmCompanyProfile";
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtNameCompany;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtAddress;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtCity;
        private SparkPOS.Helper.UserControl.AdvancedTextbox txtphone;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Helper.UserControl.AdvancedTextbox txtEmail;
        private Helper.UserControl.AdvancedTextbox txtWebsite;

    }
}