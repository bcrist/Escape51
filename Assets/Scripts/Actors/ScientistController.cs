using UnityEngine;
using System;

public class ScientistController : AIController
{
	protected override void BeginAttack()
	{
		Debug.Log ("Scientist Attack!");
	}
}
