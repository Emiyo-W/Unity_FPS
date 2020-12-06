using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public float hp = 100; //enemy hp

    public AudioClip damageAudio; //ダメージ音声
    private Animator anim; //animatorコンポーネント
    private HashIDs hash; //HashIDsスクリプト
    private bool isDead = false; //enemyの死亡判定

    void Start()
    {
        //初期化
        anim = this.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
    }

    //enemyが受けるダメージ
    public void TakeDamage(float damage)
    {
        hp -= damage;
        AudioSource.PlayClipAtPoint(damageAudio, transform.position, 1.5f);
        //hpは0より小さい時に死亡判定
        if (hp <= 0 && !isDead)
        {
            isDead = true;
            //コンポーネント全部無効化する
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<EnemyAI>().enabled = false;
            GetComponent<EnemyAnimator>().enabled = false;
            GetComponent<EnemySight>().enabled = false;
            GetComponent<EnemyShoot>().enabled = false;
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            GetComponentInChildren<Light>().enabled = false;
            GetComponentInChildren<LineRenderer>().enabled = false;

            //死亡演出
            anim.SetBool(hash.playerInSightBool, false);
            anim.SetBool(hash.deadBool, true);
        }
    }
}
