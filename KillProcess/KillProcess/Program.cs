// See https://aka.ms/new-console-template for more information

using KillProcess;

KillParameter killParameter= ToolUtility.ReadParamter();
int res = 0;
res+= ToolUtility.KillProcess(killParameter.SonProcessName);
res+= ToolUtility.KillProcess(killParameter.PerentProcessname);
Console.WriteLine($"已关闭{res}个进程，按任意键退出");
Console.ReadKey();