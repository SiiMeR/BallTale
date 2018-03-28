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

	public override void HorizontalCollisions(ref Vector3 velocity)
	{
		throw new System.NotImplementedException();
	}

	public override void VerticalCollisions(ref Vector3 velocity)
	{
		throw new System.NotImplementedException();
	}

	public override void UpdateRaycastOrigins()
	{
		throw new System.NotImplementedException();
	}
}
