using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class ButtonHighlighter : MonoBehaviour
{
	private Button previousButton;
	[SerializeField] private float scaleAmount = 1.4f;
	[SerializeField] private GameObject defaultButton;
 
	void Start()
	{
		if (defaultButton != null)
		{
			EventSystem.current.SetSelectedGameObject(defaultButton);
		}
	}
	void Update()
	{
		var selectedObj = EventSystem.current.currentSelectedGameObject;
 
		if (selectedObj == null) return;
		var selectedAsButton = selectedObj.GetComponent<Button>();
		if(selectedAsButton != null && selectedAsButton != previousButton)
		{
			if(selectedAsButton.transform.name != "PauseButton")
				HighlightButton(selectedAsButton);
		}
 
		if (previousButton != null && previousButton != selectedAsButton)
		{
			UnHighlightButton(previousButton);
		}
		previousButton = selectedAsButton;
	}
	void OnDisable()
	{
		if (previousButton != null) UnHighlightButton(previousButton);
	}
 
	void HighlightButton(Button butt)
	{
		butt.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
	}
 
	void UnHighlightButton(Button butt)
	{
		butt.transform.localScale = new Vector3(1, 1, 1);
	}
}