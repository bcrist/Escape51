using UnityEngine;
using System;

public class MeleeAIController : AIController
{
	public float attackDelay = 0.05f;
	public int attackLength = 5;
	public int attackDamage = 50;
	
	private float attackDelayLeft = 0;
	private int attackLengthLeft = 0;
	
	private Collider leftMeleeCollider;
	private Collider rightMeleeCollider;
	
	
	new public void Start()
	{
		ActorControllerStart_();
		AIControllerStart_();
		
		leftMeleeCollider = transform.Find("Left Melee").GetComponent<Collider>();
		rightMeleeCollider = transform.Find ("Right Melee").GetComponent<Collider>();
	}
	
	new public void FixedUpdate()
	{
		ActorControllerFixedUpdate_();
		MeleeAIControllerFixedUpdate_();
	}
	
	new public void Update()
	{
		AIControllerUpdate_();
		
		if (attackDelayLeft > 0)
		{
			attackDelayLeft -= Time.deltaTime;
			
			if (attackDelayLeft <= 0)
			{
				// begin attack
				Collider collider = facingDirection.x > 0 ? rightMeleeCollider : leftMeleeCollider;
				
				collider.enabled = true;
				attackLengthLeft = attackLength;
			}
		}
	}
	
	protected void MeleeAIControllerFixedUpdate_()
	{
		if (attackLengthLeft > 0)
		{
			--attackLengthLeft;
			
			if (attackLengthLeft <= 0)
			{
				leftMeleeCollider.enabled = false;
				rightMeleeCollider.enabled = false;
			}
		}
	}
	
	protected override void BeginAttack()
	{
		attackDelayLeft = attackDelay;
	}
}
