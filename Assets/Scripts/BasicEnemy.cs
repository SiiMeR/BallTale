using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(BoxController2D))]
[ExecuteInEditMode]
public class BasicEnemy : MonoBehaviour
{

	[SerializeField] private int damage = 5;
	[SerializeField] private float moveSpeed = 10;
	[SerializeField] private bool useGravity = true;
		
	private BoxController2D _controller;
	private Vector3 _velocity;
	
	public int Damage { get; set; }

	public Vector3 PathStartPos { get; set; }
	public Vector3 PathEndPos { get; set; }

    // Use this for initialization
	void Start ()
	{

		Damage = damage;
		
		_controller = GetComponent<BoxController2D>();

	}

	private void OnEnable()
	{
	//	PathStartPos = transform.position + Vector3.left * 3;
	//	PathEndPos = transform.position + Vector3.right * 3;
	}


	// Update is called once per frame
	void Update ()
	{
		UpdateMovement();
	}

	private void UpdateMovement()
	{
		if (_controller.collisions.above || _controller.collisions.below)
		{
			_velocity.y = 0;
		}

		if (_controller.collisions.left || _controller.collisions.right)
		{
			moveSpeed = -moveSpeed;
			
			GetComponent<SpriteRenderer>().flipX = Mathf.Sign(moveSpeed) == 1;
		}
		_velocity.x = moveSpeed;
		
		if (useGravity)
		{
			_velocity.y += Physics2D.gravity.x * Time.deltaTime;
		}
		
		_controller.Move(_velocity * Time.deltaTime);
	}




}
