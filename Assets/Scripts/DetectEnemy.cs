using System;
using System.Collections;
using System.Collections.Generic;
using MediaProject;
using UnityEditor;
// using TreeEditor;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    [SerializeField] private int towerRange = 5;
    [SerializeField] private int bulletDamage = 5;
    // [SerializeField] private ObjectPool objectPool;
    [HideInInspector] public bool isEnemyDetected;
    private Transform target;
    private Collider[] targetEnemy;
    private bool delayCheck = true;
    private ObjectPool objectPool;
    private Enemy enemyScript;
    private GameObject bullet;
    private Bullet bulletScript;
    private Animator _animator;

    private void Awake()
    {
        objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        _animator = this.GetComponent<Animator>();
        isEnemyDetected = false;
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
        _animator.SetBool("Attack", true);
        if (this.CompareTag("Tower1")) bullet = objectPool.GetBulletObject(0);
        else if (this.CompareTag("Tower2")) bullet = objectPool.GetBulletObject(1);
        else if (this.CompareTag("Tower3")) bullet = objectPool.GetBulletObject(2);
        else if (this.CompareTag("Tower4")) bullet = objectPool.GetBulletObject(3);
        
        bullet.SetActive(true);
        bullet.transform.position = transform.position + (transform.up * 1.5f);
        bullet.transform.rotation = Quaternion.identity;
        bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
        bulletScript.SetBulletDamage(bulletDamage);
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

            if (enemyScript.hp <= 0)
            {
                bullet.SetActive(false);
                isEnemyDetected = false;
            }
        }
    }
}
