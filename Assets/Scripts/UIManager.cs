using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    // Singleton
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (!instance) 
                instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }


    [SerializeField] private AudioMixer audioMixer;
    private float bgm;
    private float sfx;

    [SerializeField] private GameObject textPanel;
    [SerializeField] public Text installText;
    [SerializeField] public Text upgradeText;

    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject maskPanel;
    [SerializeField] private Text BGMText;
    [SerializeField] private Text SFXText;
    [SerializeField] private Text VibeText;
    private const float OffSoundValue = -80f;
    private bool BGMOn = true;
    private bool SFXOn = true;
    private bool VibeOn = true;

    [SerializeField] private GameObject HUDCanvas;

    private bool gameIsPaused = false;
    
    
    void Start()
    {
        initAudio();
    }

    
    void Update()
    {
        
    }


    public void ReStart()
    {
        SceneManager.LoadScene("PlayScene");
    }
    

    public void TranslateMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }


    private void initAudio()
    {
        bgm = GetAudioMixerValue("BGM");
        sfx = GetAudioMixerValue("SFX");

        if (bgm == OffSoundValue) {
            BGMOn = false;
            BGMText.text = "BGM OFF";
        }

        if (sfx == OffSoundValue) {
            SFXOn = false;
            BGMText.text = "SFX OFF";
        }
    }
    

    private float GetAudioMixerValue(string name)
    {
        bool isGet = audioMixer.GetFloat(name, out var value);
        return isGet ? value : 0.0f;
    }
    

    public void ToggleBGM()
    {
        if (BGMOn)
        {
            BGMText.text = "BGM OFF";
            audioMixer.SetFloat("BGM", OffSoundValue);
        }
        else
        {
            BGMText.text = "BGM ON";
            audioMixer.SetFloat("BGM", bgm == OffSoundValue ? 0f : bgm);
        }
        BGMOn = !BGMOn;
    }
    
    
    public void ToggleSFX()
    {
        if (SFXOn)
        {
            SFXText.text = "SFX OFF";
            audioMixer.SetFloat("SFX", OffSoundValue);
        }
        else
        {
            SFXText.text = "SFX ON";
            audioMixer.SetFloat("SFX", bgm == OffSoundValue ? 0f : bgm);
        }
        SFXOn = !SFXOn;
    }
    
    
    public void ToggleVibe()
    {
        if (VibeOn)
        {
            VibeText.text = "Vibe OFF";
            // TODO : Vibration On
        }
        else
        {
            VibeText.text = "Vibe ON";
            // TODO : Vibration Off
        }
        VibeOn = !VibeOn;
    }


    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }


    private void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
    

    public void CloseHUD()
    {
        if (HUDCanvas == null) return;

        Animator animator = HUDCanvas.GetComponent<Animator>();
        if (animator == null) return;

        bool isClose = animator.GetBool("close");
        animator.SetBool("close", !isClose);
    }
    

    public void OpenOptionPanel()
    {
        if (optionPanel == null) return;

        Animator animator = optionPanel.GetComponent<Animator>();
        if (animator == null) return;

        bool isOpen = animator.GetBool("open");
        maskPanel.SetActive(!isOpen); // Option창이 열렸을 때 다른 버튼을 클릭 못하게 막기 위함.
        animator.SetBool("open", !isOpen);
        
        // HUD 창은 반대로 닫거나 열거나.
        CloseHUD();
    }
    

    public void OpenTextPanel()
    {
        if (textPanel == null) return;
        
        Animator animator = textPanel.GetComponent<Animator>();
        if (animator == null) return;
        
        bool isOpen = animator.GetBool("open");
        animator.SetBool("open", !isOpen);
    }
}
