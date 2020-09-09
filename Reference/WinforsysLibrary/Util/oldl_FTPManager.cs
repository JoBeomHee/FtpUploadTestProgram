using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Winforsys.Util
{
    public delegate void FTPDownloadTotalSizeHandle(long totalSize);
    public delegate void FTPDownloadReceivedSizeHandle(int RcvSize);

    public class FTPManager : JKEventHandler
    {
        private static FTPManager instance;

        public static FTPManager Instance
        {
            get
            {
                if (instance == null) instance = new FTPManager();

                return instance;
            }
        }

        public event FTPDownloadTotalSizeHandle ftpDNTotalSizeEvt;
        public event FTPDownloadReceivedSizeHandle ftpDNRcvSizeEvt;

        string ftpServerIP = null;
        string ftpUserID = null;
        string ftpPassword = null;
        string ftpPort = null;
        bool usePassive = false;

        public event OccureExceptionEventHandler ExceptionEvent;

        public FTPManager()
        {

        }

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="ip">FTP 서버주소</param>
        /// <param name="id">아이디</param>
        /// <param name="pw">패스워드</param>
        /// <param name="port">포트</param>
        public FTPManager(string ip, string port, string id, string pw)
        {
            ftpServerIP = ip; 
            ftpUserID = id;   
            ftpPassword = pw; 
            ftpPort = port;        
            usePassive = true;   //패시브모드 사용여부
        }

        public bool ConnectToServer(string ip, string port, string id, string pw)
        {
            this.ftpServerIP = ip;
            this.ftpUserID = id;
            this.ftpPassword = pw;
            this.ftpPort = port;
            usePassive = true;   //패시브모드 사용여부

            string uri = "ftp://" + ftpServerIP + ":" + ftpPort + "/";
            
            bool result = CheckFTP(uri, id, pw);

            return result;
        }

        /// <summary>
        /// Connection Check
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckFTP(string url, string user, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, password);

                using (request.GetResponse())
                {

                }
            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// File 업로드
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Boolean UpLoad(string folder,string filename)
        {
            MakeDir(@"/dfs/aaa/bbb/");

            // 디렉토리가 없으면 만들기
            if (FtpDirectoryExists(@"\AAA") == false)
            {
                MakeDir(@"\aaa\dddd\");
            }

            if (FtpDirectoryExists(@"\AAA\") == false)
            {
                MakeDir(@"\AAA");
            }

            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + ":" + ftpPort + "/" + folder + fileInf.Name;
            
            uri = "ftp://" + ftpServerIP + ":" + ftpPort + "/" + @"\DFS\abcd\abcd.gls";
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = usePassive;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();

                return true;
            }
            catch(Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return false;
            }
            //catch (Exception ex){MessageBox.Show(ex.Message, "Upload Error");}
        }

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteFTP(string fileName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);
            }
        }

        /// <summary>
        /// 파일 정보 가져오기
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool GetFilesInfo(string filename, ref DateTime dt)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + filename));
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = usePassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetDateTimestamp;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                dt = response.LastModified;

                response.Close();
                return true;

            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return false;
            }
        }

        /// <summary>
        /// 파일 List 불러오기
        /// </summary>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public List<string> GetFilesDetailList(string subFolder)
        {
            List<string> files = new List<string>();
            string line = null;

            try
            {
                //StringBuilder result = new StringBuilder();

                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + subFolder));
                ftp.UseBinary = true;
                ftp.UsePassive = usePassive;
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);

                while ((line = reader.ReadLine()) != null)
                {
                    files.Add(line);
                }

                reader.Close();
                response.Close();
                return files;
                //MessageBox.Show(result.ToString().Split('\n'));
            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return files;
            }
        }

        /// <summary>
        /// 파일 List 가져오기
        /// </summary>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public List<string> GetFileList(string subFolder)
        {
            List<string> resultList = new List<string>();
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + subFolder));
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = usePassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                //MessageBox.Show(response.StatusDescription);

                foreach (string file in result.ToString().Split('\n'))
                {
                    resultList.Add(file);
                }

                return resultList;
            }           
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);
                
                 return resultList;
            }
        }

        /// <summary>
        /// 폴더가 존재하는지 확인
        /// </summary>
        /// <param name="localFullPathFile"></param>
        /// <returns></returns>
        private bool checkDir(string localFullPathFile)
        {
            FileInfo fInfo = new FileInfo(localFullPathFile);

            if (!fInfo.Exists)
            {
                DirectoryInfo dInfo = new DirectoryInfo(fInfo.DirectoryName);
                if (!dInfo.Exists)
                {
                    dInfo.Create();
                }
                //dInfo.Delete();
            }

            //fInfo.Delete();
            return true;

        }

        /// <summary>
        /// Server에 해당 Directory 있는지 확인
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="ftpUser"></param>
        /// <param name="ftpPassword"></param>
        /// <returns></returns>
        public bool FtpDirectoryExists(string directoryPath)
        {
            bool IsExists = true;

            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + directoryPath));
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                IsExists = false;
            }

            return IsExists;
        }

        /// <summary>
        /// 파일 다운로드
        /// </summary>
        /// <param name="localFullPathFile"></param>
        /// <param name="serverFullPathFile"></param>
        /// <returns></returns>
        public bool DownLoad(string localFullPathFile, string serverFullPathFile)
        {
            FtpWebRequest reqFTP;
            try
            {
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>
                checkDir(localFullPathFile);
                FileStream outputStream = new FileStream(localFullPathFile, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + serverFullPathFile));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = usePassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;

                if (ftpDNTotalSizeEvt != null) ftpDNTotalSizeEvt(cl);

                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    if (ftpDNRcvSizeEvt != null) ftpDNRcvSizeEvt(readCount);

                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }                        
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return false;
            }           
        }

        /// <summary>
        /// 파일 사이즈 가져오기
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = usePassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                fileSize = response.ContentLength;

                ftpStream.Close();
                response.Close();

                return fileSize;
            }            
            catch (Exception ex)
            {
                                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return fileSize;   
            }     
        }
        
        /// <summary>
        /// 파일 이름 변경
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public void Rename(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = usePassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);
            }
        }

        /// <summary>
        /// 폴더 만들기
        /// </summary>
        /// <param name="dirName"></param>
        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                // dirName = name of the directory to create.
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = usePassive;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }           
            catch (Exception ex)
            {
                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);
            }
        }
    }
}
