using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Object Pool class.
    [SerializeField] private ObjectPool objectPool;
    
    public Transform planet;
    [SerializeField] private Transform startPoint;
    
    public GameObject wayPoint;
    public List<Transform> waypoints = new List<Transform>();
    public List<Transform> Waypoints
    {
        get { return waypoints; }
        set { waypoints = value; }
    }
    

    private void Start()
    {
        // Test Spawn: 7마리만 Spawn
        StartCoroutine(SpawnWithTime());
    }


    private void Spawn()
    {
        // Test: Random으로 Enemy 소환.
        // ObjectPool에서 Enemy object를 가져와서,
        // position과 rotation 설정하고,
        // 활성화.
        int enemyNum = objectPool.enemies.Count;
        int randomIndex = Random.Range(0, enemyNum);
        GameObject enemy = objectPool.GetEnemyObject(randomIndex);

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
            Spawn();
            yield return new WaitForSeconds(1.5f);
        }
    }
}
