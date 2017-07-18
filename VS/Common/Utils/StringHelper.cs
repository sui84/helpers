using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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

        // String => Byte[]  
        public static byte[] ToByteArray(string content)
        {
            byte[] bytes = new byte[content.Length * sizeof(char)];
            System.Buffer.BlockCopy(content.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
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

        /// <summary>
        /// Convert an IBM EBCDIC string into ASCII
        /// </summary>
        /// <param name="strEBCDICString">IBM AS400 EBCDIC string</param>
        /// <returns>ASCII String</returns>
        public static string ConvertEBCDICtoASCII(byte[] strEBCDICString)
        {
            StringBuilder sb = new StringBuilder();
            char newc = '\0';

            strEBCDICString = TrimByteArray(strEBCDICString);

            for (int i = 0; i < strEBCDICString.Length; i++)
            {
                if (strEBCDICString[i] != '\0')
                {
                    newc = Convert.ToChar(e2a[(int)strEBCDICString[i]]);
                    sb.Append(newc);
                }
            }
            string result = sb.ToString();
            sb = null;

            return result;
        }

        /// <summary>
        /// Convert an ASCII string to IBM EBCDIC
        /// </summary>
        /// <param name="strASCIIString">The ASCII string to convert</param>
        /// <returns>IBM EBCDIC array</returns>
        public static byte[] ConvertASCIItoEBCDIC(string strASCIIString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] result = encoding.GetBytes(strASCIIString);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a2e[(int)result[i]];
            }

            return result;
        }

        /// <summary>
        /// Convert a decimal value to IBM Packed Decimal byte array.
        /// </summary>
        /// <param name="d">The decimal value</param>
        /// <param name="digits">Number of digits</param>
        /// <param name="decimalPosition">Decimal position</param>
        /// <returns>IBM Packed decimal byte array</returns>
        public static byte[] ToPackedDecimal(decimal d, int digits, int decimalPosition)
        {
            long value = (long)(d * (long)Math.Pow(10, decimalPosition));

            int size = (int)Math.Ceiling((double)(digits + 1) / 2);

            if (value >= Math.Pow(10, digits))
                throw new OverflowException(string.Format("Decimal value {0} exceeds {1} digits.", d, digits));

            Stack<byte> comp3 = new Stack<byte>(size);

            byte currentByte;
            if (value < 0)
            {
                currentByte = 0x0d;
                value = -value;
            }
            else
            {
                currentByte = 0x0f;
            }

            bool byteComplete = false;
            while (value != 0)
            {
                if (byteComplete)
                    currentByte = (byte)(value % 10);
                else
                    currentByte |= (byte)((value % 10) << 4);
                value /= 10;
                byteComplete = !byteComplete;
                if (byteComplete)
                    comp3.Push(currentByte);
            }
            if (!byteComplete)
                comp3.Push(currentByte);

            int count = comp3.Count;
            if (size > count)
            {
                for (int i = 0; i < size - count; i++)
                {
                    comp3.Push(0);
                }
            }
            return comp3.ToArray();
        }

        /// <summary>
        /// Character lookup for EBCDIC ASCII Conversion
        /// </summary>
        private static int[] e2a = new int[256]{
            0, 1, 2, 3,156, 9,134,127,151,141,142, 11, 12, 13, 14, 15,
            16, 17, 18, 19,157,133, 8,135, 24, 25,146,143, 28, 29, 30, 31,
            128,129,130,131,132, 10, 23, 27,136,137,138,139,140, 5, 6, 7,
            144,145, 22,147,148,149,150, 4,152,153,154,155, 20, 21,158, 26,
            32,160,161,162,163,164,165,166,167,168, 91, 46, 60, 40, 43, 33,
            38,169,170,171,172,173,174,175,176,177, 93, 36, 42, 41, 59, 94,
            45, 47,178,179,180,181,182,183,184,185,124, 44, 37, 95, 62, 63,
            186,187,188,189,190,191,192,193,194, 96, 58, 35, 64, 39, 61, 34,
            195, 97, 98, 99,100,101,102,103,104,105,196,197,198,199,200,201,
            202,106,107,108,109,110,111,112,113,114,203,204,205,206,207,208,
            209,126,115,116,117,118,119,120,121,122,210,211,212,213,214,215,
            216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,
            123, 65, 66, 67, 68, 69, 70, 71, 72, 73,232,233,234,235,236,237,
            125, 74, 75, 76, 77, 78, 79, 80, 81, 82,238,239,240,241,242,243,
            92,159, 83, 84, 85, 86, 87, 88, 89, 90,244,245,246,247,248,249,
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57,250,251,252,253,254,255
        };

        /// <summary>
        /// Character lookup for EBCDIC ASCII Conversion
        /// </summary>
        private static byte[] a2e = new byte[256]{
            0, 1, 2, 3, 55, 45, 46, 47, 22, 5, 37, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 60, 61, 50, 38, 24, 25, 63, 39, 28, 29, 30, 31,
            64, 79,127,123, 91,108, 80,125, 77, 93, 92, 78,107, 96, 75, 97,
            240,241,242,243,244,245,246,247,248,249,122, 94, 76,126,110,111,
            124,193,194,195,196,197,198,199,200,201,209,210,211,212,213,214,
            215,216,217,226,227,228,229,230,231,232,233, 74,224, 90, 95,109,
            121,129,130,131,132,133,134,135,136,137,145,146,147,148,149,150,
            151,152,153,162,163,164,165,166,167,168,169,192,106,208,161, 7,
            32, 33, 34, 35, 36, 21, 6, 23, 40, 41, 42, 43, 44, 9, 10, 27,
            48, 49, 26, 51, 52, 53, 54, 8, 56, 57, 58, 59, 4, 20, 62,225,
            65, 66, 67, 68, 69, 70, 71, 72, 73, 81, 82, 83, 84, 85, 86, 87,
            88, 89, 98, 99,100,101,102,103,104,105,112,113,114,115,116,117,
            118,119,120,128,138,139,140,141,142,143,144,154,155,156,157,158,
            159,160,170,171,172,173,174,175,176,177,178,179,180,181,182,183,
            184,185,186,187,188,189,190,191,202,203,204,205,206,207,218,219,
            220,221,222,223,234,235,236,237,238,239,250,251,252,253,254,255
        };

        private static byte[] TrimByteArray(byte[] inArray)
        {
            int i = inArray.Length - 1;
            while (inArray[i] == 0)
                --i;
            // now inArray[i] is the last non-zero byte
            byte[] outArray = new byte[i + 1];
            Array.Copy(inArray, outArray, i + 1);
            return outArray;
        }

        // string => byte[]
        public static byte[] StringToBytes(string input)
        {
            byte[] bytes = new byte[input.Length * sizeof(char)];
            System.Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        // byte[] => string
        public static string BytesToString(byte[] input)
        {
            char[] chars = new char[input.Length / sizeof(char)];
            System.Buffer.BlockCopy(input, 0, chars, 0, input.Length);
            return new string(chars);
        }

        /// <summary>
        /// Removes control characters and other non-UTF-8 characters
        /// </summary>
        /// <param name="inString">The string to process</param>
        /// <returns>A string with no control characters or entities above 0x00FD</returns>
        public static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((/*ch < 0x00FD && */ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }
    }
}
