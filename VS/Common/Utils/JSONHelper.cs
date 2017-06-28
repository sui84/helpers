using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public class JSONHelper
    {
        public static string JsonToString(object obj)
        {
            string str = fastJSON.JSON.Instance.ToJSON(obj);
            return str;
        }

        public static object StringToJson(string str)
        {
            object obj = fastJSON.JSON.Instance.ToObject(str);
            return obj;
        }

    }
}
