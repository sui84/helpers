using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;


namespace Common.Utils
{
    public class CSVHelper
    {
          private const int MaxStringLen = 500;
            /// <summary>
            /// 将DataTable中数据写入到CSV文件中
            /// </summary>
            /// <param name="dt">提供保存数据的DataTable</param>
            /// <param name="fileName">CSV的文件路径</param>
            public  void SaveCSV(DataTable dt, string fullPath)
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                string data = "";
                //写出列名称
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = dt.Rows[i][j].ToString();
                        str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                        if (str.Contains(',') || str.Contains('"')
                            || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                        {
                            str = string.Format("\"{0}\"", str);
                        }

                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
            }

            /// <summary>
            /// 将CSV文件的数据读取到DataTable中
            /// </summary>
            /// <param name="fileName">CSV文件路径</param>
            /// <returns>返回读取了CSV数据的DataTable</returns>
            public  DataTable OpenCSV(string filePath, char sepChar,int hIndex)
            {
                // Encoding encoding = Common.GetType(filePath); //Encoding.ASCII;//
                DataTable dt = new DataTable();
                
             //   using(FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)){
                

                //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    using (StreamReader sr = new StreamReader(filePath, Encoding.ASCII))
                    {// encoding);
                        //string fileContent = sr.ReadToEnd();
                        //encoding = sr.CurrentEncoding;
                        //记录每次读取的一行记录
                        string strLine = "";
                        //记录每行记录中的各字段内容
                        string[] aryLine = null;
                        string[] tableHead = null;
                        //标示列数
                        int columnCount = 0;
                        //标示是否是读取的第一行
                        bool IsFirst = true;
                        //逐行读取CSV中的数据
                        int skipIndex = 0;
                        while ((strLine = sr.ReadLine()) != null)
                        {
                            try
                            {
                                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                                //strLine = Common.ConvertStringUTF8(strLine);
                                if (skipIndex < hIndex)
                                {
                                    skipIndex += 1;
                                    continue;
                                }

                                if (IsFirst == true)
                                {
                                    tableHead = strLine.Split(sepChar);
                                    IsFirst = false;
                                    columnCount = tableHead.Length;
                                    //创建列
                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        string header = tableHead[i].Trim().TrimStart('"').TrimEnd('"'); ;
                                        DataColumn dc = new DataColumn(header);
                                        dt.Columns.Add(dc);
                                    }
                                }
                                else
                                {
                                    aryLine = strLine.Split(sepChar);
                                    if (aryLine.Count() < columnCount) continue;
                                    else
                                    {
                                        DataRow dr = dt.NewRow();
                                        for (int j = 0; j < columnCount; j++)
                                        {
                                            dr[j] = aryLine[j];
                                            if (dr[j].GetType() == Type.GetType("System.String"))
                                            {
                                                string str = aryLine[j].ToString().Trim().TrimStart('"').TrimEnd('"');
                                                if (str.Length > MaxStringLen) str = str.Substring(0, MaxStringLen);
                                                dr[j] = str;
                                            };

                                        }
                                        dt.Rows.Add(dr);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper logHelper = new LogHelper();
                                logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), string.Empty);
                            }
                        }
                        //if (aryLine != null && aryLine.Length > 0)
                        //{
                        //    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                        //}

                      
                        //sr.Close();
                        //fs.Close();
                    };
                // };

                return dt;
            }


            public DataSet GetCSVData(string file, char sepChar, int hIndex)
            {
                DataSet ds = new DataSet("ds");
                DataTable dt = OpenCSV(file,sepChar, hIndex);
                ds.Tables.Add(dt);
                return ds;
            }
        }
    }

