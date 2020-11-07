using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    public float deadZone = 5f;　//
    private Transform player; //ユーザのTransformコンポーネント
    private EnemySight enemySight; //EnemySightスクリプト
    private NavMeshAgent nav; //NavMeshAgentコンポーネント
    private Animator anim; //animatorコンポーネント
    private HashIDs hash; //HashIDsスクリプト
    private AnimatorSetup animSetup; //animatorSetupスクリプト

    void Start()
    {
        //初期化
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        enemySight = this.GetComponent<EnemySight>();
        nav = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        animSetup = new AnimatorSetup(anim, hash);

        //navの回転を無効・animatorが回転をする（スムーズに見える）
        nav.updateRotation = false;
        //Shooting(1)とGun(2)のWeightを1に設定、他の層より優先順位が高い
        anim.SetLayerWeight(1, 1f);
        anim.SetLayerWeight(2, 1f);
        //弧度に変更
        deadZone *= Mathf.Deg2Rad;
    }

    void Update()
    {
        
        NavAnimSetup();
    }


    //enemyのスピードと回転を設定
    void OnAnimatorMove()
    {
        //navによるスピードを前のフレームと同じにする
        nav.velocity = anim.deltaPosition / Time.deltaTime;
        //animatorが回転を制御
        transform.rotation = anim.rootRotation;
    }

    void NavAnimSetup()
    {
        float speed;
        float angle;

        //enemyがユーザを見つけたら
        if (enemySight.playerInSight)
        {
            //enemyga停止し,ユーザとの角度を計算する
            speed = 0;
            angle = FindAngle(transform.forward, player.position - transform.position, transform.up);
        }
        else
        {
            //enemyがユーザを見つけていない、
            //ベクトルの投影でスピードを決める（間違った方向に行かないため）
            //magnitudeはベクトルの長さ
            speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;//desiredVelocityは回避行動による潜在的な移動も含むエージェントの期待速度（読み取り専用）
            //向かう先とforwardの角度を計算
            angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);

            //enemyの回転に余裕の時間を設定したため、enemyは正しい方向までに回転後、止まらずさらに回転する
            //結果enmeyは蛇行する
            //これを防ぐため,enemyの顔向きと正しい方向間の角度がdeazoneより小さい時、navで回転をコントロール
            if (Mathf.Abs(angle) < deadZone)
            {
                //enemyの前進方向をnavでコントロール（animatorではない）
                transform.LookAt(transform.position + nav.desiredVelocity);
                //角度を0
                angle = 0;
            }
        }
        //speedとangleをanimSetupに渡す
        animSetup.Setup(speed, angle);
    }


    private float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        //navのスピードベクトルが0の時,0を返す
        if (toVector == Vector3.zero)
            return 0f;

        //角度の絶対値
        float angle = Vector3.Angle(fromVector, toVector);
        //スピードベクトルはenemyの左か右か（左はマイナス　右はプラス）
        Vector3 normal = Vector3.Cross(fromVector, toVector);　//Vector3.Cross外積
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));// Vector3.Dot 内積　Mathf.Signはfloatの符号を返す
        //角度を弧度に変化
        angle *= Mathf.Deg2Rad;

        return angle;

    }
}
