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
using System.Threading.Tasks;

using log4net;
using Dapper;
using Dapper.Contrib.Extensions;

using SparkPOS.Model;
using SparkPOS.Repository.Api;
using System.Data;
 
namespace SparkPOS.Repository.Service
{        
    public class ProductRepository : IProductRepository
    {
        private const string SQL_TEMPLATE = @"SELECT m_product.product_id, m_product.product_code, m_product.product_name, m_product.unit, m_product.stock, m_product.purchase_price, m_product.selling_price, m_product.discount, m_product.profit_percentage,
                                              m_product.minimal_stock, m_product.warehouse_stock, m_product.minimal_stock_warehouse, m_product.is_active,m_product.last_update,
                                              m_category.category_id, m_category.name_category, m_category.discount
                                              FROM m_product LEFT JOIN public.m_category ON m_product.category_id = m_category.category_id
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private const string SQL_TEMPLATE_FOR_PAGING = @"SELECT COUNT(*) 
                                                         FROM m_product LEFT JOIN public.m_category ON m_product.category_id = m_category.category_id
                                                         {WHERE}";

        private IDapperContext _context;
        private ILog _log;

        private string _sql;

        public ProductRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        private IEnumerable<Product> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Product> oList = _context.db.Query<Product, Category, Product>(sql, (p, g) =>
            {
                if (g != null)
                {
                    p.category_id = g.category_id; p.Category = g;
                }
                
                return p;
            }, param, splitOn: "category_id");

            return oList;
        }

        private PriceWholesale GetPriceWholesale(string produkId, int hargaKe, IDbTransaction transaction = null)
        {
            IPriceWholesaleRepository repo = new PriceWholesaleRepository(_context, _log);

            return repo.GetPriceWholesale(produkId, hargaKe, transaction);
        }

        private IList<PriceWholesale> GetListPriceWholesale(string produkId)
        {
            IPriceWholesaleRepository repo = new PriceWholesaleRepository(_context, _log);
        
            return repo.GetListPriceWholesale(produkId);
        }

        private IList<PriceWholesale> GetListPriceWholesale(string[] listOfProductId)
        {
            IPriceWholesaleRepository repo = new PriceWholesaleRepository(_context, _log);

            return repo.GetListPriceWholesale(listOfProductId);
        }

        public Product GetByID(string id)
        {
            Product obj = null;
            
            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_product.product_id = @id");
                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");
                
                obj = MappingRecordToObject(_sql, new { id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }
            
            return obj;
        }

        public Product GetByCode(string codeProduct, bool isCekStatusActive = false)
        {
            Product obj = null;

            try
            {
                if (isCekStatusActive)
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_product.product_code) = @codeProduct AND m_product.is_active = true");
                else
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_product.product_code) = @codeProduct");

                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");

                codeProduct = codeProduct.ToLower();

                obj = MappingRecordToObject(_sql, new { codeProduct }).SingleOrDefault();

                if (obj != null)
                    obj.list_of_harga_grosir = GetListPriceWholesale(obj.product_id).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public string GetLastCodeProduct()
        {
            return _context.GetLastInvoice(new Product().GetTableName());
        }

        private void SetPriceWholesale(IList<Product> listOfProduct)
        {
            var listOfProductId = new List<string>();

            foreach (var product in listOfProduct)
            {
                listOfProductId.Add(product.product_id);
            }

            var listOfPriceWholesale = GetListPriceWholesale(listOfProductId.ToArray());

            foreach (var product in listOfProduct)
            {
                product.list_of_harga_grosir = listOfPriceWholesale.Where(f => f.product_id == product.product_id)
                                                               .OrderBy(f => f.retail_price)
                                                               .ToList();
            }
        }

        public IList<Product> GetByName(string name, bool isLoadPriceWholesale = true, bool isCekStatusActive = false)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                if (isCekStatusActive)
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE (LOWER(m_product.product_name) LIKE @name OR LOWER(m_product.product_code) LIKE @name) AND m_product.is_active = true");
                else
                    _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_product.product_name) LIKE @name OR LOWER(m_product.product_code) LIKE @name");

                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_product.product_name");
                _sql = _sql.Replace("{OFFSET}", "");

                name = "%" + name.ToLower() + "%";

                oList = MappingRecordToObject(_sql, new { name }).ToList();

                if (isLoadPriceWholesale) SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetByName(string name, string sortBy, int pageNumber, int pageSize, ref int pagesCount, bool isLoadPriceWholesale = true)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                name = "%" + name.ToLower() + "%";

                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE LOWER(m_product.product_name) LIKE @name OR LOWER(m_product.product_code) LIKE @name");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { name });

