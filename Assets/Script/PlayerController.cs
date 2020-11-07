using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//列挙型 固定値を扱う 特定した値しかない　例えば：曜日
//ユーザの状態をnone,Idle,Walk,Run,Crouch五つあると想定、定義
//する. これらの状態を用いて、ユーザの各イベントを設計する
public enum PlayerState
{
    None,
    Idle,
    Walk,
    Run,
    Crouch,

}
public class PlayerController : MonoBehaviour
{
    private PlayerState state = PlayerState.None;
    //今の状態を返す
    public PlayerState State
    {
        get
        {
            if (walking)
                return PlayerState.Walk;
            else if (runing)
                return PlayerState.Run;
            else if (crouching)
                return PlayerState.Crouch;
            else
                return PlayerState.Idle;
        }
    }


    public float walkSpeed = 7.0f; //歩行スピード
    public float walkJumpSpeed = 6.0f;//歩行ジャンプスピード
    public float sprintSpeed = 10.0f; //走るスピード
    public float sprintJumpSpeed = 8.0f; //走るジャンプスピード
    public float crouchSpeed = 4.0f; //しゃがむスピード
    public float crouchJumpSpeed = 3.0f; //しゃがむジャンプスピード
    public float crouchDetalHeight = 0.5f; //しゃがむ高さ

    private float gravity = 20f; //重力
    public float cameraMoveSpeed = 8.0f; //カメラ移動スピード
    public AudioClip jumpAudio; //ジャンプ音声

    private Transform mainCamera; //mainCameraコンポーネント
    private PlayerParameter parameter; //PlayerParameterスクリプト（入力値）
    private AudioSource audioSource; //AudioSourceコンポーネント
    private CharacterController c_Controller; //CharacterControllerコンポーネント

    private float speed; //現在のスピード
    private float jumpSpeed; //現在のジャンプスピード
    private bool walking = false; //walk状態初期化
    private bool runing = false; //run状態初期化
    private bool crouching = false;//crouch状態初期化

    private bool grounded = false; //地面にいるか
    private float standardCameraHeight; //立っている時のカメラ高度
    private float crouchingCameraHeight; //しゃがんでいる時のカメラ高度
    private float normalControllerHeight = 0.0f; // ユーザのあたり判定の高さ
    private Vector3 normalControllerCenter = Vector3.zero; //ユーザのあたり判定の中心点
    private Vector3 moveDirection = Vector3.zero; //ユーザの移動方向の初期化


    void Start()
    {
        //初期化
        walking = false;
        crouching = false;
        crouching = false;
        speed = walkSpeed;
        jumpSpeed = walkJumpSpeed;
        mainCamera = GameObject.FindGameObjectWithTag(Tags.mainCamera).transform;
        parameter = this.GetComponent<PlayerParameter>();
        audioSource = this.GetComponent<AudioSource>();
        c_Controller = this.GetComponent<CharacterController>();
        standardCameraHeight = mainCamera.localPosition.y;
        crouchingCameraHeight = standardCameraHeight - crouchDetalHeight;
        normalControllerHeight = c_Controller.height;
        normalControllerCenter = c_Controller.center;

    }


 
    private void FixedUpdate()
    {
        UpdateMove();
        AudioManagment();
        EscApp();

    }

