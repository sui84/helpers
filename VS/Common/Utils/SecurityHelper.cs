using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public  class SecurityHelper
    {
           // deny access if POST request comes from other server
       //public static void CheckRequestValidity(HttpRequest request)
       // {
       //   // ip with 
       //   // deny access if POST request comes from other server
       //   if (request.HttpMethod == "POST" && request.UrlReferrer != null && request.Url.Host != null && request.UrlReferrer.Host != request.Url.Host)
       //   {
       //     YafBuildLink.AccessDenied();
       //   }
      //  }

       //create Random password.
       public static string CreatePassword(int length)
       {
           string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#%&()@${[]}";
           string res = string.Empty;
           var rnd = new Random();
           while (0 < length--)
           {
               res += valid[rnd.Next(valid.Length)];
           }

           return res;
       }
    }
}
