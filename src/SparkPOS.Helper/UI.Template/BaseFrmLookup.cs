﻿/**
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

using SparkPOS.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SparkPOS.Helper.UI.Template
{
    public class BaseFrmLookup : Form
    {
        /// <summary>
        /// Status tombol pilihan active atau enggak
        /// </summary>
        public virtual bool IsButtonSelectEnabled { get; private set; }

        /// <summary>
        /// Method override untuk menghandle item yang selected
        /// </summary>
        /// <param name="index">Diisi dengan index grid list</param>
        /// <param name="prompt">Information data yang selected</param>
        /// <returns></returns>
        protected bool IsSelectedItem(int index, string prompt)
        {
            if (index < 0)
            {
                var msg = "Sorry '" + prompt + "' Not yet selected.";
                MsgHelper.MsgWarning(msg);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Method protected untuk mengeset header form lookup
        /// </summary>
        /// <param name="header"></param>
        protected virtual void SetHeader(string header)
        {
        }

        protected virtual void SetTitleBtnSelect(string title)
        {
        }

        /// <summary>
        /// Method protected untuk toactivate/deactivate tombol pilih
        /// </summary>
        /// <param name="status"></param>
        protected virtual void SetActiveBtnSelect(bool status)
        {
        }

        /// <summary>
        /// Method override untuk menghandle proses pilih
        /// </summary>
        protected virtual void Select()
        {
        }

        /// <summary>
        /// Method override untuk menghandle event checkbox select all
        /// </summary>
        protected virtual void SelectAll()
        {
        }

        /// <summary>
        /// An overridden method to handle process completion
        /// </summary>
        //protected new virtual void Close()
        //{
        //    this.Close();
        //}

        //protected override void Close()
        //{
        //    // Call the base class Close() method
        //    base.Close();

        //    // Other cleanup code goes here
        //}


    }
}
