using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUpgrade : Upgrade {

	
	
	// Use this for initialization
	void Start () {
		OnAquire.AddListener(AddShootingAbility);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AddShootingAbility()
	{
		var player = FindObjectOfType<Player>().GetComponent<Player>();
		player.HasShotUpgrade = true;
		player.Currency -= Price;
	}
}
