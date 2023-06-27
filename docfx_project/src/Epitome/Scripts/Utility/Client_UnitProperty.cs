using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Saber.Base;
using FishNet.Connection;

namespace XianXia
{
    public class Client_UnitProperty : NetworkBehaviour,IClient_UnitProperty
    {
        [ShowInInspector]
        [SyncVar(Channel = FishNet.Transporting.Channel.Unreliable,OnChange = "OnHealthChange")]
        private float healthPointPer;

        [ShowInInspector]
        [SyncVar(Channel = FishNet.Transporting.Channel.Unreliable, OnChange = "OnMagicChange")]
        private float magicPointPer;

        [ShowInInspector]
        [SyncVar]
        private float unitHigh;

        [SerializeField]
        HealthMagicPointShowUI healthMagicPointShowUI;
        // Transform target;
        Animator animator;
#if !UNITY_SERVER||UNITY_EDITOR
        XianXia.Client.XianXiaSkeletonAnimationHandle xsah;
#endif

        private void Start()
        {
            if (IsClient)
            {
                _ = ChangeAnimator();
#if !UNITY_SERVER
                Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().AddUnitProperty(this.NetworkObject);
                xsah = GetComponentInChildren<XianXia.Client.XianXiaSkeletonAnimationHandle>();
#endif

            }
        }
        public float HealthPointPer { get => healthPointPer; set => healthPointPer = value; }
        public float MagicPointPer { get => magicPointPer; set => magicPointPer = value; }
        public float UnitHigh { get => unitHigh; set => unitHigh = value; }
        public HealthMagicPointShowUI HealthMagicPointShowUI { get => healthMagicPointShowUI; }
        public Animator Animator { get => animator;  }

        [Client]
        public void SetShowUI(HealthMagicPointShowUI ui)
        {
            healthMagicPointShowUI = ui;
            //if(healthMagicPointShowUI!=null)
            //{
            //    healthMagicPointShowUI.SetHealthPer(healthPointPer, false);
            //    healthMagicPointShowUI.SetMagicPer(magicPointPer, false);

            //}
        }
        public Animator ChangeAnimator()
        {
#if !UNITY_SERVER || UNITY_EDITOR
            
#endif
            animator = GetComponentInChildren<Animator>();
            return animator;
        }
        public void ChangeAnimator(Animator animator)
        {
            this.animator = animator;
        }

        [Client]
        private void OnHealthChange(float pre, float next,bool asServer)
        {
#if !UNITY_SERVER

            //if (asServer||pre==next) return;
            //Debug.Log("更新血量"+next);
            if (healthMagicPointShowUI == null) return;
            healthMagicPointShowUI.SetHealthPer(next,false);
            if (next <= 0)healthMagicPointShowUI?.gameObject.SetActive(false);
#endif
        }

