using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
	[SerializeField] private TMP_Text _priceText;
	[SerializeField] private Image _itemImage;


	private Upgrade _upgrade;
	
	public Upgrade Upgrade
	{
		get
		{
			return _upgrade;
		}
		set
		{
			_upgrade = value;

			if (value != null)
			{
				_priceText.text = value.Price.ToString();
				_itemImage.sprite = value.GetComponent<SpriteRenderer>().sprite;
				
				var color = _itemImage.color;
				color.a = 1;
				_itemImage.color = color;
			}
			else
			{
				_priceText.text = null;
				_itemImage.sprite = null;

				var color = _itemImage.color;
				color.a = 0;
				_itemImage.color = color;
			}

		}
	}

	public bool IsEmpty()
	{
		return Upgrade == null;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
