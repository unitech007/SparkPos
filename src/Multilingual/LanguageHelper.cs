using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using Syncfusion.Windows.Forms.Grid;
using Label = System.Windows.Forms.Label;
//using SparkPOS.Helper.UserControl;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using Microsoft.Reporting.WinForms;
using SparkPOS.Helper.UserControl;
using Syncfusion.Windows.Forms.Grid;
using SparkPOS.Helper;
using System.Collections;
//using Syncfusion.Windows.Forms.Grid.GridStyles;


namespace MultilingualApp
{
    public class LanguageHelper
    {
        //private static IList<GridListControlProperties> oglProperty;
        public static void ToKurdish(Control control, string kurdishText, float fontSize = 10)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.RightToLeft = RightToLeft.Yes;
                    textBox.Font = new Font("Calibri", fontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                    break;
                case Label label:
                    label.Text = kurdishText;
                    label.RightToLeft = RightToLeft.Yes;
                    label.Font = new Font("Calibri", fontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                    break;
                case Button button:
                    button.Text = kurdishText;
                    button.RightToLeft = RightToLeft.Yes;
                    button.Font = new Font("Calibri", fontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                    break;
            }
        }

        private static bool IsEnglish(string stringToCheck)
        {
            bool returnValue = false;
            if (Regex.IsMatch(stringToCheck, "^[a-zA-Z0-9. -_?|]*$", RegexOptions.Compiled))
            {
                returnValue = true;
            }

            return returnValue;
        }

        //private static bool IsEnglish(object valueToCheck)
        //{
        //    if (valueToCheck != null)
        //    {
        //        string stringToCheck = valueToCheck.ToString();
        //        if (Regex.IsMatch(stringToCheck, "^[a-zA-Z0-9. -_?]*$", RegexOptions.Compiled))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //public static void ChangeLanguage(Control control, string languageToLoad)
        //{
        //    ResourceManager rm = new ResourceManager("MultilingualApp." + languageToLoad, Assembly.GetExecutingAssembly());

        //    ProcessControl(control, rm, languageToLoad);
        //}

        private static ResourceManager GetResourceManager(string lanaguageToLoad)
        {
            ResourceManager rm = new ResourceManager("MultilingualApp." + lanaguageToLoad,
                    Assembly.GetExecutingAssembly());
            return rm;
        }
        public static void ChangeLanguage(Form frmToChange, string languageToLoad, float fontSize = 10)
        {
            if (frmToChange != null)
            {
                ResourceManager rm = GetResourceManager(languageToLoad);
                //String strWebsite = rm.GetString("Website", CultureInfo.CurrentCulture);
                //String strName = rm.GetString("Name");

                ProcessControl(frmToChange, rm, languageToLoad);
                ProcessForm(frmToChange, rm, languageToLoad);
                //  ProcessNameForm(frmToChange, rm, languageToLoad);
                // var report = new LocalReport();
                //   ProcessRDLCReport(report,rm, languageToLoad);
            }
        }

        private static void ProcessForm(Control control, ResourceManager rm, string languageToLoad)
        {
            foreach (Control childControl in control.Controls)
            {
                ProcessControl(childControl, rm, languageToLoad);
            }
        }
        //
        //private static void ProcessNameForm(Form form, ResourceManager rm, string languageToLoad)
        //{
        //    // form.RightToLeft = languageToLoad == "ar-SA" ? RightToLeft.Yes : RightToLeft.No;
        //    string formName = IsEnglish(form.Name) ? form.Name : (string)form.Tag;
        //    // form.Tag = formName;

        //    //if (languageToLoad == "ar-SA")
        //    //{
        //    //    form.Text = rm.GetString(formName);
        //    //}
        //    //else if (languageToLoad == "en-US")
        //    //{
        //    //    form.Text = rm.GetString(formName);
        //    //}

        //    foreach (Control control in form.Controls)
        //    {
        //        ProcessControl(control, rm, languageToLoad);
        //    }
        //}



        private static void ProcessControl(Control control, ResourceManager rm, string languageToLoad)
        {
            try
            {

                if (control is SparkPOS.Helper.UserControl.AdvancedTextbox textbox)
                {
                    ProcessAdvancedTextbox(textbox, rm, languageToLoad);
                }

                switch (control)
                {
                    case TextBox textBox:
                        ProcessTextBox(textBox, languageToLoad);
                        break;

                    case Label label:
                        ProcessLabel(label, rm, languageToLoad);
                        break;



                    case RadioButton radioButton:
                        ProcessRadioButton(radioButton, rm, languageToLoad);
                        break;



                    case Button button:
                        ProcessButton(button, rm, languageToLoad);
                        break;

                    case TableLayoutPanel tableLayoutPanel:
                        ProcessTableLayoutPanel(tableLayoutPanel, rm, languageToLoad);
                        break;

                    case FlowLayoutPanel flowLayoutPanel:
                        ProcessFlowLayoutPanel(flowLayoutPanel, rm, languageToLoad);
                        // ProcessAdvancedTextbox(flowLayoutPanel, rm, languageToLoad);
                        break;

                    case GridControl gridControl1:
                        ProcessGridControl(gridControl1, rm, languageToLoad);
                        break;
                    case Panel panel:
                        Processpanel(panel, rm, languageToLoad);
                        break;
                    case TabControl tabControl:
                        ProcessTabControl(tabControl, rm, languageToLoad);
                        break;
                    case GroupBox groupBox:
                        ProcessGroupBox(groupBox, rm, languageToLoad);
                        break;

                    case TreeViewAdv treeView:
                        //  if(languageToLoad == "ar-SA")
                        ProcessTreeViewAdv(treeView, rm, languageToLoad);
                        break;

                    case ComboBox comboBox:
                        ProcessComboBox(comboBox, rm, languageToLoad);
                        break;

                    case FilterRangeDate filterRangeDate:
                        ProcessFilterRangeDate(filterRangeDate, rm, languageToLoad);
                        break;
                    case CheckBox checkBox:
                        ProcessCheckBox(checkBox, rm, languageToLoad);
                        break;
                    case MenuStrip menuStrip:
                        ProcessMenuStrip(menuStrip, rm, languageToLoad);
                        break;
                    case ToolStrip toolStrip:
                        ProcessToolStrip(toolStrip, rm, languageToLoad);
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred while processing control '{control.Name}': {ex.InnerException}");
                // You can add additional error handling or logging logic as needed
            }
        }

        private static void ProcessToolStrip(ToolStrip toolStrip, ResourceManager rm, string languageToLoad)
        {
            if (languageToLoad == "ar-SA")
            {
                toolStrip.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                toolStrip.RightToLeft = RightToLeft.No;
            }

            // Translate ToolStrip items
            foreach (ToolStripItem item in toolStrip.Items)
            {
                string translation = rm.GetString(item.Text, new CultureInfo(languageToLoad));
                if (!string.IsNullOrEmpty(translation))
                {
                    item.Text = translation;
                }
            }
        }


        //    private static void ProcessComboBox(ComboBox comboBox, ResourceManager rm, string languageToLoad)
        //    {
        //        List<string> valuesToTranslate = new List<string>
        //{
        //    "Reference", "Transactions","Settings", "Report","Expense",
        //    "Name Product", "اسم المنتج",
        //    "Code Product",
        //    "-- All --", "General",
        //            "Reseller",  
        //           "Select",
        //           "Fax","Find Supplier ...","... Find Supplier",
        //    // Add other values here
        //};

        //        foreach (var item in comboBox.Items)
        //        {
        //            if (item is string value && valuesToTranslate.Contains(value))
        //            {
        //                string translatedText = rm.GetString(value);
        //                if (translatedText != null)
        //                {
        //                    // Replace the original value with the translated text
        //                    int index = comboBox.Items.IndexOf(item);
        //                    comboBox.Items[index] = translatedText;
        //                }
        //            }
        //        }
        //    }

        private static void ProcessComboBox(ComboBox comboBox, ResourceManager rm, string languageToLoad)
        {
            List<string> valuesToTranslate = new List<string>
    {
        "Reference", "Transactions", "Settings", "Report", "Expense",
        "Name Product", "اسم المنتج",
        "Code Product",
        "-- All --", "General",
        "Reseller",
        "Select",
        "Fax", "Find Supplier ...", "... Find Supplier",
        // Add other values here
    };

            List<string> itemsToUpdate = new List<string>();

            foreach (var item in comboBox.Items)
            {
                if (item is string value && valuesToTranslate.Contains(value))
                {
                    string translatedText = rm.GetString(value);
                    if (translatedText != null)
                    {
                        // Add the translated text and its index to the list
                        itemsToUpdate.Add(translatedText);
                    }
                }
            }

            // Update the ComboBox items with the translated values
            for (int i = 0; i < itemsToUpdate.Count; i++)
            {
                comboBox.Items[i] = itemsToUpdate[i];
            }
        }

        //private static void ProcessAdvancedTextbox(SparkPOS.Helper.UserControl.AdvancedTextbox textbo, ResourceManager rm, string languageToLoad)
        //{
        //    if (languageToLoad == "ar-SA")
        //    {
        //        textbo.Text = rm.GetString(textbo.Name);
        //    }
        //    else if (languageToLoad == "en-US")
        //    {
        //        // Use the original English text for now
        //    }
        //}
        private static void ProcessAdvancedTextbox(SparkPOS.Helper.UserControl.AdvancedTextbox textbox, ResourceManager rm, string languageToLoad)
        {
            List<string> valuesToMaintain = new List<string>
    {
        "admin", // Add other values to maintain here
        "localhost"
    };

            if (languageToLoad == "ar-SA")
            {
                if (!valuesToMaintain.Contains(textbox.Text))
                {
                    string translatedText = rm.GetString(textbox.Text);
                    if (translatedText != null)
                    {
                        // Set the translated text
                        textbox.Text = translatedText;
                    }
                }
            }
            else if (languageToLoad == "en-US")
            {
                // Use the original English text for now
            }
        }


        public static string TranslateText(string text, string languageToLoad)
        {
            ResourceManager rm = GetResourceManager(languageToLoad);
            if (!string.IsNullOrEmpty(text))
            {
                string translatedText = rm.GetString(string.Format(text));
                return translatedText ?? text; // Use the translated text if available, otherwise fallback to the original text
            }

            return text;
        }

        public static string TranslateWarningMessages(string message, string languageToLoad)
        {
            ResourceManager rm = GetResourceManager(languageToLoad);
            // Translate the message
            string translatedMessage = rm.GetString(message, new CultureInfo(languageToLoad));
            if (!string.IsNullOrEmpty(translatedMessage))
            {
                return translatedMessage;
            }

            // If the translation is not available, return the original message
            return message;
        }

        public static string ShowTranslatedWarnings(string message, string languageToLoad)
        {
            string translatedMsg = TranslateWarningMessages(message, languageToLoad);
            return translatedMsg;
        }
        public static string GetTranslatedAddData(string languageToLoad)
        {
            string addData = "Add Data";
            var translateData = TranslateText(addData, languageToLoad);
            if (translateData == "Add Data")
                translateData = "Add Data ";
            return translateData;
        }


        public static string GetTranslatedEditData(string languageToLoad)
        {
            string addData = "Edit Data";
            // return TranslateText(addData, languageToLoad);
            var translateData = TranslateText(addData, languageToLoad);
            if (translateData == "Edit Data")
                translateData = "Edit Data ";
            return translateData;
        }


        public static void TranslateGridListControlHeaders(List<GridListControlProperties> properties, string languageToLoad)
        {
            ResourceManager rm = GetResourceManager(languageToLoad);
            foreach (var prop in properties)
            {
                string headerText = prop.Header;
                //prop.HorizontalAlignment = (GridHorizontalAlignment)RightToLeft.Yes;
                string localizedHeaderText = rm.GetString(headerText, new CultureInfo(languageToLoad));

                prop.Header = localizedHeaderText;
                if (languageToLoad == "ar-SA")
                {
                    prop.HorizontalAlignment = GridHorizontalAlignment.Right;
                }
                else if (languageToLoad == "en-US")
                {
                    prop.HorizontalAlignment = GridHorizontalAlignment.Center;
                }
            }


        }





        private static void ProcessMenuStrip(MenuStrip menuStrip, ResourceManager rm, string languageToLoad)
        {
            foreach (ToolStripMenuItem menuItem in menuStrip.Items)
            {
                ProcessToolStripMenuItem(menuItem, rm, languageToLoad);
            }
        }

        private static void ProcessToolStripMenuItem(ToolStripMenuItem menuItem, ResourceManager rm, string languageToLoad)
        {
            string menuItemText = IsEnglish(menuItem.Text) ? menuItem.Text : (string)menuItem.Tag;
            //menuItem.Tag = menuItemText;

            if (languageToLoad == "ar-SA")
            {
                menuItem.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                menuItem.RightToLeft = RightToLeft.No;
            }

            if (menuItemText != null)
            {
                string translatedText = rm.GetString(menuItemText);
                menuItem.Text = translatedText ?? menuItemText; // Use the translated text if available, otherwise fallback to the original text

                // Process sub-items recursively
                foreach (ToolStripItem subItem in menuItem.DropDownItems)
                {
                    if (subItem is ToolStripMenuItem subMenuItem)
                    {
                        ProcessToolStripMenuItem(subMenuItem, rm, languageToLoad);
                    }
                }
            }
        }

        //private static void ProcessToolStripMenuItem(ToolStripMenuItem menuItem, ResourceManager rm, string languageToLoad)
        //{
        //    string menuItemText = IsEnglish(menuItem.Text) ? menuItem.Text : (string)menuItem.Tag;
        //    //menuItem.Tag = menuItemText;

        //    if (languageToLoad == "ar-SA")
        //    {
        //        menuItem.RightToLeft = RightToLeft.Yes;

        //        // Check if the menuItem is the mainDock item
        //        if (menuItemText == "mainDock")
        //        {
        //            Control[] dockPanelControls = menuItem.GetCurrentParent().Controls.Find(menuItemText, true);
        //            if (dockPanelControls.Length > 0)
        //            {
        //                DockPanel mainDock = (DockPanel)dockPanelControls[0];
        //                mainDock.RightToLeftLayout = true;
        //            }
        //        }
        //    }
        //    else if (languageToLoad == "en-US")
        //    {
        //        menuItem.RightToLeft = RightToLeft.No;

        //        // Check if the menuItem is the mainDock item
        //        if (menuItemText == "mainDock")
        //        {
        //            Control[] dockPanelControls = menuItem.GetCurrentParent().Controls.Find(menuItemText, true);
        //            if (dockPanelControls.Length > 0)
        //            {
        //                DockPanel mainDock = (DockPanel)dockPanelControls[0];
        //                mainDock.RightToLeftLayout = false;
        //            }
        //        }
        //    }

        //    if (menuItemText != null)
        //    {
        //        string translatedText = rm.GetString(menuItemText);
        //        menuItem.Text = translatedText ?? menuItemText; // Use the translated text if available, otherwise fallback to the original text

        //        // Process sub-items recursively
        //        foreach (ToolStripItem subItem in menuItem.DropDownItems)
        //        {
        //            if (subItem is ToolStripMenuItem subMenuItem)
        //            {
        //                ProcessToolStripMenuItem(subMenuItem, rm, languageToLoad);
        //            }
        //        }
        //    }
        //}


        private static void ProcessCheckBox(CheckBox checkBox, ResourceManager rm, string languageToLoad)
        {
            string checkBoxText = IsEnglish(checkBox.Text) ? checkBox.Text : (string)checkBox.Tag;
            checkBox.Tag = checkBoxText;

            if (languageToLoad == "ar-SA")
            {
                checkBox.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                checkBox.RightToLeft = RightToLeft.No;
            }

            if (checkBoxText != null)
            {
                string translatedText = rm.GetString(checkBoxText);
                checkBox.Text = translatedText ?? checkBoxText; // Use the translated text if available, otherwise fallback to the original text
            }
        }
        private static void ProcessTextBox(TextBox textBox, string languageToLoad)
        {
            if (languageToLoad == "ar-SA")
            {
                textBox.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                textBox.RightToLeft = RightToLeft.No;
            }
        }



        private static void ProcessTreeViewAdv(TreeViewAdv treeView, ResourceManager rm, string languageToLoad)
        {
            // Implement translation logic for TreeViewAdv control
            if (languageToLoad == "ar-SA" || languageToLoad == "en-US")
            {
                TranslateTreeViewNodes(treeView.Nodes, rm, languageToLoad);
            }
        }

        private static void TranslateTreeViewNodes(TreeNodeAdvCollection nodes, ResourceManager rm, string languageToLoad)
        {
            foreach (TreeNodeAdv node in nodes)
            {
                // Check if translation is available for the current node's text
                string translatedText = rm.GetString(node.Text, new CultureInfo(languageToLoad));
                if (!string.IsNullOrEmpty(translatedText))
                {
                    // Translation is available, assign the translated text
                    node.Text = translatedText;
                }

                // Translate child nodes recursively
                if (node.Nodes.Count > 0)
                {
                    ProcessChildNodes(node.Nodes, rm, languageToLoad);
                }
            }
        }

        private static void ProcessChildNodes(TreeNodeAdvCollection nodes, ResourceManager rm, string languageToLoad)
        {
            foreach (TreeNodeAdv node in nodes)
            {
                // Check if translation is available for the child node's text
                string translatedText = rm.GetString(node.Text, new CultureInfo(languageToLoad));
                if (!string.IsNullOrEmpty(translatedText))
                {
                    // Translation is available, assign the translated text
                    node.Text = translatedText;
                }

                // Translate child nodes recursively
                if (node.Nodes.Count > 0)
                {
                    ProcessChildNodes(node.Nodes, rm, languageToLoad);
                }
            }
        }


        private static void ProcessFilterRangeDate(FilterRangeDate filterRangeDate, ResourceManager rm, string languageToLoad)
        {
            // Implement translation logic for FilterRangeDate control
            if (languageToLoad == "ar-SA")
            {
                filterRangeDate.Text = rm.GetString(filterRangeDate.Name);
            }
            else if (languageToLoad == "en-US")
            {
                // Use the original English text for now
            }
        }


        //private static void ProcessLabel(Label label, ResourceManager rm, string languageToLoad)
        //{
        //    string lblText = IsEnglish(label.Text) ? label.Text : (string)label.Tag;
        //    label.Tag = lblText;

        //    if (languageToLoad == "ar-SA")
        //    {
        //        label.RightToLeft = RightToLeft.Yes;
        //    }
        //    else if (languageToLoad == "en-US")
        //    {
        //        label.RightToLeft = RightToLeft.No;
        //    }
        //    string translatedText;
        //    if (lblText != null)
        //    {
        //        translatedText = rm.GetString(lblText);
        //        label.Text = translatedText ?? lblText; // Use the translated text if available, otherwise fallback to the original text
        //    }
        //    //if (string.IsNullOrEmpty(label.Text))
        //    //{
        //    //    //headerText.ToString().Toti
        //    //    translatedText = rm.GetString(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(lblText.ToLower()), new CultureInfo(languageToLoad));
        //    //}
        //}

        private static void ProcessLabel(Label label, ResourceManager rm, string languageToLoad)
        {
            string lblText = IsEnglish(label.Text) ? label.Text : (string)label.Tag;
            label.Tag = lblText;

            if (languageToLoad == "ar-SA")
            {
                label.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                label.RightToLeft = RightToLeft.No;
            }

            string translatedText = rm.GetString(lblText); // Try direct localization
            if (translatedText == null)
            {
                StringComparison comparison = StringComparison.OrdinalIgnoreCase; // Case-insensitive comparison
                CultureInfo culture = new CultureInfo(languageToLoad);

                bool foundLocalization = false;
                foreach (DictionaryEntry entry in rm.GetResourceSet(culture, true, true))
                {
                    string key = entry.Key as string;
                    if (string.Equals(lblText, key, comparison))
                    {
                        translatedText = entry.Value as string;
                        foundLocalization = true;
                        break;
                    }
                }

                if (!foundLocalization)
                {
                    translatedText = lblText;
                }
            }

            label.Text = translatedText;
        }


        private static void ProcessTabControl(TabControl tabControl, ResourceManager rm, string languageToLoad)
        {
            // Process each tab page within the tab control
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                ProcessTabPage(tabPage, rm, languageToLoad);
            }
        }


        private static void ProcessTabPage(TabPage tabPage, ResourceManager rm, string languageToLoad)
        {

            foreach (Control tabPageControl in tabPage.Controls)
            {
                ProcessControl(tabPageControl, rm, languageToLoad);
            }

            string tabPageText = IsEnglish(tabPage.Text) ? tabPage.Text : (string)tabPage.Tag;
            tabPage.Tag = tabPageText;

            if (languageToLoad == "ar-SA")
            {
                tabPage.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                tabPage.RightToLeft = RightToLeft.No;
            }

            if (tabPageText != null)
            {
                string translatedText = rm.GetString(tabPageText);
                tabPage.Text = translatedText ?? tabPageText; // Use the translated text if available, otherwise fallback to the original text
            }
        }
        private static void ProcessRadioButton(RadioButton radioButton, ResourceManager rm, string languageToLoad)
        {
            string radioText = IsEnglish(radioButton.Text) ? radioButton.Text : (string)radioButton.Tag;
            radioButton.Tag = radioText;

            if (languageToLoad == "ar-SA")
            {
                radioButton.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                radioButton.RightToLeft = RightToLeft.No;
            }

            if (radioText != null)
            {
                string translatedText = rm.GetString(radioText);
                radioButton.Text = translatedText ?? radioText; // Use the translated text if available, otherwise fallback to the original text
            }
        }

        private static void ProcessButton(Button button, ResourceManager rm, string languageToLoad)
        {
            string btnText = IsEnglish(button.Text) ? button.Text : (string)button.Tag;
            button.Tag = btnText;

            if (languageToLoad == "ar-SA")
            {
                button.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                button.RightToLeft = RightToLeft.No;
            }

            button.Text = rm.GetString(btnText);


        }


        private static void ProcessTableLayoutPanel(TableLayoutPanel tableLayoutPanel, ResourceManager rm, string languageToLoad)
        {
            foreach (Control childControl in tableLayoutPanel.Controls)
            {
                ProcessControl(childControl, rm, languageToLoad);
            }
        }

        private static void Processpanel(Panel panel, ResourceManager rm, string languageToLoad)
        {
            foreach (Control childControl in panel.Controls)
            {
                ProcessControl(childControl, rm, languageToLoad);
            }
        }

        private static void ProcessFlowLayoutPanel(FlowLayoutPanel flowLayoutPanel, ResourceManager rm, string languageToLoad)
        {
            foreach (Control childControl in flowLayoutPanel.Controls)
            {
                ProcessControl(childControl, rm, languageToLoad);

                if (childControl is SparkPOS.Helper.UserControl.AdvancedTextbox textbox)
                {
                    ProcessAdvancedTextbox(textbox, rm, languageToLoad);
                }
                else if (childControl is GroupBox groupBox)
                {
                    ProcessGroupBox(groupBox, rm, languageToLoad);
                }
                else if (childControl is FlowLayoutPanel nestedFlowLayoutPanel)
                {
                    ProcessFlowLayoutPanel(nestedFlowLayoutPanel, rm, languageToLoad);
                }
                // Add conditions for other control types if needed
            }
        }


        private static void ProcessGroupBox(GroupBox groupBox, ResourceManager rm, string languageToLoad)
        {
            string groupBoxText = IsEnglish(groupBox.Text) ? groupBox.Text : (string)groupBox.Tag;
            groupBox.Tag = groupBoxText;

            if (languageToLoad == "ar-SA")
            {
                groupBox.RightToLeft = RightToLeft.Yes;
            }
            else if (languageToLoad == "en-US")
            {
                groupBox.RightToLeft = RightToLeft.No;
            }

            groupBox.Text = rm.GetString(groupBoxText);
            foreach (Control childControl in groupBox.Controls)
            {
                ProcessControl(childControl, rm, languageToLoad);
            }
        }
        public static void TranslateToolTripTitle(Form frm)
        {
            if (MainProgram.currentLanguage == "en-US")
            {
                TranslateToolTipTitle(frm, "Information");
            }
            else if (MainProgram.currentLanguage == "ar-SA")
            {
                TranslateToolTipTitle(frm, "Information");
            }
        }

        private static void TranslateToolTipTitle(Form frm, string title)
        {
            ToolTip toolTip = GetToolTipControl(frm);
            if (toolTip != null)
            {
                toolTip.ToolTipTitle = TranslateString(title);
            }
        }

        private static ToolTip GetToolTipControl(Control control)
        {
            if (control is Form form)
            {
                return form.Controls.OfType<ToolTip>().FirstOrDefault();
            }

            foreach (Control childControl in control.Controls)
            {
                ToolTip toolTip = GetToolTipControl(childControl);
                if (toolTip != null)
                {
                    return toolTip;
                }
            }

            return null;
        }

        private static string TranslateString(string text)
        {
            // Implement your translation logic here based on the current language
            if (MainProgram.currentLanguage == "ar-SA")
            {
                // Perform Arabic translation
                // Replace this with your actual translation logic
                if (text == "Information")
                {
                    return "معلومات";
                }
            }

            // Default language or unsupported language
            return text; // Return the original string if no translation is available
        }


        private static void ProcessGridControl(GridControl gridControl, ResourceManager rm, string languageToLoad)
        {
            int columnCount = gridControl.ColCount;

            for (int colIndex = 1; colIndex <= columnCount; colIndex++)
            {
                string headerText = IsEnglish(gridControl[0, colIndex].Text) ? gridControl[0, colIndex].Text : (string)gridControl[0, colIndex].Tag;
                gridControl[0, colIndex].Tag = headerText;

                string localizedHeaderText = rm.GetString(headerText);
                if (languageToLoad == "ar-SA")
                {
                    gridControl[0, colIndex].RightToLeft = RightToLeft.Yes;
                    //gridControl.Cols.Hidden[colIndex] = false; // Hide the column in Arabic language
                }
                else if (languageToLoad == "en-US")
                {
                    gridControl[0, colIndex].RightToLeft = RightToLeft.No;
                }
                gridControl[0, colIndex].Text = localizedHeaderText;
                // 
                if (colIndex == 7) // Assuming the button column is at index 7
                {
                    for (int rowIndex = 0; rowIndex <= gridControl.RowCount; rowIndex++)
                    {
                        GridStyleInfo buttonStyle = gridControl[rowIndex, colIndex];
                        string buttonText = "Delete"; // Assuming the button text is always "Delete"
                        if (buttonStyle.CellType == GridCellTypeName.PushButton)
                        {
                            //buttonStyle.HorizontalAlignment = GridHorizontalAlignment.Right;
                            if (languageToLoad == "ar-SA")
                            {
                                buttonStyle.RightToLeft = RightToLeft.Yes;
                            }
                            else if (languageToLoad == "en-US")
                            {
                                buttonStyle.RightToLeft = RightToLeft.No;
                            }
                            string strDescription = IsEnglish(buttonStyle.Description) ? buttonStyle.Description : buttonText;
                            string localizedCellButtonText = rm.GetString(strDescription);
                            buttonStyle.Description = localizedCellButtonText;
                            // buttonStyle.Handled = true;
                        }


                    }
                }
            }


        }


    }
}



