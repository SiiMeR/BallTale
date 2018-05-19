using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : SimpleMenu<PauseMenu>{

	// Use this for initialization
	void Start () {
		
	}

	protected override void Awake()
	{
		base.Awake();
		Time.timeScale = 0;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Time.timeScale = 1;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void OnQuitPressed()
	{
		SaveGame();
		
		SceneManager.LoadScene("Menu");
	}

	private void SaveGame()
	{
		print("Saved game");
	}

	public void OnOptionsPressed()
	{
		
	}
	
}
