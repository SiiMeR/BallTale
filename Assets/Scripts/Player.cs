﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RaycastEngine2D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CircleController2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _accelerationTimeAirborne = .2f;
    [SerializeField] private float _accelerationTimeGrounded = .1f;
    private Animator _animator;
    [SerializeField] private GameObject _boostArrow;
    [SerializeField] private float _boostForce = 20f;
    [SerializeField] private GameObject _boostTimer;
    [SerializeField] private Image _boostTimerFill;
    private bool _canBoost = true;
    private CircleController2D _controller;
    [SerializeField] private int _currency = 100;
    private float _currentBoostTime;
    private int _currentHealth;

    [SerializeField] private TextMeshProUGUI _damageText;
    // DEBUG

    private DeathScreen _deathScreen;

    // DEBUG
    [SerializeField] private bool _DEBUGShootEnabled;
    private bool _isBoosting;
    [SerializeField] private int _killBounceEnergy = 15;
    private Vector2 _lastFacingDirection;
    private Vector3 _lastInput;
    [SerializeField] private float _maxBoostTime = 2.0f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private float _maxJumpHeight = 4f;
    private float _maxJumpVelocity;
    [SerializeField] private float _maxShotRange;
    [SerializeField] private float _minJumpHeight = 1f;
    private float _minJumpVelocity;
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _secondsInvincibility = 1.5f;
    [SerializeField] private GameObject _shootParticle;
    [SerializeField] private float _shotCoolDown = 1.0f;
    private double _shotCoolDownTimer;
    [SerializeField] private float _shotSpeed;
    [SerializeField] private float _timeToJumpApex = .4f;
    private float _velocityXSmoothing;
    public bool IgnoreGround;

    public Vector3 Velocity;

    public List<Upgrade> Upgrades { get; set; }

    public int Currency
    {
        get => _currency;
        set => _currency = value;
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            if (value < 1) // dead
                StartCoroutine(_deathScreen.Death());
            else
                _currentHealth = value;
        }
    }

    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }


    private void Awake()
    {
        _deathScreen = FindObjectOfType<DeathScreen>();
        Upgrades = new List<Upgrade>();
        if (_DEBUGShootEnabled) Upgrades.Add(UpgradeBuilder.Instance.GetShootingUpgrade(123, "test"));
    }

    /// <summary>
    ///     An expression bodied method to check if the player's upgrade list Upgrades contains a specific upgrade.
    ///     Since every upgrade except HealthUpgrade has it's own class,
    ///     it is trivial.
    /// </summary>
    /// <typeparam name="TUpgrade">The type of the upgrade that the method will check</typeparam>
    /// <returns>True if the player has the upgrade specified by the type <typeparamref name="TUpgrade" /></returns>
    public bool HasUpgrade<TUpgrade>() where TUpgrade : Upgrade
    {
        return Upgrades.OfType<TUpgrade>().Any();
    }

    // Use this for initialization
    private void Start()
    {
        _lastFacingDirection = Vector2.right;
        AudioManager.Instance.Play("01Peaceful", isLooping: true);

        _currentHealth = _maxHealth;

        _animator = GetComponent<Animator>();

        if (_boostArrow) _boostArrow.SetActive(false);

        if (_boostTimer) _boostTimer.SetActive(false);

        _controller = GetComponent<CircleController2D>();

        var gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);

        Constants.GRAVITY = gravity;

        _maxJumpVelocity = Mathf.Abs(gravity * _timeToJumpApex);
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * _minJumpHeight);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale > 0.01f)
        {
            UpdateMovement();
            CheckShooting();

            if (Math.Abs(Velocity.x) > .01f)
                _lastFacingDirection = ConvertToInteger(new Vector2(Velocity.x, Velocity.y));
        }
    }

    private void CheckShooting()
    {
        if (!HasUpgrade<ShootingUpgrade>()) return;

        if (Input.GetButtonDown("Fire3") && _shotCoolDown < _shotCoolDownTimer)
        {
            AudioManager.Instance.Play("Shot", 0.7f);

            var particle = Instantiate(_shootParticle, transform.position, Quaternion.identity);

            var shot = particle.GetComponent<Shot>();

            shot.MoveSpeed = _shotSpeed;

            shot.Direction = _isBoosting
                ? new Vector2(_lastInput.normalized.x, _lastInput.normalized.y)
                : new Vector2(_lastFacingDirection.x, 0);


            shot.MaxRange = _maxShotRange;

            _shotCoolDownTimer = 0;
        }

        _shotCoolDownTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Killcollider") && !_animator.GetBool("Damaged"))
        {
            AudioManager.Instance.Play("MonsterHit");

            var enemy = other.transform.parent.gameObject;

            Currency += enemy.GetComponent<BasicEnemy>().CurrencyOnKill;
            Destroy(enemy);

            Velocity.y = 0;
            Velocity.y += _killBounceEnergy;
        }


        if (other.gameObject.CompareTag("HitCollider") && !_animator.GetBool("Damaged"))
        {
            var boss = other.gameObject.GetComponentInParent<Boss>();

            if (boss.CurrentState == BossState.VULNERABLE)
            {
                boss.GetDamaged();
                Velocity.y = 0;
                Velocity.y += _killBounceEnergy;
            }
        }
    }

    public void DamagePlayer(int damageToTake)
    {
        if (!_animator.GetBool("Damaged"))
        {
            _damageText.text = damageToTake.ToString();

            StartCoroutine(FloatingDamage());

            StartCoroutine(PlayerDamaged());


            CurrentHealth -= damageToTake;
            Debug.Log($"Took a hit, {_currentHealth} health left. ");
        }
    }

    public IEnumerator FloatingDamage()
    {
        var timer = 0f;

        var orig = _damageText.color;
        orig.a = 1.0f;
        _damageText.color = orig;


        var startPos = transform.position + Vector3.up;
        var endPos = startPos + Vector3.up * 2;

        while ((timer += Time.deltaTime) < 1.0f)
        {
            var t = timer / 1.0f;
            var sint = Mathf.Sin(t * Mathf.PI * 0.5f);

            _damageText.transform.position = Vector3.Lerp(startPos, endPos, timer / 1.0f);

            var c = _damageText.color;
            c.a = Mathf.Lerp(1.0f, 0.0f, sint);
            _damageText.color = c;

            yield return null;
        }

        var end = _damageText.color;
        end.a = 0.0f;
        _damageText.color = end;
    }

    public IEnumerator PlayerDamaged()
    {
        AudioManager.Instance.Play("PlayerDamaged");

        _animator.SetBool("Damaged", true);

        var randomXJitter = Random.Range(-1.5f, 1.5f);
        var randomYJitter = Random.Range(5f, 9f);

        _controller.Collisions.Below = false; // allows to move the player in y direction on ground

        Velocity += new Vector3(randomXJitter, randomYJitter, 0);

        var timer = _secondsInvincibility;
        while (timer > .0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _animator.SetBool("Damaged", false);
    }

    private void UpdateMovement()
    {
        if (_controller.Collisions.Above || _controller.Collisions.Below)
        {
            if (!IgnoreGround)
                Velocity.y = 0;
            else
                _controller.Collisions.Below = false; // allows to move the player in y direction on ground
        }

        if (_controller.Collisions.Below) _canBoost = true;

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        input = ConvertToInteger(input);


        if (input != Vector2.zero) _lastInput = input;

        Debug.DrawRay(transform.position, input, Color.yellow);

        var boostInput = Input.GetAxisRaw("Fire1");

        if (Input.GetButtonDown("Fire1") && !_controller.Collisions.Below && _canBoost)
            AudioManager.Instance.Play("BoostCharge");

        if (Math.Abs(boostInput) > 0.01f && !_controller.Collisions.Below && _canBoost)
        {
            _boostArrow.SetActive(true);
            _boostTimer.SetActive(true);


            if (_currentBoostTime > _maxBoostTime)
            {
                _boostArrow.SetActive(false);
                _boostTimer.SetActive(false);
                _canBoost = false;
                _isBoosting = false;
                _currentBoostTime = 0.0f;
                _boostTimerFill.fillAmount = 0;
                return;
            }

            var angle = Mathf.Atan2(_lastInput.y, _lastInput.x) * Mathf.Rad2Deg;

            var q = Quaternion.AngleAxis(angle, Vector3.forward);


            if (!_isBoosting)
            {
                _boostArrow.transform.rotation = q;
                _isBoosting = true;
            }
            else
            {
                _boostArrow.transform.rotation =
                    Quaternion.Slerp(_boostArrow.transform.rotation, q, 8 * Time.deltaTime);
            }

            _boostTimerFill.fillAmount = _currentBoostTime / _maxBoostTime + 0.07f;

            _currentBoostTime += Time.deltaTime;


            return;
        }


        if (Math.Abs(boostInput) < 0.01f && !_controller.Collisions.Below && _canBoost && _isBoosting)
        {
            AudioManager.Instance.Stop("BoostCharge");
            AudioManager.Instance.Play("BoostFinish");

            Velocity = _lastInput * _boostForce;


            _currentBoostTime = 0.0f;
            _canBoost = false;
            _isBoosting = false;
            _boostTimerFill.fillAmount = 0;
            _boostArrow.SetActive(false);
            _boostTimer.SetActive(false);
        }

        if (Input.GetButtonDown("Jump") && _controller.Collisions.Below)
        {
            AudioManager.Instance.Play("Jump", 0.5f);
            Velocity.y = _maxJumpVelocity;
        }

        if (Input.GetButtonUp("Jump") && _canBoost)
            if (Velocity.y > _minJumpVelocity)
                Velocity.y = _minJumpVelocity;

        var targetVelocityX = Mathf.Round(input.x) * _moveSpeed;

        Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref _velocityXSmoothing,
            _controller.Collisions.Below ? _accelerationTimeGrounded : _accelerationTimeAirborne);

        Velocity.y += Constants.GRAVITY * Time.deltaTime;

        _controller.Move(Velocity * Time.deltaTime);
    }

    private static Vector2 ConvertToInteger(Vector2 input)
    {
        if (input.x < 0) input.x = -1;

        if (input.x > 0) input.x = 1;

        if (input.y < 0) input.y = -1;

        if (input.y > 0) input.y = 1;

        return input;
    }
}