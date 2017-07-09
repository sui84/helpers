using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;  

//Microsoft.SqlServer.ConnectionInfo.dll
//Microsoft.SqlServer.Management.Sdk.Sfc.dll
namespace Common.Utils
{
    public class SqlServerHelper
    {
        public ServerConnection GetServer(string serverName, string userName, string password)
        {
            Server s = new Server(serverName);
            ServerConnection sc = s.ConnectionContext;
            sc.LoginSecure = false;
            sc.Login = userName;
            sc.Password = password;
            return sc;
        }

        public DatabaseCollection GetDatabases(string serverName)
        {
            Server s = new Server(serverName);
            return s.Databases;
        }
        public Database GetDatabse(Server s, string dbName)
        {
            ////创建数据库  
            if (!s.Databases.Contains(dbName))
            {
                Database db = new Database(s, dbName);
                db.Create();
                return db;
            }
            else
                return s.Databases[dbName];
        }

        public Table CreateTable(string serverName, string dbName, string userName, string password, Type objType, bool dropFlag = false)
        {
            Server s = new Server(serverName);
            //ServerConnection sc = new ServerConnection(serverName, userName, password);
            Database db = GetDatabse(s, dbName);
            //建表Tb  
            string tbname = objType.Name;
            Table tb = db.Tables[tbname];
            if (tb != null && dropFlag) { tb.Drop(); tb = null; }
            if (tb != null && !dropFlag) return tb;
            PropertyInfo[] props = objType.GetProperties();
            if (tb == null && props.Count()>0)
            {
                // Add Identity field
                tb = new Table(db, tbname);
                Column idenCol = new Column(tb, "Id");
                idenCol.Identity = true;
                idenCol.IdentitySeed = 1;
                idenCol.DataType = DataType.Int;
                idenCol.Nullable = false;
                tb.Columns.Add(idenCol);

                foreach (PropertyInfo p in props)
                {
                    if (p.CanRead && p.PropertyType.Namespace == "System" && !p.PropertyType.IsArray)
                    {
                        Column c = new Column(tb, p.Name);
                        Type columnType = p.PropertyType;
                        // We need to check whether the property is NULLABLE
                        // Not work!!!
                        //if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        //{
                        //    c.Nullable = true;
                        //    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                        //    columnType = p.PropertyType.GetGenericArguments()[0];
                        //}
                        // Still Not work!!!
                        //c.Nullable = ValueTypeHelper.IsNullable(p.GetType());
                        if (p.PropertyType.GetGenericArguments().Count()>0)
                        {    
                            c.Nullable = true;
                            c.DataType = GetSqlServerType(p.PropertyType.GetGenericArguments()[0].Name);
                        }
                        else {
                            c.Nullable = false;
                            c.DataType = GetSqlServerType(p.PropertyType.Name);
                        }
                        tb.Columns.Add(c);
                    }
                }
                tb.Create();
            }
            return tb;
        }

        public DataType GetSqlServerType(String type)
        {
            DataType dataType = DataType.NVarCharMax;
            switch (type)
            {
                case "Char":
                case "String":
                    dataType = DataType.NVarCharMax;
                    break;
                case "Int":
                case "Int16":
                case "Int32":
                case "Byte":
                case "Short":
                    dataType = DataType.Int;
                    break;
                case "DateTime":
                    dataType = DataType.DateTime;
                    break;
                case "Boolean":
                    dataType = DataType.Bit;
                    break;
                case "Byte[]":
                    dataType = DataType.VarBinaryMax;
                    break;
                case "Decimal":
                    //dataType = DataType.Decimal(11, 2);
                    //break;
                case "Int64":
                case "Float":
                case "Double":
                case "Single":  
                    dataType = DataType.Float;
                    break;
                default:
                    break;
            }
            return dataType;
        }

