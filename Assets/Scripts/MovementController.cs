using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	private Rigidbody2D _rigidbody;
	private bool _isGrounded;

	[SerializeField] private float _movespeed;
	[SerializeField] private float _jumpforce;
	
	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		MovePlayer();
	}


	private void MovePlayer()
	{
		if (Input.GetKeyDown(KeyCode.Space) )
		{
			_rigidbody.AddForce(new Vector2(0, _jumpforce), ForceMode2D.Impulse);
		}
		
		float moveX = Input.GetAxis("Horizontal");
		float moveY = Input.GetAxis("Vertical");
		
		Vector2 v2 = new Vector2(moveX, _rigidbody.velocity.y);

		Vector2 v3 = new Vector2(transform.forward.x, transform.forward.y);
		//_rigidbody.MovePosition(transform.position + (v3 * Time.deltaTime));
		_rigidbody.velocity = new Vector2(moveX * _movespeed,  _rigidbody.velocity.y);
	}
}
