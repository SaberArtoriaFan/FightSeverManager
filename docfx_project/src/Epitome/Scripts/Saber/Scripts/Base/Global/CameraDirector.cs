using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
public class CameraDirector : AutoSingleton<CameraDirector>
{
    public Transform m_Camera=>Camera.main.transform;
    public Transform m_CameraParent=>MainCameraCtrl.instance.transform;
    private bool isDirecting = false;

    private Vector3 camreaPos;
    private Quaternion camreaRotate;

    public float camera_Z = 1;
    public float duration = 1;

    public Vector3 ve;

     float cameraY =-1;
    //public float forward=5;
    public void Start()
    {
        //m_Camera = GetComponent<Camera>();
        //m_Camera = MainCameraCtrl.Instance.transform;
    }
    Bounds CalculateBounds(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        UnityEngine.Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList)
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
    public Vector3 FocusCameraOnGameObject(Camera c, GameObject go)
    {
        Bounds b = CalculateBounds(go);
        Vector3 max = b.size;
        // Get the radius of a sphere circumscribing the bounds
        float radius = max.magnitude / 2f;
        // Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad / 2f) * c.aspect) * Mathf.Rad2Deg;
        // Use the smaller FOV as it limits what would get cut off by the frustum        
        float fov = Mathf.Min(c.fieldOfView, horizontalFOV);
        float dist = radius / (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));
        Debug.Log("Radius = " + radius + " dist = " + dist);
        c.transform.localPosition = new Vector3(c.transform.localPosition.x, c.transform.localPosition.y, -dist);
        if (c.orthographic)
            c.orthographicSize = radius;

        // Frame the object hierarchy
        c.transform.LookAt(b.center);

        var pos = new Vector3(c.transform.localPosition.x, c.transform.localPosition.y, dist);
        return pos;
    }
    public void Focus(GameObject go, float dur)
    {

        MainCameraCtrl.Instance.enabled = false;
        FocusCameraOnGameObject(Camera.main, go);
        MainCameraCtrl.Instance.enabled = true;
    }
    public void Focus(Vector3 targetPos,float dur)
    {

        cameraY = cameraY == -1 ? m_CameraParent.position.y: cameraY;
        //StartCoroutine(IE_Focus(targetPos));
        float dis = cameraY / Mathf.Tan(m_Camera.rotation.eulerAngles.x * Mathf.PI / 180);
        Vector3 off = targetPos + cameraY  /*Vector3.up*/ *m_CameraParent.up - dis /**Vector3.forward*/* m_CameraParent.forward;
        //off = new Vector3(off.x, m_CameraParent.position.y, off.z);
        MainCameraCtrl.Instance.enabled = false;
        m_Camera.transform.localPosition = Vector3.zero;
        //Debug.Log($"x+{m_Camera.rotation.eulerAngles.x},dis+{dis}+\n+off+{off}");
        Debug.Log("0302"+off.x+"."+off.y+"."+off.z);
        m_CameraParent.DOMove(off,dur).OnComplete(() => { MainCameraCtrl.Instance.enabled = true; });
        //看向向量指向的方向

        //angel = Quaternion.LookRotation(player.position - off);
    }
    bool IsFocused(Vector3 targetPos)
    {
        return Vector3.Distance(targetPos, m_CameraParent.position) < 0.5f;
    }
    IEnumerator IE_Focus(Vector3 targetPos)
    {
        MainCameraCtrl.Instance.enabled = false;
        WaitForSeconds waitForSeconds = new WaitForSeconds(Time.deltaTime);

        float dis = m_CameraParent.position.y / Mathf.Tan(90 - m_Camera.rotation.eulerAngles.x);
        Vector3 off = targetPos + m_CameraParent.position.y * m_CameraParent.up - dis * m_CameraParent.forward;
        float timer = 0;
        while (!IsFocused(targetPos)&&timer<1f)
        {
            m_CameraParent.position = Vector3.SmoothDamp(m_CameraParent.position, off, ref ve, 0.3f);
            timer += Time.deltaTime;
            yield return waitForSeconds;
        }
        MainCameraCtrl.Instance.enabled = true;

        //m_CameraParent.position = off;
    }
    [Button]
    public void StartDirector(Vector3 pos, Vector3 dir)
    {
        MainCameraCtrl.Instance.enabled = false;
        //WarChessInputManager.Instance.enabled = false;
        camreaPos= m_Camera.transform.position ;
        camreaRotate= m_Camera.transform.rotation ;
        pos = pos + dir.normalized * camera_Z;
        pos.y += 2;
        m_CameraParent.transform.DOMove(pos, duration);
        m_Camera.transform.DORotate(Vector3.right * 15, duration);
    }
    [Button]
    public void EndDirector()
    {
        MainCameraCtrl.Instance.enabled = true;
        //WarChessInputManager.Instance.enabled = true;

        m_CameraParent.transform.DOMove(camreaPos, duration);
        m_Camera.transform.DORotateQuaternion(camreaRotate, duration);
    }
}
