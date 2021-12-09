using System.Collections;
using System.Collections.Generic;
using MediaProject;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Enemy
{
	private readonly float[] fullHP = {100f, 200f, 300f};

	public override void TakeDamage(float damage, Quaternion dir)
	{
		base.TakeDamage(damage, dir);
		UIManager.Instance.UpdateHP(hp / fullHP[GameManager.Instance.currentStage/4 - 1]);
	}

	// Boss가 죽을 때 호출해서 사용.
    protected override void Dead()
    {
	    UIManager.Instance.OpenBossHPPanel();
    	GameManager.Instance.AddLiveEnemyNum(-1);
    	gameObject.SetActive(false);
    }
}
