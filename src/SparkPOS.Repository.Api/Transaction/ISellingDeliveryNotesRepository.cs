

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparkPOS.Model;
using SparkPOS.Model.Transaction;

namespace SparkPOS.Repository.Api
{
    public interface ISellingDeliveryNotesRepository : IBaseRepository<SellingDeliveryNotes>
    {
        string GetLastDeliveryNotes();
        SellingDeliveryNotes GetByID(string id);
       // List<ItemSellingProduct> GetProductDetailsByInvoice(string invoiceNumber);
        List<ItemSellingProduct> GetProductDetailsByInvoice(string invoiceNumber);
        List<string> GetInvoiceByCustomerId(string customerId);
        /// <summary>
        /// Method untuk mendapatkan Information item invoice terakhir untuk keperluan application Cashier
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mesinId"></param>
        /// <returns></returns>
        SellingDeliveryNotes GetListItemInvoiceTerakhir(string userId, string mesinId);

        IList<SellingDeliveryNotes> GetAll(string name);
        IList<SellingDeliveryNotes> GetAll(int pageNumber, int pageSize, ref int pagesCount);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase masing-masing supplier
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        IList<SellingDeliveryNotes> GetInvoiceCustomer(string id, string invoice);
       // double GetTotalInvoice(SellingDeliveryNotes obj);
        /// <summary>
        /// Method untuk mendapatkan Information Purchase Credit yang lunas/Not yet masing-masing supplier
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="isLunas"></param>
        /// <returns></returns>
        IList<SellingDeliveryNotes> GetInvoiceKreditByCustomer(string id, bool isLunas);

        /// <summary>
        /// Method untuk mendapatkan Information Purchase Credit yang lunas/Not yet masing-masing customer based invoice
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        IList<SellingDeliveryNotes> GetInvoiceKreditByInvoice(string id, string invoice);

        IList<SellingDeliveryNotes> GetByName(string name);
        IList<SellingDeliveryNotes> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name);

        IList<SellingDeliveryNotes> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit);
        //IList<ItemSellingDeliveryNotes> GetItemDeliveryNotes(string deliveryNotesId);

        // IList<ItemSellingDeliveryNotes> GetItemSelling(string jualId);
    }
}
