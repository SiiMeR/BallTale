using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BossState
{
	MOVE,
	VULNERABLE,
	VORTEX
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

	[SerializeField] private GameObject _vortexscreen;
	[SerializeField] private GameObject _vortextext;
	
	
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
		AudioManager.Instance.Play("BossHit",vol:2.0f);
		int originalHp = CurrentHealth;
		CurrentHealth -= DamageFromPlayer;
		_hpbar.GetComponent<Image>().fillAmount = (float)CurrentHealth / _maxHealth;
		//StartCoroutine(ChangeHP(originalHp, CurrentHealth, 0.5f));
		NextState();
	}
	private IEnumerator Die()
	{
		FindObjectOfType<Player>().Currency += _currencyOnKill;
		
		GameObject.FindGameObjectWithTag("BossFight").GetComponent<Bossfight>().Endfight();
		
		yield return new WaitForSeconds(0.5f);
		
		_hpbar.transform.parent.transform.parent.gameObject.SetActive(false);
		
		StartCoroutine(Vortex());
		
	//	GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency += _currencyOnKill;
	//	Destroy(gameObject);
		
	}

	
	// Use this for initialization
	void Awake ()
	{
		_hpbar.transform.parent.parent.gameObject.SetActive(true);
		CurrentState = BossState.MOVE;
		_animator = GetComponent<Animator>();
		_controller = GetComponent<BoxController2D>();
		_currentHealth = _maxHealth;
		
		_velocity = new Vector3(_moveSpeed,_moveSpeed,0);
		
	}
	
	// Update is called once per frame
	void Update () {

		if (CurrentState != BossState.VORTEX)
		{
			CheckStates();
			StateActions();
		}
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
				
				case BossState.VORTEX:
					break;
		}
	}

	private IEnumerator Vortex()
	{
		CurrentState = BossState.VORTEX;
			
		yield return StartCoroutine(MoveBossToPos());
	
		_animator.SetBool("vortex", true);

		var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		player.IgnoreGround = true;
		
		float timer = 0;

		yield return new WaitUntil(() =>
		{
			var dirToVortex = transform.position - player.transform.position;
			
			player._velocity += dirToVortex.normalized * (Time.deltaTime * timer);
			
			_animator.speed += Mathf.Sqrt(Time.deltaTime * timer) / 10.0f;
			
			timer += 0.1f;

			var distance = Vector3.Distance(player.transform.position, transform.position);

			var color = _vortexscreen.GetComponent<Image>().color;

			color.a = Mathf.Lerp(1,0, distance / 8.0f);
			
			_vortexscreen.GetComponent<Image>().color = color;
			
			return  distance < 0.1f;
		});
		
		var c = _vortexscreen.GetComponent<Image>().color;

		c.a = 1;
			
		_vortexscreen.GetComponent<Image>().color = c;

		
		player.IgnoreGround = false;

		Time.timeScale = 0f;


		StartCoroutine(VortexText());
		

	}

	private IEnumerator VortexText()
	{
		yield return new WaitForSecondsRealtime(1.5f);
		float timer = 0;

		var text = _vortextext.GetComponent<TMP_Text>();

		var startvol = AudioManager.Instance.musicVolume;
		while ((timer += Time.unscaledDeltaTime) < 8.0f)
		{
			var c = text.color;

			c.a = Mathf.Lerp(0,1, timer / 8.0f);
			AudioManager.Instance.SetMusicVolume(Mathf.Lerp(startvol,0, timer/8.0f));

			text.color = c;

			yield return null;
		}
		var c2 = text.color;
		c2.a = 1;
		text.color = c2;
		AudioManager.Instance.SetMusicVolume(0);

		yield return new WaitForSecondsRealtime(3.0f);
		
		SceneManager.LoadScene("Menu");

		
	}


	private IEnumerator MoveBossToPos()
	{
		Vector3 startPos = transform.position;
		Vector3 endPos = new Vector3(240, 0,0);

		float secondsMove = 1.0f;
		float timer = 0;

		while ((timer += Time.deltaTime) < secondsMove)
		{
			transform.position = Vector3.Lerp(startPos, endPos, timer / secondsMove);

			yield return null;
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
		if (other.gameObject.CompareTag("Player") && CurrentState != BossState.VORTEX)
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

		var end = Mathf.Min(from, to);
		var begin = Mathf.Max(from, to);
		
		print(from/to+ " end begin");
		print(to/from + " what");
		Image hp = _hpbar.GetComponent<Image>();
		
		while((timer += Time.deltaTime) < seconds)
		{
			
			var lerp =  Mathf.Lerp(to,from, timer/(seconds));

			hp.fillAmount = lerp/_maxHealth;

			yield return null;
		}

		//hp.fillAmount = from;
	}
	
	IEnumerator Collided()
	{
		
		AudioManager.Instance.Play("Boss1WallCollide",0.5f);
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
