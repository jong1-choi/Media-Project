using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MediaProject
{
	public class BulletInfo : MonoBehaviour
	{
		public GameObject bulletPrefab { get; set; }
		public GameObject targetPrefab { get; set; }

		// public int bulletID { get; set; }

		public GameObject GetTarget(int bulletID)
		{
			if (bulletID == bulletPrefab.GetInstanceID() && bulletPrefab != null)
			{
				return targetPrefab;
			}
				
			else
			{
				Debug.Log("Wrong bullet ID");
				return null;
			} 
				
		}
		public BulletInfo(GameObject bullet)
		{
			bulletPrefab = bullet;
			// bulletID = bullet.GetInstanceID();
		}
		public BulletInfo(GameObject bullet, GameObject target)
		{
			bulletPrefab = bullet;
			targetPrefab = target;
			// bulletID = bullet.GetInstanceID();
		}
	}
}