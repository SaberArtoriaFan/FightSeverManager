
using Saber.Base;
using Saber.Camp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Unit;
using Saber.ECS;
using XianXia.Terrain;

public class BoolContainer
{
    byte value = 0;
    //��ʼֵ������Valueһ��ʼ��ʲô
    bool init;
    /// <summary>
    /// ��ֹ����
    /// ��Ϊһ��Bool����
    /// return Bool[0]&&Bool[1]&&......
    /// һ��Ϊ����ȫΪ��
    /// ֻҪvalue��ֵ����0������Զ����initֵ
    /// </summary>
    public bool BoolValue
    {
        get
        {
            if (init) return value == 0;
            else return value != 0;
        }
        set
        {
            if (value != init)
                this.value += 1;
            else
            {
                if (this.value > 0)
                    this.value -= 1;
            }
        }
    }

    public byte Value { get => value; }

    public void Reset()
    {
        value = 0;
    }
    public BoolContainer(bool init = true)
    {
        this.init = init;
    }
}
public class UnitBase : EntityBase, IDynamicObstacle
{


}

