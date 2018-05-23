using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerData
{
	public Vector3Save position;
	
	public int currency;
	public int currentHealth;
	public int maxHealth;
	public bool hasShotUpgrade;


	public PlayerData(Player player)
	{
		position = player.transform.position;
		currency = player.Currency;
		maxHealth = player.MaxHealth;
		currentHealth = player.CurrentHealth;
		hasShotUpgrade = player.HasShotUpgrade;
	}
	
}

public class SaveGameManager : Singleton<SaveGameManager>
{

	void Awake()
	{
		if (PlayerPrefs.GetInt("loadgame") == 1)
		{
			LoadGame();
		}
		
		PlayerPrefs.SetInt("loadgame",0);
	}
	
	public GameObject playerPrefab;


	public void CreateSaveGame()
	{
		var player = FindObjectOfType<Player>();
		
		var playerSave = new PlayerData(player);
		
		SaveGame.Save("player", playerSave);
	}

	public void LoadGame()
	{
		var loadedPlayer = SaveGame.Load<PlayerData>("player");

		var currentPlayer = FindObjectOfType<Player>();
		
		print("loaded from player " + loadedPlayer.position.x);
		currentPlayer.Currency = loadedPlayer.currency;
		currentPlayer.CurrentHealth = loadedPlayer.currentHealth;
		currentPlayer.MaxHealth = loadedPlayer.maxHealth;
		currentPlayer.HasShotUpgrade = loadedPlayer.hasShotUpgrade;
		currentPlayer.transform.position = loadedPlayer.position;


	}


	public void DeleteSave()
	{
		SaveGame.Delete("player");
	}
}
