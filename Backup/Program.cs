using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace ftp下载
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string exefile = Application.ExecutablePath + @".exe";
            using (FtpUpdate ftpupdate = new FtpUpdate())
            {
                ftpupdate.DownLoad();
                if (ftpupdate.exefile != string.Empty) exefile = Application.StartupPath+@"\"+ ftpupdate.exefile;
            }
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(exefile);
            //p.WaitForExit();//关键，等待外部程序退出后才能往下执行
        }
    }
}