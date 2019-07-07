using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace CommonWindowsService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CommonService() 
            };
            ServiceBase.Run(ServicesToRun);
            //CommonService s = new CommonService();
            //Thread.Sleep(999999999);
        }
    }
}
