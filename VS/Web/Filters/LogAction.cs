using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Common;
using System.Text;

namespace Mvc4Web.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public sealed class LogAction : ActionFilterAttribute
    {

        private string actionName = string.Empty;

        private Stopwatch sw = null;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            sw = Stopwatch.StartNew();

            actionName = filterContext.ActionDescriptor.ActionName;

            string function = actionName + " Start...";

            if (filterContext.ActionParameters.Count == 0)
            {

                LogFormatHelper.LogRequestParams(function);

            }

            else
            {

                object[] objs = new object[filterContext.ActionParameters.Count];

                int i = 0;

                foreach (var dic in filterContext.ActionParameters)
                {

                    objs[i++] = dic.Value;

                }

                LogFormatHelper.LogRequestParams(function, objs);

            }

            base.OnActionExecuting(filterContext);

        }





        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

            base.OnResultExecuted(filterContext);

            string function = actionName + " End";

            StringBuilder sb = new StringBuilder();

            foreach (var key in filterContext.RouteData.Values.Keys)
            {

                sb.AppendFormat("{0} = {1}", key, filterContext.RouteData.Values[key]).AppendLine();

            }

            string str = filterContext.RouteData.Values.ToString();

            LogFormatHelper.LogRequestParams(function, sw.Elapsed, sb.ToString());



            if (filterContext.Exception != null)
            {

                LogFormatHelper.LogServiceError(filterContext.Exception, actionName);

            }



        }



    }
}
