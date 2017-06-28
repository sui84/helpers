using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Foxit.PDF;
using Foxit.PDF.Cryptography;
using Foxit.PDF.Merger;
using LumenWorks.Framework.IO.Csv;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Collections.Generic;

namespace Common.Utils{

    public class PDFHelper
    {
        public static void OutputPDF(byte[] fileContents, string filePath)
        {
            byte[] bytes3 = fileContents;
            FileStream fs3 = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fs3.Write(bytes3, 0, bytes3.Length);
            fs3.Dispose();
            fs3.Close();
        }

        public static List<string> GenerdateEncryPDFs(List<byte[]> files, string tempFolder, string encryKey)
        {
            List<string> filePath = new List<string>();
            DateTime today = DateTime.Now;
            for (int i = 0; i < files.Count(); i++)
            {
                string fileName = today.ToString("yyyyMMddHHmmss_") + "_" + i.ToString() + ".PDF";
                string file = tempFolder + fileName;
                PDFHelper.EncryPDF(files[i], file, encryKey);
                filePath.Add(file);
            }
            return filePath;
        }

        public static void EncryPDF(List<byte[]> files, string filePath, string encryKey)
        {
            Aes128Security security = new Aes128Security(encryKey, encryKey);
            security.DocumentComponents = EncryptDocumentComponents.All;
            MergeDocument mergeDoc = new MergeDocument(); 
            for (int i = 0; i < files.Count();i++ )
            {
                byte[] fileContent = files[i];
                PdfDocument pdfDoc = new PdfDocument(fileContent);
                mergeDoc.Append(pdfDoc);
               // mergeDoc.Append(pdfDoc, 1 , 1);
            }
            mergeDoc.InitialPageZoom = PageZoom.FitWidth;
            mergeDoc.Security = security;
            mergeDoc.Draw(filePath);
        }

        public static void EncryPDF(byte[] fileContent, string filePath, string encryKey)
        {
            //byte[] bytes3 = fileContent;
            //FileStream fs3 = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            //fs3.Write(bytes3, 0, bytes3.Length);
            //fs3.Close();
            //fs3.Dispose();   
            //PdfDocument pdfDoc = new PdfDocument(filePath);

            Aes128Security security = new Aes128Security(encryKey, encryKey);
            security.DocumentComponents = EncryptDocumentComponents.All;
            MergeDocument mergeDoc = new MergeDocument();
            mergeDoc.Security = security;
            PdfDocument pdfDoc = new PdfDocument(fileContent);
            mergeDoc.Append(pdfDoc);
            mergeDoc.InitialPageZoom = PageZoom.FitWidth;
            mergeDoc.Draw(filePath);
        }

        public static byte[] EncryPDF(byte[] fileContent, string encryKey)
        {
            Aes128Security security = new Aes128Security(encryKey, encryKey);
            security.DocumentComponents = EncryptDocumentComponents.All;
            MergeDocument mergeDoc = new MergeDocument();
            mergeDoc.Security = security;
            PdfDocument pdfDoc = new PdfDocument(fileContent);
            mergeDoc.Append(pdfDoc);
            mergeDoc.InitialPageZoom = PageZoom.FitWidth;
            byte[] encryPDFContent = mergeDoc.Draw();
            return encryPDFContent;
        }
    }
}
