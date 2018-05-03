using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

	[SerializeField] private int _maxHealth = 100;
	[SerializeField] private int _moveSpeed = 10;
	[SerializeField] private bool _useGravity = false;
	[SerializeField] private int _currencyOnKill = 150;


	public int Damage { get; set; } = 10;
	public int DamageFromPlayer { get; set; } = 15;

	private BoxController2D _controller;
	private Vector3 _velocity;
	private int _currentHealth;

	// Use this for initialization
	void Start ()
	{
		_controller = GetComponent<BoxController2D>();
		_currentHealth = _maxHealth;

		_velocity = new Vector3(_moveSpeed,_moveSpeed,0);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovement();
	}

	void UpdateMovement()
	{
		// reflect
		if (_controller.collisions.left)
		{
			_velocity = Vector3.Reflect(_velocity, Vector3.right);
		}
		else if (_controller.collisions.right)
		{
			_velocity = Vector3.Reflect(_velocity, Vector3.left);
		}
		else if (_controller.collisions.above)
		{
			_velocity = Vector3.Reflect(_velocity, Vector3.down);
		}
		else if (_controller.collisions.below)
		{
			_velocity = Vector3.Reflect(_velocity, Vector3.up);
		}

		if (_useGravity)
		{
			_velocity.y += Physics2D.gravity.x * Time.deltaTime;
		}
		
		_controller.Move(_velocity* Time.deltaTime);
	
	}
}
