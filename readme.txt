这是一个通用的ftp下载程序
原理:
    1.首先读取本地UpdateSetting.Client.xml配置文件(下称客户配置)
    
    2.以客户配置节[Main][ftpServerIP]指定的地址连接ftp服务器(必须支持匿名)
    
    3.取服务器端由客户配置节[Main][exefile]指定的执行程序名同名(或当前执行文件名同名)目录下的UpdateSetting.Svc.xml(下称服务配置)
    
    4.比较两个配置文件的[Main][Version]节,若不相同则下载由服务配置节[Main][FileList]指定的文件及目录
    
    5.下载时先保存到本地\temp_update目录,成功下载后再覆盖原有文件
    
使用主法
   1.设置ftp服务器,建立与主程序的执行文件同名的目录,并将最新版本软件拷入,修改UpdateSetting.Svc.xml与UpdateSetting.Client.xm中的[Main][Version]不一致
     
   2.执行下面步骤3或4
   
   3.配置UpdateSetting.Client.xml中的[Main][exefile]节为主程序的执行文件名
      
   4.将当前ftp下载的.exe文件重命名为主程序的执行文件名,删除UpdateSetting.Client.xml中的[Main][exefile]节
     同时将ftp服务器端的主程序重命名,增加一个.exe后缀
   
   
   
     
     
       