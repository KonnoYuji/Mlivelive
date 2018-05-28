using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    static private AudioManager _instance;

    static public AudioManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private AudioClip defaultBGM, cheers;

    [SerializeField]
    private bool isOnBGM = false;

    private AudioSource[] audios;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<AudioManager>();
        }

        audios = GetComponents<AudioSource>();

        if(isOnBGM)
        {
            PlayDefaultBGM();
        }        
    }

    public void PlaySound(AudioSource source)
    {
        if(source.clip == null)
        {
            source.Play();
        }
        else
        {
            Debug.Log("No AudioClip");
        }
    }

    public void PlaySoundAtLocation(AudioSource source)
    {

    }

    public void PlayCheerSound()
    {
        for(int i=0; i<audios.Length; i++)
        {
            if(!audios[i].isPlaying)
            {
                audios[i].clip = cheers;
                audios[i].Play();
                return;
            }
        }
    }

    private void PlayDefaultBGM()
    {
        for (int i = 0; i < audios.Length; i++)
        {
            if (!audios[i].isPlaying)
            {
                audios[i].loop = true;
                audios[i].clip = defaultBGM;
                audios[i].Play();
                return;
            }
        }
    }
}
