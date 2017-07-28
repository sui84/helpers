using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Data;
using Common.Utils.Log;
using Common.Utils.DB;
using Common.Utils.Files;


namespace Common.Utils
{
    public class ADHelper
    {
    //private string _path;
    //private string _filterAttribute;

    //public ADHelper(string path)
    //{
    //  _path = path;
    //}
   
    public static string GetLDAPPathByDomain(string fullDomain){
        StringBuilder sb = new StringBuilder();
        sb.Append("LDAP://").Append(fullDomain).Append("/");
        string[] pathPortions = fullDomain.Split('.'); 
        foreach (string pathPortion in pathPortions)  {
            sb.Append("DC=").Append(pathPortion).Append(",");
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString(); 
    }

    private static string GetFulDomainByAd(string ad, ref string domain, ref string username)
    {
        string fullDomain = string.Format(@"{0}\{1}",domain,username); 
        return fullDomain;
    }

    public static bool IsAuthenticated(string username, string pwd)
    {
        string ad = username;
        string domain = string.Empty;
        string fullDomain = GetFulDomainByAd(ad,ref domain ,ref username);
        bool isAuth = IsAuthenticated(fullDomain , domain, username, pwd);
        return isAuth;
    }

    public static bool IsAuthenticated(string fullDomain, string domain, string username, string pwd)
    {
        string ldapPath = GetLDAPPathByDomain(fullDomain);
        string domainAndUsername = domain + @"\" + username;
        using (DirectoryEntry entry = new DirectoryEntry(ldapPath, domainAndUsername, pwd))
        {

            //try
            //{
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;
                if (null == obj)
                {
                    return false;
                }

                using (DirectorySearcher search = new DirectorySearcher(entry))
                {

                    search.Filter = "(SAMAccountName=" + username + ")";
                    search.PropertiesToLoad.Add("cn");
                    SearchResult result = search.FindOne();

                    if (null == result)
                    {
                        return false;
                    }

                    //if (Convert.ToInt32(result.Properties["lockoutTime"]) > 0)
                    //{
                    //    throw new Exception("User Account is locked. ");
                    //}

                    //Update the new path to the user in the directory.
                    // _path = result.Path;
                    // _filterAttribute = (string)result.Properties["cn"][0];
                }
            //}
            //catch (Exception ex)
            //{
            //    //throw new ApplicationException("Error authenticating user. " + ex.Message, ex);
            //    LogFormatHelper.LogFunction(string.Format("Error authenticating user {0}.{1} ",username, ex.Message));
            //    return false;
            //}
        }
        return true;
    }

    public static string GetPropertyiesByAd(string ad,string properties){
        DirectoryEntry entry = new DirectoryEntry();
        string domain = string.Empty;
        string username = string.Empty;
        string fullDomain = GetFulDomainByAd(ad, ref domain, ref username);
        entry.Path = "LDAP://" + fullDomain;
        DirectorySearcher search = new DirectorySearcher(entry);
        search.Filter = "(&(objectClass=user)(SAMAccountName=" + username + "))";
        search.PropertiesToLoad.Add(properties);
        SearchResult result = search.FindOne();

        if (result!=null && result.Properties.Contains(properties))
        {
            return result.Properties[properties][0].ToString();
        }
        return string.Empty;
    }

    public static string[] GetPropertyiesByDesc(string path, string desc, string[] properties)
    {
        DirectoryEntry entry = new DirectoryEntry();
        entry.Path = path;
        DirectorySearcher search = new DirectorySearcher(entry);
        // search.Filter = "(&(objectClass=user)(Description=" + desc + "))";
        search.Filter = @"(&(&(objectClass=*)(objectCategory=user))(Description=*" + desc + "))";
        foreach (string property in properties)
        {
            search.PropertiesToLoad.Add(property);
        }

        string[] values = new string[properties.Length];
        int i = 0;
        SearchResult result = search.FindOne();
        if (result != null)
        {
            foreach (string property in properties)
            {
                if (result.Properties.Contains(property))
                {
                    values[i++] = result.Properties[property][0].ToString();
                }
            }
        }
        return values;
    }

    public static System.DirectoryServices.PropertyCollection GetPropertyCollectionByDesc(string ldapPath, string ad, string[] properties)
    {
        string strFilter = @"(&(&(objectClass=*)(objectCategory=user))(sAMAccountName=" + ad + "))";
        System.DirectoryServices.DirectoryEntry objDirectoryEntry = new System.DirectoryServices.DirectoryEntry();
        objDirectoryEntry.Path = ldapPath;
        System.DirectoryServices.DirectorySearcher objDirectorySearcher = new System.DirectoryServices.DirectorySearcher();
        objDirectorySearcher.SearchRoot = objDirectoryEntry;
        objDirectorySearcher.Filter = strFilter;
        System.DirectoryServices.SearchResultCollection objSearchResultCollection = objDirectorySearcher.FindAll();
        objDirectoryEntry = objSearchResultCollection[0].GetDirectoryEntry();
        return objDirectoryEntry.Properties;
    }

    public static bool ChangeADPropertyValue(string ldapPath, string ad,string pname,string pvalue)
    {
        bool success = false;
        try
        {
            System.DirectoryServices.DirectoryEntry d = null;
            using (System.DirectoryServices.DirectoryEntry dir = new System.DirectoryServices.DirectoryEntry(ldapPath))
            {
                using (System.DirectoryServices.DirectorySearcher sea = new System.DirectoryServices.DirectorySearcher(dir))
                {
                    sea.Filter = "(&(objectCategory=user)(sAMAccountName=" + ad + "))";
                    System.DirectoryServices.SearchResult res = sea.FindOne();
                    //using (new Impersonator(adusername, addomain, adpassword))
                    //{
                    d = new System.DirectoryServices.DirectoryEntry();
                    d.Path = res.Path;
                    d.Properties[pname].Value = pvalue;
                    d.CommitChanges();
                    //}
                };
            };
            success = true;
        }
        catch (Exception ex)
        {
            success = false;
        }
        return success;
    }
    

    public static void GetADToDB(string ldap,string connStr)
    {
        LogHelper logHelper = new LogHelper();
        logHelper.LogInfo(string.Format("Start GetAD process") ,string.Empty);
        string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
        if (Directory.Exists(folder))
        {
            foreach (string f in Directory.GetFiles(folder))
            {
                File.Delete(f);
            }
            Directory.Delete(folder);
        }
        Directory.CreateDirectory(folder);
        Thread t1  = new Thread(() => GetADInfo(ldap));
        t1.Name = "LDAPThread1";
        t1.Start();
        t1.Join();
        ImportADInfo(folder, connStr);
        logHelper.LogInfo(string.Format("End GetAD process"), string.Empty);

    }

    public static void GetADInfo(string ldap )
    {
        try
        {
            if ((ldap.Trim().Length > 0))
            {
                int ThreadCount = 5;
                ldap = "LDAP://" + ldap;
                using (DirectoryEntry root = new DirectoryEntry(ldap))
                {
                    using (DirectorySearcher deSearch = new DirectorySearcher(root))
                    {
                        deSearch.PageSize = 2000;  // If all ?
                        deSearch.Filter = "(&(objectClass=user))";
                        using (SearchResultCollection resultCol = deSearch.FindAll())
                        {
                            if ((resultCol != null) & resultCol.Count > 0)
                            {
                                int threadCnt = Convert.ToInt32(ThreadCount);
                                LogHelper logHelper = new LogHelper();
                                logHelper.LogInfo(string.Format("Get {0} AD records in {1} Thread Start {2}", resultCol.Count, DateTime.Now, ldap),string.Empty);
                                Parallel.For(0, resultCol.Count, (new ParallelOptions { MaxDegreeOfParallelism = threadCnt }), (int i) =>
                                {
                                    try
                                    {
                                        string fileName = string.Format("{0}\\Temp\\{1}.txt", AppDomain.CurrentDomain.BaseDirectory, Thread.CurrentThread.ManagedThreadId);
                                        SearchResult result = resultCol[i];
                                        ResultPropertyCollection props = result.Properties;
                                        string ad = props.Contains("samaccountname") ? RemoveSpecialCharacter(props["samaccountname"][0].ToString()) : string.Empty;
                                        if (ad.Trim().Length > 0)
                                        {
                                            string mail = props.Contains("mail") ? RemoveSpecialCharacter(props["mail"][0].ToString()) : string.Empty;
                                            System.IO.File.AppendAllText(fileName, string.Format("{0}{6}{1}{6}{2}{6}{3}{6}{4}{6}{5}{7}", ldap, ad,  mail, CheckWindowsAccountActive(props), "|", "\r\n"), System.Text.Encoding.UTF8);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string logFile = string.Format("{0}\\Temp\\Log{1}.txt", AppDomain.CurrentDomain.BaseDirectory, Thread.CurrentThread.ManagedThreadId);
                                        logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), logFile);
                                    }
                                });
                                logHelper.LogInfo(string.Format("Get AD records in {0} Thread End {1}", ldap, DateTime.Now), string.Empty);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper logHelper = new LogHelper();
            logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), string.Empty);
        }
    }

    private static string RemoveSpecialCharacter(string str)
    {
        return str.Replace("\r", "").Replace("\n", "");
    }

    private static bool CheckWindowsAccountActive(ResultPropertyCollection props)
    {
        if (props["userAccountControl"].Count == 0)
        {
            return false;
        }
        const int UF_DISABLED = 0x2;
        bool Show = false;
        int flags = Convert.ToInt32(props["userAccountControl"][0]);
        if (Convert.ToBoolean(flags & UF_DISABLED))
        {
            Show = false;
        }
        else
        {
            Show = true;
        }
        return Show;
    }

    private static void ImportADInfo(string folder,string connStr)
    {
        int ThreadCount = 5;
        DbHelper dbHelper = new DbHelper();
        string[] files = Directory.GetFiles(folder);
        dbHelper.ClearTable("AMS_ADInfo", connStr);
        int threadCnt = Convert.ToInt32(ThreadCount);
        Parallel.For(0, files.Length, (new ParallelOptions { MaxDegreeOfParallelism = threadCnt }), (int i) =>
        {
            LogHelper logHelper = new LogHelper();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(files[i]);
                string[] columns = new string[] {
		            "LDAP",
		            "AD",
		            "Mail",
		            "Active"
	                };
                FileHelper fileHelper = new FileHelper();
                DataTable dt = fileHelper.ConvertToDataTable(lines, columns, '|');
                dbHelper.CopyData(dt, "ADInfo", connStr);
            }
            catch (Exception ex)
            {
                string logFile = string.Format("{0}\\Temp\\Log{1}.txt", AppDomain.CurrentDomain.BaseDirectory, Thread.CurrentThread.ManagedThreadId);
                logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), logFile);
            }
        });

    }

