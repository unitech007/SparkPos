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
using Dapper;
using SparkPOS.Model;
using SparkPOS.Model.Report;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Api.Report;

namespace SparkPOS.Repository.Service.Report
{
    public class ReportSellingQuotationRepository : IReportSellingQuotationRepository
    {
        private const string SQL_TEMPLATE_HEADER = @"SELECT t_product_sales.return_sale_id, t_product_sales.invoice, t_product_sales.date, t_product_sales.due_date, 
                                                     t_product_sales.tax, t_product_sales.shipping_cost, t_product_sales.discount, t_product_sales.total_invoice, t_product_sales.total_payment, t_product_sales.description, t_product_sales.system_date,
                                                     m_customer.customer_id, m_customer.name_customer, m_user.user_id, m_user.name_user,
                                                     m_role.role_id, m_role.name_role, m_shift.shift_id, m_shift.name_shift
                                                     FROM public.t_product_sales INNER JOIN public.m_user ON t_product_sales.user_id = m_user.user_id
                                                     LEFT JOIN public.m_customer ON m_customer.customer_id = t_product_sales.customer_id
                                                     INNER JOIN m_role ON m_role.role_id = m_user.role_id
                                                     LEFT JOIN m_shift ON m_shift.shift_id = t_product_sales.shift_id
                                                     {WHERE}
                                                     ORDER BY t_product_sales.date, t_product_sales.invoice";

        private const string SQL_TEMPLATE_DETAIL = @"SELECT m_product.product_id, m_product.product_name, m_product.unit, t_sales_order_item.quantity, t_sales_order_item.return_quantity, t_sales_order_item.purchase_price, 
                                                     t_sales_order_item.selling_price, t_sales_order_item.discount, COALESCE(t_sales_order_item.description, t_sales_order_item.description, '') AS description,
                                                     t_product_sales.date, t_product_sales.due_date, t_product_sales.invoice, 
                                                     m_customer.customer_id, m_customer.name_customer
                                                     FROM public.t_product_sales LEFT JOIN public.m_customer ON m_customer.customer_id = t_product_sales.customer_id
                                                     INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                     INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                                     {WHERE}
                                                     ORDER BY t_product_sales.date, t_product_sales.invoice, m_product.product_name";

        private const string SQL_TEMPLATE_PER_PRODUK = @"SELECT m_customer.customer_id, m_customer.name_customer, t_product_sales.date, m_product.product_id, m_product.product_name, m_product.unit, SUM(t_sales_order_item.quantity) AS quantity, 
                                                         SUM(t_sales_order_item.return_quantity) AS return_quantity, t_sales_order_item.purchase_price, t_sales_order_item.selling_price, t_sales_order_item.discount
                                                         FROM public.t_product_sales LEFT JOIN public.m_customer ON m_customer.customer_id = t_product_sales.customer_id 
                                                         INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                         INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                                         {WHERE}
                                                         GROUP BY m_customer.customer_id, m_customer.name_customer, t_product_sales.date, m_product.product_id, m_product.product_name, m_product.unit, t_sales_order_item.purchase_price, t_sales_order_item.selling_price, t_sales_order_item.discount                                            
                                                         ORDER BY t_product_sales.date, m_product.product_name";

        private const string SQL_TEMPLATE_PRODUK_FAVORIT = @"SELECT m_product.product_name, SUM(t_sales_order_item.quantity - t_sales_order_item.return_quantity) AS quantity
                                                             FROM public.t_product_sales INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                             INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                                             {WHERE}
                                                             GROUP BY m_product.product_id, m_product.product_name
                                                             ORDER BY SUM(t_sales_order_item.quantity - t_sales_order_item.return_quantity) DESC LIMIT @limit";

