﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void PlayerShoot();
public class GunFire : MonoBehaviour
{
    public static event PlayerShoot PlayerShootEvent;
    
    public Text bulletText; //弾のUIテキスト

    //各種アニメのstring値保存用
    private string reloadAnim = "Reload";
    private string fireAnim = "Single_Shot";
    private string walkAnim = "Walk";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string idleAnim = "Idle";

    private Animation anim; //animation コンポーネント
    private float nextFireTime = 0.0f; //射撃間隔
    private MeshRenderer flash; //エフェクト
    private int currentBullet; //現在の弾数
    private int currentChargerBullet; //現在の予備弾数
    private PlayerParameter parameter; //playerParameterスクリプト
    private PlayerController playerControl; // PlayerControllerスクリプト

    private AKGun gun; //akgunスクリプト

    void Start()
    {
        //初期化
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerParameter>();
        playerControl = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerController>();
        gun = GameObject.FindGameObjectWithTag(Tags.gun).GetComponent<AKGun>();
        anim = this.GetComponent<Animation>();
        flash = this.transform.Find("muzzle_flash").GetComponent<MeshRenderer>();
        flash.enabled = false;
        currentBullet = gun.bulletCount;
        currentChargerBullet = gun.chargerBulletCount;
        bulletText.text = "弾" + currentBullet + "/" + currentChargerBullet;
    }

    void Update()
    {
        //Rキー入力　and　現在の弾数＜30
        if (parameter.inputReload && currentBullet < gun.bulletCount)
            Reload();

        //mouse 0　入力 and リロードしていない
        if (parameter.inputFire && !anim.IsPlaying(reloadAnim))
            Fire();
        else if (!anim.IsPlaying(reloadAnim))
            StateAnim(playerControl.State);
    }


    //リロードアニメ
    private void ReloadAnim()
    {
        //リロードアニメを停止
        anim.Stop(reloadAnim);
        //アニメスピード
        anim[reloadAnim].speed = (anim[reloadAnim].clip.length / gun.reloadTime);
        //巻き戻し
        anim.Rewind(reloadAnim);
        //リロードアニメをplay
        anim.Play(reloadAnim);
    }

    //弾数表示
    private IEnumerator ReloadFinish()
    {
        //リロードアニメ終了後に変化
        yield return new WaitForSeconds(gun.reloadTime);
        //予備弾＞弾上限-現在の弾
        if (currentChargerBullet >= gun.bulletCount - currentBullet)
        {
            //弾を上限に　予備弾-補充した弾
            currentChargerBullet -= (gun.bulletCount - currentBullet);
            currentBullet = gun.bulletCount;
        }
        else
        {
            //全弾補充　予備弾0
            currentBullet += currentChargerBullet;
            currentChargerBullet = 0;
        }
        bulletText.text = "弾" + currentBullet + "/" + currentChargerBullet;
    }

    //リロード
    private void Reload()
    {
        //リロードしていない
        if (!anim.IsPlaying(reloadAnim))
        {
            //予備弾>0
            if (currentChargerBullet > 0)
                StartCoroutine(ReloadFinish());
            else
            {
                //fire音声
              
                AudioSource.PlayClipAtPoint(gun.dryFireAudio, transform.position);
                return;
            }

            AudioSource.PlayClipAtPoint(gun.reloadAudio, transform.position);
            ReloadAnim();
        }
    }

    //射撃エフェクト
    private IEnumerator Flash()
    {
        flash.enabled = true;
        yield return new WaitForSeconds(gun.flashRate);
        flash.enabled = false;
    }

    //射撃する
    private void Fire()
    {
        //射撃間隔
        if (Time.time > nextFireTime)
        {
            //現在弾数＜＝0　リロードする
            if (currentBullet <= 0)
            {
                Reload();
                nextFireTime = Time.time + gun.fireRate;
                return;
            }
            currentBullet--; //弾数1
            bulletText.text = "弾" + currentBullet + "/" + currentChargerBullet;//UI表示
            DamageEnemy(); //ダメージを与える
            if (PlayerShootEvent != null)
                PlayerShootEvent();
            
            AudioSource.PlayClipAtPoint(gun.fireAudio, transform.position, 0.2f);
            nextFireTime = Time.time + gun.fireRate;
            anim.Rewind(fireAnim);
            anim.Play(fireAnim);
            StartCoroutine(Flash());
        }
    }

    //ダメージを与える
    private void DamageEnemy()
    {
        //射線
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        //射線が当たったオブジェクトを返す
        if(Physics.Raycast(ray,out hit))
        {
            //オブジェクトはenemy 　同時にカプセル
            if(hit.transform.tag==Tags.enemy && hit.collider is CapsuleCollider)
            {
                
                //敵オブジェクトの体に命中のエフェクトを生成
                GameObject go = Instantiate(gun.explosion, hit.point, Quaternion.identity);
                //エフェクトを3秒後削除
                Destroy(go, 3);
                //ダメージ値を渡す
                hit.transform.GetComponent<EnemyHealth>().TakeDamage(gun.damage);
            }
            if(hit.transform.tag == Tags.metal)
            {
                hit.transform.GetComponent<ImaprtSound>().TakeDamage();
            }

        }
        
    }
　
    //アニメを流す関数
    private void PlayerStateAnim(string animName)
    {
        if (!anim.IsPlaying(animName))
        {
            anim.Rewind(animName);
            anim.Play(animName);
        }
    }

    //ユーザのアニメーション
    private void StateAnim(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                PlayerStateAnim(idleAnim);
                break;
            case PlayerState.Walk:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Crouch:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Run:
                PlayerStateAnim(runAnim);
                break;
        }
    }
}
