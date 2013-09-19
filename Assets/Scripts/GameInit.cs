using UnityEngine;
using System.Collections;

public class GameInit : MonoBehaviour {
	
	void Awake()
	{
		GameManager.Init();
		Destroy (gameObject);
	}
}
