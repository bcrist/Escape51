using System;
using UnityEngine;

public class ActorController : MonoBehaviour
{
	public ActorState actorState;
	
	public void Start()
	{
		healthbarRenderer = transform.Find("Healthbar").GetComponent<Renderer>();
		if (healthbarRenderer != null)
		{
			actorState = new ActorState(healthbarRenderer);
		}
	}
	
	public void OnDestroy()
	{
		if (healthbarRenderer != null)
			Destroy(healthbarRenderer.material);
	}
	
	private Renderer healthbarRenderer;
}
