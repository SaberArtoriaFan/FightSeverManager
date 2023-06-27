using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidZone : Singleton<VoidZone>
{
   public void PutIn(GameObject go)
    {
        go.transform.position = gameObject.transform.position;
    }
    public void PutOut(GameObject go,Vector3 pos)
    {
        go.transform.position = pos;
    }
}
