using UnityEngine;
using System;

public class RangedAIController : AIController
{
	public float projectileVelocity;
	public Vector3 projectileConstantDirection;
	public Vector3 projectileConstantDirectionWeight;
	public Vector3 projectileFacingDirectionWeight;
	public float projectileStartPoint;
	public float projectileLifetime;
	public float projectileDamage;
	
	public float attackDelay = 0.05f;
	private float attackDelayLeft = 0;
	
	public string projectileResource;
	
	new public void Start()
	{
		ActorControllerStart_();
		AIControllerStart_();
	}
	
	new public void Update()
	{
		AIControllerUpdate_();
		
		if (attackDelayLeft > 0)
		{
			attackDelayLeft -= Time.deltaTime;
			
			if (attackDelayLeft <= 0)
			{
				Vector3 projectileVel = projectileConstantDirection;
				projectileVel *= projectileFacingDirectionWeight;
				
				Vector3 facingDir = facingDirection;
				facingDir *= projectileFacingDirectionWeight;
				
				projectileVel += facingDir;
				
				projectileVel.Normalize();
				
				Vector3 startPosition = transform.position + projectileVel * projectileStartPoint;
				
				projectileVel *= projectileVelocity;
				
				GameObject projectile = Instantiate (Resources.Load(projectileResource));
				
				projectile.transform.position = startPosition;
				projectile.rigidbody.velocity = projectileVel;
				ProjectileController pc = projectile.GetComponent<ProjectileController>();
				pc.lifetime = projectileLifetime;
				pc.damage = projectileDamage;
			}
		}
	}
	
	protected override void BeginAttack()
	{
		attackDelayLeft = attackDelay;
	}
}
