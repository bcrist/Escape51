using UnityEngine;
using System;

public class BadassController : AIController
{
	protected override void BeginAttack()
	{
		Debug.Log ("Badass Attack!");
	}
}
