using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Winforsys.Util
{
    public enum LogType { Inform, Query, Alarm, ReceiveMsg, SendMsg, Warning, Error, Critical};
    public delegate void LogMessageReceivedEventHandler(LogType type, string msg);

    /// <summary>
    /// Log를 기록해주는 함수    /// 
    /// </summary>
    public sealed class LogManager
    {
        /// <summary>
        /// Instance : 싱글톤 구조를 위한 전역 객체        /// 
        /// </summary>
        private static LogManager instance = null;

        public static LogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    string path = GetLogPath();
                    instance = new LogManager(path);
                }

                return instance;
            }
        }

        /// <summary>
        /// Log를 기록하는 함수
        /// </summary>
        /// <param name="type">로그 타입</param>
        /// <param name="format">string format</param>
        /// <param name="arg">Log Messge 항목</param>
        public void WriteLog(LogType type, string format, params object[] arg)
        {
            string message = String.Format(format, arg);

            this.Write(string.Format("{0} >>\r\n{1}\r\n", type.ToString(), message));
        }

        /// <summary>
        /// fileDir : 로그 파일이 기록되는 Directory
        /// fileName : 로그 파일 명
        /// fileSize : 파일의 최대 Size
        /// fileLife : 파일을 삭제하는 주기 (일 단위)
        /// </summary>
        public string fileDir { get; set; }
        public string fileName { get; set; }
        public int fileSize { get; set; }
        public int fileLife { get; set; }

        private FileStream fsCurrentFile = null;

        public event LogMessageReceivedEventHandler LogMessageReceiveEvnt;

        /// <summary>
        /// Log 객체 생성
        /// </summary>
        /// <param name="path">파일을 생성하고자 하는 Directory</param>
        public LogManager(string path)
        {
            this.fileDir = path;
            this.fileSize = 1024;
            this.fileLife = 30;
            this.fileName = null;

            //Write(string.Format("###### {0} 경로에 로그 기록을 시작힙니다. ######", path));
        }

        /// <summary>
        /// Log 객체 소멸자
        /// </summary>
        ~LogManager()
        {
            if (this.fsCurrentFile != null)
                this.fsCurrentFile.Close();
        }

        /// <summary>
        /// Config.ini 파일이 있을 경우 Config.ini 파일에서 로그 파일 경로를 가져옴
        /// 파일이나 경로가 존재하지 않을 경우 실행 폴더 하위에 생성 됨
        /// </summary>
        /// <returns></returns>
        private static string GetLogPath()
        {
            FileManager.FileName = @"Config.ini";
            string path = FileManager.GetValueString("LOG", "PATH", Environment.CurrentDirectory + @"\LogFiles");

            return path;
        }

        /// <summary>
        /// 파일에 실제 로그를 기록함
        /// </summary>
        /// <param name="msg">기록하려는 Message</param>
        private void Write(string msg)
        {
            lock (this)
            {
                CheckDirectory();

                FileStream fs = GetCurrentLogFile(msg.Length);

                DateTime now = DateTime.Now;

                string date = String.Format("{0:D4}-{1:D2}-{2:D2}", now.Year, now.Month, now.Day);
                string time = String.Format("{0:D2}:{1:D2}:{2:D2}", now.Hour, now.Minute, now.Second);

                string format = String.Format("{0} {1} -> {2}\r\n", date, time, msg);

                byte[] msgByte = System.Text.Encoding.Default.GetBytes(format);

                fs.Write(msgByte, 0, msgByte.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// Directory가 존재하는지 여부를 확인 함.
        /// </summary>
        private void CheckDirectory()
        {
            if (!Directory.Exists(this.fileDir))
            {
                Directory.CreateDirectory(this.fileDir);
            }
        }

        /// <summary>
        /// 새로운 로그 파일을 생성 함.
        /// </summary>
        /// <returns></returns>
        private FileStream CreateNewLogFile()
        {
            ClearOldLogFiles();

            DateTime now = DateTime.Now;

            string date = String.Format("{0:D4}{1:D2}{2:D2}", now.Year, now.Month, now.Day);
            string time = String.Format("{0:D2}{1:D2}{2:D2}", now.Hour, now.Minute, now.Second);

            string filename = date + "-" + time + ".txt";

            FileStream fsLogFile = null;

            try
            {
                fsLogFile = File.Open(this.fileDir + "\\" + filename, FileMode.Append, FileAccess.Write, FileShare.Read);
            }
            catch (System.Exception ex)
            {
                if (this.LogMessageReceiveEvnt != null)
                    this.LogMessageReceiveEvnt(LogType.Error, String.Format("Open Log File({0}) Failed.! [{1}]", filename, ex.Message));
            }

            return fsLogFile;
        }

        /// <summary>
        /// 현재 기록하고 있는 로그 파일의 fileStream을 가져옴
        /// </summary>
        /// <param name="nextMsgLen"></param>
        /// <returns></returns>
        private FileStream GetCurrentLogFile(int nextMsgLen)
        {
            if (this.fsCurrentFile == null)
                this.fsCurrentFile = CreateNewLogFile();

            if (this.fsCurrentFile.Length + nextMsgLen >= this.fileSize * 1024)
            {
                this.fsCurrentFile.Close();
                this.fsCurrentFile = CreateNewLogFile();
            }

            return this.fsCurrentFile;
        }

        /// <summary>
        /// 생명 주기가 끝난 로그 파일을 삭제함.
        /// </summary>
        private void ClearOldLogFiles()
        {
            DirectoryInfo di = new DirectoryInfo(this.fileDir);

            foreach (FileInfo fi in di.GetFiles())
            {
                if (Path.GetExtension(fi.Name) != ".txt") continue;

                if (fi.CreationTime.AddDays(this.fileLife) < DateTime.Now)
                    fi.Delete();
            }
        }
    }
}
