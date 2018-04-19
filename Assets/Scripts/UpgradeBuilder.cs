﻿using System.Collections;
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
	
	public Upgrade GetHealthUpgrade(int health, int price)
	{
		GameObject huGo = Instantiate(_healthPrefab);

		HealthUpgrade healthUpgrade = huGo.GetComponent<HealthUpgrade>();
		
		healthUpgrade.HealthBonus = health;
		healthUpgrade.Price = price;
		healthUpgrade.Sprite = huGo.GetComponent<SpriteRenderer>().sprite;

		huGo.GetComponent<SpriteRenderer>().enabled = false;
		return healthUpgrade;
	}

}
