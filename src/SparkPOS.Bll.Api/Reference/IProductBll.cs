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

using SparkPOS.Model;
 
namespace SparkPOS.Bll.Api
{    
    public interface IProductBll : IBaseBll<Product>
    {
        Product GetByID(string id);
        Product GetByCode(string codeProduct, bool isCekStatusActive = false);
        IList<Product> GetByName(string name, bool isLoadPriceWholesale = true, bool isCekStatusActive = false);
        IList<Product> GetByName(string name, int sortByIndex, int pageNumber, int pageSize, ref int pagesCount, bool isLoadPriceWholesale = true);
        IList<Product> GetByCategory(string golonganId);
        IList<Product> GetByCategory(string golonganId, int sortByIndex, int pageNumber, int pageSize, ref int pagesCount);
        IList<Product> GetInfoMinimalStock();
        IList<Product> GetAll(int sortByIndex);
        IList<Product> GetAll(int sortByIndex, int pageNumber, int pageSize, ref int pagesCount);
        string GetLastCodeProduct();

		int Save(Product obj, ref ValidationError validationError);
		int Update(Product obj, ref ValidationError validationError);
    }
}     
