using UnityEngine;
using System.Collections;

public enum AIState {
	HoldingPosition,
	Wandering,
	MovingToWaypoint1,
	MovingToWaypoint2,
	PursuingPlayer
}

public class AIController : ActorController {
	
	public float sightRange;
	public float hearingRange;
	public float attackRange;
	
	public float maxPreferredRange;
	public float minPreferredRange;
	
	public float attentionSpan;	// amount of time between choosing a new task when idle
	
	public bool enableIdleHoldPosition;
	public bool enableIdleWander;
	public bool enableIdlePatrol;
	
	public Vector3 patrolWaypoint1;
	public Vector3 patrolWaypoint2;
	public float waypointLaziness;	// distance from waypoint at which it is considered achieved.
	
	
	
	protected GameObject player;
	
	protected AIState currentState = AIState.HoldingPosition;
	
	private float attentionLeft = 0;
	private float wanderLeft = 0;
	
	new public void Start()
	{
		AIControllerStart_();
		ActorControllerStart_();
	}
	
	void Update()
	{
		AIControllerUpdate_();
	}
	
	void AIControllerStart_()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void AIControllerUpdate_()
	{
		Vector3 toPlayer = player.transform.position - transform.position;
		float range = toPlayer.magnitude;
		bool seesPlayer = IsAwareOfPlayer(toPlayer, range);
		
		//Debug.Log("sees: " +seesPlayer);
		
		alive = !actorState.isDead();
		
		if (currentState == AIState.PursuingPlayer)
		{
			if (seesPlayer)
			{
				if (range > maxPreferredRange)
					horizontalIntention = toPlayer.x;
				else if (range < minPreferredRange)
					horizontalIntention = -toPlayer.x;
				else
					horizontalIntention = 0;
				attackIntention = range < attackRange;
				jumpIntention = false;
				crouchIntention = horizontalIntention == 0;
			}
			else
			{
				currentState = AIState.HoldingPosition;
				attackIntention = false;
				jumpIntention = false;
				crouchIntention = false;
				attentionLeft = 0;
			}
		}
		
		if (attentionLeft > 0)
		{
			attentionLeft -= Time.deltaTime;
			
			if (seesPlayer)
				currentState = AIState.PursuingPlayer;
			else
			{
				if (currentState == AIState.HoldingPosition)
				{
					horizontalIntention = 0;
					attackIntention = false;
					jumpIntention = false;
					crouchIntention = false;
				}
				else if (currentState == AIState.MovingToWaypoint1)
				{
					Vector3 toWaypoint = patrolWaypoint1 - transform.position;
					float wrange = toWaypoint.magnitude;
					horizontalIntention = toWaypoint.x;
					if (wrange < waypointLaziness)
						currentState = AIState.MovingToWaypoint2;
				}
				else if (currentState == AIState.MovingToWaypoint2)
				{
					Vector3 toWaypoint = patrolWaypoint2 - transform.position;
					float wrange = toWaypoint.magnitude;
					horizontalIntention = toWaypoint.x;
					if (wrange < waypointLaziness)
						currentState = AIState.MovingToWaypoint1;
				}
				else if (currentState == AIState.Wandering)
				{
					if (wanderLeft > 0)
					{
						wanderLeft -= Time.deltaTime;
					}
					else
					{
						wanderLeft = 0.2f;
						if (Random.value > 0.5f)
						{
							if (Random.value > 0.4f)
								horizontalIntention = 0;
							else if (Random.value > 0.5f)
								horizontalIntention = 1;
							else
								horizontalIntention = -1;
						}
					}
				}
			}
		}
		else
		{
			float rand = Random.value;
			if (rand > 0.5f)
			{
				if (enableIdleHoldPosition && rand < 0.7f)
				{
					currentState = AIState.HoldingPosition;
				}
				else if (enableIdlePatrol && rand < 0.85f)
				{
					currentState = AIState.MovingToWaypoint1;
				}
				else if (enableIdleWander)
				{
					currentState = AIState.Wandering;
					wanderLeft = 0;
				}
			}
			attentionLeft = attentionSpan;
		}
	}
	
	// Determine if the AI can see/hear the player
	bool IsAwareOfPlayer(Vector3 toPlayer, float range)
	{
		if (range < hearingRange)
			return true;
		
		float sightRangeScale = Vector3.Dot(toPlayer.normalized, facingDirection.normalized);
		sightRangeScale = Mathf.Max (0.0f, sightRangeScale);
		
		float sightRangeNormalized = hearingRange * (1.0f - sightRangeScale) + sightRange * (sightRangeScale);
		
		// do raycast to determine if there are any obstacles between the player and the AI.
		RaycastHit info;
		bool hit = Physics.Raycast (transform.position, toPlayer, out info, sightRangeNormalized);
		if (!hit)
			return false;
		return info.collider.gameObject.CompareTag("Player");
	}
}
