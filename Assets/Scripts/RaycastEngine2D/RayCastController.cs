using UnityEngine;

namespace RaycastEngine2D
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class RayCastController : MonoBehaviour
    {
        [SerializeField] internal LayerMask CollisionMask;
        internal Collider2D _collider;
        internal readonly float SKINWIDTH = .01f;

        // Use this for initialization
        public virtual void Awake() => _collider = GetComponent<Collider2D>();

        // from https://answers.unity.com/questions/1135055/how-to-get-all-layers-included-in-a-layermask.html 
        public bool IsInCollisionMask(int layer)
        {
            return IsInLayerMask(layer, CollisionMask);
        }

        public bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        internal abstract void CheckHorizontalCollisions(ref Vector3 velocity);
        internal abstract void CheckVerticalCollisions(ref Vector3 velocity);
        internal abstract void UpdateRaycastOrigins();
    }
}