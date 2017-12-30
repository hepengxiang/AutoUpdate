using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using static AutoUpdate.FtpUpdate;

namespace AutoUpdate
{
    public partial class frmUpdate : Form
    {
        private FtpUpdate _ftpUpdate;
        bool m_blnAltF4;

        public frmUpdate(FtpUpdate _ftpUpdate)
        {
            InitializeComponent();
            this._ftpUpdate = _ftpUpdate;   
        }

        private void frmUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.m_blnAltF4)
            {
                e.Cancel = true;
            }
        }

        private void frmUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && (e.KeyCode == Keys.F4))
            {
                this.m_blnAltF4 = true;
            }
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            base.Top -= 50;
            this.lblWindowTitle.Text = _ftpUpdate.WindowTitle;
            List<clsFileInfo> serverFileList = _ftpUpdate.GetServerFileList();
            if (serverFileList.Count == 0)
            {
                if (File.Exists(_ftpUpdate.ExeFile))
                {
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(_ftpUpdate.ExeFile);
                    p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                    LogInfo.WriteLogFile("服务端无可下载文件");
                    _ftpUpdate.UpdateVersionOfClient();//更新客户端版本号
                }
                else
                {
                    ShowMessage(_ftpUpdate.ExeFile + "不存在，请确认!");
                    //MessageBox.Show(_ftpUpdate.ExeFile + "不存在，请确认!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    LogInfo.WriteLogFile(_ftpUpdate.ExeFile + "不存在，请确认!");
                }
                LogInfo.WriteLogFile("**********************程序结束**********************");
                LogInfo.WriteLogFile("\n");
                exitEXE();
            }
            else
            {
                //设置进度条
                this.progressBar1.Maximum = serverFileList.Count;
                this.progressBar1.Step = 1;
                this.progressBar1.Value = 0;
                //更新开始
                Thread thread = new Thread(new ParameterizedThreadStart(updateThread));
                thread.Start((object)serverFileList);
            }
        }

        private void updateThread(object objServerFileList)
        {
            List<clsFileInfo> serverFileList = (List<clsFileInfo>)objServerFileList;
            if (Directory.Exists(_ftpUpdate.TmpPath))
                Directory.Delete(_ftpUpdate.TmpPath, true);
            Directory.CreateDirectory(_ftpUpdate.TmpPath);
            DownLoading(serverFileList);//下载文件
            LogInfo.WriteLogFile("下载完成");
            if (this.lblUpdateMsg.InvokeRequired)//找到创建此控件的线程
            {
                this.lblUpdateMsg.Invoke(new Action<string>(s => { this.lblUpdateMsg.Text = s; }), "下载完毕，更新本地文件请稍候...");
            }
            LogInfo.WriteLogFile("更新本地文件开始，请稍候...");
            UpDateFile(serverFileList);//更新文件
            LogInfo.WriteLogFile("更新本地文件结束");
            if (this.lblUpdateMsg.InvokeRequired)//找到创建此控件的线程
            {
                this.lblUpdateMsg.Invoke(new Action<string>(s => { this.lblUpdateMsg.Text = s; }), "下载完成");
            }
            if (File.Exists(_ftpUpdate.ExeFile))
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(_ftpUpdate.ExeFile);
                //p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                _ftpUpdate.UpdateVersionOfClient();//更新客户端版本号
            }
            else
            {
                ShowMessage(_ftpUpdate.ExeFile + "不存在，请确认!");
                //MessageBox.Show(_ftpUpdate.ExeFile + "不存在，请确认!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                LogInfo.WriteLogFile(_ftpUpdate.ExeFile + "不存在，请确认!");
            }
            LogInfo.WriteLogFile("**********************程序结束**********************");
            LogInfo.WriteLogFile("\n");
            exitEXE();
        }

        /// <summary>
        /// 下载所有文件到临时目录
        /// </summary>
        private void DownLoading(List<clsFileInfo> serverFileList)
        {
            clsFileInfo item;
            LogInfo.WriteLogFile("服务端文件下载开始");
            for (int i = 0; i < serverFileList.Count; i++)
            {
                item = serverFileList[i];
                //更新下载状态
                if (this.lblUpdateMsg.InvokeRequired)//找到创建此控件的线程
                {
                    this.lblUpdateMsg.Invoke(new Action<string>(s => { this.lblUpdateMsg.Text = s; }), 
                        "更新进度(" + Convert.ToString((int)(i + 1)) + "/" + serverFileList.Count.ToString() + ") : " + item.strFileName);//"下载失败"
                }
                if ((item.strSubDirectory != string.Empty) && !Directory.Exists(_ftpUpdate.TmpPath + @"\" + item.strSubDirectory))
                {
                    Directory.CreateDirectory(_ftpUpdate.TmpPath + @"\" + item.strSubDirectory);
                }
                try
                {
                    _ftpUpdate.MFtpWeb.Download(_ftpUpdate.TmpPath, item.strSubDirectory + @"\" + item.strFileName);
                    item.strTempFilePath = _ftpUpdate.TmpPath + @"\" + item.strSubDirectory + @"\" + item.strFileName;
                    item.strWorkFilePath = _ftpUpdate.CurrentDirectory + @"\" + item.strSubDirectory + @"\" + item.strFileName;
                    item.strWorkPath = _ftpUpdate.CurrentDirectory + @"\" + item.strSubDirectory;
                }
                catch
                {
                    ShowMessage("下载文件: " + item.strFileName + " 失败。");
                    //MessageBox.Show("下载文件: " + item.strFileName + " 失败。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    LogInfo.WriteLogFile("(" + item.strTempFilePath + ")文件下载失败");
                }
                //更新进度条
                if (this.progressBar1.InvokeRequired)//找到创建此控件的线程
                {
                    this.progressBar1.Invoke(new Action<int>(s => { this.progressBar1.Value = s; }), i + 1);
                }
                LogInfo.WriteLogFile("(" + item.strTempFilePath + ")文件下载成功");
            }
            LogInfo.WriteLogFile("服务端文件下载结束");
        }

        /// <summary>
        /// 解压zip并获得所有的文件
        /// </summary>
        /// <param name="fileToZip"></param>
        /// <param name="zipedFile">指定解压目录</param>
        private bool getAllZipFiles(string fileToUnZip, string zipedFolder)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    LogInfo.WriteLogFile("解压文件内有:" + ent + ";大小：" + ent.Size);
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');

                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

        /// <summary>
        /// 从临时文件更新本地文件
        /// </summary>
        private void UpDateFile(List<clsFileInfo> serverFileList)
        {
            foreach (clsFileInfo info in serverFileList)
            {
                // 移动文件
                if ((info.strSubDirectory != string.Empty) && !Directory.Exists(_ftpUpdate.CurrentDirectory + @"\" + info.strSubDirectory))
                {
                    Directory.CreateDirectory(_ftpUpdate.CurrentDirectory + @"\" + info.strSubDirectory);
                }
                if (File.Exists(info.strWorkFilePath))
                {
                    File.SetAttributes(info.strWorkFilePath, FileAttributes.Normal);
                    File.Delete(info.strWorkFilePath);
                }
                if (info != null)
                {
                    // 如果是zip文件则解压该文件
                    if (info.strTempFilePath.EndsWith(".zip"))
                    {
                        bool isZip = this.getAllZipFiles(info.strTempFilePath, info.strWorkPath);
                        if (!isZip)
                        {
                            ShowMessage("解压失败");
                            //MessageBox.Show("解压失败", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            LogInfo.WriteLogFile("文件:" + info.strFileName + "解压失败");
                        }
                        else
                        {
                            LogInfo.WriteLogFile("文件:" + info.strFileName + "解压成功");
                        }
                    }
                    else
                    {
                        File.Move(info.strTempFilePath, info.strWorkFilePath);
                    }
                    //File.Move(info.strTempFilePath, info.strWorkFilePath);
                }
            }
            LogInfo.WriteLogFile("从临时目录移动成功。");
        }

        public void exitEXE()
        {
            try
            {
                if (Directory.Exists(_ftpUpdate.TmpPath))
                    Directory.Delete(_ftpUpdate.TmpPath, true);
                System.Environment.Exit(0);
            }
            catch
            { }
        }

        /// <summary>
        /// 主线程弹出提示框
        /// </summary>
        /// <param name="msg"></param>
        public void ShowMessage(string msg)
        {
            this.Invoke(new MessageBoxShow(MessageBoxShow_F), new object[] { msg });
        }

        delegate void MessageBoxShow(string msg);
        void MessageBoxShow_F(string msg)
        {
            MessageBox.Show(msg, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
