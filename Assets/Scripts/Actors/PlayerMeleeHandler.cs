using UnityEngine;
using System.Collections;

public class PlayerMeleeHandler : MonoBehaviour {
	
	private PlayerController pc;
	
	void Start()
	{
		pc = transform.parent.gameObject.GetComponent<PlayerController>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		ActorController otherAC = other.gameObject.GetComponent<ActorController>();
		if (otherAC)
			otherAC.actorState.takeDamage(pc.attackDamage);
	}
}