    //        5.使用linq to AD 
    //static DirectoryEntry ROOT = new DirectoryEntry("LDAP://venetianchina.com.cn");

    //var users = new DirectorySource<User>(ROOT, SearchScope.Subtree);
    //users.Log = Console.Out;
    //var groups = new DirectorySource<Group>(ROOT, SearchScope.Subtree);
    //groups.Log = Console.Out;
    //var res1 = from usr in users
    //           select usr;

    //Console.WriteLine("QUERY 1\n=======");
    //foreach (var w in res1)
    //    Console.WriteLine("{0}: {1} {2}", w.Name, w.Description, w.PasswordLastSet);
    //Console.WriteLine();

  //  public string GetGroups()
  //  {
  //    DirectorySearcher search = new DirectorySearcher(_path);
  //    search.Filter = "(cn=" + _filterAttribute + ")";
  //    search.PropertiesToLoad.Add("memberOf");
  //    StringBuilder groupNames = new StringBuilder();

  //    try
  //    {
  //      SearchResult result = search.FindOne();
  //      int propertyCount = result.Properties["memberOf"].Count;
  //      string dn;
  //      int equalsIndex, commaIndex;

  //      for(int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
  //      {
  //        dn = (string)result.Properties["memberOf"][propertyCounter];
  //     equalsIndex = dn.IndexOf("=", 1);
  //        commaIndex = dn.IndexOf(",", 1);
  //        if(-1 == equalsIndex)
  //        {
  //          return null;
  //        }
  //        groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
  //        groupNames.Append("|");
  //      }
  //    }
  //  catch(Exception ex)
  //  {
  //    throw new Exception("Error obtaining group names. " + ex.Message);
  //  }
  //  return groupNames.ToString();
  //}

    }
}
