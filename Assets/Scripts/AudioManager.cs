using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (!instance) 
                instance = FindObjectOfType<AudioManager>();
            return instance;
        }
    }

    
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audios;
    
    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
    }

    // index로 원하는 audio clip 끼우고 재생.
    public void Play(int i)
    {
        audioSource.clip = audios[i];
        audioSource.Play();
    }
}
