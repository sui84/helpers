using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using ExcelLibrary.SpreadSheet;
using System.Web;

namespace Common.Utils
{
    public class FileHelper
    {
        #region 以只读方式读取文本文件
        /// <summary>
        /// 以只读方式读取文本文件
        /// </summary>
        /// <param name="FilePath">文件路径及文件名</param>
        /// <returns></returns>
        public static string ReadTxtFile(string FilePath)
        {
            string content = "";//返回的字符串
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                {
                    string text = string.Empty;
                    while (!reader.EndOfStream)
                    {
                        text += reader.ReadLine() + "\r\n";
                        content = text;
                    }
                }
            }
            return content;
        }
        public static string ReadFile(string FilePath)
        {
            string text = string.Empty;
            System.Text.Encoding code = System.Text.Encoding.GetEncoding("gb2312");
            using (var sr = new StreamReader(FilePath, code))
            {
                try
                {
                    text = sr.ReadToEnd(); // 读取文件
                    sr.Close();
                }
                catch { }
            }
            return text;
        }
        #endregion

        /// <summary>
        /// 快速排序算法
        /// </summary>
        /// 快速排序为不稳定排序,时间复杂度O(nlog2n),为同数量级中最快的排序方法
        /// <param name="arr">划分的数组</param>
        /// <param name="low">数组低端上标</param>
        /// <param name="high">数组高端下标</param>
        /// <returns></returns>
        static int Partition(FileInfo[] arr, int low, int high)
        {
            //进行一趟快速排序,返回中心轴记录位置
            // arr[0] = arr[low];
            FileInfo pivot = arr[low];//把中心轴置于arr[0]
            while (low < high)
            {
                while (low < high && arr[high].CreationTime <= pivot.CreationTime)
                    --high;
                //将比中心轴记录小的移到低端
                Swap(ref arr[high], ref arr[low]);
                while (low < high && arr[low].CreationTime >= pivot.CreationTime)
                    ++low;
                Swap(ref arr[high], ref arr[low]);
                //将比中心轴记录大的移到高端
            }
            arr[low] = pivot; //中心轴移到正确位置
            return low;  //返回中心轴位置
        }
        static void Swap(ref FileInfo i, ref FileInfo j)
        {
            FileInfo t;
            t = i;
            i = j;
            j = t;
        }
        /// <summary>
        /// 快速排序算法
        /// </summary>
        /// 快速排序为不稳定排序,时间复杂度O(nlog2n),为同数量级中最快的排序方法
        /// <param name="arr">划分的数组</param>
        /// <param name="low">数组低端上标</param>
        /// <param name="high">数组高端下标</param>
        public static void QuickSort(FileInfo[] arr, int low, int high)
        {
            if (low <= high - 1)//当 arr[low,high]为空或只一个记录无需排序
            {
                int pivot = Partition(arr, low, high);
                QuickSort(arr, low, pivot - 1);
                QuickSort(arr, pivot + 1, high);
            }
        }

