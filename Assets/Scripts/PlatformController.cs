using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RayCastController {
	
	
	public Vector3 move;

	// Use this for initialization

	// Update is called once per frame

	void Update ()
	{
		Vector3 velocity = move * Time.deltaTime;
		transform.Translate(velocity);

	}
}
