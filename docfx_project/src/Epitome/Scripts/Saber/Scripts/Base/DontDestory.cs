using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.Base
{
    public class DontDestory : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            //Transform[] objs = GetComponentsInChildren<Transform>();

            //if (objs.Length > 1)
            //{
            //    Destroy(this.gameObject);
            //}

            DontDestroyOnLoad(this.gameObject);
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
