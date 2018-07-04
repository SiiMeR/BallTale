using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class CircleController2D : RayCastController
{

	public float EdgePushStrength = 0.01f;
	
	public CollisionInfo collisions;
	
	public struct CollisionInfo
	{
		public bool above, below, left, right;

		public bool[] edgecheck;
		
		public void Reset()
		{
			above = below = left = right = false;
			edgecheck = new bool[3];
		}

		
	}
	
	public RaycastOrigins rayCastOrigins;
	
	public struct RaycastOrigins
	{
		public Vector2 top, left, right, bottom, center;
	}


	public override void Awake()
	{
		base.Awake();
		
		// something else
	}

	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();
		collisions.Reset();
		

		if (Math.Abs(velocity.x) > float.Epsilon)
		{
			HorizontalCollisions(ref velocity);
		}
		if (Math.Abs(velocity.y) > float.Epsilon)
		{
			VerticalCollisions(ref velocity);
		}

		if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
		{
			Debug.LogWarning("Velocity of " + gameObject.name + " is NaN, will not translate");
			return;
		}
		
		transform.Translate(velocity);
	}
	

	public override void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

		Vector2 rayOrigin = rayCastOrigins.center;
		
		float radius = Vector2.Distance(rayCastOrigins.center, rayCastOrigins.right);
		
		RaycastHit2D hit = Physics2D.CircleCast(rayOrigin, radius, Vector2.right * directionX, rayLength, collisionMask);
		
		if (hit)
		{
			velocity.x = (hit.distance - SKINWIDTH) * directionX;
			collisions.left = directionX == -1;
			collisions.right = directionX == 1;
		}
		
		
		Debug.DrawRay(rayOrigin , Vector2.right * directionX * 2, collisions.left || collisions.right ? Color.blue: Color.red);
	}


	public override void VerticalCollisions(ref Vector3 velocity)
	{
		var directionY = Mathf.Sign(velocity.y);
		var rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

		var rayOrigin = rayCastOrigins.center;

		var radius = Vector2.Distance(rayCastOrigins.center, rayCastOrigins.bottom);
		
		var hit = Physics2D.CircleCast(rayOrigin, radius,Vector2.up * directionY, rayLength, collisionMask);
		if (hit)
		{
			//Vector2 reflect = Vector2.Reflect(velocity,hit.normal);
			//velocity.y += reflect.y * 3;

			velocity.y = (hit.distance - SKINWIDTH) * directionY;
			
			collisions.below = directionY == -1;
			collisions.above = directionY == 1;
		}

		for (var i = 0; i < 3; i++)
		{
			var b = collider.bounds;
			b.Expand(SKINWIDTH * -2);


			var rayOg = new Vector2();
			var rayl = rayLength;
			switch (i)
			{
				case 0:
					rayOg = new Vector2(b.min.x, b.center.y);
					rayl += radius;
					break;
				case 1:
					rayOg = new Vector2(b.center.x,b.min.y);
					break;
				case 2:
					rayOg = new Vector2(b.max.x, b.center.y);
					rayl += radius;
					break;
			}
								

			var edgehit = Physics2D.Raycast(
				rayOg,
				Vector2.up * directionY,
				rayl,
				collisionMask);

			if (edgehit)
			{
				collisions.edgecheck[i] = true;
				
				Debug.DrawRay(rayOg ,
					Vector2.down * rayl, Color.yellow);
			}
			else
			{
				Debug.DrawRay(rayOg ,
					Vector2.down * rayl, Color.magenta);
			}

		}

		EdgeMove(ref velocity);

			Debug.DrawRay(rayOrigin , Vector2.up * 2 * directionY, collisions.above || collisions.below ? Color.blue: Color.red);

	}

	int bToInt(bool b)
	{
		return b ? 1 : 0;
	}

	private void EdgeMove(ref Vector3 velocity)
	{
		int leftCollision = bToInt(collisions.edgecheck[0]);
		int middleCollision = bToInt(collisions.edgecheck[1]);
		int rightCollision = bToInt(collisions.edgecheck[2]);

		int sum = leftCollision * 1 + middleCollision * 2 + rightCollision * 4;

		switch (sum)
		{
				// left other cases in in case I to use them to add functionality in future
				case 1: // l 
					velocity.x += EdgePushStrength;
					break;
			
				case 2: // m
					break;
				
				case 3: // l + m
					break;
				
				case 4: // r
					velocity.x -= EdgePushStrength;
					break;
						
				case 5: // l + r : shouldnt happen?
					break;
				
				case 6: // m + r
					break;
				
				case 7: // l + m + r
					break;
						
				
		}
	}
	

	public override void UpdateRaycastOrigins()
	{
		var bounds = collider.bounds;
		bounds.Expand(SKINWIDTH * -2);
		
		rayCastOrigins.left = new Vector2(bounds.min.x, bounds.center.y);
		rayCastOrigins.right = new Vector2(bounds.max.x, bounds.center.y);
		rayCastOrigins.top = new Vector2(bounds.center.x, bounds.max.y);
		rayCastOrigins.bottom = new Vector2(bounds.center.x, bounds.min.y);
		rayCastOrigins.center = bounds.center;
	}
}
