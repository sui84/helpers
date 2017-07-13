using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace Common.Utils
{
    /// <summary>
    /// A utility class that is used to access Microsoft Windows SharePoint Services.
    /// (Created by Jacky Chu on 2008/07/17)
    /// </summary>
    public class SharePointHelper : IDisposable
    {
        private SPSite site = null;
        private SPWeb web = null;

        /// <summary>
        /// Initializes a new instance of SharePointHelper based on the specified host name and port for the site collection.
        /// </summary>
        /// <param name="hostName">A host name for the SharePoint site. (e.g. "cpwd01", "cpwp01", etc.)</param>
        /// <param name="port">A port number for the SharePoint site. (e.g. 80, 8080, etc.)</param>
        public SharePointHelper(string hostName, int port) : this(string.Format("http://{0}:{1}/", hostName, port))
        {
            
        }

        /// <summary>
        /// Initializes a new instance of SharePointHelper based on the specified absolute URL for the site collection.
        /// </summary>
        /// <param name="url">A string that specifies the absolute URL for the site collection. (e.g. "http://cpwd01:8080/")</param>
        public SharePointHelper(string url)
        {
            site = new SPSite(url);
            web = site.OpenWeb();
            //web = site.RootWeb;
            web.AllowUnsafeUpdates = true;
        }

        /// <summary>
        /// Recursively creates the specified folder path including the ancestors if the folder doesn't exist.
        /// </summary>
        /// <param name="folderPath">A string that specifies the relative folder path for the site collection. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <returns>An instance of SPFolder if the specified folder is not empty, otherwise returns null.</returns>
        public SPFolder CreateFolders(string folderPath)
        {
            if (folderPath != null && folderPath != "") {
                if (folderPath.EndsWith("/")) {
                    folderPath = folderPath.Substring(0, folderPath.Length - 1);
                }
                SPFolder folder = web.GetFolder(folderPath);
                if (!folder.Exists) {
                    if (folderPath.LastIndexOf("/") > 0) {
                        string parentPath = folderPath.Substring(0, folderPath.LastIndexOf("/"));
                        CreateFolders(parentPath);
                    }
                    folder = web.Folders.Add(folderPath);
                }
                return folder;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Uploads a file with check-in comment into the specified folder for the site collection. The folder path will be created if it doesn't exist.
        /// Custom columns can also be specified by key-value pairs collections.
        /// P.S. It does overwrite the existing file, please use this method carefully.
        /// </summary>
        /// <param name="folderPath">A string that specifies the relative folder path for the site collection. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <param name="fileName">A file name under the given folder path to be uploaded. (e.g. "abc.txt")</param>
        /// <param name="contents">A byte array that represents the file contents.</param>
        /// <param name="remarks">A string that specifies the check-in comment of the file.</param>
        /// <param name="properties">(Optional) A key-value pair collection that contains file custom column values, the key is the display name of the column. (e.g. key="AttachmentId" value=123)</param>
        /// <returns>An instance of SPFile just uploaded.</returns>
        public SPFile UploadFile(string folderPath, string fileName, byte[] contents, string remarks, Dictionary<string, object> properties)
        {
            SPFolder folder = CreateFolders(folderPath);
            SPFile file = null;
            string url = buildUrl(folderPath, fileName);
            file = web.GetFile(url);
            string comment = remarks;
            if (comment == null) comment = "";
            if (file != null && file.Exists) {
                file = folder.Files.Add(url, contents, true, comment, true);
                if (properties != null && properties.Count > 0) {
                    SPListItem listItem = file.Item;
                    foreach (string key in properties.Keys) {
                        listItem[key] = properties[key];
                    }
                    listItem.Update();
                }
                //file.SaveBinary(contents);

                //if (remarks != null && remarks != "") {
                //    file.CheckOut();
                //    file.CheckIn(remarks);
                //}
            } else {  //file doesn't exist
                //add the file under the folder
                file = folder.Files.Add(url, contents, true, comment, true);
            }
            return file;
        }

        /// <summary>
        /// Uploads a file with check-in comment into the specified folder for the site collection. The folder path will be created if it doesn't exist.
        /// P.S. It does overwrite the existing file, please use this method carefully.
        /// </summary>
        /// <param name="folderPath">A string that specifies the relative folder path for the site collection. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <param name="fileName">A file name under the given folder path to be uploaded. (e.g. "abc.txt")</param>
        /// <param name="contents">A byte array that represents the file contents.</param>
        /// <param name="remarks">A string that specifies the check-in comment of the file.</param>
        /// <returns>An instance of SPFile just uploaded.</returns>
        public SPFile UploadFile(string folderPath, string fileName, byte[] contents, string remarks)
        {
            return this.UploadFile(folderPath, fileName, contents, remarks, null);
        }

        /// <summary>
        /// Returns the file contents of the specified relative folder path and file name for the site collection.
        /// </summary>
        /// <param name="folderPath">A relative folder path for the site collection. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <param name="fileName">A file name under the given folder path for the site collection. (e.g. "abc.txt")</param>
        /// <returns>A byte array for the file contents if the specified file exists, otherwise returns null.</returns>
        public byte[] DownloadFile(string folderPath, string fileName)
        {
            string url = buildUrl(folderPath, fileName);
            SPFile file = web.GetFile(url);
            if (file.Exists) {
                return file.OpenBinary();
            }
            return null;
        }

        /// <summary>
        /// Returns the file contents of the specified relative folder path and file name for the site collection.
        /// </summary>
        /// <param name="filePath">A relative file path for the site collection. (e.g. "Shared Documents/SR/Attachments/sample.pdf")</param>
        /// <returns>A byte array for the file contents if the specified file exists, otherwise returns null.</returns>
        public byte[] DownloadFile(string filePath) {
            string url = filePath;
            SPFile file = web.GetFile(url);
            if (file.Exists) {
                return file.OpenBinary();
            }
            return null;
        }

        /// <summary>
        /// Returns the folder object located at the specified URL.
        /// </summary>
        /// <param name="folderPath">A string that contains the relative URL for the folder. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <returns>An instance of SPFolder.</returns>
        public SPFolder GetFolder(string folderPath)
        {
            return web.GetFolder(folderPath);
        }

        /// <summary>
        /// Returns the file object located at the specified folder path and file name.
        /// </summary>
        /// <param name="folderPath">A string that contains the folder path of the URL for the file. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <param name="fileName">A string that represents the file name for the file. (e.g. "abc.txt")</param>
        /// <returns>An instance of SPFile.</returns>
        public SPFile GetFile(string folderPath, string fileName)
        {
            string url = buildUrl(folderPath, fileName);
            return web.GetFile(url);
        }

        /// <summary>
        /// Returns the file object located at the specified URL.
        /// </summary>
        /// <param name="url">A string that contians the URL for the file</param>
        /// <returns>An instance of SPFile.</returns>
        public SPFile GetFile(string url)
        {
            return web.GetFile(url);
        }

        /// <summary>
        /// Deletes a file located at the specified folder path and file name for the site.
        /// </summary>
        /// <param name="folderPath">A string that contains the folder path of the URL for the file. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <param name="fileName">A string that represents the file name for the file. (e.g. "abc.txt")</param>
        public void DeleteFile(string folderPath, string fileName)
        {
            SPFile file = GetFile(folderPath, fileName);
            file.Delete();
        }

        /// <summary>
        /// Deletes an empty folder located at the specified URL.
        /// </summary>
        /// <param name="folderPath">A string that contains the relative folder path for the site. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <returns>True for success to delete. False for fail to delete because the folder is not empty.</returns>
        public bool DeleteFolder(string folderPath)
        {
            SPFolder folder = GetFolder(folderPath);
            if (folder.SubFolders.Count == 0 && folder.Files.Count == 0) {
                folder.Delete();
                return true;
            } else {
                return false;
            }
        }


        /// <summary>
        /// Check a file whether exists in the site collection.
        /// </summary>
        /// <param name="folderPath">A relative folder path for the site collection. (e.g. "/Shared Documents/SR/Attachments/")</param>
        /// <param name="fileName">A file name under the given folder path for the site collection. (e.g. "abc.txt")</param>
        /// <returns>True for exist. False for not exist.</returns>
        public bool ExistsFile(string folderPath, string fileName)
        {
            bool exists = false;
            string url = buildUrl(folderPath, fileName);
            SPFile file = web.GetFile(url);
            exists = file.Exists;
            return exists;
        }

        /// <summary>
        /// Moves the source file to the destination URL but overwrites an existing file of the same name only if overwrite is true.
        /// </summary>
        /// <param name="fromFilePath">A string that contains the source file URL. (e.g. "/Shared Docuemnts/SR/Attachments/abc.txt")</param>
        /// <param name="toFilePath">A string that contains the destination file URL. (e.g. "/Shared Docuemnts/SR/Archive/def.txt")</param>
        /// <param name="overwrite">true to overwrite an existing file of the same name; otherwise, false.</param>
        /// <exception cref="System.IO.FileNotFoundException">FileNotFoundException will be thrown when the source file doesn't exist.</exception>
        public void MoveFile(string fromFilePath, string toFilePath, bool overwrite)
        {
            SPFile file = GetFile(fromFilePath);
            if (file.Exists) {
                if (toFilePath.LastIndexOf("/") > 0) {
                    CreateFolders(toFilePath.Substring(0, toFilePath.LastIndexOf("/")));
                    file.MoveTo(toFilePath, overwrite);
                }
            } else {
                throw new System.IO.FileNotFoundException(string.Format("File '{0}' could not be found on site {1}.", fromFilePath, web.Url));
            }
        }

        /// <summary>
        /// Copies the source file to the destination URL but overwrites an existing file of the same name only if overwrite is true.
        /// </summary>
        /// <param name="fromFilePath">A string that contains the source file URL.</param>
        /// <param name="toFilePath">A string that contains the destination file URL.</param>
        /// <param name="overwrite">true to overwrite an existing file of the same name; otherwise, false.</param>
        /// <exception cref="System.IO.FileNotFoundException">FileNotFoundException will be thrown when the source file doesn't exist.</exception>
        public void CopyFile(string fromFilePath, string toFilePath, bool overwrite)
        {
            SPFile file = GetFile(fromFilePath);
            if (file.Exists) {
                if (toFilePath.LastIndexOf("/") > 0) {
                    CreateFolders(toFilePath.Substring(0, toFilePath.LastIndexOf("/")));
                    file.CopyTo(toFilePath, overwrite);
                }
            } else {
                throw new System.IO.FileNotFoundException(string.Format("File '{0}' could not be found on site {1}.", fromFilePath, web.Url));
            }
        }

        /// <summary>
        /// Moves the source folder to the destination URL. Exception will be thrown if the destination folder already exists.
        /// </summary>
        /// <param name="fromFolderPath">A string that contains the source folder URL. (e.g. "/Shared Documents/SR/DF-20081022-0001/")</param>
        /// <param name="toFolderPath">A string that contains the destination folder URL. (e.g. "/Shared Documents/SR/SRF-20081022-0003/")</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">DirectoryNotFoundException will be thrown when the source folder doesn't exist.</exception>
        public void MoveFolder(string fromFolderPath, string toFolderPath) {
            SPFolder folder = GetFolder(fromFolderPath);
            if (folder.Exists) {
                string destpath = toFolderPath;
                if (destpath.EndsWith("/") || destpath.EndsWith("\\")) {
                    destpath = destpath.Substring(0, destpath.Length - 1);
                }
                if (destpath.LastIndexOf("/") > 0) {
                    CreateFolders(destpath.Substring(0, destpath.LastIndexOf("/")));
                    folder.MoveTo(destpath);
                }
            } else {
                throw new System.IO.DirectoryNotFoundException(string.Format("Folder '{0}' could not be found on site {1}.", fromFolderPath, web.Url));
            }
        }

        /// <summary>
        /// Returns a new instance of DataTable that contains files under the specified folder path, 
        /// and uses the folder field display names as column names.
        /// </summary>
        /// <param name="folderPath">A string that contains the folder URL.</param>
        /// <returns>An instance of DataTable.</returns>
        public DataTable ListFilesAsDataTable(string folderPath)
        {
            SPFolder folder = GetFolder(folderPath);
            DataTable dt = new DataTable(folder.Name);
            if (folder.Exists) {
                foreach (SPField field in folder.Item.Fields) {
                    if (!dt.Columns.Contains(field.Title)) {
                        dt.Columns.Add(field.Title);
                    } else {

                    }
                }

                foreach (SPFile file in folder.Files) {
                    DataRow row = dt.NewRow();
                    
                    foreach (DataColumn col in dt.Columns) {
                        row[col] = file.Item[col.ColumnName];
                    }
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }


        /// <summary>
        /// Build a relative URL for the site collection based on the specified folder path and file name.
        /// </summary>
        /// <param name="folderPath">A relative folder path for the site collection.</param>
        /// <param name="fileName">A file name under the given folder path.</param>
        /// <returns>A string that represents the relative URL for the site collection.</returns>
        private string buildUrl(string folderPath, string fileName)
        {
            string url = "";
            if (folderPath.EndsWith("/")) {
                url = folderPath + fileName;
            } else {
                url = folderPath + "/" + fileName;
            }
            return url;
        }



        #region IDisposable Members

        public void Dispose()
        {
            try {
                web.Close();
                site.Close();
            } catch (Exception ex) {
                EventLog.WriteEntry("VML.SharePoint", "Error on Dispose(): " + ex.Message + "\n" + ex.StackTrace, EventLogEntryType.Warning);
            }
        }

        #endregion

        ~SharePointHelper()
        {
            this.Dispose();
        }
    }
}
