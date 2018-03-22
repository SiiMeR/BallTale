using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RayCastController : MonoBehaviour {
	
	
	public const float SKINWIDTH = .015f;

	[HideInInspector] public LayerMask collisionMask;
	[HideInInspector] public Collider2D collider;
	
	
	public RaycastOrigins rayCastOrigins;
	
	
	public struct RaycastOrigins
	{
		public Vector2 top, left, right, bottom, center;
	}	
	
	
	// Use this for initialization
	public virtual void Awake ()
	{
		collider = GetComponent<Collider2D>();
		
	}

	public void UpdateRaycastOrigins()
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
