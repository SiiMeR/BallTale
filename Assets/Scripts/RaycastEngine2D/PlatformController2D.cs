using System;
using UnityEngine;

namespace RaycastEngine2D
{
    public class PlatformController2D : BoxController2D
    {
        [SerializeField] private LayerMask _passengerMask;
        [SerializeField] private Vector3 _moveDirection = Vector3.up;

        // Update is called once per frame

        private void Update()
        {
            UpdateRaycastOrigins();

            var velocity = _moveDirection * Time.deltaTime;

            CalculatePassengerMovement(velocity);
            
            MovePassengers(true);
            transform.Translate(velocity);
        }

        private void MovePassengers(bool beforeMovePlatform)
        {
            // TODO 
        }

        // TODO : Check if this is really needed, the normal collision checking also works
        private void CalculatePassengerMovement(Vector3 velocity)
        {
            var rayOrigin = RayCastOrigins.Center;
            
            var directionX = Mathf.Sign(velocity.x);
            var directionY = Mathf.Sign(velocity.y);

            // Vertical move
            if (Math.Abs(velocity.y) > float.Epsilon)
            {
                var rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;

                var hit = Physics2D.BoxCast(rayOrigin, _collider.bounds.size, 0, Vector2.up * directionY, rayLength,
                    _passengerMask);

                if (hit)
                {
                    var pushX = (Math.Abs(directionY - 1) < float.Epsilon) ? velocity.x : 0;
                    var pushY = velocity.y - (hit.distance - SKINWIDTH) * directionY;

                    hit.transform.Translate(new Vector3(pushX, pushY));
                }
            }

            // Horizontal move
            if (Math.Abs(velocity.x) > float.Epsilon)
            {
                var rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

                var hit = Physics2D.BoxCast(rayOrigin, _collider.bounds.size, 0, Vector2.right * directionX, rayLength,
                    _passengerMask);

                if (hit)
                {
                    var pushX = velocity.x - (hit.distance - SKINWIDTH) * directionX;

                    hit.transform.Translate(new Vector3(pushX, 0));
                }
            }

            // Downward moving platform
            if (Math.Abs(directionY - (-1)) < float.Epsilon ||
                Math.Abs(velocity.y) < float.Epsilon && Math.Abs(velocity.x) > float.Epsilon)
            {
                var rayLength = SKINWIDTH * 2;

                var hit = Physics2D.BoxCast(rayOrigin, _collider.bounds.size, 0, Vector2.up, rayLength,
                    _passengerMask);

                if (hit)
                {
                    var pushX = velocity.x;
                    var pushY = velocity.y;

                    hit.transform.Translate(new Vector3(pushX, pushY));
                }
            }
        }
    }
}