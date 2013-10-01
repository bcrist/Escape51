using System;
using UnityEngine;

public class ActorController : MonoBehaviour
{
	public ActorState actorState;
	
	protected bool wantsToMoveLeft;
	protected bool wantsToMoveRight;
	protected bool wantsToCrouch;
	protected bool wantsToJump;
	
	public float maxHorizontalVelocity = 2.5;
	public float horizontalForce = 10;
	public float jumpForce = 600;
	public float jumpLength = 3;
	public float jumpTimeout = -10;

	
	
	
	
	protected int jumpFramesLeft = 0;
	protected Vector3 jumpUpForce;
	protected Vector3 jumpLeftForce;
	protected Vector3 jumpRightForce;
	protected Vector3 moveForce;
	protected int jumpMode = 0;
	
	public void Start()
	{
		ActorControllerStart_ ();
	}
	
	public void Update()
	{
		ActorControllerUpdate_();
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
		
		jumpForce = new Vector3(0, jump * 0.9f, 0);
		jumpLeftForce = new Vector3(-jump * 0.5f, jump * 0.7f, 0);
		jumpRightForce = new Vector3(jump * 0.5f, jump * 0.7f, 0);
		moveForce = new Vector3();
	}
	
	protected void ActorControllerUpdate_()
	{
	}
	
	protected void ActorControllerFixedUpdate_()
	{
	}
	
	protected void ActorControllerOnDestroy_()
	{
		if (healthbarRenderer != null)
			Destroy(healthbarRenderer.material);
	}
	
	private Renderer healthbarRenderer;
}
