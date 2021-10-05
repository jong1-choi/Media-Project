using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MediaProject
{
	public class Enemy : MonoBehaviour
	{
		[SerializeField] private Transform planet;
		[SerializeField] private float moveSpeed = 2.0f;
		[SerializeField] private float rotationSpeed = 1.1f;
    
		private List<Transform> waypoints = new List<Transform>();
    
		private Transform targetWaypoint;
		private int targetWayPointIndex = 0;
		private int lastWayPointIndex;
		private float minDistance = 1.0f;

    
		void Start()
		{
			waypoints = GameManager.Instance.Waypoints;
			planet = GameManager.Instance.planet;
        
			targetWaypoint = waypoints[targetWayPointIndex];
			lastWayPointIndex = waypoints.Count - 1;
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
	}
}