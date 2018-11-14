using System;
using RaycastEngine2D;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float _currentLookAheadX;
    private FocusArea _focusArea;
    private float _lookAheadDirX;
    private float _smoothLookVelocityX;
    private float _smoothVelocityY;
    private float _targetLookAheadX;


    public Vector2 focusAreaSize;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public Collider2D target;
    public float verticalOffset;
    public float verticalSmoothTime;

    // Use this for initialization
    private void Start()
    {
        _focusArea = new FocusArea(target.bounds, focusAreaSize);
    }

    private void LateUpdate()
    {
        _focusArea.Update(target.bounds);

        var focusPosition = _focusArea.centre + Vector2.up * verticalOffset;

        if (Math.Abs(_focusArea.velocity.x) > float.Epsilon) _lookAheadDirX = Mathf.Sign(_focusArea.velocity.x);

        _targetLookAheadX = _lookAheadDirX * lookAheadDstX;

        _currentLookAheadX =
            Mathf.SmoothDamp(_currentLookAheadX, _targetLookAheadX, ref _smoothLookVelocityX, lookSmoothTimeX);

        focusPosition += Vector2.right * _currentLookAheadX;

        transform.position = (Vector3) focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(_focusArea.centre, focusAreaSize);
    }


    private struct FocusArea
    {
        public Vector2 velocity;
        public Vector2 centre;
        private float left, right, top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            var shiftX = 0f;
            if (targetBounds.min.x < left)
                shiftX = targetBounds.min.x - left;
            else if (targetBounds.max.x > right) shiftX = targetBounds.max.x - right;
            left += shiftX;
            right += shiftX;

            var shiftY = 0f;
            if (targetBounds.min.y < bottom)
                shiftY = targetBounds.min.y - bottom;
            else if (targetBounds.max.y > top) shiftY = targetBounds.max.y - top;
            top += shiftY;
            bottom += shiftY;

            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}