using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
	[SerializeField] private TMP_Text _priceText;
	[SerializeField] private Image _itemImage; 
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetSlotValues(Sprite image, int price)
	{
		_priceText.text = price.ToString();
		_itemImage.sprite = image;
	}
}
