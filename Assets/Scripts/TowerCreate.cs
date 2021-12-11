using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Experimental.GraphView;
// using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;


namespace MediaProject
{
	public class TowerCreate : MonoBehaviour
	{
		// [SerializeField] Text message;
		[SerializeField] private ObjectPool objectPool;
		public static bool isTowerButtonOn;
		public static bool isUpgrade;
		public static int upgradeDiskNum;
		private List<Transform> discSet;

		private Text message;
		
		private void Awake()
		{
			message = UIManager.Instance.installText;
			discSet = this.GetComponent<Disc>().discs;
			
		}

		public void TowerInstall()
		{
			isTowerButtonOn = !isTowerButtonOn;
		}

		private void Create(int diskNum)
		{
			// 타워를 설치 했으므로 트러거를 false로 전환
			isTowerButtonOn = false;

			// 타워 종류 난수 생성
			int towerNum = objectPool.towers.Count;
			int randomTypeIndex = UnityEngine.Random.Range(0, towerNum);

			// 타워가 설치될 디스크의 정보 업데이트 ( 타워 레벨 +1 )
			Disc.discInfo[diskNum] = new DiscInfo(randomTypeIndex,Disc.discInfo[diskNum].towerLevel + 1, objectPool.GetTowerObject(randomTypeIndex));

			// 생성될 타워의 위치, 방향 설정
			Transform towerPrefab = Disc.discInfo[diskNum].towerPrefab.transform;
			towerPrefab.position = discSet[diskNum].position;
			Quaternion directionToSphere = Quaternion.FromToRotation(towerPrefab.up,
				(towerPrefab.position - new Vector3(0, 0, 0)).normalized);

			towerPrefab.transform.rotation = directionToSphere;
			
			// pooling된 프리팹을 Active한 후 설정된 타입, 레벨에 해당하는 타워를 Active
			towerPrefab.gameObject.SetActive(true);
			towerPrefab.transform.GetChild(Disc.discInfo[diskNum].towerLevel - 1).gameObject.SetActive(true);
			print(Disc.discInfo[diskNum].towerLevel);
		}

		IEnumerator MaxTowerLevel()
		{
			// 잘못된 타워를 설치하면 2초간 메세지를 띄워줌
			isTowerButtonOn = false;
			message.text = "타워의 레벨이 최대치입니다.";
			yield return new WaitForSeconds(2);
			UIManager.Instance.OpenTextPanel();
			message.text = "";
		}
		
		private void Update()
		{
			// 현재 상태 검사
			// if (!(GameManager.Instance.curState == GameManager.CurState.Building)) return;
			
			if(!isTowerButtonOn || isUpgrade) return;

			// tower 버튼 클릭시 메세지 출력
			message.text = "타워를 설치할 영역을 클릭해주세요";

			if (Input.GetMouseButtonUp(0)) // tower 버튼을 누르고 디스크를 클릭 했을 때
			{
				// ray cast 처리
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				Physics.Raycast(ray, out hit);
			
				// disk 오브젝트를 클릭했을 경우
				if (hit.collider != null && hit.transform.gameObject.CompareTag("Disc"))
				{
					// 클릭하면 디스크의 이름을 String으로 가져오므로 Int로 변환
					int diskIndex = Convert.ToInt32(hit.transform.name);
					
					if (Disc.discInfo[diskIndex].towerLevel == 0)
					{
						Create(diskIndex);
						message.text = "";
						UIManager.Instance.OpenTextPanel();
					}
					
					else if (Disc.discInfo[diskIndex].towerLevel > 0 && Disc.discInfo[diskIndex].towerLevel < 4)
					{
						// 업그레이드할 타워를 선택받기 위해 업그레이드 트리거를 On
						isUpgrade = true;
						// 업그레이드 처리를 위해 받아온 diskIndex를 넘겨줌
						upgradeDiskNum = diskIndex;
						message.text = "";
					}
					else
					{
						StartCoroutine(MaxTowerLevel());
					}
					
				}
			}
		}
	}
}

