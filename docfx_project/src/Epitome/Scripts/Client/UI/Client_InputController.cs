using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    public class Client_InputController : Client_SingletonBase<Client_InputController>
    {
        //手指第一次触摸点的位置
        [SerializeField]
        Vector2 widthLimit = new Vector2(-18f, 9.4f);
        [SerializeField]
        Vector2 hightLimit = new Vector2(-5.8f, 4.2f);
        [SerializeField]
        Vector2 m_scenePos = new Vector2();
        Vector2 prePos;

        [SerializeField]
        Vector2 backGroundX;
        [SerializeField]
        Vector2 backGroundY;
        //摄像机
        Camera mainCamera;
        public Transform cameraTarget;
        BoxCollider2D backGroundBox;
        public float sliderPara = 0.5f;
        public float y_offest=-5;
        int dir = -1;//0左1右2上3下
        bool isDirector = false;

        Tween tween;
        public Vector2 WidthLimit { get => widthLimit;  }
        public Vector2 HightLimit { get => hightLimit;  }
        public Vector2 BackGroundX { get => backGroundX; }
        public Vector2 BackGroundY { get => backGroundY; }

        protected override void Start()
        {
            base.Start();
            //允许多点触摸
            Input.multiTouchEnabled = true;
            mainCamera = Camera.main;
            cameraTarget = Camera.main.transform;
            isDirector = true;

            Vector3 origin = mainCamera.ScreenToWorldPoint(Vector3.zero);
            float cameraX = Mathf.Abs(origin.x - (mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x));
            float cameraY = Mathf.Abs(origin.y - (mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y));


            TimerManager.Instance.AddTimer(() =>
            {
                ScanBackGround(Client_InstanceFinder.GetInstance<Client_Background>()?.BackGround?.gameObject);
                widthLimit.x = backGroundX.x + cameraX / 2;
                widthLimit.y = backGroundX.y - cameraX / 2;
                hightLimit.x = backGroundY.x + cameraY / 2;
                hightLimit.y = backGroundY.y - cameraY / 2;

                Vector3 pos = Vector3.right * WidthLimit.x + Vector3.up * ((HightLimit.x + HightLimit.y) / 2 + y_offest) + cameraTarget.transform.position.z * Vector3.forward;
                GetPosInLimit(ref pos);
                cameraTarget.transform.position = pos;
            }, Time.deltaTime);
        }


        void Update()
        {
            //DesktopInput();
            if (!isDirector)
                MobileInput();
        }
        Vector2[] GetCornersForBoxCollider(BoxCollider2D b)
        {
            Vector2[] verts = new Vector2[4];
            //BoxCollider b = obj.GetComponent<BoxCollider>(); //retrieves the Box Collider of the GameObject called obj
            //verts[0] = b.gameObject.transform.TransformPoint(b.offset + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);
            //verts[1] = b.gameObject.transform.TransformPoint(b.offset + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
            verts[0] = b.gameObject.transform.TransformPoint(b.offset + new Vector2(b.size.x, -b.size.y) * 0.5f);
            verts[1] = b.gameObject.transform.TransformPoint(b.offset + new Vector2(-b.size.x, -b.size.y) * 0.5f);
            verts[2] = b.gameObject.transform.TransformPoint(b.offset + new Vector2(-b.size.x, b.size.y) * 0.5f);
            verts[3] = b.gameObject.transform.TransformPoint(b.offset + new Vector2(b.size.x, b.size.y) * 0.5f);
            //verts[6] = b.gameObject.transform.TransformPoint(b.offset + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
            //verts[7] = b.gameObject.transform.TransformPoint(b.offset + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);
            return verts;
        }
        [Button]
        private void ScanBackGround(GameObject go)
        {
            BoxCollider2D collider2D = go.GetComponentInChildren<BoxCollider2D>() == null ? go.AddComponent<BoxCollider2D>() : go.GetComponentInChildren<BoxCollider2D>();
            backGroundX = new Vector2(float.MaxValue, float.MinValue);
            backGroundY = new Vector2(float.MaxValue, float.MinValue);

            foreach (var v in GetCornersForBoxCollider(collider2D))
            {
                backGroundX.x = Mathf.Min(backGroundX.x, v.x);
                backGroundX.y = Mathf.Max(backGroundX.y, v.x);
                backGroundY.x = Mathf.Min(backGroundY.x, v.y);
                backGroundY.y = Mathf.Max(backGroundY.y, v.y);
            }
            backGroundBox = collider2D;
            //DestroyImmediate(collider2D);
        }

        public static void StartDirector(Vector3 startPos, Vector3 endPos, float time,bool canMove,Action action=null)
        {
            Client_InputController client_InputController = Client_InstanceFinder.GetInstance<Client_InputController>();
            if (client_InputController == null) Debug.LogError("未找到输入管理器");
            client_InputController.GetPosInLimit(ref endPos);
            client_InputController.GetPosInLimit(ref startPos);

            Camera camera = Camera.main;
            client_InputController.cameraTarget.transform.position = startPos;
            client_InputController.isDirector = true;
            Debug.Log("摄像机移动");
            if(canMove)
                client_InputController.tween= client_InputController.cameraTarget.transform.DOMove(endPos, time).OnComplete( () => { action?.Invoke(); client_InputController.isDirector = !canMove; });
                //client_InputController.tween =client_InputController.CameraFocusAt(endPos,time, () => { action?.Invoke(); client_InputController.isDirector = !canMove; });
            else
                _ = client_InputController.cameraTarget.transform.DOMove(endPos, time).OnComplete(() =>
                {
                    action?.Invoke();
                    client_InputController.isDirector = !canMove;
                    if (client_InputController.tween != null && !client_InputController.tween.IsComplete())
                        client_InputController.DOKill();
                    client_InputController.tween = null;
                });
            //_ = client_InputController.CameraFocusAt(endPos, time, () => {
            //        action?.Invoke();
            //        client_InputController.isDirector = !canMove;
            //        if (client_InputController.tween != null && !client_InputController.tween.IsComplete())
            //            client_InputController.DOKill();
            //        client_InputController.tween = null;
            //    });
        }


        //聚焦对象
            private Tween CameraFocusAt(Vector3 pos,float time,Action action)
        {
            var cp = CalcScreenCenterPosOnPanel();
            var tp = pos;
            //1.直接移动
            // mainCamera.transform.Translate(tp - cp,Space.World);
            //2.使用tween移动
            return mainCamera.transform.DOMove(mainCamera.transform.position + (tp - cp), time).OnComplete(()=>action?.Invoke());
        }


        /// <summary>
        /// 屏幕中心点到panel上的坐标
        /// </summary>
        /// <returns></returns>
        private Vector3 CalcScreenCenterPosOnPanel()
        {
            var ray = mainCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2, (float)Screen.height / 2, 0));
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance);
            }
            else
            {
                return Vector3.zero;
            }
        }

            private void SetDir(int i)
        {
            if (dir == -1)
                dir = i;
        }
        private void GetPosInLimit(ref Vector3 target)
        {
            if (target.x < widthLimit.x) target.x = widthLimit.x;
            else if (target.x > widthLimit.y) target.x = widthLimit.y;
            if (target.y < hightLimit.x) target.y = hightLimit.x;
            else if (target.y > hightLimit.y) target.y = hightLimit.y;
        }
        private void GetPosInLimit(ref float target, bool isLeftRight)
        {
            if (isLeftRight)
            {
                if (target > widthLimit.y) target = widthLimit.y;
                else if (target < widthLimit.x) target = widthLimit.x;
            }
            else
            {
                if (target > hightLimit.y) target = hightLimit.y;
                else if (target < hightLimit.x) target = hightLimit.x;
            }
        }
        //移动端控制摄像机旋转
        private void MobileInput()
        {
            if (Input.touchCount == 0)
                return;

            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    dir = -1;
                    m_scenePos = Input.touches[0].position;
                    prePos = m_scenePos;
                }
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    ////旋转摄像机
                    //cameraTarget.Rotate(new Vector3(-Input.touches[0].deltaPosition.y, Input.touches[0].deltaPosition.x, 0), Space.Self);
                    Vector2 pos = Input.touches[0].position;
                    //sliderForce -= Time.deltaTime;

                    //判断手指移动
                    //水平移动
                    if (dir == 0 || dir == 1 || (dir == -1 && Mathf.Abs(prePos.x - pos.x) > Mathf.Abs(prePos.y - pos.y)))
                    {
                        if (prePos.x > pos.x)
                        {
                            //Debug.Log("手指向右滑");
                            SetDir(0);
                            if (dir == 1)
                                m_scenePos = prePos;
                            //TODO:...
                        }
                        else
                        {
                            //Debug.Log("手指左滑");
                            //TODO:...
                            SetDir(1);
                            if (dir == 0)
                                m_scenePos = prePos;
                        }
                        float i = (pos.x - m_scenePos.x) * sliderPara / 100;
                        i += cameraTarget.transform.position.x;
                        GetPosInLimit(ref i, true);
                        i -= cameraTarget.transform.position.x;
                        cameraTarget.transform.Translate(i * Vector3.right);
                    }
                    else
                    {
                        if (prePos.y > pos.y)
                        {
                            //Debug.Log("手指上滑");
                            SetDir(3);
                            if (dir == 2)
                                m_scenePos = prePos;
                            //TODO:...
                        }
                        else
                        {
                            // Debug.Log("手指下滑");

                            SetDir(2);
                            if (dir == 3)
                                m_scenePos = prePos;
                            //TODO:...
                        }
                        float i = (pos.y - m_scenePos.y) * sliderPara / 100;
                        //Debug.Log(i);
                        i += cameraTarget.transform.position.y;
                        //Debug.Log(i);

                        GetPosInLimit(ref i, false);

                        i -= cameraTarget.transform.position.y;
                        //Debug.Log(i);

                        //Vector3 v = Vector3.down * (m_scenePos.y - pos.y) * sliderPara / 100;
                        //Debug.Log(v);
                        //GetPosInLimit(ref v);
                        //Debug.Log(v);
                        cameraTarget.transform.Translate(i * Vector3.up);
                    }
                    prePos = pos;
                }

                //if (Input.touches[0].phase == TouchPhase.Ended && Input.touches[0].phase != TouchPhase.Canceled)
                //{
                //    Vector2 pos = Input.touches[0].position;

                //    //判断手指移动
                //    //水平移动
                //    if (Mathf.Abs(m_scenePos.x - pos.x) > Mathf.Abs(m_scenePos.y - pos.y))
                //    {
                //        if (m_scenePos.x > pos.x)
                //        {
                //            Debug.Log("手指向左滑");
                //            //TODO:...
                //        }
                //        else
                //        {
                //            Debug.Log("手指右滑");
                //            //TODO:...
                //        }
                //        cameraTarget.transform.Translate(Vector3.left * (m_scenePos.x - pos.x)*sliderPara/100);
                //    }
                //    else
                //    {
                //        if (m_scenePos.y > pos.y)
                //        {
                //            Debug.Log("手指下滑");
                //            //TODO:...
                //        }
                //        else
                //        {
                //            Debug.Log("手指上滑");
                //            //TODO:...
                //        }
                //        cameraTarget.transform.Translate(Vector3.down* (m_scenePos.y - pos.y) * sliderPara/100);
                //    }
                //}
            }//多指逻辑
            //else if (Input.touchCount > 1)
            //{
            //    //记录两个手指的位置
            //    Vector2 finger1 = new Vector2();
            //    Vector2 finger2 = new Vector2();

            //    //记录两个手指的移动
            //    Vector2 mov1 = new Vector2();
            //    Vector2 mov2 = new Vector2();

            //    for (int i = 0; i < 2; i++)
            //    {
            //        Touch touch = Input.touches[i];

            //        if (touch.phase == TouchPhase.Ended)
            //            break;

            //        if (touch.phase == TouchPhase.Moved)
            //        {
            //            float mov = 0;
            //            if (i == 0)
            //            {
            //                finger1 = touch.position;
            //                mov1 = touch.deltaPosition;
            //            }
            //            else
            //            {
            //                finger2 = touch.position;
            //                mov2 = touch.deltaPosition;

            //                if (finger1.x > finger2.x)
            //                {
            //                    mov = mov1.x;
            //                }
            //                else
            //                {
            //                    mov = mov2.x;
            //                }

            //                if (finger1.y > finger2.y)
            //                {
            //                    mov += mov1.y;
            //                }
            //                else
            //                {
            //                    mov += mov2.y;
            //                }
            //                cameraTarget.transform.Translate(0, 0, mov * 0.1f);
            //            }
            //        }

            //    }
            //}
        }
    }
}
