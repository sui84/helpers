using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils.Log;
using log4net;

namespace Common.Tests
{
    public class TestLogHelper
    {
        static void Main(string[] args)
        {

            var log = Log4netHelper.GetLogger("TEST");
            try
            {
                log.Info("TEST INFO");
                throw new Exception("test");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
