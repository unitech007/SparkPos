
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Reporting.WinForms;
using System.Reflection;
using System.IO;

using log4net;
using SparkPOS.Helper;
using SparkPOS.Model;
using SparkPOS.Bll.Api;
using SparkPOS.Bll.Service;
using ConceptCave.WaitCursor;
using System.Drawing.Imaging;
using SparkPOS.Report.DataSet;
using ZXing;
using ZXing.Common;
using QRCoder;
using SparkPOS.App.References;
using MultilingualApp;

namespace SparkPOS.App.Transaction
{
    public partial class FrmPreviewInvoiceSales : Form
    {
        private string _reportNameSpace = @"SparkPOS.Report.{0}.rdlc";
        private Assembly _assemblyReport;

        private ILog _log;
        private Customer _customer = null;
        private SellingProduct _jual = null;
        private User _user;
        private Profil _profil;
        private GeneralSupplier _GeneralSupplier;
        public FrmPreviewInvoiceSales()
        {
             InitializeComponent(); 
            //this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.ZoomMode = ZoomMode.Percent;
            this.reportViewer1.ZoomPercent = 100;
            this.reportViewer1.RefreshReport();
            this.reportViewer1.LocalReport.EnableExternalImages = true;
            ColorManagerHelper.SetTheme(this, this);
            _assemblyReport = Assembly.LoadFrom("SparkPOS.Report.dll");
            MainProgram.GlobalLanguageChange(this);
        }


        public FrmPreviewInvoiceSales(string header, SellingProduct sale)
            : this()
        {
            //this.Text = header;
            //this.lblHeader.Text = header;
            this._log = MainProgram.log;
            this._user = MainProgram.user;
            this._profil = MainProgram.profil;
            this._GeneralSupplier = MainProgram.GeneralSupplier;
            this._jual = sale;
            this._customer = this._jual.Customer;

            chkIsSdac.Checked = this._jual.is_sdac;
            chkIsSdac_CheckedChanged(chkIsSdac, new EventArgs());
            btnPreviewInvoice_Click(btnPreviewInvoice, new EventArgs());
            MainProgram.GlobalLanguageChange(this);
            this.Text = LanguageHelper.TranslateText(header, MainProgram.currentLanguage);
            this.lblHeader.Text = LanguageHelper.TranslateText(header, MainProgram.currentLanguage);
        }

        private void chkIsSdac_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;

            txtto1.Enabled = !chk.Checked;
            txtto2.Enabled = !chk.Checked;
            txtto3.Enabled = !chk.Checked;
            txtto4.Enabled = !chk.Checked;

            var to1 = _customer.name_customer.NullToString();
            var to2 = _customer.address.NullToString();
            var to3 = _customer.get_region_lengkap;
            var to4 = string.Format("HP: {0}", _customer.phone.NullToString());

            if (!chk.Checked) // address shipping tidak sama dengan address customer
            {
                to1 = string.IsNullOrEmpty(_jual.shipping_to) ? to1 : _jual.shipping_to;
                to2 = string.IsNullOrEmpty(_jual.shipping_address) ? to2 : _jual.shipping_address;
                to3 = string.IsNullOrEmpty(_jual.shipping_subdistrict) ? to3 : _jual.shipping_subdistrict;
                to4 = string.IsNullOrEmpty(_jual.shipping_village) ? to4 : _jual.shipping_village;
            }

            txtto1.Text = to1;
            txtto2.Text = to2;
            txtto3.Text = to3;
            txtto4.Text = to4;
        }

        private void btnPreviewInvoice_Click(object sender, EventArgs e)
        {
            _jual.is_sdac = chkIsSdac.Checked;

            if (!chkIsSdac.Checked) // address shipping tidak sama dengan address customer
            {
                _jual.shipping_to = txtto1.Text;
                _jual.shipping_address = txtto2.Text;
                _jual.shipping_subdistrict = txtto3.Text;
                _jual.shipping_village = txtto4.Text;
            }

            using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
            {
                PreviewInvoice(_jual);
            }
        }
        //private void PreviewInvoice(SellingProduct sale, bool isPreview = true)
        //{
        //    try
        //    {
        //        IPrintInvoiceBll printBll = new PrintInvoiceBll(_log);
        //        var listOfItemInvoice = printBll.GetInvoiceSales(sale.sale_id);
        //        string qrCodeData = "Your QR code data"; // Replace with your own data
        //        QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
        //        QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(qrCodeData, QRCoder.QRCodeGenerator.ECCLevel.Q);
        //        QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);
        //        Bitmap bmp = qRCode.GetGraphic(5);
        //        DataSet1 reportData = new DataSet1();
        //        // Add QR code image to the report
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            bmp.Save(ms, ImageFormat.Bmp);

