using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace ConvertCap
{
    public class DbHelper
    {
        public DataSet GetDataFromCommand(string cmdText, string connectionString, int? timeout = null)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connectionString.Contains("Provider="))
                    connectionString = string.Format("Provider=SQLOLEDB.1;{0}", connectionString).Replace("Integrated Security=True", "Integrated Security=SSPI");
                if (timeout.HasValue)
                    connectionString = string.Format("Connect Timeout=0;{0}", connectionString);
                connDbConnection.ConnectionString = connectionString;
                connDbConnection.Open();
                OleDbCommand objCommand = new OleDbCommand();
                objCommand.CommandText = cmdText;
                objCommand.Connection = connDbConnection;
                DataSet ds = new DataSet();
                OleDbDataAdapter objAdaptor = new OleDbDataAdapter();
                objAdaptor.SelectCommand = objCommand;
                objAdaptor.Fill(ds);
                connDbConnection.Close();
                connDbConnection.Dispose();
                return ds;
            }
            return null;
        }


        public void ExecuteCommand(string cmdText, string connectionString)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connectionString.Contains("Provider="))
                {
                    connectionString = string.Format("Provider=SQLOLEDB.1;{0}", connectionString).Replace("Integrated Security=True", "Integrated Security=SSPI");
                }
                connDbConnection.ConnectionString = connectionString;
                connDbConnection.Open();
                OleDbCommand objCommand = new OleDbCommand();
                objCommand.CommandText = cmdText;
                objCommand.Connection = connDbConnection;
                objCommand.ExecuteNonQuery();
                connDbConnection.Close();
                connDbConnection.Dispose();
            }
        }

        public void ExecuteStoreProcedure(string spName, OleDbParameter[] paras, string connectionString)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connectionString.Contains("Provider="))
                    connectionString = string.Format("Provider=SQLOLEDB.1;{0}", connectionString).Replace("Integrated Security=True", "Integrated Security=SSPI");
                connDbConnection.ConnectionString = connectionString;
                connDbConnection.Open();
                OleDbCommand objCommand = new OleDbCommand();
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = spName;
                objCommand.Parameters.AddRange(paras);
                objCommand.Connection = connDbConnection;
                objCommand.ExecuteNonQuery();
                connDbConnection.Close();
                connDbConnection.Dispose();
            }
        }
        public void ClearTable(string tbName, string connectionString)
        {
            ExecuteCommand(string.Format("truncate table {0}", tbName), connectionString);
        }

        public void DeleteTable(string tbName , string conStr, string connectionString)
        {
            ExecuteCommand(string.Format("delete from {0} {1}", tbName, conStr), connectionString);
        }

        public void CopyData(DataTable SourceData, string tbName, string connectionString, string[][] mapCols = null)
        {
            using (System.Data.SqlClient.SqlBulkCopy bcp = new System.Data.SqlClient.SqlBulkCopy(connectionString))
            {
                bcp.DestinationTableName = tbName;
                if (mapCols != null)
                {
                    for (int i = 0; i < mapCols[0].Count(); i++)
                    {
                        bcp.ColumnMappings.Add(mapCols[0][i], mapCols[1][i]);
                    }
                }
                bcp.WriteToServer(SourceData);
            }
        }

    }
}
