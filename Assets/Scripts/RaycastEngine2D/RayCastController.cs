using Extensions;
using UnityEngine;

namespace RaycastEngine2D
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class RayCastController : MonoBehaviour
    {
        internal readonly float SKINWIDTH = .01f;
        internal Collider2D _collider;
        [SerializeField] internal LayerMask collisionMask;

        // Use this for initialization
        public virtual void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        // from https://answers.unity.com/questions/1135055/how-to-get-all-layers-included-in-a-layermask.html 
        public bool IsCollidable(int layer)
        {
            return collisionMask.ContainsLayer(layer);
        }

        internal abstract void CheckHorizontalCollisions(ref Vector3 velocity);
        internal abstract void CheckVerticalCollisions(ref Vector3 velocity);
        internal abstract void UpdateRaycastOrigins();
    }
}