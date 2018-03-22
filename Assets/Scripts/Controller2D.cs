using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RayCastController
{
	public CollisionInfo collisions;
	
	public struct CollisionInfo
	{
		public bool above, below, left, right;

		public void Reset()
		{
			above = below = left = right = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		
	}


	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();
		collisions.Reset();

		if (velocity.x != 0)
		{
			HorizontalCollisions(ref velocity);
		}
		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}

		transform.Translate(velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

		Vector2 rayOrigin = rayCastOrigins.center;
		RaycastHit2D hit = Physics2D.CircleCast(rayOrigin, 0.5f, Vector2.right * directionX, rayLength, collisionMask);
		
		if (hit)
		{
			velocity.x = (hit.distance - SKINWIDTH) * directionX;
			collisions.left = directionX == -1;
			collisions.right = directionX == 1;
		}
		
		
		
		Debug.DrawRay(rayOrigin , Vector2.right * directionX * 2, collisions.left || collisions.right ? Color.blue: Color.red);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		print(other.transform.position);
	}



	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

		Vector2 rayOrigin = rayCastOrigins.center;
		
		RaycastHit2D hit = Physics2D.CircleCast(rayOrigin, 0.5f,Vector2.up * directionY, rayLength, collisionMask);
		if (hit)
		{
			Vector2 reflect = Vector2.Reflect(velocity,hit.normal);
			
			
			//velocity.y += reflect.y * 3;
			Debug.DrawRay(velocity, reflect, Color.magenta);

			velocity.y = (hit.distance - SKINWIDTH) * directionY;
			
			collisions.below = directionY == -1;
			collisions.above = directionY == 1;
		}
		
		
		
		Debug.DrawRay(rayOrigin , Vector2.up * 2 * directionY, collisions.above || collisions.below ? Color.blue: Color.red);
	}
}