        public void CreateTable(string serverName, string userName, string password, string spName, Type paraType,string spContent)
        {
            //创建存储过程  
            Server s = new Server(serverName);
            ServerConnection sc = new ServerConnection(serverName, userName, password);
            Database db = s.Databases[0];
            StoredProcedure sp = new StoredProcedure(db, spName);
            foreach (PropertyInfo p in paraType.GetProperties())
            {
                StoredProcedureParameter pa1 = new StoredProcedureParameter(sp, "@" + p.Name, GetSqlServerType(p.PropertyType.Name));
                sp.Parameters.Add(pa1);
            }
            sp.TextMode = false;
            sp.TextBody = spContent;
            sp.Create();
        }

        public DataTable GetDataTable(IList list, Type typ)
        {
            DataTable dt = new DataTable();

            // Get a list of all the properties on the object
            PropertyInfo[] pi = typ.GetProperties();

            // Loop through each property, and add it as a column to the datatable
            foreach (PropertyInfo p in pi)
            {
                // The the type of the property
                Type columnType = p.PropertyType;

                // We need to check whether the property is NULLABLE
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    columnType = p.PropertyType.GetGenericArguments()[0];
                }

                // Add the column definition to the datatable.
                dt.Columns.Add(new DataColumn(p.Name, columnType));
            }

            // For each object in the list, loop through and add the data to the datatable.
            foreach (object obj in list)
            {
                object[] row = new object[pi.Length];
                int i = 0;

                foreach (PropertyInfo p in pi)
                {
                    row[i++] = p.GetValue(obj, null);
                }

                dt.Rows.Add(row);
            }

            return dt;
        }

        public DataSet GetDataFromCommand(string connStr, string sqlStr)
        {
            ServerConnection sc = new ServerConnection(connStr);
            DataSet ds = sc.ExecuteWithResults(sqlStr);
            return ds;
        }

        private ScriptingOptions GetScriptOption()
        {
            ScriptingOptions scriptOption = new ScriptingOptions();
            scriptOption.ContinueScriptingOnError = true;
            scriptOption.IncludeIfNotExists = true;
            scriptOption.NoCollation = true;
            scriptOption.ScriptDrops = false;
            scriptOption.ContinueScriptingOnError = true;
            //scriptOption.DriAllConstraints = true;
            scriptOption.WithDependencies = false;
            scriptOption.DriForeignKeys = true;
            scriptOption.DriPrimaryKey = true;
            scriptOption.DriDefaults = true;
            scriptOption.DriChecks = true;
            scriptOption.DriUniqueKeys = true;
            scriptOption.Triggers = true;
            scriptOption.ExtendedProperties = true;
            scriptOption.NoIdentities = false;
            return scriptOption;
        }

        /// <summary>
        /// 生成数据库类型为SqlServer指定表的DDL
        /// </summary>
        private string GenerateSqlServerDDL(Table table)
        {
            StringBuilder sbOutPut = new StringBuilder();
            try
            {
                string ids;
                //编写表的脚本
                sbOutPut = new StringBuilder();
                sbOutPut.AppendLine();
                StringCollection sCollection = table.Script(GetScriptOption());

                foreach (String str in sCollection)
                {
                    //此处修正smo的bug
                    if (str.Contains("ADD  DEFAULT") && str.Contains("') AND type = 'D'"))
                    {
                        ids = str.Substring(str.IndexOf("OBJECT_ID(N'") + "OBJECT_ID(N'".Length, str.IndexOf("') AND type = 'D'") - str.IndexOf("OBJECT_ID(N'") - "OBJECT_ID(N'".Length);
                        sbOutPut.AppendLine(str.Insert(str.IndexOf("ADD  DEFAULT") + 4, "CONSTRAINT " + ids));
                    }
                    else
                        sbOutPut.AppendLine(str);

                    sbOutPut.AppendLine("GO");
                }

                return sbOutPut.ToString();
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogInfo(ex.Message+ex.StackTrace, string.Empty);
                return sbOutPut.ToString();
            }
            finally
            {
                
            }
        }
    }
}