    //ユーザの移動
    private void UpdateMove()
    {
        //地面にいる
        if (grounded)
        {
            //inputMoveVectorのx,y軸の値をx、z代入 ,高度yは0
            moveDirection = new Vector3(parameter.inputMoveVector.x, 0, parameter.inputMoveVector.y); //ローカル空間からワールドに変更
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed; //方向ベクトル*スピード　移動距離

            //ジャンプ
            if (parameter.inputJump)
            {
                //y軸にジャンプスピード（高度）
                moveDirection.y = jumpSpeed;
                //ジャンプ音声
                AudioSource.PlayClipAtPoint(jumpAudio, transform.position);
                //スピード変更
                CurrentSpeeed();
            }
        }

        //重力があるため,ユーザは時間とともに地面に落下する
        moveDirection.y -= gravity * Time.deltaTime;

        //c_Controller.Move CharacterControllerを利用して移動する
        CollisionFlags flags = c_Controller.Move(moveDirection * Time.deltaTime);
        //flags & CollisionFlags.CollidedSides 下のみ衝突検出　返す値は1(true)
        grounded = (flags & CollisionFlags.CollidedBelow) != 0;

        //Mathf.Abs 絶対値を返す　xやz軸の入力があり、ユーザは地面にいる
        if (Mathf.Abs(moveDirection.x) > 0 && grounded || Mathf.Abs(moveDirection.z) > 0 && grounded)
        {
            //走っているか
            if (parameter.inputSprint)
            {
                runing = true;
                walking = false;
                crouching = false;
            }
            else if (parameter.inputCrouch)
            {
                //歩行
                crouching = true;
                walking = false;
                runing = false;
            }
            else
            {
                //しゃがむ
                walking = true;
                crouching = false;
                runing = false;
            }
        }
        else
        {
            //移動に関する入力がない
            if (walking)
               walking = false;
            if (runing)
                runing = false;
            //しゃがんでいるか
            if (parameter.inputCrouch)
                crouching = true;
            else
                crouching = false;

        }

        //しゃがむ
        if (crouching)
        {
            c_Controller.height = normalControllerHeight - crouchDetalHeight;
            c_Controller.center = normalControllerCenter-new Vector3(0,crouchDetalHeight / 2,0);
        }
        else
        {
            //立っている状態に戻す
            c_Controller.height = normalControllerHeight;
            c_Controller.center = normalControllerCenter;

        }
        CurrentSpeeed();
        UpadateCrouch();
    }

    //足音の速さ
    private void AudioManagment()
    {
        //歩く時の速さ
        if (State == PlayerState.Walk)
        {
            audioSource.pitch = 1.0f;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (State == PlayerState.Run)
        {
            //走っている時の速さ
            audioSource.pitch = 1.3f;
            if (!audioSource.isPlaying)
                audioSource.Play();

        }
        else
            audioSource.Stop();

    }


    //歩行スピードとジャンプスピードをユーザ状態に応じて変更
    private void CurrentSpeeed()
    {
        //4ステージ
        switch (State)
        {
            case PlayerState.Walk:
                speed = walkSpeed;
                jumpSpeed = walkJumpSpeed;
                break;
            case PlayerState.Idle:
                speed = walkSpeed;
                jumpSpeed = walkJumpSpeed;
                break;
            case PlayerState.Run:
                speed = sprintSpeed;
                jumpSpeed = sprintJumpSpeed;
                break;
            case PlayerState.Crouch:
                speed = crouchSpeed;
                jumpSpeed = crouchJumpSpeed;
                break;
        }
    }


    //しゃがむの処理
    //フレームを注意した処理
    private void UpadateCrouch()
    {
        Debug.Log(mainCamera.localPosition.y);


        if (crouching)
        {
            //しゃがんでいる
            if (mainCamera.localPosition.y > crouchingCameraHeight)
            {
                //カメラ位置調整
                //スムーズに行うために、　まず現在の高度　ー　次のフレームで減少する高度　規定高度より小さければ　規定高度
                //高ければそのまま減少
                if (mainCamera.localPosition.y - crouchDetalHeight * Time.deltaTime * cameraMoveSpeed < crouchingCameraHeight)
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchingCameraHeight, mainCamera.localPosition.z);
                else
                    mainCamera.localPosition -= new Vector3(0, crouchDetalHeight * Time.deltaTime * cameraMoveSpeed, 0);
            }
            else
                mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchingCameraHeight, mainCamera.localPosition.z);
        }
        else
        {
            //戻る時は同じ
            if (mainCamera.localPosition.y < standardCameraHeight)
            {
                if (mainCamera.localPosition.y + crouchDetalHeight * Time.deltaTime * cameraMoveSpeed > standardCameraHeight)
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCameraHeight, mainCamera.localPosition.z);
                else
                    mainCamera.localPosition += new Vector3(0, crouchDetalHeight * Time.deltaTime * cameraMoveSpeed, 0);
            }
            else
                mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCameraHeight, mainCamera.localPosition.z);
        }
    }

    //ゲーム終了
    private void EscApp()
    {
        
     if(parameter.inputEscape)
                Application.Quit();
  
           
    }

}
