using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraCtrl : AutoSingleton<MainCameraCtrl>
{
    //通过键盘控制视角上下移动与旋转
    Vector3 newPos;
    Vector3 newCameraPos;
    Vector3 DragStartPos, DragCurrentPos;
    Vector3 RotateStart, RotateCurrent;
    Quaternion newRotation;
    Transform CameraTrs;
    public float PanSpeed=5;
    public float RotationAmount=0.5f;
    public Vector3 ZoomAmount = new Vector3(0,-1,1);
    public float MoveTime = 5;

    public float scale = 1;
    void KeyBoardCtrl()
    {
        if (Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))
        {
            newPos += transform.forward * Time.deltaTime * PanSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPos -= transform.forward * Time.deltaTime * PanSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPos -= transform.right * Time.deltaTime * PanSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPos += transform.right * Time.deltaTime * PanSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * RotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.down * RotationAmount);
        }
        if (Input.GetKey(KeyCode.R))
        {
            newCameraPos +=ZoomAmount;
        }
        if (Input.GetKey(KeyCode.T))
        {
            newCameraPos -= ZoomAmount;
        }


    }
    void MouseCtrl()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (plane.Raycast(ray, out enter))
            {
                DragStartPos = ray.GetPoint(enter);
            }
        }
        if (Input.GetMouseButton(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (plane.Raycast(ray, out enter))
            {
                DragCurrentPos = ray.GetPoint(enter);
                Vector3 different = DragStartPos - DragCurrentPos;
                newPos = transform.position + different;
            }
        }
        if (Input.GetMouseButtonDown(2)&&Input.GetKey(KeyCode.LeftControl))
        {
            RotateStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(2)&& Input.GetKey(KeyCode.LeftControl))
        {
            RotateCurrent = Input.mousePosition;
            Vector3 different = RotateStart - RotateCurrent;
            RotateStart = RotateCurrent;
            newRotation *= Quaternion.Euler(Vector3.up * -different.x / 20);
        }
        newCameraPos += Input.mouseScrollDelta.y * ZoomAmount;
        scale += Input.mouseScrollDelta.y;
        transform.position = Vector3.Lerp(transform.position, newPos, MoveTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, MoveTime * Time.deltaTime);
        CameraTrs.localPosition = Vector3.Lerp(CameraTrs.localPosition, newCameraPos, MoveTime * Time.deltaTime);
    }
    private void OnEnable()
    {
        newPos = transform.position;
        newRotation = transform.rotation;
        newCameraPos = CameraTrs.localPosition;
    }
    protected  override void Init()
    {
        base.Init();
        CameraTrs = transform.GetComponentInChildren<Camera>().transform;
    }
    void Update()
    {
        MouseCtrl();
        KeyBoardCtrl();
    }
}
