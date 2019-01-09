using UnityEngine;

namespace RaycastEngine2D
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CircleController2D : RayCastController
    {
        [SerializeField] private float _edgePushStrength = 0.01f;
        [SerializeField] private float _maxClimbAngle = 45;
        [SerializeField] private float _maxDescentAngle = 30;
        [HideInInspector] public CollisionInfo Collisions;
        [HideInInspector] public RaycastOrigins RayCastOrigins;

        public void Move(Vector3 velocity)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();
            Collisions.VelocityOld = velocity;

            if (velocity.y < 0) DescendSlope(ref velocity);

            if (Mathf.Abs(velocity.x) > float.Epsilon) CheckHorizontalCollisions(ref velocity);

            if (Mathf.Abs(velocity.y) > float.Epsilon) CheckVerticalCollisions(ref velocity);

            if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
            {
                Debug.LogWarning("Velocity of " + gameObject.name + " is NaN, will not translate");
                return;
            }

            transform.Translate(velocity);
        }


        internal override void CheckHorizontalCollisions(ref Vector3 velocity)
        {
            var rayOrigin = RayCastOrigins.Center;
            var directionX = Mathf.Sign(velocity.x);
            var rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

            var radius = Vector2.Distance(RayCastOrigins.Center, RayCastOrigins.Right);

            var hit = Physics2D.CircleCast(rayOrigin, radius, Vector2.right * directionX, rayLength,
                CollisionMask);

            if (hit)
            {
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (Mathf.Abs(slopeAngle) > float.Epsilon && slopeAngle <= _maxClimbAngle)
                {
                    if (Collisions.DescendingSlope)
                    {
                        Collisions.DescendingSlope = false;
                        velocity = Collisions.VelocityOld;
                    }

                    var distanceToSlopeStart = 0f;

                    if (Mathf.Abs(slopeAngle - Collisions.SlopeAngleOld) > float.Epsilon)
                    {
                        distanceToSlopeStart = hit.distance - SKINWIDTH;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (Collisions.ClimbingSlope || slopeAngle < _maxClimbAngle) return;

                velocity.x = (hit.distance - SKINWIDTH) * directionX;

                if (Collisions.ClimbingSlope)
                    velocity.y = Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));

                Collisions.Left = Mathf.Abs(directionX - -1) < float.Epsilon;
                Collisions.Right = Mathf.Abs(directionX - 1) < float.Epsilon;
            }


            Debug.DrawRay(rayOrigin, Vector2.right * directionX * 2,
                Collisions.Left || Collisions.Right ? Color.blue : Color.red);
        }


        internal override void CheckVerticalCollisions(ref Vector3 velocity)
        {
            var rayOrigin = RayCastOrigins.Center;
            var directionY = Mathf.Sign(velocity.y);
            var rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

            var radius = Vector2.Distance(RayCastOrigins.Center, RayCastOrigins.Bottom);

            var hit = Physics2D.CircleCast(rayOrigin, radius, Vector2.up * directionY, rayLength,
                CollisionMask);

            if (hit)
            {
                velocity.y = (hit.distance - SKINWIDTH) * directionY;

                if (Collisions.ClimbingSlope)
                    velocity.x = velocity.y / Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);

                Collisions.Below = Mathf.Abs(directionY - -1) < float.Epsilon;
                Collisions.Above = Mathf.Abs(directionY - 1) < float.Epsilon;
            }

            if (Collisions.ClimbingSlope)
            {
                var directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

                hit = Physics2D.CircleCast(rayOrigin, radius, Vector2.up * directionY, rayLength,
                    CollisionMask);

                if (hit)
                {
                    var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (Mathf.Abs(slopeAngle - Collisions.SlopeAngle) > float.Epsilon)
                    {
                        velocity.x = (hit.distance - SKINWIDTH) * directionX;
                        Collisions.SlopeAngle = slopeAngle;
                    }
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var bounds = _collider.bounds;
                bounds.Expand(SKINWIDTH * -2);


                var rayOg = new Vector2();
                var rayl = rayLength;
                switch (i)
                {
                    case 0:
                        rayOg = new Vector2(bounds.min.x, bounds.center.y);
                        rayl += radius;
                        break;
                    case 1:
                        rayOg = new Vector2(bounds.center.x, bounds.min.y);
                        break;
                    case 2:
                        rayOg = new Vector2(bounds.max.x, bounds.center.y);
                        rayl += radius;
                        break;
                }


                var edgehit = Physics2D.Raycast(rayOg, Vector2.up * directionY, rayl, CollisionMask);

                if (edgehit)
                {
                    Collisions.edgecheck[i] = true;


                    Debug.DrawRay(rayOg,
                        Vector2.down * rayl, Color.yellow);
                }
                else
                {
                    Debug.DrawRay(rayOg,
                        Vector2.down * rayl, Color.magenta);
                }
            }


            MoveEdge(ref velocity);

            Debug.DrawRay(rayOrigin, Vector2.up * 2 * directionY,
                Collisions.Above || Collisions.Below ? Color.blue : Color.red);
        }

        /// <summary>
        ///     Calculate the new velocity taking into account the slope angle.
        /// </summary>
        /// <param name="velocity">The current reference velocity of the object.</param>
        /// <param name="slopeAngle">The angle of the slope being climbed</param>
        private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
        {
            var moveDistance = Mathf.Abs(velocity.x);
            var climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (velocity.y <= climbVelocityY)
            {
                velocity.y = climbVelocityY;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                Collisions.Below = true;
                Collisions.ClimbingSlope = true;
                Collisions.SlopeAngle = slopeAngle;
            }
        }

        /// <summary>
        ///     Calculate the new velocity taking into account the descent angle.
        /// </summary>
        /// <param name="velocity">The current reference velocity of the object.</param>
        private void DescendSlope(ref Vector3 velocity)
        {
            var directionX = Mathf.Sign(velocity.x);
            var rayOrigin = RayCastOrigins.Center;

            var radius = Vector2.Distance(RayCastOrigins.Center, RayCastOrigins.Bottom);

            var hit = Physics2D.CircleCast(rayOrigin, radius, Vector2.down, Mathf.Infinity,
                CollisionMask);

            if (hit)
            {
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (Mathf.Abs(slopeAngle) > float.Epsilon && slopeAngle < _maxDescentAngle)
                    if (Mathf.Abs(Mathf.Sign(hit.normal.x) - directionX) < float.Epsilon)
                        if (hit.distance - SKINWIDTH <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            var moveDistance = Mathf.Abs(velocity.x);
                            var descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                            velocity.y -= descendVelocityY;

                            Collisions.SlopeAngle = slopeAngle;
                            Collisions.DescendingSlope = true;
                            Collisions.Below = true;
                        }
            }
        }

        /// <summary>
        ///     Calculates the sum of the 3 rays used to check if the circle is on edge. After that, controller velocity is
        ///     modified
        ///     depending on the sum.
        /// </summary>
        /// <param name="velocity">The current velocity of the controller as a reference.</param>
        private void MoveEdge(ref Vector3 velocity)
        {
            var leftCollision = ConvertBoolToInt(Collisions.edgecheck[0]);
            var middleCollision = ConvertBoolToInt(Collisions.edgecheck[1]);
            var rightCollision = ConvertBoolToInt(Collisions.edgecheck[2]);

            var sum = leftCollision + middleCollision * 2 + rightCollision * 4;

            switch (sum)
            {
                // left other cases in case I to use them to add functionality in future

                case 0: // none, probably in air
                    break;

                case 1: // l 
                    velocity.x += _edgePushStrength;
                    break;

                case 2: // m
                    break;

                case 3: // l + m
                    break;

                case 4: // r
                    velocity.x -= _edgePushStrength;
                    break;

                case 5: // l + r : shouldnt happen?
                    break;

                case 6: // m + r
                    break;

                case 7: // l + m + r
                    break;

                default:
                    Debug.LogWarning("Undefined error in calculating edge velocity");
                    break;
            }
        }


        internal override void UpdateRaycastOrigins()
        {
            var bounds = _collider.bounds;
            bounds.Expand(SKINWIDTH * -2);

            RayCastOrigins.Left = new Vector2(bounds.min.x, bounds.center.y);
            RayCastOrigins.Right = new Vector2(bounds.max.x, bounds.center.y);
            RayCastOrigins.Top = new Vector2(bounds.center.x, bounds.max.y);
            RayCastOrigins.Bottom = new Vector2(bounds.center.x, bounds.min.y);
            RayCastOrigins.Center = bounds.center;
        }

        private static int ConvertBoolToInt(bool b)
        {
            return b ? 1 : 0;
        }

        #region structs

        public struct CollisionInfo
        {
            public bool Above, Below, Left, Right;

            public bool ClimbingSlope, DescendingSlope;

            public float SlopeAngle, SlopeAngleOld;

            public Vector3 VelocityOld;

            internal bool[] edgecheck;

            public void Reset()
            {
                edgecheck = new bool[3];
                Above = Below = Left = Right = false;
                ClimbingSlope = DescendingSlope = false;
                SlopeAngleOld = SlopeAngle;
                SlopeAngle = 0;
            }

            public bool IsColliding => Above || Below || Left || Right;

            public override string ToString()
            {
                return
                    $"{nameof(Above)}: {Above}, {nameof(Below)}: {Below}, {nameof(Left)}: {Left}, {nameof(Right)}: {Right}";
            }
        }

        public struct RaycastOrigins
        {
            public Vector2 Top, Left, Right, Bottom, Center;
        }

        #endregion
    }
}