using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common.Utils
{
    public class StringHelper
    {
        public static string ToTitleCase(object obj)
        {
            if (obj == null)
                return string.Empty;

            string result = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Convert.ToString(obj).ToLower());
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\w+\&\w+", m => m.ToString().ToUpper());
            return result;
        }

        public static string[] SplitString(string str, string[] delimiters)
        {
            //delimiters = new string[] { "\r\n\r\n" };
            string[] strs = str.Split(delimiters, StringSplitOptions.None);
            return strs;
        }

        // Byte[] => String
        public static string ConvertBytesToString(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        // Byte[] => UTF8 String
        public static string ConvertBytesToUTF8String(byte[] bytes,params int[] skiptake)
        {
            if (skiptake.Length > 1 && skiptake[0] < bytes.Length && skiptake[0] + skiptake[1] < bytes.Length)
                return System.Text.UTF8Encoding.UTF8.GetString(bytes.Skip(skiptake[0]).Take(skiptake[1]).ToArray());
            else if (skiptake.Length == 1 && skiptake[0] < bytes.Length)
                return System.Text.UTF8Encoding.UTF8.GetString(bytes.Skip(skiptake[0]).ToArray());
            else
                return System.Text.UTF8Encoding.UTF8.GetString(bytes);
        }

        // Byte[] => GB2312 String
        public static string ConvertBytesToGB2312String(byte[] bytes)
        {
            return System.Text.Encoding.GetEncoding("GB2312").GetString(bytes);
        }

        // String => Byte[]  
        static byte[] ConvertToBytes(string str)
        {
           return Encoding.Default.GetBytes(str);
        }

        // List<String> => String
        static string ConvertListToString(List<string> strList)
        {
          return  String.Join(",", strList);
        }

        //List<string>  => List<int>
        static List<int> ConvertStrListToIntList(List<string> strList)
        {
            return strList.Select(int.Parse).ToList();
        }

        //String[] => int[]
        static int[] ConvertStrArrayToIntArray(string[] strs)
        {
            return Array.ConvertAll(strs, id => Convert.ToInt32(id));
        }

        // int[] => String
        static string ConvertArrayToString(int[] idList)
        {
            return string.Join(",", Array.ConvertAll<int, string>(idList, delegate(int i) { return i.ToString(); }));
        }

        //string => char[]
        static char[] ConverToChars(string str)
        {
            return str.ToCharArray();
        }
    }
}
