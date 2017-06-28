using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public class ConnStrHelper
    {
        #region Oracle
        public string GetOracleClientConnStr(string serverName,int port,string sid,string username,string password)
        {

            string oraStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SID={2})));User Id={3};Password={4};"
                , serverName, port, sid, username, password);
            //using Oracle.DataAccess.Client;
            //OracleConnection conn = new OracleConnection(oraStr);
            return oraStr;
        }

        #endregion

        #region SQL Server
        public string GetMSSQLClientConnStr(string serverName, string dbName)
        {
            //using System.Data.Linq.SqlClient;
            string connStr = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", serverName, dbName);
            return connStr;      
        }

        public string GetMSSQLClientConnStr(string serverName, string dbName, string username, string password)
        {
            //System.Data.SqlClient.SqlBulkCopy
            //using System.Data.Linq.SqlClient;
            string connStr = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", serverName, dbName, username, password);
            return connStr;      
        }

        public string GetMSSQLOledbConnStr(string serverName, string dbName)
        {
            //using System.Data.OleDb;
            string connStr = string.Format("Provider=SQLOLEDB.1;;Data Source={0};Initial Catalog={1};Integrated Security=SSPI", serverName, dbName);
            return connStr;
        }

        public string GetMSSQLOledbConnStr(string serverName, string dbName, string username, string password)
        {
            //using System.Data.OleDb;
            string connStr = string.Format("Provider=SQLOLEDB.1;Data Source={0};Initial Catalog={1};User ID={2};Password={3};", serverName, dbName, username, password);
            return connStr;
        }

        public string GetMSSQLJdbcConnStr(string serverName, string dbName, string username, string password)
        {
            string connStr = string.Format("jdbc:sqlserver://{0};databaseName={1};user={2};password={3};", serverName, dbName, username, password);
            return connStr;
        }
        #endregion

        #region AS400
        public string GetAs400OledbConnStr(string serverName, string dbName, string username, string password)
        {
            //using System.Data.OleDb;
            string connStr = string.Format(@"Provider=IBMDA400.DataSource.1;Persist Security Info=False;Data Source={0};User ID={1};Password={2};
            Protection Level=None;Transport Product=Client Access;SSL=DEFAULT;Force Translate=65535;Connect Timeout=3;Convert Date Time To Char=TRUE;Cursor Sensitivity=3", serverName, username, password);
            return connStr;
        }

        public string GetAs400OdbcConnStr(string name,  string username, string password)
        {
            //using System.Data.Odbc;
            string connStr = string.Format(@"Driver={iSeries Access ODBC Driver};System={0};Uid={1};Pwd={2};", name, username, password);
            return connStr;
        }
        #endregion
    }
}
