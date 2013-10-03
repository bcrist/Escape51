using UnityEngine;
using System.Collections;

public enum AIState {
	HoldingPosition,
	Wandering,
	MovingToWaypoint1,
	MovingToWaypoint2,
	PursuingPlayer,
	AttackingPlayer,
	AvoidingPlayer
}

public class AIController : ActorController {
	
	public float sightRange;
	public float hearingRange;
	
	public float maxPreferredRange;
	public float minPreferredRange;
	
	protected GameObject player;
	
	protected AIState aiState;
	
	new void Update()
	{
		AIControllerUpdate_();
		ActorControllerUpdate_();
	}
	
	void AIControllerUpdate_()
	{
	}
	
	// Determine if the AI can see/hear the player
	bool isAwareOfPlayer()
	{
		Vector3 toPlayer = player.transform.position - transform.position;
		float range = toPlayer.magnitude;
		
		if (range < hearingRange)
			return true;
		
		float sightRangeScale = Vector3.Dot(toPlayer.normalized, facingDirection.normalized);
		sightRangeScale = Mathf.Max (0.0f, sightRangeScale);
		
		float sightRangeNormalized = hearingRange * (1.0f - sightRangeScale) + sightRange * (sightRangeScale);
		
		// do raycast to determine if there are any obstacles between the player and the AI.
		RaycastHit info;
		Physics.Raycast (transform.position, toPlayer, out info, sightRangeNormalized);
		return info.collider.gameObject.CompareTag("Player");
	}
}
