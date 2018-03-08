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

		Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.left : rayCastOrigins.right;
		//	rayOrigin += velocity.x;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

		if (hit)
		{
			velocity.x = (hit.distance - SKINWIDTH) * directionX;
			rayLength = hit.distance;

			collisions.left = directionX == -1;
			collisions.right = directionX == 1;
		}
		
		Debug.DrawRay(rayCastOrigins.left , Vector2.right * -2, Color.red);
	}
	
	
	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

		Vector2 rayOrigin = (directionY == -1) ? rayCastOrigins.bottom : rayCastOrigins.top;
	//	rayOrigin += velocity.x;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

		if (hit)
		{
			velocity.y = (hit.distance - SKINWIDTH) * directionY;
			rayLength = hit.distance;
			
			collisions.below = directionY == -1;
			collisions.above = directionY == 1;
		}
		
		Debug.DrawRay(rayCastOrigins.bottom , Vector2.up * -2, Color.red);
	}
}