        // split and merge text files
        #region
        const int MAXLINELEN = 500;
        /// 分割大文件成独立小文件并保存小文件至指定目录
        /// </summary>
        /// <param name="splitunit">分割单位（KB，MB）</param>
        /// <param name="intFlag">分割大小</param>
        /// <param name="destCatalog">保存路径</param>
        /// <param name="sourcefileurl">源文件路径</param>
        /// <returns>true表示分割成功，false表示分割失败</returns>
        public bool SplitFileLines(string splitunit, int intFlag, string destCatalog, string sourcefileurl)
        {
            bool suc = false;
            try
            {
                int iFileSize = 0;
                switch (splitunit)
                {
                    case "Byte":
                        iFileSize = intFlag;
                        break;
                    case "KB":
                        iFileSize = 1024 * intFlag;
                        break;
                    case "MB":
                        iFileSize = 1024 * 1024 * intFlag;
                        break;
                    default:
                        iFileSize = 1024 * 1024 * 1024 * intFlag;
                        break;
                }

                using (FileStream fs = new FileStream(sourcefileurl, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader SplitFileReader = new BinaryReader(fs))
                    {
                        Byte[] TempBytes;
                        int ifilecount = (int)(fs.Length / iFileSize);
                        if (fs.Length % iFileSize != 0)
                        {
                            ifilecount++;
                        }
                        long lastfsize = 0;
                        for (int i = 1; i <= ifilecount; i++)
                        {
                            if (!Directory.Exists(destCatalog)) Directory.CreateDirectory(destCatalog);
                            string sTempFileName = destCatalog + "\\" + Path.GetFileNameWithoutExtension(sourcefileurl) + i.ToString().PadLeft(4, '0') + Path.GetExtension(sourcefileurl);
                            using (FileStream TempStream = new FileStream(sTempFileName, FileMode.OpenOrCreate))
                            {
                                using (BinaryWriter bw = new BinaryWriter(TempStream))
                                {
                                    int j = 1;
                                    if (i != ifilecount)
                                    {
                                        fs.Seek(lastfsize + iFileSize, SeekOrigin.Begin);//fs读取后的内容为：123456789'
                                        for (j = 1; j <= MAXLINELEN; j++)
                                        {
                                            byte[] data = new byte[1];
                                            fs.Read(data, 0, 1);
                                            string s = System.Text.Encoding.Default.GetString(data);
                                            //Console.Write(s + " ");
                                            if (s == "\r" || s == "\n")
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    fs.Seek(lastfsize, SeekOrigin.Begin);
                                    TempBytes = SplitFileReader.ReadBytes(iFileSize + j);
                                    lastfsize += iFileSize + j;
                                    bw.Write(TempBytes);
                                    bw.Close();
                                };
                                TempStream.Close();
                            };
                        }
                        fs.Close();
                        SplitFileReader.Close();
                    };
                };
                suc = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message + ex.StackTrace;
                suc = false;
            }
            return suc;
        }

        /// <summary>
        /// 分割大文件成独立小文件并保存小文件至指定目录
        /// </summary>
        /// <param name="splitunit">分割单位（KB，MB）</param>
        /// <param name="intFlag">分割大小</param>
        /// <param name="destCatalog">保存路径</param>
        /// <param name="sourcefileurl">源文件路径</param>
        /// <returns>true表示分割成功，false表示分割失败</returns>
        public bool SplitFileBySize(string splitunit, int intFlag, string destCatalog, string sourcefileurl)
        {
            bool suc = false;
            try
            {
                int iFileSize = 0;
                switch (splitunit)
                {
                    case "Byte":
                        iFileSize = intFlag;
                        break;
                    case "KB":
                        iFileSize = 1024 * intFlag;
                        break;
                    case "MB":
                        iFileSize = 1024 * 1024 * intFlag;
                        break;
                    default:
                        iFileSize = 1024 * 1024 * 1024 * intFlag;
                        break;
                }
                FileStream SplitFileStream = new FileStream(sourcefileurl, FileMode.Open);
                BinaryReader SplitFileReader = new BinaryReader(SplitFileStream);
                Byte[] TempBytes;
                int ifilecount = (int)(SplitFileStream.Length / iFileSize);
                if (SplitFileStream.Length % iFileSize != 0)
                {
                    ifilecount++;
                }
                for (int i = 1; i <= ifilecount; i++)
                {
                    string sTempFileName = destCatalog + "\\" + i.ToString().PadLeft(4, '0') + Path.GetExtension(sourcefileurl);
                    FileStream TempStream = new FileStream(sTempFileName, FileMode.OpenOrCreate);
                    BinaryWriter bw = new BinaryWriter(TempStream);
                    TempBytes = SplitFileReader.ReadBytes(iFileSize);
                    bw.Write(TempBytes);
                    bw.Close();
                    TempStream.Close();
                }
                SplitFileStream.Close();
                SplitFileReader.Close();
                suc = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message + ex.StackTrace;
                suc = false;
            }
            return suc;
        }

        public void MergeFile(string dir, string destFile)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                string[] lines = System.IO.File.ReadAllLines(file);
                File.AppendAllLines(destFile, lines, Encoding.UTF8);
                File.Delete(file);
            }
        }
        #endregion

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

