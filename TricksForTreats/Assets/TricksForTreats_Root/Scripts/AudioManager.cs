using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("AudioManager is null!");
            }
            return instance;
        }
    }
    public int audioState; // 0 all hearing, 1 no musica si sonidos, 2 musica si, sonidos no, 3 shhh
    //public int clipNumber;
    public bool restart;
    [SerializeField] AudioSource[] audioSources; // 0 musica, 1 en adelante sfx
    [SerializeField] AudioClip[] sounds;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
    public void ChangeMusic(bool musicState)
    {
        if(!musicState)
        {
            audioSources[0].Pause();
            if(audioState == 0) audioState = 1;
            else if(audioState == 2) audioState = 3;
        }
        else
        {
            audioSources[0].UnPause();
            if (audioState == 1) audioState = 0;
            else if (audioState == 3) audioState = 2;
        }
    }
    public void ChangeSFX(bool sfxState)
    {
        if(!sfxState)
        {
            audioSources[1].mute = true;
            if (audioState == 0) audioState = 2;
            else if (audioState == 1) audioState = 3;
        }
        else
        {
            audioSources[1].mute = false;
            if (audioState == 2) audioState = 0;
            else if (audioState == 3) audioState = 1;
        }
    } 
    public void PlaySFX(int clipNumber)
    {
        audioSources[1].clip = sounds[clipNumber];
        audioSources[1].Play();
    }
    private void Update()
    {
        if(restart)
        {
            audioSources[0].Stop();
            audioSources[0].Play();
            restart = false;
        }
    }

}
