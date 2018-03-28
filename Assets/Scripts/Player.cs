using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleController2D))]
public class Player : MonoBehaviour
{
	[SerializeField] private float minJumpHeight = 1f;
	[SerializeField] private float maxJumpHeight = 4f;
	
	[SerializeField] private float moveSpeed = 10;
	[SerializeField] private float timeToJumpApex = .4f;
	[SerializeField] private float accelerationTimeAirborne = .2f;
	[SerializeField] private float accelerationTimeGrounded = .1f;
	
	[SerializeField] private float maxBoostTime = 2.0f;

	[SerializeField] private float boostForce = 20f;

	[SerializeField] private GameObject boostArrow;
	
	private float gravity;
	
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private float velocityXSmoothing;

	private float currentBoostTime = 0f;
	
	private Vector3 velocity;
	private CircleController2D controller;
	private Animator arrowAnimator;
	
	private bool canBoost = true;
	
	// Use this for initialization
	void Start ()
	{
		if (boostArrow)
		{
			boostArrow.SetActive(false);
		}

		arrowAnimator = GetComponentInChildren<Animator>();
		
		controller = GetComponent<CircleController2D>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
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

		if (controller.collisions.below)
		{
			canBoost = true;
		}
		
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		Debug.DrawRay(transform.position, new Vector3(input.x, input.y, 0f).normalized, Color.yellow);


		if (Input.GetKeyDown(KeyCode.Z) && !controller.collisions.below && canBoost)
		{

			boostArrow.SetActive(true);
			
			GetComponentInChildren<Animator>().speed = 1.0f / maxBoostTime;
			GetComponentInChildren<Animator>().SetTrigger("Boost");
			
		}
		
		if (Input.GetKey(KeyCode.Z) && !controller.collisions.below && canBoost)
		{
			if (currentBoostTime > maxBoostTime)
			{
				boostArrow.SetActive(false);
				canBoost = false;
				currentBoostTime = 0.0f;
				return;
			}
			
			float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
			
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

			boostArrow.transform.rotation = Quaternion.Slerp(boostArrow.transform.rotation, q, 10 * Time.deltaTime);


			
			currentBoostTime += Time.deltaTime;
			
			return;
		}

		if (Input.GetKeyUp(KeyCode.Z) && !controller.collisions.below && canBoost)
		{
			velocity = input * boostForce ; //* ((0.5f * currentBoostTime) + 0.5f);
			currentBoostTime = 0.0f;
			canBoost = false;
			boostArrow.SetActive(false);
		}
		
		if (Input.GetKeyDown(KeyCode.UpArrow) && controller.collisions.below)
		{
			velocity.y = maxJumpVelocity;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			if (velocity.y > minJumpVelocity)
			{
				velocity.y = minJumpVelocity;
			}
			
		}

		float targetVelocityX = input.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}
}
