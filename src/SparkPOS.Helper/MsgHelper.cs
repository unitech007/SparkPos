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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SparkPOS.Helper
{
    /// <summary>
    /// Class helper untuk menghandle dialog pesan
    /// </summary>
    public static class MsgHelper
    {
        /// <summary>
        /// Method untuk menampilkan pesan konfirmasi "Yes" dan "No"
        /// </summary>
        /// <param name="prompt">Information yang ingin ditampilkan</param>
        /// <returns></returns>
        public static bool MsgConfirmation(string prompt)
        {
            string warningTitle = "Confirmation";
            warningTitle = MultilingualHelper.LoadAndCallMsgHelper(warningTitle, MainProgram.currentLanguage);

            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);

            return (MessageBox.Show(prompt, warningTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes);
        }

        /// <summary>
        /// Method untuk menampilkan konfirmasi penyimpanan data
        /// </summary>
        /// <returns></returns>
        public static bool MsgSave()
        {
            return MsgConfirmation("Are you sure you want to save this data ???");
        }

        /// <summary>
        /// Method untuk menampilkan Warning
        /// </summary>
        /// <param name="prompt">Information yang ingin ditampilkan</param>
        public static void MsgWarning(string prompt, string replace = null)
        {
            string warningTitle = "Warning";
            warningTitle = MultilingualHelper.LoadAndCallMsgHelper(warningTitle, MainProgram.currentLanguage);
            //if (prompt.Contains("{"))
            //{
            //    prompt = prompt.Substring(0, (prompt.Length - prompt.IndexOf("{")));
            //}
            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);
            if (string.IsNullOrEmpty(replace))
            {
                MessageBox.Show(prompt, warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            else
            {
                MessageBox.Show(string.Format(prompt,replace) ,warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        public static void MsgWarnings(string prompt, object[] replace = null)
        {
            string warningTitle = "Warning";
            warningTitle = MultilingualHelper.LoadAndCallMsgHelper(warningTitle, MainProgram.currentLanguage);
            //if (prompt.Contains("{"))
            //{
            //    prompt = prompt.Substring(0, (prompt.Length - prompt.IndexOf("{")));
            //}
            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);
            if (replace == null)
            {
                MessageBox.Show(prompt, warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            else
            {
                MessageBox.Show(string.Format(prompt, replace), warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }
        /// <summary>
        /// Method untuk menampilkan Warning terjadinya duplikasi data pada saat penyimpanan
        /// </summary>
        /// <param name="prompt">Information yang ingin ditampilkan</param>
        public static void MsgDuplicate(string prompt)
        {
            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);

            var pesan = "Key-DataEntered";
            object[] params1 = new object[] { prompt };
        
            MsgWarnings(pesan, params1);
        }

        /// <summary>
        /// Method untuk menampilkan Warning error
        /// </summary>
        /// <param name="prompt"></param>
        public static void MsgError(string prompt)
        {
            string warningTitle = "Warning";
            warningTitle = MultilingualHelper.LoadAndCallMsgHelper(warningTitle, MainProgram.currentLanguage);

            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);

            MessageBox.Show(prompt, warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Method untuk menampilkan Warning data Not yet selected
        /// </summary>
        /// <param name="prompt">Information yang ingin ditampilkan</param>
        public static void MsgNotSelected(string prompt)
        {
            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);

            MsgWarning("Sorry '" + prompt + "' Not yet selected !!!");
        }

        /// <summary>
        /// Method untuk menampilkan Information
        /// </summary>
        /// <param name="prompt">Information yang ingin ditampilkan</param>
        public static void MsgInfo(string prompt)
        {
            string warningTitle = "Information";
            warningTitle = MultilingualHelper.LoadAndCallMsgHelper(warningTitle, MainProgram.currentLanguage);

            prompt = MultilingualHelper.LoadAndCallMsgHelper(prompt, MainProgram.currentLanguage);

            MessageBox.Show(prompt, warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //public static void MsgInfo(string prompt)
        //{
        //    string warningTitle = "Information";

        //    // Check if translation is required
        //    if (MainProgram.currentLanguage != "en-US")
        //    {
        //        string translatedPrompt = TranslateText(prompt, "en", "ar-SA");
        //        if (!string.IsNullOrEmpty(translatedPrompt))
        //            prompt = translatedPrompt;
        //    }

        //    MessageBox.Show(prompt, warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

        //private static string TranslateText(string text, string sourceLanguage, string targetLanguage)
        //{
        //    TranslationClient client = TranslationClient.Create();

        //    TranslationResult result = client.TranslateText(text, targetLanguage, sourceLanguage);

        //    return result.TranslatedText;
        //}


        /// <summary>
        /// Method untuk menampilkan konfirmasi penghapusan data
        /// </summary>
        /// <returns></returns>
        public static bool MsgDelete()
        {
            return (MsgConfirmation("Do you want to delete this data?"));
        }

        /// <summary>
        /// Method untuk menampilkan Warning gagal pada saat menghapus data
        /// </summary>
        public static void MsgDeleteError()
        {
            MsgWarning("Key-AnotherProcess");
        }

        /// <summary>
        /// Method untuk menampilkan Warning gagal pada saat mengupdate data
        /// </summary>
        public static void MsgUpdateError()
        {
            MsgWarning("Key-EnteredFailed");
        }

        /// <summary>
        /// Method untuk menampilkan Warning there inputan yang masih empty
        /// </summary>
        /// <param name="prompt"></param>
        public static void MsgRequire(string prompt)
        {
            MsgWarning("Sorry, Information '" + prompt + "' must filled !");
        }

        /// <summary>
        /// Method untuk menampilkan Warning data yang dicari not found
        /// </summary>
        public static void MsgNotFound()
        {
            MsgWarning("Sorry the data you found was not found");
        }

        /// <summary>
        /// Method untuk menampilkan Warning range date tidak valid
        /// </summary>
        public static void MsgNotValidRangeDate()
        {
            MsgWarning("Sorry, the date range is wrong !!!");
        }

        public static void MsgWarning(object minimumReasonSelectedWarning)
        {
            throw new NotImplementedException();
        }
    }
}