        private const string SQL_TEMPLATE_PER_KASIR_HEADER = @"SELECT m_user.user_id as kasir_id, m_user.name_user as Cashier, SUM(t_machine.starting_balance) AS starting_balance, SUM(t_product_sales.tax) AS tax, SUM(t_product_sales.discount) AS diskon_nota, SUM(t_product_sales.total_invoice) AS total_invoice
                                                               FROM public.t_machine LEFT JOIN public.t_product_sales ON t_product_sales.machine_id= t_machine.machine_id
                                                               INNER JOIN public.m_user ON t_machine.user_id = m_user.user_id
                                                               {WHERE}
                                                               GROUP BY m_user.user_id, m_user.name_user
                                                               ORDER BY m_user.name_user";

        private const string SQL_TEMPLATE_PER_KASIR_DETAIL = @"SELECT m_product.product_name, COALESCE(t_sales_order_item.description, t_sales_order_item.description, '') AS description, SUM(t_sales_order_item.quantity) AS quantity, SUM(t_sales_order_item.return_quantity) AS return_quantity, 
                                                               SUM(t_sales_order_item.discount) AS discount, t_sales_order_item.selling_price
                                                               FROM public.t_machine INNER JOIN public.t_product_sales ON t_product_sales.machine_id= t_machine.machine_id
                                                               INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                               INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                                               {WHERE}
                                                               GROUP BY m_product.product_name, t_sales_order_item.description, t_sales_order_item.selling_price
                                                               ORDER BY m_product.product_name";

        private const string SQL_TEMPLATE_CUSTOMER_PRODUK = @"SELECT m_product.product_id, m_product.product_name, 
                                                              COALESCE(m_customer.customer_id, m_customer.customer_id, '6ecdf4af-d9e1-8c33-f22a-3cb8e053c02a') AS customer_id,
                                                              COALESCE(m_customer.name_customer, m_customer.name_customer, '-') AS name_customer, m_customer.address, m_customer.phone,
                                                              SUM(t_sales_order_item.quantity - t_sales_order_item.return_quantity) AS quantity
                                                              FROM public.t_product_sales LEFT JOIN public.m_customer ON t_product_sales.customer_id = m_customer.customer_id
                                                              INNER JOIN public.t_sales_order_item ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                              INNER JOIN public.m_product ON t_sales_order_item.product_id = m_product.product_id
                                                              {WHERE}
                                                              GROUP BY m_product.product_id, m_product.product_name, COALESCE(m_customer.customer_id, m_customer.customer_id, '6ecdf4af-d9e1-8c33-f22a-3cb8e053c02a'), COALESCE(m_customer.name_customer, m_customer.name_customer, '-'), m_customer.address, m_customer.phone
                                                              ORDER BY m_product.product_name, COALESCE(m_customer.name_customer, m_customer.name_customer, '-')";

        private const string SQL_TEMPLATE_PER_golongan_HEADER = @"SELECT m_category.category_id, m_category.name_category,
                                                                  SUM((t_sales_order_item.quantity - t_sales_order_item.return_quantity) * (t_sales_order_item.selling_price - (t_sales_order_item.selling_price * t_sales_order_item.discount / 100))) AS sub_total
                                                                  FROM public.m_category INNER JOIN public.m_product ON m_product.category_id = m_category.category_id
                                                                  INNER JOIN public.t_sales_order_item ON t_sales_order_item.product_id = m_product.product_id
                                                                  INNER JOIN public.t_product_sales ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                                  {WHERE}
                                                                  GROUP BY m_category.category_id, m_category.name_category
                                                                  ORDER BY m_category.name_category";

        private const string SQL_TEMPLATE_PER_golongan_DETAIL = @"SELECT m_category.category_id, m_category.name_category, t_product_sales.date, m_product.product_id, m_product.product_name, COALESCE(t_sales_order_item.description, t_sales_order_item.description, '') AS description,  
                                                                  SUM(t_sales_order_item.quantity) AS quantity, SUM(t_sales_order_item.return_quantity) AS return_quantity, t_sales_order_item.purchase_price, t_sales_order_item.selling_price, t_sales_order_item.discount
                                                                  FROM public.m_category INNER JOIN public.m_product ON m_product.category_id = m_category.category_id
                                                                  INNER JOIN public.t_sales_order_item ON t_sales_order_item.product_id = m_product.product_id
                                                                  INNER JOIN public.t_product_sales ON t_sales_order_item.sale_id = t_product_sales.sale_id
                                                                  {WHERE}
                                                                  GROUP BY m_category.category_id, m_category.name_category, t_sales_order_item.description, t_product_sales.date, m_product.product_id, m_product.product_name, t_sales_order_item.purchase_price, t_sales_order_item.selling_price, t_sales_order_item.discount
                                                                  ORDER BY m_category.name_category, t_product_sales.date";

