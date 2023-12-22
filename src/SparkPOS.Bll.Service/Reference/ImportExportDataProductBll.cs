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
    public class ImportExportDataProductBll : IImportExportDataBll<Product>
    {
        private ILog _log;
        private IUnitOfWork _unitOfWork;

        private string _fileName;
        private XLWorkbook _workbook;

        public ImportExportDataProductBll(string fileName, ILog log)
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

                var colums = new string[] { 
                                            "CATEGORY", "CODE PRODUCT", "NAME PRODUCT", "unit",
                                            "Buying Price", "price JUAL (RETAIL)", "discount (RETAIL)", 
                                            "price GROSIR #1", "quantity Minimum GROSIR #1", "discount GROSIR #1", 
                                            "price GROSIR #2", "quantity Minimum GROSIR #2", "discount GROSIR #2", 
                                            "price GROSIR #3", "quantity Minimum GROSIR #3", "discount GROSIR #3",
                                            "STOCK ETALASE", "STOCK GUDANG", "Minimum STOCK GUDANG"
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
                var produkRow = firstRowUsed.RowUsed();

                // First possible address of the company table:
                var firstPossibleAddress = ws.Row(produkRow.RowNumber()).FirstCell().Address;

                // Last possible address of the company table:
                var lastPossibleAddress = ws.LastCellUsed().Address;

                // Get a range with the remainder of the worksheet data (the range used)
                var produkRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

                // Treat the range as a table (to be able to use the column names)
                var supplierTable = produkRange.AsTable();

                var listOfProduct = new List<Product>();

                var hargaWholesale1 = new PriceWholesale();
                var hargaWholesale2 = new PriceWholesale();
                var hargaWholesale3 = new PriceWholesale();

                listOfProduct = supplierTable.DataRange.Rows().Select(row => new Product
                {
                    Category = new Category { name_category = row.Field("CATEGORY").GetString() },
                    product_code = row.Field("CODE PRODUCT").GetString(),
                    product_name = row.Field("NAME PRODUCT").GetString(),
                    unit = row.Field("unit").GetString(),
                    purchase_price = row.Field("Buying Price").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("Buying Price").GetString()),
                    selling_price = row.Field("price JUAL (RETAIL)").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("price JUAL (RETAIL)").GetString()),
                    discount = row.Field("discount (RETAIL)").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("discount (RETAIL)").GetString()),

                    list_of_harga_grosir = new List<PriceWholesale> 
                    {
                        new PriceWholesale 
                        { 
                            retail_price = 1,
                            wholesale_price = row.Field("price GROSIR #1").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("price GROSIR #1").GetString()), 
                            minimum_quantity = row.Field("quantity Minimum GROSIR #1").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("quantity Minimum GROSIR #1").GetString()), 
                            discount = row.Field("discount GROSIR #1").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("discount GROSIR #1").GetString()) 
                        },
                        new PriceWholesale 
                        { 
                            retail_price = 2,
                            wholesale_price = row.Field("price GROSIR #2").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("price GROSIR #2").GetString()), 
                            minimum_quantity = row.Field("quantity Minimum GROSIR #2").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("quantity Minimum GROSIR #2").GetString()), 
                            discount = row.Field("discount GROSIR #2").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("discount GROSIR #2").GetString()) 
                        },
                        new PriceWholesale 
                        { 
                            retail_price = 3,
                            wholesale_price = row.Field("price GROSIR #3").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("price GROSIR #3").GetString()), 
                            minimum_quantity = row.Field("quantity Minimum GROSIR #3").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("quantity Minimum GROSIR #3").GetString()), 
                            discount = row.Field("discount GROSIR #3").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("discount GROSIR #3").GetString()) 
                        }
                    },

                    stock = row.Field("STOCK ETALASE").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("STOCK ETALASE").GetString()),
                    warehouse_stock = row.Field("STOCK GUDANG").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("STOCK GUDANG").GetString()),
                    minimal_stock_warehouse = row.Field("Minimum STOCK GUDANG").GetString().Length == 0 ? 0 : Convert.ToDouble(row.Field("Minimum STOCK GUDANG").GetString())
                }).ToList();

                if (listOfProduct.Count == 1 && listOfProduct[0].product_name.Length == 0)
                {
                    rowCount = 0;
                    return false;
                }

                rowCount = listOfProduct.Where(f => f.product_name.Length > 0 && f.Category.name_category.Length > 0)
                                       .Count();

                using (IDapperContext context = new DapperContext())
                {
                    _unitOfWork = new UnitOfWork(context, _log);
                    
                    foreach (var product in listOfProduct)
                    {
                        if (product.product_name.Length > 0 && product.Category.name_category.Length > 0)
                        {
                            var category = _unitOfWork.CategoryRepository.GetByName(product.Category.name_category, false)
                                                                 .FirstOrDefault();

                            if (category != null)
                            {
                                product.category_id = category.category_id;
                                product.Category = category;
                            }

                            if (product.product_code.Length == 0)
                                product.product_code = _unitOfWork.ProductRepository.GetLastCodeProduct();

                            if (product.product_code.Length > 15)
                                product.product_code = product.product_code.Substring(0, 15);

                            if (product.product_name.Length > 300)
                                product.product_name = product.product_name.Substring(0, 300);

                            if (product.unit.Length > 20)
                                product.unit = product.unit.Substring(0, 20);

                            var oldProduct = _unitOfWork.ProductRepository.GetByCode(product.product_code);
                            if (oldProduct == null)
                            {
                                product.is_active = true;
                                result = Convert.ToBoolean(_unitOfWork.ProductRepository.Save(product));
                            }                                
                            else
                            {
                                // khusus stock Shelfdan gudang diabaikan (tidak diupdate)
                                product.product_id = oldProduct.product_id;
                                product.code_produk_old = oldProduct.product_code;
                                product.stock = oldProduct.stock;
                                product.warehouse_stock = oldProduct.warehouse_stock;
                                product.is_active = oldProduct.is_active;

                                foreach (var grosir in product.list_of_harga_grosir.OrderBy(f => f.retail_price))
                                {
                                    var oldWholesale = oldProduct.list_of_harga_grosir
                                                             .Where(f => f.product_id == product.product_id && f.retail_price == grosir.retail_price)
                                                             .SingleOrDefault();

                                    if (oldWholesale != null)
                                    {
                                        grosir.wholesale_price_id = oldWholesale.wholesale_price_id;
                                        grosir.product_id = oldWholesale.product_id;
                                    }                                    
                                }

                                result = Convert.ToBoolean(_unitOfWork.ProductRepository.Update(product));
                            }
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

        public void Export(IList<Product> listOfObject)
        {
            try
            {
                // Creating a new workbook
                using (var wb = new XLWorkbook())
                {
                    // Adding a worksheet
                    var ws = wb.Worksheets.Add("product");

                    // Set header table
                    ws.Cell(1, 1).Value = "NO";
                    ws.Cell(1, 2).Value = "CATEGORY";
                    ws.Cell(1, 3).Value = "CODE PRODUCT";
                    ws.Cell(1, 4).Value = "NAME PRODUCT";
                    ws.Cell(1, 5).Value = "unit";
                    ws.Cell(1, 6).Value = "Buying Price";
                    ws.Cell(1, 7).Value = "price JUAL (RETAIL)";
                    ws.Cell(1, 8).Value = "discount (RETAIL)";

                    ws.Cell(1, 9).Value = "price GROSIR #1";
                    ws.Cell(1, 10).Value = "quantity Minimum GROSIR #1";
                    ws.Cell(1, 11).Value = "discount GROSIR #1";

                    ws.Cell(1, 12).Value = "price GROSIR #2";
                    ws.Cell(1, 13).Value = "quantity Minimum GROSIR #2";
                    ws.Cell(1, 14).Value = "discount GROSIR #2";

                    ws.Cell(1, 15).Value = "price GROSIR #3";
                    ws.Cell(1, 16).Value = "quantity Minimum GROSIR #3";
                    ws.Cell(1, 17).Value = "discount GROSIR #3";

                    ws.Cell(1, 18).Value = "STOCK ETALASE";
                    ws.Cell(1, 19).Value = "STOCK GUDANG";
                    ws.Cell(1, 20).Value = "Minimum STOCK GUDANG";

                    var noUrut = 1;
                    foreach (var product in listOfObject)
                    {
                        ws.Cell(1 + noUrut, 1).Value = noUrut;
                        ws.Cell(1 + noUrut, 2).Value = product.Category != null ? product.Category.name_category : string.Empty;
                        ws.Cell(1 + noUrut, 3).SetValue(product.product_code).SetDataType(XLCellValues.Text);
                        ws.Cell(1 + noUrut, 4).Value = product.product_name;
                        ws.Cell(1 + noUrut, 5).Value = product.unit;
                        ws.Cell(1 + noUrut, 6).Value = product.purchase_price;
                        ws.Cell(1 + noUrut, 7).Value = product.selling_price;
                        ws.Cell(1 + noUrut, 8).Value = product.discount;

                        var listOfPriceWholesale = product.list_of_harga_grosir;
                        if (listOfPriceWholesale.Count > 0)
                        {
                            var column = 9;
                            foreach (var grosir in listOfPriceWholesale)
                            {
                                ws.Cell(1 + noUrut, column).Value = grosir.wholesale_price;
                                ws.Cell(1 + noUrut, column + 1).Value = grosir.minimum_quantity;
                                ws.Cell(1 + noUrut, column + 2).Value = grosir.discount;

                                column += 3;
                            }
                        }

                        ws.Cell(1 + noUrut, 18).Value = product.stock;
                        ws.Cell(1 + noUrut, 19).Value = product.warehouse_stock;
                        ws.Cell(1 + noUrut, 20).Value = product.minimal_stock_warehouse;

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
