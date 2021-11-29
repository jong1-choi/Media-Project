using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform sphere;
    
    // 0 : Main Menu
    // 1 : Option Menu
    // 2 : Sound Menu
    // 4 : Quit Menu
    [SerializeField] private GameObject[] menus;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioMixer audioMixer;

    private AudioSource _audioSource;
    
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioMixer.SetFloat("Master", masterSlider.value);
        audioMixer.SetFloat("BGM", musicSlider.value);
        audioMixer.SetFloat("SFX", sfxSlider.value);
    }

    void FixedUpdate()
    {
        sphere.Rotate(new Vector3(0,1,0), -15f * Time.deltaTime);
    }

    
    // 활성화되어 있는 Panel 찾아서 비활성화.
    private void SetActiveFalseMenu()
    {
        foreach ( GameObject o in menus ) 
        {
            if ( o.activeInHierarchy )
            {
                o.SetActive( false );
                break;
            }
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    
    public void OnClickPlay()
    {
        _audioSource.Play();
        SceneManager.LoadScene("PlayScene");
    }

    public void TurnToMenu( GameObject o )
    {
        _audioSource.Play();
        SetActiveFalseMenu();
        o.SetActive(true);
    }
    
}
