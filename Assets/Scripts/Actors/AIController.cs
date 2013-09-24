using UnityEngine;
using System.Collections;

public class AIController : ActorController {
	
	// Use this for initialization
	void Start()
	{
		actorState = new ActorState();
		actorState.healthBarRenderer = transform.Find("Healthbar").GetComponent<MeshRenderer>();
	}
	
	void OnDestroy()
	{
		Destroy(transform.Find ("Healthbar").renderer.material);
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	void FixedUpdate()
	{
	}
}