        //            DataSet1.QRCodeRow qRCodeRow = reportData.QRCode.NewQRCodeRow();
        //            qRCodeRow.image = ms.ToArray();
        //            reportData.QRCode.AddQRCodeRow(qRCodeRow);
        //            // listOfItemInvoice.ToList().ForEach((c) => { c.qrCode = ms.ToArray() }) ;
        //            // listOfItemInvoice.ToList().ForEach(c => c.qrCode = ms.ToArray());

        //            //ReportDataSource qrCodeDataSource = new ReportDataSource();
        //            //qrCodeDataSource.Name = "RvInvoiceSalesProductTanpaLabel";
        //            //qrCodeDataSource.Value = reportData.QRCode;

        //            //// Add the QR code data source to the report
        //            //this.reportViewer1.LocalReport.DataSources.Add(qrCodeDataSource);
        //        }

        //        if (listOfItemInvoice.Count > 0)
        //        {

        //            var reportDataSource = new ReportDataSource
        //            {
        //                Name = "InvoiceSales",
        //                Value = listOfItemInvoice
        //            };

        //            //var reportDataSource1 = new ReportDataSource
        //            //{
        //            //    Name = "DateSet1",
        //            //    Value = reportData
        //            //};

        //            // set header invoice
        //            var parameters = new List<ReportParameter>();
        //            var index = 1;

        //            foreach (var item in _GeneralSupplier.list_of_header_nota)
        //            {
        //                var paramName = string.Format("header{0}", index);
        //                parameters.Add(new ReportParameter(paramName, item.description));

        //                index++;
        //            }

        //            foreach (var item in listOfItemInvoice)
        //            {
        //                item.is_sdac = chkIsSdac.Checked;

        //                if (!_GeneralSupplier.is_print_keterangan_nota)
        //                    item.description = string.Empty;

        //                if (!chkIsSdac.Checked)
        //                {
        //                    item.shipping_to = txtto1.Text;
        //                    item.shipping_address = txtto2.Text;
        //                    item.shipping_subdistrict = txtto3.Text;
        //                    item.shipping_village = txtto4.Text;
        //                }
        //            }

        //            // set footer invoice
        //            var dt = DateTime.Now;
        //            var cityAndDate = string.Format("{0}, {1}", _profil.city, dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);

        //            parameters.Add(new ReportParameter("city", cityAndDate));
        //            parameters.Add(new ReportParameter("footer", _user.name_user));

        //            var reportName = sale.is_dropship ? "RvInvoiceSalesProductTanpaLabelDropship" : "RvInvoiceSalesProductTanpaLabel";

        //            if (isPreview)
        //            {
        //                reportName = string.Format(_reportNameSpace, reportName);
        //                var stream = _assemblyReport.GetManifestResourceStream(reportName);

        //                this.reportViewer1.LocalReport.DataSources.Clear();
        //                this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);
        //               // this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
        //                this.reportViewer1.LocalReport.LoadReportDefinition(stream);

        //                //this.reportViewer2.LocalReport.DataSources.Clear();
        //                //this.reportViewer2.LocalReport.DataSources.Add(reportDataSource);
        //                //this.reportViewer2.LocalReport.LoadReportDefinition(stream);

        //                // Generate QR code image
        //                //string qrCodeData = "Your QR code data"; // Replace with your own data
        //                //QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
        //                //QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(qrCodeData, QRCoder.QRCodeGenerator.ECCLevel.Q);
        //                //QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);
        //                //Bitmap bmp = qRCode.GetGraphic(5);

        //                //// Add QR code image to the report
        //                //using (MemoryStream ms = new MemoryStream())
        //                //{
        //                //    bmp.Save(ms, ImageFormat.Bmp);
        //                //    DataSet1 reportData = new DataSet1();
        //                //    DataSet1.QRCodeRow qRCodeRow = reportData.QRCode.NewQRCodeRow();
        //                //    qRCodeRow.image = ms.ToArray();
        //                //    reportData.QRCode.AddQRCodeRow(qRCodeRow);

        //                //    ReportDataSource qrCodeDataSource = new ReportDataSource();
        //                //    qrCodeDataSource.Name = "RvInvoiceSalesProductTanpaLabel";
        //                //    qrCodeDataSource.Value = reportData.QRCode;

        //                //    // Add the QR code data source to the report
        //                //    this.reportViewer1.LocalReport.DataSources.Add(qrCodeDataSource);
        //                //}


        //                if (!(parameters == null))
        //                    this.reportViewer1.LocalReport.SetParameters(parameters);
        //                // this.reportViewer2.LocalReport.SetParameters(parameters);

        //                this.reportViewer1.RefreshReport();
        //                //this.reportViewer2.RefreshReport();

        //            }
        //            else
        //            {
        //                var printReport = new ReportViewerPrintHelper(reportName, reportDataSource, parameters, _GeneralSupplier.name_printer);
        //                printReport.Print();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}

        public byte[] GenerateQRCodeBytes(string qrCodeData)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new EncodingOptions { Width = 300, Height = 300 }; // Adjust size as needed
            var qrCodeImage = barcodeWriter.Write(qrCodeData);

