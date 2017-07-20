using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;

namespace Common.Utils
{
    public static class DataTableHelper
    {
        // Helper function for ADO.Net Bulkcopy to transfer a IEnumerable list to a datatable
        // Adapted from: http://msdn.microsoft.com/en-us/library/bb396189.aspx
        public static DataTable CopyToDataTable<T>(this IEnumerable<T> source)
        {
            return new DataTableCreator<T>().CreateDataTable(source, null, null);
        }

        public static DataTable CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption? options)
        {
            return new DataTableCreator<T>().CreateDataTable(source, table, options);
        }

        public static void BulkCopyToDatabase<T>(this IEnumerable<T> source, string tableName, System.Data.Linq.DataContext dataContext) where T : class
        {
            BulkCopyToDatabase(source, tableName, dataContext.Connection.ConnectionString, dataContext.CommandTimeout);
        }
        public static void BulkCopyToDatabase<T>(this IEnumerable<T> source, string tableName, string connectionString, int commandTimeout, SqlTransaction tx = null) where T : class
        {
            using (var dataTable = DataTableHelper.CopyToDataTable(source))
            {
                SqlConnection conn = null;
                bool isNewConn = false;
                try
                {
                    if (tx != null)
                        conn = tx.Connection;
                    else
                    {
                        conn = new SqlConnection(connectionString);
                        conn.Open();
                        isNewConn = true;
                    }
                    using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls, tx))
                    {
                        foreach (DataColumn dc in dataTable.Columns)
                            bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dc.ColumnName, dc.ColumnName));

                        //  We could use "dataTable.TableName" in the following line, but this does sometimes have problems, as 
                        //  LINQ-to-SQL will drop trailing "s" off table names, so try to insert into [Product], rather than [Products]
                        bulkCopy.BulkCopyTimeout = commandTimeout;
                        bulkCopy.DestinationTableName = "[" + tableName + "]";
                        bulkCopy.WriteToServer(dataTable);
                    }
                }
                finally
                {
                    if (isNewConn)
                        conn.Close();
                }
            }
        }

        #region IList如何转成List<T>
        /// <summary>
        /// IList如何转成List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> IListToList<T>(IList list)
        {
            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return new List<T>(array);
        }
        #endregion

        #region DataTable根据条件过滤表的内容
        /// <summary>
        /// 根据条件过滤表的内容
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static DataTable GetNewDataTable(DataTable dt, string condition)
        {
            if (!IsExistRows(dt))
            {
                if (condition.Trim() == "")
                {
                    return dt;
                }
                else
                {
                    DataTable newdt = new DataTable();
                    newdt = dt.Clone();
                    DataRow[] dr = dt.Select(condition);
                    for (int i = 0; i < dr.Length; i++)
                    {
                        newdt.ImportRow((DataRow)dr[i]);
                    }
                    return newdt;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 检查DataTable 是否有数据行
        /// <summary>
        /// 检查DataTable 是否有数据行
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool IsExistRows(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
                return false;

            return true;
        }
        #endregion

        #region DataTable 转 DataTableToHashtable
        /// <summary>
        /// DataTable 转 DataTableToHashtable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Hashtable DataTableToHashtable(DataTable dt)
        {
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string key = dt.Columns[i].ColumnName;
                    ht[key] = dr[key];
                }
            }
            return ht;
        }
        #endregion

        #region List转换DataTable
        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }
        #endregion

        #region DataTable/DataSet 转 XML
        /// <summary>
        /// DataTable 转 XML
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToXML(DataTable dt)
        {
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    dt.WriteXml(writer);
                    return writer.ToString();
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// DataSet 转 XML
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToXML(DataSet ds)
        {
            if (ds != null)
            {
                System.IO.StringWriter writer = new System.IO.StringWriter();
                ds.WriteXml(writer);
                return writer.ToString();
            }
            return String.Empty;
        }
        #endregion

        //其中   T t = default(T); //就是返回T的默认值。比如说T的类型是int类型的，那么这个default(T)的值就是0的；如果是string类型的话，这个返回值就是“”空字符串的。
        public static List<T> GetList<T>(DataTable table)
        {
            List<T> list = new List<T>(); //里氏替换原则
            T t = default(T);
            PropertyInfo[] propertypes = null;
            string tempName = string.Empty;
            foreach (DataRow row in table.Rows)
            {
                t = Activator.CreateInstance<T>(); ////创建指定类型的实例

                propertypes = t.GetType().GetProperties(); //得到类的属性
                foreach (PropertyInfo pro in propertypes)
                {
                    tempName = pro.Name;
                    if (table.Columns.Contains(tempName.ToUpper()))
                    {
                        object value = row[tempName];
                        if (value is System.DBNull)
                        {
                            value = "";
                        }
                        pro.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }
            return list;
        } 

    }
}
