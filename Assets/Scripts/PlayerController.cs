using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float maxHorizontalVelocity = 3;
	public float horizontalForce = 10;
	public float jump = 600;
	public float jumpLength = 3;
	public float jumpTimeout = -10;
	
	public Material normalMaterial;
	public Material groundMaterial;
	
	private int jumpFramesLeft = 0;
	private Vector3 jumpForce;
	private Vector3 moveForce;
	
	// Use this for initialization
	void Start()
	{
		jumpForce = new Vector3(0, jump, 0);
		moveForce = new Vector3();
	}
	
	// Update is called once per frame
	void Update()
	{
		float horizontalVelocity = rigidbody.velocity.x;
		
		float deltaHV = maxHorizontalVelocity - Mathf.Abs(horizontalVelocity);
		
		if (deltaHV > 0)
		{
			float horizontal = Input.GetAxis ("Horizontal");
			
			//if (horizontal * moveForce.x < 0)
			//{
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
			//}
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
		
		bool onGround = Physics.Raycast(new Ray(rigidbody.position, -transform.up), centerHeight + 0.05f);
		bool onWallLeft = Physics.Raycast(new Ray(rigidbody.position, -transform.right), radius + 0.05f);
		bool onWallRight = Physics.Raycast(new Ray(rigidbody.position, transform.right), radius + 0.05f);
		
	
		if (onGround && Input.GetAxis("Vertical") > 0 && jumpFramesLeft <= jumpTimeout)
		{
			jumpFramesLeft = (int)jumpLength;
			//GetComponent<SpriteController>().startAnimation(1);
		}
		
		transform.localScale = new Vector3(1, (Mathf.Min (0,Input.GetAxis("Vertical")) + 3) / 3.0f, 1);
		
		if (onGround)
		{
			//renderer.material = groundMaterial;
			if (moveForce.x != 0)
				rigidbody.AddForce(Physics.gravity * -0.75f);
		}
		else
		{
			//renderer.material = normalMaterial;
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
				rigidbody.AddForce(jumpForce);
		}
	}
}
