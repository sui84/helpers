using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Net;

namespace Common.Utils
{
//    Compile:
//1.Assembly generation failed - Referenced assembly does not have a strong name. Why doesn't other solutions work?
//Project Properties -> Signing -> uncheck Sign the assembly checkbox

//    BLL: AppDomain.CurrentDomain.BaseDirectory 
//Web:  Server.MapPath("~/");
//1、取得控制台应用程序的根目录方法
//     方法1、Environment.CurrentDirectory 取得或设置当前工作目录的完整限定路径
//     方法2、AppDomain.CurrentDomain.BaseDirectory 获取基目录，它由程序集冲突解决程序用来探测程序集
// 2、取得Web应用程序的根目录方法
//     方法1、HttpRuntime.AppDomainAppPath.ToString();//获取承载在当前应用程序域中的应用程序的应用程序目录的物理驱动器路径。用于App_Data中获取
//     方法2、Server.MapPath("") 或者 Server.MapPath("~/");//返回与Web服务器上的指定的虚拟路径相对的物理文件路径
//     方法3、Request.ApplicationPath;//获取服务器上ASP.NET应用程序的虚拟应用程序根目录
// 3、取得WinForm应用程序的根目录方法
//     1、Environment.CurrentDirectory.ToString();//获取或设置当前工作目录的完全限定路径
//     2、Application.StartupPath.ToString();//获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称
//     3、Directory.GetCurrentDirectory();//获取应用程序的当前工作目录
//     4、AppDomain.CurrentDomain.BaseDirectory;//获取基目录，它由程序集冲突解决程序用来探测程序集
//     5、AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称
//其中：以下两个方法可以获取执行文件名称
//     1、Process.GetCurrentProcess().MainModule.FileName;//可获得当前执行的exe的文件名。
//     2、Application.ExecutablePath;//获取启动了应用程序的可执行文件的路径，包括可执行文件的名称

    // Me.Request.Params.Item("__EVENTTARGET")
    public class SystemHelper
    {
        public void AddAccessToFolder(string dir)
        {
            try{
                DirectoryInfo  di = new DirectoryInfo(dir);
                di.Attributes = FileAttributes.Normal;
                DirectorySecurity ds = di.GetAccessControl();
                ds.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                di.SetAccessControl(ds);
            }
            catch(Exception ex){
            }
        }
        
        //待修改
        public void GetAccessToFolder(string dir)
        {
            try{
                DirectoryInfo  di = new DirectoryInfo(dir);
                DirectorySecurity ds = di.GetAccessControl();
                //ds.GetAccessRules(true, true, System.Security.Principal.NTAccount.GetType());
            }
            catch (Exception ex)
            {
            }
        }

        static Boolean DownloadFile(string url,string filePath,string proxyAddreww, int proxyPort, string userName, string password)
        {
            System.Net.WebClient wc = new WebClient();
            if (!string.IsNullOrEmpty(proxyAddreww))
            {
                wc.Proxy = new WebProxy(proxyAddreww, proxyPort);
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }
            wc.Credentials = new NetworkCredential(userName, password);
            wc.DownloadFile(url, filePath);
            return true;
        }
    }
}
