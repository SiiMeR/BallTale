using System.Collections;
using RaycastEngine2D;
using TMPro;
using UnityEngine;
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
    private Animator _animator;

    private BoxController2D _controller;
    [SerializeField] private int _currencyOnKill = 150;

    private int _currentHealth;


    [SerializeField] private GameObject _hitCollider;
    [SerializeField] private GameObject _hpbar;
    private bool _justCollided;

    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _moveSpeed = 10;
    [SerializeField] private float _secondsMove = 20.0f;
    [SerializeField] private float _secondsVulnerable = 5.0f;


    private float _timeInState;
    [SerializeField] private bool _useGravity;
    private Vector3 _velocity;

    [SerializeField] private GameObject _vortexscreen;
    [SerializeField] private GameObject _vortextext;
    public BossState CurrentState;

    public int Damage { get; set; } = 10;
    public int DamageFromPlayer { get; set; } = 15;

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
        AudioManager.Instance.Play("BossHit", 2.0f);

        var dmg = CurrentHealth - DamageFromPlayer;

        StartCoroutine(ChangeHP(CurrentHealth, dmg, 0.5f));
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
    private void Awake()
    {
        _hpbar.transform.parent.parent.gameObject.SetActive(true);
        CurrentState = BossState.MOVE;
        _animator = GetComponent<Animator>();
        _controller = GetComponent<BoxController2D>();
        _currentHealth = _maxHealth;

        _velocity = new Vector3(_moveSpeed, _moveSpeed, 0);
    }

    // Update is called once per frame
    private void Update()
    {
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

            player.Velocity += dirToVortex.normalized * (Time.deltaTime * timer);

            _animator.speed += Mathf.Sqrt(Time.deltaTime * timer) / 10.0f;

            timer += 0.1f;

            var distance = Vector3.Distance(player.transform.position, transform.position);

            var color = _vortexscreen.GetComponent<Image>().color;

            color.a = Mathf.Lerp(1, 0, distance / 8.0f);

            _vortexscreen.GetComponent<Image>().color = color;

            return distance < 0.1f;
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

            c.a = Mathf.Lerp(0, 1, timer / 8.0f);
            AudioManager.Instance.SetMusicVolume(Mathf.Lerp(startvol, 0, timer / 8.0f));

            text.color = c;

            yield return null;
        }

        var c2 = text.color;
        c2.a = 1;
        text.color = c2;
        AudioManager.Instance.SetMusicVolume(0);

        yield return new WaitForSecondsRealtime(3.0f);

        SceneManager.LoadScene("Town");
    }


    private IEnumerator MoveBossToPos()
    {
        var startPos = transform.position;
        var endPos = new Vector3(240, 0, 0);

        const float secondsMove = 1.0f;
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

        switch (CurrentState)
        {
            case BossState.VULNERABLE when !(_timeInState > _secondsVulnerable):
                return;
            case BossState.VULNERABLE:
                NextState();
                break;
            case BossState.MOVE when !(_timeInState > _secondsMove):
                return;
            case BossState.MOVE:
                NextState();
                break;
            case BossState.VORTEX:
                break;
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
                Debug.LogWarning($"State not handled by StateActions(): {CurrentState}");
                break;
        }
    }

    private void UpdateMovement()
    {
        if (!_justCollided)
        {
            // reflect
            if (_controller.Collisions.Left)
            {
                _velocity = Vector3.Reflect(_velocity, Vector3.right);
                StartCoroutine(Collided());
            }
            else if (_controller.Collisions.Right)
            {
                _velocity = Vector3.Reflect(_velocity, Vector3.left);
                StartCoroutine(Collided());
            }
            else if (_controller.Collisions.Above)
            {
                _velocity = Vector3.Reflect(_velocity, Vector3.down);
                StartCoroutine(Collided());
            }
            else if (_controller.Collisions.Below)
            {
                _velocity = Vector3.Reflect(_velocity, Vector3.up);
                StartCoroutine(Collided());
            }
        }


        if (_useGravity)
        {
            _velocity.y += Constants.GRAVITY * Time.deltaTime;
        }

        _controller.Move(_velocity * Time.deltaTime);
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

    private IEnumerator ChangeHP(float from, float to, float seconds)
    {
        var timer = 0f;

        while ((timer += Time.deltaTime) < seconds)
        {
            var currentHealth = Mathf.Lerp(from, to, timer / seconds);

            _hpbar.GetComponent<Image>().fillAmount = currentHealth / _maxHealth;

            yield return null;
        }

        CurrentHealth = (int) to;
    }

    private IEnumerator Collided()
    {
        AudioManager.Instance.Play("Boss1WallCollide", 0.5f);
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