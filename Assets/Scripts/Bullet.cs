using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MediaProject
{
    public class Bullet : MonoBehaviour
    {
        private float damage = 5.0f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy == null) return; // 예외) Enemy가 아닐 때 return.
                
                // Enemy에게 damage 주기.
                enemy.TakeDamage(damage, transform.localRotation);
                AudioManager.Instance.Play(0);
            }
        }
    }
}