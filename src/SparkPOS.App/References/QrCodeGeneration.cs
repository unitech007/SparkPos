
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SparkPOS.App.References
{
    public class QrCodeGeneration
    {
        public Image ResizeImage(Image imgToResize, Size size)
        {
            // Create a new Bitmap object with the desired size
            Image resizedImage = new Bitmap(size.Width, size.Height);

            // Create a Graphics object from the resized image
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                // Configure the graphics settings for resizing
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // Draw the original image onto the resized image
                graphics.DrawImage(imgToResize, new Rectangle(0, 0, size.Width, size.Height));
            }

            // Return the resized image
            return resizedImage;
        }

        public void GenerateQRCode(string code, string savePath, int desiredWidth, int desiredHeight)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                // Resize the QR code image
                Image resizedImage = ResizeImage(qrCodeImage, new Size(desiredWidth, desiredHeight));

                // Save the resized image to the specified path
                resizedImage.Save(savePath, ImageFormat.Jpeg);
            }
        }                                                                                                                                    




    }


}

