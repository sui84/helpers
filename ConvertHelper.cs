using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace ConvertPcap
{
    public class ConvertHelper
    {
            private const string TDS = "TDS";
            private const string HTTP = "HTTP";
            private const string REQUEST = "Request";
            private const string RESPONSE = "Response";
            public string[] GetIpPackageHead()
            {
                return new string[] { "FilePath","LineNumber", "Encapsulation type", "Arrival Time","Time shift for this packet", "Epoch Time","Time delta from previous captured frame","Time delta from previous displayed frame","Time since reference or first frame"
                            , "Frame Number", "Frame Length", "Capture Length", "Frame is marked","Frame is ignored","Protocols in frame"
                            ,"Ethernet","Type","Sender MAC address","Sender IP address","Target MAC address","Target IP address","MS Network Load Balancing"
                            , "Version", "Header Length", "Differentiated Services Field", "Differentiated Services Codepoint", "Explicit Congestion Notification", "Total Length", "Identification"
                            , "Flags" , "Reserved bit", "Don't fragment", "More fragments" , "Fragment offset", "Time to live", "Protocol", "Header checksum" ,"Header checksum status", "Source"
                          , "Source Host", "Destination" , "Destination Host", "Source GeoIP", "Destination GeoIP" 
                           , "Source Port", "Destination Port", "Stream index" , "TCP Segment Len", "Sequence number", "Next sequence number","Acknowledgment number","Header Length2"
                            , "Flags2", "Reserved", "Nonce", "Congestion Window Reduced (CWR)" , "ECN-Echo", "Urgent", "Acknowledgment" , "Push", "Reset", "Syn", "Fin", "TCP Flags"
                           , "Window size value", "Checksum","Urgent pointer","Options"
                           ,"SEQ/ACK analysis","ACKFrame","Timestamps","Tabular Data Stream","Hypertext Transfer Protocol","BinaryData","BinaryDataStr"
                };
            }
            public string[] GetOptionsFlag()
            {
                return new string[] { "FilePath", "LineNumber", "Ethernet", "Type", "Sender MAC address", "Sender IP address", "Target MAC address", "Target IP address", "MS Network Load Balancing","Source Host",  "Destination Host" };
            }
            public string[] GetOtherPackageHead()
            {
                return new string[] { "FilePath", "FrameNumber", "Type", "ColumnsOrMethod", "Url", "Header", "RowCntOrRequest", "Direction", "Cookie", "Data" };
            }
              
        // 将DataTable中数据写入到CSV文件中
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
                        //str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                        //if (str.Contains(',') || str.Contains('"')
                        //    || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                        //{
                        //    str = string.Format("\"{0}\"", str);
                        //}

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
            /// 将TXT文件的数据读取到DataTable中
            public  DataSet OpenTXT(string filePath, char sepChar,int hIndex=0)
            {
                DataSet ds = new DataSet("ds");
                // Encoding encoding = Common.GetType(filePath); //Encoding.ASCII;//
                DataTable dt = new DataTable();
                dt.TableName = "ippkg";
                DataTable tdsdt = new DataTable();
                tdsdt.TableName = "tdspkg";
                FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                StreamReader sr = new StreamReader(fs, Encoding.ASCII);// encoding);
                //string fileContent = sr.ReadToEnd();
                //encoding = sr.CurrentEncoding;
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine = null;
                string[] tableHead = GetIpPackageHead();
                AddColumns(dt, tableHead);
                dt.Columns["FilePath"].DefaultValue = filePath;
                string[] options = GetOptionsFlag();

                //标示列数
                int columnCount = tableHead.Length;
                //逐行读取CSV中的数据
                int skipIndex = 0;
                int colIndex = 0;
                bool endData = false;
                DataRow dr = dt.NewRow();
                string endDataStr = string.Empty;
                int lineNum = 0;
                while ((strLine = sr.ReadLine()) != null)
                {
                    
                    try{
                        //strLine = Common.ConvertStringUTF8(strLine, encoding);
                        //strLine = Common.ConvertStringUTF8(strLine);
                        lineNum += 1;
                        if (skipIndex < hIndex)
                        {
                            skipIndex += 1;
                            continue;
                        }
                        if (strLine.StartsWith("Frame") && colIndex > 0)
                        {
                            dr = AddRecord(filePath,dr, endDataStr, dt, tdsdt);
                            colIndex = 0;
                            endData = false;
                            endDataStr = string.Empty;
                        }
                        if (strLine.StartsWith("Frame")) dr["LineNumber"] = lineNum;
                        if (endData)
                        {
                            endDataStr += string.Format("{0}{1}", strLine, "\r\n");
                        }
                        else
                        {
                            string line = RemoveChar(strLine);

                            while(colIndex < tableHead.Count()) // (int i = colIndex; i < tableHead.Count(); i++)
                            {
                                string colName = tableHead[colIndex];
                                int strPos = line.IndexOf(string.Format(colName.TrimEnd('2'), sepChar));
                                if (strPos > -1)
                                {
                                    if (tableHead[colIndex] == "Ethernet")
                                    {
                                        dr[colIndex] = line;
                                    }
                                    else if (tableHead[colIndex] == "MS Network Load Balancing")
                                    {
                                        endData = true;
                                    }
                                    else if (tableHead[colIndex] == "Urgent pointer")
                                    {
                                        dr[colIndex] = line.Substring(line.IndexOf(sepChar) + 1).Trim();
                                        endData = true;
                                    }
                                    else
                                    {
                                        dr[colIndex] = line.Substring(line.IndexOf(sepChar) + 1).Trim();
                                        colIndex += 1;
                                    }
                                    break;
                                }
                                else if (options.Contains(colName))
                                {
                                    colIndex += 1;
                                }
                                else
                                    break;
                            }

                            
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfo(string.Format("line:{0}\r\n{1}\r\n{2}", lineNum,ex.Message, ex.StackTrace));
                    }
                }
                if (colIndex > 0)
                {
                    AddRecord(filePath,dr, endDataStr, dt, tdsdt);
                    colIndex = 0;
                    endData = false;
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }

                sr.Close();
                fs.Close();
                ds.Tables.Add(dt);
                ds.Tables.Add(tdsdt);
                return ds;
            }

            private DataRow AddRecord(string filePath,DataRow dr, string endDataStr,DataTable dt, DataTable tdsdt)
            {
                if (!string.IsNullOrWhiteSpace(endDataStr))
                {
                    string[] keyStrs = new string[] { "Options:", "[SEQ/ACK analysis]\r\n", "[Timestamps]\r\n", "Tabular Data Stream\r\n", "Hypertext Transfer Protocol\r\n", "MS Network Load Balancing\r\n", "\r\n\r\n" };
                    string[] valueStrs = SplitStr(endDataStr, keyStrs);
                    dr["Options"] = valueStrs[0];
                    dr["SEQ/ACK analysis"] = valueStrs[1];
                    string str = "This is an ACK to the segment in frame:";
                    int pos = dr["SEQ/ACK analysis"].ToString().IndexOf(str);
                    if (pos > -1)
                    {
                        int pos2 = dr["SEQ/ACK analysis"].ToString().IndexOf("]\r\n", pos);
                        dr["ACKFrame"] = dr["SEQ/ACK analysis"].ToString().Substring(pos + str.Length, pos2 - pos - str.Length);
                    }
                    else dr["ACKFrame"] = 0;
                    dr["Timestamps"] = valueStrs[2];
                    dr["Tabular Data Stream"] = valueStrs[3];
                    if (!string.IsNullOrEmpty(valueStrs[3]))
                    {
                        tdsdt.Merge(ConvertOtherPkg(filePath,Convert.ToInt32(dr["Frame Number"]), valueStrs[3], TDS));
                    }
                    dr["Hypertext Transfer Protocol"] = valueStrs[4];
                    if (!string.IsNullOrEmpty(valueStrs[4]))
                    {
                        tdsdt.Merge(ConvertOtherPkg(filePath,Convert.ToInt32(dr["Frame Number"]), valueStrs[4], HTTP));
                    }
                    dr["MS Network Load Balancing"] = valueStrs[6];
                    dr["BinaryData"] = valueStrs[6];
                    string[] binStrs = valueStrs[6].Split(new string[]{"\r\n"},StringSplitOptions.None );
                    string binaryDataStr = string.Empty;
                    foreach (string binStr in binStrs)
                    {
                        if(!string.IsNullOrWhiteSpace(binStr)){
                            // 拼接字符串
                            binaryDataStr += binStr.Substring(56,binStr.Length -56);
                        }
                    }
                    dr["BinaryDataStr"] = binaryDataStr.Replace(".","");
                }
                dt.Rows.Add(dr);
                dr = dt.NewRow();
                return dr;
            }

            private string[] SplitStr(string dataStr,string[] keyStrs)
            {
                string[] strs = new string[keyStrs.Length];
                int[] idxs = new int[keyStrs.Length];
                for(int i=0;i<keyStrs.Length;i++)
                {
                    idxs[i] = dataStr.IndexOf(keyStrs[i]);

                }
                for (int i = 0; i < idxs.Length; i++)
                {
                    if (idxs[i] > -1)
                    {
                        
                        if (i == idxs.Length - 1) strs[i] = dataStr.Substring(idxs[i]);
                        else
                        {
                            int nextIdx = 0;
                            for (int j = i + 1; j < idxs.Length; j++)
                            {
                                if (idxs[j] > -1)
                                {
                                    nextIdx = idxs[j];
                                    break;
                                }
                            }
                            strs[i] = dataStr.Substring(idxs[i], nextIdx - idxs[i]);
                        }
                    }
                    else
                    {
                        strs[i] = string.Empty;
                    }
                }
                return strs;
            }

            private string RemoveChar(string str)
            {
                string s = str.Trim();
                if(s.StartsWith("[") && s.EndsWith("]")) s = s.TrimStart('[').TrimEnd(']');
                int pos = s.IndexOf("=");
                if(pos > -1) s = s.Substring(pos + 1).Trim();
                return s;
            }
            private DataTable ConvertOtherPkg(string filePath,int frameNum,string pkgStr,string type)
            {
                DataTable dt = new DataTable();
                DataRow dr = dt.NewRow();
                string[] tableHead = GetOtherPackageHead();
                string[] strs = pkgStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                AddColumns(dt, tableHead);
                dr["FrameNumber"] = frameNum;
                dr["Type"] = type;
                dt.Columns["FilePath"].DefaultValue = filePath;
                string dataStr = string.Empty;
                string headerStr = string.Empty;
                List<string> header = new List<string>();
                bool newRow = false;
                foreach(string s in strs){
                    if (type == TDS)
                    {
                        SetValue(dr,"ColumnsOrMethod", s, "Columns");
                        if (s.IndexOf("Column Name:") > -1)
                        {
                            string columnNames = GetValue(s);
                            dataStr += string.Format("{0}{1}", (string.IsNullOrEmpty(dataStr) ? "" : ","), columnNames);
                        }
                        SetValue(dr,"RowCntOrRequest", s, "Row count");
                        if (s.IndexOf("Token - Row") > -1 || s.IndexOf("Token - NBCRow") > -1)
                        {
                            dataStr += "\r\n";
                            newRow = true;
                        }
                        if (s.IndexOf("Data:") > -1)
                        {
                            string data = GetValue(s);
                            if (newRow)
                            {
                                dataStr += data;
                                newRow = false;
                            }
                            else
                            {
                                dataStr += string.Format("{0}{1}", ",", data);

                            }
                        }
                    }
                    else if (type == HTTP)
                    {
                        SetValue(dr,"ColumnsOrMethod", s, "Request Method");
                        SetValue(dr,"Url", s, "Request URI");
                        if (s.IndexOf("Request:") > -1)
                        {
                            dr["Direction"] = REQUEST;
                        }
                        else if (s.IndexOf("Response:") > -1)
                        {
                            dr["Direction"] = RESPONSE;
                        }
                        else if (s.IndexOf("Request in frame:") > -1)
                        {
                            dr["RowCntOrRequest"] = GetValue(s); ;
                        }
                        else if (s.IndexOf("Cookie:") > -1)
                        {
                            dr["Cookie"] = GetValue(s); ;
                        }
                        if (s.IndexOf("Content-Type:") > -1 || s.IndexOf("Content-Encoding") > -1 || s.IndexOf("Vary") > -1 || s.IndexOf("Host") > -1 || s.IndexOf("Server") > -1 || s.IndexOf("X-Powered-By") > -1
                            || s.IndexOf("Date") > -1 || s.IndexOf("Content-Length") > -1 || s.IndexOf("User-Agent") > -1 || s.IndexOf("SOAPAction") > -1 || s.IndexOf("Accept-Encoding") > -1
                            || s.IndexOf("File Data") > -1 || s.IndexOf("Referer") > -1 || s.IndexOf("Accept-Language") > -1)
                        {
                            if (!header.Contains(s)) header.Add(s);
                        }
                        if (newRow) dataStr += s;
                        if (s.IndexOf("eXtensible Markup Language") > -1 || s.IndexOf("Secure Sockets Layer") > -1)
                        {
                            newRow = true;
                        } 
                    }
                    
                }
                // dr["Data"] = string.Format("\"{0}\"", csvdata);
                dr["Header"] = string.Join ("", header);
                dr["Data"] = dataStr;
                dr["FilePath"] = filePath;
                dt.Rows.Add(dr);
                return dt;
            }


            private void SetValue(DataRow dr,string colName,string str, string col)
            {
                if (str.IndexOf(col+":") > -1)
                {
                    dr[colName] = GetValue(str);
                }
            }

            private string GetValue(string str)
            {
                str = str.Trim();
                if (str.StartsWith("[") && str.EndsWith("]")) str = str.TrimStart('[').TrimEnd(']');
                int pos = str.IndexOf(':');
                if (pos > -1)
                {
                    str = str.Substring(pos + 1);
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }
                }
                str = str.Trim();
                return str;
             }

            private DataTable AddColumns(DataTable dt, string[] tableHead)
            {
                //创建列
                for (int i = 0; i < tableHead.Length; i++)
                {
                    DataColumn dc = new DataColumn(tableHead[i].Trim());
                    dt.Columns.Add(dc);
                }
                return dt;

            }


            public  void LogInfo(string msg)
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
                File.AppendAllText(fileName, String.Format("{0}:{1}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"), msg), Encoding.UTF8);
            }



        }
    }

