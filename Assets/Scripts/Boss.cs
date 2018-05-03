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
	
	[SerializeField] private GameObject _hitCollider;

	public int Damage { get; set; } = 10;
	public int DamageFromPlayer { get; set; } = 15;
	public BossState CurrentState;
	
	private BoxController2D _controller;
	private Vector3 _velocity;
	private int _currentHealth;
	private Animator _animator;
	private bool _justCollided;
	
	
	private float _timeInState;
	
	public int CurrentHealth
	{
		get { return _currentHealth; }
		set
		{
			if (value < 1)
			{
				_currentHealth = 0;
				StartCoroutine(Die());
			}
			_currentHealth = value;
		}
	}

	public void GetDamaged()
	{
		int originalHp = CurrentHealth;
		CurrentHealth -= DamageFromPlayer;
		StartCoroutine(ChangeHP(originalHp, CurrentHealth, 0.5f));
		NextState();
	}
	private IEnumerator Die()
	{
		yield return new WaitForSeconds(0.5f);
		
		_hpbar.SetActive(false);
		GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency += _currencyOnKill;
		Destroy(gameObject);
		
	}

	
	// Use this for initialization
	void Start ()
	{
		CurrentState = BossState.MOVE;
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
		switch (CurrentState)
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
		
		if (CurrentState == BossState.VULNERABLE)
		{
			if (!(_timeInState > _secondsVulnerable)) return;
			
			NextState();
		}
		
		else if (CurrentState == BossState.MOVE)
		{
			if (!(_timeInState > _secondsMove)) return;
			
			NextState();
		}
		
	}

	private void NextState()
	{
		switch (CurrentState)
		{
				case BossState.MOVE:
					_animator.SetTrigger("vulnerable");
					CurrentState = BossState.VULNERABLE;
					_timeInState = 0;
					_hitCollider.SetActive(true);
					break;
				
				case BossState.VULNERABLE:
					_animator.SetTrigger("move");
					_hitCollider.SetActive(false);
					CurrentState = BossState.MOVE;
					_timeInState = 0;
					break;
				
				default:
					Debug.LogWarning("State not handled by StateActions(): " + CurrentState);
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
			if (CurrentState == BossState.VULNERABLE)
			{
				GetDamaged();
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
