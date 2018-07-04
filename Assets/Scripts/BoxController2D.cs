using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController2D : RayCastController
{

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	private float horizontalRaySpacing;
	private float verticalRaySpacing;
	
	
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
		public Vector2 topLeft, topRight, bottomLeft, bottomRight, center;
	}

	public override void Awake()
	{
		base.Awake();
		CalculateRaySpacing();
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

	public override void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit)
			{
				velocity.x = (hit.distance - SKINWIDTH) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
			
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
			
		}
	}

	public override void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.y);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			if (hit)
			{
				velocity.y = (hit.distance - SKINWIDTH) * directionY;
				rayLength = hit.distance;

				collisions.above = directionY == -1;
				collisions.below = directionY == 1;
				
			}
			
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
		}
	}
	
	public override void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SKINWIDTH * -2);
		
		rayCastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		rayCastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		rayCastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		rayCastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

		rayCastOrigins.center = bounds.center;
	}

	void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SKINWIDTH * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
}
