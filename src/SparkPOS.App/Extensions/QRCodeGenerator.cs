using QRCoder;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

namespace SparkPOS.App
{
    public class QRCodeGenerator
    {
        public byte[] GenerateQRCode(string data, int size)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = size,
                    Width = size
                }
            };

            //Bitmap qrCodeImage = barcodeWriter.Write(data);

            //string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qrcode.png");

            Bitmap qrCodeImage = barcodeWriter.Write(data);

            string imagePath = ConfigurationManager.AppSettings["QRCodeImagePath"];
            qrCodeImage.Save(imagePath, ImageFormat.Png);

            return File.ReadAllBytes(imagePath);
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    qrCodeImage.Save(stream, ImageFormat.Png);
            //    return stream.ToArray();
            //}
        }

        //public string GenerateQRCodeBase64(string data, int size)
        //{
        //    BarcodeWriter barcodeWriter = new BarcodeWriter
        //    {
        //        Format = BarcodeFormat.QR_CODE,
        //        Options = new EncodingOptions
        //        {
        //            Height = size,
        //            Width = size
        //        }
        //    };

        //    Bitmap qrCodeImage = barcodeWriter.Write(data);

        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        qrCodeImage.Save(memoryStream, ImageFormat.Png);
        //        byte[] imageBytes = memoryStream.ToArray();
        //        return Convert.ToBase64String(imageBytes);
        //    }
        //}
        public string GenerateQRCodeBase64(string data, int size)
        {
            QRCoder.QRCodeGenerator qrCodeGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(data, QRCoder.QRCodeGenerator.ECCLevel.L); // Use default error correction level (L)
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(5);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }


    }
}
