using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RayCastController : MonoBehaviour {
	
	
	public const float SKINWIDTH = .015f;

	
	[HideInInspector] public int horizontalRayCount = 2;
	[HideInInspector] public int verticalRayCount = 2;
	
	[HideInInspector] public LayerMask collisionMask;
	[HideInInspector] public Collider2D collider;
	
	
	public RaycastOrigins rayCastOrigins;
	
	
	public struct RaycastOrigins
	{
		public Vector2 top, left, right, bottom;
	}	
	
	
	// Use this for initialization
	public virtual void Awake ()
	{
		collider = GetComponent<Collider2D>();
		
	}

	public virtual void Start()
	{
		CalculateRaySpacing();
	}
	

	public void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SKINWIDTH * -2);
		
		rayCastOrigins.left = new Vector2(bounds.min.x, bounds.center.y);
		rayCastOrigins.right = new Vector2(bounds.max.x, bounds.center.y);
		rayCastOrigins.top = new Vector2(bounds.center.x, bounds.max.y);
		rayCastOrigins.bottom = new Vector2(bounds.center.x, bounds.min.y);
	}

	public void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SKINWIDTH * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount   = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

	}
}
