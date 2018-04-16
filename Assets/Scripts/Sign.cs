using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Sign : Interactable
{

	[SerializeField] private TMP_Text _titleText;
	[SerializeField] private TMP_Text _paragraphText;
	
	
	[SerializeField] [TextArea] private string _title;
	[SerializeField] [TextArea] private string _text;


	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		
		_titleText.SetText(_title);
		_paragraphText.SetText(_text);
	}
	
	// Update is called once per frame	
	protected override void Update () {
		base.Update();
	}

}
