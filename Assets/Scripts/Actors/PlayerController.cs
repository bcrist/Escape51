using UnityEngine;
using System.Collections;

public class PlayerController : ActorController {
	
	public float attackDelay = 0.05f;
	public int attackLength = 5;
	public int attackDamage = 50;
	
	private float attackDelayLeft = 0;
	private int attackLengthLeft = 0;
	
	private Collider leftMeleeCollider;
	private Collider rightMeleeCollider;
	
	new void Start()
	{
		ActorControllerStart_();
		actorState = GameManager.instance.playerState;
		
		leftMeleeCollider = transform.Find("Left Melee").GetComponent<Collider>();
		rightMeleeCollider = transform.Find ("Right Melee").GetComponent<Collider>();
	}
	
	new void FixedUpdate()
	{
		ActorControllerFixedUpdate_();
		PlayerControllerFixedUpdate_();
	}
	
	void Update()
	{
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
		
		Vector3 screenPos = Input.mousePosition;
		screenPos.z = Camera.main.GetComponent<CameraController>().cameraOffset.z * -1.0f;
		horizontalLookIntention = Camera.main.ScreenToWorldPoint(screenPos).x - transform.position.x;
		
		horizontalIntention = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		attackIntention = Input.GetButton("Fire1");
		jumpIntention = vertical > 0;
		crouchIntention = vertical < 0;
		
		alive = !actorState.isDead();
	}
	
	void PlayerControllerFixedUpdate_()
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
