using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowserSmoke : MonoBehaviour
{
  
    public void PlayExplosionAudio(string audioName)
    {
        FindObjectOfType<AudioManager>().Play(audioName);
    }

}
