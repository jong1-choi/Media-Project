using System.Collections;
using System.Collections.Generic;
using MediaProject;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Enemy
{
	public override void TakeDamage(float damage, Quaternion dir)
	{
		base.TakeDamage(damage, dir);
		if(GameManager.Instance.currentStage == GameManager.Instance.stageEnemyNumList.Length) return;
		UIManager.Instance.UpdateHP(hp / GameManager.maxHP[GameManager.Instance.currentStage]);
	}

	// Boss가 죽을 때 호출해서 사용.
    protected override void Dead()
    {
	    UIManager.Instance.OpenBossHPPanel();
    	GameManager.Instance.AddLiveEnemyNum(-1);
        isArrived = true;
    	gameObject.SetActive(false);
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
	    if (other.CompareTag("Destination"))
	    {
		    GameManager.Instance.AddLife(-5);
		    GameManager.Instance.AddLiveEnemyNum(-1);
		    AudioManager.Instance.Play(1);
		    isArrived = true;
		    gameObject.SetActive(false);
		    UIManager.Instance.OpenBossHPPanel();
	    }
    }
}
