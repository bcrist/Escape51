using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {
	
	public string level;
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("onTriggerEnter");
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log ("Loading level: " + level);
			GameManager.instance.AwardPoints(1000);
			Application.LoadLevel(level);
		}
	}
}
