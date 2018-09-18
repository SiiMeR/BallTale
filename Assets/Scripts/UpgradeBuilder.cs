using UnityEngine;

public class UpgradeBuilder : Singleton<UpgradeBuilder>
{
    [SerializeField] private GameObject _healthPrefab;
    [SerializeField] private GameObject _shotUpgradePrefab;
    
    protected UpgradeBuilder()
    {
    } // no instantiation from other classes


    public Upgrade GetHealthUpgrade(int health, int price)
    {
        var huGo = Instantiate(_healthPrefab);

        var healthUpgrade = huGo.GetComponent<HealthUpgrade>();
        
        healthUpgrade.HealthBonus = health;
        healthUpgrade.Price = price;
        healthUpgrade.Sprite = huGo.GetComponent<SpriteRenderer>().sprite;

        huGo.GetComponent<SpriteRenderer>().enabled = false;
        return healthUpgrade;
    }

    public Upgrade GetShotUpgrade(int price, string description)
    {
        var huGo = Instantiate(_shotUpgradePrefab);

        var skillUpgrade = huGo.GetComponent<ShootingUpgrade>();

        skillUpgrade.Price = price;
        skillUpgrade.Description = description;
        skillUpgrade.Sprite = huGo.GetComponent<SpriteRenderer>().sprite;

        huGo.GetComponent<SpriteRenderer>().enabled = false;
        return skillUpgrade;
    }
}