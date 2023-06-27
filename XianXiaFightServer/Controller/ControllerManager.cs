using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Proto;
using XianXiaFightGameServer.FightClientToServer;
using XianXiaFightGameServer.Local;
namespace XianXiaFightGameServer.Controller
{
    public enum RequestType
    {
        Loacl,
        Server
    }
    public class ControllerManager
    {
        private Dictionary<RequestType, BaseController> controllerDict = new Dictionary<RequestType, BaseController>();

        //private LocalServer localserver;

        public ControllerManager()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var t in assembly.GetTypes())
            {
                if (typeof(BaseController).IsAssignableFrom(t) && t.IsAbstract == false)
                {
                    BaseController b = Activator.CreateInstance(t,this) as BaseController;
                    if (b == null) break;
                    if (!controllerDict.ContainsKey(b.RequestType)) controllerDict.Add(b.RequestType, b);
                }
            }
            //this.localserver=localserver;
            //this.server = server;
            //UserController userController = new UserController();
            //controllerDict.Add(userController.GetRequestCode, userController);
            //RoomController roomController = new RoomController();
            //controllerDict.Add(roomController.GetRequestCode, roomController);
            //MatchController matchController = new MatchController();
            //controllerDict.Add(matchController.GetRequestCode, matchController);
            //GameServerController gameServerController = new GameServerController();
            //controllerDict.Add(gameServerController.GetRequestCode, gameServerController);
        }

        public void Local_HandleRequest(MainPack pack,LocalServer server, LocalClient client)
        {
            if (controllerDict.TryGetValue(RequestType.Loacl, out BaseController controller))
            {
                string metName = pack.ActionCode.ToString();
                MethodInfo method = controller.GetType().GetMethod(metName);
                if (method == null) { Console.WriteLine("未找到指定方法"); return; }
                object[] obj = new object[] {server, client, pack };
                object retobj = method.Invoke(controller, obj);
                if (retobj != null)
                {
                    //Console.WriteLine("找到了指定方法" + metName);
                    client.Send(retobj as MainPack);
                };
            }
            else
            {
                Console.WriteLine("没有找到相应的处理方法");
            }
        }
        public void HandleRequest(MainPack pack, ClientServer client)
        {
            //pack.
            if (controllerDict.TryGetValue(RequestType.Server, out BaseController controller))
            {
                string metName = pack.ActionCode.ToString() ;/*pack. .ToString();*/
                MethodInfo method = controller.GetType().GetMethod(metName);
                if (method == null)
                {
                    Saber.SaberDebug.LogError($"[{RequestType.Server}]没有找到相应的处理方法:{pack.ActionCode},请检查你的ControllerManager！！！");
                    return; }
                object[] obj = new object[] {  client, pack };
                object retobj = method.Invoke(controller, obj);
                if (retobj != null)
                {
                    Saber.SaberDebug.Log($"[To {RequestType.Server}]:返回{pack.ActionCode}，结果{pack.ReturnCode}");
                    client.Send(retobj as MainPack);
                };
            }
            else
            {
                Saber.SaberDebug.LogError($"[{RequestType.Server}]没有找到相应的处理方法:{pack.ActionCode},请检查你的ControllerManager！！！");
            }
        }
    }
}
