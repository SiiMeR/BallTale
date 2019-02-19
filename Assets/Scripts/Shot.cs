using System.Collections;
using Extensions;
using RaycastEngine2D;
using UnityEngine;

public struct ShotData // TODO : This could perhaps be made into a ScriptableObject
{
    internal readonly Vector2 _direction;
    internal readonly float _moveSpeed;
    internal readonly float _maxRange;

    public ShotData(Vector2 direction, float moveSpeed, float maxRange)
    {
        _direction = direction;
        _moveSpeed = moveSpeed;
        _maxRange = maxRange;
    }
}

[RequireComponent(typeof(CircleController2D))]
public class Shot : MonoBehaviour
{
    private CapsuleCollider2D _collider;

    private CircleController2D _controller;
    private Vector2 _currentVelocity;
    private float _distanceCovered;
    [SerializeField] private LayerMask _killMask; // What it kills
    private ParticleSystem _particleSystem;

    private ShotData _shotData;
    private SpriteRenderer _spriteRenderer;

    public ShotData ShotData
    {
        private get => _shotData;
        set
        {
            _shotData = value;
            GetComponentInChildren<SpriteRenderer>().transform.rotation = CalculateSpriteAngle(value._direction);
        }
    }

    public bool IsBeingDestroyed { get; set; }

    private Quaternion CalculateSpriteAngle(Vector2 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Use this for initialization
    private void Start()
    {
        _collider = GetComponent<CapsuleCollider2D>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _controller = GetComponent<CircleController2D>();
        _currentVelocity = ShotData._direction * ShotData._moveSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsBeingDestroyed) return;

        if (IsOverMaxRange() || IsColliding())
            StartCoroutine(DestroyShot());
        else
            UpdateMovement();
    }

    private bool IsOverMaxRange()
    {
        return _distanceCovered > ShotData._maxRange;
    }

    private bool IsColliding()
    {
        return _controller.Collisions.IsColliding;
    }

    private IEnumerator DestroyShot()
    {
        IsBeingDestroyed = true;

        DisableComponents();

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void DisableComponents()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        _controller.enabled = false;
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void UpdateMovement()
    {
        _distanceCovered += ShotData._moveSpeed * Time.deltaTime;
        _controller.Move(_currentVelocity * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDestroyable(other.gameObject.layer))
            DestroyOther(other);

        else if (_controller.IsCollidable(other.gameObject.layer)) StartCoroutine(DestroyShot());
    }

    private void DestroyOther(Collider2D other)
    {
        // TODO, CHECK IF IT IS A THING THAT GIVES MONEY OR NOT

        AudioManager.Instance.Play("MonsterHit");

        if (other.gameObject.GetComponent<BasicEnemy>()) // TODO : This looks fishy
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency +=
                other.gameObject.GetComponent<BasicEnemy>().CurrencyOnKill;

        Destroy(other.gameObject);

        StartCoroutine(DestroyShot());
    }

    private bool IsDestroyable(int layer)
    {
        return _killMask.ContainsLayer(layer);
    }
}