                sortBy = string.Format("ORDER BY {0}", sortBy);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE LOWER(m_product.product_name) LIKE @name OR LOWER(m_product.product_code) LIKE @name");
                _sql = _sql.Replace("{ORDER BY}", sortBy);
                _sql = _sql.Replace("{OFFSET}", "OFFSET @pageSize * (@pageNumber - 1) LIMIT @pageSize");                

                oList = MappingRecordToObject(_sql, new { name, pageNumber, pageSize }).ToList();

                if (isLoadPriceWholesale) SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetByCategory(string golonganId)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_product.category_id = @golonganId");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_product.product_name");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql, new { golonganId }).ToList();

                SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetByCategory(string golonganId, string sortBy, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "WHERE m_product.category_id = @golonganId");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize, new { golonganId });

                sortBy = string.Format("ORDER BY {0}", sortBy);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_product.category_id = @golonganId");
                _sql = _sql.Replace("{ORDER BY}", sortBy);
                _sql = _sql.Replace("{OFFSET}", "OFFSET @pageSize * (@pageNumber - 1) LIMIT @pageSize");

                oList = MappingRecordToObject(_sql, new { golonganId, pageNumber, pageSize }).ToList();

                SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetInfoMinimalStock()
        {
            IList<Product> oList = new List<Product>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE m_product.minimal_stock_warehouse > 0 AND (m_product.stock + m_product.warehouse_stock) <= m_product.minimal_stock_warehouse");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_product.product_name");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetAll()
        {
            IList<Product> oList = new List<Product>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY m_product.product_name");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();

                SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetAll(string sortBy)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                sortBy = string.Format("ORDER BY {0}", sortBy);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", sortBy);
                _sql = _sql.Replace("{OFFSET}", "");

                oList = MappingRecordToObject(_sql).ToList();

                SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Product> GetAll(string sortBy, int pageNumber, int pageSize, ref int pagesCount)
        {
            IList<Product> oList = new List<Product>();

            try
            {
                var sqlPageCount = SQL_TEMPLATE_FOR_PAGING.Replace("{WHERE}", "");
                pagesCount = _context.GetPagesCount(sqlPageCount, pageSize);

                sortBy = string.Format("ORDER BY {0}", sortBy);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", sortBy);
                _sql = _sql.Replace("{OFFSET}", "OFFSET @pageSize * (@pageNumber - 1) LIMIT @pageSize");                

                oList = MappingRecordToObject(_sql, new { pageNumber, pageSize }).ToList();

                SetPriceWholesale(oList);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        private bool IsExist(string codeProduct)
        {
            var count = _context.db.GetAll<Product>()
                                .Where(f => f.product_code == codeProduct)
                                .Count();

            return count > 0;
        }

        public int Save(Product obj)
        {
            var result = 0;

            try
            {
                if (!IsExist(obj.product_code))
                {
                    if (obj.product_id == null)
                        obj.product_id = _context.GetGUID();

                    _context.BeginTransaction();
                    
                    var transaction = _context.transaction;

                    _context.db.Insert<Product>(obj, transaction);

                    foreach (var item in obj.list_of_harga_grosir)
                    {
                        var hargaWholesale = GetPriceWholesale(obj.product_id, item.retail_price, transaction);

                        if (hargaWholesale == null)
                        {
                            if (item.wholesale_price_id == null)
                                item.wholesale_price_id = _context.GetGUID();

                            item.product_id = obj.product_id;

                            _context.db.Insert<PriceWholesale>(item, transaction);
                        }
                    }

                    _context.Commit();

                    result = 1;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(Product obj)
        {
            var result = 0;

            try
            {
                if (!(IsExist(obj.product_code) && obj.product_code != obj.code_produk_old))
                {
                    _context.BeginTransaction();

                    var transaction = _context.transaction;

                    result = _context.db.Update<Product>(obj, transaction) ? 1 : 0;

                    foreach (var item in obj.list_of_harga_grosir)
                    {
                        item.product_id = obj.product_id;

                        var hargaWholesale = GetPriceWholesale(obj.product_id, item.retail_price, transaction);
                        
                        if (hargaWholesale == null)
                        {
                            if (item.wholesale_price_id == null)
                                item.wholesale_price_id = _context.GetGUID();                            

                            _context.db.Insert<PriceWholesale>(item, transaction);
                            result = 1;
                        }
                        else
                        {
                            result = _context.db.Update<PriceWholesale>(item, transaction) ? 1 : 0;
                        }
                    }

                    _context.Commit();

                    result = 1;
                }
            }
            catch (Exception ex)
            {
                result = 0;
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(Product obj)
        {
            var result = 0;

            try
            {
                result = _context.db.Delete<Product>(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }        
    }
}     
