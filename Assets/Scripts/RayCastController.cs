using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class RayCastController : MonoBehaviour {
	
	
	public readonly float SKINWIDTH = .01f;

	public LayerMask collisionMask;
	[HideInInspector] public Collider2D collider;
	
	// Use this for initialization
	public virtual void Awake ()
	{
		collider = GetComponent<Collider2D>();
	}

	
	// from https://answers.unity.com/questions/1135055/how-to-get-all-layers-included-in-a-layermask.html 
	public virtual bool IsInCollisionMask(int layer)
	{
		if (collisionMask == (collisionMask | (1 << layer)))
		{
			return true;
		}
 
		return false;
	}
	
	public virtual bool IsInLayerMask(int layer, LayerMask layerMask)
	{
		if (layerMask == (layerMask | (1 << layer)))
		{
			return true;
		}
 
		return false;
	}

	public abstract void HorizontalCollisions(ref Vector3 velocity);
	public abstract void VerticalCollisions(ref Vector3 velocity);
	public abstract void UpdateRaycastOrigins();




}
