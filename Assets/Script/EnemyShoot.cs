using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public float maximumDamage = 200;     //最大ダメージ
    public float minimumDamage = 30;      //最小ダメージ
    public AudioClip shotClip;            //射撃音
    public float flashIntensity = 3f;     //射撃エフェクトの強度
    public float fadeSpeed = 8f;          //銃口光強度変化スピード


    private Animator anim;                  //animatorコンポーネント
    private HashIDs hash;                   //HashIDsスクリプト
    private LineRenderer laserShotLine;     //射撃線のコンポーネント
    private Light laserShotLight;           //銃口の光コンポーネント
    private SphereCollider col;             //ユーザの位置を判断する球コンポーネント         
    private Transform player;               //ユーザ座標
    private PlayerHealth playerHealth;      //PlayerHealthスクリプト
    private bool shooting;                  //射撃状態
    private float scaledDamage;             //ダメージ範囲

    void Start()
    {
        //GetComponent
        anim = this.GetComponent<Animator>();
        laserShotLine = this.GetComponentInChildren<LineRenderer>();
        laserShotLight = laserShotLine.gameObject.GetComponent<Light>();

        col = GetComponentInChildren<SphereCollider>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        //射撃線を無効・強度0
        laserShotLine.enabled = false;
        laserShotLight.intensity = 0;
        //ダメージ範囲 ＝ 最大ダメージ - 最小ダメージ
        scaledDamage = maximumDamage - minimumDamage;
    }

    void Update()
    {
        //shotに該当するアニメのshot Curveを取得
        float shot = anim.GetFloat(hash.shotFloat);

        //shotが0.5より大き、射撃していない場合関数Shoot()を呼び出す
        if (shot > 0.5f && !shooting)
            Shoot();
        //shotがo.5より小さい時、射撃終了（射撃していない）
        if (shot < 0.5f)
        {
            shooting = false;
            laserShotLine.enabled = false;
        }

        //銃口の光強度を0に変化させる
        laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);
    }


    //ユーザに向かって射撃する
    void OnAnimatorIK(int layerIndex)
    {
        //aimWeight Curveを取得(手を上げると手を下げるアニメ)
        float aimWeight = anim.GetFloat(hash.aimWeightFloat);
        //ユーザの座標に向かって
        anim.SetIKPosition(AvatarIKGoal.RightHand, player.position + Vector3.up * 1.5f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
    }

    //射撃
    private void Shoot()
    {
        //射撃状態をtrue
        shooting = true;
        //ユーザとの距離でダメージ値が変動する（近いほどダメージが高い「最大・最小値あり」）
        float fractionalDistance = ((col.radius - Vector3.Distance(transform.position, player.position)) / col.radius);
        float damage = scaledDamage * fractionalDistance + minimumDamage;
        //damge値をPlayerHealthに渡す
        playerHealth.TakeDamage(damage);
        //ShotEffectsを呼び出す
        ShotEffects();
    }

    //射撃のエフェクト
    private void ShotEffects()
    {
        //射撃線の起点
        laserShotLine.SetPosition(0, laserShotLine.transform.position);
        //射撃線の終点
        laserShotLine.SetPosition(1, player.position + Vector3.up * 1.5f);
        //射撃線を有効
        laserShotLine.enabled = true;
        //光の強度を設定
        laserShotLight.intensity = flashIntensity;
        //射撃音をenemyの座標で再生
        AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);
    }
}
