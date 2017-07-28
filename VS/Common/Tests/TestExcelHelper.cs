using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common.Utils;
using System.Data;
using ClosedXML.Excel;
using ExcelLibrary.SpreadSheet;
using Common.Utils.Files;
using Common.Utils.DB;

namespace Common.Tests
{
    public class TestExcelHelper
    {
        static void Main(string[] args)
        {
            ConnStrHelper ch = new ConnStrHelper();
            DbHelper dbh = new DbHelper();
            string connStr = ch.GetMSSQLOledbConnStr("localhost", "TEST");
            string sql = "select * from TM";
            DataSet ds = dbh.GetDataFromCommand(sql,connStr);

            string xlsfile = @"D:\TEMP\HelloWorld.XLS";
            NPOIExcelHelper ex = new NPOIExcelHelper();
            ex.ToExcel(ds.Tables[0], xlsfile);
            SaveXLSFile(ds, xlsfile);

            string xlsxfile = @"D:\TEMP\HelloWorld.XLSX";
            SaveXLSXFile(ds, xlsxfile);

            DirectoryInfo dir = new DirectoryInfo(@"D:\TEMP");
            FileInfo[] files = dir.GetFiles();
            FileHelper.QuickSort(files, 0, files.Length - 1);//按时间排序
        }

        //excel 2010
        public static void SaveXLSXFile(DataSet ds, string file)
        {
            var workbook = new XLWorkbook();
            workbook.Worksheets.Add(ds);
            workbook.SaveAs(file);
        }

        //excel 2007
        //Prompt error: the format not match
        public static void SaveXLSFile(DataSet ds, string file)
        {
            ExcelLibraryHelper fh = new ExcelLibraryHelper();
            fh.SaveExcelData(ds.Tables[0], file);
        }
    }
}
