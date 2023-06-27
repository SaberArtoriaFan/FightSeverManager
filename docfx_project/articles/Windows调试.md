# Windows调试
## 1.与Linux不同
Linux开启后默认自动连接后端服务器，并且关闭调试接口
## 2.CommondUtility
### 2.1如何添加调试指令
```CSharp
namespace XianXiaFightGameServer.Tool
{
    internal static class CommondUtility
    {
        private static void CLS(string s)
        {
            Console.Clear();
        }
        private static void Kill(string s)
        {
            int id= Convert.ToInt32(s);
            if(InstanceFinder.GetInstance<LocalServer>().AllProcessNameDict.TryGetValue(id, out var process)) 
            { 
                process.Kill();
            }
        }
    }
}
```
在**CommondUtility**类中添加指令函数（例如：CLS），该函数必须为**静态**，**无返回值**，**参数**有且只能有一个**string**    

### 2.2如何调试
在Windows平台上启动程序，在命令行窗口中输入**-指令函数名**,如果该指令有接收参数则**空格后输入参数**\
例如：
>-CLS
>
>-Kill 9988
>
>-InitServer 192.168.2.20,8885


**注意**：命令指令并不严格区分大小写
>-iNitserveR 192.168.2.20,8885    


以上指令同时生效，所以请避免在CommondUtility类中出现大小写不分后**同名**的指令函数（例如不要同时出现"CLS"与"cls"）