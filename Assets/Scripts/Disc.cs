using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace MediaProject
{
	public class Disc : MonoBehaviour
	{
		[SerializeField] private Transform planet;
    
		// 설치영역(disc)를 담는 list.
		public List<Transform> discs = new List<Transform>();
		public static Dictionary<int, DiscInfo> discInfo = new Dictionary<int, DiscInfo>();
		// disk가 Map 속으로 들어가지 않게, 약간의 간격을 주는 변수 
		private float E = 0.32f;

		void Awake()
		{
			// disc 캐싱
			int count = transform.childCount;
			for (int i = 0; i < count; i++)
			{
				discs.Add(transform.GetChild(i).gameObject.transform);
				discInfo[i] = new DiscInfo(-1,0,null);
			}
			SettingDiscs();
		}

		private void SettingDiscs()
		{
			float radius = planet.GetComponent<SphereCollider>().radius;
			float scaleRadius = radius * planet.localScale.x;
        
			// map의 표면 위치 구해서, disc를 위치시켜줌.
			// map에 따른 disc의 회전방향 구해서, disc를 회전시켜줌.
			foreach (Transform obj in discs)
			{
				Vector3 sphereToPoint = (obj.position - planet.position).normalized;
				Vector3 toSurfaceVec = (scaleRadius + E) * sphereToPoint;
				obj.position = planet.transform.position + toSurfaceVec;
            
				Quaternion directionToSphere = Quaternion.FromToRotation(obj.up, (obj.position - planet.position).normalized) * obj.localRotation;
				obj.rotation = directionToSphere;
			}
		}
	}
}
