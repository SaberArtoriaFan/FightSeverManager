using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianXiaFightGameServer.Controller
{
   public abstract class BaseController
    {
        protected ControllerManager controllerManager;
        public abstract RequestType RequestType { get; }
        public BaseController(ControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;
        }
    }
}
