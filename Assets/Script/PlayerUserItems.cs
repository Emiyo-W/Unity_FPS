using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUserItems : MonoBehaviour
{

    private PlayerParameter parameter; //PlayerParameterスクリプト
    private PlayerItems playerItems; // PlayerItemsスクリプト
    private const int bombId = 1; //bombId
    private float speed = 5f; //bombの飛べスピード
    private float gravity = 10f; // bombの落下スピード


    public Transform p_Bomb;


    void Start()
    {
        parameter = this.GetComponent<PlayerParameter>();
        playerItems = this.GetComponent<PlayerItems>();
    }

    void Update()
    {
        if (parameter.inputGrenade && playerItems.HaveItem(bombId))
            UserBomb();

    }


        
    private void UserBomb()
    {
        Vector3 b_transform = this.transform.position + Vector3.up;
        Instantiate(p_Bomb, b_transform, Quaternion.identity);
    }
    
    
    
      
}
