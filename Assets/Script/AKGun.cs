using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AKGun : MonoBehaviour
{

    public float fireRate = 0.1f; //射撃間隔
    public float damage = 40;
    public float reloadTime = 1.5f;
    public float flashRate = 0.02f;
    public AudioClip fireAudio; //fire音声
    public AudioClip reloadAudio; //リロード音声

    public AudioClip dryFireAudio; //弾切れ音声
    public GameObject explosion; //エフェクト
    public int bulletCount = 30; //UI ,弾上限
    public int chargerBulletCount = 60; // UI 予備弾上限
    
        
    
}
