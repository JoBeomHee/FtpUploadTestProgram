using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Winforsys.Util
{
    public class REGUtil
    {
        public static bool WriteValue(string sKey, string sName, string sObjValue)
        {
            bool result = true;

            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE", true).CreateSubKey(sKey);

                rk.SetValue(sName, sObjValue);

                rk.Close();
            }
            catch(Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                result = false;
            }

            return result;
        }

        public static string ReadValue(string sKey, string sName)
        {
            string result = string.Empty;

            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE", true).CreateSubKey(sKey);

                result = rk.GetValue(sName).ToString();

                rk.Close();
            }
            catch(Exception ex)
            {
                string msg = string.Format("{0}\r\nMessage : {1}", ex.StackTrace, ex.Message);
                LogManager.Instance.WriteLog(LogType.Error, msg);

                result = null;
            }

            return result;
        }
    }
}
