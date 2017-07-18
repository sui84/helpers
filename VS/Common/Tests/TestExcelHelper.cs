using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils.Excel;
using System.IO;
using Common.Utils;
using System.Data;
using ClosedXML.Excel;
using ExcelLibrary.SpreadSheet;

namespace Common.Tests
{
    public class TestExcelHelper
    {
        static void Main(string[] args)
        {
            ConnStrHelper ch = new ConnStrHelper();
            DbHelper dbh = new DbHelper();
            string connStr = ch.GetMSSQLOledbConnStr("localhost", "TEST");
            string sql = "select * from TESTTB";
            DataSet ds = dbh.GetDataFromCommand(sql,connStr);
            string xlsxfile = @"D:\TEMP\HelloWorld.XLSX";
            string xlsfile = @"D:\TEMP\HelloWorld.XLS";
            SaveXLSXFile(ds, xlsxfile);
            SaveXLSFile(ds, xlsfile);
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
            FileHelper fh = new FileHelper();
            fh.SaveExcelData(ds.Tables[0], file);
        }
    }
}
