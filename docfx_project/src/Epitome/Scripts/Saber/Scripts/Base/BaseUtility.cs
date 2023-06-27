using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//脚本作者:Saber

namespace Saber.Base
{

    public static class BaseUtility 
    {
        private static GraphicRaycaster raycaster=null;
        public static bool IsMouseOnUI()
        {
            List<RaycastResult> allResults = new List<RaycastResult>();
        //�ж�����Ƿ���UI��
            allResults.Clear();
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = Input.mousePosition;
            data.pressPosition = Input.mousePosition;
            //Debug.Log(AllResults.Count);
            if (raycaster == null) raycaster= GraphicRaycaster.FindObjectOfType<GraphicRaycaster>();
            raycaster.Raycast(data, allResults);
            Debug.Log("鼠标在UI上" + allResults.Count);
            return allResults.Count >= 1;
        }
        public static Vector3 MouseToTerrainPosition()
        {
            Vector3 pos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit Hit, 100, LayerMask.GetMask("Terrain")))
            {
                pos = Hit.point;

            }
            return pos;
        }
        public static Vector3[] CalculatePathCrossTwoPoints2D(Vector2 startPos,Vector2 endPos,float arc)
        {
            if (arc == 0) return new Vector3[] { startPos, endPos };
            if (arc > 60) arc = 60;
            Vector3[] result = new Vector3[3];
            result[0] = startPos;
            result[2] = endPos;
            Vector2 dir=endPos - startPos;
            float temp = dir.x;
            dir.x = dir.y;
            dir.y = temp;
            Vector2 midY = (startPos + endPos) / 2;
            float hight;
            if (startPos.x == endPos.x)
            {
                hight=Mathf.Tan(arc * Mathf.PI / 180)*(startPos.y-midY.y);
                //if()
                midY.y += hight;
                result[1] = midY;
                return result;
            }
           
            hight = Mathf.Tan(arc * Mathf.PI / 180) *Vector2.Distance(midY,startPos);
            midY += hight * dir.normalized;
            result[1] = midY;
            return result;

        }
        public static Vector3[] CalculatePathCrossTwoPoints(Vector3 startPos, Vector3 endPos, float arc)
        {
            if (arc == 0) return new Vector3[] { startPos, endPos };
            if (arc > 60) arc = 60;
            Vector3 high_d;
            bool isChanged = false;
            if (startPos.y < endPos.y)
            {
                high_d = startPos;
                startPos = endPos;
                endPos = high_d;
                isChanged = true;
            }//让startPos是高的那边
            high_d = new Vector3(startPos.x, endPos.y, startPos.z);
            float d = (startPos.y - endPos.y) * Mathf.Tan(arc * Mathf.PI / 180);
            float per = d / Vector3.Distance(high_d, endPos) + 1;
            float x = endPos.x + (high_d.x - endPos.x) * per;
            float z = endPos.z + (high_d.z - endPos.z) * per;
            high_d = new Vector3((x + endPos.x) / 2, endPos.y, (z + endPos.z) / 2);
            d = Vector3.Distance(high_d, endPos);
            high_d.y += d * Mathf.Tan((90 - arc) * Mathf.PI / 180);
            if(isChanged)
            {
                Vector3 tem = startPos;
                startPos = endPos;
                endPos = tem;
            }
            return new Vector3[3] { startPos, high_d, endPos };
        }
        public static GameObject LoadGameObjectFromResources(string path)
        {
            Object temobj = Resources.Load(path);
            GameObject obj = GameObject.Instantiate(temobj) as GameObject;
            if (obj != null)
            {
                obj.name = obj.name.Replace("(Clone)", "");
                obj.transform.rotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                return obj;
            }
            else
            {
                Debug.LogError("Path error!");
                return null;
            }
        }

        public static void ShowRisingSpaceInScreen(string s, Vector3 worldPos, Vector3 dir, float speed=1, float continueTime=1.5f)
        {
            if (UIManager.Instance == null) return;
            RisingSpacePanelUI risingSpacePanelUI = UIManager.GetPanel<RisingSpacePanelUI>();
            if (risingSpacePanelUI != null)
                risingSpacePanelUI.ShowRisingSpace(s, worldPos, dir, speed, continueTime);
        }
        public static void DebugError(string s="")
        {
            Debug.LogError($"此处出错error{s}");
        }
        public static bool IsFaceTo(Vector3 forward, Vector3 dir, float limit =0)
        {
            //60度正前方
            float x = Vector3.Dot(forward.normalized, dir.normalized);
            //Debug.Log(x + "asas");
            return x >= limit;
        }
        public static void SetGameObjectLayerContainChildren(GameObject go, int layer)
        {
            if (go == null) return;
            foreach(var v in go.GetComponentsInChildren<Transform>())
            {
                v.gameObject.layer = layer;
            }
        }

        public static Canvas GetMainCanvas()
        {
            Canvas mianCanvas = null;
            Canvas[] canvas = GameObject.FindObjectsOfType<Canvas>();
            foreach (Canvas c in canvas)
            {
                if (c.tag == "MainCanvas")
                {
                    mianCanvas = c;
                    break;
                }
            }
            return mianCanvas;
        }
        //public static bool IsFaceTo(Vector3 forward, Vector3 dir, float limit = 0)
        //{
        //    //60度正前方
        //    float x = Vector3.Dot(forward.normalized, dir.normalized);
        //    //Debug.Log(x + "asas");
        //    return x >= limit;
        //}

    }
}
