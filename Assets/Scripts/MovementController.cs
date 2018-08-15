using UnityEngine;

public class MovementController : MonoBehaviour
{
    private bool _isGrounded;
    [SerializeField] private float _jumpforce;

    [SerializeField] private float _movespeed;
    private Rigidbody2D _rigidbody;

    // Use this for initialization
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        MovePlayer();
    }


    private void MovePlayer()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _rigidbody.AddForce(new Vector2(0, _jumpforce), ForceMode2D.Impulse);

        var moveX = Input.GetAxis("Horizontal");
        //	float moveY = Input.GetAxis("Vertical");

        _rigidbody.velocity = new Vector2(moveX * _movespeed, _rigidbody.velocity.y);
    }
}