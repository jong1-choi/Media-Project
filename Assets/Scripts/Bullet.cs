using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MediaProject
{
    public class Bullet : MonoBehaviour
    {
        private float damage = 1.0f;
        private Transform target;
        private GameObject tower;
        private Enemy enemyScript;

        // private void Start()
        // {
        //     enemyScript = target.GetComponent<Enemy>();
        // }
        
        public void GiveTarget(Transform target)
        {
            this.target = target;
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
                DetectEnemy.getTarget.Remove(this.GetInstanceID());
                gameObject.SetActive(false);
            }
        }

        private void MoveToTarget()
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * (Time.deltaTime * 20);
            transform.forward = dir;
        }

        private void FixedUpdate()
        {
            this.MoveToTarget();
            // if (enemyScript.hp == 0)
            // {
            //     Debug.Log("enemy already die");
            //     gameObject.SetActive(false);
            // }
        }
    }
}