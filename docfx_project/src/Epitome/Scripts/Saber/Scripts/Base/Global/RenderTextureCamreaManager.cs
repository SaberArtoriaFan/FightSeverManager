using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.Base
{
    public class RenderTextureCamreaManager : AutoSingleton<RenderTextureCamreaManager>
    {
        [SerializeField]
        Camera camera;
        //[SerializeField]
        //LayerMask showLayer;
        int layer;
        Dictionary<GameObject, int> targetLayer_Dict;
        Timer timer=null;
        protected override void Init()
        {
            base.Init();
            camera=transform.GetComponentInChildren<Camera>();
            targetLayer_Dict = new Dictionary<GameObject, int>();
            if (camera == null) { Debug.Log("CANNOTfIND"); }
            layer = LayerMask.NameToLayer("RenderTexture");
        }
        void UpdateParemeter(GameObject targetRoot, float high, int far = 2, float size = 1)
        {
            if (camera == null) return;
            camera.transform.position = new Vector3(targetRoot.transform.position.x, targetRoot.transform.position.y + high, targetRoot.transform.position.z) + targetRoot.transform.forward * far;

            Quaternion q = Quaternion.identity;
            q.SetLookRotation(targetRoot.transform.forward * -1, targetRoot.transform.up);
            camera.transform.rotation = q;

            camera.orthographicSize = size;
        }
        public void SetRenderTextureCamrea(GameObject targetRoot,float high,int far=2,float size=1)
        {
            if(timer == null)
            {
                timer = TimerManager.instance.AddTimer(() => { UpdateParemeter(targetRoot, high, far, size); }, Time.deltaTime,true);
            }
            if(!targetLayer_Dict.ContainsKey(targetRoot))
                targetLayer_Dict.Add(targetRoot,targetRoot.layer);
            if(targetRoot.layer != layer)
            {
                foreach (var v in targetRoot.GetComponentsInChildren<Transform>())
                {
                    v.gameObject.layer = layer;
                }
            }
            camera.gameObject.SetActive(true);
        }
        public void CancelShow(GameObject targetRoot=null)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            if(targetRoot != null&&targetLayer_Dict.ContainsKey(targetRoot))
            {
                foreach (var v in targetRoot.GetComponentsInChildren<Transform>())
                {
                    v.gameObject.layer = targetLayer_Dict[targetRoot];
                }
                targetLayer_Dict.Remove(targetRoot);
            }
            camera.gameObject.SetActive(false);
        }
    }
}
