using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController2D : RayCastController
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

		if (Math.Abs(velocity.x) > Single.Epsilon)
		{
			HorizontalCollisions(ref velocity);
		}
		if (Math.Abs(velocity.y) > Single.Epsilon)
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
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

		Vector2 rayOrigin = rayCastOrigins.center;

		float radius = Vector2.Distance(rayCastOrigins.center, rayCastOrigins.bottom);
		
		RaycastHit2D hit = Physics2D.CircleCast(rayOrigin, radius,Vector2.up * directionY, rayLength, collisionMask);
		if (hit)
		{
			//Vector2 reflect = Vector2.Reflect(velocity,hit.normal);
			//velocity.y += reflect.y * 3;

			velocity.y = (hit.distance - SKINWIDTH) * directionY;
			
			collisions.below = directionY == -1;
			collisions.above = directionY == 1;
		}
		
		Debug.DrawRay(rayOrigin , Vector2.up * 2 * directionY, collisions.above || collisions.below ? Color.blue: Color.red);
	}

	public override void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SKINWIDTH * -2);
		
		rayCastOrigins.left = new Vector2(bounds.min.x, bounds.center.y);
		rayCastOrigins.right = new Vector2(bounds.max.x, bounds.center.y);
		rayCastOrigins.top = new Vector2(bounds.center.x, bounds.max.y);
		rayCastOrigins.bottom = new Vector2(bounds.center.x, bounds.min.y);
		rayCastOrigins.center = bounds.center;
	}
}
