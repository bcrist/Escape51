using System;
using UnityEngine;

[System.Serializable]
public class Animations
{
	public int lIdle = 0;
	public int rIdle = 1;
	public int lFallingIdle = 2;
	public int rFallingIdle = 3;
	public int lWallGrab = 4;
	public int rWallGrab = 5;
	public int lRun = 6;
	public int rRun = 7;
	public int lJump = 8;
	public int rJump = 9;
	public int lJumpLeft = 10;
	public int rJumpRight = 11;
	public int lCrouch = 12;
	public int rCrouch = 13;
	public int lAttack = 14;
	public int rAttack = 15;
	public int lRunningAttack = 16;
	public int rRunningAttack = 17;
	public int lDeath = 18;
	public int rDeath = 19;
}

public class ActorController : MonoBehaviour
{
	public enum ActorAction {
		RunLeft,
		RunRight,
		Idle,
		Crouch,
		Jump,
		Attack,
		RunLeftAttack,
		RunRightAttack
	}
	
	private enum JumpMode {
		JumpUp,
		JumpLeft,
		JumpRight
	}

	public float maxHorizontalVelocity = 2.5f;
	public float maxHorizontalVelocityWhileAttacking = 1.0f;
	public float horizontalForce = 10;
	public float jumpForce = 600;
	public float jumpLength = 3;
	public float jumpTimeout = -10;	
	public bool canWallJump = true;
	public float attackTimeout = 0.4f;
	public float movingAttackTimeout = 0.6f;
	public float normalHeight = 0.9f;
	public float normalWidth = 0.4f;
	public Vector3 normalOffset;
	public float crouchHeight = 0.5f;
	public float crouchWidth = 0.5f;
	public Vector3 crouchOffset;
	public float deadHeight = 0.8f;
	public float deadWidth = 0.3f;
	public Vector3 deadOffset;
	
	public Animations animations;
		
	public Vector3 leftFull = new Vector3(-1.0f, -0.15f, 0.0f);
	public Vector3 leftHalf = new Vector3(-1.0f, -0.2f, -1.0f);
	public Vector3 rightHalf = new Vector3(1.0f, -0.2f, -1.0f);
	public Vector3 rightFull = new Vector3(1.0f, -0.15f, 0.0f);
	
	public ActorState actorState;
	
	protected float horizontalIntention = 0;
	protected float horizontalLookIntention = 0;
	protected bool jumpIntention = false;
	protected bool crouchIntention = false;
	protected bool attackIntention = false;
	protected bool alive = true;
	
	protected ActorAction currentAction = ActorAction.Idle;
	protected Vector3 facingDirection;
	protected int lastActiveAnimation;
	protected bool isCrouching;
	
	private float attackTimer = 0; 
	
	private Renderer healthbarRenderer;
	
	private int jumpFramesLeft = 0;
	private Vector3 jumpUpForce;
	private Vector3 jumpLeftForce;
	private Vector3 jumpRightForce;
	
	private JumpMode jumpMode = JumpMode.JumpUp;
	
	public void Start()
	{
		ActorControllerStart_ ();
	}
	
	public void FixedUpdate()
	{
		ActorControllerFixedUpdate_();
	}
	
	public void OnDestroy()
	{
		ActorControllerOnDestroy_();
	}
	
	protected virtual void BeginAttack() 
	{
	}
	
