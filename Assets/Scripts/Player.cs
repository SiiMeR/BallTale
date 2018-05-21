using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CircleController2D))]
public class Player : MonoBehaviour
{

	[SerializeField] private int maxHealth = 100;
	[SerializeField] private float secondsInvincibility = 1.5f;

	[SerializeField] private int killBounceEnergy = 15;
	
	[SerializeField] private float shotSpeed;
	[SerializeField] private float shotCoolDown = 1.0f;
	[SerializeField] private float maxShotRange;
	
	[SerializeField] private float minJumpHeight = 1f;
	[SerializeField] private float maxJumpHeight = 4f;
	[SerializeField] private float timeToJumpApex = .4f;
	
	[SerializeField] private float moveSpeed = 10;
	
	[SerializeField] private float accelerationTimeAirborne = .2f;
	[SerializeField] private float accelerationTimeGrounded = .1f;
	
	[SerializeField] private float maxBoostTime = 2.0f;
	[SerializeField] private float boostForce = 20f;
	[SerializeField] private GameObject boostArrow;
	[SerializeField] private GameObject deathScreen;
	[SerializeField] private GameObject shootParticle;

	[SerializeField] private int currency = 100;


	public bool HasShotUpgrade = false;
	public bool IgnoreGround = false;
	
	private float _lastFacingDirection;
	
	private float _maxJumpVelocity;
	private float _minJumpVelocity;
	private float _velocityXSmoothing;

	private float _currentBoostTime = 0f;
	
	public Vector3 _velocity;
	
	private CircleController2D _controller;

	private Animator _animator;
	private Animator _arrowAnimator;
	
	private bool _canBoost = true;
	
	private int _currentHealth;
	private double _shotCoolDownTimer;
	private bool _isBoosting;

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

	public int MaxHealth
	{
		get { return maxHealth; }
		set { maxHealth = value; }
	}


	IEnumerator Death()
	{		
		deathScreen.SetActive(true);
		
		yield return new WaitUntil((() => Input.GetKeyDown(KeyCode.Return)));
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		
		deathScreen.SetActive(false);
	}


	// Use this for initialization
	void Start ()
	{
		AudioManager.instance.Play("01Peaceful", isLooping: true);
		
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

		if (Input.GetButtonDown("Cancel"))
		{
			if (PauseMenu.Instance)
			{
				PauseMenu.Hide();
			}
			else
			{
				PauseMenu.Show();
			}

		}


		if (Time.timeScale > 0.01f)
		{
			UpdateMovement();
			CheckShooting();
			
			if (Math.Abs(_velocity.x) > .01f)
			{
				_lastFacingDirection = Mathf.Sign(_velocity.x);
	
			}


		

		}

	}

	private void CheckShooting()
	{
		if (!HasShotUpgrade) return;

		if (Input.GetButtonDown("Fire3") && shotCoolDown < _shotCoolDownTimer)
		{
			
			AudioManager.instance.Play("Shot",0.3f);
			
			var particle = Instantiate(shootParticle, transform.position, Quaternion.identity);
			
			var shot = particle.GetComponent<Shot>();

			shot.MoveSpeed = shotSpeed;
			shot.Direction = _lastFacingDirection;
			shot.MaxRange = maxShotRange;

			_shotCoolDownTimer = 0;
			
		}
		
		_shotCoolDownTimer += Time.deltaTime;



	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Killcollider") && !_animator.GetBool("Damaged"))
		{
			AudioManager.instance.Play("MonsterHit");
			
			var enemy = other.transform.parent.gameObject;

			Currency += enemy.GetComponent<BasicEnemy>().CurrencyOnKill;
			Destroy(enemy);

			_velocity.y = 0;
			_velocity.y += killBounceEnergy;
		}


		if (other.gameObject.CompareTag("HitCollider") && !_animator.GetBool("Damaged"))
		{
			var boss = other.gameObject.GetComponentInParent<Boss>();

			if (boss.CurrentState == BossState.VULNERABLE)
			{
				boss.GetDamaged();
				_velocity.y = 0;
				_velocity.y += killBounceEnergy;
			}		

		}
		
	}
	
