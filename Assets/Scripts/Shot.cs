using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{

	private float direction;
	private float speed;

	public float Direction
	{
		get { return direction; }
		set { direction = value; }
	}

	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector3(direction,0,0) * speed * Time.deltaTime);
	}

}
