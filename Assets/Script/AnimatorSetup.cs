using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetup 
{
    public float speedDampTime = 0.1f;    //余裕speed
    public float angularSpeedDampTime = 0.7f; //yoyuu 回転speed
    public float angleResponseTime = 1f; //回転時間

    private Animator anim;
    private HashIDs hash;

    //コンストラクタ
    public AnimatorSetup(Animator anim,HashIDs hash)
    {
        this.anim = anim;
        this.hash = hash;
    }

    //animatasetupのパラメータ
    public void Setup(float speed,float angle)
    {
        //回転角度/回転時間
        float angularSpeed = angle / angleResponseTime;
        
        anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
        anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);

    }
}
