using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Saber阿尔托莉雅
namespace Saber.Base
{
    public interface ISelectAction
    {
        void OnSelected();

        void UnSelected();
    }
    public interface IMoveAction
    {
        void MoveTo(Vector3 vector3);
    }

    public class VecterShell
    {
        readonly Vector3 vector3;
        public VecterShell(Vector3 vector3)
        {
            this.vector3 = vector3;
        }
        public Vector3 Vector3 => vector3;
    }

}
namespace XianXia.Unit {
    public interface IClothes
    {
        int Init(string modelName);
    }
}
