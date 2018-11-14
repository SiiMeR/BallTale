using System.Collections;
using RaycastEngine2D;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxController2D))]
public class BasicEnemy : MonoBehaviour
{
    private BoxController2D _controller;
    private bool _justTurnedAround;


    [SerializeField] private Vector3 _movetarget;
    [SerializeField] private Vector3 _lastmovetarget;

    [SerializeField] private bool _moveVertical;

    [SerializeField] private Vector3 _pathFirstPos;
    [SerializeField] private Vector3 _pathLastPos;
    [SerializeField] private Vector3 _pathMiddlePos;

    [SerializeField] private float _unitsToMove = 15;
    private Vector3 _velocity;

    [SerializeField] private int currencyOnKill = 10;
    [SerializeField] private int damage = 5;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private bool _useGravity = true;
    [SerializeField] private bool _useWaypointMovement = true;

    public int Damage { get; set; }


    public int CurrencyOnKill
    {
        get { return currencyOnKill; }
        set { currencyOnKill = value; }
    }

    public Vector3 PathMiddlePos
    {
        get
        {
            if (_pathMiddlePos == Vector3.zero)
            {
                _pathMiddlePos = transform.position;
            }

            return _pathMiddlePos;
        }
        
        private set => _pathMiddlePos = value;
    }

    public Vector3 PathFirstPos
    {
        get { return _pathFirstPos; }
        set { _pathFirstPos = value; }
    }

    public Vector3 PathLastPos
    {
        get { return _pathLastPos; }
        set { _pathLastPos = value; }
    }

#if UNITY_EDITOR
    public void ResetWaypoints()
    {
        PathLastPos = transform.position + Vector3.right * 3;
        PathFirstPos = transform.position + Vector3.left * 3;
        PathMiddlePos = transform.position;
        _movetarget = PathFirstPos;
        _lastmovetarget = PathMiddlePos;

        EditorUtility.SetDirty(this);
    }
#endif

    // Use this for initialization
    private void Start()
    {
        PathMiddlePos = transform.position;
        _movetarget = PathFirstPos;
        _lastmovetarget = PathMiddlePos;

        Damage = damage;

        _controller = GetComponent<BoxController2D>();

        _velocity.x = moveSpeed;
    }


    // Update is called once per frame
    private void Update()
    {
        if (_useWaypointMovement)
        {
            UpdateNewMovement();
            CheckDirection();
            CheckCollision();
        }
        else
        {
            UpdateMovementDumb();
            UpdateDirectionDumb();
        }
    }

    private void UpdateDirectionDumb()
    {
        if (_controller.Collisions.Left ||
            _controller.Collisions.Right)
        {
            _velocity.x = -_velocity.x;
        }
    }

    private void CheckCollision()
    {
        if (_controller.Collisions.Left ||
            _controller.Collisions.Right ||
            _controller.Collisions.Above ||
            _controller.Collisions.Below)
        {
            UpdateDirection();
        }
    }


    private void UpdateMovementDumb()
    {
        GetComponent<SpriteRenderer>().flipX = Mathf.Sign(_velocity.x) < float.Epsilon; // moving right

        if (_useGravity)
        {
            _velocity.y += Constants.GRAVITY * Time.deltaTime;
        }

        if (_controller.Collisions.Above || _controller.Collisions.Below)
        {
            _velocity.y = 0;
        }

        _controller.Move(_velocity * Time.deltaTime);
    }


    private void UpdateNewMovement()
    {
        var direction = _movetarget - transform.position;


        GetComponent<SpriteRenderer>().flipX = Mathf.Abs(Mathf.Sign(direction.x) - 1) < float.Epsilon; // moving right

        if (_useGravity)
        {
            _velocity.y += Constants.GRAVITY * Time.deltaTime;

            if (!_justTurnedAround)
            {
                _velocity.x = Mathf.Sign(direction.x) * Mathf.Abs(moveSpeed);
            }
        }
        else
        {
            _velocity = direction.normalized * Mathf.Abs(moveSpeed);
        }

        if (_controller.Collisions.Above || _controller.Collisions.Below)
        {
            _velocity.y = 0;
        }

        _controller.Move(_velocity * Time.deltaTime);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<Player>();

            player.DamagePlayer(Damage);
        }
    }

    private void CheckDirection()
    {
        if (Vector2.Distance(_movetarget, transform.position) < 0.1f && !_justTurnedAround)
        {
            UpdateDirection();
        }
    }

    private void UpdateDirection()
    {
        StartCoroutine(TurnAround());
        if (_movetarget == PathFirstPos)
        {
            _lastmovetarget = PathFirstPos;
            _movetarget = PathMiddlePos;
        }
        else if (_movetarget == PathMiddlePos)
        {
            if (_lastmovetarget == PathFirstPos)
            {
                _movetarget = PathLastPos;
            }
            else if (_lastmovetarget == PathLastPos)
            {
                _movetarget = PathFirstPos;
            }

            _lastmovetarget = PathMiddlePos;
        }
        else if (_movetarget == PathLastPos)
        {
            _lastmovetarget = PathLastPos;
            _movetarget = PathMiddlePos;
        }
    }

    private IEnumerator TurnAround()
    {
        var timer = 1.0f;
        while (timer > .0f)
        {
            _justTurnedAround = true;
            timer -= Time.deltaTime;
            yield return null;
        }

        _justTurnedAround = false;
    }
}