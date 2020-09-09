using System;
using System.IO;
using System.Text;

namespace FtpUploadTest.Class
{
    /// <summary>
    /// Text File Parsing Class
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// FileName : Parsing 할 파일명
        /// Exception : 마지막으로 발생한 Error 메세지
        /// </summary>
        public static string FileName { get; set; }
        public static string Exception { get { return _exception; } }

        private static string _exception;

        /// <summary>
        /// 파일을 Parsing 하여 찾는 Value를 반환해줌
        /// </summary>
        /// <param name="SectionName">최상위 Section [ ]로 묶여 있음</param>
        /// <param name="KeyName">찾고자 하는 Key 값. = 앞에 있음</param>
        /// <param name="DefaultValue">찾는 값이 없을 경우 반환할 기본 값</param>
        /// <returns></returns>
        public static string GetValueString(string SectionName, string KeyName, string DefaultValue)
        {
            _exception = string.Empty;

            if (FileName == null || FileName.Length == 0) return DefaultValue;

            string value = string.Empty;
            bool IsSection = false;

            try
            {
                StreamReader sr = new StreamReader(FileName);

                string lineString = string.Empty;

                while (lineString != null)
                {
                    lineString = sr.ReadLine();

                    if (lineString == null) break;

                    if (lineString.StartsWith(";")) continue;

                    if (lineString.Contains("[") && lineString.Contains("]"))
                    {
                        IsSection = lineString.Contains(string.Format("[{0}]", SectionName));
                    }

                    if (IsSection && lineString.Contains(KeyName))
                    {
                        value = lineString.Replace(KeyName, "").Replace("=", "").Trim();
                        break;
                    }
                }

                sr.Close();
            }
            catch (Exception ex)
            {
                _exception = ex.Message;

                return DefaultValue;
            }

            if (value == string.Empty) return DefaultValue;

            return value;
        }

        /// <summary>
        /// int 값을 반환해주는 함수
        /// </summary>
        /// <param name="SectionName">최상위 Section [ ]로 묶여 있음</param>
        /// <param name="KeyName">찾고자 하는 Key 값. = 앞에 있음</param>
        /// <param name="DefaultValue">찾는 값이 없을 경우 반환할 기본 값</param>
        /// <returns></returns>
        public static int GetValueInt(string SectionName, string KeyName, int DefaultValue)
        {
            _exception = string.Empty;

            if (FileName.Length == 0) return DefaultValue;

            bool IsSection = false;
            string value = string.Empty;

            try
            {
                StreamReader sr = new StreamReader(FileName);

                string lineString = string.Empty;

                while (lineString != null)
                {
                    lineString = sr.ReadLine();

                    if (lineString == null) break;

                    if (lineString.StartsWith(";")) continue;

                    if (lineString.Contains("[") && lineString.Contains("]"))
                    {
                        IsSection = lineString.Contains(string.Format("[{0}]", SectionName));
                    }

                    if (IsSection && lineString.Contains(KeyName))
                    {
                        value = lineString.Replace(KeyName, "").Replace("=", "").Replace(" ", "");
                        break;
                    }
                }

                sr.Close();

                if (value == string.Empty) return DefaultValue;

                return int.Parse(value);
            }
            catch (Exception ex)
            {
                _exception = ex.Message;

                return DefaultValue;
            }
        }

        /// <summary>
        /// 선택 된 File 에 Value를 변경함
        /// </summary>
        /// <param name="SectionName">변경 하고자 하는 Section</param>
        /// <param name="KeyName">변경 하고자 하는 Key</param>
        /// <param name="ValueString">변경 값</param>
        /// <returns></returns>
        public static bool SetValue(string SectionName, string KeyName, string ValueString)
        {
            _exception = string.Empty;

            if (FileName.Length == 0) return false;

            bool IsSection = false;
            string value = string.Empty;

            try
            {
                StreamReader sr = new StreamReader(FileName);

                StringBuilder strContent = new StringBuilder();

                string lineString = string.Empty;

                while (lineString != null)
                {
                    lineString = sr.ReadLine();

                    if (lineString == null) break;

                    if (lineString.StartsWith(";")) continue;

                    if (lineString.Contains("[") && lineString.Contains("]"))
                    {
                        IsSection = lineString.Contains(string.Format("[{0}]", SectionName));
                    }

                    if (IsSection && lineString.Contains(KeyName))
                    {
                        value = lineString.Replace(KeyName, "").Replace("=", "").Replace(" ", "");

                        strContent.AppendLine(lineString.Replace(value, ValueString));                        
                    }
                    else
                    {
                        strContent.AppendLine(lineString);
                    }                
                }

                sr.Close();

                StreamWriter sw = new StreamWriter(FileName);

                sw.Flush();
                sw.Write(strContent.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                _exception = ex.Message;
                return false;
            }

            return true;
        }
    }
}
