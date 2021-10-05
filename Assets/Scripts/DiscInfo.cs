using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MediaProject
{
	public class DiscInfo
	{
		// towerType 
		// 0 = 근접
		// 1 = 원거리
		// 2 = 슬로우
		// 3 = 스턴
		public int towerType { get; set; }
		public int towerLevel { get; set; }
		
		public GameObject towerPrefab { get; set; }

		public DiscInfo(int type, int level, GameObject tower)
		{
			towerType = type;
			towerLevel = level;
			towerPrefab = tower;
		}
	}    
}

