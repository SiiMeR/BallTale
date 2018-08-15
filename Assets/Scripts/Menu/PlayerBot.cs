using UnityEngine;

public class PlayerBot : MonoBehaviour
{
    private CircleController2D _controller;

    public Vector2 _jumpPos1;
    public Vector2 _jumpPos2;
    public Vector2 _jumpPos3;
    public float _jumpVelocity;
    public GameObject _loopStart;
    public float _moveSpeed;

    private Vector3 _velocity;

    // Use this for initialization
    private void Start()
    {
        _controller = GetComponent<CircleController2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        _velocity.x = _moveSpeed;
        _velocity.y += -50f * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);


        if (Vector2.Distance(_jumpPos1, transform.position) < 1.0f ||
            Vector2.Distance(_jumpPos2, transform.position) < 1.0f ||
            Vector2.Distance(_jumpPos3, transform.position) < 1.0f)
            Jump();
    }

    private void Jump()
    {
        _velocity.y = _jumpVelocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.name)
        {
            case "wallend":
                transform.position = _loopStart.transform.position;
                break;
        }
    }
}