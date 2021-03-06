using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    // Object Pool class
    [SerializeField] private ObjectPool objectPool;

    // Sphere planet
    public Transform planet;

    // Castle
    [SerializeField] private Transform startPoint;

    // Waypoint
    public GameObject wayPoint;
    public List<Transform> waypoints = new List<Transform>();

    public List<Transform> Waypoints
    {
        get { return waypoints; }
        set { waypoints = value; }
    }

    // Timer
    [SerializeField] public GameObject timerPanel;
    [SerializeField] private Text timeText;
    [SerializeField] private float peacefulTime = 1f;
    private float timeRemaining;

    // State
    [HideInInspector]public int currentStage;
    private int curEnemyIndex;
    [SerializeField] private Text stageText;

    public enum CurState
    {
        Peaceful,
        Playing
    };

    public CurState curState;

    [SerializeField] private GameObject liveNumPanel;
    [SerializeField] private Text liveNumText;
    [HideInInspector] public int[] stageEnemyNumList = {10, 10, 10, 1, 10, 10, 10, 1, 10, 10, 10, 1};
    public static float[] maxHP = {15, 21, 27 , 180, 33, 39, 45, 250, 51, 57, 63, 330};
    public int curLiveEnemyNum = 0;

    [SerializeField] private List<GameObject> bosses;

    [SerializeField] private Text lifeText;
    [SerializeField] private int life = 20;

    [SerializeField] private Text moneyText;
    private int money = 100;

    [SerializeField] private Text endingText;

    private bool isGameEnd = false;

    void Start()
    {
        curEnemyIndex = 0;
        currentStage = 0;
        timeRemaining = peacefulTime;
        curState = CurState.Peaceful;
        stageText.text = "Stage 1";
        liveNumText.text = "0 / 0";
        UpdateMoneyText();
    }


    void Update()
    {
        if (isGameEnd) return;
        if (currentStage == stageEnemyNumList.Length)
        {
            GameOver("Game Clear !");
            isGameEnd = true;
            return;
        }
        
        if (curState == CurState.Playing)
        {
            // Play ????????? ???.
            // ?????? ?????????????????? ??? ????????? ???????????? ??????.
        }
        else if (curState == CurState.Peaceful)
        {
            RunTimer();    
        }
        
    }
    

    private void Spawn( int index )
    {
        // ObjectPool?????? Enemy object??? ????????????,
        // position??? rotation ????????????,
        // ?????????.
        GameObject enemy = objectPool.GetEnemyObject( index );

        enemy.transform.position = startPoint.position;
        enemy.transform.rotation = Quaternion.identity;
        enemy.SetActive(true);
    }

    
    private void SpawnBoss( int index )
    {
        if (bosses.Count < index + 1)
        {
            print("GameManager.SpawnBoss() : boss ???????????? ??? index ?????????.");
            return;
        }

        UIManager.Instance.OpenBossHPPanel();
        Instantiate(bosses[index], startPoint.position, Quaternion.identity);
        curEnemyIndex--;
    }
    
    
    // ?????? ?????? ???????????? ?????? Spawn ????????? ?????????.
    private IEnumerator SpawnWithTime(int enemyNum)
    {
        // Test: enemyNum?????? ?????? 1??? ???????????? ??????
        while (enemyNum-- != 0)
        {
            // ?????? stage??? ?????? ??? ??????
            switch (currentStage + 1)
            {
                case 4: // 1 Boss
                    UIManager.Instance.UpdateHP(1);
                    SpawnBoss(0);
                    break;
                case 8: // 2 Boss
                    UIManager.Instance.UpdateHP(1);
                    SpawnBoss(1);
                    break;
                case 12: // 3 Boss
                    UIManager.Instance.UpdateHP(1);
                    SpawnBoss(2);
                    break;
                default:
                    Spawn( curEnemyIndex );
                    break;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    
    // Timer ??????, Timer??? ??? ????????? ??? ??????.
    private void RunTimer()
    {
        timeRemaining -= Time.deltaTime;
        
        // ????????? ??? ????????? ???.
        if (timeRemaining <= 0)
        {
            StopCoroutine(SpawnWithTime(stageEnemyNumList[currentStage]));
            StartCoroutine(SpawnWithTime(stageEnemyNumList[currentStage]));
            SetPlayingState();
        }
        DisplayTime(timeRemaining);
    }
    
    
    // ioClose : true => Timer UI??? ?????????.
    // ioClose : false => Timer ??? UI??? ?????????.
    public void TimerClose(bool isClose)
    {
        if (!timerPanel) return;
        
        Animator animator = timerPanel.GetComponent<Animator>();
        if (!animator) return;

        animator.SetBool("close", isClose);
    }
    
    
    // ioOpen : true => ???????????? ??? UI??? ?????????.
    // ioOpen : false => ???????????? ??? UI??? ?????????.
    public void LiveNumOpen(bool isOpen)
    {
        if (!liveNumPanel) return;
        
        Animator animator = liveNumPanel.GetComponent<Animator>();
        if (!animator) return;

        animator.SetBool("open", isOpen);
    }

    
    // ?????? ???????????? ??? UI ????????????.
    private void UpdateLiveEnemyNumText()
    {
        if(currentStage == stageEnemyNumList.Length) return;
        liveNumText.text = curLiveEnemyNum + " / " + stageEnemyNumList[currentStage];
    }
    
    
    // Life UI ????????????.
    private void UpdateLifeText()
    {
        lifeText.text = life.ToString();
    }


    private void UpdateMoneyText()
    {
        moneyText.text = money.ToString();
    }
    
    
    // ?????? ?????? ?????????. ?????? ?????? ??? num = -1 ??? ?????????.
    public void AddLiveEnemyNum(int num)
    {
        if(currentStage == stageEnemyNumList.Length) return;
        curLiveEnemyNum += num;
        CheckExistEnemy();
        UpdateLiveEnemyNumText();
    }
    

    // ?????? ??????????????? ?????? ??????????????? ??????.
    private void CheckExistEnemy()
    {
        if (curLiveEnemyNum > 0) return;
        currentStage++;
        curEnemyIndex++;
        stageText.text = "Stage " + (currentStage + 1); // ??????????????? ???????????? ?????? 1??? ??????. (???????????? Stage 1?????????)
        SetPeacefulState();
    }
    
    
    // Peaceful ????????? ????????? ??? ?????????.
    private void SetPeacefulState()
    {
        timeRemaining = peacefulTime;
        curState = CurState.Peaceful;
        TimerClose(false);
        LiveNumOpen(false);
    }
    
    
    // Playing ????????? ????????? ??? ?????????.
    private void SetPlayingState()
    {
        if(currentStage == stageEnemyNumList.Length) return;
        curLiveEnemyNum = stageEnemyNumList[currentStage];
        curState = CurState.Playing;
        UpdateLiveEnemyNumText();
        TimerClose(true);
        LiveNumOpen(true);
    }
    
    
    // Timer ??? : ??? ????????? ????????????.
    private void DisplayTime( float time )
    {
        if (time <= 0)
        {
            timeText.text = "";
            return;
        }
            
        time += 1;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
    
    
    // Life??? ?????????. ?????? ???????????? ???????????? ??? -1??? ?????????.
    public void AddLife(int num)
    {
        if (isGameEnd) return;
        life += num;
        CheckLife();
    }


    // Life??? ????????? ??????.
    private void CheckLife()
    {
        if (life <= 0)
        {
            GameOver("Game Over");
            isGameEnd = true;
        }
        else
        {
            UpdateLifeText();
        }
    }
    
    
    // GameOver ????????? ???.
    private void GameOver(String text)
    {
        if (isGameEnd) return;
        endingText.text = text;
        UIManager.Instance.OpenGameOverPanel();
    }


    // money??? ????????? UI??? ????????????.
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }


    // money ??????. ???????????? false. ???????????? ??? ?????? true.
    public bool PayMoney(int amount)
    {
        int updateMoney = money - amount;
        if (updateMoney < 0)
            return false;

        money = updateMoney;
        UpdateMoneyText();
        return true;
    }
}
