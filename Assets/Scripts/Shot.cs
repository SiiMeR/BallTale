using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleController2D))]
public class Shot : MonoBehaviour
{

	[SerializeField] private LayerMask KillMask;
	
	private float direction;

	private CircleController2D _controller;
	
	

	public float Direction
	{
		get { return direction; }
		set
		{
			gameObject.GetComponent<SpriteRenderer>().flipX = value == -1;
			
			direction = value;
		}
	}

	public float Speed { get; set;}

	public Vector2 Velocity { get; set; }
	

	// Use this for initialization
	void Start ()
	{
		_controller = GetComponent<CircleController2D>();
		Velocity = new Vector2(direction, 0) * Speed;
	}
	
	// Update is called once per frame
	void Update () {
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
		
		_controller.Move(Velocity * Time.deltaTime);
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		print(other.gameObject.tag);
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

}
