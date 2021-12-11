using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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
    [SerializeField] public Text systemText;
    private bool isWrongTextOpen;
    public bool IsWrongTextOpen
    {
        get { return isWrongTextOpen; }
        set { isWrongTextOpen = value; }
    }

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
 
    [SerializeField] private GameObject pauseButton;
    private bool gameIsPaused = false;

    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private GameObject bossHPPanel;
    [SerializeField] public Slider HPSlider;

    
    
    void Start()
    {
        IsWrongTextOpen = false;
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


    public void UpdateHP(float value)
    {
        HPSlider.value = value;
    }


    public void UpdateSystemText(String text)
    {
        systemText.text = text;
    }
    

    public void ClosePauseButton()
    {
        if (pauseButton == null)
        {
            print("pauseButton is null");
            return;
        }

        Animator animator = pauseButton.GetComponent<Animator>();
        if (animator == null)
        {
            print("pauseButton animator is null");
            return;
        }
        
        bool isClose = animator.GetBool("close");
        animator.SetBool("close", !isClose);
    }
    

    public void CloseHUD()
    {
        if (HUDCanvas == null) return;

        Animator animator = HUDCanvas.GetComponent<Animator>();
        if (animator == null) return;

        bool isClose = animator.GetBool("close");
        animator.SetBool("close", !isClose);
    }
    
    
    public void CloseHUD(bool b)
    {
        if (HUDCanvas == null) return;

        Animator animator = HUDCanvas.GetComponent<Animator>();
        if (animator == null) return;

        bool isClose = animator.GetBool("close");
        animator.SetBool("close", b);
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
    

    public void OpenTextPanel(bool isWrongText = false)
    {
        if (textPanel == null) return;
        Animator animator = textPanel.GetComponent<Animator>();
        if (animator == null) return;
        
        if (IsWrongTextOpen) // Wrong text 가 띄워져 있을 때
        {
            if (!isWrongText) // Tower Button 클릭 했을 때
            {
                IsWrongTextOpen = false;
            }
            else // (Tower Button 클릭 없이) 2초 지났을 때
            {
                bool isOpen = animator.GetBool("open");
                animator.SetBool("open", !isOpen);
                ClosePauseButton();                
            }
        }
        else if (!IsWrongTextOpen && isWrongText)
        {
            // Wrong text 띄워져 있을 때 Tower Button 클릭했고, 2초 지났을 때
            // Nothing...
        }
        else
        {
            bool isOpen = animator.GetBool("open");
            animator.SetBool("open", !isOpen);
            ClosePauseButton();
        }
    }


    public void OpenGameOverPanel()
    {
        if (gameOverPanel == null) return;
        
        Animator animator = gameOverPanel.GetComponent<Animator>();
        if (animator == null) return;
        
        bool isOpen = animator.GetBool("open");
        animator.SetBool("open", !isOpen);
        
        CloseHUD(true);
    }
    
    
    public void OpenBossHPPanel()
    {
        if (bossHPPanel == null) return;
        
        Animator animator = bossHPPanel.GetComponent<Animator>();
        if (animator == null) return;
        
        bool isOpen = animator.GetBool("open");
        animator.SetBool("open", !isOpen);
    }
    
}
