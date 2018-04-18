using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
	[SerializeField] private TMP_Text _priceText;
	[SerializeField] private Image _itemImage;

	public GameObject Upgrade;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetSlotValues(GameObject upgrade)
	{
		Upgrade = upgrade;
		_priceText.text = upgrade.GetComponent<HealthUpgrade>().Price.ToString();
		_itemImage.sprite = upgrade.GetComponent<SpriteRenderer>().sprite;
	}
}
