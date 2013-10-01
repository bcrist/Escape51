using UnityEngine;
using System.Collections;

public class PlayerController : ActorController {
	
	new void Start()
	{
		Start_();
		
		actorState = GameManager.instance.playerState;
	}
	
	// Update is called once per frame
	void Update()
	{
		float horizontalVelocity = rigidbody.velocity.x;
		
		float deltaHV = maxHorizontalVelocity - Mathf.Abs(horizontalVelocity);
		
		if (deltaHV > 0)
		{
			float horizontal = Input.GetAxis ("Horizontal");
			
			if (horizontal > 0)
			{
				GetComponent<SpriteController>().startAnimationIfNotPlaying(0);
			}
			else if (horizontal < 0)
			{
				GetComponent<SpriteController>().startAnimationIfNotPlaying(1);
			}
			else
			{
				GetComponent<SpriteController>().startAnimationIfNotPlaying(-1);
			}
	
			moveForce.x = horizontal * horizontalForce;
		}
		else
		{
			moveForce.x = 0;
		}
	}
	
	void FixedUpdate()
	{
		float centerHeight = transform.lossyScale.y * 0.5f;
		float radius = transform.lossyScale.x * 0.5f;
		
		bool onGround = Physics.Raycast(rigidbody.position, -transform.up, centerHeight + 0.05f);
		bool onWallLeft = Physics.Raycast(rigidbody.position, -transform.right, radius + 0.05f);
		bool onWallRight = Physics.Raycast(rigidbody.position, transform.right, radius + 0.05f);
		
	
		if (onGround && Input.GetAxis("Vertical") > 0 && jumpFramesLeft <= jumpTimeout)
		{
			jumpFramesLeft = (int)jumpLength;
			jumpMode = 0;
			//GetComponent<SpriteController>().startAnimation(1);
		}
		else if (onWallLeft && !onWallRight && Input.GetAxis ("Vertical") > 0 && jumpFramesLeft <= jumpTimeout)
		{
			jumpFramesLeft = (int)jumpLength;
			jumpMode = 1;
		}
		else if (onWallRight && !onWallLeft && Input.GetAxis("Vertical") > 0 && jumpFramesLeft <= jumpTimeout)
		{
			jumpFramesLeft = (int)jumpLength;
			jumpMode = 2;
		}
		
		transform.localScale = new Vector3(1, (Mathf.Min (0,Input.GetAxis("Vertical")) + 3) / 3.0f, 1);
		
		if (onGround)
		{
			if (moveForce.x != 0)
				rigidbody.AddForce(Physics.gravity * -0.75f);
		}
			
		if (!(onWallLeft && moveForce.x < 0 || onWallRight && moveForce.x > 0))
		{
			Vector3 force = moveForce;
			if (!onGround) force *= 0.75f;
			rigidbody.AddForce(force);
		}
		
		if (jumpFramesLeft > jumpTimeout)
		{
			--jumpFramesLeft;
			if (jumpFramesLeft > 0)
			{
				switch (jumpMode)
				{
				case 0:
					rigidbody.AddForce (rigidbody.velocity * rigidbody.velocity.magnitude * -1f);
					rigidbody.AddForce(jumpForce); break;
				case 1:
					rigidbody.AddForce (rigidbody.velocity * rigidbody.velocity.magnitude  * -1.25f);
					rigidbody.AddForce(jumpRightForce); break;
				case 2:
					rigidbody.AddForce (rigidbody.velocity * rigidbody.velocity.magnitude  * -1.25f);
					rigidbody.AddForce(jumpLeftForce); break;
				default:
					break;
				}
			}
		}
	}
}
