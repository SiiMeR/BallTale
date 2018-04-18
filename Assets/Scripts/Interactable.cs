using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

	[SerializeField] private GameObject _notice;
	[SerializeField] private GameObject _panel;
	[SerializeField] private bool _pauseOnInteract;
	
	private bool _isCollidingWPlayer;
	
	
	// Use this for initialization
	protected virtual void Start () {
		_notice.SetActive(false);	
		_panel.SetActive(false);

	}
	
	// Update is called once per frame	
	protected virtual void Update () {
		if (Input.GetKeyDown(KeyCode.X) && _isCollidingWPlayer)
		{
			FlipDialogue();
		}
	}

	public void FlipDialogue()
	{
		if (_pauseOnInteract)
		{
			
			Time.timeScale = _panel.activeInHierarchy ? 1.0f : 0.0f; 
			// use this if you want pausing on open.
		}
		_panel.SetActive(!_panel.activeInHierarchy);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.gameObject.CompareTag("Player"))
		{
			_notice.SetActive(true);
			_isCollidingWPlayer = true;
		}

	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_notice.SetActive(false);
			_panel.SetActive(false);
			_isCollidingWPlayer = false;
		}
		
	}
}
