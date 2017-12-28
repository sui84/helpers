using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    // require testing
    public class FindDeletedData
    {
                /// <summary>
        /// 分析sql2005日志，找回被delete的数据，引用请保留以下信息
        /// 作者：jinjazz (csdn的剪刀)
        /// 作者blog：http://blog.csdn.net/jinjazz
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
            {
                conn.ConnectionString = "Data Source=localhost;Initial Catalog=dbLogTest;Integrated Security=True";
                conn.Open();
                using (System.Data.SqlClient.SqlCommand command = conn.CreateCommand())
                {
                    //察看dbo.log_test对象的sql日志
                    command.CommandText = @"SELECT allocunitname,operation,[RowLog Contents 0] as r0,[RowLog Contents 1]as r1 
                                from::fn_dblog (null, null)   
                                where allocunitname like 'dbo.log_test2%'and
                                operation in('LOP_INSERT_ROWS','LOP_DELETE_ROWS')";  //,'LOP_MODIFY_ROW'

                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                    //根据表字段的顺序建立字段数组
                    Datacolumn[] columns = new Datacolumn[]
                        {
                            new Datacolumn("id", System.Data.SqlDbType.Int),
                            new Datacolumn("code", System.Data.SqlDbType.Char,10),
                            new Datacolumn("name", System.Data.SqlDbType.VarChar),
                            new Datacolumn("date", System.Data.SqlDbType.DateTime),
                            new Datacolumn("memo", System.Data.SqlDbType.VarChar),
                            new Datacolumn("note", System.Data.SqlDbType.VarChar)
                        };
                    //循环读取日志
                    while (reader.Read())
                    {
                        /*

                        string name = "name"; //jinjazz  name
                        //byte[] data2 = Encoding.Unicode.GetBytes(name);
                        char[] values = name.ToCharArray();
                        foreach (char letter in values)
                        {
                            // Get the integral value of the character. 
                            int value = Convert.ToInt32(letter); 
                            // Convert the decimal value to a hexadecimal value in string form. 
                            string hexOutput = String.Format("{0:X}", value); 
                            Console.WriteLine("Hexadecimal value of {0} is {1}", letter, hexOutput); 

                        }
                        //Before
                        //0x2A
                        //002E00
                        //6A696E6A617A7A    jinjazz

                        //6E616D65   name

                        //After
                        //0x29 
                        //002D00   
                        //50686F656265   Phoebe
                         
                          */

                       

                        string hexStr = StringToHexString("jinjazz");
                       // string str = HexStringToString("6A696E6A617A7A");
                      //  string str = Encoding.Unicode.GetString(strToToHexByte("6A696E6A617A7A"));
                        string hexStr2 = "0x2A002E006A696E6A617A7A";
                        hexStr2 = hexStr2.Substring(5);
                       // string str = Encoding.Unicode.GetString(strToToHexByte(hexStr2));

                        Datacolumn nameColumn = new Datacolumn("name", System.Data.SqlDbType.VarChar);
                        Datacolumn lengthColumn =new Datacolumn("id", System.Data.SqlDbType.Int);
                        lengthColumn.Value = BitConverter.ToInt32(strToToHexByte("2E00"),0);
                        
                        
                        //StringToHexString
                        //HexStringToString


                        byte[] data = (byte[])reader["r0"];
                        try
                        {
                            //把二进制数据结构转换为明文
                            TranslateData(data, columns);
                            Console.WriteLine("数据对象{1}的{0}操作：", reader["operation"], reader["allocunitname"]);
                            foreach (Datacolumn c in columns)
                            {
                                Console.WriteLine("{0} = {1}", c.Name, c.Value);
                            }
                            Console.WriteLine();
                        }
                        catch
                        {
                            //to-do...
                        }

                    }
                    reader.Close();
                }
                conn.Close();
            }
            Console.WriteLine("************************日志分析完成");
            Console.ReadLine();
        }

        static string StringToHexString(string s)
        {
            byte[] b = Encoding.Unicode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += "%" + Convert.ToString(b[i], 16);
              //  if (b[i]==0) continue;
                result +=  Convert.ToString(b[i], 16);
            }
            return result;
        }
        static string HexStringToString(string hs)
        {
            //以%分割字符串，并去掉空字符
            string[] chars = hs.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] b = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                b[i] = Convert.ToByte(chars[i], 16);
            }
            //按照指定编码将字节数组变为字符串
            return Encoding.Unicode.GetString(b);
        } 
        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
       /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(string hexString)
        {
             hexString = hexString.Replace(" ", "");
           if ((hexString.Length % 2) != 0)
                 hexString += " ";
            /*
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            */
           byte[] returnBytes = new byte[hexString.Length];
           for (int i = 0; i < returnBytes.Length-1; i = i + 2)
           {
               returnBytes[i] = Convert.ToByte(hexString.Substring(i, 2), 16);
               returnBytes[i+1] = 0;
           }
            return returnBytes;
         }


        //自定义的column结构
        public class Datacolumn
        {
            public string Name;
            public System.Data.SqlDbType DataType;
            public short Length = -1;
            public object Value = null;
            public Datacolumn(string name, System.Data.SqlDbType type)
            {
                Name = name;
                DataType = type;
            }
            public Datacolumn(string name, System.Data.SqlDbType type, short length)
            {
                Name = name;
                DataType = type;
                Length = length;
            }
        }
        /// <summary>
        /// sql二进制结构翻译，这个比较关键，测试环境为sql2005，其他版本没有测过。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        static void TranslateData(byte[] data, Datacolumn[] columns)
        {
            //我只根据示例写了Char,DateTime,Int三种定长度字段和varchar一种不定长字段，其余的有兴趣可以自己补充
            //这里没有暂时没有考虑Null和空字符串两种情况，以后会补充。

            //引用请保留以下信息：
            //作者：jinjazz 
            //sql的数据行二进制结构参考我的blog
            //http://blog.csdn.net/jinjazz/archive/2008/08/07/2783872.aspx
            //行数据从第5个字节开始
            short index = 4;
            //先取定长字段
            foreach (Datacolumn c in columns)
            {
                switch (c.DataType)
                {
                    case System.Data.SqlDbType.Char:
                        //读取定长字符串，需要根据表结构指定长度
                        c.Value = System.Text.Encoding.Default.GetString(data, index, c.Length);
                        index += c.Length;
                        break;
                    case System.Data.SqlDbType.DateTime:
                        //读取datetime字段，sql为8字节保存
                        System.DateTime date = new DateTime(1900, 1, 1);
                        //前四位1/300秒保存
                        int second = BitConverter.ToInt32(data, index);
                        date = date.AddSeconds(second / 300);
                        index += 4;
                        //后四位1900-1-1的天数
                        int days = BitConverter.ToInt32(data, index);
                        date = date.AddDays(days);
                        index += 4;
                        c.Value = date;
                        break;
                    case System.Data.SqlDbType.Int:
                        //读取int字段,为4个字节保存
                        c.Value = BitConverter.ToInt32(data, index);
                        index += 4;
                        break;
                    default:
                        //忽略不定长字段和其他不支持以及不愿意考虑的字段
                        break;
                }
            }
            //跳过三个字节
            index += 3;
            //取变长字段的数量,保存两个字节
            short varColumnCount = BitConverter.ToInt16(data, index);
            index += 2;
            //接下来,每两个字节保存一个变长字段的结束位置,
            //所以第一个变长字段的开始位置可以算出来
            short startIndex = (short)(index + varColumnCount * 2);
            //第一个变长字段的结束位置也可以算出来
            short endIndex = BitConverter.ToInt16(data, index);
            //循环变长字段列表读取数据
            foreach (Datacolumn c in columns)
            {
                switch (c.DataType)
                {
                    case System.Data.SqlDbType.VarChar:
                        //根据开始和结束位置，可以算出来每个变长字段的值
                        c.Value = System.Text.Encoding.Default.GetString(data, startIndex, endIndex - startIndex);
                        //下一个变长字段的开始位置
                        startIndex = endIndex;
                        //获取下一个变长字段的结束位置
                        index += 2;
                        endIndex = BitConverter.ToInt16(data, index);
                        break;
                    default:
                        //忽略定长字段和其他不支持以及不愿意考虑的字段
                        break;
                }
            }
            //获取完毕
        }
    }
}