            byte[] qrCodeBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                qrCodeBytes = stream.ToArray();
            }

            return qrCodeBytes;
        }

        private void PreviewInvoice(SellingProduct sale, bool isPreview = true)
        {
            try
            {
                IPrintInvoiceBll printBll = new PrintInvoiceBll(_log);
                var listOfItemInvoice = printBll.GetInvoiceSales(sale.sale_id);
                // Generate the QR code

                if (listOfItemInvoice.Count > 0)
                {

                    var reportDataSource = new ReportDataSource
                    {
                        Name = "InvoiceSales",
                        Value = listOfItemInvoice
                    };

                 
                    string code = "QR CODE DETAILS:\n\n";

                    foreach (var item in listOfItemInvoice)
                    {
                        string productInfo = $"ITEM PRODUCT NAME: {item.product_name}\nINVOICE NO: {item.invoice}\nVAT TOTAL: {item.tax}\nINVOICE TOTAL (WITH VAT): {item.total_invoice + item.tax}\n\n";
                        item.sub_total_with_tax = item.total_invoice + item.tax;
                        // Modify the text style
                        code += productInfo;
                    }

                   
                    
                    int desiredWidth = 150;
                    int desiredHeight = 150;
                    // string savePath = Application.StartupPath + "\\test.png";
                    //string savePath = Path.Combine(Application.StartupPath, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
                    //string savePath = Path.Combine(Application.StartupPath, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
                    //string savePath = Path.Combine(Application.StartupPath, "QrCode_" +  DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
                    //System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location
                    string savePath = Path.Combine(
       System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
       "QrCode_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png"
   );
                  //  MessageBox.Show(savePath);
                  
                    var qrcode = new QrCodeGeneration();
                    QrCodeGeneration qrcode1 = new QrCodeGeneration();
                    // qrcode = new QrCodeGeneration();
                    qrcode.GenerateQRCode(code, savePath, desiredWidth, desiredHeight);
                    //qrcode.GenerateQRCode(qrCodeData, savePath, desiredWidth, desiredHeight);

                    var parameters = new List<ReportParameter>();
                    var index = 1;

                    foreach (var item in _GeneralSupplier.list_of_header_nota)
                    {
                        var paramName = string.Format("header{0}", index);
                        parameters.Add(new ReportParameter(paramName, item.description));

                        index++;
                    }

                    foreach (var item in listOfItemInvoice)
                    {
                        item.is_sdac = chkIsSdac.Checked;

                        if (!_GeneralSupplier.is_print_keterangan_nota)
                            item.description = string.Empty;

                        if (!chkIsSdac.Checked)
                        {
                            item.shipping_to = txtto1.Text;
                            item.shipping_address = txtto2.Text;
                            item.shipping_subdistrict = txtto3.Text;
                            item.shipping_village = txtto4.Text;
                        }
                    }
                    var dt = DateTime.Now;
                    var cityAndDate = string.Format("{0}, {1}", _profil.city, dt.Day + " " + DayMonthHelper.GetMonthIndonesia(dt.Month) + " " + dt.Year);
                    parameters.Add(new ReportParameter("city", cityAndDate));
                    parameters.Add(new ReportParameter("footer", _user.name_user));
                    string qrCodeImagePath = "file:\\" + savePath;
                    parameters.Add(new ReportParameter("QR_Img", qrCodeImagePath, true));
                    var reportName = sale.is_dropship ? "RvInvoiceSalesProductTanpaLabelDropship" : "RvInvoiceSalesProductTanpaLabel";
                    
                    if (isPreview)
                    {
                        reportName = string.Format(_reportNameSpace, reportName);
                        var stream = _assemblyReport.GetManifestResourceStream(reportName);
                        stream = RdlcReportHelper.TranslateReport(stream, MainProgram.currentLanguage);

                        this.reportViewer1.LocalReport.DataSources.Clear();
                        this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                        this.reportViewer1.LocalReport.LoadReportDefinition(stream);
                        if (!(parameters == null))
                            this.reportViewer1.LocalReport.SetParameters(parameters);

                        this.reportViewer1.RefreshReport();

                    }
                    else
                    {
                        var printReport = new ReportViewerPrintHelper(reportName, reportDataSource, parameters, _GeneralSupplier.name_printer);
                        printReport.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                MainProgram.LogException(ex);
                MsgHelper.MsgError(ex.ToString());

            }

        }
        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (MsgHelper.MsgConfirmation("Do you want to continue the printing process ?"))
            {
                using (new StCursor(Cursors.WaitCursor, new TimeSpan(0, 0, 0, 0)))
                {
                    ISellingProductBll bll = new SellingProductBll(_log);
                    var result = bll.Update(_jual);

                    PreviewInvoice(_jual, false);
                }
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSelesai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmPreviewInvoiceSales_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressHelper.IsEsc(e))
                this.Close();
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void FrmPreviewInvoiceSales_Load(object sender, EventArgs e)
        {

        }
    }

}
