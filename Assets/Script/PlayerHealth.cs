using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{

    public bool isDead; //ユーザの死亡状態（bool）
    public float resetAfterDeathTine = 5f; //死亡後のリセット時間
    public float maxHp = 100; //ユーザの最大hp
    public float hp = 100; //ユーザのリアルタイムhp
    public float recoverSpeed = 1;  //hpの自動回復
    public AudioClip deathClip; //死亡音声
    public AudioClip damageClip; //ダメージを受けた音声

    private float timer = 0;
    private FadeInOut fader; //FadeInOutスクリプト


    void Start()
    {
        //初期化
        hp = maxHp;
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        BleedBehavior.BloodAmount = 0;
    }

    void Update()
    {
        //ユーザ死亡していない
        if (!isDead)
        {
            //hpは自動回復
            hp += recoverSpeed * Time.deltaTime;
            if (hp > maxHp)
                hp = maxHp;
        }
        //hp<0の時
        if (hp < 0)
        {
            //isDeadをtrue シーンを読み込む
            if (!isDead)
                PlayerDead();
            else
                LeverReset();
        }
     }
    //受けたダメージ　，外部で呼び出される
    public void TakeDamage(float damage)
    {
        //ユーザが死んでいるなら,それ以上ダメージを受けない
        if (isDead)
            return;
        //ダメージを受けている音声を流す
        AudioSource.PlayClipAtPoint(damageClip, transform.position);
        //受けたダメージの量によって画面に血のエフェクトの濃さを調整する
        BleedBehavior.BloodAmount += Mathf.Clamp01(damage / hp);
        //hpに反映する
        hp -= damage;
    }

    //死亡時入力やオブジェクトなどを無効にする
    public void DisableInput()
    {
        transform.Find("MainCamera/Weapon_Camera").gameObject.SetActive(false);
        this.GetComponent<AudioSource>().enabled = false;
        this.GetComponent<PlayerController>().enabled = false;
        this.GetComponent<FPInput>().enabled = false;
        if (GameObject.Find("Canvas") != null)
            GameObject.Find("Canvas").SetActive(false);
        if (transform.Find("MainCamera").GetComponent<PlayerCamera>().enabled)
            transform.Find("MainCamera").GetComponent<PlayerCamera>().enabled = false;
    }

    public void PlayerDead()
    {
        isDead = true;
        //入力を無効
        DisableInput();
        //死亡音声を流す
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }

    //ほかのシーンを読み込む
    public void LeverReset()
    {
        timer += Time.deltaTime;
        if (timer >= resetAfterDeathTine)
            fader.EndScene(); // シーンをリセット

    }
}
