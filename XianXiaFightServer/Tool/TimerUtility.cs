using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using XianXiaFightGameServer.Tool;
using Timer = System.Timers.Timer;

namespace XianXiaFightGameServer.Tool
{
    //internal static  class TimerUtility
    //{

    //    public static Timer AddTimer_Once(ElapsedEventHandler handler,float timelong)
    //    {
    //        Timer timer = new Timer();
    //        //timer.SynchronizingObject=InstanceFinder.GetInstance<AutoProcessManager>(); 
    //        timer.Elapsed += (o,t)=> { handler?.Invoke(o,t);timer.Stop();timer.Dispose(); };
    //        timer.Interval= timelong*1000;
    //        timer.Enabled= true;
    //        timer.Start();
    //        return timer;
    //    }
    //    public static Timer AddTimer_Repeated(ElapsedEventHandler handler, float timelong,uint repeatTime=0)
    //    {
    //        Timer timer=new Timer();
    //        timer.Interval = timelong * 1000;
    //        timer.Enabled= true;
    //        timer.AutoReset= true;
    //        if (repeatTime == 0)
    //            timer.Elapsed += handler;
    //        else
    //        {
    //            uint time = 0;
    //            timer.Elapsed += (o, t) =>
    //            {
    //                handler?.Invoke(o, t);
    //                if (++time >= repeatTime)
    //                {
    //                    timer.Stop();
    //                    timer.Dispose();
    //                }
    //            };
    //        }
    //        timer.Start();
    //        return timer;
    //    }



    //}
}
