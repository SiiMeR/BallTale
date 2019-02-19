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
    private CircleController2D _controller;
    private float _currentBoostTime;
    private int _currentHealth;
    [SerializeField] private TextMeshProUGUI _damageText;
    private DeathScreen _deathScreen;

    // DEBUG
    [SerializeField] private bool _DEBUGShootEnabled;
    private Firearm _firearm;
    private bool _isBoosting;
    [SerializeField] private int _killBounceEnergy = 15;
    private Vector2 _lastFacingDirection;
    private Vector3 _lastInput;
    [SerializeField] private float _maxBoostTime = 2.0f;
    [SerializeField] private float _maxJumpHeight = 4f;
    private float _maxJumpVelocity;
    [SerializeField] private float _minJumpHeight = 1f;
    private float _minJumpVelocity;
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _secondsInvincibility = 1.5f;
    [SerializeField] private float _timeToJumpApex = .4f;
    private float _velocityXSmoothing;

    public bool IgnoreGround;
    public Vector3 Velocity;
    public List<Upgrade> Upgrades { get; set; }

    public int Currency { get; set; } = 100;

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

    public int MaxHealth { get; set; } = 100;


    private void Awake()
    {
        _firearm = GetComponent<Firearm>();
        _deathScreen = FindObjectOfType<DeathScreen>();
        Upgrades = new List<Upgrade>();
        if (_DEBUGShootEnabled) Upgrades.Add(UpgradeBuilder.Instance.GetShootingUpgrade(123, "test"));
    }

    /// <summary>
    ///     Checks if the player's upgrade list Upgrades contains a specific upgrade.
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

        _currentHealth = MaxHealth;

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
        if (!ApplicationSettings.IsPaused())
        {
            UpdateMovement();

            if (WantsToShoot())
            {
                var direction = _isBoosting
                    ? _lastInput.normalized.ToVector2()
                    : new Vector2(Mathf.Sign(_lastFacingDirection.x), 0);

                _firearm.TryToShoot(direction);
            }
        }
    }

    private bool WantsToShoot()
    {
        return Input.GetButtonDown("Fire3") && HasUpgrade<ShootingUpgrade>();
    }

    private bool IsMoving()
    {
        return Math.Abs(Velocity.x) > float.Epsilon;
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


        if (other.gameObject.CompareTag("HitCollider") && !_animator.GetBool("Damaged")
        ) // TODO it seems like it doesn't belong here
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

    private bool CanBoost(float boostInput)
    {
        return !_controller.Collisions.Below && _currentBoostTime < _maxBoostTime &&
               Math.Abs(boostInput) > float.Epsilon;
    }

    private bool IsBoosting()
    {
        return true;
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

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (input != Vector2.zero) _lastInput = input;

        Debug.DrawRay(transform.position, input, Color.yellow);

        var boostInput = Input.GetAxisRaw("Fire1");

        if (Input.GetButtonDown("Fire1") && CanBoost(boostInput))
            AudioManager.Instance.Play("BoostCharge");

        if (CanBoost(boostInput)
        ) // TODO : this is missing a flag that checks if the player has touched the ground before boosting again
        {
            _boostArrow.SetActive(true);
            _boostTimer.SetActive(true);


            if (_currentBoostTime > _maxBoostTime) // marks the end of the boost
            {
                _boostArrow.SetActive(false);
                _boostTimer.SetActive(false);
                // _canBoost = false;
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


        if (Math.Abs(boostInput) < 0.01f && _isBoosting) // let go
        {
            AudioManager.Instance.Stop("BoostCharge");
            AudioManager.Instance.Play("BoostFinish");

            Velocity = _lastInput * _boostForce;

            _currentBoostTime = 0.0f;
            _isBoosting = false;
            _boostTimerFill.fillAmount = 0;
            _boostArrow.SetActive(false);
            _boostTimer.SetActive(false);
        }


        // jumping and movement part 


        if (Input.GetButtonDown("Jump") && _controller.Collisions.Below)
        {
            AudioManager.Instance.Play("Jump", 0.5f);
            Velocity.y = _maxJumpVelocity;
        }

        if (Input.GetButtonUp("Jump") && !_controller.Collisions.Below)
            if (Velocity.y > _minJumpVelocity)
                Velocity.y = _minJumpVelocity;

        var targetVelocityX = Mathf.Round(input.x) * _moveSpeed;

        Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref _velocityXSmoothing,
            _controller.Collisions.Below ? _accelerationTimeGrounded : _accelerationTimeAirborne);

        Velocity.y += Constants.GRAVITY * Time.deltaTime;

        _controller.Move(Velocity * Time.deltaTime);

        if (IsMoving())
            _lastFacingDirection = Velocity.normalized.ToVector2();
    }
}