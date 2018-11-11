using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public abstract class RayCastController : MonoBehaviour
{
    public readonly float SKINWIDTH = .01f;

    public LayerMask collisionMask;

    [HideInInspector]
    public Collider2D _collider;

    // Use this for initialization
    public virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }


    // from https://answers.unity.com/questions/1135055/how-to-get-all-layers-included-in-a-layermask.html 
    public virtual bool IsInCollisionMask(int layer)
    {
        return collisionMask == (collisionMask | (1 << layer));
    }

    public virtual bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public abstract void HorizontalCollisions(ref Vector3 velocity);
    public abstract void VerticalCollisions(ref Vector3 velocity);
    public abstract void UpdateRaycastOrigins();
}