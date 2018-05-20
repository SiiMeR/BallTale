using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBot : MonoBehaviour {

	private Vector3 _velocity;
	public float _jumpVelocity;
	public float _moveSpeed;
	public GameObject _loopStart;

	public Vector2 _jumpPos1;
	public Vector2 _jumpPos2;
	public Vector2 _jumpPos3;
	

	private CircleController2D _controller;
	
	// Use this for initialization
	void Start () {
		_controller = GetComponent<CircleController2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		_velocity.x = _moveSpeed;
		_velocity.y += -50f * Time.deltaTime;
		
		_controller.Move(_velocity * Time.deltaTime);

		
		if (Vector2.Distance(_jumpPos1, transform.position) < 1.0f ||
		    Vector2.Distance(_jumpPos2, transform.position) < 1.0f ||
		    Vector2.Distance(_jumpPos3, transform.position) < 1.0f)
		{
			Jump();
		}

	}

	void Jump()
	{
		_velocity.y = _jumpVelocity;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.name)
		{
				case "wallend":
					transform.position = _loopStart.transform.position;
					break;
				
		}
	}
}
