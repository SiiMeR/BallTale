using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Sign : MonoBehaviour
{

	
	[SerializeField] private GameObject notice;
	[SerializeField] private GameObject dialoguePanel;

	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text paragraphText;
	
	
	[SerializeField] [TextArea] private string Title;
	[SerializeField] [TextArea] private string Text;


	private bool isCollidingwPlayer;
	
	// Use this for initialization
	void Start () {
		notice.SetActive(false);	
		dialoguePanel.SetActive(false);

		titleText.SetText(Title);
		paragraphText.SetText(Text);
	}
	
	// Update is called once per frame	
	void Update () {
		if (Input.GetKeyDown(KeyCode.X) && isCollidingwPlayer)
		{
			FlipDialogue();
		}
	}

	public void FlipDialogue()
	{
		
		//Time.timeScale = dialoguePanel.activeInHierarchy ? 1.0f : 0.0f; use this if you want pausing on sign open.
		dialoguePanel.SetActive(!dialoguePanel.activeInHierarchy);
	}
	
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.gameObject.CompareTag("Player"))
		{
			notice.SetActive(true);
			isCollidingwPlayer = true;
		}

	}



	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			notice.SetActive(false);
			dialoguePanel.SetActive(false);
			isCollidingwPlayer = false;
		}
		
	}
}
