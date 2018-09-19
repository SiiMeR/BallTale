using System;
using System.Collections.Generic;
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
        if (PlayerPrefs.GetInt("loadgame") == 1) LoadGame();

        PlayerPrefs.SetInt("loadgame", 0);
    }


    public void CreateSaveGame()
    {
        SaveGame.Delete("shop.txt");
        
        var player = FindObjectOfType<Player>();

        var playerSave = new PlayerData(player);

        SaveGame.Save("player.txt", playerSave);

        var shop = FindObjectOfType<Shop>();

        var upgrades = shop._saleQueue.ToList();

        foreach (var shopSlot in shop.Slots)
        {
            if (shopSlot.Upgrade != null)
            {
                upgrades.Add(shopSlot.Upgrade);
            }
        }
      //  Debug.Log($"{upgrades.Count}");
        SaveGame.Save("shop.txt", upgrades.ToArray());
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

        var instantiatedUpgrades = RecreateUpgrades(upgrades);
        shop.RefillSlots(instantiatedUpgrades);
    }

    private List<Upgrade> RecreateUpgrades(IEnumerable<Upgrade> upgrades)
    {
        var realUpgrades = new List<Upgrade>();

        foreach (var upgrade in upgrades)
        {
            switch (upgrade.GetType().ToString()) 
            {                
                    case "HealthUpgrade":
                        var healthUp = (HealthUpgrade) upgrade;
                        realUpgrades.Add(UpgradeBuilder.Instance.GetHealthUpgrade(healthUp.HealthBonus,healthUp.Price));
                        break;
                    
                    case "ShootingUpgrade":
                        realUpgrades.Add(UpgradeBuilder.Instance.GetShotUpgrade(upgrade.Price, upgrade.Description)); 
                        break;
                    
                    default:
                        Debug.Log($"Upgrade not defined in loading script: {upgrade.GetType()}");
                        break;
                       
            }
        }
        
        return realUpgrades;
    }
}