        [Client]
        private void OnMagicChange(float pre, float next,bool asServer)
        {
#if !UNITY_SERVER
            if (healthMagicPointShowUI==null) return;
            healthMagicPointShowUI.SetMagicPer(next,false);
#endif
        }
//        [ServerRpc(RequireOwnership =false)]
//        public void ServerRpc_SetHealthSelfColor()
//        {
//#if UNITY_SERVER
//            InstanceFinder.GetInstance<NormalUtility>().Server_SetUnitColor
//#endif
//        }
        [ObserversRpc]
        public void ORPC_SetAnimatorSpeed(float speed)
        {
#if !UNITY_SERVER || UNITY_EDITOR
            if (xsah == null)
                xsah = GetComponentInChildren<Client.XianXiaSkeletonAnimationHandle>();
            xsah.Speed = speed;
#endif
        }
        [ObserversRpc]
        public void ORPC_AnimatorParameter_Bool(int id, bool val)
        {
            SetAnimatorParameter_Bool(id, val);
        }
        [Client]
        public void SetAnimatorParameter_Bool(int id,bool val)
        {
            Debug.Log("设置动画参数");
            animator.SetBool(id, val);

#if !UNITY_SERVER
#endif
        }
        [Button]
        [Client]
        public void TestSetAnimatorParameter_Bool()
        {
            Debug.Log("设置动画参数");
            animator.SetBool(FSM.AnimatorParameters.Attack,!animator.GetBool(FSM.AnimatorParameters.Attack));

#if !UNITY_SERVER
#endif
        }
        [Client]
        public void SetAnimatorParameter_Float(int id, float val)
        {
#if !UNITY_SERVER

            animator.SetFloat(id, val);
#endif
        }
        [Client]
        public void SetAnimatorParameter_Trigger(int id)
        {

#if !UNITY_SERVER
            if (animator == null || animator.isActiveAndEnabled == false) { _ = ChangeAnimator(); return; }
            animator.ResetTrigger(id);
            animator.SetTrigger(id);
#endif
        }
        [Client]
        private void UpdateUnitProperty()
        {
            //healthMagicPointShowUI.gameObject.SetActive(true);
#if !UNITY_SERVER
            if (healthMagicPointShowUI == null || transform == null) return;
            Vector3 ItemScreenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * unitHigh * transform.localScale.x/*+ Vector3.up * 0.05f * MainCameraCtrl.Instance.scale*/);
            Vector3 RightPos = new Vector3(ItemScreenPos.x, ItemScreenPos.y, 0);
            healthMagicPointShowUI.transform.position = RightPos;
#endif
        }
        [ObserversRpc(ExcludeServer =false,RunLocally = true)]
        public void ORPC_ReIdle( string name, float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
        {
            if (animator == null) _=ChangeAnimator();
            animator.Update(0);
            if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
            {
                animator.CrossFade(name, transitionDuration, layer, normalizedTime);
            }
            else
            {
                animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
                animator.Update(0);
                animator.CrossFade(name, transitionDuration, layer, normalizedTime);
            }
            animator.speed = 0;
            //Debug.Log("死亡DD" + name);
        }
        [ObserversRpc(ExcludeServer = false, RunLocally = true)]
        public void ORPC_ReDeath(string name, float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
        {
            if (animator == null) _ = ChangeAnimator();
            animator.Update(0);
            if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
            {
                animator.CrossFade(name, transitionDuration, layer, normalizedTime);
            }
            else
            {
                animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
                animator.Update(0);
                animator.CrossFade(name, transitionDuration, layer, normalizedTime);
            }
            animator.SetBool("death", true);
            animator.speed = 1;
            Debug.Log("死亡DD" + name);
        }
    [ObserversRpc]
    public void ORPC_ChangeLayer(GameObject go, int layer)
    {
            Client_ChangeLayer(go, layer);
    }
        [Client]
        public void Client_ChangeLayer(GameObject go, int layer)
        {
            if (go == null) return;
            bool active = go.activeSelf;
            go.SetActive(true);
            var gos = go.transform.GetComponentsInChildren<Transform>();
            foreach (var v in gos)
                v.gameObject.layer = layer;
            go.SetActive(active);
        }
    //[Client]
    //private static void Client_UnitDead(GameObject go)
    //{
    //    if (go == null) return;
    //    Client_UnitProperty client_UnitProperty = go.GetComponent<Client_UnitProperty>();
    //    if()
    //}
    private void LateUpdate()
        {
            if(IsClient)
                UpdateUnitProperty();

#if Client 
#endif
        }
        void OnEnable()
        {
            //gameObject.transform.position = gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
#if Client || UNITY_EDITOR
            healthMagicPointShowUI?.gameObject.SetActive(true);
#endif
        }
        void OnDisable()
        {
#if Client || UNITY_EDITOR
            healthMagicPointShowUI?.gameObject.SetActive(false);
#endif
        }
        void OnDestroy()
        {
//#if !UNITY_SERVER
//            if (healthMagicPointShowUI != null&&                        
//                Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>() != null&& 
//                InstanceFinder.NetworkManager.IsClient)
//                Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().RemoveUnitProperty(gameObject);
//#endif
        }
        public override void OnDespawnServer(NetworkConnection connection)
        {
            base.OnDespawnServer(connection);
        }
        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);
            //gameObject.transform.position = gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
        }
    }
}
