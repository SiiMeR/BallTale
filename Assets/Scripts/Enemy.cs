using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxController2D))]
public class Enemy : MonoBehaviour {
	
	[SerializeField] private float moveSpeed = 10;
	[SerializeField] private float gravity;

	private BoxController2D controller;
	private Vector3 velocity;

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<BoxController2D>();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		updateMovement();
	}

	private void updateMovement()
	{
		if (controller.collisions.above || controller.collisions.below)
		{
			velocity.y = 0;
		}

		if (controller.collisions.left || controller.collisions.right)
		{
			moveSpeed = -moveSpeed;
		}
		velocity.x = moveSpeed;
		velocity.y += gravity * Time.deltaTime;
		
		controller.Move(velocity * Time.deltaTime);
	}
}
