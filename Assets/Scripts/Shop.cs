using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : Interactable
{
	[SerializeField] private List<Upgrade> _itemsOnSale;

	[SerializeField] private List<GameObject> _slots;
	
	// Use this for initialization
	protected  override void Start () {
		base.Start();
		
	//	_itemsOnSale = new List<Upgrade>();
		
		
//		_itemsOnSale.Add(new Upgrade());
//		titleText.SetText(Title);
	//	paragraphText.SetText(Text);

		GameObject healthUpgrade = UpgradeBuilder.Instance.GetHealthUpgrade(10, 100);
		
		_slots[0].GetComponent<Slot>().SetSlotValues(
			healthUpgrade.GetComponent<SpriteRenderer>().sprite, 
			healthUpgrade.GetComponent<HealthUpgrade>().HealthBonus
			);
	}
	
	// Update is called once per frame	
	protected override void Update()
	{
		base.Update();
	}

	
}

