using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ExcelLibrary.SpreadSheet;
using Common.Utils.Log;

namespace Common.Utils.Files
{
    public class ExcelLibraryHelper
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
                            try
                            {
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
        public void SaveExcelData(DataTable dt, string file)
        {
            Workbook wb = new Workbook();
            Worksheet ws = new Worksheet("Sheet1");
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ws.Cells[0, i] = new Cell(dt.Columns[i].ColumnName.ToString());
            }
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ws.Cells[i, j] = new Cell(dt.Rows[i - 1][j].ToString());
                }
            }
            wb.Worksheets.Add(ws);
            wb.Save(file);

        }

    }
}
