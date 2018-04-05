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


	[SerializeField] private Vector3 _pathFirstPos;
	[SerializeField] private Vector3 _pathMiddlePos;
	[SerializeField] private Vector3 _pathLastPos;

	[SerializeField] private float _unitsToMove = 15;
	[SerializeField] private bool _moveVertical;
	
	public int Damage { get; set; }

	public Vector3 PathMiddlePos
	{
		get
		{
			if (_pathMiddlePos == Vector3.zero)
			{
				_pathMiddlePos = transform.position;
			}
			return _pathMiddlePos;
		}
		private set { _pathMiddlePos = value; }
	}

	public Vector3 PathFirstPos
	{
		get
		{
			if (_pathFirstPos == Vector3.zero)
			{
				_pathFirstPos = transform.position + Vector3.left * 3;
			}
			return _pathFirstPos;
		}
		set { _pathFirstPos = value; }
	}

	public Vector3 PathLastPos
	{
		get
		{
			if (_pathLastPos == Vector3.zero)
			{
				_pathLastPos = transform.position + Vector3.right * 3;
			}
			
			return _pathLastPos;
		}
		set { _pathLastPos = value; }
	}

	private BoxController2D _controller;
	private Vector3 _velocity;
	
	[SerializeField] private Vector3 _movetarget;
	[SerializeField] private Vector3 _lastmovetarget;

	// Use this for initialization
	void Start ()
	{
		_movetarget = PathFirstPos;
		_lastmovetarget = PathMiddlePos;
		
		Damage = damage;
		
		_controller = GetComponent<BoxController2D>();

	}


	// Update is called once per frame
	void Update ()
	{
	//	UpdateDirection();
		UpdateMovement();
	}

	private void UpdateDirection()
	{
		if (Vector3.Distance(_movetarget, transform.position) < 0.01f)
		{
			if (_movetarget == PathFirstPos)
			{
				_lastmovetarget = _movetarget;
				_movetarget = _pathMiddlePos;
			}
			else if (_movetarget == PathMiddlePos)
			{
				if (_lastmovetarget == PathFirstPos)
				{
					_lastmovetarget = _movetarget;
					_movetarget = PathLastPos;
				}
				else if (_lastmovetarget == PathLastPos)
				{
					_lastmovetarget = _movetarget;
					_movetarget = PathFirstPos;
				}
			}
			else if (_movetarget == _pathLastPos)
			{
				_lastmovetarget = _movetarget;
				_movetarget = PathMiddlePos;
			}

		}
	}

	private void UpdateMovement()
	{
		var moveDirection = Mathf.Sign(moveSpeed);
		GetComponent<SpriteRenderer>().flipX = moveDirection == 1; // moving right

		if (!_moveVertical)
		{
			if (_controller.collisions.left || _controller.collisions.right)
			{
				moveSpeed = -moveSpeed;
			}
			if (_controller.collisions.above || _controller.collisions.below)
			{
				_velocity.y = 0;
			}
			
			_velocity.x = moveSpeed;
		}

		else
		{
			if (_controller.collisions.above || _controller.collisions.below)
			{
				moveSpeed = -moveSpeed;
			}
			
			_velocity.y = moveSpeed;
		}
		
		
		if (Vector3.Distance(transform.position, PathMiddlePos) > _unitsToMove && _unitsToMove > 0)
		{
			moveSpeed = -moveSpeed;
			
		}

	//	var movingDirection = (_movetarget - transform.position).normalized;
		
	//	_velocity += moveSpeed * movingDirection * Time.deltaTime;
		
		if (useGravity)
		{
			_velocity.y += Physics2D.gravity.x * Time.deltaTime;
		}
		
		_controller.Move(_velocity* Time.deltaTime);
	}




}
