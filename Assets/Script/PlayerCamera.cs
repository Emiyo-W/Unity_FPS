using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


//実行開始後、自動的にカメラにアタッチする
[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    //mouseのスピード
    public Vector2 mouseLookSensitivity = new Vector2(5, 5);
    //回転の範囲
    public Vector2 rotationXLimit = new Vector2(87, -87);
    public Vector2 rotationYLimit = new Vector2(-360, 360);
    //カメラの黙認位置
    public Vector3 positionOffset = new Vector3(0, 1, -0.2f);

    //mouse現在座標の初期化
    private Vector2 currentMouseLook = Vector2.zero;
    //縦の回転
    private float x_Angle = 0;
    //横の回転
    private float y_Angle = 0;
    //mouse入力の取得
    private PlayerParameter parameter;

    private Transform m_Transform;

    void Start()
    {
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerParameter>();
        m_Transform = transform;
        //相対位置を初期化（誤操作防止）
        m_Transform.localPosition = positionOffset;
    }

    void Update()
    {
        UpdateInput();
    }

    void LateUpdate()
    {
        //プレイヤー左右の回転
        Quaternion xQuaternion = Quaternion.AngleAxis(y_Angle, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(0, Vector3.left);
        m_Transform.parent.rotation = xQuaternion * yQuaternion;
        

        //カメラー上下の回転
        yQuaternion = Quaternion.AngleAxis(x_Angle, Vector3.left);
        m_Transform.rotation = xQuaternion * yQuaternion;
        
    }

    private void UpdateInput()
    {
        if (parameter.inputSmoothLook == Vector2.zero)
            return;
        GetMouseLook();
        //mouseのｘ軸をプレイヤーのy軸に代入
        y_Angle += currentMouseLook.x;
        //mouseのy軸をカメラのx軸に代入
        x_Angle += currentMouseLook.y;
       

        y_Angle = y_Angle < -360 ? y_Angle += 360 : y_Angle;
        y_Angle = y_Angle > 360 ? y_Angle -= 360 : y_Angle;
        y_Angle = Mathf.Clamp(y_Angle, rotationYLimit.x, rotationYLimit.y);

        x_Angle = x_Angle < -360 ? x_Angle += 360 : x_Angle;
        x_Angle = x_Angle > 360 ? x_Angle -= 360 : x_Angle;
        x_Angle = Mathf.Clamp(x_Angle, -rotationXLimit.x, -rotationXLimit.y);
    }

    private void GetMouseLook()
    {
        //mouse横軸の入力
        currentMouseLook.x = parameter.inputSmoothLook.x;
        //mouse縦軸の入力
        currentMouseLook.y = parameter.inputSmoothLook.y;

        currentMouseLook.x *= mouseLookSensitivity.x;
        currentMouseLook.y *= mouseLookSensitivity.y;



       


    }
}