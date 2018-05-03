using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

	[SerializeField] private int _maxHealth = 100;
	[SerializeField] private int _moveSpeed = 10;
	[SerializeField] private bool _useGravity = false;
	[SerializeField] private int _currencyOnKill = 150;
	[SerializeField] private GameObject _hpbar;	

	public int Damage { get; set; } = 10;
	public int DamageFromPlayer { get; set; } = 15;

	private BoxController2D _controller;
	private Vector3 _velocity;
	private int _currentHealth;
	private Animator _animator;
	
	
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
		
	}

	
	// Use this for initialization
	void Start ()
	{

		_animator = GetComponent<Animator>();
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
		//	print("left collision");
			_velocity = Vector3.Reflect(_velocity, Vector3.right);
		}
		else if (_controller.collisions.right)
		{
		//	print("right collision");
			_velocity = Vector3.Reflect(_velocity, Vector3.left);
		}
		else if (_controller.collisions.above)
		{
		//	print("above collision");
			_velocity = Vector3.Reflect(_velocity, Vector3.down);
		}
		else if (_controller.collisions.below)
		{
		//	print("down collision");
			_velocity = Vector3.Reflect(_velocity, Vector3.up);
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
			GetDamaged();
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
}
