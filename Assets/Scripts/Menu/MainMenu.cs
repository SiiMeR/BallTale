using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : SimpleMenu<MainMenu> {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnBackPressed()
	{
		Application.Quit();
	}

	public void OnOptionsPressed()
	{
		OptionsMenu.Show();
	}
}
