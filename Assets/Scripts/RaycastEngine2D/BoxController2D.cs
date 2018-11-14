using UnityEngine;

namespace RaycastEngine2D
{
    public class BoxController2D : RayCastController
    {        
        public int horizontalRayCount = 4;
        public int verticalRayCount = 4;
        
        [HideInInspector] public CollisionInfo Collisions;
        [HideInInspector] public RaycastOrigins RayCastOrigins;


        private readonly float DISTANCEBETWEENRAYS = .25f;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;

        public override void Awake()
        {
            base.Awake();
            CalculateRaySpacing();
        }

        public void Move(Vector3 velocity)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();

            if (Mathf.Abs(velocity.x) > float.Epsilon)
            {
                CheckHorizontalCollisions(ref velocity);
            }

            if (Mathf.Abs(velocity.y) > float.Epsilon)
            {
                CheckVerticalCollisions(ref velocity);
            }

            transform.Translate(velocity);
        }

        internal override void CheckHorizontalCollisions(ref Vector3 velocity)
        {
            var directionX = Mathf.Sign(velocity.x);
            var rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;


            for (var i = 0; i < horizontalRayCount; i++)
            {
                var rayOrigin = directionX == -1 ? RayCastOrigins.bottomLeft : RayCastOrigins.bottomRight;

                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

                var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

                if (hit)
                {
                    velocity.x = (hit.distance - SKINWIDTH) * directionX;
                    rayLength = hit.distance;

                    Collisions.Left = directionX == -1;
                    Collisions.Right = directionX == 1;
                }

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.magenta);
            }
        }

        internal override void CheckVerticalCollisions(ref Vector3 velocity)
        {
            var directionY = Mathf.Sign(velocity.y);
            var rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

            for (var i = 0; i < verticalRayCount; i++)
            {
                var rayOrigin = directionY == -1 ? RayCastOrigins.bottomLeft : RayCastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);

                var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);

                if (hit)
                {
                    velocity.y = (hit.distance - SKINWIDTH) * directionY;
                    rayLength = hit.distance;

                    Collisions.Above = directionY == 1;
                    Collisions.Below = directionY == -1;
                }

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            }
        }

        internal override void UpdateRaycastOrigins()
        {
            var bounds = _collider.bounds;
            bounds.Expand(SKINWIDTH * -2);

            RayCastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            RayCastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            RayCastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            RayCastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

            RayCastOrigins.center = bounds.center;
        }

        private void CalculateRaySpacing()
        {
            var bounds = _collider.bounds;
            bounds.Expand(SKINWIDTH * -2);

            var boundsWidth = bounds.size.x;
            var boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / DISTANCEBETWEENRAYS);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / DISTANCEBETWEENRAYS);

            _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        #region structs
        public struct CollisionInfo
        {
            public bool Above, Below, Left, Right;

            public void Reset()
            {
                Above = Below = Left = Right = default;
            }

            public bool IsColliding => Above || Below || Left || Right;
            
            public override string ToString()
            {
                return $"{nameof(Above)}: {Above}, {nameof(Below)}: {Below}, {nameof(Left)}: {Left}, {nameof(Right)}: {Right}";
            }
        }

        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight, bottomLeft, bottomRight, center;
        }
        
        #endregion
    }
}