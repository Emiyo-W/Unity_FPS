using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Bomb : MonoBehaviour
{
    public AudioClip itemPickUp; //拾う音声
    public int bombId; //bombid
    public Text bombText; //bombテキスト

    private GameObject player; //playerのオブジェクトを取得
    private PlayerItems playerItems; //PlayerItemsスクリプトを取得

    void Start()
    {
        //初期化
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerItems = player.GetComponent<PlayerItems>();
        bombText.enabled = false;
        
    }

   
    //ユーザがbombを拾う
    void OnTriggerEnter(Collider other)
    {
        //対象はユーザ
        if(other.gameObject == player)
        {
            //ユーザはbombを持っていない
            if (!playerItems.HaveItem(bombId))
            {
                //拾う音声を流す
                AudioSource.PlayClipAtPoint(itemPickUp, transform.position);
                //bombIdをplayerItemsに渡す
                playerItems.AddItem(bombId);
                //このbombを削除
                Destroy(this.gameObject);
            }
            else
            {
                //ユーザがbombを持っていたら、”これ以上持てません”と画面上に提示する
                bombText.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            bombText.enabled = false;
        }
    }
}
