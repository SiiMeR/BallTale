using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleController2D))]
public class Shot : MonoBehaviour
{

	private float direction;
	private float speed;

	private CircleController2D _controller;

	public float Direction
	{
		get { return direction; }
		set
		{
			gameObject.GetComponent<SpriteRenderer>().flipX = value == -1;
			
			direction = value;
		}
	}

	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	// Use this for initialization
	void Start ()
	{
		_controller = GetComponent<CircleController2D>();
	}
	
	// Update is called once per frame
	void Update () {
		_controller.Move(new Vector3(direction,0,0) * speed * Time.deltaTime);
	//	transform.Translate();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		print(other.gameObject.layer);
		if (other.gameObject.layer == 9)
		{
			Destroy(gameObject);
		}
	}

}
