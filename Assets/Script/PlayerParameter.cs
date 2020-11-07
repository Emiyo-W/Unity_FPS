using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerParameter : MonoBehaviour
{
    //カメラ、プレイヤー GUNそれぞれ違う変数にアクセスするため,それぞれ内部で定義するより
    //まとめて値を与える　必要に応じてアクセスするため
    [HideInInspector]
    public Vector2 inputSmoothLook;
    [HideInInspector]
    public Vector2 inputMoveVector;
    [HideInInspector]
    public bool inputCrouch;
    [HideInInspector]
    public bool inputJump;
    [HideInInspector]
    public bool inputFire;
    [HideInInspector]
    public bool inputReload;
    [HideInInspector]
    public bool inputSprint;
    [HideInInspector]
    public bool inputEscape;
    [HideInInspector]
    public bool inputGrenade;

}
