using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour // TODO : REFACTOR WITH SIGN TO BOTH USE SAME BASE
{
	
	[SerializeField] private GameObject _notice;
	[SerializeField] private GameObject _shopInterface;

	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text paragraphText;
	
	
	[SerializeField] [TextArea] private string Title;
	[SerializeField] [TextArea] private string Text;


	private bool _isCollidingwPlayer;
	
	// Use this for initialization
	void Start () {
		_notice.SetActive(false);	
		_shopInterface.SetActive(false);

//		titleText.SetText(Title);
	//	paragraphText.SetText(Text);
	}
	
	// Update is called once per frame	
	void Update () {
		if (Input.GetKeyDown(KeyCode.X) && _isCollidingwPlayer)
		{
			FlipDialogue();
		}
	}

	public void FlipDialogue()
	{
		
		Time.timeScale = _shopInterface.activeInHierarchy ? 1.0f : 0.0f; // use this if you want pausing on sign open.
		_shopInterface.SetActive(!_shopInterface.activeInHierarchy);
	}
	
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.gameObject.CompareTag("Player"))
		{
			_notice.SetActive(true);
			_isCollidingwPlayer = true;
		}

	}



	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_notice.SetActive(false);
			_shopInterface.SetActive(false);
			_isCollidingwPlayer = false;
		}
		
	}
}
