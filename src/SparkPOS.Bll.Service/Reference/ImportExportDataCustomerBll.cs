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
    public class ImportExportDataCustomerBll : IImportExportDataBll<Customer>
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        private string _fileName;
        private XLWorkbook _workbook;

        public ImportExportDataCustomerBll(string fileName, ILog log)
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
            catch(Exception ex)
            {
                Config.LogException(ex);
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

                var colums = new string[] { 
                                            "NAME", "PROVINSI", "REGENCY/CITY", "subdistrict", "ADDRESS", 
                                            "CODE POS", "CONTACT", "phone", "discount RESELLER", "LIMIT PIUTANG" 
                                          };

                for (int i = 0; i < colums.Length; i++)
                {
                    if (!(colums[i] == firstRowUsed.Cell(i + 1).GetString()))
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Config.LogException(ex);
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
                var supplierRow = firstRowUsed.RowUsed();

                // First possible address of the company table:
                var firstPossibleAddress = ws.Row(supplierRow.RowNumber()).FirstCell().Address;

                // Last possible address of the company table:
                var lastPossibleAddress = ws.LastCellUsed().Address;

                // Get a range with the remainder of the worksheet data (the range used)
                var customerRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

                // Treat the range as a table (to be able to use the column names)
                var customerTable = customerRange.AsTable();

                var listOfCustomer = new List<Customer>();

                listOfCustomer = customerTable.DataRange.Rows().Select(row => new Customer
                {
                    name_customer = row.Field("NAME").GetString(),
                    Provinsi = new Provinsi { name_province = row.Field("PROVINSI").GetString() },
                    Regency = new Regency { name_regency = row.Field("REGENCY/CITY").GetString() },
                    subdistrict = new subdistrict { name_subdistrict = row.Field("subdistrict").GetString() },
                    address = row.Field("ADDRESS").GetString(),
                    postal_code = row.Field("CODE POS").GetString(),
                    contact = row.Field("CONTACT").GetString(),
                    phone = row.Field("phone").GetString(),
                    discount = row.Field("discount RESELLER").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("discount RESELLER").GetString()),
                    credit_limit = row.Field("LIMIT PIUTANG").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("LIMIT PIUTANG").GetString())
                }).ToList();

                if (listOfCustomer.Count == 1 && listOfCustomer[0].name_customer.Length == 0)
                {
                    rowCount = 0;
                    return false;
                }

                rowCount = listOfCustomer.Count;

                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);

                    foreach (var customer in listOfCustomer)
                    {
                        if (customer.name_customer.Length > 0)
                        {
                            if (customer.name_customer.Length > 50)
                                customer.name_customer = customer.name_customer.Substring(0, 50);

                            if (customer.address.Length > 250)
                                customer.address = customer.address.Substring(0, 250);

                            if (customer.contact.Length > 50)
                                customer.contact = customer.contact.Substring(0, 50);

                            if (customer.phone.Length > 20)
                                customer.phone = customer.phone.Substring(0, 20);

                            var provinsi = _unitOfWork.AreaRepository.GetProvinsi(customer.Provinsi.name_province);
                            if (provinsi != null)
                            {
                                customer.province_id = provinsi.province_id;
                                customer.Provinsi = new Provinsi { province_id = provinsi.province_id, name_province = provinsi.name_province };
                            }

                            var regency = _unitOfWork.AreaRepository.GetRegency(customer.Regency.name_regency);
                            if (regency != null)
                            {
                                customer.regency_id = regency.regency_id;
                                customer.Regency = new Regency { regency_id = regency.regency_id, name_regency = regency.name_regency };
                            }

                            var subdistrict = _unitOfWork.AreaRepository.Getsubdistrict(customer.subdistrict.name_subdistrict);
                            if (subdistrict != null)
                            {
                                customer.subdistrict_id = subdistrict.subdistrict_id;
                                customer.subdistrict = new subdistrict { subdistrict_id = subdistrict.subdistrict_id, name_subdistrict = subdistrict.name_subdistrict };
                            }

                            result = Convert.ToBoolean(_unitOfWork.CustomerRepository.Save(customer));
                        }
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                 Config.LogException(ex);
                _log.Error("Error:", ex);
            }
            finally
            {
                _workbook.Dispose();
            }

            return result;
        }

        public void Export(IList<Customer> listOfObject)
        {
            try
            {
                // Creating a new workbook
                using (var wb = new XLWorkbook())
                {
                    // Adding a worksheet
                    var ws = wb.Worksheets.Add("customer");

                    // Set header table
                    ws.Cell(1, 1).Value = "NO";
                    ws.Cell(1, 2).Value = "NAME";

                    ws.Cell(1, 3).Value = "PROVINSI";
                    ws.Cell(1, 4).Value = "REGENCY/CITY";
                    ws.Cell(1, 5).Value = "subdistrict";

                    ws.Cell(1, 6).Value = "ADDRESS";
                    ws.Cell(1, 7).Value = "CODE POS";
                    ws.Cell(1, 8).Value = "CONTACT";
                    ws.Cell(1, 9).Value = "phone";
                    ws.Cell(1, 10).Value = "discount RESELLER";
                    ws.Cell(1, 11).Value = "LIMIT PIUTANG";

                    var noUrut = 1;
                    foreach (var customer in listOfObject)
                    {
                        ws.Cell(1 + noUrut, 1).Value = noUrut;
                        ws.Cell(1 + noUrut, 2).Value = customer.name_customer;

                        ws.Cell(1 + noUrut, 3).Value = customer.Provinsi != null ? customer.Provinsi.name_province : string.Empty;
                        ws.Cell(1 + noUrut, 4).Value = customer.Regency != null ? customer.Regency.name_regency : customer.regency_old.NullToString();
                        ws.Cell(1 + noUrut, 5).Value = customer.subdistrict != null ? customer.subdistrict.name_subdistrict : customer.subdistrict_old.NullToString();

                        ws.Cell(1 + noUrut, 6).Value = customer.address;
                        ws.Cell(1 + noUrut, 7).SetValue(customer.postal_code).SetDataType(XLCellValues.Text);
                        ws.Cell(1 + noUrut, 8).Value = customer.contact;
                        ws.Cell(1 + noUrut, 9).SetValue(customer.phone).SetDataType(XLCellValues.Text);
                        ws.Cell(1 + noUrut, 10).Value = customer.discount;
                        ws.Cell(1 + noUrut, 11).Value = customer.credit_limit;

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
                Config.LogException(ex);
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
