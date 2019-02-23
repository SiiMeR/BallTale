using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RaycastEngine2D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CircleController2D))]
public class Player : MonoBehaviour
{
    private const float BOOST_MARGIN = 0.07f;
    
    [SerializeField] private float _accelerationTimeAirborne = .2f;
    [SerializeField] private float _accelerationTimeGrounded = .1f;

    private Animator _animator;
    [SerializeField] private GameObject _boostArrow;
    [SerializeField] private float _boostForce = 20f;
    [SerializeField] private GameObject _boostTimer;
    [SerializeField] private Image _boostTimerFill;
    private bool _canBoost;
    private CircleController2D _controller;
    private float _currentBoostTime;
    private int _currentHealth;
    [SerializeField] private TextMeshProUGUI _damageText;
    private DeathScreen _deathScreen;


    // DEBUG
    [SerializeField] private bool _DEBUGShootEnabled;
    private Firearm _firearm;
    private bool _isCurrentlyBoosting;
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
            HandleActions();   
        }
    }

    private void HandleActions()
    {
        HandleShooting();
        HandleBoosting();
        HandleMovement();
    }

    private void HandleShooting()
    {
        if (!WantsToShoot() || !CanShoot()) return;
        
        var direction = GetShootingDirection();

        _firearm.TryToShoot(direction);
    }

    private Vector2 GetShootingDirection() =>
        _isCurrentlyBoosting
            ? _lastInput.normalized.ToVector2()
            : new Vector2(Mathf.Sign(_lastFacingDirection.x), 0);


    private bool WantsToShoot()
    {
        return Input.GetButtonDown("Fire3");
    }

    private bool CanShoot()
    {
        return HasUpgrade<ShootingUpgrade>();
    }


    private bool IsMoving()
    {
        return Mathf.Abs(Velocity.x) > float.Epsilon;
    }


    public void DamagePlayer(int damageToTake)
    {
        if (!_animator.GetBool("Damaged"))
        {
            _damageText.text = damageToTake.ToString();

            StartCoroutine(FloatingDamage());

            StartCoroutine(Invulnerability());


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

    public IEnumerator Invulnerability()
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

    private Quaternion CalculateSpriteAngle(Vector2 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void HandleBoosting()
    {      
        if (_controller.IsSomethingBelow()) _canBoost = true;

        if (WantsToBoost() && CanBoost())
            ActivateBoostArrow();

        else if (IsOverMaxBoostTime()) // marks the end of the boost
        {
            AudioManager.Instance.Play("PlayerDamaged");
            ResetBoostState();
        }

        else if (Input.GetButtonUp("Fire1") && _isCurrentlyBoosting) // let go
        {
            Boost();
            ResetBoostState();
        }

        else if (WantsToBoost() && _isCurrentlyBoosting)
        {
            RotateBoostArrow();
            UpdateBoostFillAmount();
            
            _currentBoostTime += Time.deltaTime;
        }
    }

    private void Boost()
    {
        AudioManager.Instance.Play("BoostFinish");
        Velocity = _lastInput * _boostForce;
    }

    private void ActivateBoostArrow()
    {
        _boostArrow.SetActive(true);
        _boostTimer.SetActive(true);

        _boostArrow.transform.rotation = CalculateSpriteAngle(_lastInput);
        _isCurrentlyBoosting = true;
        _canBoost = false;
        AudioManager.Instance.Play("BoostCharge");
    }

    private void UpdateBoostFillAmount()
    {
        _boostTimerFill.fillAmount = _currentBoostTime / _maxBoostTime + BOOST_MARGIN;
    }



    private void RotateBoostArrow()
    {
        var angle = CalculateSpriteAngle(_lastInput);
        _boostArrow.transform.rotation =
            Quaternion.Slerp(_boostArrow.transform.rotation, angle, 8 * Time.deltaTime);
    }

    private void ResetBoostState()
    {
        AudioManager.Instance.Stop("BoostCharge");
        _currentBoostTime = 0.0f;
        _isCurrentlyBoosting = false;
        _canBoost = false;
        _boostTimerFill.fillAmount = 0;
        _boostArrow.SetActive(false);
        _boostTimer.SetActive(false);
    }

    private bool IsOverMaxBoostTime() => _currentBoostTime > _maxBoostTime;

    private bool CanBoost() => !IsOverMaxBoostTime() && !_controller.IsSomethingBelow() && _canBoost;

    private bool WantsToBoost() => Input.GetButton("Fire1");

    private void HandleMovement()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (input != Vector2.zero) _lastInput = input;

        if (_isCurrentlyBoosting) return;

        if (_controller.Collisions.Above || _controller.Collisions.Below)
        {
            if (!IgnoreGround)
                Velocity.y = 0;
            else
                _controller.Collisions.Below = false; // allows to move the player in y direction on ground
        }

        Debug.DrawRay(transform.position, input, Color.yellow); // TODO> this debug could also be separated from the main logic

        if (Input.GetButtonDown("Jump") && _controller.IsSomethingBelow())
        {
            AudioManager.Instance.Play("Jump", 0.5f);
            Velocity.y = _maxJumpVelocity;
        }

        if (Input.GetButtonUp("Jump") && !_controller.IsSomethingBelow())
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
}