        private IDapperContext _context;
        private ILog _log;

        public ReportSellingQuotationRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public IList<ReportSalesProductHeader> GetByMonth(int month, int year)
        {
            IList<ReportSalesProductHeader> oList = new List<ReportSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProductHeader>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProductHeader> GetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportSalesProductHeader> oList = new List<ReportSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_product_sales.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProductHeader>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProductHeader> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProductHeader> oList = new List<ReportSalesProductHeader>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_HEADER);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportSalesProductHeader>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProductDetail> DetailGetByMonth(int month, int year)
        {
            IList<ReportSalesProductDetail> oList = new List<ReportSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProductDetail>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProductDetail> DetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportSalesProductDetail> oList = new List<ReportSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_product_sales.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProductDetail>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProductDetail> DetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProductDetail> oList = new List<ReportSalesProductDetail>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_DETAIL);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportSalesProductDetail>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerProductGetByMonth(int month, int year)
        {
            IList<ReportSalesProduct> oList = new List<ReportSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_PRODUK);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProduct>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerProductGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            IList<ReportSalesProduct> oList = new List<ReportSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_PRODUK);

                whereBuilder.Add("(EXTRACT(MONTH FROM t_product_sales.date) BETWEEN @StartingMonth AND @EndingMonth)");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProduct>(whereBuilder.ToSql(), new { StartingMonth, EndingMonth, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProduct> oList = new List<ReportSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_PRODUK);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportSalesProduct>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }


        public IList<ReportProductFavourite> ProductFavouriteGetByMonth(int month, int year, int limit)
        {
            IList<ReportProductFavourite> oList = new List<ReportProductFavourite>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PRODUK_FAVORIT);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportProductFavourite>(whereBuilder.ToSql(), new { month, year, limit })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportProductFavourite> ProductFavouriteGetByMonth(int StartingMonth, int EndingMonth, int year, int limit)
        {
            throw new NotImplementedException();
        }

