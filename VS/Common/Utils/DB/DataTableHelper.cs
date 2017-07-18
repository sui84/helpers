using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

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
    }
}
