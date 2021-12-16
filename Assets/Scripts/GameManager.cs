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
            // Play 상태일 때.
            // 현재 콘텐츠에서는 할 동작이 아무것도 없음.
        }
        else if (curState == CurState.Peaceful)
        {
            RunTimer();    
        }
        
    }
    

    private void Spawn( int index )
    {
        // ObjectPool에서 Enemy object를 가져와서,
        // position과 rotation 설정하고,
        // 활성화.
        GameObject enemy = objectPool.GetEnemyObject( index );

        enemy.transform.position = startPoint.position;
        enemy.transform.rotation = Quaternion.identity;
        enemy.SetActive(true);
    }

    
    private void SpawnBoss( int index )
    {
        if (bosses.Count < index + 1)
        {
            print("GameManager.SpawnBoss() : boss 개수보다 큰 index 들어옴.");
            return;
        }

        UIManager.Instance.OpenBossHPPanel();
        Instantiate(bosses[index], startPoint.position, Quaternion.identity);
        curEnemyIndex--;
    }
    
    
    // 일정 시간 간격으로 적을 Spawn 해주는 코루틴.
    private IEnumerator SpawnWithTime(int enemyNum)
    {
        // Test: enemyNum만큼 적을 1초 간격으로 생성
        while (enemyNum-- != 0)
        {
            // 현재 stage에 맞는 적 생성
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
    
    
    // Timer 작동, Timer가 다 되었을 때 처리.
    private void RunTimer()
    {
        timeRemaining -= Time.deltaTime;
        
        // 시간이 다 지났을 때.
        if (timeRemaining <= 0)
        {
            StopCoroutine(SpawnWithTime(stageEnemyNumList[currentStage]));
            StartCoroutine(SpawnWithTime(stageEnemyNumList[currentStage]));
            SetPlayingState();
        }
        DisplayTime(timeRemaining);
    }
    
    
    // ioClose : true => Timer UI가 숨겨짐.
    // ioClose : false => Timer 적 UI가 보여짐.
    public void TimerClose(bool isClose)
    {
        if (!timerPanel) return;
        
        Animator animator = timerPanel.GetComponent<Animator>();
        if (!animator) return;

        animator.SetBool("close", isClose);
    }
    
    
    // ioOpen : true => 살아있는 적 UI가 보여짐.
    // ioOpen : false => 살아있는 적 UI가 숨겨짐.
    public void LiveNumOpen(bool isOpen)
    {
        if (!liveNumPanel) return;
        
        Animator animator = liveNumPanel.GetComponent<Animator>();
        if (!animator) return;

        animator.SetBool("open", isOpen);
    }

    
    // 현재 살아있는 적 UI 업데이트.
    private void UpdateLiveEnemyNumText()
    {
        if(currentStage == stageEnemyNumList.Length) return;
        liveNumText.text = curLiveEnemyNum + " / " + stageEnemyNumList[currentStage];
    }
    
    
    // Life UI 업데이트.
    private void UpdateLifeText()
    {
        lifeText.text = life.ToString();
    }


    private void UpdateMoneyText()
    {
        moneyText.text = money.ToString();
    }
    
    
    // 적의 수를 더해줌. 적이 죽을 때 num = -1 로 넘겨줌.
    public void AddLiveEnemyNum(int num)
    {
        if(currentStage == stageEnemyNumList.Length) return;
        curLiveEnemyNum += num;
        CheckExistEnemy();
        UpdateLiveEnemyNumText();
    }
    

    // 현재 스테이지에 적이 존재하는지 체크.
    private void CheckExistEnemy()
    {
        if (curLiveEnemyNum > 0) return;
        currentStage++;
        curEnemyIndex++;
        stageText.text = "Stage " + (currentStage + 1); // 사용자에게 보여주기 위해 1을 더함. (사용자는 Stage 1부터임)
        SetPeacefulState();
    }
    
    
    // Peaceful 상태로 넘어갈 때 설정들.
    private void SetPeacefulState()
    {
        timeRemaining = peacefulTime;
        curState = CurState.Peaceful;
        TimerClose(false);
        LiveNumOpen(false);
    }
    
    
    // Playing 상태로 넘어갈 때 설정들.
    private void SetPlayingState()
    {
        if(currentStage == stageEnemyNumList.Length) return;
        curLiveEnemyNum = stageEnemyNumList[currentStage];
        curState = CurState.Playing;
        UpdateLiveEnemyNumText();
        TimerClose(true);
        LiveNumOpen(true);
    }
    
    
    // Timer 분 : 초 단위로 보여주기.
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
    
    
    // Life를 더해줌. 적이 목적지에 도달했을 때 -1을 넘겨줌.
    public void AddLife(int num)
    {
        if (isGameEnd) return;
        life += num;
        CheckLife();
    }


    // Life가 있는지 확인.
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
    
    
    // GameOver 되었을 때.
    private void GameOver(String text)
    {
        if (isGameEnd) return;
        endingText.text = text;
        UIManager.Instance.OpenGameOverPanel();
    }


    // money를 더하고 UI를 업데이트.
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }


    // money 지불. 부족하면 false. 충분하면 돈 깎고 true.
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
