using UnityEngine;
using System.Collections;

public class AIMeleeHandler : MonoBehaviour {

	private MeleeAIController maic;
	
	void Start()
	{
		maic = transform.parent.gameObject.GetComponent<MeleeAIController>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		ActorController otherAC = other.gameObject.GetComponent<ActorController>();
		if (otherAC)
			otherAC.actorState.takeDamage(maic.attackDamage);
	}
}
