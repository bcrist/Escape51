using System;
using UnityEngine;

public class ActorController : MonoBehaviour
{
	public enum ActorAction {
		RunLeft,
		RunRight,
		Idle,
		Crouch,
		Jump
	}
	
	private enum JumpMode {
		JumpUp,
		JumpLeft,
		JumpRight
	}

	public float maxHorizontalVelocity = 2.5f;
	public float horizontalForce = 10;
	public float jumpForce = 600;
	public float jumpLength = 3;
	public float jumpTimeout = -10;	
	
	public int lIdleAnim = 0;
	public int rIdleAnim = 1;
	public int lFallingIdleAnim = 2;
	public int rFallingIdleAnim = 3;
	public int lWallGrabAnim = 4;
	public int rWallGrabAnim = 5;
	public int lRunAnim = 6;
	public int rRunAnim = 7;
	public int lJumpAnim = 8;
	public int rJumpAnim = 9;
	public int lJumpLeftAnim = 10;
	public int rJumpRightAnim = 11;
	public int lCrouchAnim = 12;
	public int rCrouchAnim = 13;
	
	public Vector3 leftFull = new Vector3(-1.0f, -0.15f, 0.0f);
	public Vector3 leftHalf = new Vector3(-1.0f, -0.2f, -1.0f);
	public Vector3 rightHalf = new Vector3(1.0f, -0.2f, -1.0f);
	public Vector3 rightFull = new Vector3(1.0f, -0.15f, 0.0f);
	
	
	public ActorState actorState;
	
	protected float horizontalIntention = 0;
	protected bool jumpIntention = false;
	protected bool crouchIntention = false;
	
	protected ActorAction currentAction = ActorAction.Idle;
	protected Vector3 facingDirection;
	protected int lastActiveAnimation;
	
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
		//ActorAction lastAction = currentAction;
		//float lastFacingDirection = facingDirection.x;
		
		
		
		// Calculate new currentAction, facingDirection, and jump* values.
		if (Mathf.Abs (horizontalIntention) > 0.1f)
			currentAction = horizontalIntention > 0 ? ActorAction.RunRight : ActorAction.RunLeft;
		else
			currentAction = ActorAction.Idle;
		
		if (crouchIntention) // && (!onGround || Mathf.Abs (hVelocity) < 0.2f))
			currentAction = ActorAction.Crouch;
		
		if (Mathf.Abs(hVelocity) > 0.1f)
			facingDirection = hVelocity > 0 ? rightFull : leftFull;
		else
			facingDirection = facingDirection.x > 0 ? rightHalf : leftHalf;
		
		if (jumpIntention)
		{
			if (jumpFramesLeft <= jumpTimeout)
			{
				if (onWallLeft && !onWallRight) {
					jumpFramesLeft = (int)jumpLength;
					jumpMode = JumpMode.JumpRight;
				} else if (onWallRight && !onWallLeft) {
					jumpFramesLeft = (int)jumpLength;
					jumpMode = JumpMode.JumpLeft;
				} else if (onGround) {
					jumpFramesLeft = (int)jumpLength;
					//if (currentAction == ActorAction.RunRight && hVelocity > 0.5f) {
					//	jumpMode = JumpMode.JumpRight;
					//} else if (currentAction == ActorAction.RunLeft && hVelocity < -0.5f) {
					//	jumpMode = JumpMode.JumpLeft;
					//} else {
						jumpMode = JumpMode.JumpUp;
					//}
				}
			}
		}
		
		if (jumpFramesLeft > 0)
			currentAction = ActorAction.Jump;
		
		
		
		
		// Update animation if necessary
		bool facingRight = facingDirection.x > 0;
		bool startAnimImmediately = false;
		int animationId = facingRight ? rIdleAnim : lIdleAnim;
		
		if (currentAction == ActorAction.Jump)
		{
			if (jumpMode == JumpMode.JumpLeft)
				animationId = lJumpLeftAnim;
			else if (jumpMode == JumpMode.JumpRight)
				animationId = rJumpRightAnim;
			else
				animationId = facingRight ? rJumpAnim : lJumpAnim;
			startAnimImmediately = true;
		}
		else if (!onGround)
		{
			if (currentAction == ActorAction.RunLeft)
				animationId = lRunAnim;
			else if (currentAction == ActorAction.RunRight)
				animationId = rRunAnim;
			else if (onWallLeft && currentAction == ActorAction.Crouch)
			{
				animationId = rWallGrabAnim;
				startAnimImmediately = true;
			}
			else if (onWallRight && currentAction == ActorAction.Crouch)
			{
				animationId = lWallGrabAnim;
				startAnimImmediately = true;
			}
			else 
				animationId = facingRight ? rFallingIdleAnim : lFallingIdleAnim;
		}
		else // on ground, not jumping
		{
			if (hVelocity > 0.2f || (hVelocity > 0 && currentAction == ActorAction.RunRight))
				animationId = rRunAnim;
			else if (hVelocity < -0.2f || (hVelocity < 0 && currentAction == ActorAction.RunLeft))
				animationId = lRunAnim;
			else if (currentAction == ActorAction.Crouch)
				animationId = facingRight ? rCrouchAnim : lCrouchAnim;
		}
		
		if (animationId != lastActiveAnimation)
		{
			lastActiveAnimation = animationId;
			if (startAnimImmediately)
				GetComponent<SpriteController>().startAnimation(animationId);
			else
				GetComponent<SpriteController>().queueAnimation(animationId);
		}
		
		
		
		// Apply forces based on currentAction
		if (currentAction == ActorAction.Crouch)
		{
			if (onGround)
			{
				rigidbody.AddForce (Physics.gravity * 2.0f);
			}
			else if (onWallLeft || onWallRight)
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
					rigidbody.AddForce (new Vector3((currentAction == ActorAction.RunRight ? horizontalForce : -horizontalForce), 0.0f, 0.0f));
			}
			else if (Mathf.Abs (hVelocity) < maxHorizontalVelocity)
			{
				if (!onWallLeft && currentAction == ActorAction.RunLeft)
				{
					rigidbody.AddForce(new Vector3(horizontalForce * -0.75f, 0.0f, 0.0f));
				}
				else if (!onWallRight && currentAction == ActorAction.RunRight)
				{
					rigidbody.AddForce(new Vector3(horizontalForce * 0.75f, 0.0f, 0.0f));
				}
			}	
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
