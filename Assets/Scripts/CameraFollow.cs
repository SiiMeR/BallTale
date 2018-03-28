using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{


	public CircleController2D target;

	public Vector2 focusAreaSize;

	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	

	private FocusArea focusArea;

	private float currentLookAheadX;
	private float targetLookAheadX;
	private float lookAheadDirX;
	private float smoothLookVelocityX;
	private float smoothVelocityY;
	
	struct FocusArea
	{
		public Vector2 velocity;
		public Vector2 centre;
		private float left, right, top, bottom;

		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom= targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left+right)/2, (top+bottom)/ 2);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			
			centre = new Vector2((left+right)/2, (top+bottom)/ 2);
			velocity = new Vector2(shiftX, shiftY);
			
		}
	}
	// Use this for initialization
	void Start () {
		focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
	}

	void LateUpdate()
	{
		focusArea.Update(target.collider.bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

		if (focusArea.velocity.x != 0)
		{
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			
		}

		targetLookAheadX = lookAheadDirX * lookAheadDstX;

		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition += Vector2.right * currentLookAheadX;
		
		transform.position = (Vector3) focusPosition + Vector3.forward * -10;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1,0,0,0.5f);
		Gizmos.DrawCube(focusArea.centre, focusAreaSize);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
