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

        //string => Hexadecimal string
        public static string ConverToHexString(string istr)
        {
            StringBuilder hexstr = new StringBuilder("0x");
            char[] values = istr.ToCharArray();
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                hexstr.Append(hexOutput);
            }
            return hexstr.ToString();
        }

        /// 作用：将字符串内容转化为16进制数据编码，其逆过程是Decode
        public static string HexEncode(string strEncode)
        {
            string strReturn = "";//  存储转换后的编码
            foreach (short shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X4");
            }
            return strReturn;
        }

        /// 作用：将16进制数据编码转化为字符串，是Encode的逆过程
        public static string HexDecode(string strDecode)
        {
            string sResult = "";
            for (int i = 0; i < strDecode.Length / 4; i++)
            {
                sResult += (char)short.Parse(strDecode.Substring(i * 4, 4), global::System.Globalization.NumberStyles.HexNumber);
            }
            return sResult;
        }

         /**   
         * 字符串转换成十六进制字符串  
         * @param String str 待转换的ASCII字符串  
         * @return String 每个Byte之间空格分隔，如: [61 6C 6B]  
         */      
        public static String Str2HexStr(String str)    
        {      
  
            char[] chars = "0123456789ABCDEF".ToCharArray();      
            StringBuilder sb = new StringBuilder("");    
            byte[] bs = Encoding.Default.GetBytes(str) ;    
            int bit;      
        
            for (int i = 0; i < bs.Length; i++)    
            {      
                bit = (bs[i] & 0x0f0) >> 4;      
                sb.Append(chars[bit]);      
                bit = bs[i] & 0x0f;      
                sb.Append(chars[bit]);    
               // sb.Append(' ');    
            }      
            return sb.ToString().Trim();      
        }    
    
        /**   
         * 十六进制转换字符串  
         * @param String str Byte字符串(Byte之间无分隔符 如:[616C6B])  
         * @return String 对应的字符串  
         */      
        public static String HexStr2Str(String hexStr,string charset= "utf-8")    
        {      
            byte[] bytes = HexStr2Bytes(hexStr);      
            return System.Text.Encoding.GetEncoding(charset).GetString(bytes);
        }

        public static byte[] HexStr2Bytes(String hexStr)
        {
            String str = "0123456789ABCDEF";
            char[] hexs = hexStr.ToCharArray();
            byte[] bytes = new byte[hexStr.Length / 2];
            int n;

            for (int i = 0; i < bytes.Length; i++)
            {
                n = str.IndexOf(hexs[2 * i]) * 16;
                n += str.IndexOf(hexs[2 * i + 1]);
                bytes[i] = (byte)(n & 0xff);
            }
            return bytes;
        }  

    }
}
