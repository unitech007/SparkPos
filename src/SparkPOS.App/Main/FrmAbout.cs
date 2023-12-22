/**
 * Copyright (C) 2017  (http://coding4ever.net/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * The latest version of this file can be found at https://github.com/rudi-krsoftware/spark-pos
 */

using System;
using System.Windows.Forms;
using SparkPOS.Helper;

namespace SparkPOS.App.Main
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
             InitializeComponent(); 
          //  MainProgram.GlobalLanguageChange(this);
            ColorManagerHelper.SetTheme(this, this);
            AboutMsg(this);
            ShowInfoAbout();
            MainProgram.GlobalLanguageChange(this);
        }

        private void ShowInfoAbout()
        {
            var firstReleaseYear = 2023;
            var currentYear = 2026;
            var copyright = currentYear > firstReleaseYear ? string.Format("{0} - {1}", firstReleaseYear, currentYear) : firstReleaseYear.ToString();
            if (MainProgram.currentLanguage == "en-US")
            {
                lblVersion.Text = string.Format("Version {0}{1}", MainProgram.currentVersion, MainProgram.stageOfDevelopment);
                lblCopyright.Text = string.Format("Copyright © {0}  | Email: emailtoishak@gmail.com | Phone: +91 97411 63733", copyright);


            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                 lblVersion.Text = string.Format("إصدار {0}{1}", MainProgram.currentVersion, MainProgram.stageOfDevelopment);
                lblCopyright.Text = string.Format("حقوق النشر © {0}  | Email: emailtoishak@gmail.com | Phone: +91 97411 63733", copyright);


            }

            lblUrl1.Text = "https://github.com/rudi-krsoftware/spark-pos";
            lblUrl1.LinkClicked += lblUrl_LinkClicked;

            lblUrl2.Text = "http://coding4ever.net/";
            lblUrl2.LinkClicked += lblUrl_LinkClicked;
        }

        private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = (LinkLabel)sender;

            // Specify that the link was visited.
            link.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start(link.Text);
        }

        public static void AboutMsg(FrmAbout this1)
        {
            if (MainProgram.currentLanguage == "en-US")
            {
                this1.Text = "About";
                this1.label1.Text = "SparkPOS is a cutting-edge accounting software solution that encompasses all aspe" +
      "cts of financial management, empowering businesses with unparalleled control and" +
      " efficiency.\r\n-)\r\n";

            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                this1.Text = "عن";
                this1.label1.Text = "سبارك بوس هو حل برمجي محاسبي حديث يشمل جميع جوانب إدارة المال، مما يمنح الشركات القدرة والكفاءة غير المسبوقة.\r\n-)\r\n";

            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmAbout_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                btnOk_Click(sender, e);
        }

        private void imgDonate_Click(object sender, EventArgs e)
        {
            var url = "https://github.com/rudi-krsoftware/spark-pos/wiki/Cara-Berkontribusi/";

            // Navigate to a URL.
            System.Diagnostics.Process.Start(url);
        }

        private void lblVersion_Click(object sender, EventArgs e)
        {

        }

        private void FrmAbout_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
