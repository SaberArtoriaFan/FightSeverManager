
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
    //初始值决定了Value一开始是什么
    bool init;
    /// <summary>
    /// 防止过量
    /// 即为一个Bool数组
    /// return Bool[0]&&Bool[1]&&......
    /// 一个为假则全为假
    /// 只要value的值大于0，则永远返回init值
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

