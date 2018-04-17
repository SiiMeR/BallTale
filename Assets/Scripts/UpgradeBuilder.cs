using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBuilder : Singleton<UpgradeBuilder> {

	protected UpgradeBuilder() {} // no instantiation from other classes

	[SerializeField] private GameObject _healthPrefab;
	[SerializeField] private GameObject _upgradePrefab;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject GetHealthUpgrade(int health, int price)
	{
		GameObject healthUpgrade = Instantiate(_healthPrefab);

		healthUpgrade.GetComponent<HealthUpgrade>().HealthBonus = health;
		healthUpgrade.GetComponent<HealthUpgrade>().Price = price;

		return healthUpgrade;
	}

/*	GameObject GetUpgrade(string name, int price)
	{
		
	}
	*/
}
