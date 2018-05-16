using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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


	public void OnNewGamePressed()
	{
		SceneManager.LoadScene("Tutorial");
	}

	public void OnContinuePressed()
	{
		print("Continue pressed");
	}
	
	public void OnOptionsPressed()
	{
		OptionsMenu.Show();
	}

	public void OnCreditsPressed()
	{
		CreditsMenu.Show();
	}

	

}
