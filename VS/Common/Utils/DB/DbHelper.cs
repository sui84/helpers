using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace Common.Utils.DB
{
    public class DbHelper
    {
        public DataSet GetDataFromCommand(string cmdText, string connStr, int? timeout = null)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connStr.Contains("Provider="))
                    connStr = string.Format("Provider=SQLOLEDB.1;{0}", connStr).Replace("Integrated Security=True", "Integrated Security=SSPI");
                if (timeout.HasValue)
                    connStr = string.Format("Connect Timeout=0;{0}", connStr);
                connDbConnection.ConnectionString = connStr;
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

        public DataSet GetDataFromCommandByOdbc(string cmdText, string odbcStr)
        {
            using (OdbcConnection connDbConnection = new OdbcConnection())
            {
                connDbConnection.ConnectionString = odbcStr;
                connDbConnection.Open();
                OdbcCommand objCommand = new OdbcCommand();
                objCommand.CommandText = cmdText;
                objCommand.Connection = connDbConnection;
                DataSet ds = new DataSet();
                OdbcDataAdapter objAdaptor = new OdbcDataAdapter();
                objAdaptor.SelectCommand = objCommand;
                objAdaptor.Fill(ds);
                connDbConnection.Close();
                connDbConnection.Dispose();
                return ds;
            }
            return null;
        }

        public DataSet GetDataFromParasByOdbc(string cmdText,string value, OleDbParameter paras, string connStr)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connStr.Contains("Provider="))
                    connStr = string.Format("Provider=SQLOLEDB.1;{0}", connStr).Replace("Integrated Security=True", "Integrated Security=SSPI");
                //connDbConnection.ConnectionString = connStr;
                //connDbConnection.Open();
                DataSet ds = new DataSet();
                // cmdText = "select * from testtb where testname like ?"
                OleDbDataAdapter objAdaptor = new OleDbDataAdapter(cmdText, connStr);
                objAdaptor.SelectCommand.Parameters.Add("@testname","%"+value+"%");
                objAdaptor.Fill(ds);
                //connDbConnection.Close();
                //connDbConnection.Dispose();
                return ds;
            }
            return null;
        }


        public DataSet GetDataFromCommand(string[] cmdTexts, string connStr)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connStr.Contains("Provider="))
                    connStr = string.Format("Provider=SQLOLEDB.1;{0}", connStr).Replace("Integrated Security=True", "Integrated Security=SSPI");
                connDbConnection.ConnectionString = connStr;
                connDbConnection.Open();
                OleDbCommand objCommand = new OleDbCommand();
                
                objCommand.Connection = connDbConnection;
                DataSet ds = new DataSet();
                OleDbDataAdapter objAdaptor = new OleDbDataAdapter();
                for (int i = 0; i < cmdTexts.Length; i++)
                {
                    ds.Tables.Add();
                    objCommand.CommandText = cmdTexts[i];
                    objAdaptor.SelectCommand = objCommand;
                    objAdaptor.Fill(ds.Tables[i]);
                }
                connDbConnection.Close();
                connDbConnection.Dispose();
                return ds;
            }
            return null;
        }


        public void ExecuteCommand(string cmdText, string connStr)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connStr.Contains("Provider="))
                {
                    connStr = string.Format("Provider=SQLOLEDB.1;{0}", connStr).Replace("Integrated Security=True", "Integrated Security=SSPI");
                }
                connDbConnection.ConnectionString = connStr;
                connDbConnection.Open();
                OleDbCommand objCommand = new OleDbCommand();
                objCommand.CommandText = cmdText;
                objCommand.Connection = connDbConnection;
                objCommand.ExecuteNonQuery();
                connDbConnection.Close();
                connDbConnection.Dispose();
            }
        }

        public void ExecuteCommandWithTran(string cmdText, string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                //这样的写法不对
                // ExecuteNonQuery requires the command to have a transaction when the connection assigned to the command is in a pending local transaction.  The Transaction property of the command has not been initialized.
                // using (SqlTransaction tx = destinationConnection.BeginTransaction(IsolationLevel.RepeatableRead)){
                // SqlCommand sqlCmd = new SqlCommand(cmdText, conn ) }
                using (SqlTransaction tx = conn.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    try
                    {
                        SqlCommand sqlCmd = new SqlCommand(cmdText, conn, tx);
                        sqlCmd.ExecuteNonQuery();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                    }
                    finally
                    {
                        tx.Dispose();
                    }
                 }
                conn.Close();
                conn.Dispose();
            }
        }

        public void ExecuteStoreProcedure(string spName, OleDbParameter[] paras, string connStr)
        {
            using (OleDbConnection connDbConnection = new OleDbConnection())
            {
                if (!connStr.Contains("Provider="))
                    connStr = string.Format("Provider=SQLOLEDB.1;{0}", connStr).Replace("Integrated Security=True", "Integrated Security=SSPI");
                connDbConnection.ConnectionString = connStr;
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
        
        public void ClearTable(string tbName, string connStr)
        {
            ExecuteCommand(string.Format("truncate table {0}", tbName), connStr);
        }

        public void DeleteTable(string tbName , string conStr, string connStr)
        {
            ExecuteCommand(string.Format("delete from {0} {1}", tbName, conStr), connStr);
        }
        
                public string CompanyTable(DataTable dt1, DataTable dt2)
        {
            string str = string.Empty;
            for( int i =0;i< dt1.Rows.Count ;i++){
                foreach (DataColumn dcol in dt1.Columns)
                {
                    string col = dcol.ColumnName;
                    if (dt2.Rows.Count <= i){
                        str += string.Format("Row {0} Column {1} : {2} NULL \r\n",i, col,dt1.Rows[i][col]);
                    }
                    else{
                        if (dt1.Rows[i][col].ToString() != dt2.Rows[i][col].ToString()){
                            str += string.Format("Row {0} Column {1} : {2} {3} \r\n", i, col,dt1.Rows[i][col], dt2.Rows[i][col]);
                        }
                    }
                }
            }
            if (dt2.Rows.Count > dt1.Rows.Count)
            {
                for (int i = dt1.Rows.Count ; i < dt2.Rows.Count; i++)
                {
                    foreach (DataColumn col in dt2.Columns)
                    {
                        str += string.Format("Row {0} Column {1} : NULL {2} \r\n", i, col,dt2.Rows[i][col]);
                    }
                }
            }
            return str;
        }

        public void CopyData(DataTable SourceData, string tbName, string connStr, string[][] mapCols = null)
        {
            using (System.Data.SqlClient.SqlBulkCopy bcp = new System.Data.SqlClient.SqlBulkCopy(connStr))
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

        public void CopyDataWithTran(DataTable SourceData, string tbName, string connStr, string[][] mapCols = null)
        {
            //SqlBulkCopy - 这样的写法会报错 Unexpected existing transaction
            // using (SqlTransaction tx = destinationConnection.BeginTransaction(IsolationLevel.RepeatableRead)){}

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    using (System.Data.SqlClient.SqlBulkCopy bcp = new System.Data.SqlClient.SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tx))
                    {
                        try
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
                        catch (Exception ex)
                        {
                            tx.Rollback();
                        }
                        finally
                        {
                            tx.Dispose();
                        }
                    }
                }
            }
        }

        public DataTable AddColumn(DataTable dt, string colName, string defaultValue)
        {
            System.Data.DataColumn col = new System.Data.DataColumn(colName, typeof(System.String));
            col.DefaultValue = defaultValue;
            dt.Columns.Add(col);
            return dt;
        }

        public DataTable AddRow(DataTable dt, DataRow dr)
        {
            DataTable dt2 = dt.Clone();
            dt.TableName = "NewDataTable";
            dt.Rows.Add(dr.ItemArray);
            return dt2;
        }
    }
}