	protected void ActorControllerStart_()
	{
		Transform healthbar = transform.Find ("Healthbar");
		if (healthbar != null)
		{
			healthbarRenderer = healthbar.GetComponent<Renderer>();
			if (healthbarRenderer != null)
			{
				actorState = new ActorState(healthbarRenderer);
			}
		}
		
		jumpUpForce = new Vector3(0, jumpForce * 0.9f, 0);
		jumpLeftForce = new Vector3(-jumpForce * 0.5f, jumpForce * 0.8f, 0);
		jumpRightForce = new Vector3(jumpForce * 0.5f, jumpForce * 0.8f, 0);
		
		leftFull.Normalize();
		leftHalf.Normalize();
		rightHalf.Normalize();
		rightFull.Normalize();
		
		facingDirection = leftHalf;
	}
	
	
	protected void ActorControllerFixedUpdate_()
	{
		// Calculate some useful stats about the current state of the actor
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		Ray down = new Ray(transform.position + collider.center, transform.up * -1f);
		float downOffset = collider.height * 0.5f + 0.05f;
		
		Ray left = new Ray(transform.position + collider.center, transform.right * -1f);
		float leftOffset = collider.radius + 0.1f;
		
		Ray right = new Ray(transform.position + collider.center, transform.right);
		float rightOffset = collider.radius + 0.1f;
		
		bool onGround = Physics.Raycast(down, downOffset);
		bool onWallLeft = Physics.Raycast(left, rightOffset);
		bool onWallRight = Physics.Raycast(right, leftOffset);
		
		float hVelocity = rigidbody.velocity.x;
		
		// Calculate new currentAction, facingDirection, and jump* values.
		if (alive)
		{
			if (Mathf.Abs (horizontalIntention) > 0.1f)
				currentAction = horizontalIntention > 0 ? ActorAction.RunRight : ActorAction.RunLeft;
			else
				currentAction = ActorAction.Idle;
			
			if (crouchIntention) // && (!onGround || Mathf.Abs (hVelocity) < 0.2f))
				currentAction = ActorAction.Crouch;
			
			if (Mathf.Abs(hVelocity) > 0.1f)
				facingDirection = hVelocity > 0 ? rightFull : leftFull;
			else if (Mathf.Abs (horizontalLookIntention) > 0.1f)
				facingDirection = horizontalLookIntention > 0 ? rightFull : leftFull;
			else
				facingDirection = facingDirection.x > 0 ? rightHalf : leftHalf;
			
			if (jumpIntention)
			{
				if (jumpFramesLeft <= jumpTimeout)
				{
					if (canWallJump && onWallLeft && !onWallRight) {
						jumpFramesLeft = (int)jumpLength;
						jumpMode = JumpMode.JumpRight;
						facingDirection = rightFull;
					} else if (canWallJump && onWallRight && !onWallLeft) {
						jumpFramesLeft = (int)jumpLength;
						jumpMode = JumpMode.JumpLeft;
						facingDirection = leftFull;
					} else if (onGround) {
						jumpFramesLeft = (int)jumpLength;
						jumpMode = JumpMode.JumpUp;
					}
				}
			}
			
			if (jumpFramesLeft > 0)
				currentAction = ActorAction.Jump;
			
			
			// Update attack info
			if (attackTimer > 0)
				attackTimer -= Time.fixedDeltaTime;
			else if (attackIntention)
			{
				if (currentAction == ActorAction.Idle)
				{
					currentAction = ActorAction.Attack;
					attackTimer = attackTimeout;
				}
				else if (currentAction == ActorAction.RunLeft)
				{
					currentAction = ActorAction.RunLeftAttack;
					attackTimer = movingAttackTimeout;
				}
				else if (currentAction == ActorAction.RunRight)
				{
					currentAction = ActorAction.RunRightAttack;
					attackTimer = movingAttackTimeout;
				}
				
				BeginAttack();
			}
		}
		else // dead
		{
			currentAction = ActorAction.Idle;
		}
		
		
		// Update animation if necessary
		bool facingRight = facingDirection.x > 0;
		bool startAnimImmediately = false;
		bool actorCrouching = false;
		int animationId = facingRight ? animations.rIdle : animations.lIdle;
		
		if (!alive)
		{
			animationId = facingRight ? animations.rDeath : animations.lDeath;
			startAnimImmediately = true;
		}
		else if (currentAction == ActorAction.Jump)
		{
			if (jumpMode == JumpMode.JumpLeft)
				animationId = animations.lJumpLeft;
			else if (jumpMode == JumpMode.JumpRight)
				animationId = animations.rJumpRight;
			else
				animationId = facingRight ? animations.rJump : animations.lJump;
			startAnimImmediately = true;
		}
		else if (!onGround)
		{
			if (currentAction == ActorAction.RunLeft)
				animationId = animations.lRun;
			else if (currentAction == ActorAction.RunRight)
				animationId = animations.rRun;
			else if (currentAction == ActorAction.Attack)
			{
				animationId = facingRight ? animations.rAttack : animations.lAttack;
				startAnimImmediately = true;
			}
			else if (currentAction == ActorAction.RunLeftAttack)
			{
				animationId = animations.lRunningAttack;
				startAnimImmediately = true;
			}
			else if (currentAction == ActorAction.RunRightAttack)
			{
				animationId = animations.rRunningAttack;
				startAnimImmediately = true;
			}
			else if (onWallLeft && currentAction == ActorAction.Crouch)
			{
				animationId = animations.rWallGrab;
				facingDirection = rightFull;	// this may be a confusing place for this but anywhere else would require redundant if statements
				startAnimImmediately = true;
			}
			else if (onWallRight && currentAction == ActorAction.Crouch)
			{
				animationId = animations.lWallGrab;
				facingDirection = leftFull;		// this may be a confusing place for this but anywhere else would require redundant if statements
				startAnimImmediately = true;
			}
			else 
				animationId = facingRight ? animations.rFallingIdle : animations.lFallingIdle;
			
			if (isCrouching && currentAction == ActorAction.Crouch)
				actorCrouching = true;
		}
		else // on ground, not jumping
		{
			if (currentAction == ActorAction.Attack ||
				currentAction == ActorAction.RunLeftAttack ||
				currentAction == ActorAction.RunRightAttack)
			{
				startAnimImmediately = true;
				if (hVelocity > 0.2f || (hVelocity > 0 && currentAction == ActorAction.RunRightAttack))
					animationId = animations.rRunningAttack;
				else if (hVelocity < -0.2f || (hVelocity < 0 && currentAction == ActorAction.RunLeftAttack))
					animationId = animations.lRunningAttack;
				else 
					animationId = facingRight ? animations.rAttack : animations.lAttack;
			}
			else if (hVelocity > 0.2f || (hVelocity > 0 && currentAction == ActorAction.RunRight))
				animationId = animations.rRun;
			else if (hVelocity < -0.2f || (hVelocity < 0 && currentAction == ActorAction.RunLeft))
				animationId = animations.lRun;
			else if (currentAction == ActorAction.Crouch)
			{
				animationId = facingRight ? animations.rCrouch : animations.lCrouch;
				actorCrouching = true;
			}
		}
		
		if (actorCrouching != isCrouching)
		{
			isCrouching = actorCrouching;
			if (isCrouching)
			{
				collider.height = crouchHeight;
				collider.radius = crouchWidth * 0.5f;
				collider.center = crouchOffset;
				//collider.direction = 1; // y axis
			}
			else
			{
				collider.height = normalHeight;
				collider.radius = normalWidth * 0.5f;
				collider.center = normalOffset;
				//collider.direction = 1; // y axis
			}
		}
		
		if (animationId != lastActiveAnimation)
		{
			lastActiveAnimation = animationId;
			if (startAnimImmediately)
				GetComponent<SpriteController>().startAnimation(animationId);
			else
				GetComponent<SpriteController>().queueAnimation(animationId);
			
			if (!alive)
			{
				collider.height = deadHeight;
				collider.radius = deadWidth * 0.5f;
				collider.center = deadOffset;
				//collider.direction = 0; // change capsule direction to x axis.
			}
		}
	
		
		// Apply forces based on currentAction
		if (currentAction == ActorAction.Crouch)
		{
			if (onGround)
			{
				rigidbody.AddForce (Physics.gravity * 2.0f);
			}
			else if (canWallJump && (onWallLeft || onWallRight))
			{
				Vector3 newVel = rigidbody.velocity;
				newVel.x *= 0.2f;
				newVel.y *= 0.9f;
				rigidbody.velocity = newVel;
				rigidbody.AddForce (Physics.gravity * -1.0f);
			}
		}
		if (currentAction == ActorAction.RunLeft || currentAction == ActorAction.RunRight)
		{
			if (onGround)
			{
				rigidbody.AddForce(Physics.gravity * -0.75f);
				
				if (Mathf.Abs (hVelocity) < maxHorizontalVelocity)
					rigidbody.AddForce ((currentAction == ActorAction.RunRight ? horizontalForce : -horizontalForce), 0.0f, 0.0f);
			}
			else if (Mathf.Abs (hVelocity) < maxHorizontalVelocity)
			{
				if (!onWallLeft && currentAction == ActorAction.RunLeft)
				{
					rigidbody.AddForce(horizontalForce * -0.75f, 0.0f, 0.0f);
				}
				else if (!onWallRight && currentAction == ActorAction.RunRight)
				{
					rigidbody.AddForce(horizontalForce * 0.75f, 0.0f, 0.0f);
				}
			}	
		}
		else if (currentAction == ActorAction.RunLeftAttack || currentAction == ActorAction.RunRightAttack)
		{
			if (onGround)
			{
				rigidbody.AddForce(Physics.gravity * -0.75f);
				
				if (Mathf.Abs (hVelocity) < maxHorizontalVelocityWhileAttacking)
					rigidbody.AddForce ((currentAction == ActorAction.RunRightAttack ? horizontalForce : -horizontalForce), 0.0f, 0.0f);
			}
			else if (Mathf.Abs (hVelocity) < maxHorizontalVelocityWhileAttacking)
			{
				if (!onWallLeft && currentAction == ActorAction.RunLeftAttack)
				{
					rigidbody.AddForce(horizontalForce * -0.75f, 0.0f, 0.0f);
				}
				else if (!onWallRight && currentAction == ActorAction.RunRightAttack)
				{
					rigidbody.AddForce(horizontalForce * 0.75f, 0.0f, 0.0f);
				}
			}	
		}
		else if (currentAction == ActorAction.Idle && !alive && onGround)
		{
			rigidbody.AddForce (Physics.gravity * 6.0f);
		}
		
		if (jumpFramesLeft > jumpTimeout)
		{
			if (jumpFramesLeft > 0)
			{
				if (jumpFramesLeft == jumpLength)
				{
					Vector3 newVel = rigidbody.velocity;
					newVel.y = Mathf.Abs(newVel.y) * 0.3f;
					if (jumpMode != JumpMode.JumpUp)
						newVel.x = Mathf.Abs (newVel.x) * 0.1f;
					rigidbody.velocity = newVel;
				}
				
				switch (jumpMode)
				{
				case JumpMode.JumpUp:
					rigidbody.AddForce(jumpUpForce);
					break;
				case JumpMode.JumpLeft:
					rigidbody.AddForce(jumpLeftForce);
					facingDirection = leftFull;
					break;
				case JumpMode.JumpRight:
					rigidbody.AddForce(jumpRightForce);
					facingDirection = rightFull;
					break;
				default:
					break;
				}
			}
			--jumpFramesLeft;
		}
	}
	
	protected void ActorControllerOnDestroy_()
	{
		if (healthbarRenderer != null)
			Destroy(healthbarRenderer.material);
	}
}
