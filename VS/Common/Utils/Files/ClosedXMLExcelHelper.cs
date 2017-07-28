using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace Common.Utils.Files
{
    public class ClosedXMLExcelHelper
    {
        /// <summary>
        /// Convert a excel file which has the table to contain data into a data table.
        /// </summary>
        /// <param name="excelFilesStream"></param>
        /// <returns></returns>
        public static DataTable GetDataTableFrom(Stream excelFilesStream)
        {
            DataTable dt = new DataTable();
            var wb = new XLWorkbook(excelFilesStream);

            // Contract the header
            foreach (var sheet in wb.Worksheets)
            {
                // Get columns from the sheet.
                // Ensure the columns is order by its name.
                var columns = sheet.Columns().OrderBy(s => s.ToString());

                foreach (var xlColumn in columns)
                {
                    DataColumn column = new DataColumn(xlColumn.Cell(1).Value.ToString());
                    if (!String.IsNullOrWhiteSpace(column.ColumnName))
                    {
                        dt.Columns.Add(column);
                    }
                }
            }

            // Fill the body
            foreach (var sheet in wb.Worksheets)
            {
                int j = 1;
                int a = dt.Columns.Count;

                foreach (var row in sheet.Rows().OrderBy(o => o.RowNumber()).ToList())
                {
                    // Skip the header
                    if (row.RowNumber() == 1)
                        continue;

                    // Skip the empty row
                    if (row != null && row.Cells().Where(c => !string.IsNullOrWhiteSpace(c.Value.ToString())).Any())
                    {
                        DataRow dataRow = dt.NewRow();

                        for (j = 1; j <= a; j++)
                        {
                            dataRow[j - 1] = row.Cell(j).Value.ToString();
                        }

                        dt.Rows.Add(dataRow);
                    }
                }
            }

            return dt;
        }

        //public static byte[] GetBytesFromDataTable(DataTable dt)
        //{
        //    byte[] byteArray = null;

        //    var wb = new XLWorkbook();


        //    var ws = wb.Worksheets.Add(dt);

        //    // Adjust column widths to their content
        //    ws.Columns().AdjustToContents();

        //    MemoryStream ms = new MemoryStream();
        //    wb.SaveAs(ms);

        //    ms.Read(byteArray, 0, (int)ms.Length);

        //    return byteArray;
        //}

        public static MemoryStream GetStreamFrom(DataTable dt)
        {
            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add(dt, "name");

            // Shutdown the auto filter
            //foreach (var table in ws.Tables)
            //{
            //    table.ShowAutoFilter = false;
            //}

            // Adjust column widths to their content
            ws.Columns().AdjustToContents();

            ws.FirstRow().Style.Font.Bold = true;

            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);

            return ms;
        }

        public static MemoryStream GetStreamFrom(DataSet ds)
        {
            var wb = new XLWorkbook();

            wb.Worksheets.Add(ds);

            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);

            return ms;
        }

        public static void SaveAs(Stream excelFilesStream, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            var wb = new XLWorkbook(excelFilesStream);
            wb.SaveAs(path);
        }

        /// <summary>
        /// Creates the folder if needed.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    /*TODO: You must process this exception.*/
                    result = false;
                }
            }
            return result;
        }
        public void SaveToExcel(DataSet ds , string file)
        {
            var workbook = new XLWorkbook();
            workbook.Worksheets.Add(ds);
            workbook.SaveAs(file);
        }
    }
}
