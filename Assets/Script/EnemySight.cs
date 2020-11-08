using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySight : MonoBehaviour
{

    public float fieldOfViewAngle = 110f;　       //enemyの視野範囲
    public bool playerInSight;　                  //ユーザが視野にいるかどうか
    public Vector3 playerPosition;　              //ユーザの座標値
    public Vector3 resetPosition = Vector3.zero;  //ユーザ座標値をリセット

    private NavMeshAgent nav;                     //NavMeshAgentコンポーネント
    private SphereCollider col;                   //enemyの聴力範囲
    private Animator anim;                        //animatorコンポーネント
    private GameObject player;                    //ユーザgameobjectコンポネント
    private PlayerHealth playerHealth;            //PlayerHealthスクリプト
    private HashIDs hash;                         //HashIDsスクリプト
    private PlayerController playerControl;       //PlayerControllerスクリプト
    //test
    private int test = 2;

    void Start()
    {
        //初期化
        nav = this.GetComponent<NavMeshAgent>();
        col = GetComponentInChildren<SphereCollider>();
        anim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerHealth = player.GetComponent<PlayerHealth>();
        playerControl = player.GetComponent<PlayerController>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

       GunFire.PlayerShootEvent += ListenPlayer;
    }

    void Update()
    {
        //ユーザHPが0より大きい場合、現在のplayerInSight値をanimaに渡す。
        //逆はplayerInSightをfalse
        if (playerHealth.hp > 0)
            anim.SetBool(hash.playerInSightBool, playerInSight);
        else
            anim.SetBool(hash.playerInSightBool, false);
    }

    //ユーザがenemyの１）聴力範囲内、２）視野範囲内、３）間に障害物がない時に,
    //playerInSightをtrue
    void OnTriggerStay(Collider other)
    {
        //対象はユーザなのか 1)
        if (other.gameObject == player)
        {
            //初期化
            playerInSight = false;
            //ユーザとenemyとの角度を計算、directionはenemyからユーザへのベクトル 
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward); //directionとenemy正面の角度を計算

            //角度は視野範囲の半分より小さいと視野範囲内 2)
            if (angle < fieldOfViewAngle * 0.5f)
            {
                //ユーザとenemyの間に障害物があるか
                RaycastHit hit;
                //当たったオブジェクトを返す
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
                {
                    //返す値はユーザなのか  3)
                    if (hit.collider.gameObject == player)
                    {
                        //ユーザを発見
                        playerInSight = true;
                        playerPosition = player.transform.position;
                    }
                }
            }
            //ユーザの状態がwalkとrunの時　足音が発生する。enemyに位置がバレる　1)
            if (playerControl.State == PlayerState.Walk || playerControl.State == PlayerState.Run)
            {
                ListenPlayer();
            }
        }
    }

    //対象が聴力範囲から出る
    void OnTriggerExit(Collider other)
    {
        //enemyがユーザを見失った
        if (other.gameObject == player)
        {
            playerInSight = false;
        }
    }

    //enemyはユーザの足音で位置を特定
    private void ListenPlayer()
    {
        //聴力範囲内で起用
        if (Vector3.Distance(player.transform.position, transform.position) <= col.radius)
            playerPosition = player.transform.position;
    }

    void OnDestroy()
    {
        GunFire.PlayerShootEvent -= ListenPlayer;
    }
}
