using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ZXing.Common;
using ZXing.Rendering;
using ZXing.QrCode;
using ZXing;

namespace Common.Utils
{
    public class EncoderHelper
    {
        public static void GenerateBarCodePicture(string content, string savePath)
        {
            EncodeAndSave(content, savePath, BarcodeFormat.CODE_128);
        }

        public static void GenerateQRCodePicture(string content, string savePath)
        {
            EncodeAndSave(content, savePath, BarcodeFormat.QR_CODE);
        }

        private static void EncodeAndSave(string content, string savePath, BarcodeFormat format)
        {
            var writer = new BarcodeWriter
            {
                Format = format,
                /*Options = new EncodingOptions
                {
                    Height = picEncodedBarCode.Height,
                    Width = picEncodedBarCode.Width
                },*/
                Renderer = (IBarcodeRenderer<Bitmap>)Activator.CreateInstance(typeof(BitmapRenderer))
            };
            Bitmap img = writer.Write(content);
            img.Save(savePath);
        }
    }
}
