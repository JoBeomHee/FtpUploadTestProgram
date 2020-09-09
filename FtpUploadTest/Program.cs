using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Threading;
using System.IO;

using Winforsys.Util;


namespace XML_Parser
{
    class Program
    {
        public FTPManager Manager = new FTPManager();

        static void Main(string[] args)
        {
            Program pro = new Program();

            pro.GetFtpPath(pro);
            pro.FtpFileUploadDelete(pro);
        }

        private void FtpFileUploadDelete(Program pro)
        {
            while (true)
            {
                Upload_Xml_File(pro, "test");

                System.Threading.Thread.Sleep(4000);

            }
        }

        private static bool Upload_Xml_File(Program pro , string fileName)
        {
            bool result = false;

            string dataPath = "\\TEST\\";
            string localPath = @"D:\TMS\TEST\test.xml";

            //XMl File Uolad
            if (pro.Manager.UpLoad(dataPath, localPath) == false)
            {
                //Program.ShowLog(LogType.Error, string.Format("XML File Upload Fail.\n[Local PATH : {0}]\n[Server PATH : {1}{2}]", localPath, dataPath, fileName));
            }
            else
            {
                //Program.ShowLog(LogType.Inform, string.Format("XML File Upload Complete.\n[Local PATH : {0}]\n[Server PATH : {1}{2}]", localPath, dataPath, fileName));
                result = true;
            }

            System.Threading.Thread.Sleep(4000);

            //XMl File Delete
            if (pro.Manager.Delete(dataPath, localPath) == false)
            {
                //Program.ShowLog(LogType.Error, string.Format("XML File Delete Fail.\n[Local PATH : {0}]\n[Server PATH : {1}{2}]", localPath, dataPath, fileName));
            }
            else
            {
                //Program.ShowLog(LogType.Inform, string.Format("XML File Delete Complete.\n[Local PATH : {0}]\n[Server PATH : {1}{2}]", localPath, dataPath, fileName));
                result = true;
            }

            return result;
        }

        /// <summary>
        /// FTP 접속 정보 설정 메서드
        /// </summary>
        /// <param name="pro"></param>
        private void GetFtpPath(Program pro)
        {

            string addr = string.Empty;
            string port = string.Empty;
            string userId = string.Empty;
            string pw = string.Empty;

            addr = FileManager.GetValueString("FTP", "ADDR", "127.0.0.1");
            port = FileManager.GetValueString("FTP", "PORT", "21");
            userId = FileManager.GetValueString("FTP", "USER", "win");
            pw = FileManager.GetValueString("FTP", "PWD", "dnlsvh");

            pro.Manager.ipAddr = addr;
            pro.Manager.port = port;
            pro.Manager.userId = userId;
            pro.Manager.pwd = pw;
        }
    }
}
