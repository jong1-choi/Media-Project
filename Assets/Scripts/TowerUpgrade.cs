using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MediaProject
{
	public class TowerUpgrade : MonoBehaviour
	{
		// [SerializeField] Text message;
		private List<Transform> discSet;

		private void Awake()
		{
			discSet = this.GetComponent<Disc>().discs;
		}
    
		private void LevelUp(int usingDiskNum, int upgradeDiskNum)
		{
			// 레벨업을 진행하므로 트리거를 바꿔줌
			TowerCreate.isTowerButtonOn = false;
			TowerCreate.isUpgrade = false;

			// 같은 타입,레벨의 타워가 선택된 경우
			if (Disc.discInfo[usingDiskNum].towerLevel == Disc.discInfo[upgradeDiskNum].towerLevel 
			    && Disc.discInfo[usingDiskNum].towerType == Disc.discInfo[upgradeDiskNum].towerType
			    && usingDiskNum != upgradeDiskNum)
			{
				// 업그레이드 비용 검사
				int level = Disc.discInfo[usingDiskNum].towerLevel;
				if ( !GameManager.Instance.PayMoney(level * 10) )
				{
					StartCoroutine(this.WrongMsg("업그레이드 비용이 부족합니다.\n업그레이드 비용 : " + level*10));
					return;
				}

				출처: https://unitytip.tistory.com/52 [Unity Tip 모음 by dnrkckzk]
				// 업그레이드에 사용되는 타워의 프리팹 초기화
				Disc.discInfo[usingDiskNum].towerPrefab.transform.GetChild(Disc.discInfo[usingDiskNum].towerLevel - 1).gameObject.SetActive(false);
				Disc.discInfo[usingDiskNum].towerPrefab.gameObject.SetActive(false);
				
				// 업그레이드에 사용되는 타워를 가지고있던 디스크의 정보 초기화
				Disc.discInfo[usingDiskNum] = new DiscInfo(-1, 0, null);
				
				// 업그레이드 전 레벨의 타워 비활성화
				Disc.discInfo[upgradeDiskNum].towerPrefab.transform.GetChild(Disc.discInfo[upgradeDiskNum].towerLevel - 1).gameObject.SetActive(false);
				
				// 타워 레벨 증가 후 증가된 레벨의 타워 활성화
				Disc.discInfo[upgradeDiskNum].towerLevel += 1;
				Disc.discInfo[upgradeDiskNum].towerPrefab.transform.GetChild(Disc.discInfo[upgradeDiskNum].towerLevel - 1).gameObject.SetActive(true);
				
				// 업그레이드가 종료 됐으므로 업그레이드 메세지 초기화
				UIManager.Instance.UpdateSystemText("");
				UIManager.Instance.OpenTextPanel();
			}
			else
			{
				StartCoroutine(this.WrongMsg("잘못된 타워를 선택하셨습니다."));
			}
		}
		
		IEnumerator WrongMsg(String text)
		{
			// 잘못된 타워를 설치하면 2초간 메세지를 띄워줌
			UIManager.Instance.UpdateSystemText(text);
			UIManager.Instance.IsWrongTextOpen = true; // true일 때는 Panel 안 내려가도록 제어함.
			yield return new WaitForSeconds(2f);
			UIManager.Instance.OpenTextPanel(true);
			UIManager.Instance.IsWrongTextOpen = false;
			UIManager.Instance.UpdateSystemText("");
		}
		
		// Update is called once per frame
		void Update()
		{
			// 현재 상태 검사
			// if (!(GameManager.Instance.curState == GameManager.CurState.Building)) return;
			
			if (!TowerCreate.isUpgrade) return;
			UIManager.Instance.UpdateSystemText("업그레이드에 사용될 타워를 선택해주세요.");

			if (Input.GetMouseButtonDown(0)) // tower 버튼을 누르고 디스크를 클릭 했을 때
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
					
					LevelUp(diskIndex, TowerCreate.upgradeDiskNum);
				}
			}
		}
	}

}