using FishNet;
using FishNet.Component.Animating;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    public class Utility_ShapeShiftManager 
    {
        Dictionary<GameObject, GameObject> originDict = new Dictionary<GameObject, GameObject>();
        Dictionary<GameObject, GameObject> shapeShiftDict = new Dictionary<GameObject, GameObject>();

        public void StartAfterNetwork()
        {
            //base.StartAfterNetwork();
            if(InstanceFinder.IsClient)
                InstanceFinder.GetInstance<NormalUtility>().UnitDeadClientAction += UnitDeadAction;
        }
        /// <summary>
        /// ԭģ����ת�����ű���ΪReset
        /// </summary>
        /// <param name="go"></param>
        /// <param name="modelPath"></param>
        /// <param name="modelName"></param>
        public void ShapeShift(GameObject go, string modelPath, string modelName)
        {
            if (go == null) return;
            GameObject toModel;
            GameObject recycleModel;
            //��һ�ν���
            if (originDict.ContainsKey(go) == false && shapeShiftDict.ContainsKey(go)==false)
            {
                originDict.Add(go, go.GetComponentInChildren<Animator>().gameObject);

            }
            //·��Ϊ��,��˵���Ǳ��ԭ��
            if (string.IsNullOrEmpty(modelPath))
            {
                _ = originDict.TryGetValue(go, out toModel);
                _ = shapeShiftDict.TryGetValue(go, out recycleModel);
            }
            else
            {
                if (!shapeShiftDict.TryGetValue(go, out toModel))
                {
                    toModel = GameObject.Instantiate(ABUtility.Load<GameObject>(modelPath + modelName));
                    if (toModel != null)
                    {
                        toModel.transform.SetParent(go.transform);
                        toModel.transform.localPosition = Vector3.zero;
                        toModel.transform.localRotation = Quaternion.identity;
                        toModel.transform.localScale = Vector3.one;
                        toModel.SetActive(false);
                        shapeShiftDict.Add(go, toModel);
                    }
                }

                _ = shapeShiftDict.TryGetValue(go, out toModel);
                _ = originDict.TryGetValue(go, out recycleModel);
            }
            if (recycleModel == null || toModel == null) { Debug.LogError($"ģ���Ҳ����ˣ���������:����ģ�ͣ�{recycleModel},���ģ�ͣ�{toModel}"); return; }

            recycleModel.SetActive(false);
            toModel.SetActive(true);
            Animator animator = toModel.GetComponentInChildren<Animator>();

            go.GetComponent<Client_UnitProperty>()?.ChangeAnimator(animator);
            go.GetComponent<NetworkAnimator>()?.SetAnimator(animator);

            if (InstanceFinder.IsServer)

                go.GetComponent<CharacterFSM>()?.ChangeAnimator(animator);



        }

        public void UnitDeadAction(GameObject go)
        {
            if (go == null) return;
            if (originDict.ContainsKey(go))
                originDict.Remove(go);
            if (shapeShiftDict.ContainsKey(go))
                shapeShiftDict.Remove(go);
        }
        public void OnDestroy()
        {
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            if (normalUtility != null)
                normalUtility.UnitDeadClientAction -= UnitDeadAction;
            //base.OnDestroy();
            originDict.Clear();
            shapeShiftDict.Clear();
        }
    }
}
