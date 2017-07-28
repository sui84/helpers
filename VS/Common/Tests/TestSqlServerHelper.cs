using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management;
using System.Data;
using System.Threading;
using Common.Utils.DB;
using Common.Utils.ConvertType;

namespace Common.Tests
{
    public class testtb
    {
        public int Int1 { get; set; }
        public Int16 Int2 { get; set; }
        public Int32 Int3 { get; set; }
        public Int64 Int4 { get; set; }
        public Int64? Int5 { get; set; }
        public byte Byte1 { get; set; }
        public Byte[] Byte2 { get; set; }
        public short Short1 { get; set; }
        public DateTime DateTime1 { get; set; }
        public DateTime? DateTime2 { get; set; }
        public bool Bool1 { get; set; }
        public Boolean Bool2 { get; set; }
        public Decimal Decimal1 { get; set; }
        public float Float1 { get; set; }
        public Double Double1 { get; set; }
        public String String1 { get; set; }
        public Char Char1 { get; set; }
        

    }
      
    class TestSqlServerHelper
    {
        static void Main(string[] args)
        {
            try
            {

                SqlServerHelper sshelper = new SqlServerHelper();

                testtb t1 = new testtb();
                t1.Int1 = int.MaxValue;
                t1.Int2 = Int16.MaxValue;
                t1.Int3 = Int32.MaxValue;
                t1.Int4 = Int64.MaxValue;
                t1.Byte1 = byte.MaxValue;
                t1.Byte2 = Encoding.Default.GetBytes("test");
                t1.Short1 = short.MaxValue;
                t1.DateTime1 = DateTime.MaxValue;
                t1.Decimal1 = decimal.MaxValue;
                t1.Bool1 = true;
                t1.Bool2 = false;
                t1.Float1 = float.MaxValue;
                t1.Double1 = double.MaxValue;
                t1.String1 = "你好ssadsa";
                t1.Char1 = '9';
                string servername = @"localhost";
                string dbname = "TEST";
                string username = "sa";
                string password = "password";
                //ValueTypeHelper.IsNullable(t1.Int1);
                t1.Int5 = 111;
                ValueTypeHelper.IsNullable(t1.Int5);
                DatabaseCollection dbs = sshelper.GetDatabases(servername);

                Table tb = sshelper.CreateTable(servername, dbname, username, password, t1.GetType(), true);
                tb.Script();
                ConnStrHelper connhelper = new ConnStrHelper();
                string connStr = connhelper.GetMSSQLClientConnStr(servername, dbname, username, password);

                testtb t2 = new testtb();
                t2.String1 = "你好";
                t2.Byte2 = Encoding.Default.GetBytes("test2");
                t2.DateTime1 = DateTime.MaxValue;
                List<testtb> ts = new List<testtb>();
                ts.Add(t1);
                ts.Add(t2);
                DataTable dt = DataTableHelper.CopyToDataTable(ts, null, null);
                DbHelper dh = new DbHelper();
                string[][] mapCols = new string[2][];
                mapCols[0] = mapCols[1] = ReflectionHelper.GetObjectProperties(t1.GetType()).ToArray();
                dh.CopyData(dt, t1.GetType().Name, connStr, mapCols);
                //dh.CopyDataWithTran(dt,  t1.GetType().Name, connStr, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Thread.Sleep(3000);
            }

        }
    }
}
