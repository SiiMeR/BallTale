using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float currentLookAheadX;


    private FocusArea focusArea;

    public Vector2 focusAreaSize;
    private float lookAheadDirX;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    private float smoothLookVelocityX;
    private float smoothVelocityY;


    public CircleController2D target;
    private float targetLookAheadX;

    public float verticalOffset;

    public float verticalSmoothTime;

    // Use this for initialization
    private void Start()
    {
        focusArea = new FocusArea(target._collider.bounds, focusAreaSize);
    }

    private void LateUpdate()
    {
        focusArea.Update(target._collider.bounds);

        var focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0) lookAheadDirX = Mathf.Sign(focusArea.velocity.x);

        targetLookAheadX = lookAheadDirX * lookAheadDstX;

        currentLookAheadX =
            Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition += Vector2.right * currentLookAheadX;

        transform.position = (Vector3) focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
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
            float shiftX = 0;
            if (targetBounds.min.x < left)
                shiftX = targetBounds.min.x - left;
            else if (targetBounds.max.x > right) shiftX = targetBounds.max.x - right;
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
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