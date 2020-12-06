using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImaprtSound : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioClip damageAudio; //ダメージ音声
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(){
        AudioSource.PlayClipAtPoint(damageAudio, transform.position);
    }
}
