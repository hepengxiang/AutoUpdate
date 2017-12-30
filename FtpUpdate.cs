using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AutoUpdate
{
    public class FtpUpdate
    {
        private XmlDocument _XmlClient;//客户端配置文件
        private XmlDocument _XmlServer;//服务器端配置文件
        private string client_version;//客户端版本
        private string server_version;//服务端版本
        private FtpWeb _mFtpWeb;//Ftp操作类
        private string _currentDirectory;//程序所在目录
        private string _tmpPath;//存放服务端文件的临时目录
        private string _strClientFileName;//客户端XML文件名
        private string ftpServerIP;//Ftp的IP
        private string ftpServerSocket;//Ftp的端口
        private string ftpfileDirect;//Ftp的子目录
        private string userId;//Ftp的账号
        private string password;//Ftp的密码
        private string windowTitle;//更新窗体显示的标题
        private string exefile;//更新完成后运行的主程序名

        #region -- 封装 --
        public XmlDocument XmlClient
        {
            get
            {
                return _XmlClient;
            }

            set
            {
                _XmlClient = value;
            }
        }

        public XmlDocument XmlServer
        {
            get
            {
                return _XmlServer;
            }

            set
            {
                _XmlServer = value;
            }
        }

        public string Client_version
        {
            get
            {
                return client_version;
            }

            set
            {
                client_version = value;
            }
        }

        public string Server_version
        {
            get
            {
                return server_version;
            }

            set
            {
                server_version = value;
            }
        }

        public FtpWeb MFtpWeb
        {
            get
            {
                return _mFtpWeb;
            }

            set
            {
                _mFtpWeb = value;
            }
        }

        public string CurrentDirectory
        {
            get
            {
                return _currentDirectory;
            }

            set
            {
                _currentDirectory = value;
            }
        }

        public string TmpPath
        {
            get
            {
                return _tmpPath;
            }

            set
            {
                _tmpPath = value;
            }
        }

        public string StrClientFileName
        {
            get
            {
                return _strClientFileName;
            }

            set
            {
                _strClientFileName = value;
            }
        }

        public string FtpServerIP
        {
            get
            {
                return ftpServerIP;
            }

            set
            {
                ftpServerIP = value;
            }
        }

        public string FtpServerSocket
        {
            get
            {
                return ftpServerSocket;
            }

            set
            {
                ftpServerSocket = value;
            }
        }

        public string FtpfileDirect
        {
            get
            {
                return ftpfileDirect;
            }

            set
            {
                ftpfileDirect = value;
            }
        }

        public string UserId
        {
            get
            {
                return userId;
            }

            set
            {
                userId = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public string WindowTitle
        {
            get
            {
                return windowTitle;
            }

            set
            {
                windowTitle = value;
            }
        }

        public string ExeFile
        {
            get
            {
                return exefile;
            }

            set
            {
                exefile = value;
            }
        }
        #endregion

        public FtpUpdate()
        {
            try
            {
                _currentDirectory = Directory.GetCurrentDirectory();
                _tmpPath = _currentDirectory + @"\Temp_Update";

                #region -- 加载客户端XML --
                _strClientFileName = Application.StartupPath + @"\UpdateSetting.Client.xml";
                _XmlClient = new XmlDocument();
                _XmlClient.Load(_strClientFileName);
                #endregion

                ftpServerIP = GetXmlOfClient("ftpServerIP");
                ftpServerSocket = GetXmlOfClient("ftpServerSocket");
                ftpfileDirect = GetXmlOfClient("ftpServerDirect");
                userId = GetXmlOfClient("ftpServerUserId");
                password = GetXmlOfClient("ftpServerPassword");
                windowTitle = GetXmlOfClient("windowTitle");
                exefile = Application.StartupPath + @"\" + this.GetXmlOfClient("exefile");

                #region -- 初始化远程Ftp --
                if (ftpServerSocket == null || ftpServerSocket == "")
                {
                    _mFtpWeb = new FtpWeb(ftpServerIP, ftpfileDirect, userId, password);//无端口的FTP, 以文件名同名的目录
                }
                else
                {
                    _mFtpWeb = new FtpWeb(ftpServerIP, ftpServerSocket, ftpfileDirect, userId, password);//有端口的FTP,以文件名同名的目录
                }
                #endregion

                #region -- 加载服务端XML --
                _XmlServer = new XmlDocument();
                Stream updateSettingStream = _mFtpWeb.Download("UpdateSetting.Svc.xml");
                _XmlServer.Load(updateSettingStream);
                #endregion

                server_version = this.GetVersionOfServer();
                client_version = this.GetVersionOfClient();
                LogInfo.WriteLogFile("服务器版本:" + server_version);
                LogInfo.WriteLogFile("客户端版本:" + client_version);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 取服务器端版本号
        /// </summary>
        /// <returns></returns>
        private string GetVersionOfServer()
        {
            string version = "";
            XmlElement element = this._XmlServer["Main"]["Version"];
            if (element != null)
            {
                version = element.Attributes["value"].Value.Trim();
            }
            element = null;
            return version;
        }

        /// <summary>
        /// 取客户端版本号
        /// </summary>
        /// <param name="p_strKey"></param>
        /// <returns></returns>
        private string GetVersionOfClient()
        {
            return GetXmlOfClient("Version");
        }

        /// <summary>
        /// 比较客户端和服务端版本，看是否需要更新
        /// </summary>
        /// <returns></returns>
        public bool CompareVersion()
        {
            if (server_version == client_version)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 取待下载文件列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetUpdateFileList()
        {
            List<string> list = new List<string>();
            XmlNodeList elementsByTagName = _XmlServer["Main"]["FileList"].GetElementsByTagName("File");
            if ((elementsByTagName != null) && (elementsByTagName.Count != 0))
            {
                XmlNode node = null;
                for (int i = 0; i < elementsByTagName.Count; i++)
                {
                    node = elementsByTagName.Item(i);
                    if (node.Attributes["status"].Value.Trim() == "1")
                    {
                        list.Add(node.Attributes["name"].Value.Trim());
                    }
                }
                node = null;
            }
            return list;
        }

        /// <summary>
        /// 获取下载文件列表
        /// </summary>
        /// <returns></returns>
        public List<clsFileInfo> GetServerFileList()
        {
            List<clsFileInfo> serverFileList = new List<clsFileInfo>();
            List<string> source = GetUpdateFileList();
            if ((source == null) || (source.Count == 0))
                return serverFileList;

            int length = 0;
            clsFileInfo item = null;

            foreach (string str in source)
            {
                item = new clsFileInfo();
                length = str.LastIndexOf(@"\");
                if (length >= 0)
                {
                    item.strSubDirectory = str.Substring(0, length);
                    item.strFileName = str.Substring(length + 1);
                }
                else
                {
                    item.strSubDirectory = string.Empty;
                    item.strFileName = str;
                }
                serverFileList.Add(item);
                LogInfo.WriteLogFile("添加的文件:" + item.strFileName);
            }
            return serverFileList;
        }

        /// <summary>
        /// 更新客户端版本号
        /// </summary>
        public void UpdateVersionOfClient()
        {
            XmlElement element = _XmlClient["Main"]["Version"];
            LogInfo.WriteLogFile("更新客户端版本开始，版本号：" + element.Attributes["value"].Value);
            if (element != null)
            {
                element.Attributes["value"].Value = Server_version;//测试
                _XmlClient.Save("UpdateSetting.Client.xml");
            }
            LogInfo.WriteLogFile("更新客户端版本结束，版本号：" + element.Attributes["value"].Value);
            element = null;
        }

        /// <summary>
        /// 取客户端xml键值
        /// </summary>
        /// <param name="p_strKey">键</param>
        /// <returns></returns>
        private string GetXmlOfClient(string p_strKey)
        {
            string str = string.Empty;
            XmlElement element = _XmlClient["Main"][p_strKey];
            if (element != null)
            {
                str = element.Attributes["value"].Value.Trim();
            }
            element = null;
            return str;
        }

        public class clsFileInfo
        {
            public byte[] bytFileValue { get; set; }
            public string strFileName { get; set; }
            public string strSubDirectory { get; set; }
            public string strTempFilePath { get; set; }
            public string strWorkFilePath { get; set; }
            public string strWorkPath { get; set; }
        }
    }
}
