using System;
using UnityEngine;

namespace RaycastEngine2D
{
    public class PlatformController : RayCastController
    {
        public Vector3 move;

        // Update is called once per frame

        private void Update()
        {
            var velocity = move * Time.deltaTime;
            transform.Translate(velocity);
        }

        internal override void CheckHorizontalCollisions(ref Vector3 velocity)
        {
            throw new NotImplementedException();
        }

        internal override void CheckVerticalCollisions(ref Vector3 velocity)
        {
            throw new NotImplementedException();
        }

        internal override void UpdateRaycastOrigins()
        {
            throw new NotImplementedException();
        }
    }
}