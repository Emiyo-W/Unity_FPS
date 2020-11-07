using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTigger : MonoBehaviour
{

    public float timeToInactivePlayer = 2.0f;
    public float timeToRestart = 5.0f;

    private GameObject player;
    private FadeInOut fader;
    private bool playerInExit;
    private float timer;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
    }

    void Update()
    {
        if (playerInExit)
            InExitActivation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInExit = true;
        }
      
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInExit = false;
            timer = 0;
        }
    }

    private void InExitActivation()
    {
        timer += Time.deltaTime;
        if (timer >= timeToInactivePlayer)
            player.GetComponent<PlayerHealth>().DisableInput();
        if (timer >= timeToRestart)
        {
            fader.EndScene();
        }
    }
}
