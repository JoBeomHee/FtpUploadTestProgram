using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Data.SqlClient;
using System.Data.OleDb;
using Winforsys.Util;
using System.Runtime.InteropServices;

namespace Winforsys.DBManagers
{
    public sealed class SqlDBManager
    {
        public string ConnectionString { get; set; }
        public string InitFileName { get; set; }

        public string Address { get; private set; }
        public string Port { get; private set; }
        public string LastException { get; private set; }

        public bool IsRunning { get { return Connection.State == ConnectionState.Open ? true : false; } }

        public SqlConnection Connection { get; private set; }

        private LogManager dbLoger = new LogManager(@"C:\ScribeRTMS\DB_LogFiles");
        private static SqlDBManager instance;

        public static SqlDBManager Instance
        {
            get
            {
                if (instance == null) instance = new SqlDBManager();

                return instance;
            }
        }

        private SqlCommand _sqlCmd = null;

        public SqlDBManager()
        {
            _sqlCmd = new SqlCommand();
        }

        public bool GetConnection()
        {
            try
            {
                if (InitFileName != null)
                {
                    SetConnectionString();
                }

                Connection = new SqlConnection(ConnectionString);

                Connection.Open();

                dbLoger.WriteLog(LogType.Inform, string.Format("##### Database 와 연결 되었습니다. #####"));
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                LastException = ex.Message;

                return false;
            }

            if (Connection.State == ConnectionState.Open)
                return true;
            else
                return false;
        }

        public int ExecuteNonQuery(string query)
        {
            lock (this)
            {
                //if(query.Contains("MERGE INTO t_alarm_merge") == false)
                //    dbLoger.WriteLog(LogType.Inform, string.Format("ExcueteNonQuery - {0}", query));

                return Execute_NonQuery(query);
            }
        }

        public bool HasRows(string query)
        {
            lock (this)
            {
                SqlDataReader result = ExecuteReader(query);

                return result.HasRows;
            }
        }

        public SqlDataReader ExecuteReaderQuery(string query)
        {
            lock (this)
            {
                SqlDataReader result = ExecuteReader(query);

                return result;
            }
        }

        public DataSet ExecuteDsQuery(DataSet ds, string query)
        {
            ds.Reset();

            lock (this)
            {
                //dbLoger.WriteLog(LogType.Inform, string.Format("ExecuteDsQuery - {0}", query));

                return ExecuteDataAdt(ds, query);
            }
        }

        public DataSet ExecuteProcedure(DataSet ds, string procName, params string[] pValues)
        {
            lock (this)
            {
                return ExecuteProcedureAdt(ds, procName, pValues);
            }
        }

        public void CancelQuery()
        {
            _sqlCmd.Cancel();
        }

        public void Close()
        {
            Connection.Close();
        }

        #region private..........................................................

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        private bool CheckConnection()
        {
            bool result = true;           
            
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == false)
            {
                this.LastException = "네트워크 연결이 끊어졌습니다.";
                System.Windows.Forms.MessageBox.Show(this.LastException, "Error");
                result = false;
            }
            else if (this.Connection == null || this.Connection.State == ConnectionState.Closed)
            {
                result = this.GetConnection();
            }

            return result;
        }

        private void SetConnectionString()
        {
            FileManager.FileName = InitFileName;

            string user = FileManager.GetValueString("DATABASE", "USER", "RTMS_ADM");
            string pwd = FileManager.GetValueString("DATABASE", "PWD", "rtmsadm123");
            string addr = FileManager.GetValueString("DATABASE", "R_ADDR", "127.0.0.1");
            string db = FileManager.GetValueString("DATABASE", "DB", "dnlsvh");
            
            string dataSource = string.Format(@"Data Source={0};Database={1};User Id={2};Password={3}", addr, db, user, pwd);

            this.Address = addr;
            this.ConnectionString = dataSource;
        }

        private int Execute_NonQuery(string query)
        {
            int result = (int)ExcuteResult.Fail;

            try
            {
                _sqlCmd = new SqlCommand();
                _sqlCmd.Connection = this.Connection;
                _sqlCmd.CommandText = query;
                result = _sqlCmd.ExecuteNonQuery();                
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                LastException = ex.Message;

                if (CheckConnection() == false) return result;
            }

            return result;
        }

        private SqlDataReader ExecuteReader(string query)
        {
            SqlDataReader result = null;

            try
            {
                _sqlCmd = new SqlCommand();
                _sqlCmd.Connection = this.Connection;
                _sqlCmd.CommandText = query;
                result = _sqlCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                LastException = ex.Message;

                if (CheckConnection() == false) return result;
            }

            return result;
        }

        private DataSet ExecuteDataAdt(DataSet ds, string query)
        {
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter();
                cmd.SelectCommand = _sqlCmd;
                cmd.SelectCommand.Connection = this.Connection;
                cmd.SelectCommand.CommandText = query;
                cmd.Fill(ds);  
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                LastException = ex.Message;

                if (CheckConnection() == false) return null;
            }

            return ds;
        }

        private DataSet ExecuteProcedureAdt(DataSet ds, string query, params string[] values)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = _sqlCmd;
                adapter.SelectCommand.CommandText = query;
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Connection = this.Connection;

                for (int i = 0; i < values.Length; ++i)
                {
                    adapter.SelectCommand.Parameters.Add(values[i]);
                    //adapter.SelectCommand.Parameters.Add("params", values[i]);
                }

                adapter.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                this.LastException = ex.Message;

                if (CheckConnection() == false) return null;
            }

            return ds;
        }

        #endregion private..................................................................
    }
}
