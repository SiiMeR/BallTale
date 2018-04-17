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
		GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().MaxHealth += HealthBonus;
	}
}
