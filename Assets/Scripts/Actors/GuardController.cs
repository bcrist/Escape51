using UnityEngine;
using System;

public class GuardController : AIController
{
	
	protected override void BeginAttack()
	{
		Debug.Log ("Guard Attack!");
	}
}
