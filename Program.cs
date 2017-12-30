using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace AutoUpdate
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogInfo.WriteLogFile("\n");
            LogInfo.WriteLogFile("**********************程序开始**********************");

            //判断软件是否重复运行
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (process.MainModule.FileName == current.MainModule.FileName)
                    {
                        MessageBox.Show("程序已经运行！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        LogInfo.WriteLogFile("程序已经运行！");
                        LogInfo.WriteLogFile("**********************程序结束**********************");
                        LogInfo.WriteLogFile("\n");
                        System.Environment.Exit(0);
                        return;
                    }
                }
            }
            /*
             1: 取出服务器版本和本地版本对比看是否需要更新
             2：下载服务器文件
             3：覆盖客户端文件
             4：修改客户端版本号
             */
            string exefile = Application.ExecutablePath + @".exe";
            FtpUpdate ftpupdate = new FtpUpdate();
            if (ftpupdate.ExeFile != null && ftpupdate.ExeFile != string.Empty && ftpupdate.ExeFile != "")
                exefile = ftpupdate.ExeFile;
            LogInfo.WriteLogFile("exefile文件是：" + exefile);
            bool shouldUpdate = ftpupdate.CompareVersion();
            if (shouldUpdate)//客户端和服务端版本是否一致
            {
                //客户端和服务端版本不一致，执行更新操作
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmUpdate(ftpupdate));
            }
            else
            {
                if (File.Exists(ftpupdate.ExeFile))
                {
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(ftpupdate.ExeFile);
                    p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                    LogInfo.WriteLogFile("客户端和服务端版本号一致，不需要更新");
                }
                else
                {
                    MessageBox.Show(ftpupdate.ExeFile + "不存在，请确认!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    LogInfo.WriteLogFile(ftpupdate.ExeFile + "不存在，请确认!");
                }
            }
            LogInfo.WriteLogFile("**********************程序结束**********************");
            LogInfo.WriteLogFile("\n");
            System.Environment.Exit(0);
        }
    }
}