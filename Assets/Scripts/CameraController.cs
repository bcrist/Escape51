using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private GameObject player;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = player.transform.position;
		pos.z = transform.position.z;
		
		transform.position = pos;
	}
}