        public IList<ReportProductFavourite> ProductFavouriteGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int limit)
        {
            IList<ReportProductFavourite> oList = new List<ReportProductFavourite>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PRODUK_FAVORIT);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportProductFavourite>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai, limit })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }


        public IList<ReportSalesPerCashier> PerCashierGetByMonth(int month, int year)
        {
            IList<ReportSalesPerCashier> oList = new List<ReportSalesPerCashier>();

            try
            {
                var whereBuilderHeader = new WhereBuilder(SQL_TEMPLATE_PER_KASIR_HEADER);

                whereBuilderHeader.Add("EXTRACT(MONTH FROM t_machine.date) = @month");
                whereBuilderHeader.Add("EXTRACT(YEAR FROM t_machine.date) = @year");

                var listOfHeader = _context.db.Query<dynamic>(whereBuilderHeader.ToSql(), new { month, year })
                                           .ToList();

                var whereBuilderDetail = new WhereBuilder(SQL_TEMPLATE_PER_KASIR_DETAIL);
                whereBuilderDetail.Add("EXTRACT(MONTH FROM t_machine.date) = @month");
                whereBuilderDetail.Add("EXTRACT(YEAR FROM t_machine.date) = @year");
                whereBuilderDetail.Add("t_machine.user_id = @kasir_id");

                foreach (var header in listOfHeader)
                {
                    var listOfDetail = _context.db.Query<ReportSalesPerCashier>(whereBuilderDetail.ToSql(), new { month, year, header.kasir_id })
                                               .ToList();

                    foreach (var detail in listOfDetail)
                    {
                        detail.kasir_id = header.kasir_id;
                        detail.Cashier = header.Cashier;
                        detail.starting_balance = (double)header.starting_balance;
                        detail.tax = (double)header.tax;
                        detail.diskon_nota = (double)header.diskon_nota;
                        detail.total_invoice = (double)header.total_invoice;

                        oList.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesPerCashier> PerCashierGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalesPerCashier> PerCashierGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesPerCashier> oList = new List<ReportSalesPerCashier>();

            try
            {
                var whereBuilderHeader = new WhereBuilder(SQL_TEMPLATE_PER_KASIR_HEADER);

                whereBuilderHeader.Add("t_machine.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                var listOfHeader = _context.db.Query<dynamic>(whereBuilderHeader.ToSql(), new { tanggalMulai, tanggalSelesai })
                                           .ToList();

                var whereBuilderDetail = new WhereBuilder(SQL_TEMPLATE_PER_KASIR_DETAIL);
                whereBuilderDetail.Add("t_machine.date BETWEEN @tanggalMulai AND @tanggalSelesai");
                whereBuilderDetail.Add("t_machine.user_id = @kasir_id");

                foreach (var header in listOfHeader)
                {
                    var listOfDetail = _context.db.Query<ReportSalesPerCashier>(whereBuilderDetail.ToSql(), new { tanggalMulai, tanggalSelesai, header.kasir_id })
                                               .ToList();

                    foreach (var detail in listOfDetail)
                    {
                        detail.kasir_id = header.kasir_id;
                        detail.Cashier = header.Cashier;
                        detail.starting_balance = (double)header.starting_balance;
                        detail.tax = (double)header.tax;
                        detail.diskon_nota = (double)header.diskon_nota;
                        detail.total_invoice = (double)header.total_invoice;

                        oList.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCustomerProduct> CustomerProductGetByMonth(int month, int year)
        {
            IList<ReportCustomerProduct> oList = new List<ReportCustomerProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_CUSTOMER_PRODUK);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportCustomerProduct>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportCustomerProduct> CustomerProductGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportCustomerProduct> CustomerProductGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportCustomerProduct> oList = new List<ReportCustomerProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_CUSTOMER_PRODUK);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportCustomerProduct>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }


        public IList<ReportSalesProductPerCategory> PerCategoryGetByMonth(int month, int year)
        {
            IList<ReportSalesProductPerCategory> oList = new List<ReportSalesProductPerCategory>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_golongan_HEADER);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProductPerCategory>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProductPerCategory> PerCategoryGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalesProductPerCategory> PerCategoryGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProductPerCategory> oList = new List<ReportSalesProductPerCategory>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_golongan_HEADER);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportSalesProductPerCategory>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerCategoryDetailGetByMonth(int month, int year)
        {
            IList<ReportSalesProduct> oList = new List<ReportSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_golongan_DETAIL);

                whereBuilder.Add("EXTRACT(MONTH FROM t_product_sales.date) = @month");
                whereBuilder.Add("EXTRACT(YEAR FROM t_product_sales.date) = @year");

                oList = _context.db.Query<ReportSalesProduct>(whereBuilder.ToSql(), new { month, year })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<ReportSalesProduct> PerCategoryDetailGetByMonth(int StartingMonth, int EndingMonth, int year)
        {
            throw new NotImplementedException();
        }

        public IList<ReportSalesProduct> PerCategoryDetailGetByDate(DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            IList<ReportSalesProduct> oList = new List<ReportSalesProduct>();

            try
            {
                var whereBuilder = new WhereBuilder(SQL_TEMPLATE_PER_golongan_DETAIL);

                whereBuilder.Add("t_product_sales.date BETWEEN @tanggalMulai AND @tanggalSelesai");

                oList = _context.db.Query<ReportSalesProduct>(whereBuilder.ToSql(), new { tanggalMulai, tanggalSelesai })
                                .ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }
    }
}
