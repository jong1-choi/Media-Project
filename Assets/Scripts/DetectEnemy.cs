using System;
using System.Collections;
using System.Collections.Generic;
using MediaProject;
// using TreeEditor;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private int towerRange = 5;
    public static bool isEnemyDetected;
    public static Transform target;
    public static int bulletCount;
    private Collider[] targetEnemy;
    private bool delayCheck = true;
    
    private void FindEnemy()
    {
        targetEnemy = Physics.OverlapSphere(transform.position, towerRange);
        foreach (var element in targetEnemy)
        {
            if (element.CompareTag("Enemy"))
            {
                target = element.gameObject.transform;
                isEnemyDetected = true;
            }
        }
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

    private void ShootBullet()
    {
        if(bulletCount > 2) return;
        bulletCount++;
        // bullet.transform.position = transform.position;
        Instantiate(bullet, transform.position + transform.up, Quaternion.identity);
        StartCoroutine(this.ShootDelay());
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(1f);
        delayCheck = true;
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
            
            if (delayCheck)
            {
                delayCheck = false;
                ShootBullet();
                StartCoroutine(this.ShootDelay());
            }

            if (Vector3.Distance(transform.position, target.transform.position) > 10
                || target.gameObject.GetComponent<Enemy>().hp == 0)
            {
                isEnemyDetected = false;
            }
        }
    }
}
