using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
//DocumentFormat.OpenXml.dll
//EPPlus.dll
//LumenWorks.Framework.IO.dll
using OfficeOpenXml;
using System.Data;
using System.Data.OleDb;

namespace Common.Utils
{
    public class ExcelHelper
    {
        public ArrayList ConvertExcelToArrayList(string colName, string sheetName, string UploadPath, bool skipLastRecord)
        {
            colName = colName.Trim();
            sheetName = sheetName.Trim();
            ArrayList importData_ArL = new ArrayList();
            FileStream stream = File.Open(UploadPath, FileMode.Open, FileAccess.Read);

            // ????
            //Excel.IExcelDataReader excelReader = UploadPath.EndsWith(".xlsx") ? Excel.ExcelReaderFactory.CreateOpenXmlReader(stream) : Excel.ExcelReaderFactory.CreateBinaryReader(stream);

            //excelReader.IsFirstRowAsColumnNames = true;  
            //xlsx and xls => dataset
            DataSet result = null; //excelReader.AsDataSet();

            if (result.Tables.Count > 0)
            {
                if (result.Tables.Contains(sheetName))
                {
                    int headerRow = 0;

                    for (int i = 0; i < result.Tables[sheetName].Rows.Count; i++)
                    {
                        if (result.Tables[sheetName].Rows[i][0].ToString().Length > 0)
                        {
                            headerRow = i;
                            break;
                        }
                    }
                    for (int i = 0; i < result.Tables[sheetName].Columns.Count; i++)
                    {
                        if (result.Tables[sheetName].Rows[headerRow][i].ToString().Trim() != "")
                            result.Tables[sheetName].Columns[i].ColumnName = result.Tables[sheetName].Rows[headerRow][i].ToString().Trim();
                    }

                    if (result.Tables[sheetName].Columns.Contains(colName))
                    {
                        for (int i = headerRow + 1; i < result.Tables[sheetName].Rows.Count - (skipLastRecord ? 1 : 0); i++)
                        {
                            importData_ArL.Add(result.Tables[sheetName].Rows[i][colName].ToString());
                        }
                    }
                    else
                    {
                        for (int i = headerRow + 1; i < result.Tables[sheetName].Rows.Count - (skipLastRecord ? 1 : 0); i++)
                        {
                            importData_ArL.Add("");
                        }
                    }
                }
            }

            //excelReader.Close();
            return importData_ArL;
         }

        public ArrayList GetImportDataList_XLSX(string colName, string sheetName, string UploadPath)
        {
            colName = colName.Trim();
            sheetName = sheetName.Trim();
            ArrayList importData_ArL = new ArrayList();
            var existingFile = new FileInfo(UploadPath);
            // Open and read the XlSX file.

            using (var package = new OfficeOpenXml.ExcelPackage(existingFile))
            {
                // Get the work book in the file
                OfficeOpenXml.ExcelWorkbook workBook = package.Workbook;
                if (workBook != null)
                {
                    if (workBook.Worksheets.Count > 0)
                    {
                        // Get the first worksheet
                        OfficeOpenXml.ExcelWorksheet currentWorksheet = workBook.Worksheets[sheetName];

                        // read some data
                        int columnIndex = -1;
                        int j = 1;
                        while (currentWorksheet.Cells[2, j].Value != null)
                        {
                            if (currentWorksheet.Cells[2, j].Value.ToString().ToLower() == colName.ToLower())
                            {
                                columnIndex = j;
                                break;
                            }
                            j++;
                        }

                        //for (int i = headerRow + 1; i < result.Tables[sheetName].Rows.Count - (skipLastRecord ? 1 : 0); i++)
                        //{
                        //    importData_ArL.Add(result.Tables[sheetName].Rows[i][colName].ToString().Replace(" 12:00:00 AM", ""));
                        //}   
                        if (columnIndex >= 1)
                        {
                            int k = 3;
                            while (!currentWorksheet.Cells[k, columnIndex].Merge)
                            {
                                importData_ArL.Add(currentWorksheet.Cells[k, columnIndex].Value == null ? "" : currentWorksheet.Cells[k, columnIndex].Value.ToString());
                                k++;
                            }
                        }
                    }
                }
            }

            //excelReader.IsFirstRowAsColumnNames = true;  

            return importData_ArL;
        }

        // xlsx 跟 xls對日期列處理的不同
        private DateTime convertDate(string dateStr)
        {
            double dt;
            DateTime dt2;
            DateTime date;

            if (dateStr == "") date = DateTime.Now;
            else if (double.TryParse(dateStr, out dt)) date = DateTime.FromOADate(double.Parse(dateStr));
            else if (DateTime.TryParse(dateStr, out dt2)) date = DateTime.Parse(dateStr);
            else date = DateTime.Now;
            return date;
        }

        //根據模板生成新的文件
        public void convertDate(string tfpath, string outputFile)
        {
            //Excel File
            ExcelPackage excel = null;
            try
            {
                FileInfo templateFile = new FileInfo(tfpath);
                FileInfo file = new FileInfo(outputFile);
                excel = new ExcelPackage(file, templateFile);
                ExcelWorksheet ws = excel.Workbook.Worksheets[1];
                ws.Cells[1, 1].Value = "Test";
                excel.Save();
                
            }
            catch (Exception ex)
            {
                // ex.Message;
            }
            finally
            {
                excel.Dispose();
            }
        }

        /// <summary>
        /// Excel检查版本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string ConnectionString(string fileName)
        {
            bool isExcel2003 = fileName.EndsWith(".xls");
            string connectionString = string.Format(
                isExcel2003
                    ? "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;"
                    : "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\"",
                fileName);
            return connectionString;
        }
        /// <summary>
        /// Excel导入数据源
        /// </summary>
        /// <param name="sheet">sheet</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string sheet, string filename)
        {
            OleDbConnection myConn = new OleDbConnection(ConnectionString(filename));
            try
            {
                DataSet ds;
                string strCom = " SELECT * FROM [Sheet1$]";
                myConn.Open();
                OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
                ds = new DataSet();
                myCommand.Fill(ds);
                myConn.Close();
                return ds.Tables[0];
            }
            catch (Exception)
            {
                myConn.Close();
                myConn.Dispose();
                throw;
            }
        }
    }
}
