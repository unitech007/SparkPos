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

using System.Data;
using SparkPOS.Model;
using SparkPOS.Repository.Api;
 
namespace SparkPOS.Repository.Service
{        
    public class PriceWholesaleRepository : IPriceWholesaleRepository
    {
        private IDapperContext _context;
		private ILog _log;
		
        public PriceWholesaleRepository(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public PriceWholesale GetPriceWholesale(string produkId, int hargaKe, IDbTransaction transaction = null)
        {
            PriceWholesale obj = null;

            try
            {
                var sql = @"SELECT wholesale_price_id, product_id, retail_price, wholesale_price, minimum_quantity, discount 
                            FROM m_wholesale_price 
                            WHERE product_id = @produkId AND retail_price = @hargaKe";

                obj = _context.db.Query<PriceWholesale>(sql, new { produkId, hargaKe }, transaction)
                              .SingleOrDefault();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return obj;
        }

        public IList<PriceWholesale> GetListPriceWholesale(string produkId)
        {
            IList<PriceWholesale> oList = new List<PriceWholesale>();

            try
            {
                var sql = @"SELECT wholesale_price_id, product_id, retail_price, wholesale_price, minimum_quantity, discount 
                            FROM m_wholesale_price 
                            WHERE product_id = @produkId
                            ORDER BY retail_price";

                oList = _context.db.Query<PriceWholesale>(sql, new { produkId })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }

        public IList<PriceWholesale> GetListPriceWholesale(string[] listOfProductId)
        {
            IList<PriceWholesale> oList = new List<PriceWholesale>();

            try
            {
                var sql = @"SELECT wholesale_price_id, product_id, retail_price, wholesale_price, minimum_quantity, discount 
                            FROM m_wholesale_price 
                            WHERE product_id = ANY(@listOfProductId)";

                oList = _context.db.Query<PriceWholesale>(sql, new { listOfProductId })
                                .ToList();
            }
            catch (Exception ex)
            {
                 DapperContext.LogException(ex);
            }

            return oList;
        }
    }
}     