        #region
        public static string GetFileSizeAsString(long size)
        {
            double s = size;
            string[] format = new string[] { "{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB", "{0} ZB", "{0} YB" };
            int i = 0;
            while (i < format.Length - 1 && s >= 1024)
            {
                s = (100 * s / 1024) / 100.0;
                i++;
            }
            return string.Format(format[i], s.ToString("###,###,##0.##"));
        }
        #endregion

        #region 检测指定目录是否存在
        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region 检测指定文件是否存在,如果存在返回true
        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region 获取指定目录中的文件列表
        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }
        #endregion

        #region 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.
        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取指定目录及子目录中所有文件列表
        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检测指定目录是否为空
        /// <summary>
        /// 检测指定目录是否为空
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断是否存在文件
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }

                //判断是否存在文件夹
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                //这里记录日志
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                return true;
            }
        }
        #endregion

        #region 检测指定目录中是否存在指定的文件
        /// <summary>
        /// 检测指定目录中是否存在指定的文件,若要搜索子目录请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>        
        public static bool Contains(string directoryPath, string searchPattern)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, false);

                //判断指定文件是否存在
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
            }
        }

        /// <summary>
        /// 检测指定目录中是否存在指定的文件
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param> 
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, true);

                //判断指定文件是否存在
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
            }
        }
        #endregion

        #region 创建目录
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir">要创建的目录路径包括目录名</param>
        public static void CreateDir(string dir)
        {
            if (dir.Length == 0) return;
            if (!Directory.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir))
                Directory.CreateDirectory(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir);
        }
        #endregion

        #region 删除目录
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir">要删除的目录路径和名称</param>
        public static void DeleteDir(string dir)
        {
            if (dir.Length == 0) return;
            if (Directory.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir))
                Directory.Delete(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir);
        }
        #endregion

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">要删除的文件路径和名称</param>
        public static void DeleteFile(string file)
        {
            if (File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file))
            {
                File.Delete(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file);
            }
        }
        #endregion

        #region 创建文件
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="dir">带后缀的文件名</param>
        /// <param name="pagestr">文件内容</param>
        public static void CreateFile(string dir, string pagestr)
        {
            dir = dir.Replace("/", "\\");
            if (dir.IndexOf("\\") > -1)
                CreateDir(dir.Substring(0, dir.LastIndexOf("\\")));
            StreamWriter sw = new StreamWriter(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir, false, System.Text.Encoding.GetEncoding("GB2312"));
            sw.Write(pagestr);
            sw.Close();
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void CreateFileContent(string path, string content)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.GetEncoding("GB2312"));
            sw.Write(content);
            sw.Close();
        }
        #endregion

        #region 移动文件(剪贴--粘贴)
        /// <summary>
        /// 移动文件(剪贴--粘贴)
        /// </summary>
        /// <param name="dir1">要移动的文件的路径及全名(包括后缀)</param>
        /// <param name="dir2">文件移动到新的位置,并指定新的文件名</param>
        public static void MoveFile(string dir1, string dir2)
        {
            dir1 = dir1.Replace("/", "\\");
            dir2 = dir2.Replace("/", "\\");
            if (File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1))
                File.Move(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1, System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir2);
        }
        #endregion

        #region 复制文件
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="dir1">要复制的文件的路径已经全名(包括后缀)</param>
        /// <param name="dir2">目标位置,并指定新的文件名</param>
        public static void CopyFile(string dir1, string dir2)
        {
            dir1 = dir1.Replace("/", "\\");
            dir2 = dir2.Replace("/", "\\");
            if (File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1))
            {
                File.Copy(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1, System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir2, true);
            }
        }
        #endregion

        #region 根据时间得到目录名 / 格式:yyyyMMdd 或者 HHmmssff
        /// <summary>
        /// 根据时间得到目录名yyyyMMdd
        /// </summary>
        /// <returns></returns>
        public static string GetDateDir()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }
        /// <summary>
        /// 根据时间得到文件名HHmmssff
        /// </summary>
        /// <returns></returns>
        public static string GetDateFile()
        {
            return DateTime.Now.ToString("HHmmssff");
        }
        #endregion

        #region 根据时间获取指定路径的 后缀名的 的所有文件
        /// <summary>
        /// 根据时间获取指定路径的 后缀名的 的所有文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="Extension">后缀名 比如.txt</param>
        /// <returns></returns>
        public static DataRow[] GetFilesByTime(string path, string Extension)
        {
            if (Directory.Exists(path))
            {
                string fielExts = string.Format("*{0}", Extension);
                string[] files = Directory.GetFiles(path, fielExts);
                if (files.Length > 0)
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(new DataColumn("filename", Type.GetType("System.String")));
                    table.Columns.Add(new DataColumn("createtime", Type.GetType("System.DateTime")));
                    for (int i = 0; i < files.Length; i++)
                    {
                        DataRow row = table.NewRow();
                        DateTime creationTime = File.GetCreationTime(files[i]);
                        string fileName = Path.GetFileName(files[i]);
                        row["filename"] = fileName;
                        row["createtime"] = creationTime;
                        table.Rows.Add(row);
                    }
                    return table.Select(string.Empty, "createtime desc");
                }
            }
            return new DataRow[0];
        }
        #endregion

        #region 复制文件夹
        /// <summary>
        /// 复制文件夹(递归)
        /// </summary>
        /// <param name="varFromDirectory">源文件夹路径</param>
        /// <param name="varToDirectory">目标文件夹路径</param>
        public static void CopyFolder(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);

            if (!Directory.Exists(varFromDirectory)) return;

            string[] directories = Directory.GetDirectories(varFromDirectory);

            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    CopyFolder(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }
            string[] files = Directory.GetFiles(varFromDirectory);
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), true);
                }
            }
        }
        #endregion

        #region 检查文件,如果文件不存在则创建
        /// <summary>
        /// 检查文件,如果文件不存在则创建  
        /// </summary>
        /// <param name="FilePath">路径,包括文件名</param>
        public static void ExistsFile(string FilePath)
        {
            //if(!File.Exists(FilePath))    
            //File.Create(FilePath);    
            //以上写法会报错,详细解释请看下文.........   
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
            }
        }
        #endregion

        #region 删除指定文件夹对应其他文件夹里的文件
        /// <summary>
        /// 删除指定文件夹对应其他文件夹里的文件
        /// </summary>
        /// <param name="varFromDirectory">指定文件夹路径</param>
        /// <param name="varToDirectory">对应其他文件夹路径</param>
        public static void DeleteFolderFiles(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);

            if (!Directory.Exists(varFromDirectory)) return;

            string[] directories = Directory.GetDirectories(varFromDirectory);

            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    DeleteFolderFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }


            string[] files = Directory.GetFiles(varFromDirectory);

            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Delete(varToDirectory + s.Substring(s.LastIndexOf("\\")));
                }
            }
        }
        #endregion

        #region 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileName(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }
        #endregion

        #region 复制文件参考方法,页面中引用
        /// <summary>
        /// 复制文件参考方法,页面中引用
        /// </summary>
        /// <param name="cDir">新路径</param>
        /// <param name="TempId">模板引擎替换编号</param>
        public static void CopyFiles(string cDir, string TempId)
        {
            //if (Directory.Exists(Request.PhysicalApplicationPath + "\\Controls"))
            //{
            //    string TempStr = string.Empty;
            //    StreamWriter sw;
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\Default.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\Default.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\Default.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\Column.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\Column.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\List.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\Content.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\Content.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\View.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\MoreDiss.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\MoreDiss.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\DissList.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\ShowDiss.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\ShowDiss.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\Diss.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\Review.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\Review.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\Review.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //    if (File.Exists(Request.PhysicalApplicationPath + "\\Controls\\Search.aspx"))
            //    {
            //        TempStr = File.ReadAllText(Request.PhysicalApplicationPath + "\\Controls\\Search.aspx");
            //        TempStr = TempStr.Replace("{$ChannelId$}", TempId);

            //        sw = new StreamWriter(Request.PhysicalApplicationPath + "\\" + cDir + "\\Search.aspx", false, System.Text.Encoding.GetEncoding("GB2312"));
            //        sw.Write(TempStr);
            //        sw.Close();
            //    }
            //}
        }
        #endregion

        #region 创建一个目录
        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        #endregion

        #region 创建一个文件
        /// <summary>
        /// 创建一个文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //创建一个FileInfo对象
                    FileInfo file = new FileInfo(filePath);

                    //创建文件
                    FileStream fs = file.Create();

                    //关闭文件流
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 创建一个文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //创建一个FileInfo对象
                    FileInfo file = new FileInfo(filePath);

                    //创建文件
                    FileStream fs = file.Create();

                    //写入二进制流
                    fs.Write(buffer, 0, buffer.Length);

                    //关闭文件流
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                throw ex;
            }
        }
        #endregion

        #region 获取文本文件的行数
        /// <summary>
        /// 获取文本文件的行数
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static int GetLineCount(string filePath)
        {
            //将文本文件的各行读到一个字符串数组中
            string[] rows = File.ReadAllLines(filePath);

            //返回行数
            return rows.Length;
        }
        #endregion

        #region 获取一个文件的长度
        /// <summary>
        /// 获取一个文件的长度,单位为Byte
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static long GetFileSize(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);

            //获取文件的大小
            return fi.Length;
        }
        #endregion

        #region 获取文件大小并以B，KB，GB，TB
        /// <summary>
        /// 计算文件大小函数(保留两位小数),Size为字节大小
        /// </summary>
        /// <param name="size">初始文件大小</param>
        /// <returns></returns>
        public static string ToFileSize(long size)
        {
            string m_strSize = "";
            long FactSize = 0;
            FactSize = size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString("F2") + " 字节";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + " KB";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " MB";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " GB";
            return m_strSize;
        }
        #endregion

        #region 获取指定目录中的子目录列表
        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向文本文件写入内容

        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        /// <param name="encoding">编码</param>
        public static void WriteText(string filePath, string text, Encoding encoding)
        {
            //向文件写入内容
            File.WriteAllText(filePath, text, encoding);
        }
        #endregion

        #region 向文本文件的尾部追加内容
        /// <summary>
        /// 向文本文件的尾部追加内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="content">写入的内容</param>
        public static void AppendText(string filePath, string content)
        {
            File.AppendAllText(filePath, content);
        }
        #endregion

        #region 将现有文件的内容复制到新文件中
        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="destFilePath">目标文件的绝对路径</param>
        public static void Copy(string sourceFilePath, string destFilePath)
        {
            File.Copy(sourceFilePath, destFilePath, true);
        }
        #endregion

        #region 将文件移动到指定目录
        /// <summary>
        /// 将文件移动到指定目录
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        public static void Move(string sourceFilePath, string descDirectoryPath)
        {
            //获取源文件的名称
            string sourceFileName = GetFileName(sourceFilePath);

            if (IsExistDirectory(descDirectoryPath))
            {
                //如果目标中存在同名文件,则删除
                if (IsExistFile(descDirectoryPath + "\\" + sourceFileName))
                {
                    DeleteFile(descDirectoryPath + "\\" + sourceFileName);
                }
                //将文件移动到指定目录
                File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceFileName);
            }
        }
        #endregion

        #region 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// <summary>
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileNameNoExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name.Split('.')[0];
        }
        #endregion

        #region 从文件的绝对路径中获取扩展名
        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }
        #endregion

        #region 清空指定目录
        /// <summary>
        /// 清空指定目录下所有文件及子目录,但该目录依然保存.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void ClearDirectory(string directoryPath)
        {
            directoryPath = HttpContext.Current.Server.MapPath(directoryPath);
            if (IsExistDirectory(directoryPath))
            {
                //删除目录中所有的文件
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }
                //删除目录中所有的子目录
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }
        #endregion

        #region 清空文件内容
        /// <summary>
        /// 清空文件内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void ClearFile(string filePath)
        {
            //删除文件
            File.Delete(filePath);

            //重新创建该文件
            CreateFile(filePath);
        }
        #endregion

        #region 删除指定目录
        /// <summary>
        /// 删除指定目录及其所有子目录
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void DeleteDirectory(string directoryPath)
        {
            directoryPath = HttpContext.Current.Server.MapPath(directoryPath);
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }
        #endregion

        #region 本地路径
        /// <summary>
        /// 本地路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
        #endregion


    }
}
