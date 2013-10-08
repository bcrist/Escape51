using UnityEngine;
using System.Collections;

public class PlayerController : ActorController {
	
	new void Start()
	{
		ActorControllerStart_();
		actorState = GameManager.instance.playerState;
	}
	
	void Update()
	{
		horizontalIntention = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		jumpIntention = vertical > 0;
		crouchIntention = vertical < 0;
	}
}
