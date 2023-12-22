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

using log4net;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using ClosedXML.Excel;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Service;
using System.IO;
using System.Diagnostics;

namespace SparkPOS.Bll.Service
{
    public class ImportExportDataCategoryBll : IImportExportDataBll<Category>
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        private string _fileName;
        private XLWorkbook _workbook;

        public ImportExportDataCategoryBll(string fileName, ILog log)
        {
            _fileName = fileName;
            _log = log;
        }

        public bool IsOpened()
        {
            var result = false;

            try
            {
                _workbook = new XLWorkbook(_fileName);
            }
            catch
            {
                result = true;
            }

            return result;
        }

        public bool IsValidFormat(string workSheetName)
        {
            var result = true;

            try
            {
                var ws = _workbook.Worksheet(workSheetName);

                // Look for the first row used
                var firstRowUsed = ws.FirstRowUsed();

                var colums = new string[] { "CATEGORY", "PROFIT (%)", "discount" };

                for (int i = 0; i < colums.Length; i++)
                {
                    if (!(colums[i] == firstRowUsed.Cell(i + 1).GetString()))
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool Import(string workSheetName, ref int rowCount)
        {
            var result = false;

            try
            {
                var ws = _workbook.Worksheet(workSheetName);

                // Look for the first row used
                var firstRowUsed = ws.FirstRowUsed();

                // Narrow down the row so that it only includes the used part
                var golonganRow = firstRowUsed.RowUsed();

                // First possible address of the company table:
                var firstPossibleAddress = ws.Row(golonganRow.RowNumber()).FirstCell().Address;

                // Last possible address of the company table:
                var lastPossibleAddress = ws.LastCellUsed().Address;

                // Get a range with the remainder of the worksheet data (the range used)
                var golonganRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

                // Treat the range as a table (to be able to use the column names)
                var golonganTable = golonganRange.AsTable();

                var listOfCategory = new List<Category>();

                listOfCategory = golonganTable.DataRange.Rows().Select(row => new Category
                {
                    name_category = row.Field("CATEGORY").GetString(),
                    profit_percentage = row.Field("PROFIT (%)").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("PROFIT (%)").GetString()),
                    discount = row.Field("discount").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("discount").GetString())
                }).ToList();

                if (listOfCategory.Count == 1 && listOfCategory[0].name_category.Length == 0)
                {
                    rowCount = 0;
                    return false;
                }

                rowCount = listOfCategory.Count;

                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);

                    foreach (var category in listOfCategory)
                    {
                        if (category.name_category.Length > 0)
                        {
                            if (category.name_category.Length > 50)
                                category.name_category = category.name_category.Substring(0, 50);

                            var oldCategory = _unitOfWork.CategoryRepository.GetByName(category.name_category, false)
                                                                    .FirstOrDefault();

                            if (oldCategory == null) // data category Not yet there
                                result = Convert.ToBoolean(_unitOfWork.CategoryRepository.Save(category));
                        }                        
                    }                    
                }

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }
            finally
            {
                _workbook.Dispose();
            }

            return result;
        }

        public void Export(IList<Category> listOfObject)
        {
            try
            {
                // Creating a new workbook

                using (var wb = new XLWorkbook())
                {
                    // Adding a worksheet
                    var ws = wb.Worksheets.Add("category");

                    // Set header table
                    ws.Cell(1, 1).Value = "NO";
                    ws.Cell(1, 2).Value = "CATEGORY";
                    ws.Cell(1, 3).Value = "PROFIT (%)";
                    ws.Cell(1, 4).Value = "discount";

                    var noUrut = 1;
                    foreach (var category in listOfObject)
                    {
                        ws.Cell(1 + noUrut, 1).Value = noUrut;
                        ws.Cell(1 + noUrut, 2).Value = category.name_category;
                        ws.Cell(1 + noUrut, 3).Value = category.profit_percentage;
                        ws.Cell(1 + noUrut, 4).Value = category.discount;

                        noUrut++;
                    }

                    // Saving the workbook
                    wb.SaveAs(_fileName);

                    var fi = new FileInfo(_fileName);
                    if (fi.Exists)
                        Process.Start(_fileName);
                }                               
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }
        }

        public IList<string> GetWorksheets()
        {
            var listOfWorksheet = new List<string>();

            foreach (IXLWorksheet worksheet in _workbook.Worksheets)
            {
                listOfWorksheet.Add(worksheet.Name);
            }

            return listOfWorksheet;
        }
    }
}
