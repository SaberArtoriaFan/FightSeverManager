using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class UIrEMOVE : MonoBehaviour
{
    public string UIName;
    public string ENDwith;
    public bool isAddNumber = true;
    [Button]
    private void ChangeName()
    {
        Transform[] chilidren=GetComponentsInChildren<Transform>();
        int index = 0;
        for(int i = 0; i < chilidren.Length; i++)
        {
            if (chilidren[i].name.StartsWith(UIName))
            {
                string s = isAddNumber ? (++index).ToString() : "";
                chilidren[i].gameObject.name = UIName+ s+ENDwith;
            }
        }
    }
}
