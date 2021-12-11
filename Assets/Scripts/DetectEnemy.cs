using System;
using System.Collections;
using System.Collections.Generic;
using MediaProject;
// using TreeEditor;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    [SerializeField] private int towerRange = 5;
    // [SerializeField] private ObjectPool objectPool;
    public bool isEnemyDetected;
    public Transform target;
    private Collider[] targetEnemy;
    private bool delayCheck = true;
    private ObjectPool objectPool;
    private Enemy enemyScript;
    private GameObject obj;

    public static Dictionary<int, Transform> getTarget;

    private void Awake()
    {
        objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        getTarget = new Dictionary<int, Transform>();
    }

    private void FindEnemy()
    {
        targetEnemy = Physics.OverlapSphere(transform.position, towerRange);
        foreach (var element in targetEnemy)
        {
            if (element.CompareTag("Enemy"))
            {
                target = element.gameObject.transform;
                enemyScript = target.GetComponent<Enemy>();
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
        obj = objectPool.GetBulletObject(0);
        obj.SetActive(true);
        obj.transform.position = transform.position + transform.up;
        obj.transform.rotation = Quaternion.identity;
        obj.GetComponent<Bullet>().GiveTarget(target);

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

            if (Vector3.Distance(transform.position, target.transform.position) > towerRange
                || enemyScript.isArrived)
            {
                isEnemyDetected = false;
            }

            if (enemyScript.hp == 0)
            {
                obj.SetActive(false);
                isEnemyDetected = false;
            }
        }
    }
}
