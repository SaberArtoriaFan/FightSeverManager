using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    public abstract class BaseController 
    {
        public abstract ActionCode ActionCode { get; }
        public event Action<MainPack> RespondAction;
        public event Action<MainPack> SuccessAction;
        public event Action<MainPack> FailAction;
        ControllerManager controllerManager;
        const float maxWaitSeconds = 5f;
        public virtual void Init(ControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;
        }
        internal virtual void Destory()
        {
            RespondAction = null;
            this.controllerManager =null;
            SuccessAction = null;
            FailAction = null;

        }
        ~BaseController()
        {
        }
        /// <summary>
        /// ������ܵ��������󣬾ͽ��л�Ӧ�����Ҹ��ݴ������ظ��ɹ�����ʧ�ܾ��
        ///������ܵ����ǻ�Ӧ�ͷ��;��
        /// </summary>
        public  void Respond(MainPack mainPack) 
        {
            //float timer = 0;
            //WaitUntil waitUntil = new WaitUntil(() => { timer += Time.deltaTime; return timer >= maxWaitSeconds || mainPack.ReturnCode != default; });
            RespondAction?.Invoke(mainPack);
            //if (mainPack.ReturnCode == default)
            //    yield return waitUntil;
            //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.Log("��Ӧ" + mainPack.ActionCode);
            switch (mainPack.ReturnCode)
            {
                case ReturnCode.Success:
                    controllerManager.Send(SuccessHandle(mainPack));
                    break;
                case ReturnCode.Fail:
                    controllerManager.Send(FailHandle(mainPack));
                    break;
                default:
                    //controllerManager.Send(FailHandle(mainPack));
                    break;
            }
        }
        //IEnumerator IE_Respond(MainPack mainPack)
        //{
        //float timer = 0;
        //WaitUntil waitUntil = new WaitUntil(() => { timer += Time.deltaTime; return timer >= maxWaitSeconds || mainPack.ReturnCode != default; });
        //RespondAction?.Invoke(mainPack);
        //    if (mainPack.ReturnCode == default)
        //        yield return waitUntil;
        //    Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!");
        //    switch (mainPack.ReturnCode)
        //    {
        //        case ReturnCode.Success:
        //            controllerManager.Send(SuccessHandle(mainPack));
        //            break;
        //        case ReturnCode.Fail:
        //            controllerManager.Send(FailHandle(mainPack));
        //            break;
        //        default:
        //            //controllerManager.Send(FailHandle(mainPack));
        //            break;
        //    }
    //}
    public MainPack SuccessHandle(MainPack mainPack)
        {
            SuccessAction?.Invoke(mainPack);
            return SuccessHandlePack(mainPack);
        }

        protected abstract MainPack SuccessHandlePack(MainPack mainPack);
        public  MainPack FailHandle(MainPack mainPack)
        {
            FailAction?.Invoke(mainPack);
            return FailHandlePack(mainPack);
        }

        protected abstract MainPack FailHandlePack(MainPack mainPack);
    }
}
