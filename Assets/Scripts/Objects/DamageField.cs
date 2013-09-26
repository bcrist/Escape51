using UnityEngine;
using System.Collections.Generic;

public class DamageField : MonoBehaviour {

	public float ratioHealing;
	public float ratioMissingHealing;
	public int fixedHealing;
	
	public float ratioDamage;
	public float ratioRemainingDamage;
	public int fixedDamage;
	
	public float interval;
	
	private float deltaT_;
	
	private List<ActorController> actors_;
	
	void Awake()
	{
		actors_ = new List<ActorController>();
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		ActorController ac = other.gameObject.GetComponent<ActorController>();
		
		if (ac != null && !actors_.Contains(ac))
		{
			actors_.Add(ac);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		ActorController ac = other.gameObject.GetComponent<ActorController>();
		
		if (ac != null)
		{
			actors_.Remove(ac);
		}
	}
	
	void Update()
	{
		deltaT_ += Time.deltaTime;
		
		while (deltaT_ > interval)
		{
			deltaT_ -= interval;
			DamageActors();
		}
	}
	
	void DamageActors()
	{
		foreach (ActorController ac in actors_)
		{
			if (ac != null)
			{
				if (ratioHealing > 0)
					ac.actorState.healRatio(ratioHealing);
				else if (ratioMissingHealing > 0)
					ac.actorState.healRatioMissing(ratioMissingHealing);
				else if (fixedHealing > 0)
					ac.actorState.heal(fixedHealing);
				
				if (ratioDamage > 0)
					ac.actorState.takeDamageRatio(ratioDamage);
				else if (ratioRemainingDamage > 0)
					ac.actorState.takeDamageRatioRemaining(ratioRemainingDamage);
				else if (fixedDamage > 0)
					ac.actorState.takeDamage(fixedDamage);
			}
		}
	}
}
