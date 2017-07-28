using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Common.Utils.Web
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
        #region Cookie操作
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName].Value.ToString();
            }
            return "";
        }
        /// <summary>
        /// 删除Cookie对象
        /// </summary>
        /// <param name="CookiesName">Cookie对象名称</param>
        public static void RemoveCookie(string CookiesName)
        {
            HttpCookie objCookie = new HttpCookie(CookiesName.Trim());
            objCookie.Expires = DateTime.Now.AddYears(-5);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        #endregion
    }
}
