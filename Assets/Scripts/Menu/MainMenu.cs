using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : SimpleMenu<MainMenu>
{

	public Button continueButton;
	
	// Use this for initialization
	void Start () {
		if (SaveGame.Exists("player"))
		{
			continueButton.interactable = true;
		}
	}

	private void FixedUpdate()
	{
		if (SaveGame.Exists("player"))
		{
			continueButton.interactable = true;
		}
		else
		{
			continueButton.interactable = false;
		}
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
		PlayerPrefs.SetInt("loadgame", 1);
		SceneManager.LoadScene("Tutorial");
		
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
