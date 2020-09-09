using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Oracle.DataAccess.Client;
using Winforsys.Util;
using System.Runtime.InteropServices;

namespace Winforsys.DBManagers
{
    public sealed class OracleDBManager : JKEventHandler
    {
        private static OracleDBManager instance;

        public static OracleDBManager Instance
        {
            get
            {
                if (instance == null) instance = new OracleDBManager();

                return instance;
            }
        }

        public event OccureExceptionEventHandler ExceptionEvent;
        public string LastExceptionString { get; set; }

        public string ConnectionString { get; set; }
        public string InitFileName { get; set; }     
        public string Address { get; private set; }
        public string Port { get; private set; }
                
        private OracleCommand LastExecutedCommand = null;
        private int RetryCnt = 0;

        public bool IsRunning 
        {
            get 
            {
                return CheckDBConnected();
            } 
        }

        public OracleConnection Connection { get; private set; }

        public OracleDBManager()
        {
           
        }

        public static OracleDBManager GetNewInstanceConnection()
        {
            if (OracleDBManager.instance == null) return null;

            OracleDBManager dbManager = new OracleDBManager();
            dbManager.ConnectionString = OracleDBManager.Instance.ConnectionString;
            dbManager.GetConnection();
            dbManager.ExceptionEvent = instance.ExceptionEvent;

            return dbManager;
        }

        public bool GetConnection()
        {    
            try
            {
                if (this.Connection != null)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                    this.Connection = null;
                }

                if (InitFileName != null)
                {
                    SetConnectionString();
                }
                
                Connection = new OracleConnection(ConnectionString);  
              
                Connection.Open();
            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

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
                RetryCnt = 0;

                int result = Execute_NonQuery(query);

                return result;
            }
        }

        public bool HasRows(string query)
        {
            lock (this)
            {
                RetryCnt = 0;

                OracleDataReader result = ExecuteReader(query);

                return result.HasRows;
            }
        }

        public OracleDataReader ExecuteReaderQuery(string query)
        {
            lock (this)
            {
                RetryCnt = 0;

                OracleDataReader result = ExecuteReader(query);

                return result;
            }
        }    

        public DataSet ExecuteDsQuery(DataSet ds, string query)
        {
            ds.Reset();

            lock (this)
            {
                RetryCnt = 0;

                return ExecuteDataAdt(ds, query);
            }
        }

        public int ExecuteProcedure(string procName, params string[] pValues)
        {
            lock (this)
            {
                RetryCnt = 0;

                return ExecuteProcedureAdt(procName, pValues);
            }
        }

        public void Close()
        {
            Connection.Close();
        }

        public void QueryCancel()
        {
            if (this.LastExecutedCommand != null)
            {
                this.LastExecutedCommand.Cancel();
            }
        }

        #region private..........................................................

        private void SetConnectionString()
        {
            FileManager.FileName = InitFileName;

            string user = FileManager.GetValueString("DATABASE", "USER", "RTMS_ADM");
            string pwd = FileManager.GetValueString("DATABASE", "PWD", "rtmsadm123");
            string addr01 = FileManager.GetValueString("DATABASE", "R_ADDR01", "127.0.0.1");
            string addr02 = FileManager.GetValueString("DATABASE", "R_ADDR02", "127.0.0.1");

#if DEBUG
            addr01 = FileManager.GetValueString("DATABASE", "D_ADDR01", "127.0.0.1");
            addr02 = FileManager.GetValueString("DATABASE", "D_ADDR02", "127.0.0.1");
#endif

            string port = FileManager.GetValueString("DATABASE", "PORT", "1521");
            string sid = FileManager.GetValueString("DATABASE", "SID", string.Empty);
            string svr = FileManager.GetValueString("DATABASE", "SERVICE_NAME", string.Empty);
            
            string address01 = string.Format("(ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))", addr01, port);
            string address02 = string.Format("(ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))", addr02, port);

            string dataSource = string.Format(@"(DESCRIPTION =(ADDRESS_LIST ={0}{1})(CONNECT_DATA =(", address01, address02);
            
            dataSource += svr == string.Empty ? string.Format("SID = {0})))", sid) : string.Format("SERVICE_NAME = {0})))", svr);

            this.Address = addr01;
            this.Port = port;
            this.ConnectionString = "User Id=" + user + ";Password=" + pwd + ";Data Source=" + dataSource;
        }

