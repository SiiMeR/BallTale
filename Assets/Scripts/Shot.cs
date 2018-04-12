using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleController2D))]
public class Shot : MonoBehaviour
{

	[SerializeField] private LayerMask KillMask;
	
	private CircleController2D _controller;
	
	public float MaxRange { get; set; }
	
	private float _direction;
	public float Direction
	{
		get { return _direction; }
		set
		{
			gameObject.GetComponent<SpriteRenderer>().flipX = value == -1;
			
			_direction = value;
		}
	}

	public float Movespeed { get; set;}

	public Vector2 CurrentVelocity { get; set; }


	
	private float _distanceCovered;
	
	// Use this for initialization
	void Start ()
	{
		_controller = GetComponent<CircleController2D>();
		CurrentVelocity = new Vector2(_direction, 0) * Movespeed;
		_distanceCovered = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{


		if (_distanceCovered > MaxRange)
		{
			Destroy(gameObject);
		}
		
		_distanceCovered += Time.deltaTime * Movespeed;
		Move();
	}

	private void Move()
	{
		
		// any collision is death
		if (_controller.collisions.right || 
		    _controller.collisions.left  || 
		    _controller.collisions.above ||
		    _controller.collisions.below)
		{
			Destroy(gameObject);
		}
		
		_controller.Move(CurrentVelocity * Time.deltaTime);
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		// TODO, CHECK IF IT IS A THING THAT GIVES MONEY OR NOT
		if (_controller.IsInLayerMask(other.gameObject.layer, KillMask))
		{
			
			GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency +=
				other.gameObject.GetComponent<BasicEnemy>().CurrencyOnKill;
			Destroy(other.gameObject);
			Destroy(gameObject);


		}
		
		else if (_controller.IsInCollisionMask(other.gameObject.layer))
		{
			Destroy(gameObject);
		}
	}

	public Shot(float direction, float maxRange, float movespeed)
	{
		this._direction = direction;
		MaxRange = maxRange;
		Movespeed = movespeed;
	}
}
