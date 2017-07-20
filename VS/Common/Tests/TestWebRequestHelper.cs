using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Common.Utils;

namespace Common.Tests
{
    public class TestWebRequestHelper
    {
        const string SESSION = "Session";
        const string COOKIE = "Cookie";

        public class HttpState
        {
            public string SType { get; set; }
            public string SID { get; set; }
            public string SKey { get; set; }
            public string SVType { get; set; } 
            public string SValue { get; set; }
            public int STimeout { get; set; }
            public DateTime SExpires { get; set; }
            public string SMode { get; set; }
            public string SPath { get; set; } 

        }
        static void Main(string[] args)
        {
            string msg = CmdHelper.ExeCommand("ipconfig");
            msg = CmdHelper.ExeCommand(new string[]{"ipconfig","ping www.baidu.com"});

           List<HttpState> hss = new List<HttpState>();
           for (int i = 0; i < HttpContext.Current.Session.Count; i++)
           {
               HttpState state = new HttpState();
               state.SType = SESSION;
               state.SID = HttpContext.Current.Session.SessionID;
               state.SKey  = HttpContext.Current.Session.Keys[i];
               state.SVType = HttpContext.Current.Session[state.SKey].GetType().Name;
               state.SValue = HttpContext.Current.Session[state.SKey].ToString();
               state.STimeout = HttpContext.Current.Session.Timeout;
               state.SMode = HttpContext.Current.Session.Mode.ToString();
               hss.Add(state);
           }
           for (int i = 0; i < HttpContext.Current.Request.Cookies.Count; i++)
           {
               HttpState state = new HttpState();
               state.SType = COOKIE;
               state.SKey = HttpContext.Current.Request.Cookies.Keys[i];
               state.SVType = HttpContext.Current.Request.Cookies[state.SKey].GetType().Name;
               state.SValue = HttpContext.Current.Request.Cookies[state.SKey].ToString();
               state.SExpires = HttpContext.Current.Request.Cookies[state.SKey].Expires;
               state.SPath = HttpContext.Current.Request.Cookies[state.SKey].Path;
               hss.Add(state);
           }
           
        }
    }
}
