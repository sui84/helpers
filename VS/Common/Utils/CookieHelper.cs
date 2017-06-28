using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public class CookieHelper
    {
        //public HttpCookieCollection GetCookies()
        //{
        //    return HttpContext.Current.Request.Cookies;
        //}
        //public NameValueCollection GetHeaders()
        //{
        //    return HttpContext.Current.Request.Headers;
        //}

        //FormsAuthentication.SetAuthCookie(userName, true);
        //HttpContext.Current.User.Identity.Name
        //Login in 
        //FormsAuth.SignIn(model.UserName.ToUpper(), false);
        //Login out
        //FormsAuthentication.SignOut();
        //HttpContext.Current.Session.Abandon();
        //HttpContext.Current.Session.Clear();

        // WebHelper.CurrentAd = string.Format("WindowsIdentity.GetCurrent().Name:{0}\r\nRequest.LogonUserIdentity.Name:{1}\r\nRequest.LogonUserIdentity.Name:{2}\r\n",
        // WindowsIdentity.GetCurrent().Name, Request.LogonUserIdentity.Name, Request.LogonUserIdentity.Name);
        // HttpContext.Current.User.Identity

        //In ASP.NET, when you modify web.config file, IIS will recycle the app pool. 	
        //HttpRuntime.UnloadAppDomain() 
        //File.SetLastWriteTime(this.Server.MapPath("~/web.config"), DateTime.Now);
    }
}
