using System;
using UnityEngine;

public class ActorState
{
	public ActorState (Renderer healthbarRenderer)
	{
		healthbarRenderer_ = healthbarRenderer;
		
		maxHealth_ = 1000;
		limpHealth_ = 0;
		health_ = maxHealth_;
	}
		
	public int maxHealth {
		get { return maxHealth_; }
		set {
			float limpRatio = limpHealth_ / maxHealth_;
			float healthRatio = health_ / maxHealth_;
			
			maxHealth_ = value;
			
			limpHealth = (int)(value * limpRatio);
			health = (int)(value * healthRatio);
		}
	}
	
	public int limpHealth {
		get { return limpHealth_; }
		set { limpHealth_ = Mathf.Max (Mathf.Min (value, maxHealth_), 0); }
	}
	
	public int health {
		get { return health_; }
		set {
			health_ = Mathf.Max (Mathf.Min(value, maxHealth_), 0);
			
			if (healthbarRenderer_ != null)
				healthbarRenderer_.material.mainTextureOffset = new Vector2(getHealthRatioMissing() * 0.5f, 0);
		}
	}
	
	public float getHealthRatio()
	{
		return health / (float)maxHealth;
	}
	
	public float getHealthRatioMissing()
	{
		return 1 - getHealthRatio();
	}
	
	// Determines if the actor's health is below a certain limit.
	public bool isLimping()
	{
		return health_ <= limpHealth_;
	}
	
	// Determines if the actor is dead or alive.
	public bool isDead()
	{
		return health_ <= 0;
	}
	
	// heal a fixed amount of health
	public void heal(int amount)
	{
		if (!isDead())
			health += Mathf.Max(amount, 1);
	}
	
	// heal an amount equal to a percentage of the actor's maximum health
	public void healRatio(float ratio)
	{
		if (!isDead())
			health += Mathf.Max((int)(maxHealth * ratio), 1);
	}
	
	// Heal an amount equal to a percentage of the damage the actor has sustained.
	public void healRatioMissing(float ratio)
	{
		if (!isDead())
			health += Mathf.Max((int)((maxHealth - health) * ratio), 1);
	}
	
	// Take a fixed amount of damage.
	public void takeDamage(int amount)
	{
		health -= Mathf.Max(amount, 1);
	}

	// take damage equal to a percentage of the actor's maximum health
	public void takeDamageRatio(float ratio)
	{
		health -= Mathf.Max((int)(maxHealth * ratio), 1);
	}
	
	// take damage equal to a percentage of the actor's remaining health
	public void takeDamageRatioRemaining(float ratio)
	{
		health -= Mathf.Max((int)(health * ratio), 1);
	}
	
	private Renderer healthbarRenderer_;
	private int health_;
	private int maxHealth_;
	private int limpHealth_;
}

