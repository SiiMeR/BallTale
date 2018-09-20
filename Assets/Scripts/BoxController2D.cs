using UnityEngine;

public class BoxController2D : RayCastController
{
    protected const float DISTANCEBETWEENRAYS = .25f;

    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    public CollisionInfo collisions;

    public int horizontalRayCount = 4;

    public RaycastOrigins rayCastOrigins;
    public int verticalRayCount = 4;

    public override void Awake()
    {
        base.Awake();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (Mathf.Abs(velocity.x) > float.Epsilon)
        {
            HorizontalCollisions(ref velocity);
        }

        if (Mathf.Abs(velocity.y) > float.Epsilon)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    public override void HorizontalCollisions(ref Vector3 velocity)
    {
        var directionX = Mathf.Sign(velocity.x);
        var rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;


        for (var i = 0; i < horizontalRayCount; i++)
        {
            var rayOrigin = directionX == -1 ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight;

            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                velocity.x = (hit.distance - SKINWIDTH) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.magenta);
        }
    }

    public override void VerticalCollisions(ref Vector3 velocity)
    {
        var directionY = Mathf.Sign(velocity.y);
        var rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

        for (var i = 0; i < verticalRayCount; i++)
        {
            var rayOrigin = directionY == -1 ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);

            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                velocity.y = (hit.distance - SKINWIDTH) * directionY;
                rayLength = hit.distance;

                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
        }
    }

    public override void UpdateRaycastOrigins()
    {
        var bounds = _collider.bounds;
        bounds.Expand(SKINWIDTH * -2);

        rayCastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayCastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayCastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayCastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

        rayCastOrigins.center = bounds.center;
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

    public struct CollisionInfo
    {
        public bool above, below, left, right;

        public void Reset()
        {
            above = below = left = right = false;
        }


        public override string ToString()
        {
            return $"Collisions: above {above}, below {below}, left {left}, right {right}";
        }
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight, center;
    }
}