using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
	
	[SerializeField] private TextMeshProUGUI health;
	[SerializeField] private TextMeshProUGUI currency;
	[SerializeField] private GameObject deathScreen; // unused currently

	private Player player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		health.SetText("Health : " + player.CurrentHealth + " / " + player.MaxHealth);
		currency.SetText("Currency : " + player.Currency);
	}
}
