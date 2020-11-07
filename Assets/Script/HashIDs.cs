using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashIDs : MonoBehaviour
{
    public int angularSpeedFloat;
    public int speedFloat;
    public int shotFloat;
    public int aimWeightFloat;
    public int playerInSightBool;
    public int deadBool;

    void Awake()
    {
        angularSpeedFloat = Animator.StringToHash("AngularSpeed");
        speedFloat = Animator.StringToHash("Speed");
        shotFloat = Animator.StringToHash("Shot");
        aimWeightFloat = Animator.StringToHash("AimWeight");
        playerInSightBool = Animator.StringToHash("PlayerInSight");
        deadBool = Animator.StringToHash("Dead");
        
    }
}
