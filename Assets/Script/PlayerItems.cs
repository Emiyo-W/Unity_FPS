using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    //ユーザのアイテムボックス
    private List<int> playerItem;

    void Start()
    {
        //初期化
        playerItem = new List<int>();
    }


    //アイテムをボックスに追加 外部で呼び出す
    public void AddItem(int itemId)
    {
        if (!playerItem.Contains(itemId))
            playerItem.Add(itemId);
    }

    //アイテムがボックスにあるかどうか　あると trueを返す　
    //外部で呼び出す
    public bool HaveItem(int itemId)
    {
        if (playerItem.Contains(itemId))
            return true;

        return false;         
    }


}
