using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : Interactable
{
	[SerializeField] private List<Upgrade> _itemsOnSale;
	
	
	// Use this for initialization
	protected  override void Start () {
		base.Start();
		
		_itemsOnSale = new List<Upgrade>();
		
		
//		_itemsOnSale.Add(new Upgrade());
//		titleText.SetText(Title);
	//	paragraphText.SetText(Text);
	}
	
	// Update is called once per frame	
	protected override void Update()
	{
		base.Update();
	}

	
}

