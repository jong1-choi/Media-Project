using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MediaProject
{
    public class Bullet : MonoBehaviour
    {
        private float damage = 2.0f;
        public static bool isShooting = false;
        private Transform target;

        private void Start()
        {
            target = DetectEnemy.target;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy == null) return; // 예외) Enemy가 아닐 때 return.
                
                // Enemy에게 damage 주기.
                enemy.TakeDamage(damage, transform.localRotation);

                AudioManager.Instance.Play(0);

                isShooting = false;
                DetectEnemy.bulletCount--;
                Destroy(this.gameObject);
            }
        }

        private void MoveToTarget()
        {
            if(!DetectEnemy.isEnemyDetected) return;
            isShooting = true;
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * (Time.deltaTime * 20);
            transform.forward = dir;
        }

        private void FixedUpdate()
        {
            this.MoveToTarget();
            if (target.gameObject.GetComponent<Enemy>().hp == 0)
            {
                Debug.Log("enemy already die");
                isShooting = false;
                DetectEnemy.bulletCount--;
                Destroy(this.gameObject);
            }
        }
    }
}