	// TODO : Rewrite this logic into each enemy script, rather than checking them in here
	private void OnTriggerStay2D(Collider2D other)
	{	

		if (other.gameObject.CompareTag("Enemy"))
		{
			var enemy = other.gameObject.GetComponent<BasicEnemy>();
			
			DamagePlayer(enemy.Damage);
		}
		else if (other.gameObject.CompareTag("Trap"))
		{
			var trap = other.gameObject.GetComponent<Trap>();

			
			DamagePlayer(trap.Damage);
			
		}
				
	}

	public void DamagePlayer(int damageToTake)
	{
		
		if (!_animator.GetBool("Damaged"))
		{	
			
			StartCoroutine(PlayerDamaged());
			CurrentHealth -= damageToTake;
			Debug.Log($"Took a hit, {_currentHealth} health left. ");
		}
	}

	public IEnumerator PlayerDamaged()
	{
		AudioManager.instance.Play("PlayerDamaged");
		
		_animator.SetBool("Damaged", true);
		
		var randomXJitter = Random.Range(-1.5f,1.5f);
		var randomYJitter = Random.Range(0.5f, 1f); // TODO : Fix y boost not working on ground on damaged
		
		_velocity += new Vector3(randomXJitter, randomYJitter,0);
		
		
		var timer = secondsInvincibility;
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
			if (!IgnoreGround)
			{
				_velocity.y = 0;
			}
			
			
		}

		if (_controller.collisions.below)
		{
			_canBoost = true;
		}
		
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		Debug.DrawRay(transform.position, input.normalized, Color.yellow);

		float boostInput = Input.GetAxisRaw("Fire1");

		if (Input.GetButtonDown("Fire1")  && !_controller.collisions.below && _canBoost)
		{
			AudioManager.instance.Play("BoostCharge");
		}
		
		
		if (boostInput != 0 && !_controller.collisions.below && _canBoost)
		{
			
			boostArrow.SetActive(true);
			
			_arrowAnimator.speed = 1.0f / maxBoostTime;
			_arrowAnimator.SetTrigger("Boost");

			_isBoosting = true;
		}
		
		if (boostInput != 0  && !_controller.collisions.below && _canBoost)
		{
			if (_currentBoostTime > maxBoostTime)
			{
				
				boostArrow.SetActive(false);
				_canBoost = false;
				_isBoosting = false;
				_currentBoostTime = 0.0f;
				return;
			}
			
			float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
			
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

			boostArrow.transform.rotation = Quaternion.Slerp(boostArrow.transform.rotation, q, 10 * Time.deltaTime);


			
			_currentBoostTime += Time.deltaTime;
			
			return;
		}

		if (boostInput == 0 && !_controller.collisions.below && _canBoost && _isBoosting)
		{
			AudioManager.instance.Stop("BoostCharge");
			AudioManager.instance.Play("BoostFinish");
			_velocity = input * boostForce ; //* ((0.5f * currentBoostTime) + 0.5f);
			_currentBoostTime = 0.0f;
			_canBoost = false;
			_isBoosting = false;
			boostArrow.SetActive(false);
		}
		
		if (Input.GetButtonDown("Jump") && _controller.collisions.below)
		{
			AudioManager.instance.Play("Jump", 0.5f);
			_velocity.y = _maxJumpVelocity;
		}

		if (Input.GetButtonUp("Jump"))
		{
			if (_velocity.y > _minJumpVelocity)
			{
				_velocity.y = _minJumpVelocity;
			}
			
		}

		float targetVelocityX = Mathf.Round(input.x) * moveSpeed;

		_velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		
		_velocity.y += Physics2D.gravity.x * Time.deltaTime;
		_controller.Move(_velocity * Time.deltaTime);
	}
}
