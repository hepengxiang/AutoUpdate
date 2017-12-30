using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ftp下载

{       
    public class FtpUpdate : IDisposable
    {
        private XmlDocument _XmlClient;//客户端配置文件
        private XmlDocument _XmlServer;//服务器端配置文件

        private FtpWeb _ftpWeb;
        
        private List<clsFileInfo> _list;
        private string _currentDirectory;
        private string _tmpPath;
        frmUpdate _frmUpdate ;
        private string _strClientFileName;

        /// <summary>
        /// 取配置文件中的主程序名
        /// </summary>
        public string exefile
        {
            get;
            set;
        }
        public void  DownLoad()
        {
            try
            {
                _frmUpdate.lFile.Text = "准备下载文件";
                _frmUpdate.Show();
                Application.DoEvents();                            
                string ftpServerIP = GetXmlOfClient("ftpServerIP");
                string fileName = System.IO.Path.GetFileNameWithoutExtension( exefile == string.Empty?  Application.ExecutablePath:exefile) ;// 没有扩展名的文件名                      

                _ftpWeb = new FtpWeb(ftpServerIP, fileName , "", "");//以文件名同名的目录
                _XmlServer = new XmlDocument();
                _XmlServer.Load(_ftpWeb.Download("UpdateSetting.Svc.xml"));
                this.GetList();
                if (_list.Count == 0) return ;
                _frmUpdate.progressBar.Maximum = _list.Count;
                Application.DoEvents();
                if (Directory.Exists(_tmpPath)) Directory.Delete(_tmpPath, true);
                Directory.CreateDirectory(_tmpPath);
                this.DownLoading();
                _frmUpdate.lFile.Text = "下载完毕，更新本地文件请稍候...";
                Application.DoEvents();
                this.UpDateFile();
                _frmUpdate.lFile.Text = "下载完成";
                Application.DoEvents();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }            
        }


        public FtpUpdate()
        {
            _list = new List<clsFileInfo>();
            _currentDirectory = Directory.GetCurrentDirectory();
            _tmpPath = _currentDirectory + @"\Temp_Update";
            _frmUpdate = new frmUpdate()
            {
                //progressBar = { Maximum = _list.Count },
                lFile = { Text = "准备下载文件..." }
            };            
            _strClientFileName = Application.StartupPath + @"\UpdateSetting.Client.xml";
            _XmlClient = new XmlDocument();
            _XmlClient.Load(_strClientFileName);
            exefile = this.GetXmlOfClient("exefile");
            _frmUpdate.Title = this.GetXmlOfClient("title");
        }

        public void Dispose()
        {
            try
            {
                _frmUpdate.Hide();
                _frmUpdate.Dispose();
                _frmUpdate = null;
                if (Directory.Exists(_tmpPath)) Directory.Delete(_tmpPath, true);
            }
            catch
            { }
        }
        /// <summary>
        /// 取下载文件列表
        /// </summary>
        private  void GetList()
        {
            _list.Clear();
            if (this.GetVersionOfServer() == this.GetVersionOfClient()) return;           

            List<string> source = GetUpdateFileList();
            if ((source == null) || (source.Count == 0)) return ;           

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
                _list.Add(item);
            }                   
        }
        /// <summary>
        /// 下载所有文件到临时目录
        /// </summary>
        private void DownLoading()
        {
            clsFileInfo item;
            for (int i = 0; i < _list.Count; i++)
            {
                item = _list[i];
                _frmUpdate.lFile.Text = "更新进度(" + Convert.ToString((int)(i + 1)) + "/" + _list.Count.ToString() + ") : " + item.strFileName;
                Application.DoEvents();
                if ((item.strSubDirectory != string.Empty) && !Directory.Exists(_tmpPath + @"\" + item.strSubDirectory))
                {
                    Directory.CreateDirectory(_tmpPath + @"\" + item.strSubDirectory);
                }
                try
                {
                    _ftpWeb.Download(_tmpPath,item.strSubDirectory + @"\" + item.strFileName);
                    item.strTempFilePath = _tmpPath + @"\" + item.strSubDirectory + @"\" + item.strFileName;
                    item.strWorkFilePath = _currentDirectory + @"\" + item.strSubDirectory + @"\" + item.strFileName;
                }
                catch
                {
                    MessageBox.Show("下载文件: " + item.strFileName + " 失败。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                _frmUpdate.progressBar.Value = i + 1;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 从临时文件更新本地文件
        /// </summary>
        private void UpDateFile()
        {
            foreach (clsFileInfo info in _list)
            {
                if ((info.strSubDirectory != string.Empty) && !Directory.Exists(_currentDirectory + @"\" + info.strSubDirectory))
                {
                    Directory.CreateDirectory(_currentDirectory + @"\" + info.strSubDirectory);
                }
                if (File.Exists(info.strWorkFilePath))
                {
                    File.SetAttributes(info.strWorkFilePath, FileAttributes.Normal);
                    File.Delete(info.strWorkFilePath);
                }
                File.Move(info.strTempFilePath, info.strWorkFilePath);
            }
            UpdateVersionOfClient(GetVersionOfServer());         
        }
                                                               
        /// <summary>
        /// 取客户端版本号
        /// </summary>
        /// <param name="p_strKey"></param>
        /// <returns></returns>
        private  string GetVersionOfClient()
        {
            return GetXmlOfClient("Version");            
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

        /// <summary>
        /// 保存客户端版本号
        /// </summary>
        /// <param name="p_strVersion"></param>
        private  void UpdateVersionOfClient(string p_strVersion)
        {
            XmlElement element =_XmlClient["Main"]["Version"];
            if (element != null)
            {
                element.Attributes["value"].Value = p_strVersion;
                _XmlClient.Save("UpdateSetting.Client.xml");
            }
            element = null;
        }

        /// <summary>
        /// 取待下载文件列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetUpdateFileList()
        {
            List<string> list = new List<string>();
            XmlNodeList elementsByTagName = this._XmlServer["Main"]["FileList"].GetElementsByTagName("File");
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
        /// 取服务器端版本号
        /// </summary>
        /// <returns></returns>
        private  string GetVersionOfServer()
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
    }

    public  class clsFileInfo
    {
        // Properties
        public byte[] bytFileValue { get; set; }

        public string strFileName { get; set; }

        public string strSubDirectory { get; set; }

        public string strTempFilePath { get; set; }

        public string strWorkFilePath { get; set; }
    }
}
