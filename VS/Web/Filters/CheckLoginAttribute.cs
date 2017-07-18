using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc4Web.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public sealed class CheckLogin : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session != null)
            {

                if (filterContext.HttpContext.Session.IsNewSession)
                {

                    //LogFormatHelper.LogRequestParams("filterContext.HttpContext.Session.IsNewSession");

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Account", Action = "Login" }));

                }

            }

        }



    }
}
