using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class RayCastController : MonoBehaviour {
	
	
	public readonly float SKINWIDTH = .015f;

	public LayerMask collisionMask;
	[HideInInspector] public Collider2D collider;
	
	// Use this for initialization
	public virtual void Awake ()
	{
		collider = GetComponent<Collider2D>();
		
	}

	public abstract void HorizontalCollisions(ref Vector3 velocity);
	public abstract void VerticalCollisions(ref Vector3 velocity);
	public abstract void UpdateRaycastOrigins();




}
