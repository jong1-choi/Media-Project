using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Text timeText;
    [SerializeField] private float buildTime = 15f;
    [SerializeField] private float playTime = 30f;
    private float timeRemaining; // 임시로 정함.

    // State
    private int currentStage; // 0부터. (Player에게 보여지는 건 1부터.)
    [SerializeField] private Text stageText;
    public enum CurState
    {
        Building,
        Playing
    };
    public CurState curState;

    
    void Start()
    {
        currentStage = 0;
        timeRemaining = buildTime;
        curState = CurState.Building;
        stageText.text = "Stage 1";
    }


    void Update()
    {
        RunTimer();
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
    
    
    private IEnumerator SpawnWithTime()
    {
        // Test: 7마리 적을 1.5초 간격으로 생성
        int count = 7;
        while (count-- != 0)
        {
            // 현재 stage에 맞는 적 생성
            Spawn( currentStage );
            yield return new WaitForSeconds(1.5f);
        }
    }
    
    
    private void RunTimer()
    {
        timeRemaining -= Time.deltaTime;
        
        // 시간이 다 지났을 때.
        if (timeRemaining <= 0)
        {
            switch (curState)
            {
                case CurState.Building: // Building 시간이 끝났을 경우 동작.
                    StopCoroutine(SpawnWithTime());
                    StartCoroutine(SpawnWithTime());
                    SetPlayingState();
                    break;
                case CurState.Playing:  // Playing 시간이 끝났을 경우 동작.
                    currentStage++;
                    SetBuildingState();
                    break;
                default:
                    print("Exception Case: RunTimer() in GameManager class.");
                    break;
            }
        }
        DisplayTime(timeRemaining);
    }


    private void SetPlayingState()
    {
        stageText.text = "Stage " + (currentStage+1);
        timeRemaining = playTime; 
        curState = CurState.Playing;
    }


    private void SetBuildingState()
    {
        timeRemaining = buildTime; 
        curState = CurState.Building;
    }
    
    
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
}
