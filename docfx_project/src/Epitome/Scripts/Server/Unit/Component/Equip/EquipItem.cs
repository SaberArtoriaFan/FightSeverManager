using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class EquipItem : IComponentBase, IAllowOwnNum, IRealName
    {
        #region ½Ó¿Ú
        int IAllowOwnNum.AllowOwnMaxNum => throw new System.NotImplementedException();

        ComponentType IComponentBase.ComponentType => ComponentType.none;

        bool IComponentBase.Enable => throw new System.NotImplementedException();

        IContainerEntity IComponentBase.Owner { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        string IRealName.RealName => throw new System.NotImplementedException();

        void IComponentBase.ClearEnable()
        {
            throw new System.NotImplementedException();
        }

        void IComponentBase.Destory()
        {
            throw new System.NotImplementedException();
        }

        void IComponentBase.Init(IContainerEntity owner)
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}
