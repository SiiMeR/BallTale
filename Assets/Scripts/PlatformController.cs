using System;
using UnityEngine;

public class PlatformController : RayCastController
{
    public Vector3 move;

    // Use this for initialization

    // Update is called once per frame

    private void Update()
    {
        var velocity = move * Time.deltaTime;
        transform.Translate(velocity);
    }

    public override void HorizontalCollisions(ref Vector3 velocity)
    {
        throw new NotImplementedException();
    }

    public override void VerticalCollisions(ref Vector3 velocity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRaycastOrigins()
    {
        throw new NotImplementedException();
    }
}