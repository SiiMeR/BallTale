using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Types;
using UnityEngine;

public struct PlayerData
{
    public Vector3Save position;
    public int currency;
    public Upgrade[] upgrades;
    
    public PlayerData(Player player)
    {
        position = player.transform.position;
        currency = player.Currency;
        upgrades = player.Upgrades.ToArray();
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

        var shopUpgrades = shop._saleQueue.ToList();

        foreach (var shopSlot in shop.Slots)
        {
            if (shopSlot.Upgrade != null)
            {
                shopUpgrades.Add(shopSlot.Upgrade);
            }
        }
        
        SaveGame.Save("shop.txt", shopUpgrades.ToArray());
    }

    public void LoadGame()
    {
        var loadedPlayer = SaveGame.Load<PlayerData>("player.txt");

        var currentPlayer = FindObjectOfType<Player>();

        currentPlayer.Currency = loadedPlayer.currency;
        currentPlayer.transform.position = loadedPlayer.position;
        
        loadedPlayer.upgrades.ToList().ForEach(upgrade => upgrade.Apply());
        var shop = FindObjectOfType<Shop>();

        var shopUpgrades = SaveGame.Load<Upgrade[]>("shop.txt");
      
        var instantiatedShopUpgrades = RecreateUpgrades(shopUpgrades);
        shop.RefillSlots(instantiatedShopUpgrades);
    }

    private List<Upgrade> RecreateUpgrades(IEnumerable<Upgrade> upgrades)
    {
       
        var instantiatedUpgrades = new List<Upgrade>();
        
        foreach (var upgrade in upgrades)
        {
            switch (upgrade) 
            {                
                    case HealthUpgrade healthUpgrade:
                        instantiatedUpgrades.Add(UpgradeBuilder.Instance.GetHealthUpgrade(healthUpgrade.HealthBonus,healthUpgrade.Price));
                        break;
                    
                    case ShootingUpgrade shootingUpgrade:
                        instantiatedUpgrades.Add(UpgradeBuilder.Instance.GetShootingUpgrade(shootingUpgrade.Price, shootingUpgrade.Description)); 
                        break;
                    
                    default:
                        Debug.Log($"Upgrade not defined in savegame loading script: {upgrade.GetType()}");
                        break;
                       
            }
        }
        
        return instantiatedUpgrades;
    }
}