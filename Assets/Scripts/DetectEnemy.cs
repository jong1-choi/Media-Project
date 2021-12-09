using System;
using System.Collections;
using System.Collections.Generic;
using MediaProject;
// using TreeEditor;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    private bool isEnemyDetected;
    private Transform target;
    private Collider[] targetEnemy;
    
    private void FindEnemy()
    {
        targetEnemy = Physics.OverlapSphere(transform.position, 5);
        foreach (var element in targetEnemy)
        {
            if (element.CompareTag("Enemy"))
            {
                target = element.gameObject.transform;
                // Debug.Log("FindEnemy");
                isEnemyDetected = true;
            }
        }
        // 타겟이 타워의 범위를 벗어났을 때 회전처리
        
        // Debug.Log("NoEnemy");
    }

    private void RotateTower(float speed = 1f)
    {
        if (!isEnemyDetected) return;
        Vector3 towerInitialPos = transform.position - new Vector3(0, 0, 0);
        Vector3 towerToTarget = target.transform.position - transform.position;
        Vector3 towerToTargetOnPlane = Vector3.ProjectOnPlane(towerToTarget, towerInitialPos).normalized;
        // Debug.DrawRay(transform.position, towerToTarget, Color.red, 1f);
        transform.LookAt(transform.position + towerToTargetOnPlane, towerInitialPos);
    }

    void Update()
    {
        if (!isEnemyDetected)
        {
            FindEnemy();
        }
        else
        {
            this.RotateTower();
            
            if (Vector3.Distance(transform.position, target.transform.position) > 10)
            {
                isEnemyDetected = false;
            }
            // 캐릭터가 죽었을때도 추가
        }
    }
}
