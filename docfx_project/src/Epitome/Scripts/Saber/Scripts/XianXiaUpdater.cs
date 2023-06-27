using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Server
{

    public abstract class XianXiaUpdater :Singleton<XianXiaUpdater>
    {
        public abstract void EnterTargetModel(int id, Action<bool> updateResourceFinishEvent);
    }
}
