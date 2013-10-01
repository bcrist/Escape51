using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float posK1 = 20.0f;
	public float posK2 = 0.5f;
	public float velK1 = 5.0f;
	public float velK2 = 0.33f;
	
	public Vector3 cameraOffset;
	
	private GameObject player;
	
	private Vector3 velocity_;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		
		velocity_ = new Vector3();
		
		transform.position = player.transform.position + cameraOffset;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 target_position_ = player.transform.position + cameraOffset;
		Vector3 target_velocity_ = player.rigidbody.velocity;
		
		Vector3 pos_delta = target_position_ - transform.position;
		Vector3 vel_delta = target_velocity_ - velocity_;
		
		Vector3 accel = pos_delta * posK1;
		accel += pos_delta * pos_delta.magnitude * posK2;
		accel += vel_delta * velK1;
		accel += vel_delta * vel_delta.magnitude * velK2;
		
		accel *= Time.deltaTime;
		
		transform.position += velocity_ * Time.deltaTime + accel * Time.deltaTime * 0.5f;
		velocity_ += accel;
	}
}
