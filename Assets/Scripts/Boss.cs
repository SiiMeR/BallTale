using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public enum BossState
{
	MOVE,
	VULNERABLE
}

public class Boss : MonoBehaviour
{

	[SerializeField] private int _maxHealth = 100;
	[SerializeField] private int _moveSpeed = 10;
	[SerializeField] private bool _useGravity = false;
	[SerializeField] private int _currencyOnKill = 150;
	[SerializeField] private GameObject _hpbar;
	[SerializeField] private float _secondsVulnerable = 5.0f;
	[SerializeField] private float _secondsMove = 20.0f;

	public int Damage { get; set; } = 10;
	public int DamageFromPlayer { get; set; } = 15;

	private BoxController2D _controller;
	private Vector3 _velocity;
	private int _currentHealth;
	private Animator _animator;
	private bool _justCollided;
	
	[SerializeField] private BossState _currentState;
	private float _timeInState;
	
	public int CurrentHealth
	{
		get { return _currentHealth; }
		set
		{
			if (value < 1)
			{
				Die();
			}
			_currentHealth = value;
		}
	}

	public void GetDamaged()
	{
		int originalHp = CurrentHealth;
		CurrentHealth -= DamageFromPlayer;
		StartCoroutine(ChangeHP(originalHp, CurrentHealth, 0.5f));
	}
	private void Die()
	{
		Destroy(gameObject);
		GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency += _currencyOnKill;
	}

	
	// Use this for initialization
	void Start ()
	{
		_currentState = BossState.MOVE;
		_animator = GetComponent<Animator>();
		_controller = GetComponent<BoxController2D>();
		_currentHealth = _maxHealth;

		_velocity = new Vector3(_moveSpeed,_moveSpeed,0);
		
	}
	
	// Update is called once per frame
	void Update () {
		CheckStates();
		StateActions();
	}

	private void StateActions()
	{
		switch (_currentState)
		{
				case BossState.MOVE:
					UpdateMovement();
					break;
				
				case BossState.VULNERABLE:
					// nothing yet
					break;
		}
	}

	private void CheckStates()
	{
		_timeInState += Time.deltaTime;
		
		if (_currentState == BossState.VULNERABLE)
		{
			if (!(_timeInState > _secondsVulnerable)) return;
			
			NextState();
		}
		
		else if (_currentState == BossState.MOVE)
		{
			if (!(_timeInState > _secondsMove)) return;
			
			NextState();
		}
		
	}

	private void NextState()
	{
		switch (_currentState)
		{
				case BossState.MOVE:
					_animator.SetTrigger("vulnerable");
					_currentState = BossState.VULNERABLE;
					_timeInState = 0;
					break;
				
				case BossState.VULNERABLE:
					_animator.SetTrigger("move");
					_currentState = BossState.MOVE;
					_timeInState = 0;
					break;
				
				default:
					Debug.LogWarning("State not handled by StateActions(): " + _currentState);
					break;
		}
	}

	void UpdateMovement()
	{

		if (!_justCollided)
		{
			// reflect
			if (_controller.collisions.left)
			{
				//	print("left collision");
				_velocity = Vector3.Reflect(_velocity, Vector3.right);
				StartCoroutine(Collided());
			}
			else if (_controller.collisions.right)
			{
				//	print("right collision");
				_velocity = Vector3.Reflect(_velocity, Vector3.left);
				StartCoroutine(Collided());
			}
			else if (_controller.collisions.above)
			{
				//	print("above collision");
				_velocity = Vector3.Reflect(_velocity, Vector3.down);
				StartCoroutine(Collided());
			}
			else if (_controller.collisions.below)
			{
				//	print("down collision");
				_velocity = Vector3.Reflect(_velocity, Vector3.up);
				StartCoroutine(Collided());
			}
		}
		


		if (_useGravity)
		{
			_velocity.y += Physics2D.gravity.x * Time.deltaTime;
		}
		
		_controller.Move(_velocity* Time.deltaTime);
	
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<Player>().DamagePlayer(Damage);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Shot"))
		{
			if (_currentState == BossState.VULNERABLE)
			{
				GetDamaged();
				NextState();
			}
			
			Destroy(other.gameObject);
		}
	}

	IEnumerator ChangeHP(float from, float to, float seconds)
	{
		float timer = 0;

		Image hp = _hpbar.GetComponent<Image>();
		
		while((timer += Time.deltaTime) < seconds)
		{
			hp.fillAmount = Mathf.Lerp(from, to, timer/(seconds)) / 100;

			yield return null;
		}
	}
	
	IEnumerator Collided()
	{
		var timer = .1f;

		
		while (timer > .0f)
		{
			_justCollided = true;
			timer -= Time.deltaTime;
			yield return null;
		}


		_justCollided = false;

	}
}