        private int Execute_NonQuery(string query)
        {
            int result = 0;

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = this.Connection;
                cmd.CommandText = query;

                LastExecutedCommand = cmd;
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //연결 해제 여부 확인 후 해제 시 재연결 후 다시 시도...
                if (RetryCnt < 1 && CheckDBConnected() == false)
                {
                    RetryCnt++;

                    GetConnection();

                    Exception ex02 = new Exception("Reconnect to database");

                    if (this.ExceptionEvent != null)
                        this.ExceptionEvent(string.Empty, ex02);

                    result = Execute_NonQuery(query);
                    return result;
                }

                //사용자 Cancel
                if (ex.Message.Contains("ORA-01013"))
                {
                    return -1;
                }

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}\n[{2}]", info.ReflectedType.Name, info.Name, query);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                this.LastExceptionString = ex.Message;

                result = -1;
            }

            return result;
        }

        private OracleDataReader ExecuteReader(string query)
        {
            OracleDataReader result = null;

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = this.Connection;
                cmd.CommandText = query;

                LastExecutedCommand = cmd;
                result = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                //연결 해제 여부 확인 후 해제 시 재연결 후 다시 시도...
                if (RetryCnt < 1 && CheckDBConnected() == false)
                {
                    RetryCnt++;

                    GetConnection();

                    Exception ex02 = new Exception("Reconnect to database");

                    if (this.ExceptionEvent != null)
                        this.ExceptionEvent(string.Empty, ex02);

                    result = ExecuteReader(query);
                    return result;
                }

                //사용자 Cancel
                if (ex.Message.Contains("ORA-01013"))
                {
                    return null;
                }

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}\n[{2}]", info.ReflectedType.Name, info.Name, query);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                this.LastExceptionString = ex.Message;

                return null;
            }

            return result;
        }

        private DataSet ExecuteDataAdt(DataSet ds, string query)
        {
            try
            {
                OracleDataAdapter cmd = new OracleDataAdapter();
                cmd.SelectCommand = new OracleCommand();
                cmd.SelectCommand.Connection = this.Connection;
                cmd.SelectCommand.CommandText = query;

                LastExecutedCommand = cmd.SelectCommand;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                //연결 해제 여부 확인 후 해제 시 재연결 후 다시 시도...
                if (RetryCnt < 1 && CheckDBConnected() == false)
                {
                    RetryCnt++;

                    GetConnection();

                    Exception ex02 = new Exception("Reconnect to database");

                    if (this.ExceptionEvent != null)
                        this.ExceptionEvent(string.Empty, ex02);

                    ds = ExecuteDataAdt(ds, query);
                    return ds;
                }

                //사용자 Cancel
                if (ex.Message.Contains("ORA-01013"))
                {
                    return null;
                }

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}\n[{2}]", info.ReflectedType.Name, info.Name, query);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                this.LastExceptionString = ex.Message;

                return null;
            }

            return ds;
        }

        private int ExecuteProcedureAdt(string procName, params string[] pValues)
        {
            int result = -1;

            try
            {
                OracleCommand cmd = new OracleCommand(procName, this.Connection);
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < pValues.Length; ++i)
                {
                    string par = string.Format("PARAM{0}", i + 1);

                    cmd.Parameters.Add(par, OracleDbType.Varchar2).Value = pValues[i];
                }

                cmd.Parameters.Add("execResult", OracleDbType.Int32);
                cmd.Parameters["execResult"].Direction = ParameterDirection.Output;

                LastExecutedCommand = cmd;
                cmd.ExecuteNonQuery();

                result = int.Parse(cmd.Parameters["execResult"].Value.ToString());
            }
            catch (Exception ex)
            {
                //연결 해제 여부 확인 후 해제 시 재연결 후 다시 시도...
                if (RetryCnt < 1 && CheckDBConnected() == false)
                {
                    RetryCnt++;

                    GetConnection();

                    Exception ex02 = new Exception("Reconnect to database");

                    if (this.ExceptionEvent != null)
                        this.ExceptionEvent(string.Empty, ex02);

                    result = ExecuteProcedureAdt(procName, pValues);
                    return result;
                }

                //사용자 Cancel
                if (ex.Message.Contains("ORA-01013"))
                {
                    return result;
                }

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}\n[{2}]", info.ReflectedType.Name, info.Name, procName);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                this.LastExceptionString = ex.Message;
            }

            return result;
        }

        private bool CheckDBConnected()
        {
            string query = "SELECT 1 FROM DUAL";
            OracleDataReader result = null;

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = this.Connection;
                cmd.CommandText = query;
                result = cmd.ExecuteReader();
            }
            catch { }

            if (result != null && result.HasRows)
                return true;

            return false;
        }

        #endregion private..................................................................
    }
}
