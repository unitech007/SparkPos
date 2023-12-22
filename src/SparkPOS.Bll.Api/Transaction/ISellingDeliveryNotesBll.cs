

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SparkPOS.Model;
using SparkPOS.Model.Transaction;

namespace SparkPOS.Bll.Api
{
    public interface ISellingDeliveryNotesBll : IBaseBll<SellingDeliveryNotes>
    {
        string GetLastDeliveryNotes();
        SellingDeliveryNotes GetByID(string id);

        List<string> GetInvoiceByCustomerId(string customerId);

        /// <summary>
        /// Method untuk mendapatkan Information item invoice terakhir untuk keperluan application Cashier
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mesinId"></param>
        /// <returns></returns>
        SellingDeliveryNotes GetListItemInvoiceTerakhir(string userId, string mesinId);
        List<ItemSellingProduct> GetProductDetailsByInvoice(string invoiceNumber);
        IList<SellingDeliveryNotes> GetAll(string name);
        IList<SellingDeliveryNotes> GetAll(int pageNumber, int pageSize, ref int pagesCount);

        IList<SellingDeliveryNotes> GetInvoiceCustomer(string id, string invoice);
        IList<SellingDeliveryNotes> GetInvoiceKreditByCustomer(string id, bool isLunas);
        IList<SellingDeliveryNotes> GetInvoiceKreditByInvoice(string id, string invoice);
        IList<SellingDeliveryNotes> GetByName(string name);
        IList<SellingDeliveryNotes> GetByName(string name, bool isCekKeteranganItemSelling, int pageNumber, int pageSize, ref int pagesCount);
        IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai);
        IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, int pageNumber, int pageSize, ref int pagesCount);
        IList<SellingDeliveryNotes> GetByDate(DateTime tanggalMulai, DateTime tanggalSelesai, string name);

        IList<SellingDeliveryNotes> GetByLimit(DateTime tanggalMulai, DateTime tanggalSelesai, int limit);

        //IList<ItemSellingDeliveryNotes> GetItemSelling(string jualId);

        int Save(SellingDeliveryNotes obj, ref ValidationError validationError);
        int Update(SellingDeliveryNotes obj, ref ValidationError validationError);
    }
}
