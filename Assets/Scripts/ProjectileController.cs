using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {
	
	
	public int damage = 0;
	public float lifetime = 0;
	
	private float deltaT;
	
	void OnTriggerEnter(Collider other)
	{
		ActorController otherAC = other.gameObject.GetComponent<ActorController>();
		if (otherAC)
			otherAC.actorState.takeDamage(damage);
	}
	
	void Start()
	{
		deltaT = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		deltaT += Time.deltaTime;
		
		if (deltaT > lifetime)
			Destroy (gameObject);
	}
}
