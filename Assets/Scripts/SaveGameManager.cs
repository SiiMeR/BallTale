using System.Linq;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Types;
using UnityEngine;

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
    public GameObject playerPrefab;

    private void Start()
    {
        //	PlayerPrefs.SetInt("loadgame",0);

        if (PlayerPrefs.GetInt("loadgame") == 1) LoadGame();

        PlayerPrefs.SetInt("loadgame", 0);
    }


    public void CreateSaveGame()
    {
        var player = FindObjectOfType<Player>();

        var playerSave = new PlayerData(player);

        SaveGame.Save("player.txt", playerSave);

        var shop = FindObjectOfType<Shop>();


        var upgrades = shop._saleQueue.ToArray();

        foreach (var shopSlot in shop._slots)
            if (shopSlot.Upgrade != null)
            {
                print(shopSlot.Upgrade);
                upgrades.Append(shopSlot.Upgrade);
            }

        SaveGame.Save("shop.txt", upgrades);
    }

    public void LoadGame()
    {
        var loadedPlayer = SaveGame.Load<PlayerData>("player.txt");

        var currentPlayer = FindObjectOfType<Player>();

        currentPlayer.Currency = loadedPlayer.currency;
        currentPlayer.CurrentHealth = loadedPlayer.currentHealth;
        currentPlayer.MaxHealth = loadedPlayer.maxHealth;
        currentPlayer.HasShotUpgrade = loadedPlayer.hasShotUpgrade;
        currentPlayer.transform.position = loadedPlayer.position;


        var shop = FindObjectOfType<Shop>();

        var upgrades = SaveGame.Load<Upgrade[]>("shop.txt");

        shop.RefillSlots(upgrades);
    }
}