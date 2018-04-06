using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CircleController2D))]
public class Player : MonoBehaviour
{

	[SerializeField] private int maxHealth = 10;
	[SerializeField] private float secondsInvincibility = 1.5f;
	
	
	[SerializeField] private float minJumpHeight = 1f;
	[SerializeField] private float maxJumpHeight = 4f;
	
	[SerializeField] private float moveSpeed = 10;
	[SerializeField] private float timeToJumpApex = .4f;
	[SerializeField] private float accelerationTimeAirborne = .2f;
	[SerializeField] private float accelerationTimeGrounded = .1f;
	
	[SerializeField] private float maxBoostTime = 2.0f;
	[SerializeField] private float boostForce = 20f;
	[SerializeField] private GameObject boostArrow;
	[SerializeField] private GameObject deathScreen;

	[SerializeField] private int currency = 100;
	
	private float _maxJumpVelocity;
	private float _minJumpVelocity;
	private float _velocityXSmoothing;

	private float _currentBoostTime = 0f;
	
	private Vector3 _velocity;
	private CircleController2D _controller;

	private Animator _animator;
	private Animator _arrowAnimator;
	
	private bool _canBoost = true;

	private int _currentHealth;

	public int Currency
	{
		get { return currency; }
		set { currency = value; }
	}
	
	public int CurrentHealth
	{
		get
		{
			return _currentHealth;
		}
		set
		{
			if (value < 1) // dead
			{
				StartCoroutine(Death());
			}
			else
			{
				_currentHealth = value;
				
			}
		}
	}

	IEnumerator Death()
	{		
		deathScreen.SetActive(true);
		
		yield return new WaitUntil((() => Input.GetKeyDown(KeyCode.Return)));
		
		deathScreen.SetActive(false);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	private void Die()
	{
		print("You died, but there is no implementation for death yet!");
	}


	// Use this for initialization
	void Start ()
	{
		
		deathScreen.SetActive(false);
		
		_currentHealth = maxHealth;

		_animator = GetComponent<Animator>();
		
		if (boostArrow)
		{
			boostArrow.SetActive(false);
		}

		_arrowAnimator = boostArrow.GetComponent<Animator>();
		_controller = GetComponent<CircleController2D>();

		float gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
	
		Physics2D.gravity = new Vector3(gravity, 0,0);
		_maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
		_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}


	// Update is called once per frame
	void Update ()
	{
		UpdateMovement();
	
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Killcollider") && !_animator.GetBool("Damaged"))
		{

			var enemy = other.transform.parent.gameObject;

			Currency += enemy.GetComponent<BasicEnemy>().CurrencyOnKill;
			Destroy(enemy);

			_velocity.y += 35;
		}
		
		
	}

	private void OnTriggerStay2D(Collider2D other)
	{	

		if (other.gameObject.CompareTag("Enemy"))
		{
			var enemy = other.gameObject.GetComponent<BasicEnemy>();
			

			if (!_animator.GetBool("Damaged"))
			{			
				StartCoroutine(PlayerDamaged());
				CurrentHealth -= enemy.Damage;
				Debug.Log(string.Format("Took a hit from {0}, {1} health left. ",
					other.gameObject.name,
					_currentHealth));
			}


		}
		else if (other.gameObject.CompareTag("Trap"))
		{
			var trap = other.gameObject.GetComponent<Trap>();
			
			if (!_animator.GetBool("Damaged"))
			{			
				StartCoroutine(PlayerDamaged());
				CurrentHealth -= trap.Damage;
				Debug.Log(string.Format("Took a hit from {0}, {1} health left. ",
					other.gameObject.name,
					_currentHealth));
			}
			
		}

		
	}
	
	

	IEnumerator PlayerDamaged()
	{
		
		_animator.SetBool("Damaged", true);
		
		
		var timer = secondsInvincibility; // TODO : sync this with animation TODONE : I GUESS IT IT :)
		while (timer > .0f)
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		
		_animator.SetBool("Damaged", false);
		
	}	

	private void UpdateMovement()
	{
		
		if (_controller.collisions.above || _controller.collisions.below)
		{
			_velocity.y = 0;
		}

		if (_controller.collisions.below)
		{
			_canBoost = true;
		}
		
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		Debug.DrawRay(transform.position, new Vector3(input.x, input.y, 0f).normalized, Color.yellow);


		if (Input.GetKeyDown(KeyCode.Z) && !_controller.collisions.below && _canBoost)
		{

			boostArrow.SetActive(true);
			
			_arrowAnimator.speed = 1.0f / maxBoostTime;
			_arrowAnimator.SetTrigger("Boost");
			
		}
		
		if (Input.GetKey(KeyCode.Z) && !_controller.collisions.below && _canBoost)
		{
			if (_currentBoostTime > maxBoostTime)
			{
				boostArrow.SetActive(false);
				_canBoost = false;
				_currentBoostTime = 0.0f;
				return;
			}
			
			float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
			
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

			boostArrow.transform.rotation = Quaternion.Slerp(boostArrow.transform.rotation, q, 10 * Time.deltaTime);


			
			_currentBoostTime += Time.deltaTime;
			
			return;
		}

		if (Input.GetKeyUp(KeyCode.Z) && !_controller.collisions.below && _canBoost)
		{
			_velocity = input * boostForce ; //* ((0.5f * currentBoostTime) + 0.5f);
			_currentBoostTime = 0.0f;
			_canBoost = false;
			boostArrow.SetActive(false);
		}
		
		if (Input.GetKeyDown(KeyCode.UpArrow) && _controller.collisions.below)
		{
			_velocity.y = _maxJumpVelocity;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			if (_velocity.y > _minJumpVelocity)
			{
				_velocity.y = _minJumpVelocity;
			}
			
		}

		float targetVelocityX = input.x * moveSpeed;

		_velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		
		_velocity.y += Physics2D.gravity.x * Time.deltaTime;
		_controller.Move(_velocity * Time.deltaTime);
	}
}
