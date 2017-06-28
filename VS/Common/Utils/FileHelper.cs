using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using ExcelLibrary.SpreadSheet;

namespace Common.Utils
{
    public class FileHelper
    {
        private const int MaxStringLen = 500;

        //读取Excel数据到DataSet
        public DataSet GetExcelData(string file, int hIndex)
        {
            DataSet ds = new DataSet("ds");
            //FileStream fileStream = new FileStream(file, FileMode.Open);
            //Workbook workbook = Workbook.Load(fileStream);
            Workbook workbook = Workbook.Load(file);



            //也可以直接传个文件名，但会报出 Stream was not writable.异常
           // Worksheet worksheet = workbook.Worksheets[0];
            foreach (Worksheet worksheet in workbook.Worksheets)
            {
                DataTable dt = new DataTable(worksheet.Name);
                if (worksheet.Cells.LastRowIndex > 1)
                {
                    if (hIndex >= 0)
                    {
                        for (int j = 0; j <= worksheet.Cells.LastColIndex; j++)//设置DataTable列名
                        {
                            try{
                                if (worksheet.Cells[hIndex, j].Value == null) dt.Columns.Add(string.Empty, typeof(string));
                                else dt.Columns.Add(worksheet.Cells[hIndex, j].Value.ToString().Trim(), typeof(string));
                            }
                            catch (Exception ex)
                            {
                                LogHelper logHelper = new LogHelper();
                                logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), string.Empty);
                            }
                        }
                    }
                    for (int i = hIndex + 1; i <= worksheet.Cells.LastRowIndex; i++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j <= worksheet.Cells.LastColIndex; j++)
                        {
                            try
                            {
                                if (worksheet.Cells[i, j].Value == null) continue;
                                string str = worksheet.Cells[i, j].Value.ToString().Trim();
                                if (str.Length > MaxStringLen) dr[j] = str.Substring(0, MaxStringLen);
                                else dr[j] = str;
                            }
                            catch (Exception ex)
                            {
                                LogHelper logHelper = new LogHelper();
                                logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), string.Empty);
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                    ds.Tables.Add(dt);
                }
            }
            //fileStream.Close();
            return ds;
        }

        //读取DataTable数据到ExcelFile
        public void SaveExcelData(DataTable  dt,string file)
        {
            Workbook wb = new Workbook();
            Worksheet ws = new Worksheet("Sheet1");
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ws.Cells[0, i]  = new Cell( dt.Columns[i].ColumnName.ToString());
            }
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ws.Cells[i, j] = new Cell(dt.Rows[i-1][j].ToString());
                }
            }
            wb.Worksheets.Add(ws);
            wb.Save(file);

        }

        //读取File数据到DataTable
        #region
        public DataTable GetDataTable(string file, string[] columns, char delimited)
        {
           string[] lines = System.IO.File.ReadAllLines(file);
           DataTable dt = ConvertToDataTable(lines, columns, delimited);
           return dt;
       }

        public DataTable ConvertToDataTable(string[] lines, string[] columns, char delimited)
        {
            DataTable dt = new DataTable();
            foreach (string col in columns)
            {
                dt.Columns.Add(col, typeof(System.String));
            }
            foreach (string line in lines)
            {
                DataRow dr = dt.NewRow();
                string[] data = line.Split(delimited);
                for (int i = 0; i <= columns.Length - 1; i++)
                {
                    try
                    {
                        dr[i] = data[i];
                    }
                    catch (Exception ex)
                    {
                        LogHelper logHelper = new LogHelper();
                        logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), string.Empty);
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion

        //String => File
        public void SaveStringDatae(string path, string content)
        {
            FileStream fs = null;
            //  fs = File.Create(path);
            //   fs.Write(System.Text.Encoding.Default.GetBytes(content), 0, content.Length);
            // fs.Write(System.Text.Encoding.UTF8.GetBytes(content), 0, content.Length);
            //  fs.Close();
            fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = null;
            if (System.Text.Encoding.Default == System.Text.Encoding.GetEncoding("GB2312"))
                sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("GB2312"));
            else
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.Write(content);
            sw.Close();
        }

        // File => IE open
        //byte[] result = File.ReadAllBytes(path);
        //Response.ContentType = "text/CSV";
        //        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", summaryType + ".CSV"));
        //        if (System.Text.Encoding.Default == System.Text.Encoding.GetEncoding("GB2312"))
        //            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        //        else
        //            Response.ContentEncoding = System.Text.Encoding.UTF8;

        //        // Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
        //        Response.BinaryWrite(result);
                
        //        if (File.Exists(path) == true)
        //        {
        //            File.Delete(path);   //Generate the excel
        //        }
        //        Response.End();

        // File => Stream / MemoryStream
        public MemoryStream GetFileStream(string path, string content)
        {
            MemoryStream stream2 = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                
                 byte[] buffer = new byte[0x1000];
                int count = 0;
                while ((count = stream.Read(buffer, 0, 0x1000)) > 0)
                {
                    stream2.Write(buffer, 0, count);
                }

                    
            }
            if ((stream2 != null) && (stream2.Length > 0L))
            {
                return stream2;
            }
            else return null;
           
        }
    }
}
