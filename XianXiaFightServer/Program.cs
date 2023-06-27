using System.Net.Mail;
using System.Runtime.InteropServices;
using XianXiaFightGameServer.Controller;
using XianXiaFightGameServer.Email;
using XianXiaFightGameServer.FightClientToServer;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Log;
using XianXiaFightGameServer.Tool;
using XianXiaFightServer.Tool;

Saber.SaberDebug.Log("程序启动1.1" + System.Diagnostics.Process.GetCurrentProcess());
//读取配置参数
JsonUtility.InitConfiguration();
//注册退出事件，只有windows有用
ToolUtility.InitAppEixtEvent();
//单例
new InstanceFinder();
InstanceFinder.Register(new TimerSystem());//计时器系统
InstanceFinder.Register<PoolManager>(new PoolManager());//对象池系统
MailUtility.InitMailPlatform();
ControllerManager controllerManager = new ControllerManager();//消息响应管理
//本地服务器开启
LocalServer localServer = new LocalServer(JsonUtility.FightServerParameter.SelfPort, controllerManager,JsonUtility.FightServerParameter.PreparaProcessNum,JsonUtility.ServerInstancePath,JsonUtility.UpdateResourcePath);
//后端客户端开启-》连接至总服务器
ClientServer clientServer = new ClientServer(controllerManager);
//设置端口区间
localServer.AddPort(JsonUtility.FightServerParameter.FightAllotPorts);
InstanceFinder.Register<ClientServer>(clientServer);
InstanceFinder.Register<LocalServer>(localServer);

InstanceFinder.Register(new AutoProcessManager()); //自动分配战斗服务器实例
InstanceFinder.GetInstance<TimerSystem>().AddTimer(InstanceFinder.GetInstance<AutoProcessManager>().AutoReayFight, 0.3f,true);//开启自动分配
//设置自身IP地址
clientServer.SetSelfIP(JsonUtility.FightServerParameter.SelfIP);
//Liunx自动开启连接后端服务器
if(OSPlatformUtility.MyPlatformTarget == OSPlatformUtility.PlatformTarget.Linux)
    clientServer.InitSocket(JsonUtility.FightServerParameter.ServerIP, JsonUtility.FightServerParameter.ServerPort);


Console.WriteLine("再下面就是监听输入了");

//Liunx卡住
if (OSPlatformUtility.MyPlatformTarget==OSPlatformUtility.PlatformTarget.Linux)
{
    //localServer.StartFightServerProcess();
    var cancellationTokenSource = new CancellationTokenSource();
    AppDomain.CurrentDomain.ProcessExit += (s, e) => cancellationTokenSource.Cancel();
    Console.CancelKeyPress += (s, e) => cancellationTokenSource.Cancel();
    await Task.Delay(-1, cancellationTokenSource.Token).ContinueWith(t =>
    {

    });
}
//Windows接受调试
else if(OSPlatformUtility.MyPlatformTarget == OSPlatformUtility.PlatformTarget.Windows)
{

    while (true)
    {
        CommondUtility.ReadLine();
    }
}





