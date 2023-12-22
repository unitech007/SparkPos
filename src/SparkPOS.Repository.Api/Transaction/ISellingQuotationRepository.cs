

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SparkPOS.Model.Transaction;

namespace SparkPOS.Repository.Api
{
    public interface ISellingQuotationRepository : IBaseRepository<SellingQuotation>
    {
        string GetLastQuotation();
        SellingQuotation GetByID(string id);

        /// <summary>
        /// Method untuk mendapatkan Information item invoice terakhir untuk keperluan application Cashier
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mesinId"></param>
        /// <returns></returns>
        SellingQuotation GetListItemInvoiceTerakhir(string userId, string mesinId);

        IList<SellingQuotation> GetAll(string name);
        IList<SellingQuotation> GetAll(int pageNumber, int pageSize, ref int pagesCount);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase masing-masing supplier
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        IList<SellingQuotation> GetInvoiceCustomer(string id, string invoice);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase Credit yang lunas/Not yet masing-masing supplier
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="isLunas"></param>
        /// <returns></returns>
        IList<SellingQuotation> GetInvoiceKreditByCustomer(string id, bool isLunas);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase Credit yang lunas/Not yet masing-masing customer based invoice
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        IList<SellingQuotation> GetInvoiceKreditByInvoice(string id, string invoice);

        IList<SellingQuotation> GetByName(string name);
        IList<SellingQuotation> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingQuotation> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name);

        IList<SellingQuotation> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit);

        IList<ItemSellingQuotation> GetItemSelling(string jualId);
    }
}
