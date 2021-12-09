using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MediaProject
{
	public class Enemy : MonoBehaviour
	{
		[SerializeField] private float moveSpeed = 2.0f;
		[SerializeField] private float rotationSpeed = 1.1f;
		
		private Transform planet;
		
		private List<Transform> waypoints = new List<Transform>();
    
		private Transform targetWaypoint;
		private int targetWayPointIndex = 0;
		private int lastWayPointIndex;
		private float minDistance = 1.0f;

		[SerializeField] private ParticleSystem hurtParticle;
		[SerializeField] protected float hp = 10.0f;
    
		
		void Start()
		{
			waypoints = GameManager.Instance.Waypoints;
			planet = GameManager.Instance.planet;
        
			targetWaypoint = waypoints[targetWayPointIndex];
			lastWayPointIndex = waypoints.Count - 1;
		}


		void OnEnable()
		{
			hp = 10f;
			targetWayPointIndex = 0;
			if(waypoints.Count != 0) targetWaypoint = waypoints[targetWayPointIndex];
		}


		void FixedUpdate()
		{
			Move();
			RotateWithSphere();
		}

		// waypoint로 이동 및 회전.
		void Move()
		{
			float moveStep = moveSpeed * Time.deltaTime;
			float rotationStep = rotationSpeed * Time.deltaTime;

			Vector3 directionToTarget = targetWaypoint.position - transform.position;
			Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);

			Debug.DrawRay(transform.position, transform.forward * 5f, Color.green, 0f);
			Debug.DrawRay(transform.position, directionToTarget, Color.red, 0f);
        
			float distance = Vector3.Distance(transform.position, targetWaypoint.position);
			CheckDistanceToWaypoint(distance);
        
			transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveStep);
        
		}

		// waypoint 에 도달했는지 확인 후, waypoint를 업데이트.
		void CheckDistanceToWaypoint( float currentDistance )
		{
			if (currentDistance <= minDistance)
			{
				// Update target waypoint
				targetWayPointIndex++;
				if (targetWayPointIndex > lastWayPointIndex)
				{
					// 끝에 도달했을 때 처리 부분.
					targetWayPointIndex = 0; // Test
				}
				targetWaypoint = waypoints[targetWayPointIndex];
			}
		}

		// Enemy의 발이 Map을 향하도록 회전.
		public void RotateWithSphere()
		{
			Quaternion directionToSphere = Quaternion.FromToRotation(transform.up, (transform.position - planet.position).normalized) * transform.localRotation;
			transform.rotation = directionToSphere;
		}
		
		// damage를 입을 때 호출.
		public virtual void TakeDamage( float damage )
		{
			// hp를 깎고, 맞는 소리를 재생.
			hp -= damage;
			AudioManager.Instance.Play(0);
			if (hp <= 0)
			{
				Dead();
			}
		}
		
		// damage를 입을 때 호출. 맞는 방향으로 particle이 튀도록 함.
		public virtual void TakeDamage( float damage, Quaternion dir )
		{
			// hp를 깎고, 맞는 소리를 재생.
			hp -= damage;
			AudioManager.Instance.Play(0);
			if (hp <= 0)
			{
				Dead();
			}
			
			// 맞는 방향(z방향)으로 particle 생성.
			ParticleSystem particle = Instantiate(hurtParticle, transform.position, dir);
			// particle 시간 끝나면 파괴.
			// Destroy(particle.gameObject, particle.main.duration); // TODO: particle duration 조절.
			Destroy(particle.gameObject, 3.0f);
		}
		
		// Enemy가 죽을 때 호출해서 사용.
		protected virtual void Dead()
		{
			GameManager.Instance.AddLiveEnemyNum(-1);
			gameObject.SetActive(false);
		}

		// Enemy가 목적지에 도달할 때 사용.
		private void ArriveGoal()
		{
			
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Destination"))
			{
				GameManager.Instance.AddLife(-1);
				GameManager.Instance.AddLiveEnemyNum(-1);
				AudioManager.Instance.Play(1);
				gameObject.SetActive(false);
			}
		}
	}
}