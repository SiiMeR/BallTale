using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : Upgrade
{

	public int HealthBonus { get; set; }
	// Use this for initialization
	void Start () {
		OnAquire.AddListener(AddHealth);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AddHealth()
	{
		var player = FindObjectOfType<Player>();
		player.CurrentHealth += HealthBonus;
		player.MaxHealth += HealthBonus;
		player.Currency -= Price;
	}
}
