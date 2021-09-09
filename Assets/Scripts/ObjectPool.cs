using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // *****************
    // 기본 Logic
    // 1. 각 enemy들을 원하는 만큼 생성.
    // 2. 비활성화, SetActive(false)
    // 3. enemyPools 리스트에 enemies 리스트들을 추가. 
    //
    // 사용할 때 ( 현재 GameManager의 Spawn()에서 사용중 )
    // 1. 사용하고 싶은 다른 Script에서 GetEnemyObject()로 Object를 받음.
    // 2. 받은 Object를 활성화.
    // 3. ... 사용 ..
    // *****************
    
    
    // enemy의 list를 담는 pool list 
    private List<List<GameObject>> enemyPools;
    // enemy가 담기는 list
    [SerializeField] public List<GameObject> enemies;
    // ObjectPooling으로 "몇마리"를 미리 생성해줄지.
    [SerializeField] private int enemyCount = 5;
    
    // enemyCount보다 더 많이 생성하고 싶을 때, more = true.
    // enemyCount까지만 생성하고 싶을 때, more = false.
    [SerializeField] private bool more = true;
    
    
    // enemy를 생성하기 전에 다른 Script에서 접근하면 안돼서,
    // Start()보다 빠른 Awake()에서 생성 함수(Create())를 호출.
    void Awake()
    {
        Create();
    }

    
    private void Create()
    {
        enemyPools = new List<List<GameObject>>();
        for(int i=0; i< enemies.Count; i++)
        {
            enemyPools.Add(new List<GameObject>());
            for (int j = 0; j < enemyCount; j++)
            {
                GameObject obj = Instantiate(enemies[i]);   // Enemy 생성.
                obj.SetActive(false);                       // Enemy 비활성화
                enemyPools[i].Add(obj);                     // Pool에 Enemy 추가.
            }
        }
    }


    // index로 원하는 enemy를 return.
    public GameObject GetEnemyObject(int index)
    {
        // Pool을 뒤져서,
        foreach (GameObject obj in enemyPools[index])
        {
            // Hierarchy 창에서 비활성화인 enemy를 찾아서 return.
            if (!obj.activeInHierarchy) return obj;
        }
        
        // Option: enemyCount보다 많은 수의 enemy를 생성하고 싶다면, more을 true로.
        if (more)
        {
            GameObject obj = Instantiate(enemies[index]);
            enemyPools[index].Add(obj);
            return obj;
        }
        
        // more이 false이고, enemyCount만큼 활성화되었을 때는 null을 return.
        // 즉, 생성된 모든 enemy가 활성화되어서, 활성화시킬 enemy가 없는 것.
        return null;
    }

}
