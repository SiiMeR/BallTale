using System.Collections;
using RaycastEngine2D;
using UnityEngine;

[RequireComponent(typeof(CircleController2D))]
public class Shot : MonoBehaviour
{
    private CircleController2D _controller;

    private Vector2 _direction;

    private float _distanceCovered;
    [SerializeField] private LayerMask _killMask;

    public float MaxRange { get; set; }

    public Vector2 Direction
    {
        get => _direction;
        set
        {
            var angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
            var q = Quaternion.AngleAxis(angle, Vector3.forward);

            GetComponentInChildren<SpriteRenderer>().transform.rotation = q;
            _direction = value;
        }
    }

    public float MoveSpeed { get; set; }

    public Vector2 CurrentVelocity { get; set; }

    // Use this for initialization
    private void Start()
    {
        _controller = GetComponent<CircleController2D>();
        CurrentVelocity = _direction * MoveSpeed;
        _distanceCovered = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_distanceCovered > MaxRange) StartCoroutine(DestroyShot());

        _distanceCovered += Time.deltaTime * MoveSpeed;

        Move();
    }

    private IEnumerator DestroyShot()
    {
        if (GetComponentInChildren<SpriteRenderer>()) GetComponentInChildren<SpriteRenderer>().enabled = false;

        GetComponent<CapsuleCollider2D>().enabled = false;

        GetComponentInChildren<ParticleSystem>()?.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void Move()
    {
        // any collision is death for the shot
        if (_controller.Collisions.Right ||
            _controller.Collisions.Left ||
            _controller.Collisions.Above ||
            _controller.Collisions.Below)
            StartCoroutine(DestroyShot());

        _controller.Move(CurrentVelocity * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitCollider"))
        {
            StartCoroutine(DestroyShot());
            return;
        }

        // TODO, CHECK IF IT IS A THING THAT GIVES MONEY OR NOT
        if (_controller.IsInLayerMask(other.gameObject.layer, _killMask))
        {
            AudioManager.Instance.Play("MonsterHit");


            if (other.gameObject.GetComponent<BasicEnemy>())
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Currency +=
                    other.gameObject.GetComponent<BasicEnemy>().CurrencyOnKill;


            Destroy(other.gameObject);

            StartCoroutine(DestroyShot());
        }

        else if (_controller.IsInCollisionMask(other.gameObject.layer))
        {
            StartCoroutine(DestroyShot());
        }
    }
}