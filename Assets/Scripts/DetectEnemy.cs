using System;
using System.Collections;
using System.Collections.Generic;
using MediaProject;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    private bool isEnemyDetected;
    private GameObject target;
    private Collider[] targetEnemy;

    private void FindEnemy()
    {
        targetEnemy = Physics.OverlapSphere(transform.position, 5);
        foreach (var element in targetEnemy)
        {
            if (element.CompareTag("Enemy"))
            {
                target = element.gameObject;
                // Debug.Log("FindEnemy");
                isEnemyDetected = true;
            }
        }
        // 타겟이 타워의 범위를 벗어났을 때 회전처리
        
        // Debug.Log("NoEnemy");
    }

    void Update()
    {
        if (!isEnemyDetected)
        {
            FindEnemy();
        }
        else
        {
            // 캐릭터 기준으로 z축만 회전해야함
            transform.LookAt(target.transform.position);
            
            if (Vector3.Distance(transform.position, target.transform.position) > 10)
            {
                isEnemyDetected = false;
            }
            // 캐릭터가 죽었을때도 추가
        }
    }
}
