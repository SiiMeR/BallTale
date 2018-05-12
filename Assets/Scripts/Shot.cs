using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleController2D))]
public class Shot : MonoBehaviour
{

	[SerializeField] private LayerMask _killMask;
	
	private CircleController2D _controller;
	
	public float MaxRange { get; set; }
	
	private float _direction;
	public float Direction
	{
		get { return _direction; }
		set
		{
			gameObject.GetComponent<SpriteRenderer>().flipX = Math.Abs(value - (-1)) < 0.01f;
			
			_direction = value;
		}
	}

	public float MoveSpeed { get; set;}

	public Vector2 CurrentVelocity { get; set; }
	
	private float _distanceCovered;
	
	// Use this for initialization
	void Start ()
	{
		_controller = GetComponent<CircleController2D>();
		CurrentVelocity = new Vector2(_direction, 0) * MoveSpeed;
		_distanceCovered = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{


		if (_distanceCovered > MaxRange)
		{
			StartCoroutine(DestroyShot());
		}
		
		_distanceCovered += Time.deltaTime * MoveSpeed;
		Move();
	}

	IEnumerator DestroyShot()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<CapsuleCollider2D>().enabled = false;
		
		GetComponentInChildren<ParticleSystem>()?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		
		
		yield return new WaitForSeconds(0.5f);
		
		Destroy(gameObject);
	}
	private void Move()
	{
		
		// any collision is death for the shot
		if (_controller.collisions.right || 
		    _controller.collisions.left  || 
		    _controller.collisions.above ||
		    _controller.collisions.below)
		{
			StartCoroutine(DestroyShot());
		}
		
		_controller.Move(CurrentVelocity * Time.deltaTime);
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		// TODO, CHECK IF IT IS A THING THAT GIVES MONEY OR NOT
		if (_controller.IsInLayerMask(other.gameObject.layer, _killMask))
		{
			GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency +=
				other.gameObject.GetComponent<BasicEnemy>().CurrencyOnKill;
			
			Destroy(other.gameObject);

			StartCoroutine(DestroyShot());


		}
		
		else if (_controller.IsInCollisionMask(other.gameObject.layer))
		{;
			StartCoroutine(DestroyShot());
		}
	}



}
