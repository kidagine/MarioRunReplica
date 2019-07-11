using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sounds[] sounds;

    void Awake()
    {
        foreach (Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Pause(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        s.source.Pause();
    }

    public void UnPause(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        s.source.UnPause();
    }

    public Sounds GetSound(string name)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        return s;
    }

}
