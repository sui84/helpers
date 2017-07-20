using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils.Web;

namespace Common.Tests
{
    public class TestVerifyCodeHelper
    {
        static void Main(string[] args)
        {
            VerifyCodeHelper vch = new VerifyCodeHelper();
            string vcstr = vch.GetVerifyCode(@"d:\temp\vc.bmp");
        }

    }
}
