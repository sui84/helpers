using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ibm.as400.access;
using Common.Utils.Files;
using System.IO;

namespace Common.Tests
{
    public class TestJarHelper
    {
        static void Main(string[] args)
        {
            
           

            FileHelper fh = new FileHelper();
            fh.SplitFileBySize("MB", 1000, @"D:\DB\passdict\bigdict", @"D:\DB\passdict\28GB超大密码字典_28GBwordlist\acdc's dictionary.txt");
            //AS400 as400 = new AS400("","","");
            //string str = "";
        }
    }
}
